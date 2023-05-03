using Mirle.Stocker.R46YP320.Events;

namespace Mirle.Stocker.R46YP320
{
    public interface IIOStage
    {
        event StockerEvents.IOStageEventHandler OnCstidChanged;

        event StockerEvents.IOStageEventHandler OnLoadPresenceChanged;

        string CstId { get; }
        bool HasCarrier { get; }
        int Id { get; }
    }
}