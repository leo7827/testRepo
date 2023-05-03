using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class CraneServiceStatusEventArgs : EventArgs
    {
        public CraneServiceStatusEventArgs(string stockerCraneID)
        {
            this.StockerCraneID = stockerCraneID;
        }
        public string StockerCraneID { get; }
    }
}