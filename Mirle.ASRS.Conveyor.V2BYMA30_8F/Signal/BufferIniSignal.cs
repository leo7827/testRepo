using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyor.V2BYMA30_8F.Signal
{
    public class BufferIniSignal
    {
        public Bit IniReq { get; internal set; }

        public Bit PathFinish { get; internal set; }
    } 
}
