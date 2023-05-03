using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class CraneMotorSignal
    {
        public Word Motor_01_Travel1 { get; internal set; }
        public Word Motor_02_Travel2 { get; internal set; }
        public Word Motor_03_Travel3 { get; internal set; }
        public Word Motor_04_Travel4 { get; internal set; }
        public Word Motor_05_Lifter1 { get; internal set; }
        public Word Motor_06_Lifter2 { get; internal set; }
        public Word Motor_07_Lifter3 { get; internal set; }
        public Word Motor_08_Lifter4 { get; internal set; }
        public Word Motor_09_Rotate1 { get; internal set; }
        public Word Motor_10_Fork1 { get; internal set; }
        public Word Motor_11_Fork2 { get; internal set; }

        public Word Motor_01_Travel1_ErrorCode { get; internal set; }
        public Word Motor_02_Travel2_ErrorCode { get; internal set; }
        public Word Motor_03_Travel3_ErrorCode { get; internal set; }
        public Word Motor_04_Travel4_ErrorCode { get; internal set; }
        public Word Motor_05_Lifter1_ErrorCode { get; internal set; }
        public Word Motor_06_Lifter2_ErrorCode { get; internal set; }
        public Word Motor_07_Lifter3_ErrorCode { get; internal set; }
        public Word Motor_08_Lifter4_ErrorCode { get; internal set; }
        public Word Motor_09_Rotate1_ErrorCode { get; internal set; }
        public Word Motor_10_Fork1_ErrorCode { get; internal set; }
        public Word Motor_11_Fork2_ErrorCode { get; internal set; }
    }
}