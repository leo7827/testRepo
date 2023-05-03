using Mirle.Extensions;
using Mirle.Stocker.R46YP320.Events;
using Mirle.Stocker.R46YP320.Signal;

namespace Mirle.Stocker.R46YP320
{
    public class IOStage : IIOStage
    {
        #region IIOStage interface implementation

        public event StockerEvents.IOStageEventHandler OnCstidChanged;

        public event StockerEvents.IOStageEventHandler OnLoadPresenceChanged;

        public string CstId => Signal.CarrierId.GetData().ToASCII();
        public bool HasCarrier => Signal.LoadPresence.IsOn();
        public int Id { get; }

        #endregion IIOStage interface implementation

        private readonly IOPort _ioPort;
        private string _lastCstid = string.Empty;
        private bool _lastLoadPresenceIsOn = false;

        public IOStage(int id, IOStageSignal ioStageSignal, IOPort ioPort)
        {
            _ioPort = ioPort;
            Id = id;
            Signal = ioStageSignal;
        }

        public IOStageSignal Signal { get; }

        public void InitialStatus()
        {
            _lastLoadPresenceIsOn = Signal.LoadPresence.IsOn();
            _lastCstid = CstId;
        }

        public void RefreshStatus()
        {
            var newLoadPresence = Signal.LoadPresence.IsOn();
            if (_lastLoadPresenceIsOn != newLoadPresence)
            {
                _lastLoadPresenceIsOn = newLoadPresence;
                var args = new IOStageEventArgs(stageId: this.Id, ioPortId: _ioPort.Id) { SignalIsOn = newLoadPresence, CstId = CstId };
                OnLoadPresenceChanged?.Invoke(this, args);
            }

            var newCstid = CstId;
            if (_lastCstid != newCstid)
            {
                _lastCstid = newCstid;
                var args = new IOStageEventArgs(this.Id, _ioPort.Id) { CstId = newCstid };
                OnCstidChanged?.Invoke(this, args);
            }
        }
    }
}