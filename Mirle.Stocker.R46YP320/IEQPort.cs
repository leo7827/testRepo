using Mirle.Stocker.R46YP320.Events;

namespace Mirle.Stocker.R46YP320
{
    public interface IEQPort
    {
        event StockerEvents.EQEventHandler OnCSTPresentChanged;

        event StockerEvents.EQEventHandler OnCstidChanged;

        event StockerEvents.EQEventHandler OnInServiceChanged;

        event StockerEvents.EQEventHandler OnLoadRequestStatusChanged;

        string CSTID { get; }
        bool HasCarrier { get; }
        int Id { get; }
        bool IsInService { get; }
        bool IsReadyToDeposit { get; }
        bool IsReadyToRetrieve { get; }
        bool IsReady { get; }
        StockerEnums.PortLoadRequestStatus LoadRequestStatus { get; }
        StockerEnums.EQPortStatus Status { get; }
    }
}