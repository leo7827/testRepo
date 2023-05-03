using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mirle.DataBase;
using Mirle.LCS.Extensions;
using Mirle.LCS.LCSShareMemory;
using Mirle.LCS.Models;
using Mirle.Stocker;
using Mirle.Stocker.Enums;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.Module;
using Mirle.Stocker.TaskControl.TraceLog;

namespace Mirle.R46YP320.STK.TaskService
{
    public class SelectedEquipmentStatusService : SelectedEquipmentStatusModule
    {
        private readonly DataCollectionEventsService _DataCollectionEventsService;
        private readonly RepositoriesService _Repositories;
        private readonly IStocker _Stocker;

        public SelectedEquipmentStatusService(TaskInfo taskInfo, IStocker stocker, DataCollectionEventsModule dataCollectionEventsService, LoggerService loggerService) : base(taskInfo, stocker, loggerService)
        {
            _DataCollectionEventsService = (DataCollectionEventsService)dataCollectionEventsService;
            _Repositories = new RepositoriesService(taskInfo, loggerService);
            _Stocker = stocker;
        }
        //6-ControlState
        //79-SCState
        //118-CurrentPortStates
        //351-PortTypes
        //254-UnitAlarmStatList
        //375-ActiveZones_2
        //350-CurrEQPortStatus
        //267-CurrentCraneStates
        //83-EnhancedTransfers
        //81-EnhancedCarriers
        //256-ShelfAllStats
        //280-MonitoredCranes
        public LCSParameter.ControlMode ControlState() => _LCSParameter.ControlMode_Cur;
        public LCSParameter GetLCSParameter() => _LCSParameter;
        public string SEMIVersion => "E88-0307";

        public LCSParameter.SCState SCState => _LCSParameter.SCState_Cur;

        private bool InOutService(DB _db, string PortID)
        {
            List<bool> allPortInService = new List<bool>();
            var allport = _Repositories.GetAllShelfInfoByZoneID(_db, PortID).Where(i => i.Stage == 1);
            foreach (var port in allport.Where(i => i.ZoneID == PortID))
            {
                if (port.PortType == (int)PortType.IO)
                {
                    IIOPort ioPort = _Stocker.GetIOPortById(port.PortTypeIndex);
                    allPortInService.Add(ioPort.IsInService && port.Enable);
                }
                else if (port.PortType == (int)PortType.EQ)
                {
                    IEQPort eqPort = _Stocker.GetEQPortById(port.PortTypeIndex);
                    allPortInService.Add(eqPort.IsInService && port.Enable);
                }
            }
            return allPortInService.Any(i => i == true) ? true : false;
        }

        private bool NeedAddOP(VShelfInfo row)
        {
            bool needAddOP = false;
            if (row.ShelfType == (int)ShelfType.Port)
            {
                var port = _Repositories.GetAllPortDef().FirstOrDefault(i => i.HostEQPortID.Contains(row.ZoneID));
                if (port != null && port.PortType == (int)PortType.IO)
                {
                    needAddOP = (port.ReportStage != row.Stage);
                }
            }

            return needAddOP;
        }

        public IEnumerable<AlarmData> GetAlarmDef(int alarmID)
        {
            using (DB _db = GetDB())
            {
                return _Repositories.GetAlarmDef(_db, alarmID);
            }
        }


        public IEnumerable<int> AlarmsSet()
        {
            using (DB _db = GetDB())
            {
                return _Repositories.GetCurrentAlarmSet(_db)
                    .Where(row => row.AlarmLevel == (int)AlarmLevel.Alarm)
                    .Select(row => Convert.ToInt32(row.AlarmID));
            }
        }

        public void CalibrateSystemTime() => _Stocker.CalibrateSystemTimeAsync();
    }
}
