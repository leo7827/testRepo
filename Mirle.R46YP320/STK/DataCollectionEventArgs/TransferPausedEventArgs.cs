using System;
using System.Collections.Generic;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class TransferPausedEventArgs : EventArgs
    {
        public TransferPausedEventArgs(Stocker.TaskControl.Info.UpdateCommandInfo transfer, string carrierLoc, VIDEnums.ResultCode resultCode)
        {
            TransferCommand transferCommand = new TransferCommand();

            CommandInfo commandInfo = new CommandInfo();
            commandInfo.CommandID = transfer.CommandID;
            commandInfo.Priority = transfer.HostPriority;
            transferCommand.CommandInfo = commandInfo;

            TransferInfo transferInfo = new TransferInfo();
            transferInfo.CarrierID = transfer.CarrierID;
            transferInfo.CarrierLoc = carrierLoc;
            transferInfo.Dest = transfer.HostDestination;
            transferCommand.TransferInfo = transferInfo;

            this.TransferCommand = transferCommand;
            this.CarrierLoc = carrierLoc;
            this.ResultCode = resultCode;
        }
        public TransferCommand TransferCommand { get; }
        public string CarrierLoc { get; }
        public VIDEnums.ResultCode ResultCode { get; }
    }
}