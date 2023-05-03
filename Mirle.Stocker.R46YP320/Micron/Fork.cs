using Mirle.Stocker.R46YP320.Events;
using Mirle.Stocker.R46YP320.Signal;
using System.Threading.Tasks;
using Mirle.Structure.Info;
using Mirle.Structure;
using Mirle.Extensions;

namespace Mirle.Stocker.R46YP320
{
    public class Fork : IFork
    {
        #region IFork interface implementation

        public event StockerEvents.ForkEventHandler OnCSTPresenceChanged;

        public event StockerEvents.ForkEventHandler OnCSTReadReportOn;

        public string BCRResult => Signal.BCRResultCstId.GetData().ToASCII();
        public string CurrentCommand => Signal.CurrentCommand.GetValue().ToString("D5");
        public string CompleteCommand => Signal.CompletedCommand.GetValue().ToString("D5");
        public bool HasCarrier => Signal.CSTPresence.IsOn();
        public int Id { get; }

        public TransferBatch GetCommand()
        {
            return cmd;
        }

        public ForkSetupInfo GetConfig()
        {
            return forkConfig;
        }
        #endregion IFork interface implementation

        private readonly Crane _crane;
        private string _lastBCRResult = string.Empty;
        private int _lastCompletedCommand = 0;
        private bool _lastCSTPresenceIsOn = false;
        private int _lastCurrentCommand = 0;
        private bool _lastCycle1IsOn = false;
        private bool _lastCycle2IsOn = false;
        private bool _lastForking1IsOn = false;
        private bool _lastForking2IsOn = false;
        private bool _lastIdleIsOn = false;

        private TransferBatch cmd = new TransferBatch();
        private ForkSetupInfo forkConfig = new ForkSetupInfo();

        public Fork(int id, ForkSignal signal, Crane crane)
        {
            _crane = crane;
            Id = id;
            Signal = signal;
            forkConfig.Limit = new ForkLimitInfo();
        }

        public event StockerEvents.ForkEventHandler OnActive;

        public event StockerEvents.ForkEventHandler OnCompletedCommandChanged;

        public event StockerEvents.ForkEventHandler OnCurrentCommandChanged;

        public event StockerEvents.ForkEventHandler OnCycle1;

        public event StockerEvents.ForkEventHandler OnCycle2;

        public event StockerEvents.ForkEventHandler OnForking1;

        public event StockerEvents.ForkEventHandler OnForking2;

        public event StockerEvents.ForkEventHandler OnIdle;

        public ForkSignal Signal { get; }

        public string TrackingId => Signal.TrackingCstId.GetData().ToASCII();

        public void InitialStatus()
        {
            _lastIdleIsOn = Signal.Idle.IsOn();
            _lastCycle1IsOn = Signal.Cycle1.IsOn();
            _lastCycle2IsOn = Signal.Cycle2.IsOn();
            _lastForking1IsOn = Signal.Forking1.IsOn();
            _lastForking2IsOn = Signal.Forking2.IsOn();
            _lastBCRResult = Signal.BCRResultCstId.GetData().ToASCII();
            _lastCurrentCommand = Signal.CurrentCommand.GetValue();
            _lastCompletedCommand = Signal.CompletedCommand.GetValue();
        }

