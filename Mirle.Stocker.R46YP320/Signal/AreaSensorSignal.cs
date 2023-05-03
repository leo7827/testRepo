using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class AreaSensorSignal
    {
        public int Id { get; }
        public bool LastAreaSensorIson { get; internal set; }

        public AreaSensorSignal(int id)
        {
            Id = id;
        }

        public Bit AreaSensor { get; internal set; }
    }
}