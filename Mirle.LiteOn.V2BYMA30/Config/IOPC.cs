using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.LiteOn.V2BYMA30.Config
{
    public interface IOPC
    {
        IConveyors Conveyors { get; set; }
        ITimeinterval TimeInterval { get; set; }
    }
}
