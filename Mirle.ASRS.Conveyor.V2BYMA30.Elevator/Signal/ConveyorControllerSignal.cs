using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyor.V2BYMA30_Elevator.Signal
{
    public class ConveyorControllerSignal
    {
        public Word Heartbeat { get; internal set; }
        public WordBlock SystemTimeCalibration { get; internal set; }

        /// <summary>
        /// 0:Initial  1:通知PLC 切換人員模式
        /// </summary>
        public Word ModeChange { get; internal set; }

        public Word Path { get; internal set; }

        public Word DoorNoticce { get; internal set; }

        public Word ErrorIndex { get; internal set; }
    }
}
