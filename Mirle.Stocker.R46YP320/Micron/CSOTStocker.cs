using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Mirle.Extensions;
using Mirle.Stocker.R46YP320.Events;
using Mirle.Stocker.R46YP320.Signal;

namespace Mirle.Stocker.R46YP320
{
    public class CSOTStocker : IStocker
    {
        #region IStocker interface implementation

        public LCSEnums.AvailStatus AvailStatus
        {
            get
            {
                if (_controlMode == LCSEnums.ControlMode.Single || _controlMode == LCSEnums.ControlMode.TwinFork)
                {
                    _cranes.TryGetValue(1, out Crane crane1);
                    return crane1.AvailStatus;
                }
                else
                {
                    _cranes.TryGetValue(1, out Crane crane1);
                    _cranes.TryGetValue(2, out Crane crane2);
                    if (crane1.Signal.SingleCraneMode.IsOn())
                        return crane1.AvailStatus;
                    if (crane2.Signal.SingleCraneMode.IsOn())
                        return crane2.AvailStatus;
                    return crane1.AvailStatus == LCSEnums.AvailStatus.Avail && crane2.AvailStatus == LCSEnums.AvailStatus.Avail
                        ? LCSEnums.AvailStatus.Avail : LCSEnums.AvailStatus.NotAvail;
                }
            }
        }

        public IEnumerable<ICrane> Cranes => _cranes.Values;
        public IEnumerable<IEQPort> EQPorts => _eqs.Values;
        public int HandOffEndBay => Signal.HandOff_EndBay.GetValue();
        public int HandOffStartBay => Signal.HandOff_StartBay.GetValue();
        public IEnumerable<IIOPort> IOPorts => _ios.Values;
        public int ShareAreaEndBay => Signal.ShareArea_EndBay.GetValue();
        public int ShareAreaStartBay => Signal.ShareArea_StartBay.GetValue();

        public StockerEnums.StockerStatus Status
        {
            get
            {
                if (_controlMode == LCSEnums.ControlMode.Single || _controlMode == LCSEnums.ControlMode.TwinFork)
                {
                    _cranes.TryGetValue(1, out Crane crane1);
                    switch (crane1.Status)
                    {
                        case StockerEnums.CraneStatus.ESCAPE:
                        case StockerEnums.CraneStatus.BUSY:
                            return StockerEnums.StockerStatus.RUN;

                        case StockerEnums.CraneStatus.Waiting:
                        case StockerEnums.CraneStatus.IDLE:
                            return StockerEnums.StockerStatus.IDLE;

                        case StockerEnums.CraneStatus.MAINTAIN:
                            return StockerEnums.StockerStatus.MAINTAIN;

                        case StockerEnums.CraneStatus.NOSTS:
                        case StockerEnums.CraneStatus.NONE:
                            return StockerEnums.StockerStatus.NONE;

                        case StockerEnums.CraneStatus.HOMEACTION:
                        case StockerEnums.CraneStatus.STOP:
                        case StockerEnums.CraneStatus.WAITINGHOMEACTION:
                            return StockerEnums.StockerStatus.DOWN;
                    }
                    return StockerEnums.StockerStatus.NONE;
                }
                else
                {
                    _cranes.TryGetValue(1, out Crane crane1);
                    _cranes.TryGetValue(2, out Crane crane2);
                    if (crane1.Status == StockerEnums.CraneStatus.BUSY || crane2.Status == StockerEnums.CraneStatus.BUSY)
                    { return StockerEnums.StockerStatus.RUN; }
                    else if (crane1.Status == StockerEnums.CraneStatus.ESCAPE || crane2.Status == StockerEnums.CraneStatus.ESCAPE)
                    { return StockerEnums.StockerStatus.RUN; }
                    else if (crane1.Status == StockerEnums.CraneStatus.IDLE || crane2.Status == StockerEnums.CraneStatus.IDLE)
                    { return StockerEnums.StockerStatus.IDLE; }
                    else if (crane1.Status == StockerEnums.CraneStatus.MAINTAIN && crane2.Status == StockerEnums.CraneStatus.MAINTAIN
                            || crane1.Status == StockerEnums.CraneStatus.MAINTAIN && crane2.Status == StockerEnums.CraneStatus.NONE
                            || crane1.Status == StockerEnums.CraneStatus.NONE && crane2.Status == StockerEnums.CraneStatus.MAINTAIN)
                    { return StockerEnums.StockerStatus.MAINTAIN; }
                    else if (crane1.Status == StockerEnums.CraneStatus.STOP && crane2.Status == StockerEnums.CraneStatus.STOP
                            || crane1.Status == StockerEnums.CraneStatus.STOP && crane2.Status == StockerEnums.CraneStatus.NONE
                            || crane1.Status == StockerEnums.CraneStatus.NONE && crane2.Status == StockerEnums.CraneStatus.STOP
                            || crane1.Status == StockerEnums.CraneStatus.STOP && crane2.Status == StockerEnums.CraneStatus.MAINTAIN
                            || crane1.Status == StockerEnums.CraneStatus.MAINTAIN && crane2.Status == StockerEnums.CraneStatus.STOP)
                    { return StockerEnums.StockerStatus.DOWN; }
                    else if (crane1.Status == StockerEnums.CraneStatus.Waiting && crane2.Status == StockerEnums.CraneStatus.Waiting)
                    { return StockerEnums.StockerStatus.IDLE; }
                    else
                    { return StockerEnums.StockerStatus.NONE; }
                }
            }
        }

