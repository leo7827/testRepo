using Mirle.Stocker.R46YP320.Events;
using Mirle.Stocker.R46YP320.Signal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mirle.Stocker.R46YP320
{
    public class IOPort : IIOPort
    {
        #region IIOPort interface implementation

        public event StockerEvents.IOEventHandler OnCSTRemoved;

        public event StockerEvents.IOEventHandler OnCSTWaitIn;

        public event StockerEvents.IOEventHandler OnCSTWaitOut;

        public event StockerEvents.IOEventHandler OnDirectionChanged;

        public event StockerEvents.IOEventHandler OnInServiceChanged;

        public StockerEnums.IOPortDirection Direction
        {
            get
            {
                if (Signal.InMode.IsOn() && Signal.OutMode.IsOff())
                {
                    return StockerEnums.IOPortDirection.InMode;
                }
                else if (Signal.InMode.IsOff() && Signal.OutMode.IsOn())
                {
                    return StockerEnums.IOPortDirection.OutMode;
                }
                else
                {
                    return StockerEnums.IOPortDirection.ModeChanging;
                }
            }
        }

        public int Id { get; }

        public bool IsCstWainIn
        {
            get
            {
                _stages.TryGetValue(1, out var stage1);
                return Signal.Run.IsOn() && Direction == StockerEnums.IOPortDirection.InMode
                       && Signal.WaitIn.IsOn() && Signal.UnloadOK.IsOn() && stage1.Signal.LoadPresence.IsOn();
            }
        }

        public bool IsInService => Signal.Run.IsOn();
        public bool IsPortModeChangeable => Signal.PortModeChangeable.IsOn() && _nextCanPortModeChangeTime < DateTime.Now;

        public StockerEnums.PortLoadRequestStatus LoadRequestStatus
        {
            get
            {
                if (Signal.Run.IsOn() && Signal.LoadOK.IsOn() != Signal.UnloadOK.IsOn())
                {
                    if (Signal.LoadOK.IsOn())
                    {
                        return StockerEnums.PortLoadRequestStatus.Load;
                    }

                    if (Signal.UnloadOK.IsOn())
                    {
                        return StockerEnums.PortLoadRequestStatus.Unload;
                    }
                }

                return StockerEnums.PortLoadRequestStatus.None;
            }
        }

        public IEnumerable<IIOStage> Stages => _stages.Values;

        public StockerEnums.IOPortStatus Status
        {
            get
            {
                if (Signal.Fault.IsOn()) { return StockerEnums.IOPortStatus.ERROR; }
                else if (Signal.Run.IsOn()) { return StockerEnums.IOPortStatus.NORMAL; }
                else { return StockerEnums.IOPortStatus.NONE; }
            }
        }

        public IEnumerable<IIOVehicle> Vehicles => _vehicles.Values;

        public IIOStage GetStageById(int id)
        {
            _stages.TryGetValue(id, out var stage);
            return stage;
        }

        public IIOVehicle GetVehicleById(int id)
        {
            _vehicles.TryGetValue(id, out var vehicle);
            return vehicle;
        }

        public bool HasCarrierOf(string cstid)
        {
            return _stages.Values.Any(s => s.CstId == cstid) || _vehicles.Values.Any(v => v.CstId == cstid);
        }
        public bool IsReadyFromCraneSide => Signal.Ready_CraneSide.IsOn();

        #endregion IIOPort interface implementation

        private readonly Dictionary<int, IOStage> _stages = new Dictionary<int, IOStage>();
        private readonly Dictionary<int, IOVehicle> _vehicles = new Dictionary<int, IOVehicle>();
        private List<AlarmInfo> _alarms = new List<AlarmInfo>();
        private bool _canAutoSetRun = false;
        private int _lastAlarmIndex = 0;
        private bool _lastAutoManualModeIsAuto = false;
        private bool _lastBCRReadDoneIsOn = false;
        private bool _lastCSTRemoveIsOn = false;
        private bool _lastCSTWaitInIsOn = false;
        private bool _lastCSTWaitOutIsOn = false;
        private StockerEnums.IOPortDirection _lastDirection = StockerEnums.IOPortDirection.None;
        private bool _lastInServiceIsOn = false;
        private StockerEnums.PortLoadRequestStatus _lastLoadRequestStatus = StockerEnums.PortLoadRequestStatus.None;
        private bool _lastRunEnableIsOn = false;
        private DateTime _nextCanAutoSetRunTime = DateTime.Now;
        private DateTime _nextCanPortModeChangeTime = DateTime.Now;

        public IOPort(IOPortSignal ioPortSignal)
        {
            Id = ioPortSignal.Id;
            Signal = ioPortSignal;

            foreach (var vehicle in Signal.Vehicles)
            {
                _vehicles.Add(vehicle.Id, new IOVehicle(vehicle.Id, vehicle, this));
            }
            foreach (var stage in Signal.Stages)
            {
                _stages.Add(stage.Id, new IOStage(stage.Id, stage, this));
            }
        }

        public event StockerEvents.AlarmEventHandler OnAlarmCleared;

        public event StockerEvents.AlarmEventHandler OnAlarmIndexChanged;

        public event StockerEvents.IOEventHandler OnAutoManualModeChanged;

        public event StockerEvents.IOEventHandler OnBCRReadDone;

        public event StockerEvents.IOEventHandler OnLoadRequestStatusChanged;

        public bool CanAutoSetRun
        {
            get => _canAutoSetRun && _nextCanAutoSetRunTime < DateTime.Now;
            set => _canAutoSetRun = value;
        }

        public bool IsReadyToDeposit
        {
            get
            {
                return Signal.Run.IsOn() && Signal.Fault.IsOff()
                       && Direction == StockerEnums.IOPortDirection.OutMode
                       && Signal.UnloadOK.IsOff() && Signal.LoadOK.IsOn();
            }
        }

        public bool IsReadyToRetrieve
        {
            get
            {
                return Signal.Run.IsOn() && Signal.Fault.IsOff()
                       && Direction == StockerEnums.IOPortDirection.InMode
                       && Signal.UnloadOK.IsOn() && Signal.LoadOK.IsOff();
            }
        }

        public bool IsAlarm => Signal.Fault.IsOn() && Signal.ErrorCode.GetValue() != 0;

        public DateTime NextCanAutoSetRunTime => _nextCanAutoSetRunTime;
        public IOPortSignal Signal { get; }

        public void InitialStatus()
        {
            if (Signal.Run.IsOn())
            {
                _lastInServiceIsOn = true;
            }
            _lastAutoManualModeIsAuto = Signal.AutoManualMode.IsOn();
            _lastBCRReadDoneIsOn = Signal.BCRReadDone.IsOn();
            _lastCSTWaitInIsOn = Signal.WaitIn.IsOn();
            _lastCSTWaitOutIsOn = Signal.WaitOut.IsOn();
            _lastCSTRemoveIsOn = Signal.CSTRemoveCheck_Req.IsOn();
            _lastDirection = Direction;
        }

        public void RefreshStatus()
        {
            var newInServiceIsOn = Signal.Run.IsOn();
            if (_lastInServiceIsOn != newInServiceIsOn)
            {
                _lastInServiceIsOn = newInServiceIsOn;
                var args = new IOEventArgs(Id) { SignalIsOn = newInServiceIsOn };
                OnInServiceChanged?.Invoke(this, args);
            }

            var newAutoManualModeIsAuto = Signal.AutoManualMode.IsOn();
            if (_lastAutoManualModeIsAuto != newAutoManualModeIsAuto)
            {
                _lastAutoManualModeIsAuto = newAutoManualModeIsAuto;
                var args = new IOEventArgs(Id) { SignalIsOn = newAutoManualModeIsAuto };
                OnAutoManualModeChanged?.Invoke(this, args);
            }

            var newLoadRequestStatus = this.LoadRequestStatus;
            if (_lastLoadRequestStatus != newLoadRequestStatus)
            {
                _lastLoadRequestStatus = newLoadRequestStatus;
                var args = new IOEventArgs(Id) { NewLoadRequestStatus = newLoadRequestStatus };
                OnLoadRequestStatusChanged?.Invoke(this, args);
            }

            var newBCRReadDoneIson = Signal.BCRReadDone.IsOn();
            if (_lastBCRReadDoneIsOn == false && newBCRReadDoneIson)
            {
                Task.Delay(1000).ContinueWith(task =>
                {
                    var args = new IOEventArgs(Id) { CstId = Signal.CSTID_BarcodeResultOnP1 };
                    OnBCRReadDone?.Invoke(this, args);
                });
            }
            _lastBCRReadDoneIsOn = newBCRReadDoneIson;

            _stages.TryGetValue(1, out var stage1);
            var newCSTWaitInIsOn = Signal.WaitIn.IsOn();
            if (_lastCSTWaitInIsOn == false && newCSTWaitInIsOn && stage1.Signal.LoadPresence.IsOn())
            {
                var args = new IOEventArgs(Id) { CstId = stage1.CstId };
                OnCSTWaitIn?.Invoke(this, args);
            }
            _lastCSTWaitInIsOn = newCSTWaitInIsOn;

            var newCSTWaitOutIsOn = Signal.WaitOut.IsOn();
            if (_lastCSTWaitOutIsOn == false && newCSTWaitOutIsOn)
            {
                var args = new IOEventArgs(Id) { };
                OnCSTWaitOut?.Invoke(this, args);
            }
            _lastCSTWaitOutIsOn = newCSTWaitOutIsOn;

            var newCSTRemoveIsOn = Signal.CSTRemoveCheck_Req.IsOn();
            if (_lastCSTRemoveIsOn == false && newCSTRemoveIsOn)
            {
                var args = new IOEventArgs(Id) { };
                OnCSTRemoved?.Invoke(this, args);
            }
            _lastCSTRemoveIsOn = newCSTRemoveIsOn;

            var newDirection = Direction;
            if (_lastDirection != newDirection)
            {
                _lastDirection = newDirection;
                if (newDirection == StockerEnums.IOPortDirection.InMode
                    || newDirection == StockerEnums.IOPortDirection.OutMode)
                {
                    var args = new IOEventArgs(Id) { NewDirection = newDirection };
                    OnDirectionChanged?.Invoke(this, args);
                }
            }

            var newRunEnableIsOn = Signal.RunEnable.IsOn();
            if (_lastRunEnableIsOn == false && newRunEnableIsOn)
            {
                _nextCanAutoSetRunTime = DateTime.Now.AddSeconds(60);
            }
            else if (newRunEnableIsOn == false)
            {
                _nextCanAutoSetRunTime = DateTime.MaxValue;
            }
            _lastRunEnableIsOn = newRunEnableIsOn;

            var alarmIndex = Signal.ErrorIndex.GetValue();
            var pcAlarmIndex = Signal.Controller.PcErrorIndex.GetValue();
            if (alarmIndex != pcAlarmIndex)
            {
                //Have New Alarm
                if (_lastAlarmIndex != alarmIndex && (alarmIndex > pcAlarmIndex || alarmIndex == 1))
                {
                    var alarmCode = Signal.ErrorCode.GetValue();
                    if (alarmCode != 0)
                    {
                        _lastAlarmIndex = alarmIndex;
                        _alarms.Add(new AlarmInfo() { AlarmIndex = alarmIndex, AlarmCode = alarmCode });
                        OnAlarmIndexChanged?.Invoke(this, new AlarmEventArgs(alarmIndex, alarmCode));
                    }
                }

                Signal.Controller.PcErrorIndex.SetValue(alarmIndex);
            }
            else if (_alarms.Any() && Signal.Fault.IsOff() && Signal.ErrorCode.GetValue() == 0)
            {
                //Alarm Reset
                OnAlarmCleared?.Invoke(this, new AlarmEventArgs(0, 0));
                _alarms.Clear();
            }

            if (Signal.Run.IsOn() && Signal.Controller.Run.IsOn())
            {
                Signal.Controller.Run.SetOff();
            }
            if (Signal.Run.IsOff() && Signal.Controller.Stop.IsOn())
            {
                Signal.Controller.Stop.SetOff();
            }
        }

        public Task ClearControlBitsAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.Controller.Word1.SetValue(0);
                    Signal.Controller.Word3.SetValue(0);
                    Signal.Controller.Word4.SetValue(0);
                }
                catch (Exception ex)
                {
                }
            });
        }

        public Task RequestBCRReadAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.IDReadCommand.SetOn();
                Task.Delay(500).Wait();
                Signal.Controller.IDReadCommand.SetOff();
            });
        }

        public Task RequestBuzzerStopAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.BuzzerStop.SetOn();
                Task.Delay(500).Wait();
                Signal.Controller.BuzzerStop.SetOff();
            });
        }

        public Task RequestDisableFBCRAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.BCRDisable_P1.SetOn();
            });
        }

        public Task RequestEnableFBCRAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.BCRDisable_P1.SetOff();
            });
        }

        public Task RequestFaultResetAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.FaultReset.SetOn();
                Task.Delay(500).Wait();
                Signal.Controller.FaultReset.SetOff();
            });
        }

        public Task RequestInModeAsync()
        {
            return Task.Run(() =>
            {
                if (Direction == StockerEnums.IOPortDirection.OutMode
                    && IsPortModeChangeable)
                {
                    _nextCanPortModeChangeTime = DateTime.Now.AddSeconds(6);

                    Signal.Controller.RequestOutputMode.SetOff();

                    Signal.Controller.RequestInputMode.SetOn();
                    Task.Delay(5_000).Wait();
                    Signal.Controller.RequestInputMode.SetOff();
                }
            });
        }

        public Task RequestMoveBackMGVAsync()
        {
            return Task.Run(() =>
            {
                if (Direction == StockerEnums.IOPortDirection.InMode
                    && Signal.GetStageSignalById(1).LoadPresence.IsOn())
                {
                    Signal.Controller.MoveBack.SetOn();
                    Task.Delay(500).Wait();
                    Signal.Controller.MoveBack.SetOff();
                }
            });
        }

        public Task RequestOpenDoorOHSAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.DoorOpenOHS.SetOn();
                Task.Delay(1100).Wait();
                Signal.Controller.DoorOpenOHS.SetOff();
            });
        }

        public Task RequestOutModeAsync()
        {
            return Task.Run(() =>
            {
                if (Direction == StockerEnums.IOPortDirection.InMode
                    && IsPortModeChangeable)
                {
                    _nextCanPortModeChangeTime = DateTime.Now.AddSeconds(6);

                    Signal.Controller.RequestInputMode.SetOff();

                    Signal.Controller.RequestOutputMode.SetOn();
                    Task.Delay(5_000).Wait();
                    Signal.Controller.RequestOutputMode.SetOff();
                }
            });
        }

        public Task RequestRunAsync()
        {
            return Task.Run(() =>
            {
                if (Signal.RunEnable.IsOn() && Signal.Run.IsOff() && Signal.Controller.Run.IsOff())
                {
                    Signal.Controller.Stop.SetOff();

                    Signal.Controller.Run.SetOn();
                    Task.Delay(1000).Wait();
                    Signal.Controller.Run.SetOff();
                }
            });
        }

        public Task RequestStopAsync()
        {
            return Task.Run(() =>
            {
                if (Signal.Run.IsOn() && Signal.Controller.Stop.IsOff())
                {
                    Signal.Controller.Run.SetOff();

                    Signal.Controller.Stop.SetOn();
                    Task.Delay(1000).Wait();
                    Signal.Controller.Stop.SetOff();
                }
            });
        }

        private class AlarmInfo
        {
            public int AlarmCode { get; set; }
            public int AlarmIndex { get; set; }
        }
    }
}
