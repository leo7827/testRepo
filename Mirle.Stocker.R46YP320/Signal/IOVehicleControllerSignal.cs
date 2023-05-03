using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class IOVehicleControllerSignal
    {
        public Bit HomeReturn { get; internal set; }
        public Bit Run { get; internal set; }
        public Bit ErrorReset { get; internal set; }
    }
}