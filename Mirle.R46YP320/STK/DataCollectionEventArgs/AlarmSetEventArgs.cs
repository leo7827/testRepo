using System;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class AlarmSetEventArgs : EventArgs
    {
        public AlarmSetEventArgs(Stocker.TaskControl.Info.UpdateCommandInfo transfer, string carrierLoc, string errorID, string StockerUnitID, VIDEnums.StockerUnitState StockerUnitState, int AlarmID, VIDEnums.RecoeryOption recoeryOption)
        {
            TransferCommand transferCommand = new TransferCommand();

            CommandInfo commandInfo = new CommandInfo();
            commandInfo.CommandID = transfer == null ? "" : transfer.CommandID;
            commandInfo.Priority = transfer == null ? 0 : transfer.HostPriority;
            transferCommand.CommandInfo = commandInfo;

            TransferInfo transferInfo = new TransferInfo();
            transferInfo.CarrierID = transfer == null ? "" : transfer.CarrierID;
            transferInfo.CarrierLoc = transfer == null ? "" : carrierLoc;
            transferInfo.Dest = transfer == null ? "" : transfer.HostDestination;
            transferCommand.TransferInfo = transferInfo;

            this.TransferCommand = transferCommand;
            this.ErrorID = errorID;

            StockerUnitInfo stockerUnitInfo = new StockerUnitInfo();
            stockerUnitInfo.StockerUnitID = StockerUnitID;
            stockerUnitInfo.StockerUnitState = StockerUnitState;
            this.StockerUnitInfo = stockerUnitInfo;

            this.AlarmID = AlarmID;
            this.RecoeryOption = recoeryOption;
        }

        public TransferCommand TransferCommand { get; }
        public string ErrorID { get; }
        public StockerUnitInfo StockerUnitInfo { get; }
        public VIDEnums.StockerUnitState StockerUnitState { get; }
        public int AlarmID { get; }
        public VIDEnums.RecoeryOption RecoeryOption { get; }
    }
}