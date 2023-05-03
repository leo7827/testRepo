using System;

namespace Mirle.Stocker.R46YP320.Events
{
    public class ReqAckEventArgs : EventArgs
    {
        public int CraneId { get; private set; }
        public int ForkId { get; private set; }

        public ReqAckEventArgs(int craneId, int forkId)
        {
            ForkId = forkId;
            CraneId = craneId;
        }
    }
}