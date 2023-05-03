using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mirle.Stocker.R46YP320
{
    public interface IStocker
    {
        LCSEnums.AvailStatus AvailStatus { get; }
        IEnumerable<ICrane> Cranes { get; }
        IEnumerable<IEQPort> EQPorts { get; }
        int HandOffEndBay { get; }
        int HandOffStartBay { get; }
        IEnumerable<IIOPort> IOPorts { get; }
        int ShareAreaEndBay { get; }
        int ShareAreaStartBay { get; }
        StockerEnums.StockerStatus Status { get; }

        bool IsPMSChargeShelfOnline { get; }
        bool IsPMSChargeShelfInPreserved { get; }

        ICrane GetCraneById(int id);

        IEQPort GetEQPortById(int id);

        IIOPort GetIOPortById(int id);

        bool IsAreaSensorOnById(int id);

        bool IsDataLinkStatusOnById(int id);

        Task SetPMSChargeShelfAsync(bool isPreserved);

        Task<bool> CalibrateSystemTimeAsync();
    }
}