        public ICrane GetCraneById(int id)
        {
            _cranes.TryGetValue(id, out Crane crane);
            return crane;
        }

        public IEQPort GetEQPortById(int id)
        {
            _eqs.TryGetValue(id, out EQPort eqPort);
            return eqPort;
        }

        public IIOPort GetIOPortById(int id)
        {
            _ios.TryGetValue(id, out IOPort ioPort);
            return ioPort;
        }

        public bool IsAreaSensorOnById(int id)
        {
            return Signal.GetAreaSensorSignalById(id).AreaSensor.IsOn();
        }

        public bool IsDataLinkStatusOnById(int id)
        {
            return Signal.GetDataLinkStatusSignalById(id).DataLinkStatus.IsOn();
        }

        #endregion IStocker interface implementation

        private readonly LCSEnums.ControlMode _controlMode;
        private readonly Dictionary<int, Crane> _cranes = new Dictionary<int, Crane>();
        private readonly Dictionary<int, EQPort> _eqs = new Dictionary<int, EQPort>();
        private readonly Dictionary<int, IOPort> _ios = new Dictionary<int, IOPort>();
        private readonly SignalMapper4_11 _signalDefine;
        private LCSEnums.AvailStatus _lastAvailStatus = LCSEnums.AvailStatus.None;
        private bool _lastKeySwitchIsAuto = false;
        private bool _lastSafetyDoorIsClosed = false;
        private bool _lastMaintenanceModeIsOn = false;
        private readonly object _calibrateSystemTimeLock = new object();

        /// <summary>
        /// 上次與PLC校時的時間 YYYYMMDDHH
        /// /// </summary>
        private DateTime _calibrationTimestamp = DateTime.MinValue;

        public CSOTStocker(SignalMapper4_11 signalDefine, LCSEnums.ControlMode controlMode)
        {
            _signalDefine = signalDefine;
            _controlMode = controlMode;

            foreach (var crane in _signalDefine.Cranes)
            {
                _cranes.Add(crane.Id, new Crane(crane, signalDefine.Stocker));
            }

            foreach (var io in signalDefine.IoPorts)
            {
                _ios.Add(io.Id, new IOPort(io));
            }

            foreach (var eq in signalDefine.EqPorts)
            {
                _eqs.Add(eq.Id, new EQPort(eq));
            }
        }

        public event StockerEvents.StockerEventHandler OnAreaSensorChanged;

        public event StockerEvents.StockerEventHandler OnAvailStatusChanged;

        public event StockerEvents.StockerEventHandler OnDataLinkStatusChanged;

        public event StockerEvents.StockerEventHandler OnKeySwitchChanged;

        public event StockerEvents.StockerEventHandler OnSafetyDoorClosedChanged;

        public event StockerEvents.StockerEventHandler OnMaintenanceModeChanged;

        public bool IsPMSChargeShelfOnline => false;
        public bool IsPMSChargeShelfInPreserved => false;
        public bool KeySwitchIsAuto => Signal.KeySwitch_HP.IsOn();//&& Signal.KeySwitch_OP.IsOn();
        public bool SafetyDoorIsClosed => Signal.SafetyDoorClosed_HP.IsOn();// && Signal.SafetyDoorClosed_OP.IsOn();
        public bool MaintenanceModeIsOn => Signal.MaintenanceMode.IsOn();
        public StockerSignal Signal => _signalDefine.Stocker;

        public void InitialStatus()
        {
            _lastAvailStatus = this.AvailStatus;
            _lastKeySwitchIsAuto = this.KeySwitchIsAuto;
            _lastSafetyDoorIsClosed = this.SafetyDoorIsClosed;

            foreach (var areaSensor in Signal.AreaSensors)
            {
                areaSensor.LastAreaSensorIson = areaSensor.AreaSensor.IsOn();
            }
            foreach (var datalink in Signal.DataLinkStatusStations)
            {
                datalink.LastDataLinkStatusIsOn = datalink.DataLinkStatus.IsOn();
            }
        }

