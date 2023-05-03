using System.Collections.Generic;
using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class StockerSignal
    {
        public StockerControllerSignal Controller { get; internal set; }

        internal readonly Dictionary<int, AreaSensorSignal> _areaSensors = new Dictionary<int, AreaSensorSignal>();
        internal readonly Dictionary<int, DataLinkStatusSignal> _dataLinkStatusStations = new Dictionary<int, DataLinkStatusSignal>();
        
        public IEnumerable<AreaSensorSignal> AreaSensors => _areaSensors.Values;
        public IEnumerable<DataLinkStatusSignal> DataLinkStatusStations => _dataLinkStatusStations.Values;

        public AreaSensorSignal GetAreaSensorSignalById(int id)
        {
            _areaSensors.TryGetValue(id, out var areaSensor);
            return areaSensor;
        }

        public DataLinkStatusSignal GetDataLinkStatusSignalById(int id)
        {
            _dataLinkStatusStations.TryGetValue(id, out var dataLinkStatus);
            return dataLinkStatus;
        }

        public EQCommonSignal EQCommon { get; internal set; }

        public Bit PLCBatteryLow_CPU { get; internal set; }
        public Bit PLCBatteryLow_SRAM { get; internal set; }

        public Bit MaintenanceMode { get; internal set; }
        public Bit CassetteIdSetComplete { get; internal set; }

        public Bit SafetyDoorClosed_HP { get; internal set; }
        public Bit SafetyDoorClosed_OP { get; internal set; }
        public Bit KeySwitch_HP { get; internal set; }
        public Bit KeySwitch_OP { get; internal set; }
        public Bit EQNetworkError { get; internal set; }
        public Bit IONetworkError { get; internal set; }

        public Word ShareArea_StartBay { get; internal set; }
        public Word ShareArea_EndBay { get; internal set; }
        public Word HandOff_StartBay { get; internal set; }
        public Word HandOff_EndBay { get; internal set; }

        public Word AreaSensorSignal1 { get; internal set; }
        public Word AreaSensorSignal2 { get; internal set; }
        public Word AreaSensorSignal3 { get; internal set; }
        public Word AreaSensorSignal4 { get; internal set; }
    }
}
