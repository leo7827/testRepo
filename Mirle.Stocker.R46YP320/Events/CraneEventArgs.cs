using System;

namespace Mirle.Stocker.R46YP320.Events
{
    public class CraneEventArgs : EventArgs
    {
        public int Id { get; }
        public LCSEnums.AvailStatus NewAvailStatus { get; set; }
        public StockerEnums.CraneStatus NewStatus { get; set; }
        public int T3 { get; set; }
        public string Location { get; set; }
        public bool KeySwitchIsOn { get; set; }
        public bool SignalIsOn { get; set; }

        /// <summary>
        /// Only For CraneEventHandler OnTaskCmdWriteToMPLC
        /// </summary>
        public string CommandId { get; set; }

        /// <summary>
        /// Only For CraneEventHandler OnTaskCmdWriteToMPLC
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// Only For CraneEventHandler OnTaskCmdWriteToMPLC
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// Only For CraneEventHandler OnTaskCmdWriteToMPLC
        /// </summary>
        public string BoxID { get; set; }

        public CraneEventArgs(int id)
        {
            Id = id;
        }
    }
}