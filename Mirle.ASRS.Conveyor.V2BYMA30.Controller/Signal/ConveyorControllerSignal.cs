using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyor.V2BYMA30_3F.Signal
{
    public class ConveyorControllerSignal
    {
        public Word Heartbeat { get; internal set; }
        public WordBlock SystemTimeCalibration { get; internal set; }

        public Word Path { get; internal set; }

        public Word Door { get; internal set; }

        public Word ErrorIndex { get; internal set; }
    }
}