        public void RefreshStatus()
        {
            var newIdleIsOn = Signal.Idle.IsOn();
            if (_lastIdleIsOn == false && newIdleIsOn)
            {
                var currentCommandId = Signal.CurrentCommand.GetValue().ToString("D5");
                var args = new ForkEventArgs(Id, _crane.Id) { CurrentCommandId = currentCommandId };
                OnIdle?.Invoke(this, args);
            }
            else if (_lastIdleIsOn && newIdleIsOn == false)
            {
                var currentCommandId = Signal.CurrentCommand.GetValue().ToString("D5");
                var args = new ForkEventArgs(Id, _crane.Id) { CurrentCommandId = currentCommandId };
                OnActive?.Invoke(this, args);
            }
            _lastIdleIsOn = newIdleIsOn;

            var newCycle1IsOn = Signal.Cycle1.IsOn();
            if (_lastCycle1IsOn == false && newCycle1IsOn)
            {
                var currentCommandId = Signal.CurrentCommand.GetValue().ToString("D5");
                var args = new ForkEventArgs(Id, _crane.Id) { CurrentCommandId = currentCommandId };
                OnCycle1?.Invoke(this, args);
            }
            _lastCycle1IsOn = newCycle1IsOn;

            var newCycle2IsOn = Signal.Cycle2.IsOn();
            if (_lastCycle2IsOn == false && newCycle2IsOn)
            {
                var currentCommandId = Signal.CurrentCommand.GetValue().ToString("D5");
                var args = new ForkEventArgs(Id, _crane.Id) { CurrentCommandId = currentCommandId };
                OnCycle2?.Invoke(this, args);
            }
            _lastCycle2IsOn = newCycle2IsOn;

            var newForking1IsOn = Signal.Forking1.IsOn();
            if (_lastForking1IsOn == false && newForking1IsOn)
            {
                var currentCommandId = Signal.CurrentCommand.GetValue().ToString("D5");
                var args = new ForkEventArgs(Id, _crane.Id) { CurrentCommandId = currentCommandId };
                OnForking1?.Invoke(this, args);
            }
            _lastForking1IsOn = newForking1IsOn;

            var newForing2IsOn = Signal.Forking2.IsOn();
            if (_lastForking2IsOn == false && newForing2IsOn)
            {
                var currentCommandId = Signal.CurrentCommand.GetValue().ToString("D5");
                var args = new ForkEventArgs(Id, _crane.Id) { CurrentCommandId = currentCommandId };
                OnForking2?.Invoke(this, args);
            }
            _lastForking2IsOn = newForing2IsOn;

            var newCSTPresenceIsOn = Signal.CSTPresence.IsOn();
            if (_lastCSTPresenceIsOn != newCSTPresenceIsOn)
            {
                _lastCSTPresenceIsOn = newCSTPresenceIsOn;
                var currentCommandId = Signal.CurrentCommand.GetValue().ToString("D5");
                var args = new ForkEventArgs(Id, _crane.Id) { SignalIsOn = newCSTPresenceIsOn, CurrentCommandId = currentCommandId };
                OnCSTPresenceChanged?.Invoke(this, args);
            }

            var newBCRResult = Signal.BCRResultCstId.GetData().ToASCII();
            if (_lastBCRResult != newBCRResult && string.IsNullOrEmpty(newBCRResult) == false)
            {
                var currentCommandId = Signal.CurrentCommand.GetValue().ToString("D5");
                var args = new ForkEventArgs(Id, _crane.Id) { BCRResult = newBCRResult, CurrentCommandId = currentCommandId };
                OnCSTReadReportOn?.Invoke(this, args);
            }
            _lastBCRResult = newBCRResult;

            var newCurrentCommand = Signal.CurrentCommand.GetValue();
            if (_lastCurrentCommand != newCurrentCommand && newCurrentCommand != 0)
            {
                var currentCommandId = Signal.CurrentCommand.GetValue().ToString("D5");
                var args = new ForkEventArgs(Id, _crane.Id) { CurrentCommandId = currentCommandId };
                OnCurrentCommandChanged?.Invoke(this, args);
            }
            _lastCurrentCommand = newCurrentCommand;

            var newCompletedCommand = Signal.CompletedCommand.GetValue();
            if (_lastCompletedCommand != newCompletedCommand && newCompletedCommand != 0)
            {
                var completedCommandId = Signal.CompletedCommand.GetValue().ToString("D5");
                Task.Delay(100).Wait();
                var completedCode = Signal.CompletedCode.GetValue().ToString("X2");
                var args = new ForkEventArgs(Id, _crane.Id)
                {
                    CompletedCommandId = completedCommandId,
                    CompletedCode = completedCode,
                    BCRResult = Signal.BCRResultCstId.GetData().ToASCII(),
                };
                OnCompletedCommandChanged?.Invoke(this, args);
            }
            _lastCompletedCommand = newCompletedCommand;
        }

        public Task Enable(bool enable)
        {
            return Task.Run(() =>
            {
                Signal.ForkDisable.Set(enable);
            });
        }
    }
}
