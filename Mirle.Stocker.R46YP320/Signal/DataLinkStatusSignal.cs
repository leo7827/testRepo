using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class DataLinkStatusSignal
    {
        public int Id { get; }
        public bool LastDataLinkStatusIsOn { get; internal set; }

        public DataLinkStatusSignal(int id)
        {
            Id = id;
        }

        public Bit DataLinkStatus { get; internal set; }
    }
}