        public void RefreshStatus()
        {
            var newAvailStatus = this.AvailStatus;
            if (_lastAvailStatus != newAvailStatus)
            {
                _lastAvailStatus = newAvailStatus;
                var args = new StockerEventArgs() { NewAvailStatus = newAvailStatus };
                OnAvailStatusChanged?.Invoke(this, args);
            }

            var newKeySwitchIsAuto = this.KeySwitchIsAuto;
            if (_lastKeySwitchIsAuto != newKeySwitchIsAuto)
            {
                _lastKeySwitchIsAuto = newKeySwitchIsAuto;
                var args = new StockerEventArgs() { KeySwitchIsOn = newKeySwitchIsAuto };
                OnKeySwitchChanged?.Invoke(this, args);
            }

            var newMaintenanceModeIsOn = this.MaintenanceModeIsOn;
            if (_lastMaintenanceModeIsOn != newMaintenanceModeIsOn)
            {
                _lastMaintenanceModeIsOn = newMaintenanceModeIsOn;
                var args = new StockerEventArgs() { MaintenanceModeIsOn = newMaintenanceModeIsOn };
                OnMaintenanceModeChanged?.Invoke(this, args);
            }

            var newSafetyDoorIsClosed = this.SafetyDoorIsClosed;
            if (_lastSafetyDoorIsClosed != newSafetyDoorIsClosed)
            {
                _lastSafetyDoorIsClosed = newSafetyDoorIsClosed;
                var args = new StockerEventArgs() { SafetyDoorIsClosed = newSafetyDoorIsClosed };
                OnSafetyDoorClosedChanged?.Invoke(this, args);
            }

            foreach (var areaSensor in Signal.AreaSensors)
            {
                var newStatus = areaSensor.AreaSensor.IsOn();
                if (areaSensor.LastAreaSensorIson != newStatus)
                {
                    areaSensor.LastAreaSensorIson = newStatus;
                    var args = new StockerEventArgs() { StationId = areaSensor.Id, ErrorIsOn = newStatus };
                    OnAreaSensorChanged?.Invoke(this, args);
                }
            }
            foreach (var datalink in Signal.DataLinkStatusStations)
            {
                var newStatus = datalink.DataLinkStatus.IsOn();
                if (datalink.LastDataLinkStatusIsOn != newStatus)
                {
                    datalink.LastDataLinkStatusIsOn = newStatus;
                    var args = new StockerEventArgs() { StationId = datalink.Id, ErrorIsOn = newStatus };
                    OnDataLinkStatusChanged?.Invoke(this, args);
                }
            }
        }

        public Task SetMCSOnlineAsync()
        {
            return Task.Run(() =>
            {
                var mcsOnline = Signal.Controller.MCSOnline;
                if (mcsOnline.IsOn())
                {
                    mcsOnline.SetOff();
                }
                else
                {
                    mcsOnline.SetOn();
                }
            });
        }

        public Task<bool> SetShareAreaAndHandoffAsync(int shareAreaStart, int shareAreaEnd, int handoffStart, int handoffEnd)
        {
            return Task.Run(() =>
            {
                if (shareAreaStart <= handoffStart && handoffStart <= handoffEnd && handoffEnd <= shareAreaEnd)
                {
                    Signal.Controller.ShareArea_StartBay.SetValue(shareAreaStart);
                    Signal.Controller.ShareArea_EndBay.SetValue(shareAreaEnd);
                    Signal.Controller.HandOff_StartBay.SetValue(handoffStart);
                    Signal.Controller.HandOff_EndBay.SetValue(handoffEnd);
                    return true;
                }
                return false;
            });
        }

        public Task SetPMSChargeShelfAsync(bool isPreserved)
        {
            return Task.Run(() =>
            {
            });
        }

        public Task<bool> CalibrateSystemTimeAsync()
        {
            return Task.Run(() =>
            {
                CalibrateSystemTimeInDifferentHour(true);
                return true;
            });
        }

        internal void CalibrateSystemTimeInDifferentHour(bool setItNow = false)
        {
            var ctrl = Signal.Controller;

            lock (_calibrateSystemTimeLock)
            {
                var now = DateTime.Now;
                int[] bcdDatetime = new int[3];
                bcdDatetime[0] = (now.Year % 100 * 100 + now.Month).IntToBCD();
                bcdDatetime[1] = (now.Day * 100 + now.Hour).IntToBCD();
                bcdDatetime[2] = (now.Minute % 100 * 100 + now.Second).IntToBCD();

                ctrl.SystemTimeCalibration.SetData(bcdDatetime);

                if (setItNow || _calibrationTimestamp.Hour != now.Hour)
                {
                    ctrl.DatetimeCalibration.SetOn();
                    Task.Delay(3000).Wait();
                    ctrl.DatetimeCalibration.SetOff();
                    _calibrationTimestamp = now;
                }
            }
        }
    }
}
