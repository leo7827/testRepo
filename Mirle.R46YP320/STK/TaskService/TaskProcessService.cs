using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mirle.Structure.Info;
using Mirle.Stocker.TaskControl.Info;
using Mirle.DataBase;
using Mirle.LCS.Extensions;
using Mirle.LCS.LCSShareMemory;
using Mirle.LCS.Models;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.Stocker;
using Mirle.Stocker.Enums;
using Mirle.Stocker.TaskControl.Module;
using Mirle.Stocker.TaskControl.TraceLog;
using Mirle.Stocker.TaskControl.TraceLog.Format;

namespace Mirle.R46YP320.STK.TaskService
{
    public class TaskProcessService : TaskProcessModule
    {
        private readonly Dictionary<string, DateTime> _BatchCommand = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, DateTime> _CarrierOnPort = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, DateTime> _CarrierOnPortWaitOut = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, DateTime> _CarrierOnHandoff = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, DateTime> _CarrierOnCrane = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, DateTime> _CarrierIDOnCraneButNoPresentOn = new Dictionary<string, DateTime>();
        private readonly string[] _CraneIdleTime = new string[3] { string.Empty, string.Empty, string.Empty, };
        private readonly DataCollectionEventsService _dataCollectionEventsService;
        private readonly RepositoriesService _repositories;
        private readonly SelectTask _selectTaskForTwinFork;
        private readonly IEnumerable<TwoShelf> _allTwoShelves;
        private DateTime _lastUpdatePriorityDateTime;

        private DateTime _lastDeleteHistory = DateTime.MinValue;

        public TaskProcessService(TaskInfo taskInfo, IStocker stocker, DataCollectionEventsModule dataCollectionEventsService, LoggerService loggerService) : base(taskInfo, stocker, loggerService)
        {
            _dataCollectionEventsService = (DataCollectionEventsService)dataCollectionEventsService;
            _repositories = new RepositoriesService(taskInfo, loggerService);
            _selectTaskForTwinFork = new SelectTask(_TaskInfo.Config.SystemConfig.MinBay, _TaskInfo.Config.SystemConfig.MaxBay, loggerService);
            _lastUpdatePriorityDateTime = System.DateTime.Now;
            _allTwoShelves = _repositories.GetAllTwoShelfInfo();
        }

        protected override void TaskScenariosProcess()
        {
            AbnormalProcess();
            NormalProcess();
            OtherProcess();
        }

        private void OtherProcess()
        {
            GetCraneIdleTime();
            CheckBatchCommandTimeOut();
            PriorityProcess(); 
            
            if (DateTime.Now > _lastDeleteHistory.AddDays(30) || (DateTime.Now > _lastDeleteHistory.AddDays(7) && _LCSParameter.SCState_Cur == LCSParameter.SCState.Paused))
            {
                using (var _db = _TaskInfo.GetDB())
                {
                    _repositories.DeleteHistory(_db);
                }
                _lastDeleteHistory = DateTime.Now;
            }
        }

