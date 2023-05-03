using Mirle.Structure.Info;
using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class PriorityUpdateFailedEventArgs : EventArgs
    {
        public PriorityUpdateFailedEventArgs(Stocker.TaskControl.Info.TransferCommand transfer, string carrierLoc)
        {
            TransferCommand transferCommand = new TransferCommand();

            CommandInfo commandInfo = new CommandInfo();
            commandInfo.CommandID = transfer.CommandID;
            commandInfo.Priority = transfer.HostPriority;
            transferCommand.CommandInfo = commandInfo;

            TransferInfo transferInfo = new TransferInfo();
            transferInfo.CarrierID = transfer.CSTID;
            transferInfo.CarrierLoc = carrierLoc;
            transferInfo.Dest = transfer.HostDestination;
            transferCommand.TransferInfo = transferInfo;

            this.TransferCommand = transferCommand;
            this.CarrierLoc = carrierLoc;
        }
        public TransferCommand TransferCommand { get; }
        public string CarrierLoc { get; }
    }
}