using System;

namespace Mirle.Stocker.R46YP320.Events
{
    public class IOVehicleEventArgs : EventArgs
    {
        public int IOPortId { get; }
        public int VehicleId { get; }
        public bool SignalIsOn { get; set; }
        public StockerEnums.IOPortVehicleStatus NewStatus { get; set; }
        public string CstId { get; set; }
        public string BoxId { get; set; }

        public IOVehicleEventArgs(int vehicleId, int ioPortId)
        {
            IOPortId = ioPortId;
            VehicleId = vehicleId;
        }
    }
}