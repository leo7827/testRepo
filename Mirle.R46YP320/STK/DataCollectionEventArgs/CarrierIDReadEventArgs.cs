using System;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CarrierIDReadEventArgs : EventArgs
    {
        public CarrierIDReadEventArgs(string carrierID, string carrierLoc, VIDEnums.IDReadStatus dreadStatus)
        {
            this.CarrierID = carrierID;
            this.CarrierLoc = carrierLoc;
            this.IDreadStatus = dreadStatus;
        }
        public string CarrierID { get; }
        public string CarrierLoc { get; }
        public VIDEnums.IDReadStatus IDreadStatus { get; }

        //public CarrierIDReadEventArgs(string equipmentName, string carrierID, string carrierLoc, VIDEnums.IDReadStatus dreadStatus)
        //{
        //    this.EqpName = equipmentName;
        //    this.CarrierID = carrierID;
        //    this.CarrierLoc = carrierLoc;
        //    this.IDreadStatus = dreadStatus;
        //}
        //public string EqpName { get; }
        //public string CarrierID { get; }
        //public string CarrierLoc { get; }
        //public VIDEnums.IDReadStatus IDreadStatus { get; }
    }
}