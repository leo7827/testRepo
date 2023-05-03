using System;
using Mirle.R46YP320.STK.TaskService;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CarrierStatusEventArgs : EventArgs
    {
        public CarrierStatusEventArgs(string carrierID, string carrierLoc, string carrierZoneName)
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