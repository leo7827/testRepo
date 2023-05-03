using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class IOPortControllerSignal
    {
        public Word Word1 { get; internal set; }
        public Word Word3 { get; internal set; }
        public Word Word4 { get; internal set; }

        public Bit ReserveToMGV { get; internal set; }
        public Bit FaultReset { get; internal set; }
        public Bit BuzzerStop { get; internal set; }
        public Bit Run { get; internal set; }
        public Bit Stop { get; internal set; }
        public Bit IDReadCommand { get; internal set; }
        public Bit DoorOpenOHS { get; internal set; }
        public Bit MoveBack { get; internal set; }

        public Bit DoorOpenMGV { get; internal set; }
        public Bit AreaSensorToIOPort { get; internal set; }
        public Bit RequestMGVMode { get; internal set; }
        public Bit RequestAGVMode { get; internal set; }
        public Bit RequestInputMode { get; internal set; }
        public Bit RequestOutputMode { get; internal set; }

        public Word PcErrorIndex { get; internal set; }

        public Bit BCRDisable_P1 { get; internal set; }
    }
}