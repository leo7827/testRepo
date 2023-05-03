using Mirle.Stocker.R46YP320.Events;
using Mirle.Stocker.R46YP320.Signal;

namespace Mirle.Stocker.R46YP320
{
    public class EQPort : IEQPort
    {
        #region IEQPort interface implementation

        public event StockerEvents.EQEventHandler OnCSTPresentChanged;

        public event StockerEvents.EQEventHandler OnCstidChanged;

        public event StockerEvents.EQEventHandler OnInServiceChanged;

        public event StockerEvents.EQEventHandler OnLoadRequestStatusChanged;

        public string CSTID => Signal.CSTID;
        public bool HasCarrier => Signal.Carrier.IsOn();
        public int Id { get; }
        public bool IsInService => Signal.POnline.IsOn();

        public bool IsReadyToDeposit
        {
            get
            {
                return IsInService && Signal.U_REQ.IsOff() && Signal.L_REQ.IsOn();
            }
        }

        public bool IsReadyToRetrieve
        {
            get
            {
                return IsInService && Signal.U_REQ.IsOn() && Signal.L_REQ.IsOff();
            }
        }
        public bool IsReady => Signal.Ready.IsOn();

        public StockerEnums.PortLoadRequestStatus LoadRequestStatus
        {
            get
            {
                if(Signal.L_REQ.IsOn())
                    return StockerEnums.PortLoadRequestStatus.Load;
                else if(Signal.U_REQ.IsOn())
                    return StockerEnums.PortLoadRequestStatus.Unload;
                else
                    return StockerEnums.PortLoadRequestStatus.None;
            }
        }

        public StockerEnums.EQPortStatus Status
        {
            get
            {
                if(Signal.PEStop.IsOn()) { return StockerEnums.EQPortStatus.ERROR; }
                else if(Signal.POnline.IsOn()) { return StockerEnums.EQPortStatus.NORMAL; }
                else { return StockerEnums.EQPortStatus.NONE; }
            }
        }

        #endregion IEQPort interface implementation

        private bool _lastCSTPresentIsOn = false;
        private bool _lastCSTPresentIsOff = false;
        private bool _lastInServiceIsOn = false;
        private string _lastCstID = "";
        private StockerEnums.PortLoadRequestStatus _lastLoadRequestStatus = StockerEnums.PortLoadRequestStatus.None;
        private bool _lastPriorityUpIsOn = false;

        public EQPort(EQPortSignal eqPortSignal)
        {
            Id = eqPortSignal.Id;
            Signal = eqPortSignal;
        }

        public event StockerEvents.EQEventHandler OnPriorityUpChanged;

        public EQPortSignal Signal { get; }

        public void InitialStatus()
        {
            if(Signal.POnline.IsOn())
            {
                _lastInServiceIsOn = true;
                _lastCSTPresentIsOn = Signal.Carrier.IsOn();
                _lastPriorityUpIsOn = Signal.PriorityUp.IsOn();
            }
        }

        public void RefreshStatus()
        {
            var newInServiceIsOn = Signal.POnline.IsOn();
            if(_lastInServiceIsOn != newInServiceIsOn)
            {
                _lastInServiceIsOn = newInServiceIsOn;
                var args = new EQEventArgs(Id) { SignalIsOn = newInServiceIsOn };
                OnInServiceChanged?.Invoke(this, args);
            }

            var newCSTPresentIsOn = Signal.POnline.IsOn() && Signal.Carrier.IsOn();
            if(_lastCSTPresentIsOn != newCSTPresentIsOn && newCSTPresentIsOn)
            {
                var args = new EQEventArgs(Id) { SignalIsOn = true };
                OnCSTPresentChanged?.Invoke(this, args);
            }
            _lastCSTPresentIsOn = newCSTPresentIsOn;
            var newCSTPresentIsOff = Signal.Carrier.IsOff();
            if (_lastCSTPresentIsOff != newCSTPresentIsOff && newCSTPresentIsOff)
            {
                var args = new EQEventArgs(Id) { SignalIsOn = false };
                OnCSTPresentChanged?.Invoke(this, args);
            }
            _lastCSTPresentIsOff = newCSTPresentIsOff;

            var newCstID = Signal.CSTID;
            if(_lastCstID != newCstID)
            {
                var args = new EQEventArgs(Id) { CstId = newCstID };
                OnCstidChanged?.Invoke(this, args);
                _lastCstID = newCstID;
            }

            var newPriorityUpIsOn = Signal.POnline.IsOn() && Signal.PriorityUp.IsOn();
            if(_lastPriorityUpIsOn != newPriorityUpIsOn)
            {
                _lastPriorityUpIsOn = newPriorityUpIsOn;
                var args = new EQEventArgs(Id) { SignalIsOn = newPriorityUpIsOn };
                OnPriorityUpChanged?.Invoke(this, args);
            }

            var newLoadRequestStatus = this.LoadRequestStatus;
            if(_lastLoadRequestStatus != newLoadRequestStatus)
            {
                var args = new EQEventArgs(Id) { NewLoadRequestStatus = newLoadRequestStatus };
                OnLoadRequestStatusChanged?.Invoke(this, args);

                _lastLoadRequestStatus = newLoadRequestStatus;
            }
        }
    }
}
