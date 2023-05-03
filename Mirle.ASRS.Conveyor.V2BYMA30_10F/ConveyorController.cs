using Mirle.MPLC;
using Mirle.MPLC.DataType;
using System;
using System.Collections.Generic;
using Mirle.ASRS.Conveyor.V2BYMA30_10F.Events;
using System.Linq;
using Mirle.ASRS.Conveyor.V2BYMA30_10F.Service;
using Mirle.ASRS.Conveyor.V2BYMA30_10F.Signal;
using Mirle.ASRS.Conveyor.V2BYMA30_10F.View;
using System.Windows.Forms;
using Mirle.MPLC.SharedMemory;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.MXComponent;
using Mirle.Extensions;
using System.Reflection;

namespace Mirle.ASRS.Conveyor.V2BYMA30_10F
{
    public class ConveyorController : IDisposable
    {
        public delegate void AlarmBitEventHandler(object sender, AlarmBitEventArgs e);
        public delegate void AlarmEventHandler(object sender, AlarmEventArgs e);

        public event AlarmBitEventHandler OnAlarmBitSignalChanged;
        public event AlarmEventHandler OnHeartbeatError;

        private readonly LoggerService _LoggerService;
        private readonly ConveyorConfig _ConveyorConfig;
        private readonly SignalMapper _Signal;
        private readonly Dictionary<int, Buffer> _Buffers = new Dictionary<int, Buffer>();
        private readonly bool[] _AlarmBit = new bool[16];
        private bool _HeartbeatReport = false;
        private DateTime _LastHeartbeatTime = DateTime.Now;

        private ThreadWorker _Heartbeat;
        private ThreadWorker _CalibrateSystemTime;
        private ThreadWorker _ModeChange;
        private ThreadWorker _ErrorIndex;
        private ThreadWorker _Buffer;

        private IPLCHost _plcHost;
        private IMPLCProvider _mplc;

        public int[] OpcData => _Signal.GetConveyorSignal().OpcData.GetData();

        public ConveyorSignal Signal => _Signal.GetConveyorSignal();
        public bool IsConnected => _mplc.IsConnected;
        public int BufferCount => SignalMapper.BufferCount;

        public ConveyorController(IMPLCProvider _mplc)
        {
            _Signal = new SignalMapper(_mplc);

            InitalBuffer();
        }

        public ConveyorController(ConveyorConfig config)
        {
            _ConveyorConfig = config;
            _LoggerService = new LoggerService(config.ConveyorId);

            if (_ConveyorConfig.IsMemorySimulator == false)
            {
                if (_ConveyorConfig.IsUseMCProtocol)
                {
                    InitialMCPLCR();
                }
                else
                {
                    InitialPLCR();
                }

                IMPLCProvider mplcWriter = new MPLCService(_plcHost.GetMPLCProvider(), _LoggerService);

                _mplc = new ReadWriteWrapper(reader: _plcHost as IMPLCProvider, writer: mplcWriter);
            }
            else
            {
                var smReader = new SMReadOnlyCachedReader();
                var smWirter = new SMReadWriter();
                foreach (var block in SignalMapper.SignalBlocks)
                {
                    smReader.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{_ConveyorConfig.ConveyorId}-{block.SharedMemoryName}"));
                    smWirter.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{_ConveyorConfig.ConveyorId}-{block.SharedMemoryName}"));
                }
                _mplc = new ReadWriteWrapper(smReader, smWirter);
            }

            _Signal = new SignalMapper(_mplc);
            InitalBuffer();
        }

        private void InitialMCPLCR()
        {
            var plcHostInfo = new MPLC.MCProtocol.PLCHostInfo();
            plcHostInfo.IPAddress = _ConveyorConfig.IPAddress;
            plcHostInfo.Port = _ConveyorConfig.Port;
            plcHostInfo.HostId = _ConveyorConfig.ConveyorId;
            plcHostInfo.BlockInfos = SignalMapper.SignalBlocks.Select(b => new BlockInfo(b.DeviceRange, $@"Global\{_ConveyorConfig.ConveyorId}-{b.SharedMemoryName}", b.PLCRawdataIndex));

            _plcHost = new MPLC.MCProtocol.PLCHost(plcHostInfo);
            _plcHost.Interval = 200;
            _plcHost.MPLCTimeout = 5000;
            _plcHost.EnableWriteRawData = true;
            _plcHost.EnableAutoReconnect = true;
            _plcHost.LogBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "LOG";
            _plcHost.Start();
        }

        private void InitialPLCR()
        {
            var plcHostInfo = new PLCHostInfo();
            plcHostInfo.ActLogicalStationNo = _ConveyorConfig.MPLCNo;
            plcHostInfo.HostId = _ConveyorConfig.ConveyorId;
            plcHostInfo.BlockInfos = SignalMapper.SignalBlocks.Select(b => new BlockInfo(b.DeviceRange, $@"Global\{_ConveyorConfig.ConveyorId}-{b.SharedMemoryName}", b.PLCRawdataIndex));

            _plcHost = new PLCHost(plcHostInfo);
            _plcHost.Interval = 200;
            _plcHost.EnableWriteRawData = true;
            _plcHost.EnableAutoReconnect = true;
            _plcHost.LogBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "LOG";
            _plcHost.Start();
        }

