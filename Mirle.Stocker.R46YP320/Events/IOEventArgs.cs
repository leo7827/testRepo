using System;

namespace Mirle.Stocker.R46YP320.Events
{
    public class IOEventArgs : EventArgs
    {
        public int IOId { get; }
        public bool SignalIsOn { get; set; }
        public StockerEnums.PortLoadRequestStatus NewLoadRequestStatus { get; set; }
        public string CstId { get; set; }
        public string BoxId { get; set; }
        public StockerEnums.IOPortDirection NewDirection { get; set; }

        public IOEventArgs(int ioId)
        {
            IOId = ioId;
        }
    }
}