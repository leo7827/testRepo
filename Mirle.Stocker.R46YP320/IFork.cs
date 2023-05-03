using Mirle.Stocker.R46YP320.Events;
using Mirle.Structure;

namespace Mirle.Stocker.R46YP320
{
    public interface IFork
    {
        event StockerEvents.ForkEventHandler OnCSTPresenceChanged;

        event StockerEvents.ForkEventHandler OnCSTReadReportOn;

        string BCRResult { get; }
        string CurrentCommand { get; }
        string CompleteCommand { get; }
        bool HasCarrier { get; }
        int Id { get; }
        Structure.Info.TransferBatch GetCommand();
        //ForkLimitInfo GetLimit();
        ForkSetupInfo GetConfig();
    }
}