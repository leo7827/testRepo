using Mirle.MPLC.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.Conveyor.V2BYMA30_Elevator.Signal
{
    public class ElevatorStatusSignal
    {
        public Bit AgvMode { get; internal set; }
        public Bit PlatformOn { get; internal set; }
        public Bit PlatformOff { get; internal set; }
        public Bit Idle { get; internal set; }
        public Bit Running { get; internal set; }
        public Bit Up { get; internal set; }
        public Bit Down { get; internal set; }
       
    }
}