        private void GetCraneIdleTime()
        {
            try
            {
                int iRM = 1;
                ICrane crane = _Stocker.GetCraneById(iRM);
                if (crane.IsInService && crane.IsIdle && crane.Status != StockerEnums.CraneStatus.STOP)
                {
                    if (string.IsNullOrWhiteSpace(_CraneIdleTime[iRM]))
                        _CraneIdleTime[iRM] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                else
                    _CraneIdleTime[iRM] = string.Empty;
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void CheckBatchCommandTimeOut()
        {
            try
            {
                if (_TaskInfo.Config.SystemConfig.BatchIDTimeout == 0)
                    return;

                using (var db = GetDB())
                {
                    var allCmd = _repositories.GetAllTransferCmd(db).Where(row => !string.IsNullOrWhiteSpace(row.BatchID));
                    foreach (var cmd in allCmd)
                    {
                        if (cmd.TransferState != (int)TransferState.Queue)
                        {
                            if (cmd.TransferState == (int)TransferState.Complete)
                            {
                                var shelf = _repositories.GetShelfInfoByCarrierID(db, cmd.CSTID);
                                if (shelf.CSTState != (int)CarrierState.StoreAlternate)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (_BatchCommand.ContainsKey(cmd.BatchID))
                                    _BatchCommand.Remove(cmd.BatchID);
                                continue;
                            }
                        }

                        if (!_BatchCommand.ContainsKey(cmd.BatchID))
                        {
                            _BatchCommand.Add(cmd.BatchID, DateTime.Now);
                            continue;
                        }

                        if (DateTime.Now <= _BatchCommand[cmd.BatchID].AddSeconds(_TaskInfo.Config.SystemConfig.BatchIDTimeout))
                            continue;
                        if (_repositories.GetAllTransferCmd(db).Where(row => row.BatchID == cmd.BatchID).Count() > 1)
                            continue;
                        _repositories.UpdateTransferBatchIDForLCS(db, cmd.CommandID, string.Empty);
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(cmd.CommandID, $"Clear TransferCmd: {cmd.CommandID} BatchID: {cmd.BatchID} Success"));

                        _BatchCommand.Remove(cmd.BatchID);
                    }
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void PriorityProcess()
        {
            try
            {
                //每 1 分鐘自動加乘一次 Priority
                if (_lastUpdatePriorityDateTime.AddMinutes(1) < DateTime.Now)
                {
                    _repositories.UpdatePriority();
                    _lastUpdatePriorityDateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void AbnormalProcess()
        {
            CarrierOnCraneAutoTimeout();
            CarrierIdOnCraneButNoPresentOnAutoTimeout();
        }

        private void CarrierOnCraneAutoTimeout()
        {
            try
            {
                //Timeout Set = 0 = 不啟用
                if (_TaskInfo.Config.SystemConfig.CarrierOnCraneAutoTimeout == 0)
                    return;

                int iRM = 1;
                var crane = _Stocker.GetCraneById(iRM);
                for (var iFork = 1; iFork <= 2; iFork++)
                {
                    if (!crane.IsInService ||
                        //!crane.IsIdle ||
                        crane.Status == StockerEnums.CraneStatus.STOP ||
                        !crane.GetForkById(iFork).HasCarrier)
                    {
                        if (_CarrierOnCrane.ContainsKey($"{iRM}+{iFork}"))
                            _CarrierOnCrane.Remove($"{iRM}+{iFork}");
                        continue;
                    }

                    using (var db = GetDB())
                    {
                        if (_repositories.GetShelfInfoByShelfID(db,
                            _TaskInfo.GetCraneInfo(iRM, iFork).CraneShelfID, out var info))
                        {
                            var CSTID = info.CSTID;
                            if (!string.IsNullOrWhiteSpace(CSTID) && _repositories.GetAllTransferCmd(db).Any(row => row.CSTID == CSTID))
                            {
                                if (_CarrierOnCrane.ContainsKey($"{iRM}+{iFork}"))
                                    _CarrierOnCrane.Remove($"{iRM}+{iFork}");
                                return;
                            }
                        }

                        if (_repositories.GetAllTransferCmd(db).Any(row =>
                            row.TransferMode == (int)TransferMode.TO && row.CraneNo == iRM &&
                            row.HostSource == _TaskInfo.GetCraneInfo(iRM, iFork).CraneID))
                        {
                            if (_CarrierOnCrane.ContainsKey($"{iRM}+{iFork}"))
                                _CarrierOnCrane.Remove($"{iRM}+{iFork}");
                            return;
                        }

                        if (!_CarrierOnCrane.ContainsKey($"{iRM}+{iFork}"))
                            _CarrierOnCrane.Add($"{iRM}+{iFork}", DateTime.Now);

                        if (DateTime.Now <= _CarrierOnCrane[$"{iRM}+{iFork}"]
                                .AddSeconds(_TaskInfo.Config.SystemConfig.CarrierOnCraneAutoTimeout))
                            continue;

                        string message = string.Empty;
                        string defaultMessage = "Carrier On Crane Auto Timeout";
                        string commandId = _repositories.GetTransferCommandID(db);
                        var carrierId = string.Empty;

                        if (!_repositories.GetShelfInfoByShelfID(db,
                            _TaskInfo.GetCraneInfo(iRM, iFork).CraneShelfID, out VShelfInfo carrierInfo))
                        {
                            message = defaultMessage + $", Get Crane ShelfInfo Fail, Please Check";
                            _LoggerService.WriteLogTrace(new TaskProcessTrace(commandId, carrierId,
                                _TaskInfo.GetCraneInfo(iRM, iFork).CraneShelfID, string.Empty, message));
                            continue;
                        }

                        carrierId = carrierInfo.CSTID;
                        if (!_repositories.GetAlternateShelf(db, iRM, _Stocker.GetCraneById(iRM).CurrentBay, out VShelfInfo dest))
                        {
                            message = defaultMessage + $", Get Empty Shelf Fail, Please Check";
                            _LoggerService.WriteLogTrace(new TaskProcessTrace(commandId, carrierId,
                                _TaskInfo.GetCraneInfo(iRM, iFork).CraneShelfID, string.Empty, message));
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(carrierId))
                        {
                            carrierId = GetAbnormalCstId(iRM, iFork, commandId, defaultMessage);
                            if (_CarrierOnCrane.ContainsKey($"{iRM}+{iFork}"))
                                _CarrierOnCrane.Remove($"{iRM}+{iFork}");
                            continue;
                        }

                        if (!_repositories.CreateToCommand(defaultMessage, commandId, carrierId, iRM, iFork, dest, 99, carrierInfo))
                            continue;

                        _dataCollectionEventsService.OnOperatorInitiatedAction(this,
                            new OperatorInitiatedActionEventArgs(commandId,
                                VIDEnums.CommandType.TRANSFER, carrierId,
                                _TaskInfo.GetCraneInfo(iRM, iFork).CraneID, dest.CarrierLoc, 99));

                        _CarrierOnCrane.Remove($"{iRM}+{iFork}");
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private string GetAbnormalCstId(int iRM, int iFork, string commandId, string defaultMessage)
        {
            string carrierId;
            carrierId = _repositories.GetAbnormalCSTID(AbnormalCSTIDType.Failure,
                _TaskInfo.GetCraneInfo(iRM, iFork).CraneCarrierLoc, string.Empty);
            _repositories.InsertCST(commandId, "",
                _TaskInfo.GetCraneInfo(iRM, iFork).CraneShelfID, 1, carrierId,
                CarrierState.Transfering, defaultMessage);
            return carrierId;
        }

        private void CarrierIdOnCraneButNoPresentOnAutoTimeout()
        {
            try
            {
                //Timeout Set = 0 = 不啟用
                if (_TaskInfo.Config.SystemConfig.CarrierOnCraneAutoTimeout == 0)
                    return;

                int iRM = 1;
                var crane = _Stocker.GetCraneById(iRM);
                for (var iFork = 1; iFork <= 2; iFork++)
                {
                    if (!crane.IsInService ||
                        !crane.IsIdle ||
                        crane.Status == StockerEnums.CraneStatus.STOP ||
                        crane.GetForkById(iFork).HasCarrier)
                    {
                        if (_CarrierIDOnCraneButNoPresentOn.ContainsKey($"{iRM}+{iFork}"))
                            _CarrierIDOnCraneButNoPresentOn.Remove($"{iRM}+{iFork}");
                        continue;
                    }

                    using (var db = GetDB())
                    {
                        if (!_repositories.GetShelfInfoByShelfID(db,
                            _TaskInfo.GetCraneInfo(iRM, iFork).CraneShelfID, out var info))
                            continue;

                        var cstid = info.CSTID;
                        if (string.IsNullOrWhiteSpace(cstid))
                        {
                            if (_CarrierIDOnCraneButNoPresentOn.ContainsKey($"{iRM}+{iFork}"))
                                _CarrierIDOnCraneButNoPresentOn.Remove($"{iRM}+{iFork}");
                            continue;
                        }

                        if (!_CarrierIDOnCraneButNoPresentOn.ContainsKey($"{iRM}+{iFork}"))
                            _CarrierIDOnCraneButNoPresentOn.Add($"{iRM}+{iFork}", DateTime.Now);

                        if (DateTime.Now <= _CarrierIDOnCraneButNoPresentOn[$"{iRM}+{iFork}"]
                                .AddSeconds(_TaskInfo.Config.SystemConfig.CarrierOnCraneAutoTimeout))
                            continue;

                        var commandId = string.Empty;
                        if (_repositories.GetAllTransferCmd(db).Any(row => row.CraneNo == iRM && row.CSTID == cstid))
                        {
                            var transfer = _repositories.GetAllTransferCmd(db).FirstOrDefault(row => row.CraneNo == iRM && row.CSTID == cstid);
                            CancelCommand(transfer?.CommandID);
                            commandId = transfer?.CommandID;
                        }

                        const string defaultMessage = "CarrierID On Crane But No Present On Auto Timeout";
                        _repositories.DeleteCST(commandId, "", _TaskInfo.GetCraneInfo(iRM, iFork).CraneShelfID, 1, cstid, defaultMessage);
                        _CarrierIDOnCraneButNoPresentOn.Remove($"{iRM}+{iFork}");
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void CancelCommand(string CommandID)
        {
            string message = string.Empty;
            string defaultMessage = "CancelScenarios";
            int iResult = ErrorCode.Initial;
            string strEM = "";
            try
            {
                using (DB _db = GetDB())
                {
                    UpdateCommandInfo infos = _repositories.GetCommandInfoByCommandID(_db, CommandID);
                    if (!_repositories.GetShelfInfoByShelfID(_db, infos.Source, out VShelfInfo main_Source))
                    {
                        if (!_repositories.GetShelfInfoByPLCPortID(_db, Convert.ToInt32(infos.Source), out main_Source))
                        {
                            message = defaultMessage + ", Get Transfer Source Fail, Please Check";
                            _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                            return;
                        }
                    }
                    if (!_repositories.GetShelfInfoByShelfID(_db, infos.Destination, out VShelfInfo main_Destination))
                    {
                        if (!_repositories.GetShelfInfoByPLCPortID(_db, Convert.ToInt32(infos.Destination), out main_Destination))
                        {
                            message = defaultMessage + ", Get Transfer Destination Fail, Please Check";
                            _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                            return;
                        }
                    }
                    _dataCollectionEventsService.OnTransferCancelInitiated(this, new TransferCancelInitiatedEventArgs(infos, main_Source.CarrierLoc));

                    if (infos.TransferMode != (int)TransferMode.MOVE && infos.TransferMode != (int)TransferMode.SCAN)
                    {
                        #region Begin
                        iResult = _db.CommitCtrl(DB.TransactionType.Begin);
                        if (iResult != ErrorCode.Success)
                        {
                            message = defaultMessage + $", Begin Fail, Result:{iResult}";
                            _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                            _dataCollectionEventsService.OnTransferCancelFailed(this, new TransferCancelFailedEventArgs(infos, main_Source.CarrierLoc));
                            return;
                        }
                        message = defaultMessage + ", Begin Success";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                        #endregion Begin

                        #region UpdateShelfDef
                        iResult = _repositories.UpdateShelfDef(_db, main_Source.ShelfID, ShelfState.Stored, ref strEM);
                        if (iResult != ErrorCode.Success)
                        {
                            message = defaultMessage + $", ShelfID:{main_Source.ShelfID}, ShelfState:{(char)ShelfState.Stored}, Update ShelfDef Fail, Message:{strEM}";
                            _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                            iResult = _db.CommitCtrl(DB.TransactionType.Rollback);
                            _dataCollectionEventsService.OnTransferCancelFailed(this, new TransferCancelFailedEventArgs(infos, main_Source.CarrierLoc));
                            return;
                        }
                        message = defaultMessage + $", ShelfID:{main_Source.ShelfID}, Source:{(char)ShelfState.Stored}, Update ShelfDef Success";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                        #endregion UpdateShelfDef

                        #region UpdateShelfDef
                        iResult = _repositories.UpdateShelfDef(_db, main_Destination.ShelfID, ShelfState.EmptyShelf, ref strEM);
                        if (iResult != ErrorCode.Success)
                        {
                            message = defaultMessage + $", ShelfID:{main_Destination.ShelfID}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Fail, Message:{strEM}";
                            _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                            iResult = _db.CommitCtrl(DB.TransactionType.Rollback);
                            _dataCollectionEventsService.OnTransferCancelFailed(this, new TransferCancelFailedEventArgs(infos, main_Source.CarrierLoc));
                            return;
                        }
                        message = defaultMessage + $", ShelfID:{main_Destination.ShelfID}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Success";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                        #endregion UpdateShelfDef

                        #region Commit  
                        iResult = _db.CommitCtrl(DB.TransactionType.Commit);
                        if (iResult != ErrorCode.Success)
                        {
                            message = defaultMessage + $", Begin Fail, Result={iResult}";
                            _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                            _dataCollectionEventsService.OnTransferCancelFailed(this, new TransferCancelFailedEventArgs(infos, main_Source.CarrierLoc));
                            return;
                        }
                        message = defaultMessage + ", Commit Success";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                        #endregion Commit
                    }

                    #region Begin
                    iResult = _db.CommitCtrl(DB.TransactionType.Begin);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", Begin Fail, Result:{iResult}";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                        _dataCollectionEventsService.OnTransferCancelFailed(this, new TransferCancelFailedEventArgs(infos, main_Source.CarrierLoc));
                        return;
                    }
                    message = defaultMessage + ", Begin Success";
                    _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                    #endregion Begin

                    #region UpdateTransferState
                    iResult = _repositories.UpdateTransferStateToCancelOK(_db, infos);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", Update TransferState Fail, Result:{iResult}";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                        _db.CommitCtrl(DB.TransactionType.Rollback);
                        _dataCollectionEventsService.OnTransferCancelFailed(this, new TransferCancelFailedEventArgs(infos, main_Source.CarrierLoc));
                        return;
                    }
                    message = defaultMessage + $", Update TransferState Success";
                    _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                    #endregion UpdateTransferState

                    #region DeleteTransfer
                    iResult = _repositories.DeleteTransfer(_db, infos.CommandID);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", Delete Transfer Fail, Result={iResult}";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                        _db.CommitCtrl(DB.TransactionType.Rollback);
                        _dataCollectionEventsService.OnTransferCancelFailed(this, new TransferCancelFailedEventArgs(infos, main_Source.CarrierLoc));
                        return;
                    }
                    message = defaultMessage + $", Delete Transfer Success";
                    _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                    #endregion DeleteTransfer

                    #region Commit  
                    iResult = _db.CommitCtrl(DB.TransactionType.Commit);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", Begin Fail, Result={iResult}";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                        _dataCollectionEventsService.OnTransferCancelFailed(this, new TransferCancelFailedEventArgs(infos, main_Source.CarrierLoc));
                        return;
                    }
                    message = defaultMessage + ", Commit Success";
                    _LoggerService.WriteLogTrace(new TaskProcessTrace(infos.CommandID, message));
                    #endregion Commit

                    _dataCollectionEventsService.OnTransferCancelCompleted(this, new TransferCancelCompletedEventArgs(infos, main_Source.CarrierLoc));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void NormalProcess()
        {
            try
            {
                //STK 狀態是 Auto 的才可以做動作
                if (_LCSParameter.SCState_Cur != LCSParameter.SCState.Auto)
                {
                    _TaskInfo.AddTaskCannotExecuteReason($"SCState", $"SCState is not Auto");
                    return;
                }

                _TaskInfo.RemoveTaskCannotExecuteReason("SCState");

                using (var db = GetDB())
                {
                    IEnumerable<CommandTrace> commandTraces = _repositories.GetCommandTrace(db).ToList();
                    if (!commandTraces.Any())
                    {
                        _TaskInfo.ClearTaskCannotExecuteReason();
                        return;
                    }

                    int iRM = 1;
                    for (var iFork = 1; iFork <= 2; iFork++)
                    {
                        if (ForkIsDisable(iRM, iFork))
                            continue;

                        if (_repositories.ExistsTaskExecutting(db, iRM) > 0)
                            continue;

                        if (!CanCraneReceiveNewCommand())
                        {
                            _TaskInfo.AddTaskCannotExecuteReason($"Crane {iRM}", $"Crane {iRM} Can Not Receive New Command");
                            continue;
                        }

                        if (!CanCraneReceiveNewCommand(iRM))
                        {
                            continue;
                        }

                        _TaskInfo.RemoveTaskCannotExecuteReason($"Crane{iRM}");

                        #region BatchOnDiffCrane
                        var baseCommandTrace = GetCanExeCommandsByBatch(iRM, iFork, commandTraces);
                        if (_TaskInfo.Config.SystemConfig.BatchOnDiffCrane == "Y")
                        {
                            foreach (var trace in baseCommandTrace)
                            {
                                var fork = trace.MainFork == 1 || trace.MainFork == 2 ? trace.MainFork : iFork;

                                if (CreateCraneTask(trace, fork))
                                    return;
                            }
                        }
                        #endregion BatchOnDiffCrane

                        //找出所有可執行的命令
                        baseCommandTrace = GetCanExeCommands(2, iRM, iFork, commandTraces);

                        //取得 Source/Dest 相鄰的儲位
                        var twoSourceInfo = _repositories.GetTwoShelfInfo(db, baseCommandTrace, _allTwoShelves, "Source");
                        var twoDestInfo = _repositories.GetTwoShelfInfo(db, baseCommandTrace, _allTwoShelves, "Dest");

                        foreach (var trace in baseCommandTrace)
                        {
                            IEnumerable<CommandTrace> baseCommandTraceForTwinfork = _selectTaskForTwinFork.GetCanExeCommands(baseCommandTrace, trace, twoSourceInfo, twoDestInfo, _Stocker.GetCraneById(iRM).CurrentBay);

                            if (baseCommandTraceForTwinfork != null && baseCommandTraceForTwinfork.Count() > 1 && !ForkIsDisable(iRM))
                            {
                                if (CreateCraneTwinForkTask(baseCommandTraceForTwinfork))
                                {
                                    return;
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(trace.BatchID))
                                continue;

                            var fork = trace.MainFork == 1 || trace.MainFork == 2 ? trace.MainFork : iFork;

                            //單筆命令
                            if (CreateCraneTask(trace, fork))
                                return;
                        }
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private bool ForkIsDisable(int iRM, int iFork)
        {
            switch (iRM)
            {
                case 1 when iFork == 1:
                    return !_repositories.GetShelfInfoByShelfID("0001001").Enable;
                case 1 when iFork == 2:
                    return !_repositories.GetShelfInfoByShelfID("0001002").Enable;
                case 2 when iFork == 1:
                    return !_repositories.GetShelfInfoByShelfID("0002001").Enable;
                case 2 when iFork == 2:
                    return !_repositories.GetShelfInfoByShelfID("0002002").Enable;
                default:
                    return false;
            }
        }
        private bool ForkIsDisable(int iRM)
        {
            switch (iRM)
            {
                case 1:
                    return !_repositories.GetShelfInfoByShelfID("0001001").Enable || !_repositories.GetShelfInfoByShelfID("0001002").Enable;
                case 2:
                    return !_repositories.GetShelfInfoByShelfID("0002001").Enable || !_repositories.GetShelfInfoByShelfID("0002002").Enable;
                default:
                    return false;
            }
        }

        private bool IsSourceLimitCommand(bool isSingleCrane1Mode, bool isSingleCrane2Mode, CommandTrace trace, string limitBay, int fork)
        {
            string sourceBank = Convert.ToInt16(trace.NextSourceBank).ToString();
            string sourceBay = Convert.ToInt16(trace.NextSourceBay + (isSingleCrane1Mode ? 1 : isSingleCrane2Mode ? -1 : 0)).ToString();

            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"Check Crane:{trace.NextCrane}, Fork:{fork}, SourceBay:{sourceBay}"));

            bool isTheSourceLimitDest =
                _selectTaskForTwinFork.IsTheDestLimitDest(
                    isSingleCrane1Mode,
                    isSingleCrane2Mode,
                    //trace.MainCrane, fork,
                    trace.NextCrane, fork,
                    sourceBank,
                    sourceBay,
                    _TaskInfo.Config.SystemConfig.MinBay,
                    _TaskInfo.Config.SystemConfig.MaxBay,
                    limitBay);
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"Check Source Limit:{isTheSourceLimitDest}"));
            return isTheSourceLimitDest;
        }

        private bool IsDestLimitCommand(bool isSingleCrane1Mode, bool isSingleCrane2Mode, CommandTrace trace, string limitBay, int fork)
        {
            string destBank = Convert.ToInt16(trace.NextDestBank).ToString();
            string destBay = Convert.ToInt16(trace.NextDestBay + (isSingleCrane1Mode ? 1 : isSingleCrane2Mode ? -1 : 0)).ToString();

            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"Check Crane:{trace.NextCrane}, Fork:{fork}, DestBay:{destBay}"));

            bool isTheDestLimitDest =
                _selectTaskForTwinFork.IsTheDestLimitDest(
                    isSingleCrane1Mode,
                    isSingleCrane2Mode,
                    //trace.MainCrane, fork,
                    trace.NextCrane, fork,
                    destBank, destBay,
                    _TaskInfo.Config.SystemConfig.MinBay,
                    _TaskInfo.Config.SystemConfig.MaxBay,
                    limitBay);
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"Check Dest Limit:{isTheDestLimitDest}"));
            return isTheDestLimitDest;
        }

        private IEnumerable<CommandTrace> GetCanExeCommandsByBatch(int iRM, int iFork, IEnumerable<CommandTrace> commandTraces)
        {
            ICrane crane = _Stocker.GetCraneById(iRM);
            if (crane.IsIdle && crane.GetForkById(iFork).HasCarrier)
            {
                return new List<CommandTrace>(); ;
            }
            else
            {
                var baseCommandTrace = new List<CommandTrace>();
                var trace = new List<CommandTrace>();
                var command = commandTraces.Where(cmd => !string.IsNullOrWhiteSpace(cmd.BatchID));
                foreach (var cmd1 in command)
                {
                    if (cmd1.NextCrane != iRM)
                    {
                        continue;
                    }

                    if (command.Any(row => row.BatchID == cmd1.BatchID && row.CommandID != cmd1.CommandID))
                    {
                        var cmd2 = command.First(row => row.BatchID == cmd1.BatchID && row.CommandID != cmd1.CommandID);

                        if (string.IsNullOrWhiteSpace(cmd1.NextDest))
                        {
                            trace.Add(cmd1);
                        }
                    }
                }

                if (trace.Count == 0)
                {
                    return new List<CommandTrace>();
                }

                int mainRM_CurrentBay = GetMainRMCurrentBay(crane.IsIdle, crane.CurrentBay, crane.GetForkById(iFork).CurrentCommand, commandTraces);
                if (crane.IsHandOffReserved || _repositories.ExistsIntoShareArea(iRM, commandTraces, _Stocker.ShareAreaStartBay, _Stocker.ShareAreaEndBay))
                {
                    baseCommandTrace = _repositories.GetOptimalTaskByNearOrder(iRM, mainRM_CurrentBay, trace, _Stocker.HandOffStartBay, _Stocker.HandOffEndBay).Where(row => row.NextCrane == iRM).ToList();
                }
                else
                {
                    ICrane crane2 = _Stocker.GetCraneById(iRM == 1 ? 2 : 1);
                    if (crane2.IsHandOffReserved)
                    {
                        baseCommandTrace = _repositories.GetOptimalTaskByReverseOrder(iRM, mainRM_CurrentBay, trace, _Stocker.HandOffStartBay, _Stocker.HandOffEndBay).Where(row => row.NextCrane == iRM).ToList();
                    }
                    else
                    {
                        baseCommandTrace = _repositories.GetOptimalTaskByPriority(iRM, mainRM_CurrentBay, trace).Where(row => row.NextCrane == iRM && (row.MainFork == iFork || row.MainFork == 0)).ToList();
                    }
                }

                return baseCommandTrace;
            }
        }

        private IEnumerable<CommandTrace> GetCanExeCommands(int iCranceQty, int iRM, int iFork, IEnumerable<CommandTrace> commandTraces)
        {
            IEnumerable<CommandTrace> baseCommandTrace = new List<CommandTrace>();
            ICrane crane = _Stocker.GetCraneById(iRM);
            if (crane.IsIdle && crane.GetForkById(iFork).HasCarrier)
            {
                baseCommandTrace = commandTraces.Where(row => ((row.MainTransferMode == (int)TransferMode.TO ||
                                                                row.MainTransferMode == (int)TransferMode.MOVE) &&
                                                               row.NextCrane == iRM)).ToList();
            }
            else
            {
                int mainRM_CurrentBay = GetMainRMCurrentBay(crane.IsIdle, crane.CurrentBay, crane.GetForkById(iFork).CurrentCommand, commandTraces);

                if (iCranceQty >= 2)
                    baseCommandTrace = GetbaseCommandTracebyTwinFork(iRM, iFork, crane.IsHandOffReserved, commandTraces, mainRM_CurrentBay);
                else
                    baseCommandTrace = _repositories.GetOptimalTaskByPriority(iRM, mainRM_CurrentBay, commandTraces).ToList();
            }
            return baseCommandTrace;
        }

        private int GetMainRMCurrentBay(bool isCraneIdle, int craneCurrentBay, string commandID, IEnumerable<CommandTrace> commandTraces)
        {
            int mainRM_CurrentBay = 0;

            if (isCraneIdle || string.IsNullOrWhiteSpace(commandID.Replace("0", "")))
                mainRM_CurrentBay = craneCurrentBay;
            else
            {
                CommandTrace objTmp = commandTraces.FirstOrDefault(x => x.CommandID == commandID);
                if (objTmp == null)
                    mainRM_CurrentBay = craneCurrentBay;
                else
                    mainRM_CurrentBay = objTmp.ExecDestBay;
            }

            return mainRM_CurrentBay;
        }

        private IEnumerable<CommandTrace> GetbaseCommandTracebyTwinFork(int craneNo, int forkNo, bool IsHandOffReserved, IEnumerable<CommandTrace> commandTraces, int mainRM_CurrentBay)
        {
            IEnumerable<CommandTrace> baseCommandTrace = new List<CommandTrace>();
            if (IsHandOffReserved || _repositories.ExistsIntoShareArea(craneNo, commandTraces, _Stocker.ShareAreaStartBay, _Stocker.ShareAreaEndBay))
            {
                baseCommandTrace = _repositories.GetOptimalTaskByNearOrder(craneNo, mainRM_CurrentBay, commandTraces,
                    _Stocker.HandOffStartBay, _Stocker.HandOffEndBay).Where(row => row.NextCrane == craneNo).ToList();
            }
            else
            {
                ICrane crane2 = _Stocker.GetCraneById(craneNo == 1 ? 2 : 1);
                if (crane2.IsHandOffReserved)
                {
                    baseCommandTrace = _repositories.GetOptimalTaskByReverseOrder(craneNo, mainRM_CurrentBay, commandTraces,
                        _Stocker.HandOffStartBay, _Stocker.HandOffEndBay).Where(row => row.NextCrane == craneNo).ToList();
                }
                else
                    baseCommandTrace = _repositories.GetOptimalTaskByPriority(craneNo, mainRM_CurrentBay, commandTraces).Where(row => row.NextCrane == craneNo && (row.MainFork == forkNo || row.MainFork == 0)).ToList();
            }
            return baseCommandTrace;
        }

        private bool CanCraneReceiveNewCommand()
        {
            bool bolRet = true;
            if (_TaskInfo.Config.STKCConfig.ControlMode != (int)LCS.Enums.LCSEnums.ControlMode.Single)
            {
                if (_Stocker.GetCraneById(1).IsSingleCraneMode || _Stocker.GetCraneById(2).IsSingleCraneMode)
                    bolRet = true;
                else
                {
                    for (int intRMIndex = 1; intRMIndex <= _Stocker.Cranes.Count(); intRMIndex++)
                    {
                        if (_Stocker.GetCraneById(intRMIndex).IsExecutingHPReturn)
                            bolRet = bolRet & false;
                        else
                        {
                            switch (_Stocker.GetCraneById(intRMIndex).Status)
                            {
                                case StockerEnums.CraneStatus.BUSY:
                                case StockerEnums.CraneStatus.IDLE:
                                case StockerEnums.CraneStatus.ESCAPE:
                                case StockerEnums.CraneStatus.Waiting:
                                    bolRet = bolRet & true;
                                    break;

                                case StockerEnums.CraneStatus.NONE:
                                case StockerEnums.CraneStatus.NOSTS:
                                    if (!_Stocker.GetCraneById(intRMIndex).IsInService)
                                        bolRet = bolRet & false;
                                    if (!_Stocker.GetCraneById(intRMIndex).IsIdle)
                                        bolRet = bolRet & false;
                                    if (!_Stocker.GetCraneById(intRMIndex).IsKeySwitchIsAuto)
                                        bolRet = bolRet & false;
                                    break;

                                case StockerEnums.CraneStatus.HOMEACTION:
                                case StockerEnums.CraneStatus.MAINTAIN:
                                case StockerEnums.CraneStatus.STOP:
                                case StockerEnums.CraneStatus.WAITINGHOMEACTION:
                                default:
                                    bolRet = bolRet & false;
                                    break;
                            }
                        }
                        if (!bolRet)
                            break;
                    }
                }
            }
            else
                bolRet = true;

            return bolRet;
        }

        //Add Check Crane Presence And CarrierData
        private bool CanCraneReceiveNewCommand(int craneNo)
        {
            var crane = _Stocker.GetCraneById(craneNo);
            if (crane == null || !_TaskInfo.GetCraneInfo(craneNo, out var craneInfo))
            {
                //無定義
                _TaskInfo.AddTaskCannotExecuteReason($"Crane{craneNo}", $"Can't Find CraneInfo");
                return false;
            }

            var carrier = _repositories.GetCassetteData(craneInfo.CraneShelfID, out var cassettes);
            var presence = crane.GetForkById(1).HasCarrier;

            if (carrier && presence) //荷有、有帳
            {
                _TaskInfo.RemoveTaskCannotExecuteReason($"Crane{craneNo}");
                return true;
            }

            if (!carrier && !presence) //荷無、無帳
            {
                _TaskInfo.RemoveTaskCannotExecuteReason($"Crane{craneNo}");
                return true;
            }

            _TaskInfo.AddTaskCannotExecuteReason($"Crane{craneNo}", $"{(carrier ? "Have" : "No")}Carrier Data And Crane Presence {(presence ? "On" : "Off")}");
            return false;
        }

        private double GetCraneIdleTotalSeconds(int craneNo)
        {
            if (string.IsNullOrWhiteSpace(_CraneIdleTime[craneNo]))
                return 0;
            else
                return Math.Abs((DateTime.Now - Convert.ToDateTime(_CraneIdleTime[craneNo])).TotalSeconds);
        }

        private bool CreateCraneTask(CommandTrace trace, int forkNumber)
        {
            const string defaultMessage = "Task Create";
            try
            {
                using (var db = GetDB())
                {
                    if (!CheckNextDestExist(trace, db)) { return false; }

                    if (!GetSource(trace, db, defaultMessage, out var source)) { return false; }

                    if (!GetSingleForkCommandDest(trace, db, source, defaultMessage, out var dest)) { return false; }

                    var bookingSource = false;
                    var bookingDest = false;
                    var needAlternate = false;

                    if (!CheckSourceAndDestCanTransfer(db, trace, forkNumber, ref bookingSource, ref bookingDest, ref needAlternate, source, ref dest))
                    {
                        var cmd = _repositories.GetTransferCmdByCommandID(trace.CommandID);
                        var cst = _repositories.GetShelfInfoByCarrierID(db, trace.CarrierID);
                        if ((cmd.TransferState != (int)TransferState.Queue && cst.CSTState != (int)CarrierState.StoreAlternate) ||
                            dest.PortType != (int)PortType.IO ||
                            dest.PortLocationType != (int)PortLocationType.AutoPort)
                            return false;

                        var otherPort = ChooseOtherPort(dest);
                        if (otherPort == null)
                            return false;

                        if (!CheckSourceAndDestCanTransfer(db, trace, forkNumber, ref bookingSource, ref bookingDest, ref needAlternate, source, ref otherPort))
                        {
                            if (cst.ShelfType != (int)ShelfType.Port)
                                return false;

                            if (!_repositories.GetAlternateShelf(db, dest.LocateCraneNo, source, out dest))
                            {
                                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"No Dest");
                                return false;
                            }

                            return CreateTask(trace, forkNumber, db, defaultMessage, source, dest, bookingSource, bookingDest, needAlternate);
                        }

                        dest = otherPort;
                        if (dest.ShelfType == (int)ShelfType.Port)
                        {
                            UpdateTransferDest(trace, otherPort);
                            return false;
                        }
                    }

                    return CreateTask(trace, forkNumber, db, defaultMessage, source, dest, bookingSource, bookingDest, needAlternate);
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        private bool CreateChangeForkTask(CommandTrace trace, int forkNumber)
        {
            const string defaultMessage = "Task Create Change Fork";
            try
            {
                using (var db = GetDB())
                {
                    //確認是否已經Alternate
                    if (trace.MainSource != trace.NextSource)
                        return false;

                    if (!GetSource(trace, db, defaultMessage, out var source))
                        return false;

                    if (!GetSingleForkCommandDest(trace, db, source, defaultMessage, out var dest))
                        return false;

                    var bookingDest = false;
                    var needAlternate = false;

                    if (!CheckToCommandButNoHasCarrier(trace, forkNumber))
                        return false;

                    if (!CheckSource(db, trace, out var bookingSource, source, dest))
                        return false;

                    #region Check Destination
                    if (trace.NextTransferMode != (int)TransferMode.SCAN && trace.NextTransferMode != (int)TransferMode.MOVE)
                    {
                        //always alternate
                        needAlternate = true;
                    }
                    #endregion Check Destination

                    if (needAlternate)
                    {
                        if (!_repositories.GetAlternateShelf(db, dest.LocateCraneNo, source, out dest, true))
                            return false;
                    }

                    return CreateTask(trace, forkNumber, db, defaultMessage, source, dest, bookingSource, bookingDest, needAlternate);
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        private bool CreateCraneTwinForkTask(IEnumerable<CommandTrace> trace)
        {
            const string defaultMessage = "TwinFork Task Create";
            try
            {
                using (var db = GetDB())
                {
                    var commandTraces = trace.ToList();
                    var traceTmp = commandTraces.ElementAt(0);

                    if (!CheckNextDestExist(traceTmp, db))
                    {
                        return false;
                    }

                    var objFirstCmd = commandTraces.ElementAt(0);
                    var objSecondCmd = commandTraces.ElementAt(1);

                    if (objFirstCmd.MainDest == objSecondCmd.MainDest)
                    {
                        _TaskInfo.AddTaskCannotExecuteReason(objFirstCmd.CommandID, $"{objFirstCmd.CommandID} Dest is same as {objSecondCmd.CommandID} Dest");
                        _TaskInfo.AddTaskCannotExecuteReason(objSecondCmd.CommandID, $"{objSecondCmd.CommandID} Dest is same as {objFirstCmd.CommandID} Dest");
                        return false;
                    }

                    var firstBookingSource = false;
                    var firstBookingDest = false;
                    var firstNeedAlternate = false;

                    var secondBookingSource = false;
                    var secondBookingDest = false;
                    var secondNeedAlternate = false;

                    if (!GetSource(objFirstCmd, db, defaultMessage, out var firstSource)) { return false; }

                    if (!GetSource(objSecondCmd, db, defaultMessage, out var secondSource)) { return false; }

                    if (firstSource.ShelfType == (int)ShelfType.Crane && objFirstCmd.MainTransferMode == (int)TransferMode.FROM_TO)
                    {
                        _TaskInfo.AddTaskCannotExecuteReason(objFirstCmd.CommandID, $", firstSource is Crane, Please Check");
                        var message = defaultMessage + $", firstSource is Crane, Please Check";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(objFirstCmd.CommandID, objFirstCmd.CarrierID, objFirstCmd.MainSource, objFirstCmd.MainDest, message));
                        return false;
                    }

                    if (secondSource.ShelfType == (int)ShelfType.Crane && objSecondCmd.MainTransferMode == (int)TransferMode.FROM_TO)
                    {
                        _TaskInfo.AddTaskCannotExecuteReason(objSecondCmd.CommandID, $"secondSource is Crane,Please Check");
                        var message = defaultMessage + $", secondSource is Crane, Please Check";
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(objSecondCmd.CommandID, objSecondCmd.CarrierID, objSecondCmd.MainSource, objSecondCmd.MainDest, message));
                        return false;
                    }

                    var twoShelf = _repositories.GetTwoShelfInfoHandOff(db);
                    if (twoShelf.Any())
                    {
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(objFirstCmd.CommandID, $"Get HandOff:{twoShelf.First().LeftForkShelfID}"));
                        _LoggerService.WriteLogTrace(new TaskProcessTrace(objSecondCmd.CommandID, $"Get HandOff:{twoShelf.First().RightForkShelfID}"));
                    }
                    VShelfInfo tmpHandOff = new VShelfInfo();
                    if (!GetTwinForkCommandDest(objFirstCmd, 1, twoShelf, db, firstSource, defaultMessage, ref tmpHandOff, out var firstDest))
                        return false;

                    //if (!GetTwinForkCommandDest(objSecondCmd, 2, twoShelf, db, secondSource, defaultMessage, ref tmpHandOff, out var secondDest))
                    //    return false; 
                    if (!GetTwinForkCommandDest(objSecondCmd, 2, twoShelf, db, secondSource, defaultMessage, ref firstDest, out var secondDest))
                        return false;

                    if (!CheckSourceAndDestCanTransfer(db, objFirstCmd, 1, ref firstBookingSource, ref firstBookingDest, ref firstNeedAlternate, firstSource, ref firstDest))
                        return false;

                    if (!CheckSourceAndDestCanTransfer(db, objSecondCmd, 2, ref secondBookingSource, ref secondBookingDest, ref secondNeedAlternate, secondSource, ref secondDest))
                        return false;

                    ChangePort(ref objFirstCmd, ref objSecondCmd, ref firstDest, ref secondDest);
                    //檢查命令極限位置
                    var rm1 = _Stocker.GetCraneById(1).IsSingleCraneMode;
                    var rm2 = _Stocker.GetCraneById(2).IsSingleCraneMode;
                    bool isTheLimit = false;
                    isTheLimit = isTheLimit ? isTheLimit : IsSourceLimitCommand(rm1, rm2, objFirstCmd, "0", 1);
                    isTheLimit = isTheLimit ? isTheLimit : IsDestLimitCommand(rm1, rm2, objFirstCmd, "0", 1);
                    isTheLimit = isTheLimit ? isTheLimit : IsSourceLimitCommand(rm1, rm2, objSecondCmd, "0", 2);
                    isTheLimit = isTheLimit ? isTheLimit : IsDestLimitCommand(rm1, rm2, objSecondCmd, "0", 2);
                    if (isTheLimit) return false;
                    return GetTwinForkTask(db, objFirstCmd, objSecondCmd, defaultMessage, firstSource, firstDest, secondSource, secondDest, firstBookingSource, firstBookingDest, firstNeedAlternate, secondBookingSource, secondBookingDest, secondNeedAlternate);
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        private bool CheckNextDestExist(CommandTrace trace, DB db)
        {
            if (_repositories.GetShelfInfoByShelfID(db, trace.NextDest, out var nextDest))
                return true;

            if (_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(trace.NextDest), out nextDest))
                return true;

            return false;
        }

        private bool GetSingleForkCommandDest(CommandTrace trace, DB db, VShelfInfo source, string defaultMessage, out VShelfInfo dest)
        {
            string message;
            dest = null;

            if (trace.NextTransferMode == (int)TransferMode.SCAN)
                return true;

            if (string.IsNullOrWhiteSpace(trace.NextDest) && trace.NextDestBay == 0)
            {
                //尋找Handoff
                VShelfInfo tmpHandOff = new VShelfInfo();
                if (_repositories.GetHandoffShelf(db, source, tmpHandOff, out dest))
                {
                    trace.NextDest = dest.ShelfID;
                    trace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                    return true;
                }

                message = defaultMessage + $", Handoff Zone Is Full";

                if (trace.NextTransferMode == (int)TransferMode.TO)
                {
                    //如果是To命令 找不到Handoff 則 Alternate 到儲位
                    if (ToCommandAlternateShelf(source, ref dest))
                    {
                        trace.NextDest = dest.ShelfID;
                        trace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                        return true;
                    }

                    message += $", Alternate Zone Is Full";
                }

                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, message);
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, trace.CarrierID, trace.MainSource, trace.MainDest, message));
                return false;
            }

            if (_repositories.GetShelfInfoByShelfID(db, trace.NextDest, out dest))
                return true;

            if (_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(trace.NextDest), out dest))
                return true;

            _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, defaultMessage + $", Get Task Destination Fail, Please Check");
            message = defaultMessage + $", Get Task Destination Fail, Please Check";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, trace.CarrierID, trace.MainSource, trace.MainDest, message));
            return false;
        }

        private bool ToCommandAlternateShelf(VShelfInfo source, ref VShelfInfo dest)
        {
            return _repositories.GetAlternateShelf(source.LocateCraneNo, 0, null, string.Empty, out dest);
        }

        private bool GetSource(CommandTrace trace, DB db, string defaultMessage, out VShelfInfo source)
        {
            source = null;
            if (trace.NextTransferMode == (int)TransferMode.MOVE)
                return true;

            if (_repositories.GetShelfInfoByShelfID(db, trace.NextSource, out source))
                return true;

            if (_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(trace.NextSource), out source))
                return true;

            _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, defaultMessage + $", Get Task Source Fail, Please Check");
            var message = defaultMessage + $", Get Task Source Fail, Please Check";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, trace.CarrierID, trace.MainSource, trace.MainDest, message));
            return false;

        }

        private bool CreateTask(CommandTrace trace, int forkNumber, DB db, string defaultMessage, VShelfInfo source, VShelfInfo dest, bool bookingSource, bool bookingDest, bool needAlternate)
        {
            string message;
            var strTaskNo = GetTaskNo(db, trace, forkNumber);

            if (!Begin(trace, db, defaultMessage, strTaskNo, source, dest))
            {
                return false;
            }

            if (bookingSource)
            {
                #region BookingSource

                if (!UpdateShelfDef(trace, db, source, ShelfState.StorageOutReserved, defaultMessage, strTaskNo, dest))
                {
                    return false;
                }

                #endregion BookingSource
            }

            if (bookingDest || needAlternate)
            {
                #region BookingDestination

                if (!UpdateShelfDef(trace, db, dest, ShelfState.StorageInReserved, defaultMessage, strTaskNo, dest))
                {
                    return false;
                }

                #endregion BookingDestination
            }

            #region InsertTask
            int iResult;
            switch (trace.NextTransferMode)
            {
                case (int)TransferMode.MOVE:
                    iResult = _repositories.InsertTaskForMove(db, trace, strTaskNo[0], forkNumber);
                    break;
                case (int)TransferMode.SCAN:
                    iResult = _repositories.InsertTaskForScan(db, trace, strTaskNo[0], forkNumber);
                    break;
                default:
                    if (_TaskInfo.Config.SystemConfig.TransferCmdIsFromAndTo == "Y")
                        iResult = _repositories.InsertTaskByTwinFork(db, trace, strTaskNo, dest, forkNumber);
                    else
                    {
                        if (needAlternate)
                            iResult = _repositories.InsertTask(db, trace, strTaskNo[0], forkNumber, dest);
                        else
                            iResult = _repositories.InsertTask(db, trace, strTaskNo[0], forkNumber);
                    }

                    break;
            }

            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Insert Task Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID,
                    source?.ShelfID, dest?.ShelfID, message));
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }
            message = defaultMessage + ", Insert Task Success";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID, source?.ShelfID,
                dest?.ShelfID, message));

            #endregion InsertTask

            if (!UpdateTransferStateToInitialize(trace, db, defaultMessage, strTaskNo, source, dest))
            {
                return false;
            }

            if (!Commit(trace, db, defaultMessage, strTaskNo, source, dest))
            {
                return false;
            }

            return true;
        }

