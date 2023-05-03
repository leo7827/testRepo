using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class ControlModeEventArgs : EventArgs
    {
        public ControlModeEventArgs(string equipmentName)
        {
            this.EqpName = equipmentName;
        }

        public string EqpName { get; }
    }
}