using Mirle.Stocker.R46YP320.Events;

namespace Mirle.Stocker.R46YP320
{
    public interface IIOVehicle
    {
        event StockerEvents.IOVehicleEventHandler OnLoadPresenceChanged;

        event StockerEvents.IOVehicleEventHandler OnCstidChanged;

        string CstId { get; }
        bool HasCarrier { get; }
        int Id { get; }
    }
}