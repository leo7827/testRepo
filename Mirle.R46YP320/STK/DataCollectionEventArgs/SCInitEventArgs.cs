using Mirle.Structure.Info;
using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class SCInitEventArgs : EventArgs
    {
        public SCInitEventArgs(string equipmentName, VIDEnums.PauseReason pauseReason)
        {
            this.EqpName = equipmentName;
            this.PauseReason = pauseReason;
        }

        public string EqpName { get; }

        public VIDEnums.PauseReason PauseReason { get; }
    }
}