        private bool Begin(CommandTrace trace, DB db, string defaultMessage, string[] strTaskNo, VShelfInfo source, VShelfInfo dest)
        {
            string message;
            var iResult = db.CommitCtrl(DB.TransactionType.Begin);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Begin Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID,
                    source?.ShelfID, dest?.ShelfID, message));
                return false;
            }

            message = defaultMessage + ", Begin Success";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID, source?.ShelfID,
                dest?.ShelfID, message));
            return true;
        }

        private bool UpdateTransferStateToInitialize(CommandTrace trace, DB db, string defaultMessage, string[] strTaskNo, VShelfInfo source, VShelfInfo dest)
        {
            string message;

            var iResult = _repositories.UpdateTransferStateToInitialize(db, trace.CommandID);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Update TransferState Initialize Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }

            message = defaultMessage + ", Update TransferState Initialize Success";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));

            return true;
        }

        private bool InsertFromCommand(CommandTrace trace, DB db, string defaultMessage, string taskNo, int forkNumber, VShelfInfo source, VShelfInfo dest)
        {
            string message;

            var iResult = _repositories.InsertFrom(db, trace, taskNo, forkNumber);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Insert From Command Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }

            message = defaultMessage + ", Insert From Command Success";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));

            return true;
        }

        private bool InsertToCommand(CommandTrace trace, DB db, string defaultMessage, string taskNo, int forkNumber, VShelfInfo source, VShelfInfo dest)
        {
            string message;

            var iResult = _repositories.InsertTo(db, trace, taskNo, forkNumber, dest);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Insert To Command Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }

            message = defaultMessage + ", Insert To Command Success";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));

            return true;
        }

        private bool UpdateShelfDef(CommandTrace trace, DB db, VShelfInfo shelfInfo, ShelfState shelfState, string defaultMessage, string[] strTaskNo, VShelfInfo dest)
        {
            string strEM = "";
            string message;
            var iResult = _repositories.UpdateShelfDef(db, shelfInfo.ShelfID, shelfState, ref strEM);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Booking {(shelfState == ShelfState.StorageOutReserved ? "Source" : "Dest")} Fail, Message:{strEM}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID, shelfInfo?.ShelfID, dest?.ShelfID, message));
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }

            message = defaultMessage + $", Booking {(shelfState == ShelfState.StorageOutReserved ? "Source" : "Dest")} Success";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID, shelfInfo?.ShelfID,
                dest?.ShelfID, message));
            return true;
        }

        private bool Commit(CommandTrace trace, DB db, string defaultMessage, string[] strTaskNo, VShelfInfo source, VShelfInfo dest)
        {
            string message;

            var iResult = db.CommitCtrl(DB.TransactionType.Commit);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Begin Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID,
                    source?.ShelfID, dest?.ShelfID, message));
                return false;
            }

            message = defaultMessage + ", Commit Success";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, strTaskNo[0], trace.CarrierID, source?.ShelfID,
                dest?.ShelfID, message));

            return true;
        }

        private bool GetTwinForkTask(DB db, CommandTrace objFirstCmd, CommandTrace objSecondCmd, string defaultMessage,
            VShelfInfo firstSource, VShelfInfo firstDest, VShelfInfo secondSource, VShelfInfo secondDest,
            bool firstBookingSource, bool firstBookingDest, bool firstNeedAlternate, bool secondBookingSource,
            bool secondBookingDest, bool secondNeedAlternate)
        {
            string message;
            var arrayTaskNo = GetTaskNoForTwinFork(db, objFirstCmd, objSecondCmd);

            #region Begin

            var iResult = db.CommitCtrl(DB.TransactionType.Begin);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Begin Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(objFirstCmd.CommandID, arrayTaskNo[0], objFirstCmd.CarrierID,
                    firstSource?.ShelfID, firstDest?.ShelfID, message));
                _LoggerService.WriteLogTrace(new TaskProcessTrace(objSecondCmd.CommandID, arrayTaskNo[1],
                    objSecondCmd.CarrierID, secondSource?.ShelfID, secondDest?.ShelfID, message));
                return false;
            }

            message = defaultMessage + ", Begin Success";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(objFirstCmd.CommandID, arrayTaskNo[0], objFirstCmd.CarrierID,
                firstSource?.ShelfID, firstDest?.ShelfID, message));
            _LoggerService.WriteLogTrace(new TaskProcessTrace(objSecondCmd.CommandID, arrayTaskNo[1], objSecondCmd.CarrierID,
                secondSource?.ShelfID, secondDest?.ShelfID, message));

            #endregion Begin

            if (!Booking(db, objFirstCmd, firstBookingSource, firstSource, firstDest, arrayTaskNo[0],
                (firstBookingDest || firstNeedAlternate))) { return false; }

            if (!Booking(db, objSecondCmd, secondBookingSource, secondSource, secondDest, arrayTaskNo[1],
                (secondBookingDest || secondNeedAlternate))) { return false; }

            var taskCount = 0;
            if (objFirstCmd.MainTransferMode == (int)TransferMode.FROM_TO)
            {
                if (!InsertFromCommand(objFirstCmd, db, defaultMessage, arrayTaskNo[taskCount], 1, firstSource, firstDest)) { return false; }
                taskCount++;
            }

            if (objSecondCmd.MainTransferMode == (int)TransferMode.FROM_TO)
            {
                if (!InsertFromCommand(objSecondCmd, db, defaultMessage, arrayTaskNo[taskCount], 2, secondSource, secondDest)) { return false; }
                taskCount++;
            }

            Thread.Sleep(100);

            if (!InsertToCommand(objFirstCmd, db, defaultMessage, arrayTaskNo[taskCount], 1, firstSource, firstDest)) { return false; }
            taskCount++;

            if (!InsertToCommand(objSecondCmd, db, defaultMessage, arrayTaskNo[taskCount], 2, secondSource, secondDest)) { return false; }

            if (!UpdateTransferStateToInitialize(objFirstCmd, db, defaultMessage, arrayTaskNo, firstSource, firstDest)) { return false; }

            if (!UpdateTransferStateToInitialize(objSecondCmd, db, defaultMessage, arrayTaskNo, secondDest, secondSource)) { return false; }

            if (string.IsNullOrWhiteSpace(objFirstCmd.BatchID) && string.IsNullOrWhiteSpace(objSecondCmd.BatchID))
            {
                var batchId = $"LCS_BATCHID_{_repositories.GetBatchID(db)}";
                if (!UpdateTransferBatchIdForLcs(db, objFirstCmd, firstSource, firstDest, arrayTaskNo[0], batchId)) { return false; }
                if (!UpdateTransferBatchIdForLcs(db, objSecondCmd, secondSource, secondDest, arrayTaskNo[1], batchId)) { return false; }
            }

            #region Commit

            iResult = db.CommitCtrl(DB.TransactionType.Commit);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Commit Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(objFirstCmd.CommandID, arrayTaskNo[0], objFirstCmd.CarrierID,
                    firstSource?.ShelfID, firstDest?.ShelfID, message));
                _LoggerService.WriteLogTrace(new TaskProcessTrace(objFirstCmd.CommandID, arrayTaskNo[1], objFirstCmd.CarrierID,
                    firstSource?.ShelfID, firstDest?.ShelfID, message));
                return false;
            }

            message = defaultMessage + ", Commit Success";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(objSecondCmd.CommandID, arrayTaskNo[0], objSecondCmd.CarrierID,
                secondSource?.ShelfID, secondDest?.ShelfID, message));
            _LoggerService.WriteLogTrace(new TaskProcessTrace(objSecondCmd.CommandID, arrayTaskNo[1], objSecondCmd.CarrierID,
                secondSource?.ShelfID, secondDest?.ShelfID, message));

            #endregion Commit

            return true;
        }

        private bool GetTwinForkCommandDest(CommandTrace trace, int forkNumber, List<TwoShelf> twoShelf, DB db, VShelfInfo source, string defaultMessage, ref VShelfInfo tmpHandOff, out VShelfInfo dest)
        {
            string message;
            dest = null;
            if (string.IsNullOrWhiteSpace(trace.NextDest) && trace.NextDestBay == 0)
            {
                //尋找看是否有 兩兩成對的handOff區
                if (twoShelf.Any())
                {
                    _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, trace.CarrierID, trace.MainSource, trace.MainDest, $"{defaultMessage} fork:{forkNumber} L:{twoShelf.FirstOrDefault()?.LeftForkShelfID} R:{twoShelf.FirstOrDefault()?.RightForkShelfID}"));
                    var left = forkNumber == 1 ? twoShelf.FirstOrDefault()?.LeftForkShelfID : twoShelf.FirstOrDefault()?.RightForkShelfID;
                    if (tmpHandOff?.ShelfID == left) left = twoShelf.FirstOrDefault()?.LeftForkShelfID;
                    var leftShelf = _repositories.GetShelfInfoByShelfID(db, left);
                    dest = leftShelf;
                    trace.NextDest = leftShelf.ShelfID;
                    trace.NextDestBay = Convert.ToInt32(leftShelf.Bay_Y);
                    return true;
                }

                //如果HandOff區的格位小於兩格 就等待BatchCommand TimeOut後 一個一個搬送 
                var handOffCount = _repositories.GetAllShelfInfoOnHandOff(db, true, ShelfState.EmptyShelf, true).Count();
                if (handOffCount >= 2)
                {
                    //尋找Handoff
                    if (_repositories.GetHandoffShelf(db, source, tmpHandOff, out dest))
                    {
                        trace.NextDest = dest.ShelfID;
                        trace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                        //tmpHandOff = dest;
                        return true;
                    }
                }

                message = defaultMessage + $", Handoff Zone Is Less than two";

                if (trace.NextTransferMode == (int)TransferMode.TO)
                {
                    //如果是To命令 找不到Shelf 則 Alternate 到儲位
                    if (ToCommandAlternateShelf(source, ref dest))
                    {
                        trace.NextDest = dest.ShelfID;
                        trace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                        return true;
                    }

                    message += $", Alternate Zone Is Full";
                }

                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, message);
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, trace.CarrierID, trace.MainSource, trace.MainDest, message));
                return false;
            }

            if (_repositories.GetShelfInfoByShelfID(db, trace.NextDest, out dest)) { return true; }

            if (_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(trace.NextDest), out dest)) { return true; }

            _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, defaultMessage + $", Get Task Destination Fail, Please Check");
            message = defaultMessage + $", Get Task Destination Fail, Please Check";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, trace.CarrierID, trace.MainSource, trace.MainDest, message));
            return false;

        }

        private VShelfInfo ChooseOtherPort(VShelfInfo dest)
        {
            var destPLCPort = dest.PLCPortID;
            var otherPort = _repositories.GetAllShelfInfoByHostEQPortID(dest.ZoneID)
                .Where(i => i.Stage == 1 && i.PLCPortID != destPLCPort);
            return otherPort.FirstOrDefault();
        }

        private void ChangePort(ref CommandTrace objFirstCmd, ref CommandTrace objSecondCmd, ref VShelfInfo firstDest, ref VShelfInfo secondDest)
        {
            //手動命令不交換, 避免上報錯誤
            if (objFirstCmd.CommandID.StartsWith("MANUAL") || objSecondCmd.CommandID.StartsWith("MANUAL"))
                return;

            var firstCmd = _repositories.GetTransferCmdByCommandID(objFirstCmd.CommandID);
            var secondCmd = _repositories.GetTransferCmdByCommandID(objSecondCmd.CommandID);

            //目的地的Zone 相同才可以交換, 避免目的地越換越遠...
            if (firstCmd.HostDestination != secondCmd.HostDestination)
                return;

            //Bank1 時, 如果左手放右邊, 右手放左邊, 就修改
            if (Convert.ToInt16(firstDest.Bank_X) == 1 && Convert.ToInt16(secondDest.Bank_X) == 1)
            {
                if (Convert.ToInt16(firstDest.Bay_Y) > Convert.ToInt16(secondDest.Bay_Y))
                {
                    ChangeDest(ref firstDest, ref secondDest);
                    UpdateTransferDest(objFirstCmd, firstDest);
                    UpdateTransferDest(objSecondCmd, secondDest);
                }
            }
            //Bank2 時, 如果左手放右邊, 右手放左邊, 就修改
            else if (Convert.ToInt16(firstDest.Bank_X) == 2 && Convert.ToInt16(secondDest.Bank_X) == 2)
            {
                if (Convert.ToInt16(firstDest.Bay_Y) < Convert.ToInt16(secondDest.Bay_Y))
                {
                    ChangeDest(ref firstDest, ref secondDest);
                    UpdateTransferDest(objFirstCmd, firstDest);
                    UpdateTransferDest(objSecondCmd, secondDest);
                }
            }
        }

        private void ChangeDest(ref VShelfInfo firstDest, ref VShelfInfo secondDest)
        {
            var tmpDest = firstDest;
            firstDest = secondDest;
            secondDest = tmpDest;
        }

        private void UpdateTransferDest(CommandTrace trace, VShelfInfo shelfInfo)
        {
            _repositories.UpdateTransferCmdDest(trace.CommandID, shelfInfo.ShelfType == (int)ShelfType.Port ? shelfInfo.PLCPortID.ToString() : shelfInfo.ShelfID);
            string message = $"Update TransferCommand Dest to {(shelfInfo.ShelfType == (int)ShelfType.Port ? shelfInfo.PLCPortID.ToString() : shelfInfo.ShelfID)} Success !";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, trace.CarrierID, "", shelfInfo?.ShelfID, message));
        }

        private bool CheckSourceAndDestCanTransfer(DB db, CommandTrace trace, int forkNumber, ref bool bookingSource, ref bool bookingDest, ref bool needAlternate, VShelfInfo source, ref VShelfInfo dest)
        {
            if (!CheckScanAndSourceIsPort(trace, source))
                return false;

            //避免 P2車上有物無法取物
            //避免 P4車上有物無法Scan
            if (!CheckIsNotToCommandButCraneHasCarrier(trace, forkNumber))
                return false;

            //避免 P3車上無物無法置物
            if (!CheckToCommandButNoHasCarrier(trace, forkNumber))
                return false;

            //避免 P0來源Port無法取物
            if (!CheckSource(db, trace, out bookingSource, source, dest))
                return false;

            //避免 P1目的Port無法置物
            if (!CheckDestination(db, trace, out bookingDest, ref needAlternate, source, ref dest))
                return false;

            _TaskInfo.RemoveTaskCannotExecuteReason(trace.CommandID);
            return true;
        }

        #region Check Source
        private bool CheckSource(DB db, CommandTrace trace, out bool bookingSource, VShelfInfo source, VShelfInfo dest)
        {
            bookingSource = false;

            //Move 不用確認
            if (trace.NextTransferMode == (int)TransferMode.MOVE)
                return true;

            //Block 不做搬送
            if (!source.Enable)
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} is disable");
                return false;
            }

            //其餘的做狀態Check
            switch (source.ShelfType)
            {
                case (int)ShelfType.Shelf:
                    CheckSourceIsShelf(db, trace, out bookingSource, source);

                    break;
                case (int)ShelfType.Port:
                    if (!CheckSourceIsPort(db, trace, source, dest))
                        return false;
                    break;
            }

            return true;
        }

        private void CheckSourceIsShelf(DB db, CommandTrace trace, out bool bookingSource, VShelfInfo source)
        {
            bookingSource = false;

            //SCAN 不用預約
            if (trace.NextTransferMode == (int)TransferMode.SCAN)
                return;

            if (source.ShelfState != (int)ShelfState.Stored && !_repositories.IsHandOff(db, source.ZoneID))
                return;

            bookingSource = true;
        }

        private bool CheckSourceIsPort(DB db, CommandTrace trace, VShelfInfo source, VShelfInfo dest)
        {
            var info = _TaskInfo.GetPortInfo(source.PLCPortID);
            if (info.NetHStnNo > 0)
            {
                if (_Stocker.IsDataLinkStatusOnById(info.NetHStnNo))
                {
                    _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} DataLink Status On");
                    return false;
                }
            }

            if (info.AreaSensorStnNo > 0)
            {
                if (_Stocker.IsAreaSensorOnById(info.AreaSensorStnNo))
                {
                    _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} AreaSensor On");
                    return false;
                }
            }

            if (info.PortType == PortType.IO)
            {
                if (!CheckSourceIsIoPort(db, trace, source, info))
                {
                    return false;
                }
            }
            else if (info.PortType == PortType.EQ)
            {
                if (!CheckSourceIsEqPort(db, trace, source, dest, info))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckSourceIsEqPort(DB db, CommandTrace trace, VShelfInfo source, VShelfInfo dest, PortDefInfo info)
        {
            if (dest.LocateCraneNo == (int)LocateCraneNo.Both && _repositories.ExistsPortTaskExecutting(db, info.PLCPortID))
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} has task");
                return false;
            }

            //IsReadyToRetrieve = IsInService && Signal.UC_REQ.IsOff() && Signal.LC_REQ.IsOn();
            if (!_Stocker.GetEQPortById(info.PortTypeIndex).IsReadyToRetrieve)
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} is not ready to retrieve");
                return false;
            }

            return true;
        }

        private bool CheckSourceIsIoPort(DB db, CommandTrace trace, VShelfInfo source, PortDefInfo info)
        {
            //Port != InMode 不可搬送
            if (_Stocker.GetIOPortById(info.PortTypeIndex).Direction != StockerEnums.IOPortDirection.InMode)
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} is OutMode");
                return false;
            }

            //Port.Status = ERROR 不可搬送
            if (_Stocker.GetIOPortById(info.PortTypeIndex).Status == StockerEnums.IOPortStatus.ERROR)
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} Status is error");
                return false;
            }

            //Port out of service 不可搬送
            if (!_Stocker.GetIOPortById(info.PortTypeIndex).IsInService)
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} is out of service");
                return false;
            }

            if (source.LocateCraneNo == (int)LocateCraneNo.Both && _repositories.ExistsPortTaskExecutting(db, info.PLCPortID))
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} has task");
                return false;
            }

            //Port loadRequestStatus != Unload 不可搬送
            if (_Stocker.GetIOPortById(info.PortTypeIndex).LoadRequestStatus != StockerEnums.PortLoadRequestStatus.Unload)
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} loadRequestStatus is not unload");
                return false;
            }

            return true;
        }
        #endregion

        #region Check Destination   
        private bool CheckDestination(DB db, CommandTrace trace, out bool bookingDest, ref bool needAlternate, VShelfInfo source, ref VShelfInfo dest)
        {
            bookingDest = false;

            //SCAN 和 Move 不檢查
            if (trace.NextTransferMode == (int)TransferMode.SCAN || trace.NextTransferMode == (int)TransferMode.MOVE)
                return true;

            //Block 不可做搬送
            if (!dest.Enable)
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{dest.CarrierLoc} is disable");
                return false;
            }

            //目的地有CSTID 不可做搬送
            if (!string.IsNullOrWhiteSpace((dest.CSTID)))
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{dest.CarrierLoc} has a CSTID");
                return false;
            }

            switch (dest.ShelfType)
            {
                case (int)ShelfType.Shelf:
                    CheckDestIsShelf(db, out bookingDest, dest);
                    break;

                case (int)ShelfType.Port:
                    if (!CheckDestIsPort(db, trace, ref needAlternate, source, ref dest))
                        return false;
                    break;
            }

            return true;
        }

        private bool CheckDestIsPort(DB db, CommandTrace trace, ref bool needAlternate, VShelfInfo source, ref VShelfInfo dest)
        {
            var port = _TaskInfo.GetPortInfo(dest.PLCPortID);
            if (port.NetHStnNo > 0 && _Stocker.IsDataLinkStatusOnById(port.NetHStnNo))
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"Dest Port:{port.HostEQPortID}, NetH Signal On");
                return false;
            }

            if (port.AreaSensorStnNo > 0 && _Stocker.IsAreaSensorOnById(port.AreaSensorStnNo))
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"Dest Port:{port.HostEQPortID}, But Area Sensor Signal On");
                return false;
            }

            if (port.PortType == PortType.IO)
            {
                if (!CheckDestIsIoPort(db, trace, out needAlternate, source, dest, port))
                {
                    return false;
                }
            }
            else if (port.PortType == PortType.EQ)
            {
                if (!CheckDestIsEqPort(db, trace, out needAlternate, source, dest, port))
                {
                    return false;
                }
            }

            if (!needAlternate)
                return true;

            if (!_repositories.GetAlternateShelf(db, dest.LocateCraneNo, source, out dest))
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"No Dest");
                return false;
            }

            return true;
        }

        private bool CheckDestIsEqPort(DB db, CommandTrace trace, out bool needAlternate, VShelfInfo source, VShelfInfo dest, PortDefInfo info)
        {
            needAlternate = false;
            if (dest.LocateCraneNo == (int)LocateCraneNo.Both && _repositories.ExistsPortTaskExecutting(db, info.PLCPortID))
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{dest.CarrierLoc} has task");
                return false;
            }

            if (_Stocker.GetEQPortById(info.PortTypeIndex).IsReadyToDeposit)
                return true;

            if ((source.LocateCraneNo == dest.LocateCraneNo || source.LocateCraneNo != (int)LocateCraneNo.Both) &&
                trace.NextTransferMode != (int)TransferMode.TO && source.ShelfType != (int)ShelfType.Port)
            {
                _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID,
                    $"{dest.CarrierLoc} is not IsInService or Signal.UC_REQ.IsOff() or Signal.LC_REQ.IsOn()");
                return false;
            }

            needAlternate = true;

            return true;
        }

        private bool CheckDestIsIoPort(DB db, CommandTrace trace, out bool needAlternate, VShelfInfo source, VShelfInfo dest, PortDefInfo info)
        {
            needAlternate = false;
            if (_Stocker.GetIOPortById(info.PortTypeIndex).Direction == StockerEnums.IOPortDirection.OutMode)
            {
                if ((_Stocker.GetIOPortById(info.PortTypeIndex).Status != StockerEnums.IOPortStatus.ERROR) &&
                    (_Stocker.GetIOPortById(info.PortTypeIndex).IsInService) &&
                    (dest.LocateCraneNo != (int)LocateCraneNo.Both ||
                     !_repositories.ExistsPortTaskExecutting(db, info.PLCPortID)) &&
                    (_Stocker.GetIOPortById(info.PortTypeIndex).LoadRequestStatus ==
                     StockerEnums.PortLoadRequestStatus.Load))
                    return true;

                if ((source.ShelfType != (int)ShelfType.Crane) && (source.ShelfType != (int)ShelfType.Port) &&
                    (source.LocateCraneNo == dest.LocateCraneNo || source.LocateCraneNo != (int)LocateCraneNo.Both))
                {
                    if (_Stocker.GetIOPortById(info.PortTypeIndex).Status == StockerEnums.IOPortStatus.ERROR)
                    {
                        _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{dest.CarrierLoc} status is Error");
                    }

                    if (!_Stocker.GetIOPortById(info.PortTypeIndex).IsInService)
                    {
                        _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{dest.CarrierLoc} is out of Service");
                    }

                    if (_Stocker.GetIOPortById(info.PortTypeIndex).LoadRequestStatus != StockerEnums.PortLoadRequestStatus.Load)
                    {
                        _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{dest.CarrierLoc} status is not Load Request");
                    }

                    if (dest.LocateCraneNo == (int)LocateCraneNo.Both && _repositories.ExistsPortTaskExecutting(db, info.PLCPortID))
                    {
                        _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{dest.CarrierLoc} has task");
                    }

                    _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{dest.CarrierLoc} status is not OK");
                    return false;
                }

                string message = $"Check Destination , Need Alternate";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, trace.CarrierID, source?.ShelfID,
                    dest?.ShelfID, message));
                needAlternate = true;
            }
            else
            {
                if ((source.ShelfType == (int)ShelfType.Crane) ||
                    (source.ShelfType == (int)ShelfType.Port) ||
                    (source.LocateCraneNo != dest.LocateCraneNo && source.LocateCraneNo == (int)LocateCraneNo.Both))
                {
                    needAlternate = true;
                }
                else
                {
                    _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{dest.CarrierLoc} is InMode");
                    return false;
                }
            }

            return true;
        }

        private void CheckDestIsShelf(DB db, out bool bookingDest, VShelfInfo dest)
        {
            bookingDest = dest.ShelfState == (int)ShelfState.EmptyShelf || _repositories.IsHandOff(db, dest.ZoneID);
        }

        #endregion Check Destination

        private bool CheckToCommandButNoHasCarrier(CommandTrace trace, int forkNumber)
        {
            if (trace.NextTransferMode != (int)TransferMode.TO)
                return true;

            if (_Stocker.GetCraneById(trace.NextCrane).GetForkById(forkNumber).HasCarrier)
                return true;

            _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"TransferMode is ToCommand but there is no Cst on the crane ");
            return false;

        }

        private bool CheckIsNotToCommandButCraneHasCarrier(CommandTrace trace, int forkNumber)
        {
            if (trace.NextTransferMode == (int)TransferMode.MOVE)
                return true;

            if (trace.NextTransferMode == (int)TransferMode.TO)
                return true;

            if (!_Stocker.GetCraneById(trace.NextCrane).GetForkById(forkNumber).HasCarrier)
                return true;

            _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"TransferMode is not ToCommand but there is a Cst on the crane ");
            return false;
        }

        private bool CheckScanAndSourceIsPort(CommandTrace trace, VShelfInfo source)
        {
            if (trace.NextTransferMode != (int)TransferMode.SCAN || source.ShelfType != (int)ShelfType.Port)
                return true;

            switch (source.PortType)
            {
                case (int)PortType.IO when _Stocker.GetIOPortById(source.PortTypeIndex).LoadRequestStatus !=
                                            StockerEnums.PortLoadRequestStatus.Unload:
                    _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} loadRequestStatus is not unload");
                    return false;
                case (int)PortType.EQ when _Stocker.GetEQPortById(source.PortTypeIndex).LoadRequestStatus !=
                                            StockerEnums.PortLoadRequestStatus.Unload:
                    _TaskInfo.AddTaskCannotExecuteReason(trace.CommandID, $"{source.CarrierLoc} loadRequestStatus is not unload");
                    return false;
            }

            return true;
        }

        private bool Booking(DB _db, CommandTrace trace, bool booking_Source, VShelfInfo source, VShelfInfo dest, string taskNo, bool needBooking)
        {
            string strEM = "";
            string message = string.Empty;
            string defaultMessage = "Task Create";
            int iResult = 0;
            #region BookingSource
            if (booking_Source)
            {
                iResult = _repositories.UpdateShelfDef(_db, source.ShelfID, ShelfState.StorageOutReserved, ref strEM);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Booking Source Fail, Message:{strEM}";
                    _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));
                    _db.CommitCtrl(DB.TransactionType.Rollback);
                    return false;
                }
                message = defaultMessage + ", Booking Source Success";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));
            }
            #endregion BookingSource

            #region BookingDestination
            if (needBooking)
            {
                iResult = _repositories.UpdateShelfDef(_db, dest.ShelfID, ShelfState.StorageInReserved, ref strEM);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Booking Destination Fail, Message:{strEM}";
                    _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));
                    _db.CommitCtrl(DB.TransactionType.Rollback);
                    return false;
                }
                message = defaultMessage + ", Booking Destination Success";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));
            }
            #endregion BookingDestination
            return true;
        }

        private bool UpdateTransferBatchIdForLcs(DB _db, CommandTrace trace, VShelfInfo source, VShelfInfo dest, string taskNo, string batchId)
        {
            string message = string.Empty;
            string defaultMessage = "Task Create";
            int iResult = _repositories.UpdateTransferBatchIDForLCS(_db, trace.CommandID, batchId);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Update Transfer BatchID Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));
                _db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }
            message = defaultMessage + $", Update Transfer BatchID Success, BatchID:{batchId}";
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, taskNo, trace.CarrierID, source?.ShelfID, dest?.ShelfID, message));
            return true;
        }

        private string[] GetTaskNo(DB _db, CommandTrace trace, int forkNumber)
        {
            string[] TaskNo = new string[0];
            Array.Resize(ref TaskNo, TaskNo.Length + 1);
            TaskNo[TaskNo.Length - 1] = _repositories.GetMultiTaskNo(_db, trace.NextCrane, forkNumber, "TASK");

            if (trace.MainTransferMode == (int)TransferMode.FROM_TO || (trace.MainTransferMode == (int)TransferMode.TO && trace.NextCrane != trace.MainCrane))
            {
                Array.Resize(ref TaskNo, TaskNo.Length + 1);
                TaskNo[TaskNo.Length - 1] = _repositories.GetMultiTaskNo(_db, trace.NextCrane, forkNumber, "TASK");
            }
            return TaskNo;
        }
        private string[] GetTaskNoForTwinFork(DB _db, CommandTrace objFirstCmd, CommandTrace objSecondCmd)
        {
            string[] arrayTaskNo = new string[0];

            //第一筆 TransferCmd 的 From
            Array.Resize(ref arrayTaskNo, arrayTaskNo.Length + 1);
            arrayTaskNo[arrayTaskNo.Length - 1] = _repositories.GetMultiTaskNo(_db, objFirstCmd.NextCrane, objFirstCmd.NextFork = objFirstCmd.NextFork == 0 ? 1 : objFirstCmd.NextFork, "TASK");

            //第二筆 TransferCmd 的 From
            Array.Resize(ref arrayTaskNo, arrayTaskNo.Length + 1);
            arrayTaskNo[arrayTaskNo.Length - 1] = _repositories.GetMultiTaskNo(_db, objSecondCmd.NextCrane, objSecondCmd.NextFork = objSecondCmd.NextFork == 0 ? 2 : objSecondCmd.NextFork, "TASK");

            if (objFirstCmd.MainTransferMode == (int)TransferMode.FROM_TO || (objFirstCmd.MainTransferMode == (int)TransferMode.TO && objFirstCmd.NextCrane != objFirstCmd.MainCrane))
            {
                //第一筆 TransferCmd 的 To
                Array.Resize(ref arrayTaskNo, arrayTaskNo.Length + 1);
                arrayTaskNo[arrayTaskNo.Length - 1] = _repositories.GetMultiTaskNo(_db, objFirstCmd.NextCrane, objFirstCmd.NextFork, "TASK");
            }

            if (objSecondCmd.MainTransferMode == (int)TransferMode.FROM_TO || (objSecondCmd.MainTransferMode == (int)TransferMode.TO && objSecondCmd.NextCrane != objSecondCmd.MainCrane))
            {
                //第二筆 TransferCmd 的 To
                Array.Resize(ref arrayTaskNo, arrayTaskNo.Length + 1);
                arrayTaskNo[arrayTaskNo.Length - 1] = _repositories.GetMultiTaskNo(_db, objSecondCmd.NextCrane, objSecondCmd.NextFork, "TASK");
            }
            return arrayTaskNo;
        }
    }
}
