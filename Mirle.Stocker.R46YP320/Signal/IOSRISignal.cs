using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class IOSRISignal
    {
        public Bit AutoManualSwitchIsAuto { get; internal set; }
        public Bit SafetyDoorClosed { get; internal set; }
        public Bit EMO { get; internal set; }

        public Bit MainCircuitOnEnable { get; set; }
    }
}