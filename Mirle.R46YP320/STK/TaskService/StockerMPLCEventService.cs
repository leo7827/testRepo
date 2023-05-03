using Mirle.DataBase;
using Mirle.LCS.Enums;
using Mirle.LCS.Models;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.R46YP320.STK.MCS;
using Mirle.Stocker;
using Mirle.Stocker.Enums;
using Mirle.Stocker.Events;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.Module;
using Mirle.Stocker.TaskControl.TraceLog;
using Mirle.Stocker.TaskControl.TraceLog.Format;
using Mirle.Structure.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Mirle.R46YP320.STK.TaskService
{
    public class StockerMPLCEventService : StockerMPLCEventModule
    {
        private readonly DataCollectionEventsService _DataCollectionEventsService;
        private readonly RepositoriesService _Repositories;
        private readonly MCSCrane _CraneReport;

        public StockerMPLCEventService(TaskInfo taskInfo, IStocker stocker, DataCollectionEventsModule dataCollectionEventsService, LoggerService loggerService, MCSCrane craneReport) : base(taskInfo, stocker, loggerService)
        {
            _DataCollectionEventsService = (DataCollectionEventsService)dataCollectionEventsService;
            _Repositories = new RepositoriesService(taskInfo, loggerService);
            _CraneReport = craneReport;
        }

        #region Crane
        protected override void Crane_OnTaskCmdWriteToMPLC(object sender, CraneEventArgs args)
        {
            try
            {
                string defaultMessage = "Task Transferring";
                string message = string.Empty;
                string strCommandID = args.CommandId;
                string strTaskNo = args.TaskNo;
                string strCSTID = args.CarrierID;
                string strCraneID = _TaskInfo.GetCraneInfo(args.Id).CraneID;
                string strCraneShelfID = _TaskInfo.GetCraneInfo(args.Id).CraneShelfID;

                //如果是手動命令, 因為怕OperatorInitiatedAction會晚報, 所以等一下再報
                if (strCommandID.StartsWith("MANUAL"))
                {
                    Task.Delay(500).Wait();
                }

                using (DB _db = GetDB())
                {
                    Stocker.TaskControl.Info.TransferCommand transfer = _Repositories.GetTransferCmdByCommandID(_db, strCommandID);
                    if (transfer != null)
                    {
                        IEnumerable<TaskCmd> taskCmds = _Repositories.GetTaskByCommandID(_db, strCommandID);
                        int taskCount = taskCmds.Count();
                        if (taskCount > 1)
                        {
                            TaskCmd taskCmd = taskCmds.FirstOrDefault(i => i.TaskNo == args.TaskNo);
                            if ((transfer.TransferMode == (int)TransferMode.FROM_TO && taskCount == 2) ||
                                (transfer.TransferMode != (int)TransferMode.FROM_TO && taskCount == 1))
                            {
                                int iResult = ErrorCode.Initial; string strEM = "";
                                iResult = _Repositories.UpdateTransferStateToTransferring(_db, strCommandID, ref strEM, taskCmd.InitialDT);
                                if (iResult != ErrorCode.Success)
                                    iResult = _Repositories.UpdateTransferStateToTransferring(_db, strCommandID, ref strEM, taskCmd.InitialDT);

                                if (iResult != ErrorCode.Success)
                                {
                                    message = defaultMessage + $", TransferState:{(int)TransferState.Transferring}, Update Transfer Fail, Message={strEM}";
                                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(args.Id, strCommandID, strTaskNo, strCSTID, message));
                                }
                                message = defaultMessage + $", TransferState:{(int)TransferState.Transferring}, Update Transfer Success";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(args.Id, strCommandID, strTaskNo, strCSTID, message));
                                if (transfer.TransferMode != (int)TransferMode.MOVE &&
                                    transfer.TransferMode != (int)TransferMode.SCAN &&
                                    taskCmd.TransferMode != (int)TransferMode.TO &&
                                    taskCmd.TransferMode != (int)TransferMode.FROM_TO)
                                {
                                    UpdateCommandInfo infos = _Repositories.GetCommandInfoByCommandID(_db, transfer.CommandID);
                                    var shelfInfo = _Repositories.GetShelfInfoByCarrierID(_db, transfer.CSTID);
                                    _DataCollectionEventsService.OnTransferInitiated(this, new TransferInitiatedEventArgs(infos, shelfInfo.CarrierLoc));
                                    _CraneReport.SetCraneActive(strCraneID, args.Id, taskCmd.ForkNumber, strCommandID);
                                }
                                else if (transfer.TransferMode == (int)TransferMode.FROM_TO &&
                                         taskCount == 2 &&
                                         taskCmd.TransferMode == (int)TransferMode.FROM_TO)
                                {
                                    _CraneReport.SetCraneActive(strCraneID, args.Id, taskCmd.ForkNumber, strCommandID);
                                }
                            }
                            else if (transfer.TransferMode == (int)TransferMode.FROM_TO && taskCount == 4 ||
                                     transfer.TransferMode == (int)TransferMode.TO && taskCount == 3)
                            {
                                _CraneReport.SetCraneActive(strCraneID, args.Id, taskCmd.ForkNumber, strCommandID);
                            }
                        }
                        else if (taskCount == 1)
                        {
                            if (transfer.TransferMode == (int)TransferMode.SCAN)
                            {
                                UpdateCommandInfo infos = _Repositories.GetCommandInfoByCommandID(_db, transfer.CommandID);
                                var carrierLoc = _Repositories.GetShelfInfoByShelfID(_db, infos.Source, out VShelfInfo source);
                                _DataCollectionEventsService.OnScanInitiated(this, new ScanInitiatedEventArgs(infos.CarrierID, source.CarrierLoc, source.ZoneID));
                            }
                            else if (transfer.TransferMode == (int)TransferMode.FROM ||
                                     transfer.TransferMode == (int)TransferMode.TO ||
                                     transfer.TransferMode == (int)TransferMode.FROM_TO)
                            {
                                UpdateCommandInfo infos = _Repositories.GetCommandInfoByCommandID(_db, transfer.CommandID);
                                var shelfInfo = _Repositories.GetShelfInfoByCarrierID(_db, transfer.CSTID);
                                _DataCollectionEventsService.OnTransferInitiated(this, new TransferInitiatedEventArgs(infos, shelfInfo.CarrierLoc));
                            }

                            _CraneReport.SetCraneActive(strCraneID, taskCmds.FirstOrDefault().CraneNo, taskCmds.FirstOrDefault().ForkNumber, strCommandID);
                            message = defaultMessage + $", TransferState:{(int)TransferState.Transferring}, Update Transfer Success";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(args.Id, strCommandID, strTaskNo, strCSTID, message));
                        }
                        else
                        {
                            message = defaultMessage + ", Get Task Fail, Please Check";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(args.Id, message));
                        }
                    }
                    else
                    {
                        message = defaultMessage + $", Get Transfer Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(args.Id, strCommandID, strTaskNo, strCSTID, message));
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void Crane_OnAvailStatusChanged(object sender, CraneEventArgs args)
        {
            try
            {
                string message = string.Empty;
                string strCraneID = _TaskInfo.GetCraneInfo(args.Id).CraneID;
                string alarmText = _TaskInfo.GetCraneInfo(args.Id).CraneID + " Status Change to " + _CraneAvailStatus[args.Id].ToString();
                int alarmID = int.Parse(LCS.Models.Define.LCSAlarm.Crane1KeySwitchOff_F002,
                                  System.Globalization.NumberStyles.HexNumber) + 900000;
                message = $"Crane Avail Status:{_CraneAvailStatus[args.Id]}>{args.NewAvailStatus}";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(args.Id, message));
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void Crane_OnStatusChanged(object sender, CraneEventArgs args)
        {
            switch (args.NewStatus)
            {
                case StockerEnums.CraneStatus.BUSY:
                    Crane_StatusChange_Active(args.Id);
                    break;
                case StockerEnums.CraneStatus.IDLE:
                    Crane_StatusChange_Idle(args.Id);
                    break;
                case StockerEnums.CraneStatus.ESCAPE:
                    Crane_StatusChange_Escape(args.Id);
                    break;
                case StockerEnums.CraneStatus.Waiting:
                    Crane_StatusChange_Waiting(args.Id);
                    break;
                default:
                    break;
            }
        }

        #region Crane_OnStatusChanged
        private void Crane_StatusChange_Active(int craneNo)
        {
            try
            {
                ICrane crane = _Stocker.GetCraneById(craneNo);
                int intRealTimeTravelPosition_cm = crane.CurrentPosition;
                string strCommandID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CommandID;
                string strCSTID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CSTID;
                string strTaskNo = _LCSExecutingCMD.GetExecutingCMD(craneNo).CommandID;
                string strCraneID = _TaskInfo.GetCraneInfo(craneNo).CraneID;
                string message = string.Empty;

                message = $"Crane Status Change:{_CraneStatus[craneNo].ToString()}>Active";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, message));

                _Crane_LastIdleTotalSec[craneNo] = 0;
                _Crane_CurrentPosition[craneNo] = intRealTimeTravelPosition_cm;
                _Crane_Travel[craneNo] = 0;

                //if (_CraneStatus[craneNo] == StockerEnums.CraneStatus.ESCAPE)
                //    _DataCollectionEventsService.OnCraneIdle(this, new CraneStatusEventArgs(_EscapeCommandID, strCraneID));

                if (string.IsNullOrWhiteSpace(_LCSExecutingCMD.GetExecutingCMD(craneNo).TaskNo))
                {
                    message = $"Crane Status Change:{_CraneStatus[craneNo].ToString()}>Active, But No TaskNo, wait 0.3s ...";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, message));
                    Task.Delay(300).Wait();
                    strCommandID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CommandID;
                    strCSTID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CSTID;
                }

                if (!string.IsNullOrWhiteSpace(_LCSExecutingCMD.GetExecutingCMD(craneNo).TaskNo))
                {
                    if (_CraneStatus[craneNo] != StockerEnums.CraneStatus.BUSY)
                    {
                        //_DataCollectionEventsService.OnCraneActive(this, new CraneStatusEventArgs(strCommandID, strCraneID));

                        _Crane_CurrentPosition[craneNo] = intRealTimeTravelPosition_cm;
                        _Crane_Travel[craneNo] = 0;

                        //if (_LCSExecutingCMD.GetExecutingCMD(craneNo).TransferMode == TransferMode.TO)
                        //    _DataCollectionEventsService.OnCarrierTransferring(this, new CarrierTransferringEventArgs(strCSTID, strCraneID));
                        message = $"Crane Status Change:{_CraneStatus[craneNo].ToString()}>Active, Fork Has Carrier: {crane.GetLeftFork().HasCarrier}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, message));
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
            finally
            { _CraneStatus[craneNo] = StockerEnums.CraneStatus.BUSY; }
        }

        private void Crane_StatusChange_Idle(int craneNo)
        {
            try
            {
                ICrane crane = _Stocker.GetCraneById(craneNo);
                string strCommandID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CommandID;
                string strCSTID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CSTID;
                string strTaskNo = _LCSExecutingCMD.GetExecutingCMD(craneNo).TaskNo;
                string strCraneID = _TaskInfo.GetCraneInfo(craneNo).CraneID;
                string message = string.Empty;

                message = $"Crane Status Change:{_CraneStatus[craneNo].ToString()}>Idle";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, message));

                int intRealTimeTravelPosition_cm = crane.CurrentPosition;
                _Crane_Travel[craneNo] = _Crane_Travel[craneNo] + Math.Abs(intRealTimeTravelPosition_cm - _Crane_CurrentPosition[craneNo]);
                _Crane_CurrentPosition[craneNo] = intRealTimeTravelPosition_cm;
                _Crane_Travel[craneNo] = 0;

                //if (_CraneStatus[craneNo] == StockerEnums.CraneStatus.BUSY || _CraneStatus[craneNo] == StockerEnums.CraneStatus.NOSTS)
                //_DataCollectionEventsService.OnCraneIdle(this, new CraneStatusEventArgs(strCommandID, strCraneID));
                //else
                //_DataCollectionEventsService.OnCraneIdle(this, new CraneStatusEventArgs(_EscapeCommandID, strCraneID));
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
            finally
            { _CraneStatus[craneNo] = StockerEnums.CraneStatus.IDLE; }
        }

        private void Crane_StatusChange_Escape(int craneNo)
        {
            try
            {
                ICrane crane = _Stocker.GetCraneById(craneNo);
                string strCommandID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CommandID;
                string strCSTID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CSTID;
                string strTaskNo = _LCSExecutingCMD.GetExecutingCMD(craneNo).TaskNo;
                string strCraneID = _TaskInfo.GetCraneInfo(craneNo).CraneID;
                string message = string.Empty;

                message = $"Crane Status Change:{_CraneStatus[craneNo].ToString()}>Escape";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, message)); ;

                int intRealTimeTravelPosition_cm = crane.CurrentPosition;

                switch (_CraneStatus[craneNo])
                {
                    case StockerEnums.CraneStatus.BUSY:
                        _Crane_Travel[craneNo] = _Crane_Travel[craneNo] + Math.Abs(intRealTimeTravelPosition_cm - _Crane_CurrentPosition[craneNo]);
                        //_DataCollectionEventsService.OnCraneIdle(this, new CraneStatusEventArgs(strCommandID, strCraneID));
                        _Crane_CurrentPosition[craneNo] = intRealTimeTravelPosition_cm;
                        _Crane_Travel[craneNo] = 0;
                        //_DataCollectionEventsService.OnCraneActive(this, new CraneStatusEventArgs(_EscapeCommandID, strCraneID));
                        break;
                    case StockerEnums.CraneStatus.ESCAPE:
                    case StockerEnums.CraneStatus.NONE:
                    case StockerEnums.CraneStatus.NOSTS:
                    case StockerEnums.CraneStatus.IDLE:
                    default:
                        _Crane_CurrentPosition[craneNo] = intRealTimeTravelPosition_cm;
                        _Crane_Travel[craneNo] = 0;
                        //_DataCollectionEventsService.OnCraneActive(this, new CraneStatusEventArgs(_EscapeCommandID, strCraneID));
                        break;
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
            finally
            { _CraneStatus[craneNo] = StockerEnums.CraneStatus.ESCAPE; }
        }

        private void Crane_StatusChange_Waiting(int craneNo)
        {
            string strCommandID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CommandID;
            string strCSTID = _LCSExecutingCMD.GetExecutingCMD(craneNo).CSTID;
            string strTaskNo = _LCSExecutingCMD.GetExecutingCMD(craneNo).TaskNo;
            string strCraneID = _TaskInfo.GetCraneInfo(craneNo).CraneID;
            string message = string.Empty;

            message = $"Crane Status Change:{_CraneStatus[craneNo].ToString()}>Waiting";
            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, message));
        }
        #endregion Crane_OnStatusChanged

        protected override void Crane_OnLocationUpdated(object sender, CraneEventArgs args)
        {
            if (args.T3 == 0)
            {
                int intRMIndex = args.Id;
                ICrane crane = _Stocker.GetCraneById(intRMIndex);
                int intRealTimeTravelPosition_cm = crane.CurrentPosition;
                _Crane_Travel[intRMIndex] = _Crane_Travel[intRMIndex] + Math.Abs(intRealTimeTravelPosition_cm - _Crane_CurrentPosition[intRMIndex]);
                _Crane_CurrentPosition[intRMIndex] = intRealTimeTravelPosition_cm;
            }
        }

        #region Fork
        protected override void CraneFork_OnCSTPresenceChanged(object sender, ForkEventArgs args)
        {
            if (args.SignalIsOn)
                CraneFork_OnCSTPresenceOn(args.CraneId, args.ForkId);
            else
                CraneFork_OnCSTPresenceOff(args.CraneId, args.ForkId);
        }

        #region CraneFork_OnCSTPresenceChanged
        private void CraneFork_OnCSTPresenceOn(int craneNo, int frokNumber)
        {
            try
            {
                int iResult = ErrorCode.Initial;
                string message = string.Empty;
                string defaultMessage = $"Presence On";
                string strCommandID = _LCSExecutingCMD.GetExecutingCMD(craneNo, frokNumber).CommandID;
                string strTaskNo = _LCSExecutingCMD.GetExecutingCMD(craneNo, frokNumber).TaskNo;
                string strCSTID = _LCSExecutingCMD.GetExecutingCMD(craneNo, frokNumber).CSTID;
                string strCraneID = _TaskInfo.GetCraneInfo(craneNo, frokNumber).CraneID;
                string strCraneShelfID = _TaskInfo.GetCraneInfo(craneNo, frokNumber).CraneShelfID;

                message = defaultMessage;
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));

                using (DB _db = GetDB())
                {
                    if (!_Repositories.GetTask(_db, strCommandID, strTaskNo, craneNo, out TaskCmd task))
                    {
                        message = defaultMessage + $", Get Task Fail-1";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                        if (!_Repositories.GetTask(_db, strCommandID, strTaskNo, out task))
                        {
                            message = defaultMessage + $", Get Task Fail-2";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                            if (!_Repositories.GetTask(_db, craneNo, strTaskNo, out task))
                            {
                                message = defaultMessage + $", Get Task Fail-3";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                                return;
                            }
                        }
                    }

                    if (_Repositories.GetTransferCmdByCommandID(_db, task.CommandID, out Stocker.TaskControl.Info.TransferCommand transfer))
                    {
                        if (transfer.TransferMode == (int)TransferMode.SCAN)
                        {
                            return;
                        }

                        if (!_Repositories.GetShelfInfoByShelfID(_db, task.Source, out VShelfInfo source))
                        {
                            if (!_Repositories.GetShelfInfoByPLCPortID(_db, Convert.ToInt32(task.Source), out source))
                            {
                                message = defaultMessage + $", Get ShelfInfo Fail, Please Check";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                                return;
                            }
                        }

                        if (!_Repositories.GetShelfInfoByShelfID(_db, task.Destination, out VShelfInfo dest))
                        {
                            if (!_Repositories.GetShelfInfoByPLCPortID(_db, Convert.ToInt32(task.Destination), out dest))
                            {
                                message = defaultMessage + $", Get ShelfInfo Fail, Please Check";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                                return;
                            }
                        }
                        string strEM = "";

                        #region UpdateTransferStateToTransferring
                        if (transfer.TransferState != (int)TransferState.Transferring)
                        {
                            iResult = _Repositories.UpdateTransferStateToTransferring(_db, task.CommandID, ref strEM, task.InitialDT);
                            if (iResult != ErrorCode.Success)
                                iResult = _Repositories.UpdateTransferStateToTransferring(_db, task.CommandID, ref strEM, task.InitialDT);

                            if (iResult != ErrorCode.Success)
                            {
                                message = defaultMessage + $", TransferState:{(int)TransferState.Transferring}, Update TransferState Fail, Message={strEM}";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                            }
                            else
                            {
                                message = defaultMessage + $", TransferState:{(int)TransferState.Transferring}, Update TransferState Success";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                            }
                        }
                        #endregion UpdateTransferStateToTransferring

                        #region UpdateShelfDef-Source
                        iResult = _Repositories.UpdateShelfDef(_db, source.ShelfID, ShelfState.EmptyShelf, ref strEM);
                        if (iResult != ErrorCode.Success)
                            iResult = _Repositories.UpdateShelfDef(_db, source.ShelfID, ShelfState.EmptyShelf, ref strEM);

                        if (iResult != ErrorCode.Success)
                        {
                            message = defaultMessage + $", SheldID:{task.Source}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Fail, Message={strEM}";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                        }
                        else
                        {
                            message = defaultMessage + $", SheldID:{source.ShelfID}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Success";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                        }
                        #endregion UpdateShelfDef-Source

                        #region UpdateShelfDef-CraneShelfID
                        iResult = _Repositories.UpdateShelfDef(_db, strCraneShelfID, ShelfState.Stored, ref strEM);
                        if (iResult != ErrorCode.Success)
                            iResult = _Repositories.UpdateShelfDef(_db, strCraneShelfID, ShelfState.Stored, ref strEM);

                        if (iResult != ErrorCode.Success)
                        {
                            message = defaultMessage + $", ShelfID:{strCraneShelfID}, ShelfState:{(char)ShelfState.Stored}, Update ShelfDef Fail, Message={strEM}";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                        }
                        else
                        {
                            message = defaultMessage + $", ShelfID:{strCraneShelfID}, ShelfState:{(char)ShelfState.Stored}, Update ShelfDef Success";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                        }
                        #endregion UpdateShelfDef-CraneShelfID

                        #region UpdateCarrierData
                        iResult = _Repositories.UpdateCarrierData(_db, source.CSTID, strCraneShelfID, CarrierState.Transfering, ref strEM);
                        if (iResult != ErrorCode.Success)
                            iResult = _Repositories.UpdateCarrierData(_db, source.CSTID, strCraneShelfID, CarrierState.Transfering, ref strEM);

                        if (iResult != ErrorCode.Success)
                        {
                            message = defaultMessage + $", CarrierID:{source.CSTID}, ShelfID:{strCraneShelfID}, CarrierState:{(int)CarrierState.Transfering}, Update CarrierData Fail, Message={strEM}";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                        }
                        else
                        {
                            message = defaultMessage + $", CarrierID:{source.CSTID}, ShelfID:{strCraneShelfID}, CarrierState:{(int)CarrierState.Transfering}, Update CarrierData Success";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                        }
                        #endregion UpdateCarrierData

                        System.Threading.Thread.Sleep(1000);

                        var count = (source.LocateCraneNo != dest.LocateCraneNo && source.LocateCraneNo != 3) ? 2 : 1;
                        IEnumerable<TaskCmd> taskCmds = _Repositories.GetTaskByCommandID(_db, strCommandID);
                        int taskCount = taskCmds.Count();
                        if (taskCount == 1) count = 2;

                        var needReportCarrierResumed = NeedReportCarrierResumed(source, count, task);
                    }
                    else
                    {
                        message = defaultMessage + ", Get Transfer Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private bool NeedReportCarrierResumed(VShelfInfo shelf, int iCount, TaskCmd taskCmd)
        {
            bool needReportCarrierResumed = false;

            if (shelf != null)
            {
                if (shelf.CSTState == (int)VIDEnums.CarrierState.Alternate)
                {
                    needReportCarrierResumed = true;
                }
                else
                {
                    if (taskCmd.TransferMode == (int)TransferMode.FROM_TO)
                    {
                        needReportCarrierResumed = iCount == 1;
                    }
                    else
                    {
                        needReportCarrierResumed = iCount == 2;
                    }
                }
            }
            else
            {
                if (taskCmd.TransferMode == (int)TransferMode.FROM_TO)
                {
                    needReportCarrierResumed = iCount == 1;
                }
                else
                {
                    needReportCarrierResumed = iCount == 2;
                }
            }
            return needReportCarrierResumed;
        }

        private void CraneFork_OnCSTPresenceOff(int craneNo, int frokNumber)
        {
            try
            {
                string message = string.Empty;
                string defaultMessage = $"Presence Off";
                string strCommandID = _LCSExecutingCMD.GetExecutingCMD(craneNo, frokNumber).CommandID;
                string strCSTID = _LCSExecutingCMD.GetExecutingCMD(craneNo, frokNumber).CSTID;
                string strTaskNo = _LCSExecutingCMD.GetExecutingCMD(craneNo, frokNumber).TaskNo;
                string strCraneID = _TaskInfo.GetCraneInfo(craneNo, frokNumber).CraneID;
                string strCraneShelfID = _TaskInfo.GetCraneInfo(craneNo, frokNumber).CraneShelfID;
                using (DB _db = GetDB())
                {
                    string strEM = "";
                    int iResult = _Repositories.UpdateShelfDef(_db, strCraneShelfID, ShelfState.EmptyShelf, ref strEM);
                    if (iResult == ErrorCode.Success)
                    {
                        message = defaultMessage + ", Update ShelfDef Success";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, strCSTID, message));
                    }
                    else
                    {
                        message = defaultMessage + $", Update ShelfDef Fail, Message={strEM}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, strCSTID, message));
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }
        #endregion CraneFork_OnCSTPresenceChanged

        protected override void CraneFork_OnCSTReadReportOn(object sender, ForkEventArgs args)
        {
            //if (args.ForkId == 1)
            CraneFork_OnCSTReadReportOn(args.CraneId, args.ForkId, args.BCRResult);
        }
        private void CraneFork_OnCSTReadReportOn(int craneNo, int frokNumber, string carrierID)
        {
            try
            {
                string defaultMessage = $"Crane{craneNo} Fork{frokNumber} BCR Read Report On";
                string message = string.Empty;

                if (string.IsNullOrWhiteSpace(carrierID))
                {
                    message = defaultMessage + $", ResultID is Empty, Please Check";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, message));
                    return;
                }

                string strCommandID = _LCSExecutingCMD.GetExecutingCMD(craneNo, frokNumber).CommandID;
                string strTaskNo = _LCSExecutingCMD.GetExecutingCMD(craneNo, frokNumber).TaskNo;
                string strCSTID = _LCSExecutingCMD.GetExecutingCMD(craneNo, frokNumber).CSTID;
                string strCraneID = _TaskInfo.GetCraneInfo(craneNo, frokNumber).CraneID;
                string strCraneShelfID = _TaskInfo.GetCraneInfo(craneNo, frokNumber).CraneShelfID;

                using (DB _db = GetDB())
                {
                    if (!_Repositories.GetTask(_db, strCommandID, strTaskNo, craneNo, out TaskCmd task))
                    {
                        if (!_Repositories.GetTask(_db, strCommandID, strTaskNo, out task))
                        {
                            if (!_Repositories.GetTask(_db, craneNo, strTaskNo, out task))
                            {
                                message = defaultMessage + $", Get Task Fail, Please Check";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                                return;
                            }
                        }
                    }

                    if (task.TransferMode == (int)TransferMode.SCAN)
                    {
                        return;
                    }

                    if (!_Repositories.GetShelfInfoByShelfID(_db, strCraneShelfID, out VShelfInfo info))
                    {
                        if (!_Repositories.GetShelfInfoByPLCPortID(_db, Convert.ToInt32(task.Source), out info))
                        {
                            message = defaultMessage + $", Get ShelfInfo Fail, Please Check";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));
                            return;
                        }
                    }

                    var carrierLoc = info.CarrierLoc;
                    var IDRPosition = info.ZoneID;
                    var bCRReadStatus = GetAbnormalCSTIDType(_db, carrierLoc, craneNo, frokNumber, strCSTID, ref carrierID);

                    message = defaultMessage + $", CommandID:{strCommandID}, Task:{strTaskNo}, CarrierID:{strCSTID}, ResultID:{carrierID}. IDreadStatus:{(int)bCRReadStatus}";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(craneNo, frokNumber, strCommandID, strTaskNo, message));

                    _TaskInfo.SetCraneBCRResult(craneNo, frokNumber, carrierID);

                    switch (bCRReadStatus)
                    {
                        case AbnormalCSTIDType.Failure:
                            //_DataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs("", carrierLoc, VIDEnums.IDReadStatus.Failure));
                            break;
                        case AbnormalCSTIDType.Mismatch:
                            //_DataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(carrierID, carrierLoc, VIDEnums.IDReadStatus.Mismatch));
                            break;
                        case AbnormalCSTIDType.Duplicate:
                            //_DataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(carrierID, carrierLoc, VIDEnums.IDReadStatus.Duplicate));
                            break;
                    }

                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private AbnormalCSTIDType GetAbnormalCSTIDType(DB _db, string carrierLoc, int craneNo, int forkNumber, string strCSTID, ref string carrierID)
        {
            AbnormalCSTIDType bCRReadStatus = AbnormalCSTIDType.Success;

            switch (carrierID.Trim())
            {
                case _ReplyCSTID_NoRead:
                case _ReplyCSTID_ReadFail:
                case "":
                    carrierID = _Repositories.GetAbnormalCSTID(AbnormalCSTIDType.Failure, carrierLoc, strCSTID);
                    bCRReadStatus = AbnormalCSTIDType.Failure;
                    break;
                case _ReplyCSTID_NoCST:
                    bCRReadStatus = AbnormalCSTIDType.NoCarrier;
                    carrierID = _Repositories.GetAbnormalCSTID(AbnormalCSTIDType.NoCarrier, carrierLoc, strCSTID);
                    break;
                default:
                    if (_LCSExecutingCMD.GetExecutingCMD(craneNo, forkNumber).CSTID == carrierID.Trim())
                        bCRReadStatus = AbnormalCSTIDType.Success;
                    else
                    {
                        if (_Repositories.GetShelfInfoByCarrierID(_db, carrierID, out VShelfInfo duplicate))
                        {
                            carrierID = _Repositories.GetAbnormalCSTID(AbnormalCSTIDType.Duplicate, carrierLoc, strCSTID);
                            bCRReadStatus = AbnormalCSTIDType.Duplicate;
                        }
                        else if (string.IsNullOrWhiteSpace(_LCSExecutingCMD.GetExecutingCMD(craneNo).CSTID))
                        {
                            bCRReadStatus = AbnormalCSTIDType.Success;
                        }
                        else
                        {
                            carrierID = _Repositories.GetAbnormalCSTID(AbnormalCSTIDType.Mismatch, carrierLoc, strCSTID);
                            bCRReadStatus = AbnormalCSTIDType.Mismatch;
                        }
                    }

                    break;
            }
            return bCRReadStatus;
        }
        #endregion Fork

        #endregion Crane

        #region IO Port
        protected override void IO_OnCSTWaitOut(object sender, IOEventArgs args)
        {
        }

        protected override void IO_OnInServiceChanged(object sender, IOEventArgs args)
        {
            try
            {
                //var PortReportFlag = _Repositories.GetAllPortDef(GetDB()).FirstOrDefault(i => i.PortType == (int)PortType.IO && i.PortTypeIndex == args.IOId)?.ReportMCSFlag;
                PortDefInfo portDefInfo = _TaskInfo.GetIOPortInfo(args.IOId);
                string message = string.Empty;
                string defaultMessage = "IO Port Service Changed";
                bool enable = false;
                bool inService = args.SignalIsOn;
                int alarmID = Int32.Parse(LCS.Models.Define.LCSAlarm.EquipmentStop_F006, System.Globalization.NumberStyles.HexNumber);
                if (portDefInfo != null)
                {
                    string alarmText = portDefInfo.HostEQPortID + " Status Change ";
                    IIOPort port = _Stocker.GetIOPortById(args.IOId);

                    if (_Repositories.GetShelfInfoByHostEQPortID(GetDB(), portDefInfo.HostEQPortID, out VShelfInfo shelfInfo))
                    {
                        enable = shelfInfo.Enable;

                        message = defaultMessage + $", Service:{(inService ? "ON" : "OFF")}, Enable:{(enable ? "ON" : "OFF")}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));

                        //if (PortReportFlag)
                        //{
                            //if (inService && enable)
                            //    _DataCollectionEventsService.OnUnitAlarmCleared(this, new UnitAlarmEventArgs(alarmID, alarmText, portDefInfo.HostEQPortID, VIDEnums.MainteState.NotMaintenance));
                            //else
                            //    _DataCollectionEventsService.OnUnitAlarmSet(this, new UnitAlarmEventArgs(alarmID, alarmText, portDefInfo.HostEQPortID, VIDEnums.MainteState.NotMaintenance));
                        //}
                    }
                    else
                    {
                        message = defaultMessage + $", Service:{(inService ? "ON" : "OFF")}, Get ShelfInfo Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOId}", message));
                    }
                }
                else
                {
                    message = defaultMessage + $", Service:{(inService ? "ON" : "OFF")}, Get PortDef Fail, Please Check";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOId}", message));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void IO_OnCSTWaitIn(object sender, IOEventArgs args)
        {
            try
            {
                var PortReportFlag = _Repositories.GetAllPortDef(GetDB()).FirstOrDefault(i => i.PortType == (int)PortType.IO && i.PortTypeIndex == args.IOId).ReportMCSFlag;
                PortDefInfo portDefInfo = _TaskInfo.GetIOPortInfo(args.IOId);

                if (portDefInfo != null)
                {
                    if (!_Stocker.GetIOPortById(args.IOId).IsReadyFromCraneSide)
                    {
                        using (DB _db = GetDB())
                            PortWaitInScenarios(_db, portDefInfo, args.CstId, PortReportFlag);
                    }
                    else
                    {
                        string message = $"WaitIn On IOPort, CarrierID:{args.CstId}, But ReadyFromCraneSide Is On";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOId}", message));
                    }
                }
                else
                {
                    string message = $"WaitIn On IOPort, CarrierID:{args.CstId}";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOId}", message));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void IO_OnCSTRemoved(object sender, IOEventArgs args)
        {
            try
            {
                var PortReportFlag = _Repositories.GetAllPortDef(GetDB()).FirstOrDefault(i => i.PortType == (int)PortType.IO && i.PortTypeIndex == args.IOId).ReportMCSFlag;

                PortDefInfo portDefInfo = _TaskInfo.GetIOPortInfo(args.IOId);
                string message = string.Empty;
                string defaultMessage = "IO Port CST Removed On";

                if (portDefInfo != null)
                {
                    message = defaultMessage;
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));

                    if (portDefInfo.PortLocationType == PortLocationType.MGVPort)
                    {
                        using (DB _db = GetDB())
                        {
                            if (_Repositories.GetShelfInfoByHostEQPortID(_db, portDefInfo.HostEQPortID, out VShelfInfo shelfInfo))
                            {
                                if (!string.IsNullOrWhiteSpace(shelfInfo.CSTID))
                                {
                                    bool isDeleteCarrierDataSuccess = _Repositories.DeleteCST("", "", shelfInfo.ShelfID, shelfInfo.Stage, shelfInfo.CSTID, defaultMessage);
                                    if (!isDeleteCarrierDataSuccess)
                                        return;
                                    else
                                    {
                                        if (PortReportFlag)
                                        {
                                            _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Manual, shelfInfo.CarrierLoc));
                                            message = defaultMessage + $", CarrierRemoved CSTID:{shelfInfo.CSTID}";
                                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));

                                        }
                                    }
                                }
                                else
                                {
                                    message = defaultMessage + $", carrierID is Empty, Please Check";
                                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));
                                }
                            }
                            else
                            {
                                message = defaultMessage + $", Get ShelfInfo Fail, Please Check";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));
                            }
                        }
                    }
                }
                else
                {
                    message = defaultMessage + ", Get PortDef Fail, Please Check";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOId}", message));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void IO_OnDirectionChanged(object sender, IOEventArgs args)
        {
            try
            {
                //var PortReportFlag = _Repositories.GetAllPortDef(GetDB()).FirstOrDefault(i => i.ReportMCSFlag == true && i.PortTypeIndex == args.IOId).ReportMCSFlag;
                //if (PortReportFlag)
                //{
                //    PortDefInfo portDefInfo = _TaskInfo.GetIOPortInfo(args.IOId);
                //    string defaultMessage = $"IO Port Direction Changed, Direction:{args.NewDirection.ToString()}";
                //    string message = string.Empty;
                //    if (portDefInfo != null)
                //    {
                //        message = defaultMessage;
                //        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));

                //        string CarrierZoneNmae = string.Empty;
                //        using (DB _db = GetDB())
                //        {
                //            IEnumerable<VShelfInfo> shelfInfos = _Repositories.GetAllShelfInfoByHostEQPortID(_db, portDefInfo.HostEQPortID);
                //            foreach (VShelfInfo info in shelfInfos)
                //            {
                //                CarrierZoneNmae = info.ZoneID;
                //                if (!string.IsNullOrWhiteSpace(info.CSTID))
                //                {
                //                    bool existsCommand = _Repositories.GetTransferCmdByCarrierID(_db, info.CSTID, out TransferCommand transferCommand);

                //                    bool isDeleteCarrierDataSuccess = _Repositories.DeleteCST("", "", info.ShelfID, info.Stage, info.CSTID, defaultMessage);
                //                    if (!isDeleteCarrierDataSuccess)
                //                        return;

                //                    if (existsCommand)
                //                    {
                //                        bool isDeleteTransferAndTaskSuccess = DeleteTransferAndTask(_db, transferCommand.CommandID, portDefInfo.HostEQPortID, info.CSTID, message, defaultMessage);
                //                        if (!isDeleteTransferAndTaskSuccess)
                //                            return;
                //                    }

                //                    if (existsCommand)
                //                    {
                //                        UpdateCommandInfo infos = _Repositories.GetCommandInfoByCommandID(_db, transferCommand.CommandID);
                //                        if (transferCommand.TransferState == (int)TransferState.Queue)
                //                        {
                //                            _DataCollectionEventsService.OnTransferCancelInitiated(this, new TransferCancelInitiatedEventArgs(infos, portDefInfo.HostEQPortID));
                //                            _DataCollectionEventsService.OnTransferCancelCompleted(this, new TransferCancelCompletedEventArgs(infos, portDefInfo.HostEQPortID));
                //                        }
                //                        else
                //                        {
                //                            _DataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(infos, portDefInfo.HostEQPortID, VIDEnums.ResultCode.Successful));
                //                        }
                //                    }

                //                    if (portDefInfo.ReportStage > 1)
                //                        _DataCollectionEventsService.OnCarrierWaitOut(this, new CarrierWaitOutEventArgs(info.CSTID, info.ZoneID, VIDEnums.PortType.LP));

                //                    if (info.PortLocationType == (int)PortLocationType.MGVPort || info.PortLocationType == (int)PortLocationType.ViewPort)
                //                        _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(info.CSTID, VIDEnums.HandoffType.Manual, info.CarrierLoc));
                //                    else
                //                        _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(info.CSTID, VIDEnums.HandoffType.Automated, info.CarrierLoc));
                //                }
                //            }
                //        }

                //        //switch (args.NewDirection)
                //        //{
                //        //    case StockerEnums.IOPortDirection.InMode:
                //        //        _DataCollectionEventsService.OnPortTypeChanging(this, new PortStateEventArgs(CarrierZoneNmae));
                //        //        _DataCollectionEventsService.OnPortTypeInput(this, new PortStateEventArgs(CarrierZoneNmae));
                //        //        break;
                //        //    case StockerEnums.IOPortDirection.OutMode:
                //        //        _DataCollectionEventsService.OnPortTypeChanging(this, new PortStateEventArgs(CarrierZoneNmae));
                //        //        _DataCollectionEventsService.OnPortTypeOutput(this, new PortStateEventArgs(CarrierZoneNmae));
                //        //        break;
                //        //    case StockerEnums.IOPortDirection.ModeChanging:
                //        //        break;
                //        //    case StockerEnums.IOPortDirection.None:
                //        //        break;
                //        //}
                //    }
                //    else
                //    {
                //        message = defaultMessage + ", Get PortDef Fail, Please Check";
                //        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOId}", message));
                //    }
                //}
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        #region Stage/Vehicle
        protected override void IOStage_OnCstidChanged(object sender, IOStageEventArgs args)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(args.CstId))
                {
                    PortDefInfo portDefInfo = _TaskInfo.GetIOPortInfo(args.IOPortId);
                    if (portDefInfo != null)
                    {
                        string message = $"IO Port Stage CarrierID Changed, CarrierID:{args.CstId}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, args.StageId, message));
                    }
                    else
                    {
                        string message = $"IO Port Stage CarrierID Changed, Get PortDef Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOPortId}", message));
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void IOVehicle_OnCstidChanged(object sender, IOVehicleEventArgs args)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(args.CstId))
                {
                    PortDefInfo portDefInfo = _TaskInfo.GetIOPortInfo(args.IOPortId);
                    if (portDefInfo != null)
                    {
                        string message = $"IO Port Vehicle CarrierID Changed, CarrierID:{args.CstId}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, args.VehicleId + 5, message));
                    }
                    else
                    {
                        string message = $"IO Port Vehicle CarrierID Changed, Get PortDef Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOPortId}", message));
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void IOStage_OnLoadPresenceChanged(object sender, IOStageEventArgs args)
        {
            try
            {
                var PortReportFlag = _Repositories.GetAllPortDef(GetDB()).FirstOrDefault(i => i.PortType == (int)PortType.IO && i.PortTypeIndex == args.IOPortId).ReportMCSFlag;

                PortDefInfo portDefInfo = _TaskInfo.GetIOPortInfo(args.IOPortId);
                IIOPort port = _Stocker.GetIOPortById(portDefInfo.PortTypeIndex);
                string defaultMessage = $"IO Port Stage Presence:{(args.SignalIsOn ? "ON" : "OFF")}";
                string message = string.Empty;
                if (portDefInfo != null)
                {
                    if (port.Direction == StockerEnums.IOPortDirection.OutMode)
                    {
                        if (args.SignalIsOn)
                            IOStage_OnLoadPresenceOn(portDefInfo, args.StageId, args.CstId, PortReportFlag);
                        else
                            IOStage_OnLoadPresenceOff(portDefInfo, args.StageId, PortReportFlag);
                    }
                }
                else
                {
                    message = defaultMessage + $", Get PortDef Fail, Please Check";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOPortId}", message));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }
        private void IOStage_OnLoadPresenceOff(PortDefInfo portDefInfo, int stage, bool PortReportFlag)
        {
            try
            {
                string defaultMessage = $"IO Port Stage Presence:OFF";
                string message = string.Empty;
                IIOPort port = _Stocker.GetIOPortById(portDefInfo.PortTypeIndex);

                using (DB _db = GetDB())
                {
                    string strEM = "";
                    message = defaultMessage;
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));

                    IEnumerable<VShelfInfo> shelfInfos = _Repositories.GetAllShelfInfoByHostEQPortID(_db, portDefInfo.HostEQPortID);
                    if (shelfInfos.Any())
                    {
                        foreach (VShelfInfo shelfInfo in shelfInfos)
                        {
                            if (portDefInfo.ShelfID == shelfInfo.ShelfID && shelfInfo.Stage == stage && portDefInfo.ReportStage == stage)
                            {
                                if (!string.IsNullOrWhiteSpace(shelfInfo.CSTID))
                                {
                                    #region UpdateShelfDef
                                    int iResult = _Repositories.UpdateShelfDef(_db, shelfInfo.ShelfID, stage, ShelfState.EmptyShelf, ref strEM);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        iResult = _Repositories.UpdateShelfDef(_db, shelfInfo.ShelfID, stage, ShelfState.EmptyShelf, ref strEM);
                                    }

                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", SheldID:{shelfInfo.ShelfID}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Fail, Message={strEM}";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    else
                                    {
                                        message = defaultMessage + $", SheldID:{shelfInfo.ShelfID}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Success";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    #endregion UpdateShelfDef

                                    #region DeleteCarrierData
                                    iResult = _Repositories.DeleteCarrierData(_db, shelfInfo.CSTID, shelfInfo.ShelfID, ref strEM);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        iResult = _Repositories.DeleteCarrierData(_db, shelfInfo.CSTID, shelfInfo.ShelfID, ref strEM);
                                    }

                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", CarrierID:{shelfInfo.CSTID},  Delete CassetteData Fail, Message={strEM}";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    else
                                    {
                                        message = defaultMessage + $", CarrierID:{shelfInfo.CSTID},  Delete CassetteData Success";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    #endregion DeleteCarrierData

                                    #region InsertCarrierDataToHistory
                                    iResult = _Repositories.InsertCarrierDataToHistory(_db, shelfInfo.CSTID, shelfInfo.ShelfID, ref strEM);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        iResult = _Repositories.InsertCarrierDataToHistory(_db, shelfInfo.CSTID, shelfInfo.ShelfID, ref strEM);
                                    }

                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", CarrierID:{shelfInfo.CSTID}, Insert HisCassetteData Fail, Message={strEM}";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    else
                                    {
                                        message = defaultMessage + $", CarrierID:{shelfInfo.CSTID}, Insert HisCassetteData Success";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    #endregion InsertCarrierDataToHistory

                                    if (portDefInfo.PortLocationType == PortLocationType.MGVPort || portDefInfo.PortLocationType == PortLocationType.ViewPort)
                                    {
                                        if (_TaskInfo.Config.SystemConfig.CarrierRemoveFromPortWhenPresenceOff == ((char)Enable.Enable).ToString() && PortReportFlag)
                                            _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Manual, shelfInfo.CarrierLoc));
                                    }
                                    else
                                    {
                                        if (_TaskInfo.Config.SystemConfig.CarrierRemoveFromPortWhenPresenceOff == ((char)Enable.Enable).ToString() && PortReportFlag)
                                            _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Automated, shelfInfo.CarrierLoc));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        message = defaultMessage + $", Get ShelfInfo Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                    }

                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }
        private void IOStage_OnLoadPresenceOn(PortDefInfo portDefInfo, int stage, string plcCarrierID, bool PortReportFlag)
        {
            try
            {
                string defaultMessage = $"IO Port Stage Presence:ON, PLCCarrierID:{plcCarrierID}";
                string message = string.Empty;
                IIOPort port = _Stocker.GetIOPortById(portDefInfo.PortTypeIndex);

                using (DB _db = GetDB())
                {
                    string strEM = "";
                    DateTime timeout = DateTime.Now.AddMilliseconds(1000);
                    while (string.IsNullOrWhiteSpace(plcCarrierID) && DateTime.Now < timeout)
                    {
                        Task.Delay(50).Wait();
                        plcCarrierID = port.GetStageById(stage).CstId;
                    }
                    message = defaultMessage;
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));

                    IEnumerable<VShelfInfo> shelfInfos = _Repositories.GetShelfInfoLikeCarrierID(_db, plcCarrierID);
                    if (shelfInfos.Any())
                    {
                        foreach (VShelfInfo shelfInfo in shelfInfos)
                        {
                            if (portDefInfo.ShelfID == shelfInfo.ShelfID && stage > 1)
                            {
                                int iResult = ErrorCode.Initial;

                                if (!string.IsNullOrWhiteSpace(shelfInfo.CSTID))
                                {
                                    #region UpdateShelfDef
                                    iResult = _Repositories.UpdateShelfDef(_db, shelfInfo.ShelfID, shelfInfo.Stage, ShelfState.EmptyShelf, ref strEM);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        iResult = _Repositories.UpdateShelfDef(_db, shelfInfo.ShelfID, shelfInfo.Stage, ShelfState.EmptyShelf, ref strEM);
                                    }

                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{shelfInfo.Stage}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Fail, Message={strEM}";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    else
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{shelfInfo.Stage}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Success";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    #endregion UpdateShelfDef

                                    #region UpdateShelfDef
                                    iResult = _Repositories.UpdateShelfDef(_db, shelfInfo.ShelfID, stage, ShelfState.Stored, ref strEM);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        iResult = _Repositories.UpdateShelfDef(_db, shelfInfo.ShelfID, stage, ShelfState.Stored, ref strEM);
                                    }

                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{stage}, ShelfState:{(char)ShelfState.Stored}, Update ShelfDef Fail, Message={strEM}";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    else
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{stage}, ShelfState:{(char)ShelfState.Stored}, Update ShelfDef Success";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    #endregion UpdateShelfDef

                                    #region UpdateCarrierData

                                    var carrierState = portDefInfo.ReportStage == stage ? CarrierState.WaitOutLP : CarrierState.WaitOut;
                                    iResult = _Repositories.UpdateCarrierData(_db, shelfInfo.CSTID, shelfInfo.ShelfID, stage, carrierState, ref strEM);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        iResult = _Repositories.UpdateCarrierData(_db, shelfInfo.CSTID, shelfInfo.ShelfID, stage, carrierState, ref strEM);
                                    }

                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{stage}, CarrierState:{(int)CarrierState.Transfering}, Update CarrierData Fail, Message={strEM}";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    else
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{stage}, CarrierState:{(int)CarrierState.Transfering}, Update CarrierData Success";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    #endregion UpdateCarrierData

                                    if (portDefInfo.ReportStage == stage)
                                    {
                                        if (_TaskInfo.Config.SystemConfig.CarrierRemoveFromPortWhenPresenceOff == ((char)Enable.Disable).ToString())
                                        {
                                            bool isDeleteCarrierDataSuccess = _Repositories.DeleteCST("", "", shelfInfo.ShelfID, stage, shelfInfo.CSTID, defaultMessage);
                                            if (!isDeleteCarrierDataSuccess)
                                                return;
                                        }

                                        if (_Repositories.GetTransferCmdByCarrierID(_db, shelfInfo.CSTID, out Stocker.TaskControl.Info.TransferCommand transfer))
                                        {
                                            if (Convert.ToInt32(transfer.Destination) == portDefInfo.PLCPortID)
                                            {
                                                //UpdateCommandInfo infos = _Repositories.GetCommandInfoByCommandID(_db, transfer.CommandID);

                                                bool isDeleteTransferAndTaskSuccess = DeleteTransferAndTask(_db, transfer.CommandID, portDefInfo.HostEQPortID, plcCarrierID, message, defaultMessage);
                                                if (!isDeleteTransferAndTaskSuccess)
                                                    return;

                                                //if (portDefInfo.ReportStage != 1)
                                                //{
                                                //    _DataCollectionEventsService.OnTransferCompleted(this, new TransferCompletedEventArgs(infos, portDefInfo.HostEQPortID, VIDEnums.ResultCode.Successful));
                                                //}
                                            }
                                        }

                                        if (PortReportFlag)
                                        {
                                            _DataCollectionEventsService.OnCarrierWaitOut(this, new CarrierWaitOutEventArgs(shelfInfo.CSTID, portDefInfo.HostEQPortID, VIDEnums.PortType.LP));
                                            var HandoffType = portDefInfo.PortLocationType == PortLocationType.MGVPort ? VIDEnums.HandoffType.Manual : VIDEnums.HandoffType.Automated;
                                            if (_TaskInfo.Config.SystemConfig.CarrierRemoveFromPortWhenPresenceOff == ((char)Enable.Disable).ToString())
                                                _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, HandoffType, shelfInfo.CarrierLoc));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                message = defaultMessage + $", Carrier Location On {shelfInfo.ShelfID}, Please Check";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                            }
                        }
                    }
                    else
                    {
                        message = defaultMessage + $", Get ShelfInfo Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                    }

                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void IOVehicle_OnLoadPresenceChanged(object sender, IOVehicleEventArgs args)
        {
            try
            {
                var PortReportFlag = _Repositories.GetAllPortDef(GetDB()).FirstOrDefault(i => i.PortType == (int)PortType.IO && i.PortTypeIndex == args.IOPortId).ReportMCSFlag;

                PortDefInfo portDefInfo = _TaskInfo.GetIOPortInfo(args.IOPortId);
                IIOPort port = _Stocker.GetIOPortById(portDefInfo.PortTypeIndex);
                string defaultMessage = $"IO Port Vehicle Presence:{(args.SignalIsOn ? "ON" : "OFF")}";
                string message = string.Empty;
                if (portDefInfo != null)
                {
                    if (port.Direction == StockerEnums.IOPortDirection.OutMode)
                    {
                        if (args.SignalIsOn)
                            IOVehicle_OnLoadPresenceOn(portDefInfo, args.VehicleId + 5, args.CstId, PortReportFlag);
                        else
                            IOVehicle_OnLoadPresenceOff(portDefInfo, args.VehicleId + 5, PortReportFlag);
                    }
                }
                else
                {
                    message = defaultMessage + $", Get PortDef Fail, Please Check";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"IO PortTypeIndex:{args.IOPortId}", message));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }
        private void IOVehicle_OnLoadPresenceOff(PortDefInfo portDefInfo, int stage, bool PortReportFlag)
        {
            try
            {
                string defaultMessage = $"IO Port Vehicle Presence:OFF";
                string message = string.Empty;
                IIOPort port = _Stocker.GetIOPortById(portDefInfo.PortTypeIndex);

                using (DB _db = GetDB())
                {
                    message = defaultMessage;
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));

                    IEnumerable<VShelfInfo> shelfInfos = _Repositories.GetAllShelfInfoByHostEQPortID(_db, portDefInfo.HostEQPortID);
                    if (shelfInfos.Any())
                    {
                        foreach (VShelfInfo shelfInfo in shelfInfos)
                        {
                            if (portDefInfo.ShelfID == shelfInfo.ShelfID && shelfInfo.Stage == stage && portDefInfo.ReportStage == stage)
                            {
                                if (!string.IsNullOrWhiteSpace(shelfInfo.CSTID))
                                {
                                    _Repositories.DeleteCST("", "", shelfInfo.ShelfID, shelfInfo.Stage, shelfInfo.CSTID, defaultMessage);

                                    if (PortReportFlag)
                                    {
                                        if (portDefInfo.PortLocationType == PortLocationType.MGVPort)
                                        {
                                            if (_TaskInfo.Config.SystemConfig.CarrierRemoveFromPortWhenPresenceOff == ((char)Enable.Enable).ToString())
                                                _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Manual, shelfInfo.CarrierLoc));
                                        }
                                        else
                                        {
                                            if (_TaskInfo.Config.SystemConfig.CarrierRemoveFromPortWhenPresenceOff == ((char)Enable.Enable).ToString())
                                                _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Automated, shelfInfo.CarrierLoc));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                message = defaultMessage + $", Carrier Location On {shelfInfo.ShelfID}, Please Check";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                            }
                        }
                    }
                    else
                    {
                        message = defaultMessage + $", Get ShelfInfo Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                    }

                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }
        private void IOVehicle_OnLoadPresenceOn(PortDefInfo portDefInfo, int stage, string plcCarrierID, bool PortReportFlag)
        {
            try
            {
                string defaultMessage = $"IO Port Vehicle Presence:ON, PLCCarrierID:{plcCarrierID}";
                string message = string.Empty;
                IIOPort port = _Stocker.GetIOPortById(portDefInfo.PortTypeIndex);

                using (DB _db = GetDB())
                {
                    string strEM = "";
                    DateTime timeout = DateTime.Now.AddMilliseconds(1000);
                    while (string.IsNullOrWhiteSpace(plcCarrierID) && DateTime.Now < timeout)
                    {
                        Task.Delay(50).Wait();
                        plcCarrierID = port.GetStageById(stage).CstId;
                    }
                    message = defaultMessage;
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));

                    IEnumerable<VShelfInfo> shelfInfos = _Repositories.GetShelfInfoLikeCarrierID(_db, plcCarrierID);
                    if (shelfInfos.Any())
                    {
                        foreach (VShelfInfo shelfInfo in shelfInfos)
                        {
                            if (portDefInfo.ShelfID == shelfInfo.ShelfID)
                            {
                                int iResult = ErrorCode.Initial;

                                if (!string.IsNullOrWhiteSpace(shelfInfo.CSTID))
                                {
                                    #region UpdateShelfDef
                                    iResult = _Repositories.UpdateShelfDef(_db, shelfInfo.ShelfID, shelfInfo.Stage, ShelfState.EmptyShelf, ref strEM);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{shelfInfo.Stage}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Fail, Message={strEM}";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    else
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{shelfInfo.Stage}, ShelfState:{(char)ShelfState.EmptyShelf}, Update ShelfDef Success";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    #endregion UpdateShelfDef

                                    #region UpdateShelfDef
                                    iResult = _Repositories.UpdateShelfDef(_db, shelfInfo.ShelfID, stage, ShelfState.Stored, ref strEM);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{stage}, ShelfState:{(char)ShelfState.Stored}, Update ShelfDef Fail, Message={strEM}";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    else
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{stage}, ShelfState:{(char)ShelfState.Stored}, Update ShelfDef Success";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    #endregion UpdateShelfDef

                                    #region UpdateCarrierData
                                    iResult = _Repositories.UpdateCarrierData(_db, shelfInfo.CSTID, shelfInfo.ShelfID, stage, CarrierState.Transfering, ref strEM);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{stage}, CarrierState:{(int)CarrierState.Transfering}, Update CarrierData Fail, Message={strEM}";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    else
                                    {
                                        message = defaultMessage + $", ShelfID:{shelfInfo.ShelfID}, Stage:{stage}, CarrierState:{(int)CarrierState.Transfering}, Update CarrierData Success";
                                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                                    }
                                    #endregion UpdateCarrierData

                                    if (portDefInfo.ReportStage == stage)
                                    {
                                        if (_Repositories.GetTransferCmdByCarrierID(_db, shelfInfo.CSTID, out Stocker.TaskControl.Info.TransferCommand transfer))
                                        {
                                            if (Convert.ToInt32(transfer.FinishLocation) == portDefInfo.PLCPortID)
                                            {
                                                DeleteTransferAndTask(_db, transfer.CommandID, portDefInfo.HostEQPortID, shelfInfo.CSTID, message, defaultMessage);

                                                if (portDefInfo.ReportStage != 1)
                                                {
                                                    if (PortReportFlag)
                                                    {
                                                        UpdateCommandInfo infos = _Repositories.GetCommandInfoByCommandID(_db, transfer.CommandID);
                                                        _DataCollectionEventsService.OnTransferCompleted(this,
                                                            new TransferCompletedEventArgs(infos, portDefInfo.HostEQPortID, VIDEnums.ResultCode.Successful));
                                                    }
                                                }
                                            }
                                        }

                                        if (PortReportFlag)
                                        {
                                            _DataCollectionEventsService.OnCarrierWaitOut(this, new CarrierWaitOutEventArgs(shelfInfo.CSTID, portDefInfo.HostEQPortID, VIDEnums.PortType.LP));
                                            if (portDefInfo.PortLocationType == PortLocationType.MGVPort)
                                            {
                                                if (_TaskInfo.Config.SystemConfig.CarrierRemoveFromPortWhenPresenceOff == ((char)Enable.Disable).ToString())
                                                    _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Manual, shelfInfo.CarrierLoc));
                                            }
                                            else
                                            {
                                                if (_TaskInfo.Config.SystemConfig.CarrierRemoveFromPortWhenPresenceOff == ((char)Enable.Disable).ToString())
                                                    _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Automated, shelfInfo.CarrierLoc));
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                message = defaultMessage + $", Carrier Location On {shelfInfo.ShelfID}, Please Check";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                            }
                        }
                    }
                    else
                    {
                        message = defaultMessage + $", Get ShelfInfo Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, stage, message));
                    }

                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }
        #endregion Stage/Vehicle

        #endregion IO Port

        #region EQ Port
        protected override void EQ_OnCstidChanged(object sender, EQEventArgs args)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(args.CstId))
                {
                    PortDefInfo portDefInfo = _TaskInfo.GetEQPortInfo(args.EQId);
                    if (portDefInfo != null)
                    {
                        string message = $"EQ Port CarrierID Changed, CarrierID:{args.CstId}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));
                    }
                    else
                    {
                        string message = $"EQ Port CarrierID Changed, Get PortDef Fail, Please Check";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"EQ PortTypeIndex:{args.EQId}", message));
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void EQ_OnInServiceChanged(object sender, EQEventArgs args)
        {
            try
            {
                PortDefInfo portDefInfo = _TaskInfo.GetEQPortInfo(args.EQId);
                bool inService = args.SignalIsOn;
                bool enable = false;
                string message = string.Empty;
                string defaultMessage = $"EQ Port Service Changed, EQ Port Service:{(inService ? "ON" : "OFF")}";

                if (portDefInfo != null)
                {
                    IEQPort port = _Stocker.GetEQPortById(args.EQId);

                    using (DB _db = GetDB())
                    {
                        if (_Repositories.GetShelfInfoByHostEQPortID(_db, portDefInfo.HostEQPortID, out VShelfInfo shelfInfo))
                        {
                            enable = shelfInfo.Enable;

                            message = defaultMessage + $", Enable:{(enable ? "ON" : "OFF")}";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));

                            //if (inService && port.Status != StockerEnums.EQPortStatus.ERROR && port.IsInService && enable)
                            //    _DataCollectionEventsService.OnPortInService(this, new PortStateEventArgs(shelfInfo.ZoneID));
                            //else
                            //    _DataCollectionEventsService.OnPortOutOfService(this, new PortStateEventArgs(shelfInfo.ZoneID));
                        }
                        else
                        {
                            message = defaultMessage + $", Enable:{(enable ? "ON" : "OFF")}, Get ShelfInfo Fail, Please Check";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));
                        }
                    }
                }
                else
                {
                    message = defaultMessage + ", Get PortDef Fail, Please Check";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"EQ PortTypeIndex:{args.EQId}", message));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void EQ_OnLoadRequestStatusChanged(object sender, EQEventArgs args)
        {
            try
            {
                PortDefInfo portDefInfo = _TaskInfo.GetEQPortInfo(args.EQId);
                string message = string.Empty;
                string defaultMessage = $"EQ Port Request Status Changed, Port Status:{args.NewLoadRequestStatus}";

                if (portDefInfo != null)
                {
                    message = defaultMessage;
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));

                    using (DB _db = GetDB())
                    {
                        if (_Repositories.GetShelfInfoByHostEQPortID(_db, portDefInfo.HostEQPortID, out VShelfInfo shelfInfo))
                        {
                            if (args.NewLoadRequestStatus == StockerEnums.PortLoadRequestStatus.Unload)
                            {
                                var eqPortDef = _Repositories.GetAllPortDef(_db).FirstOrDefault(p => p.HostEQPortID == portDefInfo.HostEQPortID);
                                if (portDefInfo.PortLocationType == PortLocationType.EQPort_CassetteInOut
                                    && eqPortDef != null && eqPortDef.PRESENTON_INSCST)
                                {
                                    IEQPort port = _Stocker.GetEQPortById(args.EQId);
                                    PortWaitInScenarios(_db, portDefInfo, port.CSTID, true);
                                }
                            }
                        }
                        else
                        {
                            message = defaultMessage + ", Get ShelfInfo Fail, Please Check";
                            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));
                        }
                    }
                }
                else
                {
                    message = defaultMessage + ", Get PortDef Fail, Please Check";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"EQ PortTypeIndex:{args.EQId}", message));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        protected override void EQ_OnCSTPresentChanged(object sender, EQEventArgs args)
        {
            if (args.SignalIsOn)
                EQ_PortCSTPresentOn(args.EQId);
            else
                EQ_PortCSTPresentOff(args.EQId);
        }

        private void EQ_PortCSTPresentOn(int portTypeIndex)
        {
            try
            {
                string message = string.Empty;
                string defaultMessage = $"EQ Port Present Changed, Present:On";
                PortDefInfo portDefInfo = _TaskInfo.GetEQPortInfo(portTypeIndex);
                if (portDefInfo != null)
                {
                    using (DB _db = GetDB())
                    {
                        _Repositories.GetShelfInfoByHostEQPortID(_db, portDefInfo.HostEQPortID, out VShelfInfo shelfInfo);
                        IEQPort port = _Stocker.GetEQPortById(portTypeIndex);
                        message = defaultMessage + $", PLC CarrierID:{port.CSTID}";
                        //_DataCollectionEventsService.OnEqPresence(this, new PortStateEventArgs(shelfInfo.ZoneID));
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));
                    }
                }
                else
                {
                    message = defaultMessage + $", Get PortDef Fail, Please Check";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"EQ PortTypeIndex:{portTypeIndex}", message));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void EQ_PortCSTPresentOff(int portTypeIndex)
        {
            try
            {
                PortDefInfo portDefInfo = _TaskInfo.GetEQPortInfo(portTypeIndex);
                string message = string.Empty;
                string defaultMessage = $"EQ Port Present Changed, Present:Off";

                if (portDefInfo != null)
                {
                    IEQPort port = _Stocker.GetEQPortById(portTypeIndex);
                    if (portDefInfo.PortLocationType == PortLocationType.EQPort_CassetteInOut && port.LoadRequestStatus != StockerEnums.PortLoadRequestStatus.Unload)
                    {
                        message = defaultMessage;
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));

                        using (DB _db = GetDB())
                        {
                            var transfer = _Repositories.GetAllTransferCmd(_db).Where(i => i.HostSource == portDefInfo.HostEQPortID);
                            if (transfer.Any())
                            {
                                message = defaultMessage + $", But Port Is Transfer Command, Please Check";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));
                                return;
                            }

                            IEnumerable<VShelfInfo> shelfInfos = _Repositories.GetAllShelfInfoByHostEQPortID(_db, portDefInfo.HostEQPortID);
                            foreach (VShelfInfo shelfInfo in shelfInfos)
                            {
                                string carrierID = shelfInfo.CSTID;

                                bool isDeleteCarrierDataSuccess = _Repositories.DeleteCST("", "", shelfInfos.First().ShelfID, shelfInfo.Stage, carrierID, defaultMessage);
                                if (!isDeleteCarrierDataSuccess)
                                    continue;

                                _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Automated, shelfInfo.CarrierLoc));

                                message = defaultMessage + $", CarrierRemoved CSTID:{shelfInfo.CSTID}";
                                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));
                            }
                        }
                    }
                    using (DB _db = GetDB())
                    {
                        _Repositories.GetShelfInfoByHostEQPortID(_db, portDefInfo.HostEQPortID, out VShelfInfo shelfInfo);
                        //_DataCollectionEventsService.OnEqNoPresence(this, new PortStateEventArgs(shelfInfo.ZoneID));
                        message = defaultMessage + $", PLC CarrierID:{port.CSTID}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDefInfo.HostEQPortID, message));
                    }
                }
                else
                {
                    message = defaultMessage + $", Get PortDef Fail, Please Check";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace($"EQ PortTypeIndex:{portTypeIndex}", message));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }
        #endregion EQ Port

        private void PortWaitInScenarios(DB _db, PortDefInfo portDef, string carrierID, bool PortReportFlag)
        {
            try
            {
                int iResult = ErrorCode.Initial;
                string carrier_Waitin = carrierID;
                string message = string.Empty;
                string defaultmessage = $"WaitIn On {portDef.PortType.ToString()}Port:{portDef.HostEQPortID}";
                VShelfInfo shelfInfo = null;

                message = defaultmessage + $", PLC CarrierID:{carrier_Waitin}";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, carrierID, message));

                shelfInfo = _Repositories.GetShelfInfoByHostEQPortID(_db, portDef.HostEQPortID, string.Empty);
                if (shelfInfo == null)
                {
                    message = defaultmessage + $", Get Shelf Fail";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, carrierID, message));
                    return;
                }

                if (!string.IsNullOrWhiteSpace(shelfInfo.CSTID))
                {
                    message = defaultmessage + $", LCS CarrierID:{shelfInfo.CSTID}";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, carrierID, message));

                    if (carrier_Waitin != _ReplyCSTID_NoRead || carrier_Waitin != _ReplyCSTID_ReadFail)
                    {
                        bool isDeleteCSTOnPortSuccess = _Repositories.DeleteCST("", "", shelfInfo.ShelfID, 1, shelfInfo.CSTID, defaultmessage);
                        if (!isDeleteCSTOnPortSuccess)
                            return;
                        if (PortReportFlag)
                        {
                            if (portDef.PortLocationType == PortLocationType.MGVPort)
                                _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Manual, shelfInfo.CarrierLoc));
                            //else
                            //    _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Automated, shelfInfo.CarrierLoc));
                            else if (portDef.PortType == PortType.EQ)
                            {
                                _DataCollectionEventsService.OnCarrierRemovedCompleted(this, new CarrierRemoveCompletedEventArgs(shelfInfo.CSTID, shelfInfo.CarrierLoc, shelfInfo.ZoneID));
                            }
                            else
                            {
                                _DataCollectionEventsService.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelfInfo.CSTID, VIDEnums.HandoffType.Automated, shelfInfo.CarrierLoc));
                            }
                        }
                    }
                }

                if (carrier_Waitin == _ReplyCSTID_NoRead || carrier_Waitin == _ReplyCSTID_ReadFail || carrier_Waitin == _ReplyCSTID_NoCST)
                {
                    string unknowCarrierID = _Repositories.GetAbnormalCSTID(AbnormalCSTIDType.Failure, shelfInfo.CarrierLoc, string.Empty);

                    bool isUnknowCSTOnPortSuccess = _Repositories.InsertCST("", "", portDef.ShelfID, 1, unknowCarrierID, CarrierState.WaitIn, defaultmessage);
                    if (!isUnknowCSTOnPortSuccess)
                        return;

                    _Repositories.InsertIdReadErrorLog(shelfInfo.CarrierLoc, unknowCarrierID);

                    if (PortReportFlag)
                    {
                        _DataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(unknowCarrierID, shelfInfo.HostEQPort, VIDEnums.IDReadStatus.Failure));
                        _DataCollectionEventsService.OnCarrierWaitIn(this, new CarrierWaitInEventArgs(unknowCarrierID, shelfInfo.CarrierLoc, shelfInfo.ZoneID));
                    }
                }
                else
                {
                    if (_Repositories.GetShelfInfoByCarrierID(_db, carrier_Waitin, out VShelfInfo duplicateShelfInfo))
                    {
                        if (duplicateShelfInfo.ShelfType == (int)ShelfType.Shelf || duplicateShelfInfo.ShelfType == (int)ShelfType.Crane)
                            WaitInWhenDuplicateOnShelfOrCrane(_db, carrier_Waitin, portDef, duplicateShelfInfo, PortReportFlag);
                        else if (duplicateShelfInfo.ShelfType == (int)ShelfType.Port)
                            WaitInWhenDuplicateOnPort(_db, carrier_Waitin, portDef, duplicateShelfInfo, PortReportFlag);
                    }
                    else
                    {
                        bool isCSTWaitInOnPortSuccess = _Repositories.InsertCST("", "", portDef.ShelfID, 1, carrier_Waitin, CarrierState.WaitIn, defaultmessage);
                        if (!isCSTWaitInOnPortSuccess)
                            return;

                        if (PortReportFlag)
                        {
                            _DataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(carrier_Waitin, shelfInfo.HostEQPort, VIDEnums.IDReadStatus.Successful));
                            _DataCollectionEventsService.OnCarrierWaitIn(this, new CarrierWaitInEventArgs(carrier_Waitin, shelfInfo.CarrierLoc, shelfInfo.ZoneID));
                        }
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void WaitInWhenDuplicateOnShelfOrCrane(DB _db, string waitInCarrierID, PortDefInfo portDef, VShelfInfo duplicateShelfInfo, bool PortReportFlag)
        {
            int iResult = ErrorCode.Initial;
            string unknowCarrierID = string.Empty;
            string message = string.Empty;
            string defaultMessage = $"WaitIn On {portDef.PortType.ToString()}Port:{portDef.HostEQPortID}, Duplicate On Shelf";
            try
            {
                var shelfInfo = _Repositories.GetShelfInfoByHostEQPortID(_db, portDef.HostEQPortID, string.Empty);
                unknowCarrierID = _Repositories.GetAbnormalCSTID(AbnormalCSTIDType.Duplicate, portDef.HostEQPortID, waitInCarrierID);

                if (_Repositories.GetTransferCmdByCarrierID(_db, waitInCarrierID, out Stocker.TaskControl.Info.TransferCommand transfer))
                {
                    bool isDuplicateCSTOnPortSuccess = _Repositories.InsertCST("", "", portDef.ShelfID, 1, unknowCarrierID, CarrierState.WaitIn, defaultMessage);
                    if (!isDuplicateCSTOnPortSuccess)
                        return;

                    if (PortReportFlag)
                    {
                        _DataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(waitInCarrierID, portDef.HostEQPortID, VIDEnums.IDReadStatus.Duplicate));
                        _DataCollectionEventsService.OnCarrierWaitIn(this, new CarrierWaitInEventArgs(unknowCarrierID, shelfInfo.CarrierLoc, shelfInfo.ZoneID));
                    }
                }
                else
                {
                    string strEM = "";
                    #region Begin
                    iResult = _db.CommitCtrl(DB.TransactionType.Begin);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", Begin Fail, Result:{iResult}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                        return;
                    }
                    message = defaultMessage + $", Begin Success";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                    #endregion Begin

                    #region UpdateShelfDef
                    iResult = _Repositories.UpdateShelfDef(_db, portDef.ShelfID, ShelfState.Stored, ref strEM);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", SheldID:{portDef.ShelfID}, ShelfState:{(char)ShelfState.Stored}, Update ShelfDef Fail, Message={strEM}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                        return;
                    }
                    message = defaultMessage + $", SheldID:{portDef.ShelfID}, ShelfState:{(char)ShelfState.Stored}, Update ShelfDef Success";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                    #endregion UpdateShelfDef

                    #region UpdateCarrierData
                    iResult = _Repositories.UpdateCarrierData(_db, waitInCarrierID, portDef.ShelfID, CarrierState.WaitIn, ref strEM);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", CarrierID:{waitInCarrierID}, CarrierState:{(int)CarrierState.WaitIn}, Update CassetteData Fail, Message:{strEM}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                        iResult = _db.CommitCtrl(DB.TransactionType.Rollback);
                        return;
                    }
                    message = defaultMessage + $", CarrierID:{waitInCarrierID}, CarrierState:{(int)CarrierState.WaitIn}, Update CassetteData Success";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                    #endregion UpdateCarrierData

                    #region InsertCarrierData
                    iResult = _Repositories.InsertCarrierData(_db, unknowCarrierID, duplicateShelfInfo.ShelfID, CarrierState.WaitIn, ref strEM);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", SheldID:{duplicateShelfInfo.ShelfID}, Carrier:{unknowCarrierID}, CarrierState:{(char)CarrierState.WaitIn}, Insert CarrierData Fail, Message={strEM}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                        return;
                    }
                    message = defaultMessage + $", SheldID:{duplicateShelfInfo.ShelfID}, Carrier:{unknowCarrierID}, CarrierState:{(int)CarrierState.WaitIn}, Insert CarrierData Success";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                    #endregion InsertCarrierData

                    #region Commit
                    iResult = _db.CommitCtrl(DB.TransactionType.Commit);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", Begin Fail, Result={iResult}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                        return;
                    }
                    message = defaultMessage + $", Commit Success";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                    #endregion Commit

                    #region InsertCarrierDataToHistory
                    iResult = _Repositories.InsertCarrierDataToHistory(_db, waitInCarrierID);
                    if (iResult != ErrorCode.Success)
                    {
                        message = defaultMessage + $", CarrierID:{waitInCarrierID}, Insert HisCassetteData Fail, Result={iResult}";
                        _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                    }
                    message = defaultMessage + $", CarrierID:{waitInCarrierID}, Insert HisCassetteData Success";
                    _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(portDef.HostEQPortID, 1, waitInCarrierID, message));
                    #endregion InsertCarrierDataToHistory

                    if (PortReportFlag)
                    {
                        _DataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(waitInCarrierID, portDef.HostEQPortID, VIDEnums.IDReadStatus.Duplicate));
                       _DataCollectionEventsService.OnCarrierWaitIn(this, new CarrierWaitInEventArgs(waitInCarrierID, shelfInfo.CarrierLoc, shelfInfo.ZoneID));
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void WaitInWhenDuplicateOnPort(DB _db, string waitInCarrierID, PortDefInfo portDef, VShelfInfo duplicateShelfInfo, bool PortReportFlag)
        {
            string message = string.Empty;
            string defaultMessage = $"WaitIn On {portDef.PortType.ToString()}Port:{portDef.HostEQPortID}, Duplicate On Port";
            try
            {
                var shelfInfo = _Repositories.GetShelfInfoByHostEQPortID(_db, portDef.HostEQPortID, string.Empty);
                string unknowCarrierID = _Repositories.GetAbnormalCSTID(AbnormalCSTIDType.Duplicate, portDef.HostEQPortID, waitInCarrierID);

                bool isDuplicateCSTOnPortSuccess = _Repositories.InsertCST("", "", portDef.ShelfID, 1, unknowCarrierID, CarrierState.WaitIn, defaultMessage);
                if (!isDuplicateCSTOnPortSuccess)
                    return;

                if (PortReportFlag)
                {
                    _DataCollectionEventsService.OnCarrierIDRead(this, new CarrierIDReadEventArgs(waitInCarrierID, portDef.HostEQPortID, VIDEnums.IDReadStatus.Duplicate));
                    _DataCollectionEventsService.OnCarrierWaitIn(this, new CarrierWaitInEventArgs(unknowCarrierID, shelfInfo.CarrierLoc, shelfInfo.ZoneID));
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }

        private bool DeleteTransferAndTask(DB _db, string CommandID, string HostEQPortID, string CarrierID, string message, string defaultMessage)
        {
            #region Begin
            int iResult = _db.CommitCtrl(DB.TransactionType.Begin);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Begin Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
                return false;
            }
            message = defaultMessage + ", Begin Success";
            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
            #endregion Begin

            #region InsertTransferToHistory
            iResult = _Repositories.InsertTransferToHistory(_db, CommandID);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", CommandID:{CommandID}, Insert HisTransfer Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
                _db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }
            message = defaultMessage + $", CommandID:{CommandID}, Insert HisTransfer Success";
            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
            #endregion InsertTransferToHistory

            #region DeleteTransfer
            iResult = _Repositories.DeleteTransfer(_db, CommandID);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", CommandID:{CommandID}, Delete Transfer Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
                _db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }
            message = defaultMessage + $", CommandID:{CommandID}, Delete Transfer Success";
            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
            #endregion DeleteTransfer

            #region InsertTaskToHistory
            iResult = _Repositories.InsertTaskToHistory(_db, CommandID);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", CommandID:{CommandID}, Insert HisTask Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
                _db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }
            message = defaultMessage + $", CommandID:{CommandID}, Insert HisTask Success";
            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
            #endregion InsertTaskToHistory

            #region DeleteTask
            iResult = _Repositories.DeleteTask(_db, CommandID);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", CommandID:{CommandID}, Delete Transfer Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
                _db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }
            message = defaultMessage + $", CommandID:{CommandID}, Delete Transfer Success";
            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
            #endregion DeleteTask

            #region Commit
            iResult = _db.CommitCtrl(DB.TransactionType.Commit);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Begin Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
                return false;
            }
            message = defaultMessage + ", Commit Success";
            _LoggerService.WriteLogTrace(new StockerMPLCEventTrace(HostEQPortID, 1, CarrierID, message));
            #endregion Commit

            return true;
        }
    }
}
