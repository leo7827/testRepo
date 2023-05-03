using System;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CarrierWaitOutEventArgs : EventArgs
    {
        public CarrierWaitOutEventArgs(string carrierID, string carrierLoc, VIDEnums.PortType portType)
        {
            this.CarrierID = carrierID;
            this.CarrierLoc = carrierLoc;
            this.PortType = portType;
        }
        public string CarrierID { get; }
        public string CarrierLoc { get; }
        public VIDEnums.PortType PortType { get; }
    }
}