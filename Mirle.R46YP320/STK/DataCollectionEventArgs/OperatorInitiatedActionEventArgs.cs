using System;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class OperatorInitiatedActionEventArgs : EventArgs
    {
        public OperatorInitiatedActionEventArgs(string commandID, VIDEnums.CommandType commandType, string carrierID, string source, string dest, int priority)
        {
            this.CommandID = commandID;
            this.CommandType = commandType;
            this.CarrierID = carrierID;
            this.Source = source;
            this.Dest = dest;
            this.Priority = priority;
        }

        public string CommandID { get; }
        public VIDEnums.CommandType CommandType { get; }
        public string CarrierID { get; }
        public string Source { get; }
        public string Dest { get; }
        public int Priority { get; }
    }
}