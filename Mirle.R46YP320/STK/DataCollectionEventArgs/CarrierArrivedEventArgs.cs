using System;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CarrierArrivedEventArgs : EventArgs
    {
        public CarrierArrivedEventArgs(string carrierID, string carrierLoc, VIDEnums.PortType portType)
        {
            this.CarrierID = carrierID;
            this.CarrierLoc = carrierLoc;
            this.PortType = portType;
        }
        public string CarrierID { get; }
        public string CarrierLoc { get; }
        public VIDEnums.PortType PortType { get; }

        //public CarrierArrivedEventArgs(string equipmentName, string carrierID, string carrierLoc, string portType)
        //{
        //    this.EqpName = equipmentName;
        //    this.CarrierID = carrierID;
        //    this.CarrierLoc = carrierLoc;
        //    this.PortType = portType;
        //}
        //public string EqpName { get; }
        //public string CarrierID { get; }
        //public string CarrierLoc { get; }
        //public string PortType { get; }
    }
}