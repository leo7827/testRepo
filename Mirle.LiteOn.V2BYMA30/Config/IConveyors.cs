using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Config.Net;

namespace Mirle.LiteOn.V2BYMA30.Config
{
    public interface IConveyors
    {
        [Option(Alias = "1FE04CV")] bool E04CV10F { get; set; }

        [Option(Alias = "8FE04Lift")] bool E04Lift { get; set; }
        [Option(Alias = "8FE04CV")] bool E04CV8F { get; set; } 
    }
}
