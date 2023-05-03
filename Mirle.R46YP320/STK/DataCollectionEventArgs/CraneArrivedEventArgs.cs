using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CraneArrivedEventArgs : EventArgs
    {
        public CraneArrivedEventArgs(string carrierID, string commandID, string portID)
        {
            this.CarrierID = carrierID;
            this.CommandID = commandID;
            this.PortID = portID;
        }
        public string CarrierID { get; }
        public string CommandID { get; }
        public string PortID { get; }
    }
}