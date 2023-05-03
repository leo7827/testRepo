using Mirle.Extensions;
using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class EQPortSignal
    {
        public int Id { get; }

        public EQPortSignal(int id)
        {
            Id = id;
        }

        public Bit L_REQ { get; internal set; }
        public Bit U_REQ { get; internal set; }
        public Bit Ready { get; internal set; }
        public Bit Carrier { get; internal set; }
        public Bit PError { get; internal set; }
        public Bit Spare { get; internal set; }
        public Bit POnline { get; internal set; }
        public Bit PEStop { get; internal set; }
        public Bit Transferring_FromSTK { get; internal set; }
        public Bit TR_REQ_FromSTK { get; internal set; }
        public Bit BUSY_FromSTK { get; internal set; }
        public Bit COMPLETE_FromSTK { get; internal set; }
        public Bit CRANE_1_FromSTK { get; internal set; }
        public Bit CRANE_2_FromSTK { get; internal set; }
        public Bit AError_FromSTK { get; internal set; }
        public Bit ForkNumber_FromSTK { get; internal set; }

        public Bit PriorityUp { get; internal set; }

        public WordBlock CarrierId { get; internal set; }

        public string CSTID => CarrierId.GetData().ToASCII();
    }
}
