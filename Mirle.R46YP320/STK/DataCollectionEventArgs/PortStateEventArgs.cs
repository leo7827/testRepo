using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class PortStateEventArgs : EventArgs
    {
        public PortStateEventArgs(string portID)
        {
            this.PortID = portID;
        }
        public string PortID { get; }
    }
}