using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CarrierTransferringEventArgs : EventArgs
    {
        public CarrierTransferringEventArgs(string carrierID, string carrierLoc)
        {
            this.CarrierID = carrierID;
            this.CarrierLoc = carrierLoc;
        }
        public string CarrierID { get; }
        public string CarrierLoc { get; }
    }
}