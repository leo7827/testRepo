using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class ForkSignal
    {
        public Bit CSTPresence { get; internal set; }

        public Bit Forking1 { get; internal set; }
        public Bit Forking2 { get; internal set; }
        public Bit Cycle1 { get; internal set; }
        public Bit Cycle2 { get; internal set; }

        public Bit LoadPresenceSensor { get; internal set; }
        public Bit ForkHomePosition { get; internal set; }

        public Bit Forking { get; internal set; }

        public Bit Idle { get; internal set; }

        public Bit Rised { get; internal set; }
        public Bit Downed { get; internal set; }

        public WordBlock TrackingCstId { get; internal set; }
        public WordBlock BCRResultCstId { get; internal set; }

        public Word CompletedCode { get; internal set; }
        public Word CompletedCommand { get; internal set; }
        public Word CurrentCommand { get; internal set; }
        public Word ForkCounter { get; internal set; }

        public Bit ForkDisable { get; internal set; }
    }
}
