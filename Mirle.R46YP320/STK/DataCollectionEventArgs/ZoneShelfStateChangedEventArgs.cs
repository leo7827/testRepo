using Mirle.Structure.Info;
using System;
using System.Collections.Generic;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class ZoneShelfStateChangedEventArgs : EventArgs
    {
        public ZoneShelfStateChangedEventArgs(List<ZoneData2> zone)
        {
            this.DV_AllEnhancedDisableLocations = zone;
        }

        public List<ZoneData2> DV_AllEnhancedDisableLocations { get; }
    }
}