using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mirle.Structure.Info;
using Mirle.DataBase;
using Mirle.LCS.Models;
using Mirle.LCS.Models.Define;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.R46YP320.STK.MCS;
using Mirle.Stocker;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.Module;
using Mirle.Stocker.TaskControl.TraceLog;
using Mirle.Stocker.TaskControl.TraceLog.Format;
using ErrorCode = Mirle.DataBase.ErrorCode;
using TransferCommand = Mirle.Stocker.TaskControl.Info.TransferCommand;

namespace Mirle.R46YP320.STK.TaskService
{
    public class UpdateProcessService : UpdateProcessModule
    {
        private readonly DataCollectionEventsService _dataCollectionEventsService;
        private readonly RepositoriesService _repositories;
        private readonly MCSCrane _craneReport;

        public UpdateProcessService(TaskInfo taskInfo, IStocker stocker, DataCollectionEventsModule dataCollectionEventsService, LoggerService loggerService, MCSCrane craneReport) : base(taskInfo, stocker, loggerService)
        {
            _dataCollectionEventsService = (DataCollectionEventsService)dataCollectionEventsService;
            _repositories = new RepositoriesService(taskInfo, loggerService);
            _craneReport = craneReport;
        }

        protected override void UpdateScenariosProcess()
        {
            using (var db = GetDB())
            {
                var infos = _repositories.GetUpdateCommandInfoByCancel(db);
                foreach (var info in infos)
                    CancelScenarios(info);

                infos = _repositories.GetUpdateCommandInfoByAbort(db);
                foreach (var info in infos)
                    AbortScenarios(info);

                infos = _repositories.GetUpdateCommandInfoByDelete(db);
                foreach (var info in infos)
                    DeleteScenarios(info);

                infos = _repositories.GetUpdateCommandInfo(db);
                foreach (var info in infos)
                {
                    if (info.TaskState != (int)TaskState.Complete)
                        continue;

                    info.Task_BCRReplyCSTID = string.IsNullOrWhiteSpace(info.Task_BCRReplyCSTID) ? "ERROR1" : info.Task_BCRReplyCSTID;

                    if (info.TransferMode == (int)TransferMode.SCAN)
                    {
                        switch (info.Task_CompleteCode)
                        {
                            case LCS.Models.Define.CompleteCode.TransferRequestWrong:
                            case LCS.Models.Define.CompleteCode.PickupCycle_Error:
                            case LCS.Models.Define.CompleteCode.DepositCycle_Error:
                                info.Task_BCRReplyCSTID = "ERROR1";
                                ScanCompleteScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.CommandTimeoutFromSTKC:
                                ScanResetScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.CannotScanHasCarrierOnCraneFromSTKC_P4:
                                ReportScanInitAndCraneActive(db, info);
                                ScanCompleteScenarios(info);
                                break;
                            default:
                                ScanCompleteScenarios(info);
                                break;
                        }
                    }
                    else
                    {
                        switch (info.Task_CompleteCode)
                        {
                            case LCS.Models.Define.CompleteCode.Success_FromReturnCodeAck:
                            case LCS.Models.Define.CompleteCode.Success_ToReturnCode:
                                NormalCompleteScenarios(info);
                                break;

                            case LCS.Models.Define.CompleteCode.IDMismatch:
                                IdMismatchScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.ScanIDReadError:
                            case LCS.Models.Define.CompleteCode.IDReadError:
                                IdReadErrorScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.EmptyRetrieval:
                                EmptyRetrievalScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.DoubleStorage:
                                DoubleStorageScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.TransferRequestWrong:
                            case LCS.Models.Define.CompleteCode.InlineInterlockError_OnLine:
                            case LCS.Models.Define.CompleteCode.InlineInterlockError_LD:
                            case LCS.Models.Define.CompleteCode.InlineInterlockError_ULD:
                                TransferCycleErrorScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.CannotExcuteFromSTKC:
                                IdleTimeOutResetScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.CannotDepositToDestinationPortFromSTKC_P1:
                            case LCS.Models.Define.CompleteCode.CannotDepositNoCarrierOnCraneFromSTKC_P3:
                                P1P3ResetScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.CannotRetrieveFromSourcePortFromSTKC_P0:
                            case LCS.Models.Define.CompleteCode.CannotRetrieveHasCarrierOnCraneFromSTKC_P2:
                                P0P2ResetScenarios(info);
                                break;
                            case LCS.Models.Define.CompleteCode.PickupCycle_Error:
                            case LCS.Models.Define.CompleteCode.DepositCycle_Error:
                            case LCS.Models.Define.CompleteCode.CommandTimeoutFromSTKC:
                                F1F2PEResetScenarios(info);
                                break;
                        }
                    }
                }
            }
        }

        private void ReportScanInitAndCraneActive(DB db, UpdateCommandInfo info)
        {
            _repositories.GetShelfInfoByShelfID(db, info.Source, out var source);
            _dataCollectionEventsService.OnScanInitiated(this, new ScanInitiatedEventArgs(info.CarrierID, source.CarrierLoc, source.ZoneID));
            _craneReport.SetCraneActive(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
        }

        private void CancelScenarios(UpdateCommandInfo info)
        {
            const string defaultMessage = "CancelScenarios";
            try
            {
                var transferCmd = new TransferCmd(info, defaultMessage);
                using (var db = GetDB())
                {
                    var message = defaultMessage + ", Get Transfer Source Fail, Please Check";
                    var mainSource = GetVShelfInfo(db, info, info.Source, message);

                    message = defaultMessage + ", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    _dataCollectionEventsService.OnTransferCancelInitiated(this, new TransferCancelInitiatedEventArgs(info, mainSource.CarrierLoc));

                    if (info.TransferMode != (int)TransferMode.MOVE && info.TransferMode != (int)TransferMode.SCAN)
                    {
                        if (RestoreShelfState(transferCmd, mainSource, mainDestination) == false)
                        {
                            _dataCollectionEventsService.OnTransferCancelFailed(this, new TransferCancelFailedEventArgs(info, mainSource.CarrierLoc));
                            return;
                        }
                    }

                    DeleteTransfer(transferCmd);

                    _dataCollectionEventsService.OnTransferCancelCompleted(this, new TransferCancelCompletedEventArgs(info, mainSource.CarrierLoc));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private bool RestoreShelfState(TransferCmd command, VShelfInfo source, VShelfInfo destination)
        {
            return SaveChanges(db =>
                {
                    return UpdateShelfDef(command.CommandInfo, db, source, ShelfState.Stored, command.DefaultMessage)
                           && UpdateShelfDef(command.CommandInfo, db, destination, ShelfState.EmptyShelf, command.DefaultMessage);
                },
                msg => WriteTrace(command, msg));
        }


        private void WriteTrace(TransferCmd command, string message)
        {
            var info = command.CommandInfo;
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, $"{command.DefaultMessage}, {message}"));
        }

        private void AbortScenarios(UpdateCommandInfo info)
        {
            const string defaultMessage = "AbortScenarios";
            try
            {
                using (var db = GetDB())
                {
                    var allTask = _repositories.GetAllTask(db).Where(i => i.CommandID == info.CommandID).ToList();
                    if (info.Task_CompleteCode == null ||
                        allTask.Any(i => i.CompleteCode == LCS.Models.Define.CompleteCode.EmptyRetrieval) ||
                        allTask.Any(i => i.CompleteCode == LCS.Models.Define.CompleteCode.DoubleStorage))
                        return;

                    var message = defaultMessage + ", Get Task FinishLocation Source Fail, Please Check";
                    var finishLocation = GetVShelfInfo(db, info, info.CurrentPosition, message);

                    message = defaultMessage + ", Get Transfer Source Fail, Please Check";
                    var mainSource = GetVShelfInfo(db, info, info.Source, message);

                    message = defaultMessage + ", Get Task Source Fail, Please Check";
                    var taskSource = GetVShelfInfo(db, info, info.Task_Source, message);

                    message = defaultMessage + ", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    message = defaultMessage + ", Get Task Destination Fail, Please Check";
                    var taskDestination = GetVShelfInfo(db, info, info.Task_Destination, message);

                    if (finishLocation == null || mainSource == null || taskSource == null || mainDestination == null || taskDestination == null)
                        return;

                    _dataCollectionEventsService.OnTransferAbortInitiated(this, new TransferAbortInitiatedEventArgs(info, finishLocation.CarrierLoc));

                    if (!Begin(info, db, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, mainSource.CarrierLoc));
                        return;
                    }

                    if (finishLocation.ShelfID == taskDestination.ShelfID)
                    {
                        if (mainDestination.ShelfID != taskDestination.ShelfID)
                        {
                            if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage))
                            {
                                _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, finishLocation.CarrierLoc));
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (mainDestination.ShelfID != taskDestination.ShelfID)
                        {
                            if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage))
                            {
                                _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, finishLocation.CarrierLoc));
                                return;
                            }
                        }

