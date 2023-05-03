using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mirle.DataBase;
using Mirle.LCS.Models;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.R46YP320.STK.TaskService;
using Mirle.Stocker;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.TraceLog;

namespace Mirle.R46YP320.STK.MCS
{
    public enum ForkingStatus
    {
        Initial,
        Start,
        WaitForRisedOrDowned,
        Rised,
        Downed,
        WaitForCompleted,
        Completed,
        Finish
    }

    public class Fork
    {
        private readonly Crane _crane;
        private readonly Stocker.R46YP320.Fork _forkSignal;
        private readonly DataCollectionEventsService _dataCollectionEventsService;
        private ForkingStatus _currentStatus = ForkingStatus.Completed;
        private ForkingStatus _nextStatus = ForkingStatus.Completed;
        private readonly RepositoriesService _repositories;
        protected readonly TaskInfo _taskInfo;

        public Fork(Crane crane, int id, string forkId, IFork forkSignal, DataCollectionEventsService dataCollectionEventsService, TaskInfo taskInfo, LoggerService loggerService)
        {
            ID = id;
            ForkID = forkId;
            _crane = crane;
            _forkSignal = forkSignal as Stocker.R46YP320.Fork;
            _dataCollectionEventsService = dataCollectionEventsService;
            _taskInfo = taskInfo;
            _repositories = new RepositoriesService(taskInfo, loggerService);
        }

        public int ID { get; }
        public string ForkID { get; }
        public string CommandID { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Dest { get; set; } = string.Empty;
        public string CarrierLoc { get; set; } = string.Empty;

        private bool _lastForkHPIsOn = true;
        private bool _lastRisedIsOn = false;
        private bool _lastDownedIsOn = false;
        private Task _checkStatusTask;
        private CancellationTokenSource _cancellationTokenSource;

        private void CheckStatusAsync(CancellationToken token)
        {
            do
            {
                try
                {
                    if (token.IsCancellationRequested) break;
                    CheckStatus();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
                }

                Task.Delay(100).Wait();
            } while (_currentStatus != ForkingStatus.Finish);
        }

        private void CheckStatus()
        {
            var newForkHPIsOn = _forkSignal.Signal.ForkHomePosition.IsOn();
            var newRisedIsOn = _forkSignal.Signal.Rised.IsOn();
            var newDownedIsOn = _forkSignal.Signal.Downed.IsOn();

            if (_lastRisedIsOn != newRisedIsOn && newRisedIsOn)
            {
                _nextStatus = ForkingStatus.Rised;
                _lastRisedIsOn = newRisedIsOn;
                CarrierLoc = Source;
            }
            else if (_lastDownedIsOn != newDownedIsOn && newDownedIsOn)
            {
                _nextStatus = ForkingStatus.Downed;
                _lastDownedIsOn = newDownedIsOn;
                CarrierLoc = ForkID;
            }
            else if (_lastForkHPIsOn != newForkHPIsOn)
            {
                if (_nextStatus != ForkingStatus.Start && newForkHPIsOn == false)
                {
                    _nextStatus = ForkingStatus.Start;
                    CarrierLoc = string.IsNullOrWhiteSpace(CarrierLoc) ? Source : ForkID;
                }
                else if (_nextStatus >= ForkingStatus.Start && newForkHPIsOn)
                {
                    _nextStatus = ForkingStatus.Completed;
                    CarrierLoc = _forkSignal.HasCarrier ? ForkID : Dest;
                }
                _lastForkHPIsOn = newForkHPIsOn;
            }

            if (_currentStatus != _nextStatus)
            {
                switch (_nextStatus)
                {
                    case ForkingStatus.Start:
                        _dataCollectionEventsService.OnForkingStarted(this, new ForkStatusEventArgs(CommandID, _crane.CraneID, CarrierLoc));
                        break;

                    case ForkingStatus.Rised:
                        _dataCollectionEventsService.OnForkRised(this, new ForkStatusEventArgs(CommandID, _crane.CraneID, CarrierLoc));
                        break;

                    case ForkingStatus.Downed:
                        _dataCollectionEventsService.OnForkDowned(this, new ForkStatusEventArgs(CommandID, _crane.CraneID, CarrierLoc));
                        break;

                    case ForkingStatus.Completed:
                        _dataCollectionEventsService.OnForkingCompleted(this, new ForkStatusEventArgs(CommandID, _crane.CraneID, CarrierLoc));
                        _lastRisedIsOn = false;
                        _lastDownedIsOn = false;
                        break;
                }
                _currentStatus = _nextStatus;
            }
        }

        public void SetInitial(string commandId)
        {
            if (_checkStatusTask != null && _checkStatusTask.IsCompleted == false && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _checkStatusTask.Wait(1000);
            }

            _currentStatus = ForkingStatus.Initial;
            _nextStatus = ForkingStatus.Initial;
            CommandID = commandId;
            CarrierLoc = string.Empty;
            Source = string.Empty;
            Dest = string.Empty;
            var taskCmds = _repositories.GetTaskByCommandID(commandId, _crane.ID);
            foreach (var item in taskCmds)
            {
                switch (item.TransferMode)
                {
                    case (int)TransferMode.FROM:
                        using (DB db = _taskInfo.GetDB())
                        {
                            Source = GetVShelfInfo(db, item.Source)?.CarrierLoc;
                            Dest = ForkID;
                        }
                        break;

                    case (int)TransferMode.TO:
                        using (DB db = _taskInfo.GetDB())
                        {
                            Source = string.IsNullOrWhiteSpace(Source) ? ForkID : Source;
                            Dest = GetVShelfInfo(db, item.Destination)?.CarrierLoc;
                        }
                        break;

                    case (int)TransferMode.SCAN:
                        using (DB db = _taskInfo.GetDB())
                        {
                            Source = GetVShelfInfo(db, item.Source)?.CarrierLoc;
                            Dest = GetVShelfInfo(db, item.Source)?.CarrierLoc;
                        }
                        break;
                }
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _checkStatusTask = Task.Run(() =>
            {
                CheckStatusAsync(_cancellationTokenSource.Token);
            });
        }

        public void SetFinish()
        {
            _nextStatus = ForkingStatus.Finish;
        }

        private VShelfInfo GetVShelfInfo(DB db, string shelfId)
        {
            if (!_repositories.GetShelfInfoByShelfID(db, shelfId, out var shelf))
            {
                if (!_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(shelfId), out shelf))
                {
                    return null;
                }
            }
            return shelf;
        }
    }
}
