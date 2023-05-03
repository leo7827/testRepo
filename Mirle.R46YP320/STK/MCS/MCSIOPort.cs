using Mirle.DataBase;
using Mirle.LCS.Models;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.R46YP320.STK.TaskService;
using Mirle.Stocker;
using Mirle.Stocker.Enums;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.TraceLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Mirle.LCS.Models.Define;
using Mirle.STKC.R46YP320.Service;
using ErrorCode = Mirle.DataBase.ErrorCode;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.MCS
{
    public class MCSIOPort
    {
        private readonly LoggerService _LoggerService;
        private readonly IStocker _stocker;
        private readonly TaskInfo _TaskInfo;
        private readonly RepositoriesService _Repositories;
        private readonly DataCollectionEventsService _dataCollectionEvents;
        private readonly IIOPort _ioPortSignalLeft;
        private readonly IIOPort _ioPortSignalRight;
        private readonly AlarmService _alarmService;

        private DateTime PortModeTimeOut;
        private bool _isNeedReportPortTypeChanging = false;

        //private StockerEnums.IOPortDirection needChange = StockerEnums.IOPortDirection.None;

        public string PortID { get; private set; }
        public VIDEnums.PortTransferState InService { get; private set; }
        public VIDEnums.PortUnitType Direction { get; private set; }
        public StockerEnums.IOPortDirection IOPortDirReq { get; private set; } = StockerEnums.IOPortDirection.None;

        public MCSIOPort(IStocker stocker, DataCollectionEventsService dataCollectionEvents, TaskInfo taskInfo, LoggerService loggerService, string portId, IIOPort ioPortSignalLeft, IIOPort ioPortSignalRight, AlarmService alarmService)
        {
            _dataCollectionEvents = dataCollectionEvents;
            _LoggerService = loggerService;
            _Repositories = new RepositoriesService(taskInfo, loggerService);
            _ioPortSignalLeft = ioPortSignalLeft;
            _ioPortSignalRight = ioPortSignalRight;
            _alarmService = alarmService;
            PortID = portId;
            _TaskInfo = taskInfo;
            _stocker = stocker;
        }

        public VIDEnums.PortTransferState GetMCSPortTransferState()
        {
            return InService;
        }

        public StockerEnums.IOPortDirection GetIOPortDirReq()
        {
            return IOPortDirReq;
        }

        public VIDEnums.PortUnitType GetIOPortMode()
        {
            return Direction;
        }

        public void InitialStatus()
        {
            InService = (_ioPortSignalLeft.IsInService || _ioPortSignalRight.IsInService) ? VIDEnums.PortTransferState.InService : VIDEnums.PortTransferState.OutOfService;

            Direction = _ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.InMode ? VIDEnums.PortUnitType.Input : VIDEnums.PortUnitType.Output;

            IOPortDirReq = StockerEnums.IOPortDirection.None;

            PortModeTimeOut = DateTime.Now;
        }

        public void RefreshStatus()
        {
            var port = _Repositories.GetAllShelfInfoByHostEQPortID(PortID).ToList();
            PortDirChangeReq();

            ReportInOutService(port);

            ReportF005AlarmWhenSinglePortOutOfService(port);

            ReportDirection(port);

            CheckPortModeChange(port);
        }

        private void ReportF005AlarmWhenSinglePortOutOfService(IEnumerable<VShelfInfo> port)
        {
            if (_ioPortSignalLeft.Id == _ioPortSignalRight.Id) return; // for MGV

            var leftVShelfInfo = port.FirstOrDefault(p => p.PortTypeIndex == _ioPortSignalLeft.Id);
            var rightVShelfInfo = port.FirstOrDefault(p => p.PortTypeIndex == _ioPortSignalRight.Id);
            if (leftVShelfInfo == null || rightVShelfInfo == null) return;

            // get current alarm
            var currentAlarms = _alarmService.GetAllCurrentAlarm().ToList();
            var leftAlarmed = currentAlarms.Any(a => a.EQId == leftVShelfInfo.HostEQPort && a.AlarmCode == LCSAlarm.PortInServiceOff_F005);
            var rightAlarmed = currentAlarms.Any(a => a.EQId == rightVShelfInfo.HostEQPort && a.AlarmCode == LCSAlarm.PortInServiceOff_F005);

            // check alarm
            var newLeftInServiceState = leftVShelfInfo.Enable && _ioPortSignalLeft.IsInService;
            if (leftAlarmed && newLeftInServiceState)
            {
                _alarmService.ClearAlarm(leftVShelfInfo.HostEQPort, LCSAlarm.PortInServiceOff_F005);
            }
            else if (leftAlarmed == false && newLeftInServiceState == false)
            {
                _alarmService.SetAlarm(leftVShelfInfo.HostEQPort, LCSAlarm.PortInServiceOff_F005, AlarmTypes.LCS);
            }

            var newRightInServiceState = rightVShelfInfo.Enable && _ioPortSignalRight.IsInService;
            if (rightAlarmed && newRightInServiceState)
            {
                _alarmService.ClearAlarm(rightVShelfInfo.HostEQPort, LCSAlarm.PortInServiceOff_F005);
            }
            else if (rightAlarmed == false && newRightInServiceState == false)
            {
                _alarmService.SetAlarm(rightVShelfInfo.HostEQPort, LCSAlarm.PortInServiceOff_F005, AlarmTypes.LCS);
            }
        }

        public void SetIOPortDirReq(StockerEnums.IOPortDirection portUnitType)
        {
            IOPortDirReq = portUnitType;
            _isNeedReportPortTypeChanging = true;
        }

        private void PortDirChangeReq()
        {
            if (_ioPortSignalRight == null)
            {
                if (IOPortDirReq != StockerEnums.IOPortDirection.None && _ioPortSignalLeft.Direction != IOPortDirReq && _ioPortSignalLeft.IsPortModeChangeable)
                {
                    if (IOPortDirReq == StockerEnums.IOPortDirection.InMode)
                    {
                        _ioPortSignalLeft.RequestInModeAsync();
                        IOPortDirReq = StockerEnums.IOPortDirection.None;
                    }
                    else if (IOPortDirReq == StockerEnums.IOPortDirection.OutMode)
                    {
                        _ioPortSignalLeft.RequestOutModeAsync();
                        IOPortDirReq = StockerEnums.IOPortDirection.None;
                    }
                }
            }
            else
            {
                //if (IOPortDirReq == StockerEnums.IOPortDirection.None && _ioPortSignalLeft.IsInService == false && _ioPortSignalRight.IsInService == false)
                //{
                //    IOPortDirReq = StockerEnums.IOPortDirection.OutMode;
                //}
                if (IOPortDirReq == StockerEnums.IOPortDirection.None && _ioPortSignalLeft.IsInService && _ioPortSignalRight.IsInService == false &&
                    _ioPortSignalRight.Direction != _ioPortSignalLeft.Direction) 
                {
                    if (_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.InMode || _ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.OutMode)
                    {
                        IOPortDirReq = _ioPortSignalLeft.Direction;
                        _isNeedReportPortTypeChanging = true;
                    }
                }
                else if (IOPortDirReq == StockerEnums.IOPortDirection.None && _ioPortSignalLeft.IsInService == false && _ioPortSignalRight.IsInService &&
                         _ioPortSignalRight.Direction != _ioPortSignalLeft.Direction)
                {
                    if (_ioPortSignalRight.Direction == StockerEnums.IOPortDirection.InMode || _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.OutMode)
                    {
                        IOPortDirReq = _ioPortSignalRight.Direction;
                        _isNeedReportPortTypeChanging = true;
                    }
                }
                //else if (IOPortDirReq == StockerEnums.IOPortDirection.None && _ioPortSignalLeft.IsInService && _ioPortSignalRight.IsInService)
                //{
                //    if (_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.InMode || _ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.OutMode)
                //    {
                //        IOPortDirReq = _ioPortSignalLeft.Direction;
                //    }
                //}
                else if (IOPortDirReq == StockerEnums.IOPortDirection.None && _ioPortSignalLeft.IsInService && _ioPortSignalRight.IsInService)
                {
                    if ((_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.InMode || _ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.OutMode) 
                        && (_ioPortSignalRight.Direction == StockerEnums.IOPortDirection.InMode || _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.OutMode) 
                        && (_ioPortSignalRight.Direction != _ioPortSignalRight.Direction))
                    {
                        IOPortDirReq = _ioPortSignalLeft.Direction;
                        _isNeedReportPortTypeChanging = true;
                    }
                }

                bool changeLeft = false;
                bool changeRight = false;

                if (_ioPortSignalRight.IsInService)
                {
                    if (IOPortDirReq != StockerEnums.IOPortDirection.None)
                    {
                        if (_ioPortSignalRight.Direction == IOPortDirReq)
                        {
                            changeRight = true;
                        }
                        else if (_ioPortSignalRight.IsPortModeChangeable && _ioPortSignalRight.IsInService)
                        {
                            if (IOPortDirReq == StockerEnums.IOPortDirection.InMode)
                            {
                                _ioPortSignalRight.RequestInModeAsync();
                            }

                            if (IOPortDirReq == StockerEnums.IOPortDirection.OutMode)
                            {
                                _ioPortSignalRight.RequestOutModeAsync();
                            }
                        }
                    }
                }

                if (_ioPortSignalLeft.IsInService)
                {
                    if (IOPortDirReq != StockerEnums.IOPortDirection.None)
                    {
                        if (_ioPortSignalLeft.Direction == IOPortDirReq)
                        {
                            changeLeft = true;
                        }
                        else if (_ioPortSignalLeft.IsPortModeChangeable && _ioPortSignalLeft.IsInService)
                        {
                            if (IOPortDirReq == StockerEnums.IOPortDirection.InMode)
                            {
                                _ioPortSignalLeft.RequestInModeAsync();
                            }

                            if (IOPortDirReq == StockerEnums.IOPortDirection.OutMode)
                            {
                                _ioPortSignalLeft.RequestOutModeAsync();
                            }
                        }
                    }
                }

                if (changeLeft && changeRight)
                {
                    IOPortDirReq = StockerEnums.IOPortDirection.None;
                }
            }
        }

        private void ReportDirection(IEnumerable<VShelfInfo> port)
        {
            if ((_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.InMode && _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.InMode && Direction != VIDEnums.PortUnitType.Input) ||
                (_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.OutMode && _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.OutMode && Direction != VIDEnums.PortUnitType.Output))
            {
                Direction = Direction == VIDEnums.PortUnitType.Output ? VIDEnums.PortUnitType.Input : VIDEnums.PortUnitType.Output;
                IOPortTypeChangeScenarios(port, PortID, Direction);
            }
            //else if (Direction != VIDEnums.PortUnitType.Input &&
            //    ((_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.InMode && _ioPortSignalRight.IsInService == false) ||
            //    (_ioPortSignalLeft.IsInService == false && _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.InMode)))
            //{
            //    Direction = Direction == VIDEnums.PortUnitType.Output ? VIDEnums.PortUnitType.Input : VIDEnums.PortUnitType.Output;
            //    IOPortTypeChangeScenarios(port, PortID, Direction);
            //}
            //else if (Direction != VIDEnums.PortUnitType.Output &&
            //    ((_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.OutMode && _ioPortSignalRight.IsInService == false) ||
            //    (_ioPortSignalLeft.IsInService == false && _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.OutMode)))
            //{
            //    Direction = Direction == VIDEnums.PortUnitType.Output ? VIDEnums.PortUnitType.Input : VIDEnums.PortUnitType.Output;
            //    IOPortTypeChangeScenarios(port, PortID, Direction);
            //}
            if (_isNeedReportPortTypeChanging)
            {
                _dataCollectionEvents.OnPortTypeChanging(this, new PortStateEventArgs(PortID));
                _isNeedReportPortTypeChanging = false;
            }
        }

        private void IOPortTypeChangeScenarios(IEnumerable<VShelfInfo> shelfInfos, string hostEQPortID, VIDEnums.PortUnitType Direction)
        {
            using (var db = _TaskInfo.GetDB())
            {
                if (Direction == VIDEnums.PortUnitType.Input)
                {
                    ReportIOPortCarrierRemoved(hostEQPortID, db, shelfInfos);

                    ReportIOPortModeChange(hostEQPortID, Direction);
                }
                else
                {
                    ReportIOPortModeChange(hostEQPortID, Direction);

                    ReportIOPortCarrierRemoved(hostEQPortID, db, shelfInfos);
                }
            }
        }

        private void ReportIOPortCarrierRemoved(string hostEQPortID, DB db, IEnumerable<VShelfInfo> shelfInfos)
        {
            foreach (var shelf in shelfInfos)
            {
                if (string.IsNullOrWhiteSpace(shelf.CSTID))
                    continue;

                string defaultMessage = string.Empty;
                bool isDeleteCarrierDataSuccess = _Repositories.DeleteCST("", "", shelf.ShelfID, shelf.Stage, shelf.CSTID, defaultMessage);
                if (!isDeleteCarrierDataSuccess)
                    continue;

                bool existsCommand = _Repositories.GetTransferCmdByCarrierID(db, shelf.CSTID, out Stocker.TaskControl.Info.TransferCommand transferCommand);
                if (existsCommand)
                {
                    CancelCommand(hostEQPortID, shelf.CarrierLoc, db, defaultMessage, transferCommand);
                    _Repositories.DeleteTransferAndTask(db, transferCommand.CommandID, "", shelf.CSTID, defaultMessage);
                }
                PortDefInfo portDefInfo = _TaskInfo.GetIOPortInfo(shelf.PortTypeIndex);
                if (portDefInfo.ReportStage > 1)
                    _dataCollectionEvents.OnCarrierWaitOut(this, new CarrierWaitOutEventArgs(shelf.CSTID, shelf.CarrierLoc, VIDEnums.PortType.LP));

                if (shelf.PortLocationType == (int)PortLocationType.MGVPort || shelf.PortLocationType == (int)PortLocationType.ViewPort)
                    _dataCollectionEvents.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelf.CSTID, VIDEnums.HandoffType.Manual, shelf.CarrierLoc));
                else
                    _dataCollectionEvents.OnCarrierRemoved(this, new CarrierRemovedEventArgs(shelf.CSTID, VIDEnums.HandoffType.Automated, shelf.CarrierLoc));
            }
        }

        private void ReportIOPortModeChange(string hostEQPortID, VIDEnums.PortUnitType Direction)
        {
            switch (Direction)
            {
                case VIDEnums.PortUnitType.Input:
                    //_dataCollectionEvents.OnPortTypeChanging(this, new PortStateEventArgs(hostEQPortID));
                    _dataCollectionEvents.OnPortTypeInput(this, new PortStateEventArgs(hostEQPortID));
                    break;
                case VIDEnums.PortUnitType.Output:
                    //_dataCollectionEvents.OnPortTypeChanging(this, new PortStateEventArgs(hostEQPortID));
                    _dataCollectionEvents.OnPortTypeOutput(this, new PortStateEventArgs(hostEQPortID));
                    break;
            }
        }

        private void CancelCommand(string hostEQPortID, string carrierLoc, DB db, string defaultMessage, Stocker.TaskControl.Info.TransferCommand transferCommand)
        {
            UpdateCommandInfo commandInfo = new UpdateCommandInfo();
            commandInfo.CommandID = transferCommand.CommandID;
            commandInfo.HostPriority = transferCommand.HostPriority;
            commandInfo.CarrierID = transferCommand.CSTID;
            commandInfo.HostSource = transferCommand.HostSource;
            commandInfo.HostDestination = transferCommand.HostDestination;
            commandInfo.Source = transferCommand.Source;
            commandInfo.Destination = transferCommand.Destination;
            commandInfo.TaskNo = "";

            var mainSource = GetVShelfInfo(db, commandInfo.Source);
            UpdateShelfDef(commandInfo, db, mainSource, ShelfState.EmptyShelf, defaultMessage);

            var taskCmds = _Repositories.GetTaskByCommandID(db, transferCommand.CommandID);
            foreach (var item in taskCmds)
            {
                var dest = GetVShelfInfo(db, item.Destination);
                UpdateShelfDef(commandInfo, db, dest, ShelfState.EmptyShelf, defaultMessage);
            }

            var mainDestination = GetVShelfInfo(db, commandInfo.Destination);
            UpdateShelfDef(commandInfo, db, mainDestination, ShelfState.EmptyShelf, defaultMessage);

            if (transferCommand.TransferState == (int)TransferState.Queue)
            {
                _dataCollectionEvents.OnOperatorInitiatedAction(this, new OperatorInitiatedActionEventArgs(commandInfo.CommandID, VIDEnums.CommandType.CANCEL, commandInfo.CarrierID, commandInfo.HostSource, commandInfo.HostDestination, commandInfo.HostPriority));
                _dataCollectionEvents.OnTransferCancelInitiated(this, new TransferCancelInitiatedEventArgs(commandInfo, hostEQPortID));
                _dataCollectionEvents.OnTransferCancelCompleted(this, new TransferCancelCompletedEventArgs(commandInfo, hostEQPortID));
            }
            else
            {
                _dataCollectionEvents.OnTransferCompleted(this, new TransferCompletedEventArgs(commandInfo, carrierLoc, VIDEnums.ResultCode.InterlockError));
            }
        }

        private VShelfInfo GetVShelfInfo(DB db, string shelfId)
        {
            if (!_Repositories.GetShelfInfoByShelfID(db, shelfId, out var shelf))
            {
                if (!_Repositories.GetShelfInfoByPLCPortID(db, Convert.ToInt32(shelfId), out shelf))
                {
                    return null;
                }
            }
            return shelf;
        }
        private bool UpdateShelfDef(UpdateCommandInfo info, DB db, VShelfInfo shelfInfo, ShelfState shelfState, string defaultMessage)
        {
            string message;
            string strEM = "";
            var iResult = _Repositories.UpdateShelfDef(db, shelfInfo.ShelfID, shelfState, ref strEM);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage +
                          $", ShelfID:{shelfInfo.ShelfID}, ShelfState:{(char)shelfState}, Update ShelfDef Fail, Message:{strEM}";
                _LoggerService.WriteLogTrace(info.CommandID, info.TaskNo, info.CarrierID, message);
                return false;
            }

            message = defaultMessage +
                      $", ShelfID:{shelfInfo.ShelfID}, Source:{(char)shelfState}, Update ShelfDef Success";
            _LoggerService.WriteLogTrace(info.CommandID, info.TaskNo, info.CarrierID, message);

            return true;
        }

        private void ReportInOutService(IEnumerable<VShelfInfo> port)
        {
            port = port.Where(i => i.Stage == 1);
            bool needRepotPortInOutService = NeedRepotPortInOutService(port);
            if (needRepotPortInOutService)
            {
                var TempLeftInService = (_ioPortSignalLeft.IsInService && port.FirstOrDefault(p => p.PortTypeIndex == _ioPortSignalLeft.Id).Enable) ? VIDEnums.PortTransferState.InService : VIDEnums.PortTransferState.OutOfService;
                var TempRightInService = (_ioPortSignalRight.IsInService && port.FirstOrDefault(p => p.PortTypeIndex == _ioPortSignalRight.Id).Enable) ? VIDEnums.PortTransferState.InService : VIDEnums.PortTransferState.OutOfService;
                var TempInService = (TempLeftInService == VIDEnums.PortTransferState.InService || TempRightInService == VIDEnums.PortTransferState.InService) ? VIDEnums.PortTransferState.InService : VIDEnums.PortTransferState.OutOfService;
                var craneAvail = port.FirstOrDefault().LocateCraneNo != 2 ?
                    (_stocker.GetCraneById(1) as Stocker.R46YP320.Crane).AvailStatus != LCS.Enums.LCSEnums.AvailStatus.Avail :
                    (_stocker.GetCraneById(2) as Stocker.R46YP320.Crane).AvailStatus != LCS.Enums.LCSEnums.AvailStatus.Avail;
                TempInService = craneAvail ? VIDEnums.PortTransferState.OutOfService : TempInService;
                if (InService != TempInService)
                {
                    InService = TempInService;
                    ReportIOPortOutOfService(PortID, InService);
                }
            }
        }

        private bool NeedRepotPortInOutService(IEnumerable<VShelfInfo> port)
        {
            var needRepotPortInOutService = false;
            var craneAvail = port.FirstOrDefault().LocateCraneNo != 2 ?
                (_stocker.GetCraneById(1) as Stocker.R46YP320.Crane).AvailStatus != LCS.Enums.LCSEnums.AvailStatus.Avail :
                (_stocker.GetCraneById(2) as Stocker.R46YP320.Crane).AvailStatus != LCS.Enums.LCSEnums.AvailStatus.Avail;
            if (craneAvail)
            {
                if (InService == VIDEnums.PortTransferState.InService)
                {
                    needRepotPortInOutService = true;
                }
            }
            else
            {
                if (InService == VIDEnums.PortTransferState.OutOfService)
                {
                    if ((_ioPortSignalLeft.IsInService || _ioPortSignalRight.IsInService) && port.All(i => i.Enable == true))
                    {
                        needRepotPortInOutService = true;
                    }
                    else if ((_ioPortSignalLeft.IsInService && _ioPortSignalRight.IsInService) && port.Any(i => i.Enable == true))
                    {
                        needRepotPortInOutService = true;
                    }
                }
                else
                {
                    if ((!_ioPortSignalLeft.IsInService && !_ioPortSignalRight.IsInService) || port.All(i => i.Enable == false))
                    {
                        needRepotPortInOutService = true;
                    }
                    else if (!_ioPortSignalLeft.IsInService && !port.FirstOrDefault(p => p.PortTypeIndex == _ioPortSignalRight.Id).Enable)
                    {
                        needRepotPortInOutService = true;
                    }
                    else if (!_ioPortSignalRight.IsInService && !port.FirstOrDefault(p => p.PortTypeIndex == _ioPortSignalLeft.Id).Enable)
                    {
                        needRepotPortInOutService = true;
                    }
                }
            }

            return needRepotPortInOutService;
        }

        private void ReportIOPortOutOfService(string hostEQPortID, VIDEnums.PortTransferState inService)
        {
            if (inService == VIDEnums.PortTransferState.InService)
                _dataCollectionEvents.OnPortInService(this, new PortStateEventArgs(hostEQPortID));
            else
                _dataCollectionEvents.OnPortOutOfService(this, new PortStateEventArgs(hostEQPortID));
        }

        private void CheckPortModeChange(IEnumerable<VShelfInfo> port)
        {
            //Timeout Set = 0 = 不啟用
            if (_TaskInfo.Config.SystemConfig.PortModeTimeOut == 0)
                return;

            //確認現在的方向
            switch (Direction)
            {
                case VIDEnums.PortUnitType.Output:
                    //如果左邊是 In 右邊是 Out 就轉右邊為Out
                    if (_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.InMode && _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.OutMode)
                        PortModeChange(port, _ioPortSignalLeft, StockerEnums.IOPortDirection.OutMode);

                    //如果左邊是 Out 右邊是 In 就轉左邊為Out
                    else if (_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.OutMode && _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.InMode)
                        PortModeChange(port, _ioPortSignalRight, StockerEnums.IOPortDirection.OutMode);

                    //其餘跳過
                    else
                        PortModeTimeOut = DateTime.Now;
                    break;

                case VIDEnums.PortUnitType.Input:
                    //如果左邊是 In 右邊是 Out 就轉右邊為In
                    if (_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.InMode && _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.OutMode)
                        PortModeChange(port, _ioPortSignalRight, StockerEnums.IOPortDirection.InMode);

                    //如果左邊是 Out 右邊是 In 就轉左邊為In
                    else if (_ioPortSignalLeft.Direction == StockerEnums.IOPortDirection.OutMode && _ioPortSignalRight.Direction == StockerEnums.IOPortDirection.InMode)
                        PortModeChange(port, _ioPortSignalLeft, StockerEnums.IOPortDirection.InMode);

                    //其餘跳過
                    else
                        PortModeTimeOut = DateTime.Now;
                    break;
            }
        }

        private void PortModeChange(IEnumerable<VShelfInfo> port, IIOPort ioPort, StockerEnums.IOPortDirection mode)
        {
            if (!CheckPortStatus(port.FirstOrDefault(i => i.PortTypeIndex == ioPort.Id), ioPort))
                return;

            switch (mode)
            {
                case StockerEnums.IOPortDirection.InMode:
                    ioPort.RequestInModeAsync();
                    break;
                case StockerEnums.IOPortDirection.OutMode:
                    ioPort.RequestOutModeAsync();
                    break;
            }

            PortModeTimeOut = DateTime.Now;
        }

        private bool CheckPortStatus(VShelfInfo shelf, IIOPort port)
        {
            if (port.Status == StockerEnums.IOPortStatus.ERROR || !port.IsInService || !shelf.Enable || !port.IsPortModeChangeable)
            {
                PortModeTimeOut = DateTime.Now;
                return false;
            }

            return DateTime.Now > PortModeTimeOut.AddSeconds(_TaskInfo.Config.SystemConfig.PortModeTimeOut);

        }
    }
}
