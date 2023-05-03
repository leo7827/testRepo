using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class IOVehicleSignal
    {
        public int Id { get; }
        public IOVehicleControllerSignal Controller { get; internal set; }

        public IOVehicleSignal(int id)
        {
            Id = id;
            Controller = new IOVehicleControllerSignal();
        }

        public Bit Active { get; internal set; }
        public Bit LoadPresence { get; internal set; }
        public Bit Error { get; internal set; }
        public Bit HomePosition { get; internal set; }
        public Bit HPReturn { get; internal set; }
        public Bit Auto { get; internal set; }
        public Bit CurrentLocation_P1 { get; internal set; }
        public Bit CurrentLocation_P2 { get; internal set; }
        public Bit CurrentLocation_P3 { get; internal set; }
        public Bit CurrentLocation_P4 { get; internal set; }
        public Bit CurrentLocation_P5 { get; internal set; }

        public Word RealTimePosition { get; internal set; }

        public WordBlock CarrierId { get; internal set; }

        public Word MileageTravel { get; internal set; }
    }
}