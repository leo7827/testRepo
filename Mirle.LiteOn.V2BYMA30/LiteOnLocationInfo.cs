using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirle.Def;

namespace Mirle.LiteOn.V2BYMA30
{
    public class LiteOnLocationInfo
    {
        public Location[] GetLocations;
        public LiteOnLocationInfo(int size)
        {
            GetLocations = new Location[size];
        }
    }
}
