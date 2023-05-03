using Mirle.Stocker.R46YP320.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mirle.Stocker.R46YP320
{
    public interface IIOPort
    {
        event StockerEvents.IOEventHandler OnCSTRemoved;

        event StockerEvents.IOEventHandler OnCSTWaitIn;

        event StockerEvents.IOEventHandler OnCSTWaitOut;

        event StockerEvents.IOEventHandler OnDirectionChanged;

        event StockerEvents.IOEventHandler OnInServiceChanged;

        event StockerEvents.IOEventHandler OnBCRReadDone;

        StockerEnums.IOPortDirection Direction { get; }
        int Id { get; }
        bool IsCstWainIn { get; }
        bool IsInService { get; }
        bool IsPortModeChangeable { get; }
        bool IsReadyToDeposit { get; }
        bool IsReadyToRetrieve { get; }
        bool IsAlarm { get; }
        bool CanAutoSetRun { get; set; }
        StockerEnums.PortLoadRequestStatus LoadRequestStatus { get; }
        IEnumerable<IIOStage> Stages { get; }
        StockerEnums.IOPortStatus Status { get; }
        IEnumerable<IIOVehicle> Vehicles { get; }

        IIOStage GetStageById(int id);

        IIOVehicle GetVehicleById(int id);

        bool HasCarrierOf(string cstid);

        Task RequestInModeAsync();

        Task RequestOutModeAsync();

        Task RequestBCRReadAsync();

        bool IsReadyFromCraneSide { get; }
    }
}