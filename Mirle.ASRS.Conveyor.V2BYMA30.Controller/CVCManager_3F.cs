using Mirle.MPLC;
using Mirle.MPLC.DataType;
using System;
using System.Collections.Generic;
using Mirle.ASRS.Conveyor.V2BYMA30_3F.Events;
using System.Linq;
using Mirle.ASRS.Conveyor.V2BYMA30_3F.Service;
using Mirle.ASRS.Conveyor.V2BYMA30_3F.Signal;
using Mirle.ASRS.Conveyor.V2BYMA30_3F.View;
using System.Windows.Forms;
using Mirle.MPLC.SharedMemory;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.MXComponent;
using Mirle.Extensions;

namespace Mirle.ASRS.Conveyor.Controller
{
    public class CVCManager_3F : IDisposable
    {
        private readonly LoggerService _LoggerService;
        private readonly ConveyorConfig _ConveyorConfig;
        private readonly SignalMapper _Signal;
        private readonly Dictionary<int, V2BYMA30_3F.Buffer> _Buffers = new Dictionary<int, V2BYMA30_3F.Buffer>();
        private IPLCHost _plcHost;
        private IMPLCProvider _mplc;
        private bool _HeartbeatReport = false;
        private DateTime _LastHeartbeatTime = DateTime.Now;

        private ThreadWorker _Heartbeat;
        private ThreadWorker _CalibrateSystemTime;
        private ThreadWorker _Buffer;
        public bool IsConnected => _mplc.IsConnected;
        public int BufferCount => SignalMapper.BufferCount;

        public CVCManager_3F(ConveyorConfig config)
        {
            _ConveyorConfig = config;
            _LoggerService = new LoggerService(config.ConveyorId);
            if (config.IsMemorySimulator == false)
            {
                if (config.IsUseMCProtocol)
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
            InitialBuffer();
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

        public V2BYMA30_3F.Buffer GetBuffer(int bufferIndex)
        {
            _Buffers.TryGetValue(bufferIndex, out var buffer);
            return buffer;
        }

        public void Pause()
        {
            _Heartbeat?.Pause();
            _CalibrateSystemTime?.Pause();
            _Buffer?.Pause();
        }

        public void Start()
        {
            Pause();

            _Heartbeat = new ThreadWorker(Heartbeat, 300, true);
            _CalibrateSystemTime = new ThreadWorker(CalibrateSystemTime, 1000, true);          
            _Buffer = new ThreadWorker(RefreshBuffer, 500, true);
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
            }
        }

        public void Refresh()
        {
            if (!IsConnected)
                return;
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
            bcdDatetime[0] = (now.Year % 100 * 100 + now.Month).IntToBCD();
            bcdDatetime[1] = (now.Day * 100 + now.Hour).IntToBCD();
            bcdDatetime[2] = (now.Minute % 100 * 100 + now.Second).IntToBCD();

            ctrl.SystemTimeCalibration.SetData(bcdDatetime);
        }


        private void InitialBuffer()
        {
            foreach (int i in Enumerable.Range(1, SignalMapper.BufferCount))
            {
                var buffer = new V2BYMA30_3F.Buffer(_Signal.GetBufferSignal(i), "CV");
                _Buffers.Add(i, buffer);
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
                _Buffer?.Dispose();

                disposedValue = true;
            }
        }

        ~CVCManager_3F()
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
