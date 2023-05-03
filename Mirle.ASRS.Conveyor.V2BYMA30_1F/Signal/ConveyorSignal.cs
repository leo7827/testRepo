using Mirle.MPLC.DataType;
using System.Collections;
using System.Collections.Generic;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.Signal
{
    public class ConveyorSignal
    {
        public Word Heartbeat { get; internal set; }
       
        public Word ErrorIndex { get; internal set; }
        public Word ErrorCode { get; internal set; }
        public Word ErrorStatus { get; internal set; }
       
        public BufferAlarmBitSignal AlarmBit { get; internal set; }
        public ConveyorControllerSignal Controller { get; internal set; }
    }
}
