using Mirle.Extensions;
using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class IOStageSignal
    {
        public int Id { get; }

        public IOStageSignal(int id)
        {
            Id = id;
        }

        public Bit LoadPresence { get; internal set; }
        public WordBlock CarrierId { get; internal set; }

        public string CSTID => CarrierId.GetData().ToASCII();
    }
}