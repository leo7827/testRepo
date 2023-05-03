using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class SCStateEventArgs : EventArgs
    {
        public SCStateEventArgs(string equipmentName)
        {
            this.EqpName = equipmentName;
        }

        public string EqpName { get; }
    }
}