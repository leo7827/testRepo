using System;
using System.Collections.Generic;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class TransferResumedEventArgs : EventArgs
    {
        public TransferResumedEventArgs(Stocker.TaskControl.Info.UpdateCommandInfo transfer)
        {
            TransferCommand transferCommand = new TransferCommand();

            CommandInfo commandInfo = new CommandInfo();
            commandInfo.CommandID = transfer.CommandID;
            commandInfo.Priority = transfer.HostPriority;
            transferCommand.CommandInfo = commandInfo;

            TransferInfo transferInfo = new TransferInfo();
            transferInfo.CarrierID = transfer.CarrierID;
            transferInfo.CarrierLoc = transfer.CurrentPosition;
            transferInfo.Dest = transfer.Destination;
            transferCommand.TransferInfo = transferInfo;

            this.TransferCommand = transferCommand;
        }
        public TransferCommand TransferCommand { get; }
    }
}