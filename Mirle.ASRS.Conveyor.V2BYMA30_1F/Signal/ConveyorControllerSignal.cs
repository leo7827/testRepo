using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.Signal
{
    public class ConveyorControllerSignal
    {
        public Word Heartbeat { get; internal set; }
        public WordBlock SystemTimeCalibration { get; internal set; }

     

        public Word ErrorIndex { get; internal set; }
    }
}
