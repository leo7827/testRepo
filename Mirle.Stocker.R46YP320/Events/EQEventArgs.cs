using System;

namespace Mirle.Stocker.R46YP320.Events
{
    public class EQEventArgs : EventArgs
    {
        public int EQId { get; }
        public bool SignalIsOn { get; set; }
        public string CstId { get; set; }
        public string BoxId { get; set; }
        public StockerEnums.PortLoadRequestStatus NewLoadRequestStatus { get; set; }

        public EQEventArgs(int eqId)
        {
            EQId = eqId;
        }
    }
}