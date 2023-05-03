using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.WCS.DRCS
{
    public interface ILocation
    {
        string DeviceID { get; }
        string LocationId { get; }
        LocationTypes LocationTypes { get; }
    }
}
