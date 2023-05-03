using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyor.V2BYMA30_8F.Signal
{
    public class BufferAckSignal
    {
        public Word InitalAck { get; internal set; }
        public Word ReadBcrAck { get; internal set; }

        public BufferIniSignalAck IniSignalACK { get; internal set; }
    }
}
