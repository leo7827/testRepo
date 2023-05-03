using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Net;

namespace Mirle.LiteOn.V2BYMA30.Config
{
    public interface ITimeinterval
    {
        [Option(Alias = "TimeInterval")] int TimeInterval { get; set; }
    }
}
