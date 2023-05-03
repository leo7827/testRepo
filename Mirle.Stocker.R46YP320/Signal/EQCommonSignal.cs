using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class EQCommonSignal
    {
        public Bit PLCBatteryLow_CPU { get; internal set; }
        public Bit PLCBatteryLow_SRAM { get; internal set; }
    }
}