using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CarrierStoredAltEventArgs : EventArgs
    {
        public CarrierStoredAltEventArgs(string carrierID, string carrierLoc, string dest)
        {
            this.CarrierID = carrierID;
            this.CarrierLoc = carrierLoc;
            this.Dest = dest;
        }
        public string CarrierID { get; }
        public string CarrierLoc { get; }
        public string Dest { get; }
    }
}