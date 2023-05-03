using Mirle.LCS.Models;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.R46YP320.STK.TaskService;
using Mirle.Stocker;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.TraceLog;
using System.Collections.Generic;

namespace Mirle.R46YP320.STK.MCS
{
    public enum CraneRunStatus
    {
        Active,
        Idle,
    }
    public class Crane
    {
        private readonly DataCollectionEventsService _dataCollectionEventsService;
        private readonly ICrane _crane;

        public Crane(int id, string craneId, DataCollectionEventsService dataCollectionEventsService, ICrane crane, TaskInfo taskInfo, LoggerService loggerService)
        {
            ID = id;
            CraneID = craneId;
            _dataCollectionEventsService = dataCollectionEventsService;
            _crane = crane;
            LeftFork = new Fork(this, 1, $"{craneId}L", _crane.GetLeftFork(), _dataCollectionEventsService, taskInfo, loggerService);
            RightFork = new Fork(this, 2, $"{craneId}R", _crane.GetRightFork(), _dataCollectionEventsService, taskInfo, loggerService);
        }

        public int ID { get; }
        public string CraneID { get; }

        public string ReportCommandID { get; set; } = string.Empty;
        public CraneRunStatus RunStatus { get; private set; } = CraneRunStatus.Idle;

        public Fork LeftFork { get; }
        public Fork RightFork { get; }

        public IEnumerable<Fork> Forks
        {
            get
            {
                yield return LeftFork;
                yield return RightFork;
            }
        }

        public Fork GetForkById(int id)
        {
            if (id == 1)
            {
                return LeftFork;
            }
            return RightFork;
        }

        public void SetActive(int forkNo, string CommandID)
        {
            if (RunStatus == CraneRunStatus.Idle)
            {
                RunStatus = CraneRunStatus.Active;
                _dataCollectionEventsService.OnCraneActive(this, new CraneStatusEventArgs(CommandID, CraneID));
                ReportCommandID = CommandID;
            }

            if (forkNo == 1)
                LeftFork.CommandID = CommandID;
            else
                RightFork.CommandID = CommandID;
        }

        public void SetIdle(int forkNo)
        {
            if (forkNo == 1)
                LeftFork.CommandID = "";
            else
                RightFork.CommandID = "";

            if (RunStatus == CraneRunStatus.Active && string.IsNullOrWhiteSpace(LeftFork.CommandID) && string.IsNullOrWhiteSpace(RightFork.CommandID))
            {
                RunStatus = CraneRunStatus.Idle;
                _dataCollectionEventsService.OnCraneIdle(this, new CraneStatusEventArgs(ReportCommandID, CraneID));
            }
        }

        public void SetForkStartEvent(int forkNo, string commandID)
        {
            var fork = GetForkById(forkNo);
            //fork.SetForkStart(commandID);
            fork.SetInitial(commandID);
        }

        public void SetForkCompletedEvent(int forkNo)
        {
            var fork = GetForkById(forkNo);
            fork.SetFinish();
        }
    }
}
