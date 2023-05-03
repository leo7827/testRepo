using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CraneStatusEventArgs : EventArgs
    {
        public CraneStatusEventArgs(string commandID, string stockerCraneID)
        {
            this.CommandID = commandID;
            this.StockerCraneID = stockerCraneID;
        }
        public string CommandID { get; }
        public string StockerCraneID { get; }
    }
}