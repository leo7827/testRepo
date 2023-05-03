using Mirle.Structure.Info;
using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class ScanCompletedEventArgs : EventArgs
    {
        public ScanCompletedEventArgs(string commandID, string carrierID, string carrierLoc, VIDEnums.IDReadStatus idReadStatus)
        {
            this.CommandID = commandID;
            this.CarrierID = carrierID;
            this.CarrierLoc = carrierLoc;
            this.IDReadStatus = idReadStatus;
        }
        public string CommandID { get; }
        public string CarrierID { get; }
        public string CarrierLoc { get; }
        public VIDEnums.IDReadStatus IDReadStatus { get; }
    }
}