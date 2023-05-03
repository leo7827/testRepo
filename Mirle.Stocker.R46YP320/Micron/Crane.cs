using Mirle.Extensions;
using Mirle.Stocker.R46YP320.Events;
using Mirle.Stocker.R46YP320.Signal;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mirle.Stocker.R46YP320
{
    public class Crane : ICrane
    {
        #region ICrane interface implementation

        public event StockerEvents.CraneEventHandler OnAvailStatusChanged;

        public event StockerEvents.CraneEventHandler OnLocationUpdated;

        public event StockerEvents.CraneEventHandler OnStatusChanged;

        public event StockerEvents.CraneEventHandler OnTaskCmdWriteToMPLC;

        public int CurrentBank => Signal.ForkAtBank1.IsOn() ? 1 : 2;
        public int CurrentBay => Signal.CurrentBay.GetValue();
        public int CurrentLevel => Signal.CurrentLevel.GetValue();
        public int CurrentPosition => Signal.CurrentPosition.GetValue();
        public IEnumerable<IFork> Forks => _forks.Values;
        public int Id { get; }
        public bool IsExecutingHPReturn => Signal.HPReturn.IsOn();

        public bool IsHandOffReserved => Signal.Dual_HandOffReserved.IsOn();

        public bool IsInterventionEntry => Signal.Dual_InterventionEntry.IsOn();

        public bool IsIdle => Signal.Idle.IsOn();

        public bool IsInService => Signal.InService.IsOn();

        public bool IsKeySwitchIsAuto => Signal.SRI.TheAMSwitchIsAuto_RM.IsOn();

        public bool IsSingleCraneMode => Signal.SingleCraneMode.IsOn();

        public bool IsAlarm => Signal.Error.IsOn() || Signal.ErrorCode.GetValue() != 0;

        public string Location => Signal.Location.GetValue().ToString("D5");

        public bool TwinForkIsLoad => GetLeftFork().HasCarrier && GetRightFork().HasCarrier;

        public StockerEnums.CraneStatus Status
        {
            get
            {
                if (Signal.HPReturn.IsOn())
                {
                    return StockerEnums.CraneStatus.HOMEACTION;
                }
                else if (Signal.Error.IsOn())
                {
                    return StockerEnums.CraneStatus.STOP;
                }
                else if (Signal.InService.IsOff())
                {
                    return StockerEnums.CraneStatus.MAINTAIN;
                }
                else if (Signal.InService.IsOn() && Signal.Run.IsOn() && Signal.Idle.IsOn() && Signal.Active.IsOff() &&
                         Signal.Escape.IsOff())
                {
                    return StockerEnums.CraneStatus.IDLE;
                }
                else if (Signal.InService.IsOn() && Signal.Run.IsOn() && Signal.Idle.IsOff() && Signal.Active.IsOn() && Signal.Escape.IsOff())
                {
                    return Signal.Dual_InterferenceWaiting.IsOn() ? StockerEnums.CraneStatus.Waiting : StockerEnums.CraneStatus.BUSY;
                }
                else if (Signal.InService.IsOn() && Signal.Run.IsOn() && Signal.Idle.IsOff() && Signal.Escape.IsOn())
                {
                    return StockerEnums.CraneStatus.ESCAPE;
                }
                else
                {
                    if (Signal.Run.IsOn() &&
                        _stockerSignal.KeySwitch_HP.IsOn() && _stockerSignal.KeySwitch_OP.IsOn() &&
                        _stockerSignal.SafetyDoorClosed_HP.IsOn() && _stockerSignal.SafetyDoorClosed_OP.IsOn())
                    {
                        return StockerEnums.CraneStatus.IDLE;
                    }
                    else if (Signal.Run.IsOff())
                    {
                        return StockerEnums.CraneStatus.STOP;
                    }
                    else if (_stockerSignal.KeySwitch_HP.IsOff() || _stockerSignal.KeySwitch_OP.IsOff() ||
                             _stockerSignal.SafetyDoorClosed_HP.IsOff() || _stockerSignal.SafetyDoorClosed_OP.IsOff())
                    {
                        return StockerEnums.CraneStatus.MAINTAIN;
                    }
                    else
                    {
                        return StockerEnums.CraneStatus.NOSTS;
                    }
                }
            }
        }

        public IFork GetForkById(int id)
        {
            _forks.TryGetValue(id, out var fork);
            return fork;
        }

        public IFork GetLeftFork()
        {
            return GetForkById(1);
        }

        public IFork GetRightFork()
        {
            return GetForkById(2);
        }

        #endregion ICrane interface implementation

        public bool CanAutoSetRun { get; set; } = true;
        public int Speed { get; set; } = 100;
        public bool ReadyRecieveNewCommand => Signal.ReadyToReceiveNewCommand.IsOn();
        private readonly Dictionary<int, Fork> _forks = new Dictionary<int, Fork>();
        private readonly StockerSignal _stockerSignal;
        private List<AlarmInfo> _alarms = new List<AlarmInfo>();
        private bool _escapeTimeoutReported = false;
        private int _lastAlarmIndex = 0;
        private LCSEnums.AvailStatus _lastAvailStatus = LCSEnums.AvailStatus.None;
        private bool _lastEscapeSignalIsOn = false;
        private DateTime _lastEscapeSignalIsOnTime = DateTime.Now;
        private bool _lastKeySwitchIsAuto = false;
        private bool _lastLocationUpdatedIsOn = false;
        private StockerEnums.CraneStatus _lastStatus = StockerEnums.CraneStatus.NONE;

        public Crane(CraneSignal crane, StockerSignal stockerSignal)
        {
            _stockerSignal = stockerSignal;
            Id = crane.Id;
            Signal = crane;
            ReqAckController = new CraneReqAckController(this);

            _forks.Add(1, new Fork(1, Signal.LeftFork, this));
            _forks.Add(2, new Fork(2, Signal.RightFork, this));
        }

        public event StockerEvents.AlarmEventHandler OnAlarmCleared;

        public event StockerEvents.AlarmEventHandler OnAlarmIndexChanged;

        public event StockerEvents.CraneEventHandler OnEscapeTimeoutStatusChanged;

        public event StockerEvents.CraneEventHandler OnKeySwitchChanged;

        public LCSEnums.AvailStatus AvailStatus
        {
            get
            {
                if (Signal.Error.IsOff() && Signal.InService.IsOn() && Signal.Run.IsOn())
                {
                    return LCSEnums.AvailStatus.Avail;
                }
                return LCSEnums.AvailStatus.NotAvail;
            }
        }

        public CraneReqAckController ReqAckController { get; }
        public CraneSignal Signal { get; }

        public Task ClearCommandWriteZoneAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.D5021.SetValue(0);
                Signal.Controller.D5022.SetValue(0);

                if (Signal.Controller.TransferNo.GetValue() != 0
                    || Signal.Controller.CmdType_TransferWithoutIDRead.IsOn()
                    || Signal.Controller.CmdType_Transfer.IsOn()
                    || Signal.Controller.CmdType_Move.IsOn()
                    || Signal.Controller.CmdType_Scan.IsOn())
                {
                    Signal.Controller.CommandAbort.SetOff();

                    Signal.Controller.CmdType_TransferWithoutIDRead.SetOff();
                    Signal.Controller.CmdType_Transfer.SetOff();
                    Signal.Controller.CmdType_Move.SetOff();
                    Signal.Controller.CmdType_Move.SetOff();

                    Signal.Controller.UseLeftFork.SetOff();
                    Signal.Controller.UseRightFork.SetOff();

                    Signal.Controller.CommandData.Clear();
                }
            });
        }

        public IEnumerable<string> GetBufferCommands()
        {
            yield return Signal.CommandBuffer1.GetValue().ToString("D5");
            yield return Signal.CommandBuffer2.GetValue().ToString("D5");
            yield return Signal.CommandBuffer3.GetValue().ToString("D5");
        }

        public void InitialStatus()
        {
            _lastAvailStatus = this.AvailStatus;
            _lastStatus = this.Status;
            _lastLocationUpdatedIsOn = Signal.LocationUpdated.IsOn();
            _lastKeySwitchIsAuto = Signal.SRI.TheAMSwitchIsAuto_RM.IsOn();
            _lastEscapeSignalIsOn = Signal.Escape.IsOn();
            if (_lastEscapeSignalIsOn)
            {
                _lastEscapeSignalIsOnTime = DateTime.Now;
            }

            foreach (var forkItem in Forks)
            {
                var fork = forkItem as Fork;
                fork.InitialStatus();
            }
        }

        public bool IsReadyToReceiveNewCommand()
        {
            return Signal.Run.IsOn() && Signal.ReadyToReceiveNewCommand.IsOn();
        }

        public void RefreshStatus()
        {
            CheckAvailStatus();

            CheckStatus();

            CheckLocationUpdate();

            var newKeySwitchIsAuto = Signal.SRI.TheAMSwitchIsAuto_RM.IsOn();
            if (_lastKeySwitchIsAuto != newKeySwitchIsAuto)
            {
                _lastKeySwitchIsAuto = newKeySwitchIsAuto;
                var args = new CraneEventArgs(Id) { KeySwitchIsOn = newKeySwitchIsAuto };
                OnKeySwitchChanged?.Invoke(this, args);
            }

            CheckEscapeTimeoutStatus();

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
            else if (_alarms.Any() && Signal.Error.IsOff() && Signal.ErrorCode.GetValue() == 0)
            {
                //Alarm Reset
                OnAlarmCleared?.Invoke(this, new AlarmEventArgs(0, 0));
                _alarms.Clear();
            }
        }

        public Task RequestBuzzzerStopAsync()
        {
            return Task.Run(() =>
            {
                Signal.Controller.BuzzerStop.SetOn();
                Task.Delay(500).Wait();
                Signal.Controller.BuzzerStop.SetOff();
            });
        }

        public Task RequestCommandAbortAsync()
        {
            return Task.Run(() =>
            {
                if (Signal.Active.IsOff())
                {
                    Signal.Controller.CommandAbort.SetOn();
                }
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

        public Task RequestReturnHomeAsync()
        {
            return Task.Run(() =>
            {
                if (Signal.HPReturn.IsOff())
                {
                    Signal.Controller.HomeReturn.SetOn();
                    Task.Delay(500).Wait();
                    Signal.Controller.HomeReturn.SetOff();
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

                if (Signal.RunEnable.IsOn() && Signal.Run.IsOn() && Signal.Controller.Run.IsOn())
                {
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

                if (Signal.RunEnable.IsOn() && Signal.Run.IsOff() && Signal.Controller.Stop.IsOn())
                {
                    Signal.Controller.Stop.SetOff();
                }
            });
        }

        public Task<bool> WriteNewCommandAsync(CraneCmdInfo cmd)
        {
            return Task.Run(() =>
            {
                if (Signal.ReadyToReceiveNewCommand.IsOff())
                    throw new InvalidOperationException("ReadyToReceiveNewCommand Bit is Off");

                var axisSpeed = new[] { cmd.TravelAxisSpeed, cmd.LifterAxisSpeed, cmd.RotateAxisSpeed, cmd.ForkAxisSpeed };
                Signal.Controller.AxisSpeed.SetData(axisSpeed);

                Signal.Controller.NextStation.SetValue((int)cmd.NextStation);

                Signal.Controller.CommandAbort.SetOff();

                var cmdData = new List<int>();
                cmdData.Add(cmd.TaskNo);
                //FromLocation
                if (cmd.CmdType == CraneCmdType.SCAN || cmd.CmdType == CraneCmdType.FROM || cmd.CmdType == CraneCmdType.FROM_TO)
                {
                    cmdData.Add(cmd.FromLocation % 65536);
                    cmdData.Add(cmd.FromLocation / 65536);
                }
                else
                {
                    cmdData.Add(0);
                    cmdData.Add(0);
                }
                //ToLocation
                if (cmd.CmdType == CraneCmdType.MOVE || cmd.CmdType == CraneCmdType.TO || cmd.CmdType == CraneCmdType.FROM_TO)
                {
                    cmdData.Add(cmd.ToLocation % 65536);
                    cmdData.Add(cmd.ToLocation / 65536);
                }
                else
                {
                    cmdData.Add(0);
                    cmdData.Add(0);
                }
                cmdData.Add(cmd.BatchId);
                cmdData.AddRange(cmd.CstId.ToIntArray(10));
                Signal.Controller.CommandData.SetData(cmdData.ToArray());

                if (cmd.ForkType == CraneCmdForkType.Left)
                {
                    Signal.Controller.UseRightFork.SetOff();
                    Signal.Controller.UseLeftFork.SetOn();
                }
                else
                {
                    Signal.Controller.UseLeftFork.SetOff();
                    Signal.Controller.UseRightFork.SetOn();
                }

                switch (cmd.CmdType)
                {
                    case CraneCmdType.MOVE:
                        Signal.Controller.CmdType_Move.SetOn();
                        break;

                    case CraneCmdType.FROM:
                    case CraneCmdType.FROM_TO:
                        if (cmd.BCREnable)
                        {
                            Signal.Controller.CmdType_Transfer.SetOn();
                        }
                        else
                        {
                            Signal.Controller.CmdType_TransferWithoutIDRead.SetOn();
                        }
                        break;

                    case CraneCmdType.TO:
                        Signal.Controller.CmdType_TransferWithoutIDRead.SetOn();
                        break;

                    case CraneCmdType.SCAN:
                        Signal.Controller.CmdType_Scan.SetOn();
                        break;
                }

                //Wait for MPLC Receive
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var receivedAck = false;
                var readyoff = false;
                do
                {
                    Task.Delay(100).Wait();

                    if (receivedAck == false && Signal.TransferCommandReceived.IsOn())
                        receivedAck = true;
                    if (readyoff == false && Signal.ReadyToReceiveNewCommand.IsOff())
                        readyoff = true;
                    if (receivedAck && readyoff)
                    {
                        break;
                    }
                } while (stopWatch.ElapsedMilliseconds < 5000);

                Task.Run(() =>
                {
                    try
                    {
                        var args = new CraneEventArgs(Id)
                        {
                            CommandId = cmd.CommandIdForEvent,
                            TaskNo = cmd.TaskNoForEvent,
                            CarrierID = cmd.CarrierIDForEvent,
                        };
                        OnTaskCmdWriteToMPLC?.Invoke(this, args);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                    }
                });

                return true;
            });
        }

        private void CheckAvailStatus()
        {
            var newAvailStatus = this.AvailStatus;
            if (_lastAvailStatus != newAvailStatus)
            {
                _lastAvailStatus = newAvailStatus;
                var args = new CraneEventArgs(Id) { NewAvailStatus = newAvailStatus };
                OnAvailStatusChanged?.Invoke(this, args);
            }
        }

        private void CheckEscapeTimeoutStatus()
        {
            var newEscapeSignalIsOn = Signal.Escape.IsOn();
            if (_lastEscapeSignalIsOn == false && newEscapeSignalIsOn)
            {
                _lastEscapeSignalIsOnTime = DateTime.Now;
            }
            else if (_lastEscapeSignalIsOn && newEscapeSignalIsOn && _escapeTimeoutReported == false
                     && _lastEscapeSignalIsOnTime < DateTime.Now.AddMinutes(-5))
            {
                _escapeTimeoutReported = true;
                var args = new CraneEventArgs(Id) { SignalIsOn = true };
                OnEscapeTimeoutStatusChanged?.Invoke(this, args);
            }
            else if (_escapeTimeoutReported && newEscapeSignalIsOn == false)
            {
                _escapeTimeoutReported = false;
                var args = new CraneEventArgs(Id) { SignalIsOn = false };
                OnEscapeTimeoutStatusChanged?.Invoke(this, args);
            }
            _lastEscapeSignalIsOn = newEscapeSignalIsOn;
        }

        private void CheckLocationUpdate()
        {
            var newLocationUpdatedIsOn = Signal.LocationUpdated.IsOn();
            if (_lastLocationUpdatedIsOn == false && newLocationUpdatedIsOn)
            {
                var args = new CraneEventArgs(Id)
                {
                    T3 = Signal.T3.GetValue(),
                    Location = Signal.Location.GetValue().ToString(),
                };
                OnLocationUpdated?.Invoke(this, args);
            }

            _lastLocationUpdatedIsOn = newLocationUpdatedIsOn;
        }

        private void CheckStatus()
        {
            var newStatus = this.Status;
            if (_lastStatus != newStatus)
            {
                _lastStatus = newStatus;
                var args = new CraneEventArgs(Id) { NewStatus = newStatus };
                OnStatusChanged?.Invoke(this, args);
            }
        }

        private class AlarmInfo
        {
            public int AlarmCode { get; set; }
            public int AlarmIndex { get; set; }
        }

        public string GetBCRResultByForkNoAndWait(int forkNo, int millisecondsTimeout = 1000)
        {
            if (forkNo < 1 || forkNo > 2)
                return string.Empty;
            if (millisecondsTimeout <= 0 || millisecondsTimeout > 5000)
                millisecondsTimeout = 1000;

            var stopwatch = Stopwatch.StartNew();
            SpinWait.SpinUntil(() => false, 800); // wait for BCRResult

            string bcrResult;
            do
            {
                SpinWait.SpinUntil(() => false, 200);

                bcrResult = forkNo == 1
                    ? GetLeftFork().BCRResult
                    : GetRightFork().BCRResult;
            } while (string.IsNullOrWhiteSpace(bcrResult) && stopwatch.ElapsedMilliseconds < millisecondsTimeout);

            return bcrResult;
        }
    }
}
