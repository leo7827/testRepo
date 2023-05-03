using System;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class ZoneCapacityChangeEventArgs : EventArgs
    {
        public ZoneCapacityChangeEventArgs(string carrierID, ZoneData zoneData)
        {
            this.CarrierID = carrierID;
            this.ZoneData = zoneData;
        }

        public string CarrierID { get; }
        public ZoneData ZoneData { get; }
    }
}