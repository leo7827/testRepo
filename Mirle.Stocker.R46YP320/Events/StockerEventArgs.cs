using System;

namespace Mirle.Stocker.R46YP320.Events
{
    public class StockerEventArgs : EventArgs
    {
        public LCSEnums.AvailStatus NewAvailStatus { get; set; }
        public bool KeySwitchIsOn { get; set; }
        public bool SafetyDoorIsClosed { get; set; }
        public bool MPLCIsConnected { get; set; }
        public int StationId { get; set; }
        public bool ErrorIsOn { get; set; }
        public bool MaintenanceModeIsOn { get; set; }

        public StockerEventArgs()
        {
        }
    }
}