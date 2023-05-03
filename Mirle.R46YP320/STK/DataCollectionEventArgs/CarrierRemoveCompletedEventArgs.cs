using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CarrierRemoveCompletedEventArgs : EventArgs
    {
        public CarrierRemoveCompletedEventArgs(string carrierID, string carrierLoc, string carrierZoneName)
        {
            this.CarrierID = carrierID;
            this.CarrierLoc = carrierLoc;
            this.CarrierZoneName = carrierZoneName;
        }
        public string CarrierID { get; }
        public string CarrierLoc { get; }
        public string CarrierZoneName { get; }
    }
}