using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyor.V2BYMA30_8F.Signal
{
    public class BufferIniSignalAck
    {
        public Bit IniACK { get; internal set; }

        public Bit PathFinishACK { get; internal set; }
    }
}
