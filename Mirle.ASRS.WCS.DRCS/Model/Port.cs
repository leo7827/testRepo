using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirle.ASRS.WCS.DRCS.Define;

namespace Mirle.ASRS.WCS.DRCS.Model
{
    public class Port : IPort
    {
        public string DeviceID { get; }
        public string LocationId { get; }
        public string HostPortID { get; }
        public PortTypes PortType { get; }
        public int PortTypeIndex { get; }
        public int PLCPortID { get; }
        public int AlarmType { get; private set; }
        public int TimeoutAutoUD { get; private set; }
        public string AutoUDLoc { get; private set; }
        public string AlternateLoc { get; private set; }
        public LocationTypes LocationTypes => LocationTypes.Port;

        public Port(string deviceID, string locatioId, string hostPortID, PortTypes portType, int portTypeIndex, int pLCPortID)
        {
            DeviceID = deviceID;
            LocationId = locatioId;
            HostPortID = hostPortID;
            PortType = portType;
            PortTypeIndex = portTypeIndex;
            PLCPortID = pLCPortID;
        }

        public void SetAlarmType(int alarmType)
        {
            AlarmType = alarmType;
        }

        public void SetAutoUnload(int timeoutAutoUD, string autoUDLoc)
        {
            TimeoutAutoUD = timeoutAutoUD;
            AutoUDLoc = autoUDLoc;
        }

        public void SetAlternateLoc(string alternateLoc)
        {
            AlternateLoc = alternateLoc;
        }
    }
}
