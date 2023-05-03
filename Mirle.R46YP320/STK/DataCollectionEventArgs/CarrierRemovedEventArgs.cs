using System;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CarrierRemovedEventArgs : EventArgs
    {
        public CarrierRemovedEventArgs(string carrierID, VIDEnums.HandoffType handoffType, string carrierLoc)
        {
            this.CarrierID = carrierID;
            this.HandoffType = handoffType;
            this.CarrierLoc = carrierLoc;
        }
        public string CarrierID { get; }
        public VIDEnums.HandoffType HandoffType { get; }
        public string CarrierLoc { get; }
    }
}