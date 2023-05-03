using Mirle.MPLC;
using Mirle.Stocker.R46YP320.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mirle.Stocker.R46YP320
{
    public class StockerController
    {
        private readonly IMPLCProvider _mplc;
        private readonly CSOTStocker _stocker;

        public CSOTStocker GetStocker()
        {
            return _stocker;
        }

        /// <summary>
        /// 上次與PLC校時的時間 YYYYMMDDHH
        /// </summary>
        private DateTime _calibrationTimestamp = DateTime.MinValue;

        private ThreadWorker _heartbeat;
        private ThreadWorker _crane1EventWorker;
        private ThreadWorker _crane2EventWorker;
        private ThreadWorker _stockerEventWorker;

        private Stopwatch _mainProcStopwatch = new Stopwatch();
        private Stopwatch _crane1Stopwatch = new Stopwatch();
        private Stopwatch _crane2Stopwatch = new Stopwatch();

        public StockerController(IMPLCProvider mplc, CSOTStocker stocker)
        {
            _mplc = mplc;
            _stocker = stocker;

            InitialStatus();
            foreach (var craneItem in _stocker.Cranes)
            {
                var crane = craneItem as Crane;
                crane.InitialStatus();
                crane.ReqAckController.InitailStatus();
                foreach (var forkItem in crane.Forks)
                {
                    var fork = forkItem as Fork;
                    fork.InitialStatus();
                }
            }
            _stocker.InitialStatus();
            foreach (var ioItem in _stocker.IOPorts)
            {
                var io = ioItem as IOPort;
                io.InitialStatus();
                foreach (var vehicleItem in io.Vehicles)
                {
                    var vehicle = vehicleItem as IOVehicle;
                    vehicle.InitialStatus();
                }
                foreach (var stageItem in io.Stages)
                {
                    var stage = stageItem as IOStage;
                    stage.InitialStatus();
                }
            }
            foreach (var eqItem in _stocker.EQPorts)
            {
                var eq = eqItem as EQPort;
                eq.InitialStatus();
            }

            _heartbeat = new ThreadWorker(Heartbeat, 2000, false);
            _crane1EventWorker = new ThreadWorker(() => CraneEventProc(1), 200, false);
            _crane2EventWorker = new ThreadWorker(() => CraneEventProc(2), 200, false);
            _stockerEventWorker = new ThreadWorker(StockerEventProc, 200, false);
            ClearControlBit();
        }

        private void ClearControlBit()
        {
            Task.Delay(5000).ContinueWith(async t =>
            {
                foreach (var ioItem in _stocker.IOPorts)
                {
                    var io = ioItem as IOPort;
                    await io.ClearControlBitsAsync();
                }
            });
        }

        public bool PLCIsConnected => _mplc.IsConnected;
        public long MainProcessTime { get; private set; }
        public long Crane1ProcessTime { get; private set; }
        public long Crane2ProcessTime { get; private set; }

        private bool _lastMPLCConnectionStatus = true;

        public event StockerEvents.StockerEventHandler OnMPLCConnectionStatusChanged;

        private void InitialStatus()
        {
        }

        private void RefreshStatus()
        {
            var newStatus = PLCIsConnected;
            if (_lastMPLCConnectionStatus != newStatus)
            {
                _lastMPLCConnectionStatus = newStatus;
                var args = new StockerEventArgs() { MPLCIsConnected = newStatus };
                OnMPLCConnectionStatusChanged?.Invoke(this, args);
            }
        }

        private void Heartbeat()
        {
            var ctrl = _stocker.Signal.Controller;

            if (ctrl.Heartbeat.IsOff())
            {
                ctrl.Heartbeat.SetOn();
            }

            _stocker.CalibrateSystemTimeInDifferentHour();
        }

        public void Start()
        {
            _heartbeat.Start();
            _crane1EventWorker.Start();
            _crane2EventWorker.Start();
            _stockerEventWorker.Start();
        }

        public void Pause()
        {
            _heartbeat.Pause();
            _crane1EventWorker.Pause();
            _crane2EventWorker.Pause();
            _stockerEventWorker.Pause();
        }

        private void CraneEventProc(int craneId)
        {
            var stopWatch = craneId == 1 ? _crane1Stopwatch : _crane2Stopwatch;
            stopWatch.Restart();
            var crane = _stocker.GetCraneById(craneId) as Crane;
            crane.RefreshStatus();
            crane.ReqAckController.RefreshStatus();
            foreach (var forkItem in crane.Forks)
            {
                var fork = forkItem as Fork;
                fork.RefreshStatus();
            }
            stopWatch.Stop();

            if (craneId == 1)
            {
                Crane1ProcessTime = stopWatch.ElapsedMilliseconds;
            }
            else
            {
                Crane2ProcessTime = stopWatch.ElapsedMilliseconds;
            }
        }

        private void StockerEventProc()
        {
            _mainProcStopwatch.Restart();

            RefreshStatus();
            _stocker.RefreshStatus();
            foreach (var ioItem in _stocker.IOPorts)
            {
                var io = ioItem as IOPort;
                io.RefreshStatus();

                foreach (var vehicleItem in io.Vehicles)
                {
                    var vehicle = vehicleItem as IOVehicle;
                    vehicle.RefreshStatus();
                }

                foreach (var stageItem in io.Stages)
                {
                    var stage = stageItem as IOStage;
                    stage.RefreshStatus();
                }
            }
            foreach (var eqItem in _stocker.EQPorts)
            {
                var eq = eqItem as EQPort;
                eq.RefreshStatus();
            }

            _mainProcStopwatch.Stop();
            MainProcessTime = _mainProcStopwatch.ElapsedMilliseconds;
        }
    }
}