        public Buffer GetBuffer(int bufferIndex)
        {
            _Buffers.TryGetValue(bufferIndex, out var buffer);
            return buffer;
        }
        public bool WriteErrorIndex(int index)
        {
            var ctrl = _Signal.GetConveyorSignal().Controller;
            try
            {
                ctrl.ErrorIndex.SetValue(index);

                return true;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        public void Start()
        {
            Pause();

            _Heartbeat = new ThreadWorker(Heartbeat, 300, true);
            _CalibrateSystemTime = new ThreadWorker(CalibrateSystemTime, 1000, true);
            //_Refresh = new ThreadWorker(Refresh, 500, true);
            _ErrorIndex = new ThreadWorker(ErrorIndex, 500, true);
            _Buffer = new ThreadWorker(RefreshBuffer, 500, true);
            _ModeChange = new ThreadWorker(ModeChange, 500, true);
        }

        public void Pause()
        {
            _Heartbeat?.Pause();
            _CalibrateSystemTime?.Pause();
            //_Refresh?.Pause();
            _Buffer?.Pause();
        }

        public void Heartbeat()
        {
            if (!IsConnected)
                return;
            var plc = _Signal.GetConveyorSignal();
            var ctrl = _Signal.GetConveyorSignal().Controller;
            if (plc.Heartbeat.GetValue() == ctrl.Heartbeat.GetValue())
            {
                if (plc.Heartbeat.GetValue() == 0)
                    ctrl.Heartbeat.SetValue(1);
                else
                    ctrl.Heartbeat.SetValue(0);

                _LastHeartbeatTime = DateTime.Now;

                if (_HeartbeatReport)
                {
                    _HeartbeatReport = false;
                    OnHeartbeatError?.Invoke(this, new AlarmEventArgs(false));
                }
            }
            else
            {
                if (_LastHeartbeatTime.AddSeconds(3) > DateTime.Now && _HeartbeatReport)
                {
                    _HeartbeatReport = true;
                    OnHeartbeatError?.Invoke(this, new AlarmEventArgs(true));
                }
            }
        }

        public void Refresh()
        {
            if (!IsConnected)
                return;
            for (int i = 0; i < 16; i++)
            {
                CheckAlarmBit(ref _AlarmBit[i], OnAlarmBitSignalChanged, i, _Signal.GetConveyorSignal().AlarmBit.AlarmBit[i].Checked);
            }
        }

        public void ModeChange()
        {
            if (!IsConnected)
                return;
            var plc = _Signal.GetConveyorSignal();
            var ctrl = _Signal.GetConveyorSignal().Controller;
            if (plc.ModeStatus.GetValue() == ctrl.ModeChange.GetValue())
            {
                ctrl.ModeChange.SetValue(0);
            }
        }

        public void ErrorIndex()
        {
            if (!IsConnected)
                return;
            var plc = _Signal.GetConveyorSignal();
            var ctrl = _Signal.GetConveyorSignal().Controller;
            if (plc.ErrorIndex.GetValue() == ctrl.ErrorIndex.GetValue())
            {
                ctrl.ErrorIndex.SetValue(0);
            }

        }

        private void RefreshBuffer()
        {
            if (!IsConnected)
                return;
            foreach (var buffer in _Buffers.Values)
            {
                buffer.Refresh();
            }
        }

        private void CalibrateSystemTime()
        {
            if (!IsConnected)
                return;
            var ctrl = _Signal.GetConveyorSignal().Controller;
            var now = DateTime.Now;
            int[] bcdDatetime = new int[3];
           
            //原本的16 進制改成 10進制
            //bcdDatetime[0] = (now.Year % 100 * 100 + now.Month).IntToBCD();
            //bcdDatetime[1] = (now.Day * 100 + now.Hour).IntToBCD();
            //bcdDatetime[2] = (now.Minute % 100 * 100 + now.Second).IntToBCD();

            bcdDatetime[0] = (now.Year % 100 * 256 + now.Month);
            bcdDatetime[1] = (now.Day * 256 + now.Hour);
            bcdDatetime[2] = (now.Minute * 256 + now.Second);

            ctrl.SystemTimeCalibration.SetData(bcdDatetime);
        }

        private void InitalBuffer()
        {
            foreach (int i in Enumerable.Range(1, SignalMapper.BufferCount))
            {
                var buffer = new Buffer(_Signal.GetBufferSignal(i), "CV");
                _Buffers.Add(i, buffer);
            }
        }

        private void CheckAlarmBit(ref bool reportedFlag, AlarmBitEventHandler eventHandler, int bit, Bit alarmBit)
        {
            if (alarmBit.IsOn())
            {
                if (reportedFlag == false && alarmBit.IsOn())
                {
                    reportedFlag = true;
                    eventHandler?.Invoke(this, new AlarmBitEventArgs(0, bit, true));
                }
            }
            else
            {
                if (reportedFlag && alarmBit.IsOff())
                {
                    reportedFlag = false;
                    eventHandler?.Invoke(this, new AlarmBitEventArgs(0, bit, false));
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                Pause();

                _Heartbeat?.Dispose();
                _CalibrateSystemTime?.Dispose();
                //_Refresh?.Dispose();
                _Buffer?.Dispose();

                disposedValue = true;
            }
        }

        ~ConveyorController()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
