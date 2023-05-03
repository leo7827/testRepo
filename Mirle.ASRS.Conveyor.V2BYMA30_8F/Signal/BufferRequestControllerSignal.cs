using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyor.V2BYMA30_8F.Signal
{
    public class BufferRequestControllerSignal
    {
        public Word InitalReq { get; internal set; }
        public Word ReadBcrReq { get; internal set; }

        public BufferIniSignal IniSignalReq { get; internal set; }

    }
}
