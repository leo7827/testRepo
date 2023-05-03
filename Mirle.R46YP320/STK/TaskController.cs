using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mirle.LCS.Models;
using Mirle.R46YP320.STK.MCS;
using Mirle.R46YP320.STK.TaskService;
using Mirle.Stocker;
using Mirle.Stocker.TaskControl;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.TraceLog;

namespace Mirle.R46YP320.STK
{
    public class TaskController : TaskControllerService
    {
        public void SaveS10F3Process(int tid, string text)
        {
        }

        public TaskController(LCSINI config, IStocker stocker, LoggerService loggerService) : base(config, stocker, loggerService)
        {
            IEnumerable<ShelfDef> craneInfos = RepositoriesService.GetCraneInfo(config, loggerService);
            ShelfDef crane11 = craneInfos.FirstOrDefault(c => c.ShelfID == "0001001");
            ShelfDef crane12 = craneInfos.FirstOrDefault(c => c.ShelfID == "0001002");
            ShelfDef crane21 = craneInfos.FirstOrDefault(c => c.ShelfID == "0002001");
            ShelfDef crane22 = craneInfos.FirstOrDefault(c => c.ShelfID == "0002002");
            Dictionary<string, CraneInfo> list = new Dictionary<string, CraneInfo>();
            if (crane11 != null)
                list.Add("11", new CraneInfo() { CraneID = crane11.ZoneID, CraneShelfID = crane11.ShelfID, CraneCarrierLoc = crane11.ZoneID + "L" });
            if (crane12 != null)
                list.Add("12", new CraneInfo() { CraneID = crane12.ZoneID, CraneShelfID = crane12.ShelfID, CraneCarrierLoc = crane12.ZoneID + "R" });
            if (crane21 != null)
                list.Add("21", new CraneInfo() { CraneID = crane21.ZoneID, CraneShelfID = crane21.ShelfID, CraneCarrierLoc = crane21.ZoneID + "L" });
            if (crane22 != null)
                list.Add("22", new CraneInfo() { CraneID = crane22.ZoneID, CraneShelfID = crane22.ShelfID, CraneCarrierLoc = crane22.ZoneID + "R" });
            _taskInfo.SetCraneDefine(list);

            IEnumerable<PortDef> portDefs = RepositoriesService.GetPortDef(config, loggerService);
            _taskInfo.SetPortDefine(portDefs.Select(row => new PortDefInfo()
            {
                PLCPortID = row.PLCPortID,
                HostEQPortID = row.HostEQPortID,
                ShelfID = row.ShelfID,
                PortType = (PortType)row.PortType,
                PortLocationType = (PortLocationType)row.PortLocationType,
                PortTypeIndex = row.PortTypeIndex,
                Stage = row.Stage,
                Vehicles = row.Vehicles,
                NetHStnNo = row.NetHStnNo,
                AreaSensorStnNo = row.AreaSensorStnNo,
                ReportStage = row.ReportStage == 0 ? row.Stage : row.ReportStage,
            }));

            _dataCollectionEventsModule = new DataCollectionEventsService();
            _enhancedRemoteCommandModule = new EnhancedRemoteCommandService(_taskInfo, stocker, _dataCollectionEventsModule, loggerService);

            MCSCrane craneReport = new MCSCrane(_taskInfo, _dataCollectionEventsModule, stocker, loggerService);
            _hostCommandModule = new HostCommandService(_taskInfo, stocker, _dataCollectionEventsModule, loggerService, craneReport);
            _selectedEquipmentStatusModule = new SelectedEquipmentStatusService(_taskInfo, stocker, _dataCollectionEventsModule, loggerService);

            _updateProcessModule = new UpdateProcessService(_taskInfo, stocker, _dataCollectionEventsModule, loggerService, craneReport);
            _stockerMPLCEventModule = new StockerMPLCEventService(_taskInfo, stocker, _dataCollectionEventsModule, loggerService, craneReport);
            _taskProcessModule = new TaskProcessService(_taskInfo, stocker, _dataCollectionEventsModule, loggerService);
            _alarmProcessModule = new AlarmProcessService(_taskInfo, stocker, _dataCollectionEventsModule, loggerService);

        }

        public DataCollectionEventsService DataCollectionEvents() => (DataCollectionEventsService)_dataCollectionEventsModule;
        public HostCommandService HostCommand() => (HostCommandService)_hostCommandModule;
        public EnhancedRemoteCommandService EnhancedRemoteCommand() => (EnhancedRemoteCommandService)_enhancedRemoteCommandModule;
        public SelectedEquipmentStatusService SelectedEquipmentStatus() => (SelectedEquipmentStatusService)_selectedEquipmentStatusModule;
        public TaskProcessService TaskProcessService() => (TaskProcessService)_taskProcessModule;
        public AlarmProcessService AlarmProcess() => (AlarmProcessService)_alarmProcessModule;
        public TaskInfo StockerInfo() => _taskInfo;
        public override void Start()
        {
            _updateProcessModule.Start();
            _taskProcessModule.Start();
            _alarmProcessModule.Start();
        }
        public override void Stop()
        {
            _updateProcessModule.Stop();
            _taskProcessModule.Stop();
            _alarmProcessModule.Stop();
        }
    }
}
