using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class ForkStatusEventArgs : EventArgs
    {
        public ForkStatusEventArgs(string commandID, string stockerCraneID, string carrierLoc)
        {
            this.CommandID = commandID;
            this.StockerCraneID = stockerCraneID;
            this.CarrierLoc = carrierLoc;
        }
        public string CommandID { get; }
        public string StockerCraneID { get; }
        public string CarrierLoc { get; }
    }
}