                        if (!UpdateShelfDef(info, db, taskDestination, ShelfState.EmptyShelf, defaultMessage))
                        {
                            _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, finishLocation.CarrierLoc));
                            return;
                        }

                        if (!UpdateShelfDef(info, db, finishLocation, ShelfState.Stored, defaultMessage))
                        {
                            _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, finishLocation.CarrierLoc));
                            return;
                        }
                    }

                    if (!UpdateCarrierData(info, db, finishLocation, CarrierState.StoreCompleted, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, finishLocation.CarrierLoc));
                        return;
                    }

                    if (!Commit(info, db, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, mainSource.CarrierLoc));
                        return;
                    }

                    DeleteTransfer(new TransferCmd(info, defaultMessage));

                    _dataCollectionEventsService.OnTransferAbortCompleted(this, new TransferAbortCompletedEventArgs(info, finishLocation.CarrierLoc));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void DeleteScenarios(UpdateCommandInfo info)
        {
            const string defaultMessage = "DeleteScenarios";
            try
            {
                DeleteTransfer(new TransferCmd(info, defaultMessage));
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void NormalCompleteScenarios(UpdateCommandInfo info)
        {
            if (info.TransferMode == (int)TransferMode.MOVE)
                NormalCompleteScenarios_Move(info);
            else
                NormalCompleteScenarios_All(info);
        }

        private void NormalCompleteScenarios_Move(UpdateCommandInfo info)
        {
            const string defaultMessage = "Normal Complete Move";
            try
            {
                using (var db = GetDB())
                {
                    if (!Begin(info, db, defaultMessage)) { return; }

                    if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                    if (!UpdateTransferState(info, db, TransferState.UpdateOK_Complete, defaultMessage)) { return; }

                    if (!Commit(info, db, defaultMessage)) { return; }

                    DeleteTransfer(new TransferCmd(info, defaultMessage));

                    _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void NormalCompleteScenarios_All(UpdateCommandInfo info)
        {
            var defaultMessage = "NormalComplete";
            try
            {
                using (var db = GetDB())
                {
                    if (!GetNormalCompleteFinishLocation(info, db, defaultMessage, out var finishLocation))
                        return;

                    var message = defaultMessage + ", Get FinishLocation Fail, Please Check";
                    var mainDest = GetVShelfInfo(db, info, info.Destination, message);
                    if (mainDest == null)
                        return;

                    var carrierState = GetCarrierState(finishLocation);

                    if (info.Destination == info.Task_Destination || finishLocation.CarrierLoc.Contains(info.HostDestination))
                    {
                        #region 已搬至目的位置

                        if (!Begin(info, db, defaultMessage)) { return; }

                        if (!UpdateShelfDef(info, db, finishLocation, ShelfState.Stored, defaultMessage)) { return; }

                        if (!UpdateCarrierData(info, db, finishLocation, carrierState, defaultMessage)) { return; }

                        if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                        if (!UpdateTransferState(info, db, TransferState.UpdateOK_Complete, defaultMessage)) { return; }

                        if (!Commit(info, db, defaultMessage)) { return; }

                        if (finishLocation.ShelfType == (int)ShelfType.Crane)
                        {
                            defaultMessage += ", Carrier On Crane ";
                            DeleteTransfer(new TransferCmd(info, defaultMessage));
                            _dataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(info, finishLocation.CarrierLoc, VIDEnums.ResultCode.Successful));
                        }
                        else if (finishLocation.ShelfType == (int)ShelfType.Shelf)
                        {
                            defaultMessage += ", Carrier On Shelf ";
                            DeleteTransfer(new TransferCmd(info, defaultMessage));
                            var resultCode = GetResultCode(db, finishLocation);
                            _dataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(info, finishLocation.CarrierLoc, resultCode));
                        }
                        else
                        {
                            var port = _TaskInfo.GetPortInfo(finishLocation.PLCPortID);

                            message = defaultMessage + $", Carrier On Port";
                            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

                            if (port.PortType == PortType.EQ || port.ReportStage == 1)
                            {
                                defaultMessage += ", Carrier On Port ";
                                DeleteTransfer(new TransferCmd(info, defaultMessage));
                                _dataCollectionEventsService.OnCarrierWaitOut(this, new CarrierWaitOutEventArgs(info.CarrierID, finishLocation.CarrierLoc, VIDEnums.PortType.LP));
                                _dataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(info, finishLocation.HostEQPort, VIDEnums.ResultCode.Successful));
                            }
                            else
                            {
                                _dataCollectionEventsService.OnCarrierWaitOut(this, new CarrierWaitOutEventArgs(info.CarrierID, finishLocation.CarrierLoc + "OP", VIDEnums.PortType.OP));
                                _dataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(info, finishLocation.HostEQPort + "OP", VIDEnums.ResultCode.Successful));
                            }
                        }

                        if (finishLocation.ShelfType != (int)ShelfType.Crane || info.TransferMode == (int)TransferMode.FROM)
                        {
                            _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                        }
                        #endregion 已搬至目的位置
                    }
                    else
                    {
                        #region 未搬至目的位置

                        if (info.AbortFlag)
                            _dataCollectionEventsService.OnTransferAbortInitiated(this, new TransferAbortInitiatedEventArgs(info, finishLocation.CarrierLoc));

                        if (!Begin(info, db, defaultMessage)) { return; }

                        if (!UpdateShelfDef(info, db, finishLocation, ShelfState.Stored, defaultMessage)) { return; }

                        if (finishLocation.ShelfType == (int)ShelfType.Crane)
                        {
                            if (!UpdateCarrierData(info, db, finishLocation, CarrierState.Transfering, defaultMessage)) { return; }
                        }
                        else
                        {
                            if (!UpdateCarrierData(info, db, finishLocation, CarrierState.StoreAlternate, defaultMessage)) { return; }
                        }

                        defaultMessage += $", Carrier On Handoff, ";
                        if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                        if (info.AbortFlag)
                        {
                            if (!UpdateTransferState(info, db, TransferState.UpdateOK_Abort, defaultMessage)) { return; }
                        }
                        else
                        {
                            if (!UpdateTransferState(info, db, TransferState.Complete, defaultMessage)) { return; }
                        }

                        if (finishLocation.ShelfType != (int)ShelfType.Crane)
                        {
                            _repositories.UpdateLCSPriority(db, info.CommandID, 900);
                        }

                        if (!Commit(info, db, defaultMessage)) { return; }

                        if (info.AbortFlag)
                        {
                            var isDeleteTransferAndTaskSuccess = DeleteTransfer(new TransferCmd(info, defaultMessage));
                            if (!isDeleteTransferAndTaskSuccess)
                            {
                                _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, finishLocation.CarrierLoc));
                                return;
                            }

                            _dataCollectionEventsService.OnTransferAbortCompleted(this, new TransferAbortCompletedEventArgs(info, finishLocation.CarrierLoc));
                        }

                        if (finishLocation.ShelfType != (int)ShelfType.Crane)
                        {
                            _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                        }
                        #endregion 未搬至目的位置
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private VIDEnums.ResultCode GetResultCode(DB db, VShelfInfo finishLocation)
        {
            var zone = _repositories.GetZoneCapacity(db, finishLocation.ZoneID);
            if (zone != null)
            {
                var resultCode = zone.ZoneCapacity == 0 ? VIDEnums.ResultCode.AllBinLocationsOccupied : VIDEnums.ResultCode.Successful;
                return resultCode;
            }
            else
            {
                return VIDEnums.ResultCode.Successful;
            }
        }

        private bool GetNormalCompleteFinishLocation(UpdateCommandInfo info, DB db, string defaultMessage, out VShelfInfo finishLocation)
        {
            if (_repositories.GetShelfInfoByShelfID(db, info.Task_FinishLocation, out finishLocation))
                return true;

            if (!_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(info.Task_FinishLocation), out finishLocation))
                return true;

            if (finishLocation == null && !string.IsNullOrWhiteSpace(info.Task_CSTTakeOffDT))
            {
                if (!_repositories.GetShelfInfoByShelfID(db, info.Task_Destination, out finishLocation))
                    _repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(info.Task_Destination), out finishLocation);
            }

            if (finishLocation == null && !string.IsNullOrWhiteSpace(info.Task_CSTOnDT))
            {
                if (!_repositories.GetShelfInfoByShelfID(db, _TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneShelfID, out finishLocation))
                    _repositories.GetShelfInfoByZoneID(db, _TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, out finishLocation);
            }

            if (finishLocation != null)
                return true;

            if (_repositories.GetShelfInfoByShelfID(db, info.Task_Source, out finishLocation))
                return true;

            if (_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(info.Task_Source), out finishLocation))
                return true;

            var message = defaultMessage + ", Get FinishLocation Fail, Please Check";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
            return false;
        }

        private void EmptyRetrievalScenarios(UpdateCommandInfo info)
        {
            try
            {
                string message;
                const string defaultMessage = "EmptyRetrieval";
                var craneId = _TaskInfo.GetCraneInfo(info.Task_CraneNo, info.Task_ForkNumber).CraneID;
                var alarmId = int.Parse(LCS.Models.Define.LCSAlarm.EmptyRetrieval_F010,
                                  System.Globalization.NumberStyles.HexNumber) + 900000;
                var stockerUnitId = craneId + (info.Task_ForkNumber == 1 ? "L" : "R");
                var errorId = VIDEnums.ErrorId.SourceEmpty.ToString() + "-" + stockerUnitId;

                if (info.TransferState != (int)TransferState.Aborting && info.TransferState != (int)TransferState.Initialize && info.TransferState != (int)TransferState.Transferring)
                {
                    using (var db = GetDB())
                    {
                        if (_repositories.GetCommandInfoByCommandID(db, info.CommandID).TransferState == (int)TransferState.Aborting)
                            return;

                        if (!UpdateTransferStateToAborting(db, info, defaultMessage))
                            return;

                        message = defaultMessage + ", Get Task Source Fail, Please Check";
                        var emptyRetrievalLocation = GetVShelfInfo(db, info, info.Task_Source, message);
                        if (emptyRetrievalLocation == null)
                            return;
                        
                        _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);

                        return;
                    }
                }

                if ((!info.AbortFlag || info.TransferState != (int)TransferState.Aborting) && !info.CommandID.Contains("MANUAL"))
                    return;

                using (var db = GetDB())
                {
                    if (_repositories.GetTransferCmdByCommandID(db, info.CommandID) == null)
                        return;

                    message = defaultMessage + ", Get Task Source Fail, Please Check";
                    var emptyRetrievalLocation = GetVShelfInfo(db, info, info.Task_Source, message);

                    message = defaultMessage + ", Get Task Destination Fail, Please Check";
                    var taskDestination = GetVShelfInfo(db, info, info.Task_Destination, message);

                    message = defaultMessage + ", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    if (emptyRetrievalLocation == null || taskDestination == null || mainDestination == null)
                        return;

                    var carrierId = _repositories.GetAbnormalCSTID(AbnormalCSTIDType.EmptyRetrieval, info.Task_Source, info.CarrierID);

                    _dataCollectionEventsService.OnTransferAbortInitiated(this, new TransferAbortInitiatedEventArgs(info, emptyRetrievalLocation.CarrierLoc));

                    var taskCmds = _repositories.GetTaskByCommandID(db, info.CommandID);

                    var capacities = _repositories.GetZoneCapacity(db, emptyRetrievalLocation.ZoneID);
                    if (!Begin(info, db, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                        return;
                    }

                    if (_TaskInfo.Config.SystemConfig.TransferCmdIsFromAndTo == "Y")
                    {
                        foreach (var item in taskCmds)
                        {
                            if (item.Destination != mainDestination.ShelfID)
                            {
                                if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage))
                                {
                                    _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                                    return;
                                }
                            }

                            //var dest = GetVShelfInfo(db, info, item.Destination, message);
                            //if (!UpdateShelfDef(info, db, dest, ShelfState.EmptyShelf, defaultMessage))
                            //{
                            //    _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                            //    return;
                            //}
                        }
                    }
                    else
                    {
                        if (taskDestination != mainDestination)
                        {
                            if (!UpdateShelfDef(info, db, taskDestination, ShelfState.EmptyShelf, defaultMessage))
                            {
                                //_dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                                //return;
                            }
                        }

                        if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage))
                        {
                            //_dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                            //return;
                        }
                    }

                    if (!InsertCarrierDataToHistory(info, db, emptyRetrievalLocation, defaultMessage))
                    {
                        //_dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                    }

                    if (!DeleteCarrierData(info, db, emptyRetrievalLocation, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                        return;
                    }

                    if (_TaskInfo.Config.SystemConfig.EmptyRetrievalCreateUNKNOWN == ((char)Enable.Enable).ToString())
                    {
                        if (!UpdateShelfDef(info, db, emptyRetrievalLocation, ShelfState.Stored, defaultMessage))
                        {
                            _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                            return;
                        }

                        if (_TaskInfo.Config.SystemConfig.EmptyRetrievalBlock == ((char)Enable.Enable).ToString())
                        {
                            if (DisableShelfDef(info, db, emptyRetrievalLocation, defaultMessage))
                            {
                                _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                                return;
                            }
                        }

                        if (!InstallCarrierData(info, db, carrierId, emptyRetrievalLocation, CarrierState.Installed, defaultMessage))
                        {
                            _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                            return;
                        }
                    }
                    else
                    {
                        if (!UpdateShelfDef(info, db, emptyRetrievalLocation, ShelfState.EmptyShelf, defaultMessage))
                        {
                            _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                            return;
                        }
                    }

                    if (!UpdateTaskState(info, db, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                        return;
                    }

                    if (!UpdateTransferState(info, db, TransferState.UpdateOK_Abort, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                        return;
                    }

                    if (!Commit(info, db, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                        return;
                    }

                    if (!DeleteTransfer(new TransferCmd(info, defaultMessage)))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, emptyRetrievalLocation.CarrierLoc));
                        return;
                    }

                    _dataCollectionEventsService.OnTransferAbortCompleted(this, new TransferAbortCompletedEventArgs(info, emptyRetrievalLocation.CarrierLoc));

                    if (info.CommandID.Contains("MANUAL"))
                    {
                        _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void DoubleStorageScenarios(UpdateCommandInfo info)
        {
            try
            {
                string message;
                var defaultMessage = "DoubleStorage";
                string carrierID;
                var strCraneID = _TaskInfo.GetCraneInfo(info.Task_CraneNo, info.Task_ForkNumber).CraneID;
                var alarmID = int.Parse(LCS.Models.Define.LCSAlarm.DoubleStorage_F011, System.Globalization.NumberStyles.HexNumber) + 900000;

                var stockerUnitID = strCraneID + (info.Task_ForkNumber == 1 ? "L" : "R");
                var errorID = VIDEnums.ErrorId.DestOccupied.ToString() + "-" + stockerUnitID;

                if (info.TransferState != (int)TransferState.Aborting)
                {
                    using (var db = GetDB())
                    {
                        if (!UpdateTransferStateToAborting(db, info, defaultMessage))
                            return;

                        message = defaultMessage + ", Get Task Source Fail, Please Check";
                        var doubleStorageLocation = GetVShelfInfo(db, info, info.Task_Source, message);
                        if (doubleStorageLocation == null)
                            return;

                        _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);

                        return;
                    }
                }

                if ((!info.AbortFlag || info.TransferState != (int)TransferState.Aborting) && !info.CommandID.Contains("MANUAL"))
                    return;

                using (var db = GetDB())
                {
                    message = defaultMessage + ", Get Task Destination Fail, Please Check";
                    var doubleStorageLocation = GetVShelfInfo(db, info, info.Task_Destination, message);

                    message = defaultMessage + ", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    message = defaultMessage + ", Get Task FinishLocation Fail, Please Check";
                    var taskFinishLocation = GetVShelfInfo(db, info, info.Task_FinishLocation, message);

                    if (doubleStorageLocation == null || mainDestination == null || taskFinishLocation == null)
                        return;

                    carrierID = _repositories.GetAbnormalCSTID(AbnormalCSTIDType.DoubleStorage, doubleStorageLocation.CarrierLoc, info.CarrierID, doubleStorageLocation.CarrierLoc);

                    _dataCollectionEventsService.OnTransferAbortInitiated(this, new TransferAbortInitiatedEventArgs(info, taskFinishLocation.CarrierLoc));

                    if (!Begin(info, db, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, doubleStorageLocation.CarrierLoc));
                        return;
                    }

                    if (mainDestination.ShelfID != doubleStorageLocation.ShelfID)
                    {
                        if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage))
                        {
                            _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, doubleStorageLocation.CarrierLoc));
                            return;
                        }
                    }

                    if (!UpdateShelfDef(info, db, doubleStorageLocation, ShelfState.Stored, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, doubleStorageLocation.CarrierLoc));
                        return;
                    }

                    if (_TaskInfo.Config.SystemConfig.DoubleStorageBlock == ((char)Enable.Enable).ToString())
                    {
                        if (DisableShelfDef(info, db, doubleStorageLocation, defaultMessage))
                        {
                            _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, doubleStorageLocation.CarrierLoc));
                            return;
                        }
                    }

                    if (!InstallCarrierData(info, db, carrierID, doubleStorageLocation, CarrierState.Installed, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, doubleStorageLocation.CarrierLoc));
                        return;
                    }

                    if (!UpdateTaskState(info, db, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, doubleStorageLocation.CarrierLoc));
                        return;
                    }

                    if (!UpdateTransferState(info, db, TransferState.UpdateOK_Abort, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, doubleStorageLocation.CarrierLoc));
                        return;
                    }

                    if (!Commit(info, db, defaultMessage))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, doubleStorageLocation.CarrierLoc));
                        return;
                    }

                    if (!DeleteTransfer(new TransferCmd(info, defaultMessage)))
                    {
                        _dataCollectionEventsService.OnTransferAbortFailed(this, new TransferAbortFailedEventArgs(info, doubleStorageLocation.CarrierLoc));
                        return;
                    }

                    _dataCollectionEventsService.OnTransferAbortCompleted(this, new TransferAbortCompletedEventArgs(info, taskFinishLocation.CarrierLoc));
                    
                    if (info.CommandID.Contains("MANUAL"))
                    {
                        _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void ScanResetScenarios(UpdateCommandInfo info)
        {
            const string defaultMessage = "Scan Reset Scenarios";
            try
            {
                using (var db = GetDB())
                {
                    if (!Begin(info, db, defaultMessage)) { return; }

                    if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                    if (!UpdateTransferState(info, db, TransferState.Complete, defaultMessage)) { return; }

                    Commit(info, db, defaultMessage);
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void ScanCompleteScenarios(UpdateCommandInfo info)
        {
            try
            {
                const string defaultMessage = "ScanComplete";
                var craneInfo = _TaskInfo.GetCraneInfo(info.Task_CraneNo, info.Task_ForkNumber);

                var bcrResult = info.Task_BCRReplyCSTID;
                var reportCstid = bcrResult;
                var reportCarrierRemove = false;
                var reportCarrierInstall = false;
                using (var db = GetDB())
                {
                    string message;
                    if (!_repositories.GetShelfInfoByShelfID(db, info.Task_FinishLocation, out var finishLocation))
                    {
                        if (!_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(info.FinishLocation), out finishLocation))
                        {
                            message = defaultMessage + $", Get Finish Location Fail, Please Check";
                            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                        }
                    }

                    if (!_repositories.GetShelfInfoByShelfID(db, info.Task_Source, out var scanCompleteLocation))
                    {
                        if (!_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(info.Task_Source), out scanCompleteLocation))
                        {
                            message = defaultMessage + $", Get Task Source Fail, Please Check";
                            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                            ScanCompleteFinishCmd(info);
                            return;
                        }
                    }

                    var idReadStatus = VIDEnums.IDReadStatus.Successful;
                    var isDuplicate = _repositories.ExistsDuplicate(db, bcrResult, scanCompleteLocation.ShelfID, out var duplicateinfo, out var commandId);

                    var scenario = new Scenario();
                    scenario.GetScanScenario(_TaskInfo.Config.SystemConfig.StockerID, craneInfo.CraneCarrierLoc, info.CarrierID, ref bcrResult, ref reportCstid, ref reportCarrierRemove, ref reportCarrierInstall, isDuplicate, ref idReadStatus);

                    _dataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(reportCstid, scanCompleteLocation.CarrierLoc, idReadStatus));
                    _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                    _dataCollectionEventsService.OnScanCompleted(this, new ScanCompletedEventArgs(info.CommandID, reportCstid, scanCompleteLocation.CarrierLoc, idReadStatus));

                    if (reportCarrierRemove)
                        ScanCompleteRemoveCarrier(info, scanCompleteLocation);

                    switch (info.Task_CompleteCode)
                    {
                        case LCS.Models.Define.CompleteCode.PickupCycle_Error:
                        case LCS.Models.Define.CompleteCode.DepositCycle_Error:
                            if (reportCarrierInstall)
                                ScanCompleteInstallCarrier(db, info, reportCstid, finishLocation);
                            break;

                        default:
                            if (reportCarrierInstall)
                                ScanCompleteInstallCarrier(db, info, reportCstid, scanCompleteLocation);
                            break;
                    }
                }

                ScanCompleteFinishCmd(info);

            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void ScanCompleteFinishCmd(UpdateCommandInfo info)
        {
            string message;
            const string defaultMessage = "ScanComplete";

            using (var db = GetDB())
            {
                #region Begin
                var iResult = db.CommitCtrl(DB.TransactionType.Begin);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", CarrierID:{info.CarrierID}, BCRResult:{info.Task_BCRReplyCSTID }, Begin Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                    return;
                }
                message = defaultMessage + $", CarrierID:{info.CarrierID}, BCRResult:{info.Task_BCRReplyCSTID }, Begin Success";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                #endregion Begin

                #region UpdateTaskState
                iResult = _repositories.UpdateTaskState(db, info);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Update TaskState Fail, Result={iResult}";
                    _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                    db.CommitCtrl(DB.TransactionType.Rollback);
                    return;
                }
                message = defaultMessage + $", Update TaskState Success";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                #endregion Begin

                #region UpdateTransferState
                iResult = _repositories.UpdateTransferState(db, info, TransferState.UpdateOK_Complete);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Update TransferState Fail, Result={iResult}";
                    _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                    db.CommitCtrl(DB.TransactionType.Rollback);
                    return;
                }
                message = defaultMessage + $", Update TransferState Success";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                #endregion UpdateTransferState

                #region Commit
                iResult = db.CommitCtrl(DB.TransactionType.Commit);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Commit Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                    //_Repositories.UpdateTransferStatToUpdateFaile(info.CommandID);
                    return;
                }
                message = defaultMessage + $", Commit Success";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                #endregion Commit

                DeleteTransfer(new TransferCmd(info, defaultMessage));
            }
        }

        private void ScanCompleteRemoveCarrier(UpdateCommandInfo info, VShelfInfo location, bool needAdd1 = false)
        {
            const string defaultMessage = "ScanComplete";

            var isDeleteCstSuccess = _repositories.DeleteCST(info.CommandID, info.TaskNo, location.ShelfID, 1, location.CSTID, defaultMessage);
        }

        private bool ScanCompleteInstallCarrier(DB _db, UpdateCommandInfo info, string carrierId, VShelfInfo location)
        {
            string defaultMessage = "ScanComplete";

            if (_repositories.GetShelfInfoByShelfID(_db, location.ShelfID, out VShelfInfo currentLocation))
            {
                if (string.IsNullOrEmpty(currentLocation.CSTID) == false)
                    ScanCompleteRemoveCarrier(info, currentLocation, true);
            }

            bool isDuplicate = _repositories.ExistsDuplicate(_db, info.Task_BCRReplyCSTID, location.ShelfID, out VShelfInfo duplicateinfo, out string commandID);

            if (isDuplicate)
            {
                var craneInfo = _TaskInfo.GetCraneInfo(info.Task_CraneNo);

                var dupCSTID = craneInfo.BCRResult;

                if (string.IsNullOrEmpty(dupCSTID) || dupCSTID.StartsWith("UNKD") == false)
                {
                    dupCSTID = _repositories.GetAbnormalCSTID(AbnormalCSTIDType.Duplicate, string.Empty, duplicateinfo.CSTID);
                }

                if (string.IsNullOrEmpty(commandID) && duplicateinfo.ShelfType == (int)ShelfType.Shelf)
                {
                    //改別人
                    ScanCompleteRemoveCarrier(info, duplicateinfo);
                    ScanCompleteInstallCarrier(_db, info, dupCSTID, duplicateinfo);
                }
                else
                {
                    //改自己
                    carrierId = dupCSTID;
                }
            }

            bool isInsertCSTSuccess = _repositories.InsertCST(info.CommandID, info.TaskNo, location.ShelfID, 1, carrierId, CarrierState.StoreCompleted, defaultMessage);
            if (!isInsertCSTSuccess)
                return isDuplicate;

            return isDuplicate;
        }

        private void IdMismatchScenarios(UpdateCommandInfo info)
        {
            const string defaultMessage = "IDMismatch";
            var carrierId = string.Empty;

            try
            {
                if (info.CarrierID == info.Task_BCRReplyCSTID)
                    return;

                using (var db = GetDB())
                {
                    var message = defaultMessage + $", Get Task Source Fail, Please Check";
                    var idMismatchLocation = GetVShelfInfo(db, info, info.Task_FinishLocation, message);

                    message = defaultMessage + $", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    message = defaultMessage + $", Get Task Destination Fail, Please Check";
                    var taskDestination = GetVShelfInfo(db, info, info.Task_Destination, message);

                    if (idMismatchLocation == null || mainDestination == null || taskDestination == null)
                        return;

                    var isDuplicate = _repositories.ExistsDuplicate(db, info.Task_BCRReplyCSTID, idMismatchLocation.ShelfID, out var duplicateinfo, out var commandID);
                    if (isDuplicate)
                        carrierId = _repositories.GetAbnormalCSTID(AbnormalCSTIDType.Duplicate, "", duplicateinfo.CSTID);

                    var idMismatchLocCarrierState = GetCarrierState(idMismatchLocation);
                    if (!Begin(info, db, defaultMessage))
                        return;

                    if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage))
                        return;

                    if (taskDestination.ShelfID != mainDestination.ShelfID)
                    {
                        if (!UpdateShelfDef(info, db, taskDestination, ShelfState.EmptyShelf, defaultMessage))
                            return;
                    }

                    if (!UpdateShelfDef(info, db, idMismatchLocation, ShelfState.Stored, defaultMessage))
                        return;

                    if (isDuplicate)
                    {
                        if (string.IsNullOrWhiteSpace(commandID))
                        {
                            if (!InsertCarrierDataToHistory(info, db, duplicateinfo, defaultMessage))
                            {
                                //return; 
                            }
                            if (!DeleteCarrierData(info, db, duplicateinfo, defaultMessage)) { return; }
                            var duplicateinfoCarrierState = GetCarrierState(duplicateinfo);
                            if (!InstallCarrierData(info, db, carrierId, duplicateinfo, duplicateinfoCarrierState, defaultMessage)) { return; }

                            if (!InsertCarrierDataToHistory(info, db, idMismatchLocation, defaultMessage))
                            {
                                //return;
                            }

                            if (!DeleteCarrierData(info, db, idMismatchLocation, defaultMessage)) { return; }
                            if (!InstallCarrierData(info, db, info.Task_BCRReplyCSTID, idMismatchLocation, idMismatchLocCarrierState, defaultMessage)) { return; }
                        }
                        else
                        {
                            if (!InsertCarrierDataToHistory(info, db, idMismatchLocation, defaultMessage))
                            {
                                //return;
                            }

                            if (!DeleteCarrierData(info, db, idMismatchLocation, defaultMessage)) { return; }

                            if (!InstallCarrierData(info, db, carrierId, idMismatchLocation, idMismatchLocCarrierState, defaultMessage)) { return; }
                        }
                    }
                    else
                    {
                        if (!InsertCarrierDataToHistory(info, db, idMismatchLocation, defaultMessage))
                        {
                            //return;
                        }

                        if (!DeleteCarrierData(info, db, idMismatchLocation, defaultMessage)) { return; }

                        if (!InstallCarrierData(info, db, info.Task_BCRReplyCSTID, idMismatchLocation, idMismatchLocCarrierState, defaultMessage)) { return; }
                    }

                    if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                    if (!UpdateTransferState(info, db, TransferState.UpdateOK_Complete, defaultMessage)) { return; }

                    if (!Commit(info, db, defaultMessage)) { return; }

                    DeleteTransfer(new TransferCmd(info, defaultMessage));

                    if (isDuplicate)
                    {
                        _dataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(info.Task_BCRReplyCSTID, idMismatchLocation.CarrierLoc, VIDEnums.IDReadStatus.Duplicate));

                        if (info.TransferMode != (int)TransferMode.SCAN)
                            _dataCollectionEventsService.OnTransferCompleted(this,
                                new TransferCompletedEventArgs(info, idMismatchLocation.CarrierLoc, VIDEnums.ResultCode.IDDuplicate));
                    }
                    else
                    {
                        _dataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(info.Task_BCRReplyCSTID, idMismatchLocation.CarrierLoc, VIDEnums.IDReadStatus.Mismatch));

                        if (info.TransferMode != (int)TransferMode.SCAN)
                            _dataCollectionEventsService.OnTransferCompleted(this,
                                new TransferCompletedEventArgs(info, idMismatchLocation.CarrierLoc, VIDEnums.ResultCode.IDMismatch));
                    }

                    _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void IdReadErrorScenarios(UpdateCommandInfo info)
        {
            const string defaultMessage = "IDReadError";
            try
            {
                if (info.Task_BCRReplyCSTID != _ReplyCSTID_NoRead && info.Task_BCRReplyCSTID != _ReplyCSTID_ReadFail && !string.IsNullOrWhiteSpace(info.Task_BCRReplyCSTID))
                    return;

                using (var db = GetDB())
                {
                    var message = defaultMessage + $", Get Task Source Fail, Please Check";
                    var idReadErrorLocation = GetVShelfInfo(db, info, info.Task_FinishLocation, message);

                    message = defaultMessage + $", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    message = defaultMessage + $", Get Task Destination Fail, Please Check";
                    var taskDestination = GetVShelfInfo(db, info, info.Task_Destination, message);

                    if (idReadErrorLocation == null || mainDestination == null || taskDestination == null)
                        return;

                    var carrierId = _TaskInfo.GetCraneBCRResult(info.Task_CraneNo, info.Task_ForkNumber);

                    _repositories.InsertIdReadErrorLog(_TaskInfo.GetCraneInfo(info.Task_CraneNo, info.Task_ForkNumber).CraneCarrierLoc, carrierId);

                    var carrierState = GetCarrierState(idReadErrorLocation);

                    if (!Begin(info, db, defaultMessage)) { return; }

                    if (mainDestination.ShelfID != taskDestination.ShelfID)
                    {
                        if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage)) { return; }
                    }

                    if (!UpdateShelfDef(info, db, taskDestination, ShelfState.EmptyShelf, defaultMessage)) { return; }

                    if (!UpdateShelfDef(info, db, idReadErrorLocation, ShelfState.Stored, defaultMessage)) { return; }

                    if (!string.IsNullOrWhiteSpace(info.CarrierID))
                    {
                        if (!InsertCarrierDataToHistory(info, db, idReadErrorLocation, defaultMessage))
                        {
                            //return;
                        }

                        if (!DeleteCarrierData(info, db, idReadErrorLocation, defaultMessage)) { return; }
                    }

                    if (!InstallCarrierData(info, db, carrierId, idReadErrorLocation, carrierState, defaultMessage)) { return; }

                    if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                    if (!UpdateTransferState(info, db, TransferState.UpdateOK_Complete, defaultMessage)) { return; }

                    if (!Commit(info, db, defaultMessage)) { return; }

                    DeleteTransfer(new TransferCmd(info, defaultMessage));

                    _dataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(carrierId, idReadErrorLocation.CarrierLoc, VIDEnums.IDReadStatus.Failure));
                    _dataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(info, idReadErrorLocation.CarrierLoc, VIDEnums.ResultCode.IDReadFailed));
                    
                    _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void TransferCycleErrorScenarios(UpdateCommandInfo info)
        {
            const string defaultMessage = "TransferCycleError";
            try
            {
                using (var db = GetDB())
                {
                    var message = defaultMessage + $", Get FinishLocation Fail, Please Check";
                    var finishLocation = GetVShelfInfo(db, info, info.Task_FinishLocation, message);

                    message = defaultMessage + $", Get Task Source Fail, Please Check";
                    var taskSource = GetVShelfInfo(db, info, info.Task_Source, message);

                    message = defaultMessage + $", Get Task Destination Fail, Please Check";
                    var taskDestination = GetVShelfInfo(db, info, info.Task_Destination, message);

                    message = defaultMessage + $", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    if (finishLocation == null || taskSource == null || taskDestination == null || mainDestination == null)
                        return;

                    var carrierState = GetCarrierState(finishLocation);

                    if (!Begin(info, db, defaultMessage)) { return; }

                    if (info.TransferMode != (int)TransferMode.MOVE && info.TransferMode != (int)TransferMode.SCAN)
                    {
                        if (taskSource.ShelfID == finishLocation.ShelfID)
                        {
                            if (!UpdateShelfDef(info, db, taskSource, ShelfState.Stored, defaultMessage)) { return; }
                        }

                        if (mainDestination.ShelfID != taskDestination.ShelfID)
                        {
                            if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage)) { return; }
                        }

                        if (finishLocation.ShelfID != taskDestination.ShelfID)
                        {
                            if (!UpdateShelfDef(info, db, taskDestination, ShelfState.EmptyShelf, defaultMessage)) { return; }
                        }

                        if (finishLocation.ShelfID == taskDestination.ShelfID)
                        {
                            if (!UpdateShelfDef(info, db, finishLocation, ShelfState.Stored, defaultMessage)) { return; }

                            if (!UpdateCarrierData(info, db, finishLocation, carrierState, defaultMessage)) { return; }
                        }
                    }

                    if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                    if (!UpdateTransferState(info, db, TransferState.UpdateOK_Complete, defaultMessage)) { return; }

                    if (!Commit(info, db, defaultMessage)) { return; }

                    DeleteTransfer(new TransferCmd(info, defaultMessage));

                    if (finishLocation.ShelfID == mainDestination.ShelfID)
                        _dataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(info, finishLocation.CarrierLoc, VIDEnums.ResultCode.Successful));
                    else
                        _dataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(info, finishLocation.CarrierLoc, VIDEnums.ResultCode.InterlockError));

                    _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);

                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void IdleTimeOutResetScenarios(UpdateCommandInfo info)
        {
            const string defaultMessage = "IdleTimeOutReset";
            try
            {
                using (var db = GetDB())
                {
                    var taskNeedUpdate = _repositories.GetAllTask(db)
                        .Where(i => i.CommandID == info.CommandID)
                        .All(i => i.CompleteCode != CompleteCode.DepositCycle_Error &&
                                  i.CompleteCode != CompleteCode.PickupCycle_Error &&
                                  i.CompleteCode != CompleteCode.CommandTimeoutFromSTKC);

                    if (!taskNeedUpdate)
                        return;

                    var message = defaultMessage + $", Get Transfer Source Fail, Please Check";
                    var mainSource = GetVShelfInfo(db, info, info.Source, message);

                    message = defaultMessage + $", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    message = defaultMessage + $", Get Task Source Fail, Please Check";
                    var taskSource = GetVShelfInfo(db, info, info.Task_Source, message);

                    message = defaultMessage + $", Get Task Destination Fail, Please Check";
                    var taskDestination = GetVShelfInfo(db, info, info.Task_Destination, message);

                    if (!Begin(info, db, defaultMessage)) { return; }

                    //確認 CST 是否在Source, 如果再Source 則不更新Shelf狀態
                    var carrierShelfInfo = _repositories.GetShelfInfoByCarrierID(db, info.CarrierID);
                    if (carrierShelfInfo == null || carrierShelfInfo.ShelfID != mainSource.ShelfID)
                    {
                        if (mainSource.ShelfID != taskSource.ShelfID)
                        {
                            if (!UpdateShelfDef(info, db, taskSource, ShelfState.Stored, defaultMessage)) { return; }
                        }
                    }

                    if (mainDestination.ShelfID != taskDestination.ShelfID)
                    {
                        if (!UpdateShelfDef(info, db, taskDestination, ShelfState.EmptyShelf, defaultMessage)) { return; }
                    }

                    if (!Commit(info, db, defaultMessage)) { return; }

                    if (!Begin(info, db, defaultMessage)) { return; }

                    if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                    if (info.Task_CompleteCode == LCS.Models.Define.CompleteCode.CannotExcuteFromSTKC)
                    {
                        if (!UpdateTransferState(info, db, TransferState.UpdateOK_Complete, defaultMessage)) { return; }
                    }
                    else
                    {
                        if (!UpdateTransferState(info, db, TransferState.Complete, defaultMessage)) { return; }
                    }

                    Commit(info, db, defaultMessage);
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void P0P2ResetScenarios(UpdateCommandInfo info)
        {
            var defaultMessage = $"{info.Task_CompleteCode} Reset Scenarios";
            try
            {
                using (var db = GetDB())
                {
                    VShelfInfo taskDestination;
                    var message = defaultMessage + $", Get Task Destination Fail, Please Check";
                    if (_TaskInfo.Config.SystemConfig.TransferCmdIsFromAndTo == "Y")
                    {
                        //TwinFork 是下 From To, 所以需要取得 To 命令的 Dest 
                        var toCommandTask = _repositories.GetTaskByCommandID(db, info.CommandID).FirstOrDefault(i => i.TransferMode == (int)TransferMode.TO && i.CraneNo == info.Task_CraneNo);
                        taskDestination = GetVShelfInfo(db, info, toCommandTask?.Destination, message);
                    }
                    else
                    {
                        //如果是下 Transfer, 則取得該筆命令的 Dest 
                        var CommandTask = _repositories.GetTaskByCommandID(db, info.CommandID).FirstOrDefault(i => i.CraneNo == info.Task_CraneNo);
                        taskDestination = GetVShelfInfo(db, info, CommandTask?.Destination, message);
                    }

                    message = defaultMessage + $", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    message = defaultMessage + $", Get Task Source Fail, Please Check";
                    var taskSource = GetVShelfInfo(db, info, info.Task_Source, message);

                    message = defaultMessage + $", Get Transfer Source Fail, Please Check";
                    var mainSource = GetVShelfInfo(db, info, info.Source, message);

                    var allTask = _repositories.GetAllTask(db)
                        .Where(i => i.CommandID == info.CommandID && i.CompleteCode == info.Task_CompleteCode)
                        .ToList();

                    if (allTask.Count < 3)
                    {
                        var trace = GetCommandTrace(db, info, info.TransferMode, info.Source);
                        var strTaskNo = GetTaskNo(db, trace);

                        if (!Begin(info, db, defaultMessage)) { return; }

                        if (!UpdateTaskStateAllTask(info, db, defaultMessage)) { return; }

                        if (!InsertTaskByTwinFork(info, db, trace, strTaskNo, taskDestination, defaultMessage)) { return; }

                        if (!UpdateTransferStateToTransferring(info, db, defaultMessage)) { return; }

                        Commit(info, db, defaultMessage);
                    }
                    else
                    {
                        if (!Begin(info, db, defaultMessage)) { return; }

                        if (!UpdateShelfDef(info, db, mainSource, ShelfState.Stored, defaultMessage)) { return; }

                        if (!UpdateShelfDef(info, db, taskDestination, ShelfState.EmptyShelf, defaultMessage)) { return; }

                        if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage)) { return; }

                        if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                        if (!UpdateTransferState(info, db, TransferState.Complete, defaultMessage)) { return; }

                        Commit(info, db, defaultMessage);

                        DeleteTransfer(new TransferCmd(info, defaultMessage));

                        _dataCollectionEventsService.OnTransferInitiated(this, new TransferInitiatedEventArgs(info, mainSource.CarrierLoc));

                        var resultCode = taskSource.ShelfType == (int)ShelfType.Port ? VIDEnums.ResultCode.InterlockError : VIDEnums.ResultCode.Unsuccessful;
                        _dataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(info, taskSource.CarrierLoc, resultCode));
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void P1P3ResetScenarios(UpdateCommandInfo info)
        {
            var defaultMessage = $"{info.Task_CompleteCode} Reset Scenarios";
            try
            {
                using (var db = GetDB())
                {
                    var carrierShelfInfo = _TaskInfo.GetCraneInfo(info.Task_CraneNo, info.Task_ForkNumber);

                    var message = defaultMessage + $", Get Task Destination Fail, Please Check";
                    var taskDestination = GetVShelfInfo(db, info, info.Task_Destination, message);

                    message = defaultMessage + $", Get Transfer Destination Fail, Please Check";
                    var mainDestination = GetVShelfInfo(db, info, info.Destination, message);

                    var allTask = _repositories.GetAllTask(db)
                        .Where(i => i.CommandID == info.CommandID && i.CompleteCode == info.Task_CompleteCode)
                        .ToList();

                    if (allTask.Count < 3)
                    {
                        var taskNo = _repositories.GetMultiTaskNo(db, info.Task_CraneNo, info.Task_ForkNumber, "TASK");
                        var trace = GetCommandTrace(db, info, (int)TransferMode.TO, carrierShelfInfo.CraneShelfID);

                        if (!Begin(info, db, defaultMessage)) { return; }

                        if (!UpdateTaskStateAllTask(info, db, defaultMessage)) { return; }

                        if (!InsertToCommand(info, db, trace, taskNo, taskDestination, defaultMessage)) { return; }

                        if (!UpdateTransferState(info, db, TransferState.Transferring, defaultMessage)) { return; }

                        Commit(info, db, defaultMessage);
                    }
                    else
                    {
                        if (!Begin(info, db, defaultMessage)) { return; }

                        if (!UpdateShelfDef(info, db, taskDestination, ShelfState.EmptyShelf, defaultMessage)) { return; }

                        if (!UpdateShelfDef(info, db, mainDestination, ShelfState.EmptyShelf, defaultMessage)) { return; }

                        if (!UpdateTaskState(info, db, defaultMessage)) { return; }

                        if (!UpdateTransferState(info, db, TransferState.Complete, defaultMessage)) { return; }

                        Commit(info, db, defaultMessage);

                        DeleteTransfer(new TransferCmd(info, defaultMessage));

                        var resultCode = taskDestination.ShelfType == (int)ShelfType.Port ? VIDEnums.ResultCode.InterlockError : VIDEnums.ResultCode.Successful;
                        _dataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(info, carrierShelfInfo.CraneCarrierLoc, resultCode));

                        _craneReport.SetCraneIdle(_TaskInfo.GetCraneInfo(info.Task_CraneNo).CraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void P4ResetScenarios(UpdateCommandInfo info)
        {
        }

        private void F1F2PEResetScenarios(UpdateCommandInfo info)
        {
            var defaultMessage = $"{info.Task_CompleteCode} Reset Scenarios";
            try
            {
                //Crane 狀態 不OK時, 不做任何事情, 直到 Crane.AvailStatus = LCSEnums.AvailStatus.Avail
                var crane = _Stocker.GetCraneById(info.Task_CraneNo) as Stocker.R46YP320.Crane;
                if (crane?.AvailStatus != LCS.Enums.LCSEnums.AvailStatus.Avail)
                    return;

                //Case 1 : On Crane
                //Case 2 : On Source
                //Case 3 : On Dest

                using (var db = GetDB())
                {
                    var allTask = _repositories.GetAllTask(db)
                        .Where(i => i.CommandID == info.CommandID && i.CompleteCode == info.Task_CompleteCode)
                        .ToList();

                    if (allTask.Count >= 3)
                    {
                        //如果 F1或F2 超過 3 次 則進入 InterLoc 流程
                        TransferCycleErrorScenarios(info);
                    }
                    else
                    {
                        var commandTask = _repositories.GetTaskByCommandID(db, info.CommandID).ToList();

                        var message = defaultMessage + $", Get Task Source Fail, Please Check";
                        var taskSource = GetVShelfInfo(db, info, info.Task_Source, message);

                        message = defaultMessage + $", Get Task Destination Fail, Please Check";
                        var taskDestination = GetVShelfInfo(db, info, info.Task_Destination, message);

                        message = defaultMessage + $", Get Finish Location Fail, Please Check";
                        var finishLoc = GetVShelfInfo(db, info, info.Task_FinishLocation, message);
                        if (_TaskInfo.Config.SystemConfig.TransferCmdIsFromAndTo == "Y")
                        {
                            if (_Stocker.GetCraneById(info.Task_CraneNo).GetForkById(info.Task_ForkNumber).HasCarrier)
                            {
                                var taskNo = _repositories.GetMultiTaskNo(db, info.Task_CraneNo, info.Task_ForkNumber, "TASK");
                                var carrierShelfInfo = _TaskInfo.GetCraneInfo(info.Task_CraneNo, info.Task_ForkNumber);
                                var trace = GetCommandTrace(db, info, (int)TransferMode.TO, carrierShelfInfo.CraneShelfID);

                                var toCommandTask = commandTask.FirstOrDefault(i => i.TransferMode == (int)TransferMode.TO && i.CraneNo == info.Task_CraneNo);
                                message = defaultMessage + $", Get Task Destination Fail, Please Check";
                                var taskNextDest = GetVShelfInfo(db, info, toCommandTask?.Destination, message);

                                if (!Begin(info, db, defaultMessage)) { return; }

                                if (!UpdateTaskStateAllTask(info, db, defaultMessage)) { return; }

                                if (!InsertToCommand(info, db, trace, taskNo, taskNextDest, defaultMessage)) { return; }

                                if (!UpdateTransferStateToTransferring(info, db, defaultMessage)) { return; }

                                Commit(info, db, defaultMessage);
                            }
                            else if (finishLoc.ShelfID == taskSource.ShelfID && finishLoc.ShelfType != (int)ShelfType.Crane)
                            {
                                var fromCommandTask = commandTask.FirstOrDefault(i => i.TransferMode == (int)TransferMode.FROM && i.CraneNo == info.Task_CraneNo);
                                var toCommandTask = commandTask.FirstOrDefault(i => i.TransferMode == (int)TransferMode.TO && i.CraneNo == info.Task_CraneNo);
                                var trace = GetCommandTrace(db, info, (int)TransferMode.FROM_TO, fromCommandTask.Source);
                                var strTaskNo = GetTaskNo(db, trace);

                                message = defaultMessage + $", Get Task Destination Fail, Please Check";
                                var taskNextDest = GetVShelfInfo(db, info, toCommandTask.Destination, message);

                                if (!Begin(info, db, defaultMessage)) { return; }

                                if (!UpdateTaskStateAllTask(info, db, defaultMessage)) { return; }

                                if (!InsertTaskByTwinFork(info, db, trace, strTaskNo, taskNextDest, defaultMessage)) { return; }

                                if (!UpdateTransferStateToTransferring(info, db, defaultMessage)) { return; }

                                Commit(info, db, defaultMessage);
                            }
                            else
                            {
                                NormalCompleteScenarios_All(info);
                            }
                        }
                        else
                        {
                            if (_Stocker.GetCraneById(info.Task_CraneNo).GetForkById(info.Task_ForkNumber).HasCarrier)
                            {
                                //CST在Crane上
                                //再多下一次To命令
                                var taskNo = _repositories.GetMultiTaskNo(db, info.Task_CraneNo, info.Task_ForkNumber, "TASK");
                                var carrierShelfInfo = _TaskInfo.GetCraneInfo(info.Task_CraneNo, info.Task_ForkNumber);
                                var trace = GetCommandTrace(db, info, (int)TransferMode.TO, carrierShelfInfo.CraneShelfID);

                                if (!Begin(info, db, defaultMessage)) { return; }

                                if (!UpdateTaskStateAllTask(info, db, defaultMessage)) { return; }

                                if (!InsertToCommand(info, db, trace, taskNo, taskDestination, defaultMessage)) { return; }

                                if (!UpdateTransferStateToTransferring(info, db, defaultMessage)) { return; }

                                Commit(info, db, defaultMessage);
                            }
                            else if (finishLoc.ShelfID == taskSource.ShelfID && finishLoc.ShelfType != (int)ShelfType.Crane)
                            {
                                //CST在Source
                                //再多下一次 From_TO
                                var trace = GetCommandTrace(db, info, info.TransferMode, info.Task_Source);
                                var strTaskNo = GetTaskNo(db, trace);

                                if (!Begin(info, db, defaultMessage)) { return; }

                                if (!UpdateTaskStateAllTask(info, db, defaultMessage)) { return; }

                                if (!InsertTaskByTwinFork(info, db, trace, strTaskNo, taskDestination, defaultMessage)) { return; }

                                if (!UpdateTransferStateToTransferring(info, db, defaultMessage)) { return; }

                                Commit(info, db, defaultMessage);
                            }
                            else
                            {
                                //CST 在 Dest
                                //走Normal流程
                                NormalCompleteScenarios_All(info);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private bool InsertTaskByTwinFork(UpdateCommandInfo info, DB db, CommandTrace trace, string[] strTaskNo, VShelfInfo dest, string defaultMessage)
        {
            string message;
            var iResult = 0;
            if (_TaskInfo.Config.SystemConfig.TransferCmdIsFromAndTo == "Y")
                iResult = _repositories.InsertTaskByTwinFork(db, trace, strTaskNo, dest, info.Task_ForkNumber);
            else
                iResult = _repositories.InsertTask(db, trace, strTaskNo[0], info.Task_ForkNumber, dest);

            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Insert Task By TwinFork Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage + ", Insert Task By TwinFork Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
            return true;
        }

        private string[] GetTaskNo(DB db, CommandTrace trace)
        {
            var strTaskNo = new string[0];
            const int taskCount = 2;
            for (var i = 0; i < taskCount; i++)
            {
                Array.Resize(ref strTaskNo, strTaskNo.Length + 1);
                strTaskNo[strTaskNo.Length - 1] = _repositories.GetMultiTaskNo(db, trace.NextCrane, 1, "TASK");
                if (Convert.ToInt32(strTaskNo[strTaskNo.Length - 1].Substring(strTaskNo[strTaskNo.Length - 1].Length - 3)) >= 999)
                    strTaskNo[strTaskNo.Length - 1] = _repositories.GetMultiTaskNo(db, trace.NextCrane, 1, "TASK");

                if (trace.MainTransferMode == (int)TransferMode.FROM_TO || (trace.MainTransferMode == (int)TransferMode.TO && trace.NextCrane != trace.MainCrane))
                {
                    Array.Resize(ref strTaskNo, strTaskNo.Length + 1);
                    strTaskNo[strTaskNo.Length - 1] = _repositories.GetMultiTaskNo(db, trace.NextCrane, 1, "TASK");
                }
            }

            return strTaskNo;
        }

        private CommandTrace GetCommandTrace(DB db, UpdateCommandInfo info, int transferMode, string nextSource)
        {
            var trace = new CommandTrace
            {
                CommandID = info.CommandID,
                NextCrane = info.Task_CraneNo,
                CarrierID = info.CarrierID,
                NextTransferMode = transferMode,
                MainTransferMode = transferMode,
                NextSource = nextSource,
                NextCraneSpeed = _repositories.GetCraneSpeed(db, info.Task_CraneNo, new CraneSpeed()),
                UserID = "LCS",
                BCRReadFlag = false,
                MainPriority = info.HostPriority * 10
            };
            return trace;
        }

        private bool InsertToCommand(UpdateCommandInfo info, DB db, CommandTrace trace, string taskNo, VShelfInfo taskDestination, string defaultMessage)
        {
            string message;
            var iResult = _repositories.InsertTo(db, trace, taskNo, info.Task_ForkNumber, taskDestination);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Insert To Command Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }

            message = defaultMessage + $", Insert To Command Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
            return true;
        }

        private bool UpdateTaskStateAllTask(UpdateCommandInfo info, DB db, string defaultMessage)
        {
            string message;
            var iResult = _repositories.UpdateTaskStateAllTask(db, info);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Update All TaskState Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }

            message = defaultMessage + $", Update All TaskState Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
            return true;
        }

        private bool SaveChanges(Func<DB, bool> dbAction, Action<string> logAction)
        {
            using (var db = GetDB())
            {
                var iResult = db.CommitCtrl(DB.TransactionType.Begin);
                if (iResult != ErrorCode.Success)
                {
                    logAction($"Begin Fail, Result:{iResult}");
                    return false;
                }

                logAction("Begin Success");

                if (dbAction(db) == false)
                {
                    db.CommitCtrl(DB.TransactionType.Rollback);
                    return false;
                }

                iResult = db.CommitCtrl(DB.TransactionType.Commit);
                if (iResult != ErrorCode.Success)
                {
                    logAction($"Commit Fail, Result={iResult}");
                    return false;
                }

                logAction("Commit Success");
                return true;
            }
        }

        private bool Begin(UpdateCommandInfo info, DB db, string defaultMessage)
        {
            string message;

            var iResult = db.CommitCtrl(DB.TransactionType.Begin);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Begin Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage + ", Begin Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

            return true;
        }

        private bool Commit(UpdateCommandInfo info, DB db, string defaultMessage)
        {
            string message;

            #region Commit  

            var iResult = db.CommitCtrl(DB.TransactionType.Commit);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Commit Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage + ", Commit Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

            #endregion Commit

            return true;
        }

        private VShelfInfo GetVShelfInfo(DB db, UpdateCommandInfo info, string shelfId, string message)
        {
            if (!_repositories.GetShelfInfoByShelfID(db, shelfId, out var shelf))
            {
                if (!_repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(shelfId), out shelf))
                {
                    _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                }
            }
            return shelf;
        }

        private static CarrierState GetCarrierState(VShelfInfo finishLocation)
        {
            CarrierState carrierState;
            switch (finishLocation.ShelfType)
            {
                case (int)ShelfType.Port when finishLocation.PortType == (int)PortType.EQ:
                    carrierState = CarrierState.WaitOut;
                    break;
                case (int)ShelfType.Port when finishLocation.PortType == (int)PortType.IO:
                    carrierState = CarrierState.WaitOut;
                    break;
                case (int)ShelfType.Crane:
                    carrierState = CarrierState.Transfering;
                    break;
                default:
                    carrierState = CarrierState.StoreCompleted;
                    break;
            }
            return carrierState;
        }

        private bool UpdateShelfDef(UpdateCommandInfo info, DB db, VShelfInfo shelfInfo, ShelfState shelfState, string defaultMessage)
        {
            string strEM = "";
            string message;

            var iResult = _repositories.UpdateShelfDef(db, shelfInfo.ShelfID, shelfState, ref strEM);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage +
                          $", ShelfID:{shelfInfo.ShelfID}, ShelfState:{(char)shelfState}, Update ShelfDef Fail, Message:{strEM}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage +
                      $", ShelfID:{shelfInfo.ShelfID}, Source:{(char)ShelfState.Stored}, Update ShelfDef Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

            return true;
        }

        private bool UpdateCarrierData(UpdateCommandInfo info, DB db, VShelfInfo finishLocation, CarrierState carrierState, string defaultMessage)
        {
            string strEM = "";
            string message;
            var iResult = _repositories.UpdateCarrierData(db, info.CarrierID, finishLocation.ShelfID, carrierState, ref strEM);

            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", ShelfID:{finishLocation.ShelfID}, CarrierID:{info.CarrierID}, CarrierState:{(int)carrierState}, Update CarrierData Fail, Message:{strEM}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage + $", ShelfID:{finishLocation.ShelfID}, CarrierID:{info.CarrierID}, CarrierState:{(int)carrierState}, Update CarrierData Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

            return true;
        }

        private bool UpdateTransferStateToTransferring(UpdateCommandInfo info, DB db, string defaultMessage)
        {
            string message; string strEM = "";
            var iResult = _repositories.UpdateTransferStateToTransferring(db, info.CommandID, ref strEM);
            if (iResult != ErrorCode.Success)
                iResult = _repositories.UpdateTransferStateToTransferring(db, info.CommandID, ref strEM);

            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Update TransferState Fail, Message={strEM}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage + $", Update TransferState Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
            return true;
        }

        private bool UpdateTransferState(UpdateCommandInfo info, DB db, TransferState newTransferState, string defaultMessage)
        {
            string message;
            var iResult = _repositories.UpdateTransferState(db, info, newTransferState);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Update TransferState Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage + $", Update TransferState Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
            return true;
        }

        private bool UpdateTaskState(UpdateCommandInfo info, DB db, string defaultMessage)
        {
            string message;

            var iResult = _repositories.UpdateTaskState(db, info);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Update TaskState, Begin Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage + $", Update TaskState Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

            return true;
        }

        private bool UpdateTransferStateToAborting(DB db, UpdateCommandInfo info, string defaultMessage)
        {
            string message;
            var iResult = _repositories.UpdateTransferStateToAborting(db, info.CommandID);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Update TransferState To Aborting Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage + $", Update TransferState To Aborting Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
            return true;
        }

        private bool InstallCarrierData(UpdateCommandInfo info, DB db, string carrierId, VShelfInfo shelfInfo, CarrierState carrierState, string defaultMessage)
        {
            string message;
            string strEM = "";
            #region InstallCarrierData

            var iResult = _repositories.InsertCarrierData(db, carrierId, shelfInfo.ShelfID, 1, carrierState, ref strEM);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage +
                          $", ShelfID:{shelfInfo.ShelfID}, CarrierID:{carrierId}, Insert CarrierData Fail, Message:{strEM}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage +
                      $", ShelfID:{shelfInfo.ShelfID}, CarrierID:{carrierId}, Insert CarrierData Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

            #endregion InstallCarrierData

            return true;
        }

        private bool DisableShelfDef(UpdateCommandInfo info, DB db, VShelfInfo shelfInfo, string defaultMessage)
        {
            string message;

            var iResult = _repositories.DisableShelfDef(db, shelfInfo.ShelfID);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage +
                          $", ShelfID:{shelfInfo.ShelfID}, Enable:{(char)ShelfEnable.Disable}, Disable ShelfDef Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                db.CommitCtrl(DB.TransactionType.Rollback);
                return true;
            }

            message = defaultMessage +
                      $", ShelfID:{shelfInfo.ShelfID}, Enable:{(char)ShelfEnable.Disable}, Disable ShelfDef Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

            return false;
        }

        private bool DeleteCarrierData(UpdateCommandInfo info, DB db, VShelfInfo shelfInfo, string defaultMessage)
        {
            string message;
            string strEM = "";
            var iResult = _repositories.DeleteCarrierData(db, shelfInfo.CSTID, shelfInfo.ShelfID, ref strEM);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage +
                          $", ShelfID:{shelfInfo.ShelfID}, CarrierID:{shelfInfo.CSTID}, Delete CarrierData Fail, Message:{strEM}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }

            message = defaultMessage +
                      $", ShelfID:{shelfInfo.ShelfID}, CarrierID:{shelfInfo.CSTID}, Delete CarrierData Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

            return true;
        }

        private bool InsertCarrierDataToHistory(UpdateCommandInfo info, DB db, VShelfInfo shelfInfo, string defaultMessage)
        {
            string message;
            string strEM = "";
            var iResult = _repositories.InsertCarrierDataToHistory(db, shelfInfo.CSTID, shelfInfo.ShelfID, ref strEM);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage +
                          $", ShelfID:{shelfInfo.ShelfID}, CarrierID:{shelfInfo.CSTID}, Insert HisCarrierData Fail, Message:{strEM}";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                return false;
            }

            message = defaultMessage +
                      $", ShelfID:{shelfInfo.ShelfID}, CarrierID:{shelfInfo.CSTID}, Insert HisCarrierData Success";
            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

            return true;
        }

        private bool DeleteTransfer(TransferCmd transferCmd)
        {
            return SaveChanges(db =>
                {
                    var commandId = transferCmd.CommandInfo.CommandID;
                    var iResult = _repositories.InsertTransferToHistory(db, commandId);
                    if (iResult != ErrorCode.Success)
                    {
                        WriteTrace(transferCmd, $"Insert HisTransfer Fail, Result={iResult}");
                        //return false;
                    }

                    WriteTrace(transferCmd, $"Insert HisTransfer Success");

                    iResult = _repositories.DeleteTransfer(db, commandId);
                    if (iResult != ErrorCode.Success)
                    {
                        WriteTrace(transferCmd, $"Delete Transfer Fail, Result={iResult}");
                        //return false;
                    }

                    WriteTrace(transferCmd, $"Delete Transfer Success");

                    iResult = _repositories.InsertTaskToHistory(db, commandId);
                    if (iResult == ErrorCode.Success)
                    {
                        WriteTrace(transferCmd,
                            $"CommandID:{commandId}, Insert HisTask Success");
                    }

                    iResult = _repositories.DeleteTask(db, commandId);
                    if (iResult == ErrorCode.Success)
                    {
                        WriteTrace(transferCmd, $"Delete Task Success");
                    }

                    return true;
                },
                msg => WriteTrace(transferCmd, msg)
            );
        }
    }
    public class TransferCmd
    {
        public TransferCmd(UpdateCommandInfo info, string scenarioName)
        {
            CommandInfo = info;
            DefaultMessage = scenarioName;
        }

        public UpdateCommandInfo CommandInfo { get; private set; }
        public string DefaultMessage { get; private set; }
    }
}
