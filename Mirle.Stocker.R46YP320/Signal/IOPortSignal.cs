using System.Collections.Generic;

using Mirle.Extensions;
using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class IOPortSignal
    {
        public int Id { get; }
        public IOPortControllerSignal Controller { get; internal set; }
        public IOSRISignal SRI { get; internal set; }

        public IEnumerable<IOVehicleSignal> Vehicles => _vehicles.Values;
        internal readonly Dictionary<int, IOVehicleSignal> _vehicles = new Dictionary<int, IOVehicleSignal>();

        public IEnumerable<IOStageSignal> Stages => _stages.Values;
        internal readonly Dictionary<int, IOStageSignal> _stages = new Dictionary<int, IOStageSignal>();

        public IOVehicleSignal GetVehicleSignalById(int id)
        {
            _vehicles.TryGetValue(id, out var vehicle);
            return vehicle;
        }

        public IOStageSignal GetStageSignalById(int id)
        {
            _stages.TryGetValue(id, out var stage);
            return stage;
        }

        public IOPortSignal(int id)
        {
            Id = id;
        }

        //IO Port Status 1
        public Bit Run { get; internal set; }

        public Bit Down { get; internal set; }
        public Bit Fault { get; internal set; }
        public Bit InMode { get; internal set; }
        public Bit OutMode { get; internal set; }
        public Bit PortModeChangeable { get; internal set; }
        public Bit ForkSensor { get; internal set; }
        public Bit FFUError { get; internal set; }
        public Bit WaitIn { get; internal set; }
        public Bit WaitOut { get; internal set; }

        public Bit AutoManualMode { get; internal set; }
        public Bit LoadOK { get; internal set; }
        public Bit UnloadOK { get; internal set; }

        //IO Port Status 2
        public Bit BCRReadDone { get; internal set; }

        public Bit CSTTransferComplete_Req { get; internal set; }
        public Bit CSTRemoveCheck_Req { get; internal set; }

        //IO Port Status 3
        public Bit PLCBatteryLow_CPU { get; internal set; }

        public Bit DoorOpenLimit_MGV { get; internal set; }
        public Bit GlassDetection_MGV { get; internal set; }

        public Bit Ready_CraneSide { get; internal set; }
        public Bit TRRequest_CraneSide { get; internal set; }
        public Bit Busy_CraneSide { get; internal set; }
        public Bit Complete_CraneSide { get; internal set; }

        //IO Port Status 4
        public Bit RunEnable { get; internal set; }

        public Word ErrorCode { get; internal set; }

        public int ErrCode_Main
        {
            get
            {
                return ErrorCode.GetValue() / 256;
            }
        }

        public int ErrCode_Sub
        {
            get
            {
                return ErrorCode.GetValue() % 256;
            }
        }

        public Word CSTAttribute { get; internal set; }

        public WordBlock BCRReadResult { get; internal set; }

        public string CSTID_BarcodeResultOnP1 => BCRReadResult.GetData().ToASCII();

        public Word CountOfTransfer { get; internal set; }
        public Word ErrorIndex { get; internal set; }

        public Word IF_Signal1 { get; internal set; }
        public Word IF_Signal2 { get; internal set; }
        public Word IF_Signal3 { get; internal set; }
        public Word IF_Signal4 { get; internal set; }
        public Word IF_Signal5 { get; internal set; }

        public Bit L_REQ { get; internal set; }
        public Bit U_REQ { get; internal set; }
        public Bit Ready { get; internal set; }
        public Bit CARRIER { get; internal set; }
        public Bit PError { get; internal set; }
        public Bit Spare { get; internal set; }
        public Bit POnline { get; internal set; }
        public Bit PEStop { get; internal set; }
        public Bit Transferring_FromSTK { get; internal set; }
        public Bit TR_REQ_FromSTK { get; internal set; }
        public Bit BUSY_FromSTK { get; internal set; }
        public Bit COMPLETE_FromSTK { get; internal set; }
        public Bit CRANE_1_FromSTK { get; internal set; }
        public Bit CRANE_2_FromSTK { get; internal set; }
        public Bit AError_FromSTK { get; internal set; }
        public Bit ForkNumber_FromSTK { get; internal set; }
    }
}
