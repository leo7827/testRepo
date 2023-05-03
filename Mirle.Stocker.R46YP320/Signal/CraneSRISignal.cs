using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class CraneSRISignal
    {
        public Bit TheAMSwitchIsAuto_RM { get; internal set; }
        public Bit EMO { get; internal set; }
        public Bit HIDPowerOn { get; internal set; }
        public Bit NoError { get; internal set; }
        public Bit MainCircuitOnEnable { get; internal set; }
    }
}