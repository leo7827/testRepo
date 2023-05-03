using Mirle.LCS.Models;
using Mirle.R46YP320.STK.TaskService;
using Mirle.Stocker;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.Module;
using Mirle.Stocker.TaskControl.TraceLog;
using System.Collections.Generic;
using System.Linq;

namespace Mirle.R46YP320.STK.MCS
{
    public class MCSCrane
    {
        private readonly Dictionary<int, Crane> _cranes = new Dictionary<int, Crane>();
        private readonly DataCollectionEventsService _DataCollectionEventsService;
        private readonly object _lockObj = new object();

        public MCSCrane(TaskInfo taskInfo, DataCollectionEventsModule dataCollectionEventsService, IStocker stocker, LoggerService loggerService)
        {
            _DataCollectionEventsService = (DataCollectionEventsService)dataCollectionEventsService;
            _cranes.Add(1, new Crane(1, taskInfo.GetCraneInfo(1).CraneID, _DataCollectionEventsService, stocker.GetCraneById(1), taskInfo, loggerService));
            _cranes.Add(2, new Crane(2, taskInfo.GetCraneInfo(2).CraneID, _DataCollectionEventsService, stocker.GetCraneById(2), taskInfo, loggerService));
        }

        public void SetCraneActive(string CraneID, int CraneNo, int ForkNo, string CommandID)
        {
            lock (_lockObj)
            {
                var crane = _cranes.Values.FirstOrDefault(i => i.CraneID == CraneID);
                crane?.SetActive(ForkNo, CommandID);
                crane?.SetForkStartEvent(ForkNo, CommandID);
            }
        }

        public void SetCraneIdle(string CraneID, int CraneNo, int ForkNo, string CommandID)
        {
            lock (_lockObj)
            {
                var crane = _cranes.Values.FirstOrDefault(i => i.CraneID == CraneID);
                crane?.SetIdle(ForkNo);
                crane?.SetForkCompletedEvent(ForkNo);
            }
        }
    }
}
