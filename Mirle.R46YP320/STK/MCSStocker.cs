using Mirle.LCS.Models;
using Mirle.R46YP320.STK.MCS;
using Mirle.R46YP320.STK.TaskService;
using Mirle.MPLC.Extensions;
using Mirle.Stocker;
using Mirle.Stocker.Enums;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.TraceLog;
using System.Collections.Generic;
using System.Linq;
using Mirle.STKC.R46YP320.Service;

namespace Mirle.R46YP320.STK
{
    public class MCSStocker
    {
        private readonly IStocker _stocker;
        private readonly TaskInfo _taskInfo;
        private readonly DataCollectionEventsService _dataCollectionEvents;
        private readonly AlarmService _alarmService;
        private readonly RepositoriesService _Repositories;
        private ThreadWorker _heartBeat;
        private Dictionary<string, MCSIOPort> _mcsIOPorts = new Dictionary<string, MCSIOPort>();
        public bool IsAvail => _stocker.AvailStatus == LCS.Enums.LCSEnums.AvailStatus.Avail;

        public MCSStocker(IStocker stocker, TaskInfo taskInfo, DataCollectionEventsService dataCollectionEvents, LoggerService loggerService, AlarmService alarmService)
        {
            _stocker = stocker;
            _taskInfo = taskInfo;
            _dataCollectionEvents = dataCollectionEvents;
            _alarmService = alarmService;
            _Repositories = new RepositoriesService(taskInfo, loggerService);

            var AllPort = _Repositories.GetAllPortDef();

            InitIOPort(stocker, taskInfo, loggerService, AllPort.Where(i => i.PortType == (int)PortType.IO && i.ReportMCSFlag));

            foreach (var port in _mcsIOPorts.Values)
            {
                port.InitialStatus();
            }

            _heartBeat = new ThreadWorker(HeartBeat, 200, true);
        }

        private void InitIOPort(IStocker stocker, TaskInfo taskInfo, LoggerService loggerService, IEnumerable<PortDef> portDefs)
        {
            foreach (var io in portDefs)
            {
                var PortID = (io.HostEQPortID.EndsWith("L") || io.HostEQPortID.EndsWith("R")) ? io.HostEQPortID.Remove(io.HostEQPortID.Length - 1) : io.HostEQPortID;
                if (!_mcsIOPorts.ContainsKey(PortID))
                {
                    var port = _taskInfo.PortInfos.Where(p => p.PortType == PortType.IO && p.HostEQPortID.Contains(PortID));
                    if (port.Count() == 1)
                    {
                        _mcsIOPorts.Add(PortID, new MCSIOPort(stocker, _dataCollectionEvents, taskInfo, loggerService, PortID, _stocker.GetIOPortById(io.PortTypeIndex), _stocker.GetIOPortById(io.PortTypeIndex), _alarmService));
                    }
                    else if (port.Count() == 2)
                    {
                        var LeftPort = port.FirstOrDefault(i => i.HostEQPortID.EndsWith("L"));
                        var RightPort = port.FirstOrDefault(i => i.HostEQPortID.EndsWith("R"));
                        _mcsIOPorts.Add(PortID, new MCSIOPort(stocker, _dataCollectionEvents, taskInfo, loggerService, PortID, _stocker.GetIOPortById(LeftPort.PortTypeIndex), _stocker.GetIOPortById(RightPort.PortTypeIndex), _alarmService));
                    }
                }
            }
        }

        private void HeartBeat()
        {
            foreach (var port in _mcsIOPorts.Values)
            {
                port.RefreshStatus();
            }
        }

        public void SetIOPortDirReq(string PortID, StockerEnums.IOPortDirection portUnitType)
        {
            var IOPort = _mcsIOPorts.FirstOrDefault(i => i.Key == PortID);
            IOPort.Value.SetIOPortDirReq(portUnitType);
        }

        public void ResumeRefreshStatus()
        {
            _heartBeat.Start();
        }

        public void StopRefreshStatus()
        {
            _heartBeat.Pause();
        }
    }

}
