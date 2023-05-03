using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.Conveyor.V2BYMA30_8F
{
    public class ConveyorConfig
    {
        public string IPAddress { get; private set; }
        public int Port { get; private set; }
        public string ConveyorId { get;  }
        public int MPLCNo { get; } = 0;
        public bool IsMemorySimulator { get; } = false;
        public bool IsUseMCProtocol { get; } = false;

        public ConveyorConfig(string conveyorId, int ActLogicalStationNo, bool isMemorySimulator, bool isUseMCProtocol)
        {
            ConveyorId = conveyorId;
            IsMemorySimulator = isMemorySimulator;
            IsUseMCProtocol = isUseMCProtocol;
            MPLCNo = ActLogicalStationNo;
        }

        public void SetIPAddress(string iPAddress)
        {
            IPAddress = iPAddress;
        }

        public void SetPort(int port)
        {
            Port = port;
        }
    }
}
