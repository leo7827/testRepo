using System;

namespace Mirle.Stocker.R46YP320.Events
{
    public class ForkEventArgs : EventArgs
    {
        public int CraneId { get; private set; }
        public int ForkId { get; private set; }
        public bool SignalIsOn { get; set; }
        public string BCRResult { get; set; }
        public string BoxIDBCRResult { get; set; }
        public string CurrentCommandId { get; set; }
        public string CompletedCommandId { get; set; }
        public string CompletedCode { get; set; }

        public ForkEventArgs(int forkId, int craneId)
        {
            ForkId = forkId;
            CraneId = craneId;
        }
    }
}