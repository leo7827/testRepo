using System;
using System.Collections.Generic;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class IDReadErrorEventArgs : EventArgs
    {
        public IDReadErrorEventArgs(string carrierID, string carrierLoc, VIDEnums.IDReadStatus dreadStatus)
        {
            this.CarrierID = carrierID;
            this.CarrierLoc = carrierLoc;
            this.IDreadStatus = dreadStatus;
        }
        public string CarrierID { get; }
        public string CarrierLoc { get; }
        public VIDEnums.IDReadStatus IDreadStatus { get; }
    }
}