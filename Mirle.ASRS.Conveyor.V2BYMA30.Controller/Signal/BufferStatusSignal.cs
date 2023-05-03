using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyor.V2BYMA30_3F.Signal
{
    public class BufferStatusSignal
    {
        public Bit InMode { get; internal set; }
        public Bit OutMode { get; internal set; }
        public Bit Error { get; internal set; }
        public Bit Auto { get; internal set; }
        public Bit Manual { get; internal set; }
        public Bit Presence { get; internal set; }
        public Bit Position { get; internal set; }
        public Bit Finish { get; internal set; }
        public Bit EMO { get; internal set; }
        public Bit Online { get; internal set; }
        public Bit Offline { get; internal set; }
    }
}
