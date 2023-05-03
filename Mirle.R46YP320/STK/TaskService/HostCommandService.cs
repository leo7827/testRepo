using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mirle.DataBase;
using Mirle.LCS.Models;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.R46YP320.STK.MCS;
using Mirle.Stocker;
using Mirle.Stocker.Enums;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.Module;
using Mirle.Stocker.TaskControl.TraceLog;
using Mirle.Stocker.TaskControl.TraceLog.Format;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.TaskService
{
    public class HostCommandService : HostCommandModule
    {
        private readonly MCSCrane _craneReport;
        private readonly DataCollectionEventsService _dataCollectionEventsService;
        private readonly RepositoriesService _repositories;

        public HostCommandService(TaskInfo taskInfo, IStocker stocker, DataCollectionEventsModule dataCollectionEventsService, LoggerService loggerService, MCSCrane craneReport) : base(taskInfo, stocker, loggerService)
        {
            _dataCollectionEventsService = (DataCollectionEventsService)dataCollectionEventsService;
            _repositories = new RepositoriesService(taskInfo, loggerService);
            _craneReport = craneReport;
        }

        public DaifukuSpec.ACK.HCACK Scan(string carrierId, string carrierLoc)
        {
            using (var db = GetDB())
            {
                //Scan 沒有帶任何Carrier id 跟 位置 直接拒絕
                if (string.IsNullOrEmpty(carrierId) && string.IsNullOrEmpty(carrierLoc))
                    return DaifukuSpec.ACK.HCACK.NoSuchObjectExists;

                _repositories.GetShelfInfoByCarrierID(db, carrierId, out var source);

                string message;
                if (!string.IsNullOrEmpty(carrierLoc))
                {
                    if (!_repositories.GetShelfInfoByZoneID(db, carrierLoc, out source))
                    {
                        if (!_repositories.GetShelfInfoByCarrierLoc(db, carrierLoc, out source))
                        {
                            message = $"Get Carrier Location Fail";
                            _LoggerService.WriteLogTrace(new HostCommandTrace("Scan", carrierId, carrierLoc, message));
                            return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                        }
                    }
                }

                if (!source.Enable)
                {
                    message = $"The source is disable";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Scan", carrierId, carrierLoc, message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                if (source.ShelfType == (int)ShelfType.Port)
                {
                    message = $"Get Carrier Location On Port";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Scan", carrierId, carrierLoc, message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                if (source.ShelfType == (int)ShelfType.Crane)
                {
                    message = $"Get Carrier Location On Crane";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Scan", carrierId, carrierLoc, message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                var commandId = _repositories.GetScanCommandID(db);
                var iResult = _repositories.InsertScanCMD(db, commandId, carrierId, source, "MCS");
                if (iResult != ErrorCode.Success)
                {
                    message = $"Insert Scan Command Fail";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Scan", carrierId, carrierLoc, message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                message = $"Insert Scan Command Success";
                _LoggerService.WriteLogTrace(new HostCommandTrace("Scan", carrierId, carrierLoc, message));
                return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
            }
        }

        public DaifukuSpec.ACK.HCACK Abort(string commandId)
        {
            using (var db = GetDB())
            {
                string message;
                var transferCommands = _repositories.GetTransferCmdByCommandID(db, commandId, (int)TransferState.CommandFormatError);
                if (transferCommands == null)
                {
                    message = "The Command does not Exist";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Abort", commandId, 0, message));
                    return DaifukuSpec.ACK.HCACK.CommandDoesNotExist;
                }

                if (!_repositories.GetShelfInfoByCarrierID(db, transferCommands.CSTID, out var cassetteData))
                {
                    message = "This Carrier No Exist";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Abort", transferCommands.CSTID, cassetteData.ShelfID, message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                if (transferCommands.AbortFlag && transferCommands.TransferState == (int)TransferState.Aborting)
                {
                    message = "This Command is Aborting";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Abort", commandId, 0, message));
                    return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
                }

                if (transferCommands.TransferState != (int)TransferState.Aborting && cassetteData.CSTState != (int)VIDEnums.CarrierState.Alternate)
                {
                    message = $"TransferState:{transferCommands.TransferState}, Can't Perform";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Abort", commandId, 0, message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                var iResult = _repositories.AbortTransfer(db, commandId);
                if (iResult != ErrorCode.Success)
                {
                    message = $"Update Transfer Fail, Result={iResult}";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Abort", commandId, 0, message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                message = "Update Transfer Success";
                _LoggerService.WriteLogTrace(new HostCommandTrace("Abort", commandId, 0, message));
                return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
            }
        }

        public DaifukuSpec.ACK.HCACK Cancel(string commandId)
        {
            using (var db = GetDB())
            {
                string message;
                var transferCommands = _repositories.GetTransferCmdByCommandID(db, commandId);
                if (transferCommands == null)
                {
                    message = "The Command does not Exist";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Cancel", commandId, 0, message));
                    return DaifukuSpec.ACK.HCACK.CommandDoesNotExist;
                }

                if (transferCommands.CancelFlag || transferCommands.TransferState == (int)TransferState.Canceling)
                {
                    message = "This Command is Canceling";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Cancel", commandId, 0, message));
                    return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
                }

                if (transferCommands.TransferState != (int)TransferState.Queue)
                {
                    message = $"TransferState:{transferCommands.TransferState}, Can't Perform";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Cancel", commandId, 0, message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                var iResult = _repositories.CancelTransfer(db, commandId);
                if (iResult != ErrorCode.Success)
                {
                    message = $"Update Transfer Fail, Result={iResult}";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("Cancel", commandId, 0, message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                message = "Update Transfer Success";
                _LoggerService.WriteLogTrace(new HostCommandTrace("Cancel", commandId, 0, message));
                return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
            }
        }

        public DaifukuSpec.ACK.HCACK PriorityUpdate(string commandId, int priority)
        {
            using (var db = GetDB())
            {
                string message;
                var transferCommands = _repositories.GetTransferCmdByCommandID(db, commandId);
                if (transferCommands == null)
                {
                    message = $"The Command does not Exist";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("PriorityUpdate", commandId, priority, message));
                    return DaifukuSpec.ACK.HCACK.CommandDoesNotExist;
                }

                if (transferCommands.TransferState != (int)TransferState.Queue)
                {
                    message = "This Command is Executing";
                    _LoggerService.WriteLogTrace(new HostCommandTrace("PriorityUpdate", commandId, priority,message));
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                var shelfInfo = _repositories.GetShelfInfoByCarrierID(db, transferCommands.CSTID);
                var iResult = _repositories.UpdateMCSPriority(db, commandId, priority);
                if (iResult != ErrorCode.Success)
                {
                    Task.Delay(200).ContinueWith(t =>
                    {
                        _dataCollectionEventsService.OnPriorityUpdateFailed(this, new PriorityUpdateFailedEventArgs(transferCommands, shelfInfo.CarrierLoc));
                    });
                    message = $"Update Transfer Fail, Result={iResult}";
                }
                else
                {
                    Task.Delay(200).ContinueWith(t =>
                    {
                        _dataCollectionEventsService.OnPriorityUpdateCompleted(this, new PriorityUpdateCompletedEventArgs(transferCommands, shelfInfo.CarrierLoc,
                                priority));
                    });
                    message = "Update Transfer Success";
                }

                _LoggerService.WriteLogTrace(new HostCommandTrace("PriorityUpdate", commandId, priority, message));
                return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
            }
        }

        public DaifukuSpec.ACK.HCACK Remove(string carrierId)
        {
            using (var db = GetDB())
            {
                string message;
                if (_repositories.GetShelfInfoByCarrierID(db, carrierId, out var shelfInfo))
                {
                    if (!shelfInfo.Enable)
                    {
                        message = $"Enable:N, Can't Perform";
                        _LoggerService.WriteLogTrace(new HostCommandTrace("Remove", carrierId, shelfInfo.CarrierLoc, message));
                        return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                    }

                    if (shelfInfo.ShelfState == (char)ShelfState.StorageInReserved || shelfInfo.ShelfState == (char)ShelfState.StorageOutReserved)
                    {
                        message = $"ShelfState:{shelfInfo.ShelfState }, This Carrier is Executing";
                        _LoggerService.WriteLogTrace(new HostCommandTrace("Remove", carrierId, shelfInfo.CarrierLoc, message));
                        return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                    }

                    _repositories.DeleteCST("", "", shelfInfo.ShelfID, 1, carrierId, "Remove");

                    return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
                }

                message = $"The Carrier does not Exist";
                _LoggerService.WriteLogTrace(new HostCommandTrace("Remove", carrierId, string.Empty, message));
                return DaifukuSpec.ACK.HCACK.NoSuchObjectExists;
            }
        }

        public DaifukuSpec.ACK.HCACK Install(string carrierId, string carrierLoc)
        {
            using (var db = GetDB())
            {
                string message;

                _repositories.GetShelfInfoByCarrierID(db, carrierId, out var cstCarrierLoc);

                if (!_repositories.GetShelfInfoByShelfID(db, carrierLoc, out var mcsCarrierLoc))
                {
                    if (!_repositories.GetShelfInfoByCarrierLoc(db, carrierLoc, out mcsCarrierLoc))
                    {
                        message = $"Get ShelfInfo Fail";
                        _LoggerService.WriteLogTrace(new HostCommandTrace("Install", carrierId, carrierLoc, message));
                        return DaifukuSpec.ACK.HCACK.NoSuchObjectExists;
                    }
                }

                if (cstCarrierLoc != null)
                {
                    //Case2 Install 在已經在的位置上
                    if (cstCarrierLoc.CarrierLoc == carrierLoc)
                    {
                        message = "This Carrier Already Exist";
                        _LoggerService.WriteLogTrace(new HostCommandTrace("Install", carrierId, cstCarrierLoc.ShelfID, message));
                        return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                    }

                    if (!string.IsNullOrWhiteSpace(mcsCarrierLoc.CSTID))
                    {
                        _repositories.DeleteCST("", "", mcsCarrierLoc.ShelfID, 1, mcsCarrierLoc.CSTID, "Install");

                        message = $"Location Remove Carrier Success";
                        _LoggerService.WriteLogTrace(new HostCommandTrace("Install", mcsCarrierLoc.CSTID, mcsCarrierLoc.CarrierLoc, message));
                    }

                    //Case3 Install 的 CSTID 已經在其他位置上
                    if ((cstCarrierLoc.ShelfID != mcsCarrierLoc.ShelfID) && string.IsNullOrWhiteSpace(mcsCarrierLoc.CSTID))
                    {
                        _repositories.DeleteCST("", "", cstCarrierLoc.ShelfID, 1, carrierId, "Install");

                        _repositories.InsertCST("", "", mcsCarrierLoc.ShelfID, 1, carrierId, CarrierState.Installed, "Install");

                        return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
                    }
                }

                //Case1 Install 正常
                if (!_repositories.InsertCST("", "", mcsCarrierLoc.ShelfID, 1, carrierId, CarrierState.Installed, "Install"))
                {
                    return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                }

                return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
            }
        }

        public DaifukuSpec.ACK.HCACK PortTypeChg(string hostEqPortId, StockerEnums.IOPortDirection portDirection, ref bool ioPortDirReq)
        {
            var message = string.Empty;
            var ports = _repositories.GetAllShelfInfoByZoneID(GetDB(), hostEqPortId).Where(i => i.Stage == 1);
            var portTypeChangeSuccess = false;
            foreach (var io in ports)
            {
                if (io.PortType != (int)PortType.IO || io.PortLocationType == (int)PortLocationType.MGVPort)
                {
                    if (io.PortLocationType == (int)PortLocationType.MGVPort)
                    {
                        message = $"The Port is MGV Port";
                        _LoggerService.WriteLogTrace(new HostCommandTrace("PortTypeCHG", io.ZoneID, portDirection,
                            message));
                        return DaifukuSpec.ACK.HCACK.Rejected;
                    }

                    if (io.PortType == (int)PortType.EQ)
                    {
                        message = $"The Port is EQ Port";
                        _LoggerService.WriteLogTrace(new HostCommandTrace("PortTypeCHG", io.ZoneID, portDirection,
                            message));
                        return DaifukuSpec.ACK.HCACK.Rejected;
                    }
                }
                else
                {
                    switch (portDirection)
                    {
                        case StockerEnums.IOPortDirection.InMode
                            when _Stocker.GetIOPortById(io.PortTypeIndex).IsPortModeChangeable:
                            _Stocker.GetIOPortById(io.PortTypeIndex).RequestInModeAsync();
                            _LoggerService.WriteLogTrace(new HostCommandTrace("PortTypeCHG", io.ZoneID, portDirection,
                                message));
                            portTypeChangeSuccess = true;
                            break;
                        case StockerEnums.IOPortDirection.InMode:
                            ioPortDirReq = true;
                            _LoggerService.WriteLogTrace(new HostCommandTrace("PortTypeCHG", io.ZoneID, portDirection,
                                message));
                            portTypeChangeSuccess = true;
                            break;
                        case StockerEnums.IOPortDirection.OutMode
                            when _Stocker.GetIOPortById(io.PortTypeIndex).IsPortModeChangeable:
                            _Stocker.GetIOPortById(io.PortTypeIndex).RequestOutModeAsync();
                            _LoggerService.WriteLogTrace(new HostCommandTrace("PortTypeCHG", io.ZoneID, portDirection,
                                message));
                            portTypeChangeSuccess = true;
                            break;
                        case StockerEnums.IOPortDirection.OutMode:
                            ioPortDirReq = true;
                            _LoggerService.WriteLogTrace(new HostCommandTrace("PortTypeCHG", io.ZoneID, portDirection,
                                message));
                            portTypeChangeSuccess = true;
                            break;
                        case StockerEnums.IOPortDirection.None:
                            break;
                        case StockerEnums.IOPortDirection.ModeChanging:
                            break;
                        default:
                            return DaifukuSpec.ACK.HCACK.CannotPerformNow;
                    }
                }
            }

            return portTypeChangeSuccess ? DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed : DaifukuSpec.ACK.HCACK.NoSuchObjectExists;
        }

        public DaifukuSpec.ACK.HCACK RetryCommand(string ERRORID)
        {
            using (DB _db = GetDB())
            {
                string message = string.Empty;
                string defaultMessage = string.Empty;
                string carrierID = string.Empty;
                string strCraneID = string.Empty;
                int iAlarmID = 0;
                bool dataflag = false;


                IEnumerable<UpdateCommandInfo> infos = _repositories.GetUpdateCommandInfo(_db)
                                                                    .Where(x => x.Task_CompleteCode == LCS.Models.Define.CompleteCode.DoubleStorage ||
                                                                               x.Task_CompleteCode == LCS.Models.Define.CompleteCode.EmptyRetrieval);
                string CommandID = string.Empty;
                foreach (UpdateCommandInfo info in infos)
                {
                    if (CommandID == info.CommandID)
                        continue;

                    CommandID = info.CommandID;
                    strCraneID = _TaskInfo.GetCraneInfo(info.Task_CraneNo, info.Task_ForkNumber).CraneID;
                    string stockerUnitID = strCraneID + (info.Task_ForkNumber == 1 ? "L" : "R");
                    switch (info.Task_CompleteCode)
                    {
                        case LCS.Models.Define.CompleteCode.DoubleStorage:
                            defaultMessage = "DoubleStorage";
                            iAlarmID = int.Parse(LCS.Models.Define.LCSAlarm.DoubleStorage_F011, System.Globalization.NumberStyles.HexNumber) + 900000;
                            dataflag = true;
                            break;

                        case LCS.Models.Define.CompleteCode.EmptyRetrieval:
                            defaultMessage = "EmptyRetrieval";
                            iAlarmID = int.Parse(LCS.Models.Define.LCSAlarm.EmptyRetrieval_F010, System.Globalization.NumberStyles.HexNumber) + 900000;
                            dataflag = true;
                            break;
                        default:
                            break;
                    }

                    string strEM = "";
                    var carrierLoc = _repositories.GetShelfInfoByCarrierID(_db, info.CarrierID).CarrierLoc;
                    if (dataflag == true)
                    {
                        if (info.Task_CompleteCode == LCS.Models.Define.CompleteCode.DoubleStorage)
                        {
                            string taskNo = _repositories.GetMultiTaskNo(_db, info.Task_CraneNo, info.Task_ForkNumber, "TASK");
                            VShelfInfo dest = null;
                            if (!_repositories.GetShelfInfoByHostEQPortID(_db, info.HostDestination, out dest))
                            {
                                _repositories.GetShelfInfoByShelfID(_db, info.Task_Destination, out dest);
                            }

                            var crandShelfID = _repositories.GetShelfInfoByZoneID(_db, strCraneID, out VShelfInfo crane);
                            CommandTrace trace = new CommandTrace();
                            trace.CommandID = info.CommandID;
                            trace.NextCrane = info.Task_CraneNo;
                            trace.CarrierID = info.CarrierID;
                            trace.NextTransferMode = (int)TransferMode.TO;
                            trace.NextSource = crane.ShelfID;
                            trace.NextCraneSpeed = _repositories.GetCraneSpeed(_db, info.Task_CraneNo, new CraneSpeed());
                            trace.UserID = "MCS";
                            trace.BCRReadFlag = false;
                            trace.MainPriority = info.HostPriority;

                            #region UpdateTaskState
                            int iResult = _repositories.UpdateTaskStateAllTask(_db, info);
                            if (iResult != ErrorCode.Success)
                            {
                                message = defaultMessage + $", Update TaskState, Begin Fail, Result={iResult}";
                                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                            }
                            message = defaultMessage + $", Update TaskState Success";
                            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                            #endregion UpdateTaskState
                            iResult = _repositories.InsertTo(_db, trace, taskNo, info.Task_ForkNumber, dest);
                            _repositories.UpdateTransferStateToTransferring(_db, info.CommandID, ref strEM);

                            message = defaultMessage + $", Retry Command Sucess";
                            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

                            var task = Task.Delay(200).ContinueWith(t =>
                            {
                                _craneReport.SetCraneActive(strCraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                            });
                        }
                        else if (info.Task_CompleteCode == LCS.Models.Define.CompleteCode.EmptyRetrieval)
                        {
                            var Transfer = _repositories.GetTaskByCommandID(_db, info.CommandID).FirstOrDefault(i => i.TransferMode == (int)TransferMode.TO);

                            string taskNo = _repositories.GetMultiTaskNo(_db, info.Task_CraneNo, info.Task_ForkNumber, "TASK");
                            VShelfInfo dest = null;
                            if (!_repositories.GetShelfInfoByHostEQPortID(_db, info.HostDestination, out dest))
                            {
                                _repositories.GetShelfInfoByShelfID(_db, Transfer.Destination, out dest);
                            }
                            CommandTrace trace = new CommandTrace();
                            trace.CommandID = info.CommandID;
                            trace.NextCrane = info.Task_CraneNo;
                            trace.CarrierID = info.CarrierID;
                            trace.MainTransferMode = info.TransferMode;
                            trace.NextSource = info.Source;
                            trace.NextCraneSpeed = _repositories.GetCraneSpeed(_db, info.Task_CraneNo, new CraneSpeed());
                            trace.UserID = "MCS";
                            trace.BCRReadFlag = false;
                            trace.MainPriority = info.HostPriority;

                            #region UpdateTaskState
                            int iResult = _repositories.UpdateTaskStateAllTask(_db, info);
                            if (iResult != ErrorCode.Success)
                            {
                                message = defaultMessage + $", Update TaskState, Begin Fail, Result={iResult}";
                                _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                            }
                            message = defaultMessage + $", Update TaskState Success";
                            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                            #endregion UpdateTaskState
                            string[] strTaskNo = GetTaskNo(_db, trace);
                            iResult = _repositories.InsertTaskByTwinFork(_db, trace, strTaskNo, dest, info.Task_ForkNumber);
                            _repositories.UpdateTransferStateToTransferring(_db, info.CommandID, ref strEM);

                            message = defaultMessage + $", Retry Command Sucess";
                            _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));

                            var task = Task.Delay(200).ContinueWith(t =>
                            {
                                _craneReport.SetCraneActive(strCraneID, info.Task_CraneNo, info.Task_ForkNumber, info.CommandID);
                            });
                        }

                        return DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed;
                    }
                    else
                    {
                        message = defaultMessage + $"RetryCommand Error";
                        _LoggerService.WriteLogTrace(new UpdateProcessTrace(info.CommandID, info.TaskNo, info.CarrierID, message));
                        return DaifukuSpec.ACK.HCACK.Rejected;
                    }

                }
                message = defaultMessage + $", Retry Command Fail , No VshelfInfo ";
                _LoggerService.WriteLogTrace(new UpdateProcessTrace(string.Empty, string.Empty, string.Empty, message));

                return DaifukuSpec.ACK.HCACK.Rejected;
            }
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

        public void Resume()
        {
            try
            {
                _LCSParameter.AutoRequest();
                _LoggerService.WriteLogTrace(new HostCommandTrace("Resume", string.Empty));
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public void Pause()
        {
            try
            {
                _LCSParameter.PauseRequest(LCS.LCSShareMemory.LCSParameter.PauseReasonStatus.MCSRequest);
                _LoggerService.WriteLogTrace(new HostCommandTrace("Pause", string.Empty));
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
