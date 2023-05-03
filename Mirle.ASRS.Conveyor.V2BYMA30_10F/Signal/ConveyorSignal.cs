using Mirle.MPLC.DataType;
using System.Collections;
using System.Collections.Generic;

namespace Mirle.ASRS.Conveyor.V2BYMA30_10F.Signal
{
    public class ConveyorSignal
    {
        public Word Heartbeat { get; internal set; }

        /// <summary>
        /// 0：Initial 1:已讀
        /// </summary>
        public Word ModeStatus { get; internal set; }
        public Word ErrorIndex { get; internal set; }
        public Word ErrorCode { get; internal set; }
        public Word ErrorStatus { get; internal set; }
       
        public BufferAlarmBitSignal AlarmBit { get; internal set; }
        public ConveyorControllerSignal Controller { get; internal set; }

        public WordBlock OpcData { get; internal set; }
    }
}
