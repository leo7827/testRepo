using Mirle.Extensions;
using Mirle.Stocker.R46YP320.Events;
using Mirle.Stocker.R46YP320.Signal;
using System.Threading.Tasks;

namespace Mirle.Stocker.R46YP320
{
    public class IOVehicle : IIOVehicle
    {
        #region IIOVehicle interface implementation

        public event StockerEvents.IOVehicleEventHandler OnCstidChanged;

        public event StockerEvents.IOVehicleEventHandler OnLoadPresenceChanged;

        public string CstId => Signal.CarrierId.GetData().ToASCII();
        public bool HasCarrier => Signal.LoadPresence.IsOn();
        public int Id { get; }

        #endregion IIOVehicle interface implementation

        private readonly IOPort _ioPort;
        private string _lastCstid = string.Empty;
        private bool _lastLoadPresenceIsOn = false;
        private StockerEnums.IOPortVehicleStatus _lastStatus = StockerEnums.IOPortVehicleStatus.NONE;

        public IOVehicle(int id, IOVehicleSignal ioVehicleSignal, IOPort ioPort)
        {
            _ioPort = ioPort;
            Id = id;
            Signal = ioVehicleSignal;
        }

        public event StockerEvents.IOVehicleEventHandler OnStatusChanged;

        public IOVehicleSignal Signal { get; }

        public StockerEnums.IOPortVehicleStatus Status
        {
            get
            {
                if (Signal.Error.IsOn())
                {
                    return StockerEnums.IOPortVehicleStatus.ERROR;
                }
                else if (Signal.HPReturn.IsOn())
                {
                    return StockerEnums.IOPortVehicleStatus.HOMEACTION;
                }
                else if (Signal.Active.IsOn())
                {
                    return StockerEnums.IOPortVehicleStatus.ACTIVE;
                }
                else if (Signal.Active.IsOff())
                {
                    return StockerEnums.IOPortVehicleStatus.IDLE;
                }
                else
                {
                    return StockerEnums.IOPortVehicleStatus.NONE;
                }
            }
        }

        public void InitialStatus()
        {
            _lastLoadPresenceIsOn = Signal.LoadPresence.IsOn();
            _lastStatus = this.Status;
            _lastCstid = CstId;
        }

        public void RefreshStatus()
        {
            var newLoadPresence = Signal.LoadPresence.IsOn();
            if (_lastLoadPresenceIsOn != newLoadPresence)
            {
                _lastLoadPresenceIsOn = newLoadPresence;
                var args = new IOVehicleEventArgs(vehicleId: this.Id, ioPortId: _ioPort.Id) { SignalIsOn = newLoadPresence, CstId = CstId };
                OnLoadPresenceChanged?.Invoke(this, args);
            }

            var newStatus = this.Status;
            if (_lastStatus != newStatus)
            {
                _lastStatus = newStatus;
                var args = new IOVehicleEventArgs(vehicleId: this.Id, ioPortId: _ioPort.Id) { NewStatus = newStatus };
                OnStatusChanged?.Invoke(this, args);
            }

            var newCstid = CstId;
            if (_lastCstid != newCstid)
            {
                _lastCstid = newCstid;
                var args = new IOVehicleEventArgs(vehicleId: this.Id, ioPortId: _ioPort.Id) { CstId = CstId };
                OnCstidChanged?.Invoke(this, args);
            }
        }

        public Task RequestFaultResetAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.ErrorReset.SetOn();
                Task.Delay(500).Wait();
                Signal.Controller.ErrorReset.SetOff();
            });
        }

        public Task RequestReturnHomeAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.HomeReturn.SetOn();
                Task.Delay(500).Wait();
                Signal.Controller.HomeReturn.SetOff();
            });
        }

        public Task RequestRunAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.Run.SetOn();
                Task.Delay(500).Wait();
                Signal.Controller.Run.SetOff();
            });
        }
    }
}