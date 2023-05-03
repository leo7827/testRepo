using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Mirle.DataBase;
using Mirle.LCS.Models;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.Structure.Info;
using Mirle.Stocker;
using Mirle.Stocker.Enums;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.Module;
using Mirle.Stocker.TaskControl.TraceLog;
using Mirle.Stocker.TaskControl.TraceLog.Format;

namespace Mirle.R46YP320.STK.TaskService
{
    public class EnhancedRemoteCommandService : EnhancedRemoteCommandModule
    {
        private readonly DataCollectionEventsService _dataCollectionEventsService;
        private readonly RepositoriesService _repositories;
        private readonly IStocker _stocker;

        public EnhancedRemoteCommandService(TaskInfo taskInfo, IStocker stocker, DataCollectionEventsModule dataCollectionEventsService, LoggerService loggerService) : base(taskInfo, stocker, loggerService)
        {
            _stocker = stocker;
            _dataCollectionEventsService = (DataCollectionEventsService)dataCollectionEventsService;
            _repositories = new RepositoriesService(taskInfo, loggerService);
        }

        public DaifukuSpec.ACK.HCACK Transfer_Batch(TransferBatch transferBatch)
        {
            const string rcmd = "Transfer_Batch";
            try
            {
                using (var db = GetDB())
                {
                    if (CheckCommandAndCarrier(transferBatch, db, rcmd, out var hcack))
                    {
                        return hcack;
                    }

                    var source = CheckSource(transferBatch, db, rcmd);

                    if (CheckCarrier(transferBatch, db, rcmd, ref source, out hcack))
                    {
                        return hcack;
                    }

                    if (CheckDestination(transferBatch, db, source, rcmd, out var dest, out hcack))
                    {
                        return hcack;
                    }

                    var priority = _repositories.ComputeTaskPriority(transferBatch.Priority, source, dest);

                    var isToCommand = _TaskInfo.CraneInfos.Count(row => row.CraneShelfID == source.ShelfID) > 0;
                    if (isToCommand)
                    {
                        return InsertToCommand(transferBatch, rcmd, db, dest, priority, source, out hcack) ? DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed : hcack;
                    }

                    var needMoveBack = transferBatch.Source == transferBatch.Dest && _TaskInfo.PortInfos.Count(row => row.HostEQPortID == transferBatch.Source && row.PortLocationType == PortLocationType.MGVPort) > 0;
                    if (needMoveBack)
                    {
                        return MoveBack(transferBatch, rcmd, source);
                    }

                    return InsertTransferCommand(transferBatch, db, rcmd, dest, source, priority, out hcack) ? DaifukuSpec.ACK.HCACK.AcknowledgeLaterPerformed : hcack;
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return DaifukuSpec.ACK.HCACK.Rejected;
            }
        }

        private bool CheckCommandAndCarrier(TransferBatch transferBatch, DB db, string rcmd, out DaifukuSpec.ACK.HCACK hcack)
        {
            if (_repositories.ExistsTransferCmd(db, transferBatch.CommandID, transferBatch.CarrierID, out var commandId, out var carrierId))
            {
                var message = "Command Already Exists, ";

                if (!string.IsNullOrWhiteSpace(commandId))
                    message += $"Repeat CommandID:{commandId}";

                if (!string.IsNullOrWhiteSpace(carrierId))
                    message += $"Repeat CarrierID:{carrierId}";

                _LoggerService.WriteLogTrace(new EnhancedRemoteCommandTrace(rcmd, transferBatch.CommandID, transferBatch.CarrierID, transferBatch.Source, transferBatch.Dest, message));

                hcack = DaifukuSpec.ACK.HCACK.CannotPerformNow;
                return true;
            }

            hcack = DaifukuSpec.ACK.HCACK.Acknowledge;
            return false;
        }

        private VShelfInfo CheckSource(TransferBatch transferBatch, DB db, string rcmd)
        {
            string message;

            if (!_repositories.GetShelfInfoByHostEQPortID(db, transferBatch.Source, transferBatch.CarrierID,
                out var source))
            {
                if (!_repositories.GetShelfInfoByZoneID(db, transferBatch.Source, transferBatch.CarrierID, out source))
                {
                    if (!_repositories.GetShelfInfoByCarrierLoc(db, transferBatch.Source, transferBatch.CarrierID,
                        out source))
                    {
                        message = $"Check Source Fail";
                        _LoggerService.WriteLogTrace(new EnhancedRemoteCommandTrace(rcmd, transferBatch.CommandID,
                            transferBatch.CarrierID, transferBatch.Source, transferBatch.Dest, message));
                    }
                }
            }

            message = $"Check Source Success";
            _LoggerService.WriteLogTrace(new EnhancedRemoteCommandTrace(rcmd, transferBatch.CommandID, transferBatch.CarrierID,
                transferBatch.Source, transferBatch.Dest, message));

            return source;
        }

        private bool CheckCarrier(TransferBatch transferBatch, DB db, string rcmd, ref VShelfInfo source,
            out DaifukuSpec.ACK.HCACK hcack)
        {
            if (!_repositories.ExistsCarrier(db, transferBatch.CarrierID, out var shelfId, out var stage))
            {
                if (source == null)
                {
                    var ports = _repositories.GetAllShelfInfoByHostEQPortID(db, transferBatch.Source).Where(i => i.Stage == 1).ToList();
                    if (PortHasTwoCST(transferBatch, ports, out source))
                    {
                        hcack = DaifukuSpec.ACK.HCACK.Acknowledge;
                        return false;
                    }

                    foreach (var port in ports)
                    {
                        if (port.PortLocationType == (int)PortLocationType.AutoPort)
                        {
                            if (!_stocker.GetIOPortById(port.PortTypeIndex).GetStageById(1).HasCarrier)
                                continue;
                        }

                        if (port.CSTID == transferBatch.CarrierID || (!port.CSTID.StartsWith("UNK") && !string.IsNullOrWhiteSpace(port.CSTID)))
                        {
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(port.CSTID))
                        {
                            _repositories.DeleteCST("", "", port.ShelfID, 1, port.CSTID,
                                $"{port.HostEQPort} delete CST {transferBatch.CarrierID}");
                        }

                        _repositories.InsertCST("", "", port.ShelfID, 1, transferBatch.CarrierID, CarrierState.WaitIn,
                            $"{port.HostEQPort} insert CST {transferBatch.CarrierID}");

                        source = port;
                        break;
                    }
                }
                else
                {
                    const string message = "Carrier Not Exist";
                    _LoggerService.WriteLogTrace(new EnhancedRemoteCommandTrace(rcmd, transferBatch.CommandID,
                        transferBatch.CarrierID, transferBatch.Source, transferBatch.Dest, message));

                    hcack = DaifukuSpec.ACK.HCACK.NoSuchObjectExists;
                    return true;
                }
            }

            hcack = DaifukuSpec.ACK.HCACK.Acknowledge;
            return false;
        }

        private bool PortHasTwoCST(TransferBatch transferBatch, List<VShelfInfo> ports, out VShelfInfo source)
        {
            if (ports.All(p => _stocker.GetIOPortById(p.PortTypeIndex).GetStageById(1).HasCarrier))
            {
                List<CassetteData> cst = new List<CassetteData>();
                foreach (var port in ports)
                {
                    if (_repositories.GetCassetteData(port.ShelfID, out var cassetteDatas))
                    {
                        cst.AddRange(cassetteDatas);
                    }
                }

                var allCST = cst.OrderBy(i => Convert.ToDateTime(i.CSTInDT));
                foreach (var cassette in allCST)
                {
                    if (_repositories.GetTransferCmdByCarrierID(cassette.CSTID) != null)
                        continue;

                    _repositories.DeleteCST("", "", cassette.ShelfID, 1, cassette.CSTID,
                        $"{cassette.ShelfID} delete CST {transferBatch.CarrierID}");

                    _repositories.InsertCST("", "", cassette.ShelfID, 1, transferBatch.CarrierID, CarrierState.WaitIn,
                        $"{cassette.ShelfID} insert CST {transferBatch.CarrierID}");
                    source = ports.FirstOrDefault(i => i.ShelfID == cassette.ShelfID);
                    return true;
                }
            }

            source = null;
            return false;
        }

        private bool CheckDestination(TransferBatch transferBatch, DB db, VShelfInfo source, string rcmd, out VShelfInfo dest,
            out DaifukuSpec.ACK.HCACK hcack)
        {
            dest = null;
            var allPort = _repositories.GetAllShelfInfoByHostEQPortID(db, transferBatch.Dest).Where(i => i.Stage == 1).ToList();
            if (allPort.Any())
            {
                dest = WhichPortCanTransfer(db, allPort);
            }
            else
            {
                if (source == null)
                    {
                    hcack = DaifukuSpec.ACK.HCACK.CannotPerformNow;
                    return true;
                }
                else
                {
                    var infos = _repositories.GetShortestPath(db, source, transferBatch.Dest).ToList();
                    if (infos.Any())
                    {
                        dest = infos.First();
                    }
                    else
                    {
                        return ZoneFull(transferBatch, source, out hcack);
                    }
                }
            }

            var message = $"Check Destination Success";
            _LoggerService.WriteLogTrace(new EnhancedRemoteCommandTrace(rcmd, transferBatch.CommandID, transferBatch.CarrierID,
                transferBatch.Source, transferBatch.Dest, message));

            hcack = DaifukuSpec.ACK.HCACK.Acknowledge;
            return false;
        }

        private bool ZoneFull(TransferBatch transferBatch, VShelfInfo source, out DaifukuSpec.ACK.HCACK hcack)
        {
            var transferCommand = new UpdateCommandInfo
            {
                CommandID = transferBatch.CommandID,
                HostPriority = transferBatch.Priority,
                CarrierID = transferBatch.CarrierID,
                HostDestination = transferBatch.Dest
            };

            System.Threading.Tasks.Task.Delay(200).ContinueWith(t =>
            {
                _dataCollectionEventsService.OnTransferInitiated(this,
                    new TransferInitiatedEventArgs(transferCommand, source.CarrierLoc));
                _dataCollectionEventsService.OnTransferCompleted(this,
                    new TransferCompletedEventArgs(transferCommand, source.CarrierLoc, VIDEnums.ResultCode.AllBinLocationsOccupied));
            });

            hcack = DaifukuSpec.ACK.HCACK.Acknowledge;
            return true;
        }

        private DaifukuSpec.ACK.HCACK MoveBack(TransferBatch transferBatch, string rcmd, VShelfInfo source)
        {
            var message = $"Check Source=Dest";
            _LoggerService.WriteLogTrace(new EnhancedRemoteCommandTrace(rcmd, transferBatch.CommandID, transferBatch.CarrierID, transferBatch.Source, transferBatch.Dest, message));

            if (source.PortLocationType != (int)PortLocationType.MGVPort)
            {
                message = $"Source isn't MGV";
                _LoggerService.WriteLogTrace(new EnhancedRemoteCommandTrace(rcmd, transferBatch.CommandID,
                    transferBatch.CarrierID, transferBatch.Source, transferBatch.Dest, message));
                return DaifukuSpec.ACK.HCACK.AtLeastOneParameterIsInvalid;
            }

            var carrier = new Carrier(transferBatch.CarrierID);
            if (carrier.IsUnknown())
            {
                var idReadStatus = carrier.IsUnknownDuplicate()
                    ? VIDEnums.IDReadStatus.Duplicate
                    : VIDEnums.IDReadStatus.Failure;

                _dataCollectionEventsService.OnIDReadError(this,
                    new IDReadErrorEventArgs(transferBatch.CarrierID, transferBatch.Source, idReadStatus));
            }

            _Stocker.GetIOPortById(source.PortTypeIndex).RequestMoveBackMGVAsync();
            return DaifukuSpec.ACK.HCACK.Acknowledge;
        }

        private VShelfInfo WhichPortCanTransfer(DB db, IEnumerable<VShelfInfo> allPort)
        {
            var portList = allPort.ToList();

            if (portList.Count <= 1)
                return portList.FirstOrDefault();

            var allCmd = _repositories.GetAllTransferCmd(db, TransferState.UpdateOK_Complete).ToList();

            //取得左邊Port的狀態 
            var leftPort = portList.FirstOrDefault(i => i.HostEQPort.EndsWith("L"));
            //var leftPortStatusOk = CheckPortStatus(leftPort);
            var leftPortStatusOk = CheckPortStatus(leftPort,out var leftCount);

            //取得右邊Port的狀態 
            var rightPort = portList.FirstOrDefault(i => i.HostEQPort.EndsWith("R"));
            //var rightPortStatusOk = CheckPortStatus(rightPort);
            var rightPortStatusOk = CheckPortStatus(rightPort, out var rightCount);

            if (leftPortStatusOk && !rightPortStatusOk)
            {
                //右邊狀態不對 就都取左邊
                return leftPort;
            }

            if (!leftPortStatusOk && rightPortStatusOk)
            {
                //左邊狀態不對 就都取右邊
                return rightPort;
            }

            //其餘的依照左右邊的命令數 分別下命令
            //var leftPortCmdCount = allCmd.Count(i => i.Destination == leftPort.PLCPortID.ToString()) + PortReportStagePresentOnAdd1(leftPort);
            //var rightPortCmdCount = allCmd.Count(i => i.Destination == rightPort.PLCPortID.ToString()) + PortReportStagePresentOnAdd1(rightPort);
            var leftPortCmdCount = allCmd.Count(i => i.Destination == leftPort.PLCPortID.ToString()) + leftCount;
            var rightPortCmdCount = allCmd.Count(i => i.Destination == rightPort.PLCPortID.ToString()) + rightCount;

            return leftPortCmdCount < rightPortCmdCount ? leftPort : rightPort;
        }

        private bool CheckPortStatus(VShelfInfo port)
        {
            var ioPort = _Stocker.GetIOPortById(port.PortTypeIndex);

            if (ioPort.Direction != StockerEnums.IOPortDirection.OutMode)
                return false;
            if (ioPort.Status == StockerEnums.IOPortStatus.ERROR)
                return false;
            if (!ioPort.IsInService)
                return false;
            if (ioPort.LoadRequestStatus != StockerEnums.PortLoadRequestStatus.Load)
                return false;
            if (!port.Enable)
                return false;

            return true;
        }
        private bool CheckPortStatus(VShelfInfo port, out int presenceCount)
        {
            var ioPort = _Stocker.GetIOPortById(port.PortTypeIndex);
            presenceCount = 0;
            foreach (var stage in ioPort.Stages)
            {
                presenceCount = stage.HasCarrier ? (presenceCount + 1) : presenceCount;
            }

            if (ioPort.Direction != StockerEnums.IOPortDirection.OutMode)
                return false;
            if (ioPort.Status == StockerEnums.IOPortStatus.ERROR)
                return false;
            if (!ioPort.IsInService)
                return false;
            if (ioPort.LoadRequestStatus != StockerEnums.PortLoadRequestStatus.Load)
                return false;
            if (!port.Enable)
                return false;

            return true;
        }

        private int PortReportStagePresentOnAdd1(VShelfInfo port)
        {
            return _Stocker.GetIOPortById(port.PortTypeIndex).GetStageById(port.StageCount).HasCarrier ? 1 : 0;
        }
    }
}
