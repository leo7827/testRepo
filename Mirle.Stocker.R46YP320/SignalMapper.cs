using System;
using System.Collections.Generic;
using Mirle.MPLC;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.DataBlocks.DeviceRange;
using Mirle.MPLC.DataType;
using Mirle.Stocker.R46YP320.Signal;

namespace Mirle.Stocker.R46YP320
{
    public class SignalMapper4_11
    {
        public static readonly int MaximunEqPortNumber = 100;
        public static readonly int MaximumIoPortNumber = 25;
        public static readonly int MaximumIoPortVehicleNumber = 5;
        public static readonly int MaximumIoPortStageNumber = 5;
        public static readonly int MaximumAreaSensorNumber = 64;

        public static readonly List<BlockInfo> SignalBlocks = new List<BlockInfo>()
        {
            new BlockInfo(new DDeviceRange("D5000", "D11999"), "WordData", 0),
            new BlockInfo(new WDeviceRange("W2900", "W2BFF"), "WordData-EQ", 1),
        };

        private readonly IMPLCProvider _mplc;

        private readonly Dictionary<int, CraneSignal> _cranes = new Dictionary<int, CraneSignal>();
        private readonly Dictionary<int, EQPortSignal> _eqPorts = new Dictionary<int, EQPortSignal>();
        private readonly Dictionary<int, IOPortSignal> _ioPorts = new Dictionary<int, IOPortSignal>();

        public StockerSignal Stocker { get; private set; }

        public IEnumerable<CraneSignal> Cranes => _cranes.Values;

        public IEnumerable<EQPortSignal> EqPorts => _eqPorts.Values;

        public IEnumerable<IOPortSignal> IoPorts => _ioPorts.Values;

        public CraneSignal GetCraneSignalById(int id)
        {
            _cranes.TryGetValue(id, out var crane);
            return crane;
        }

        public EQPortSignal GetEQPortSignalById(int id)
        {
            _eqPorts.TryGetValue(id, out var eq);
            return eq;
        }

        public IOPortSignal GetIOPortSignalById(int id)
        {
            _ioPorts.TryGetValue(id, out var io);
            return io;
        }

        public SignalMapper4_11(IMPLCProvider mplc)
        {
            _mplc = mplc;
            MappingStocker();
            MappingCrane();
            MappingIoPorts();
            MappingEqPorts();
        }

        #region Stocker

        private void MappingStocker()
        {
            MappingStockerM2S();
            MappingStockerS2M();
        }

        private void MappingStockerS2M()
        {
            Stocker.Controller = new StockerControllerSignal();
            var ctrl = Stocker.Controller;
            ctrl.STKID = new WordBlock(_mplc, "D5001", 2);
            ctrl.StockerFull = new Bit(_mplc, "D5003.0");
            ctrl.MCSOnline = new Bit(_mplc, "D5003.1");

            ctrl.Heartbeat = new Bit(_mplc, "D5003.6");
            ctrl.DatetimeCalibration = new Bit(_mplc, "D5003.7");
            ctrl.CstIdSetCommand = new Bit(_mplc, "D5003.8");
            ctrl.SpeedMode = new Bit(_mplc, "D5003.9");

            ctrl.CstIdSetCommand_Type = new Word(_mplc, "D5004");
            ctrl.CstIdSetCommand_TypeCrane1 = new Bit(_mplc, "D5004.0");
            ctrl.CstIdSetCommand_TypeCrane2 = new Bit(_mplc, "D5004.1");
            ctrl.CstIdSetCommand_TypePort = new Bit(_mplc, "D5004.2");

            ctrl.PortNo = new Word(_mplc, "D5005");
            ctrl.LocationPosition = new Word(_mplc, "D5006");

            ctrl.SystemTimeCalibration = new WordBlock(_mplc, "D5007", 3);

            ctrl.EQSimulation_TrReq = new Bit(_mplc, "D5010.0");
            ctrl.EQSimulation_Busy = new Bit(_mplc, "D5010.1");
            ctrl.EQSimulation_Complete = new Bit(_mplc, "D5010.2");
            ctrl.EQSimulationSignal = new Word(_mplc, "D5010");

            ctrl.CSTID = new WordBlock(_mplc, "D5011", 6);

            ctrl.ShareArea_StartBay = new Word(_mplc, "D5017");
            ctrl.ShareArea_EndBay = new Word(_mplc, "D5018");
            ctrl.HandOff_StartBay = new Word(_mplc, "D5019");
            ctrl.HandOff_EndBay = new Word(_mplc, "D5020");

            ctrl.MPLC_IF_T1 = new Word(_mplc, "D5181");
            ctrl.MPLC_IF_T2 = new Word(_mplc, "D5182");
            ctrl.MPLC_IF_T3 = new Word(_mplc, "D5183");
            ctrl.MPLC_IF_T4 = new Word(_mplc, "D5184");
            ctrl.MPLC_IF_T5 = new Word(_mplc, "D5185");
            ctrl.MPLC_IF_T6 = new Word(_mplc, "D5186");

            ctrl.RM1_EscapePosition_Row = new Word(_mplc, "D5191");
            ctrl.RM1_EscapePosition_Bay = new Word(_mplc, "D5192");
            ctrl.RM1_EscapePosition_Level = new Word(_mplc, "D5193");

            ctrl.RM2_EscapePosition_Row = new Word(_mplc, "D5194");
            ctrl.RM2_EscapePosition_Bay = new Word(_mplc, "D5195");
            ctrl.RM2_EscapePosition_Level = new Word(_mplc, "D5196");
        }

        private void MappingStockerM2S()
        {
            Stocker = new StockerSignal();
            Stocker.PLCBatteryLow_CPU = new Bit(_mplc, "D6000.0");
            Stocker.PLCBatteryLow_SRAM = new Bit(_mplc, "D6000.1");

            Stocker.MaintenanceMode = new Bit(_mplc, "D6000.7");
            Stocker.CassetteIdSetComplete = new Bit(_mplc, "D6000.8");

            Stocker.SafetyDoorClosed_HP = new Bit(_mplc, "D6000.A");
            Stocker.SafetyDoorClosed_OP = new Bit(_mplc, "D6000.B");
            Stocker.KeySwitch_HP = new Bit(_mplc, "D6000.C");
            Stocker.KeySwitch_OP = new Bit(_mplc, "D6000.D");
            Stocker.EQNetworkError = new Bit(_mplc, "D6000.E");
            Stocker.IONetworkError = new Bit(_mplc, "D6000.F");

            Stocker.ShareArea_StartBay = new Word(_mplc, "D10900");
            Stocker.ShareArea_EndBay = new Word(_mplc, "D10901");
            Stocker.HandOff_StartBay = new Word(_mplc, "D10902");
            Stocker.HandOff_EndBay = new Word(_mplc, "D10903");

            Stocker.AreaSensorSignal1 = new Word(_mplc, "D10904");
            Stocker.AreaSensorSignal2 = new Word(_mplc, "D10905");
            Stocker.AreaSensorSignal3 = new Word(_mplc, "D10906");
            Stocker.AreaSensorSignal4 = new Word(_mplc, "D10907");

            const int areaSensorM2SStartAddress = 10904;
            int addr = areaSensorM2SStartAddress;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    var areaSensor = new AreaSensorSignal(i * 16 + j + 1);
                    areaSensor.AreaSensor = new Bit(_mplc, $"D{addr + i}.{j:X}");
                    Stocker._areaSensors.Add(areaSensor.Id, areaSensor);
                }
            }

            const int stationDataLinkStatusM2sStartAddress = 10910;
            addr = stationDataLinkStatusM2sStartAddress;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    var dataLink = new DataLinkStatusSignal(i * 16 + j + 1);
                    dataLink.DataLinkStatus = new Bit(_mplc, $"D{addr + i}.{j:X}");
                    Stocker._dataLinkStatusStations.Add(dataLink.Id, dataLink);
                }
            }
        }

        #endregion Stocker

        #region Crane

        private void MappingCrane()
        {
            const int craneS2MStartAddress = 5021;
            const int craneM2SStartAddress = 6001;

            int addr = 0;
            for (int i = 0; i < 2; i++)
            {
                var crane = new CraneSignal(i + 1);
                _cranes.Add(crane.Id, crane);

                addr = craneS2MStartAddress + 30 * i;
                MappingCraneController(crane, addr);

                addr = craneM2SStartAddress + 200 * i;
                MappingCraneStatus(crane, addr);
                MappingCraneRequestStatus(crane, addr);
                MappingCraneSRI(crane, addr);
                MappingCraneMotor(crane, addr);
                MappingCraneLeftFork(crane, addr);
                MappingCraneRightFork(crane, addr);

                addr = craneS2MStartAddress + 30 * i;
                MappingFrokDisable(crane, addr);
            }
        }

        private void MappingFrokDisable(CraneSignal crane, int startAddress)
        {
            var addr = startAddress + 23;//D5044
            crane.LeftFork.ForkDisable = new Bit(_mplc, $"D{addr}.0");
            //crane.LeftFork.ForkDisable = new Bit(_mplc, $"D{addr}.1");
            crane.RightFork.ForkDisable = new Bit(_mplc, $"D{addr}.1");
        }

        private void MappingCraneController(CraneSignal crane, int startAddress)
        {
            crane.Controller = new CraneControllerSignal();
            var ctrl = crane.Controller;
            var addr = startAddress;//D5021

            ctrl.D5021 = new Word(_mplc, $"D{addr}");
            ctrl.AckSignal = new CraneAckSignal();

            ctrl.AckSignal.EmptyRetrievalAck_LF = new Bit(_mplc, $"D{addr}.1");
            ctrl.AckSignal.DoubleStorageAck_LF = new Bit(_mplc, $"D{addr}.2");

            ctrl.AckSignal.ScanCompleteAck = new Bit(_mplc, $"D{addr}.4");
            ctrl.AckSignal.EQInlineInterlockErrAck_LF = new Bit(_mplc, $"D{addr}.5");
            ctrl.AckSignal.EQInlineInterlockErrAck_RF = new Bit(_mplc, $"D{addr}.6");
            ctrl.AckSignal.EmptyRetrievalAck_RF = new Bit(_mplc, $"D{addr}.7");
            ctrl.AckSignal.DoubleStorageAck_RF = new Bit(_mplc, $"D{addr}.8");
            ctrl.AckSignal.TransferRequestWrongAck_LF = new Bit(_mplc, $"D{addr}.9");
            ctrl.AckSignal.IOInlineInterlockErrAck_LF = new Bit(_mplc, $"D{addr}.A");
            ctrl.AckSignal.IDReadErrorAck_RF = new Bit(_mplc, $"D{addr}.B");
            ctrl.AckSignal.IDMismatchAck_RF = new Bit(_mplc, $"D{addr}.C");
            ctrl.AckSignal.IDReadErrorAck_LF = new Bit(_mplc, $"D{addr}.D");
            ctrl.AckSignal.IDMismatchAck_LF = new Bit(_mplc, $"D{addr}.E");
            ctrl.AckSignal.IOInlineInterlockErrAck_RF = new Bit(_mplc, $"D{addr}.F");

            addr = startAddress + 1;//D5022
            ctrl.D5022 = new Word(_mplc, $"D{addr}");
            ctrl.AckSignal.TransferRequestWrongAck_RF = new Bit(_mplc, $"D{addr}.0");
            ctrl.FaultReset = new Bit(_mplc, $"D{addr}.1");
            ctrl.BuzzerStop = new Bit(_mplc, $"D{addr}.2");
            ctrl.Run = new Bit(_mplc, $"D{addr}.3");
            ctrl.Stop = new Bit(_mplc, $"D{addr}.4");
            ctrl.StepStop = new Bit(_mplc, $"D{addr}.5");

            ctrl.CommandAbort = new Bit(_mplc, $"D{addr}.7");
            ctrl.CmdType_TransferWithoutIDRead = new Bit(_mplc, $"D{addr}.8");
            ctrl.CmdType_Transfer = new Bit(_mplc, $"D{addr}.9");
            ctrl.CmdType_Move = new Bit(_mplc, $"D{addr}.A");
            ctrl.CmdType_Scan = new Bit(_mplc, $"D{addr}.B");

            ctrl.HomeReturn = new Bit(_mplc, $"D{addr}.D");
            ctrl.UseLeftFork = new Bit(_mplc, $"D{addr}.E");
            ctrl.UseRightFork = new Bit(_mplc, $"D{addr}.F");

            addr = startAddress;
            //D5023
            ctrl.CommandData = new WordBlock(_mplc, $"D{addr + 2}", 14);
            ctrl.TransferNo = new Word(_mplc, $"D{addr + 2}");
            //D5024
            ctrl.FromLocation = new DWord(_mplc, $"D{addr + 3}");
            //D5026
            ctrl.ToLocation = new DWord(_mplc, $"D{addr + 5}");
            //D5028
            ctrl.BatchId = new Word(_mplc, $"D{addr + 7}");
            //D5029
            ctrl.CmdCstId = new WordBlock(_mplc, $"D{addr + 8}", 10);

            //D5039
            ctrl.PcErrorIndex = new Word(_mplc, $"D{addr + 18}");
            //D5040
            ctrl.StopRetryTimes = new Word(_mplc, $"D{addr + 19}");
            //D5041
            ctrl.TravelAxisSpeed = new Word(_mplc, $"D{addr + 20}");
            //D5042
            ctrl.LifterAxisSpeed = new Word(_mplc, $"D{addr + 21}");
            //D5043
            ctrl.RotateAxisSpeed = new Word(_mplc, $"D{addr + 22}");
            //D5044
            ctrl.ForkAxisSpeed = new Word(_mplc, $"D{addr + 23}");
            //D5041~D5044
            ctrl.AxisSpeed = new WordBlock(_mplc, $"D{addr + 20}", 4);
            //D5045
            ctrl.NextStation = new Word(_mplc, $"D{addr + 24}");
        }

        private void MappingCraneStatus(CraneSignal crane, int startAddress)
        {
            var addr = startAddress;//D6001
            crane.InService = new Bit(_mplc, $"D{addr}.0");
            crane.Run = new Bit(_mplc, $"D{addr}.1");
            crane.Error = new Bit(_mplc, $"D{addr}.2");
            crane.Idle = new Bit(_mplc, $"D{addr}.3");
            crane.Active = new Bit(_mplc, $"D{addr}.4");

            crane.TransferCommandReceived = new Bit(_mplc, $"D{addr}.7");
            crane.HPReturn = new Bit(_mplc, $"D{addr}.8");
            crane.Escape = new Bit(_mplc, $"D{addr}.9");

            crane.Approach = new Bit(_mplc, $"D{addr}.C");

            crane.ForkAtBank1 = new Bit(_mplc, $"D{addr}.E");
            crane.ForkAtBank2 = new Bit(_mplc, $"D{addr}.F");

            addr = startAddress + 1;//D6002
            crane.LocationUpdated = new Bit(_mplc, $"D{addr}.A");

            addr = startAddress + 2;//D6003

            crane.Dual_DualCraneCommunicationErr = new Bit(_mplc, $"D{addr}.2");
            crane.SingleCraneMode = new Bit(_mplc, $"D{addr}.3");
            crane.Dual_InterferenceWaiting = new Bit(_mplc, $"D{addr}.4");
            crane.Dual_HandOffReserved = new Bit(_mplc, $"D{addr}.5");
            crane.Dual_InterventionEntry = new Bit(_mplc, $"D{addr}.6");

            addr = startAddress + 3;//D6004
            crane.PLCBatteryLow_CPU = new Bit(_mplc, $"D{addr}.0");

            crane.DriverBatteryLow = new Bit(_mplc, $"D{addr}.2");

            crane.TravelHomePosition = new Bit(_mplc, $"D{addr}.5");
            crane.LifterHomePosition = new Bit(_mplc, $"D{addr}.6");
            crane.RotateHomePosition = new Bit(_mplc, $"D{addr}.7");

            crane.AnyFFUofCraneIsError = new Bit(_mplc, $"D{addr}.A");

            crane.TravelMoving = new Bit(_mplc, $"D{addr}.C");
            crane.LifterActing = new Bit(_mplc, $"D{addr}.D");

            crane.Rotating = new Bit(_mplc, $"D{addr}.F");

            addr = startAddress + 4;//D6005
            crane.RunEnable = new Bit(_mplc, $"D{addr}.0");
            crane.ReadyToReceiveNewCommand = new Bit(_mplc, $"D{addr}.1");

            crane.HomeLost = new Bit(_mplc, $"D{addr}.6");

            addr = startAddress;
            //D6006~D6049
            crane.Location = new Word(_mplc, $"D{addr + 5}");
            crane.CurrentPosition = new Word(_mplc, $"D{addr + 6}");
            crane.CSTAttribute = new Word(_mplc, $"D{addr + 7}");

            crane.ErrorCode = new Word(_mplc, $"D{addr + 15}");
            crane.T1 = new Word(_mplc, $"D{addr + 16}");
            crane.T2 = new Word(_mplc, $"D{addr + 17}");
            crane.T3 = new Word(_mplc, $"D{addr + 18}");
            crane.T4 = new Word(_mplc, $"D{addr + 19}");
            crane.EvacuationPositon = new Word(_mplc, $"D{addr + 20}");

            crane.ErrorIndex = new Word(_mplc, $"D{addr + 28}");

            crane.InterferenceRange = new Word(_mplc, $"D{addr + 31}");
            crane.MileageOfTravel = new DWord(_mplc, $"D{addr + 32}");
            crane.MiileageOfLifter = new Word(_mplc, $"D{addr + 34}");
            crane.RotatingCounter = new Word(_mplc, $"D{addr + 35}");

            crane.WrongCommandReasonCode = new Word(_mplc, $"D{addr + 37}");
            crane.CurrentBay = new Word(_mplc, $"D{addr + 38}");
            crane.CurrentLevel = new Word(_mplc, $"D{addr + 39}");
            crane.TravelAxisSpeed = new Word(_mplc, $"D{addr + 40}");
            crane.LifterAxisSpeed = new Word(_mplc, $"D{addr + 41}");
            crane.RotateAxisSpeed = new Word(_mplc, $"D{addr + 42}");
            crane.ForkAxisSpeed = new Word(_mplc, $"D{addr + 43}");

            //D6062~D6065
            crane.CraneToCraneDistance = new Word(_mplc, $"D{addr + 61}");
            crane.EscapePosition_Row = new Word(_mplc, $"D{addr + 62}");
            crane.EscapePosition_Bay = new Word(_mplc, $"D{addr + 63}");
            crane.EscapePosition_Level = new Word(_mplc, $"D{addr + 64}");

            //D6067~D6069
            crane.CommandBuffer1 = new Word(_mplc, $"D{addr + 66}");
            crane.CommandBuffer2 = new Word(_mplc, $"D{addr + 67}");
            crane.CommandBuffer3 = new Word(_mplc, $"D{addr + 68}");
        }

        private void MappingCraneRequestStatus(CraneSignal crane, int startAddress)
        {
            var addr = startAddress;//D6001
            crane.RequestSignal = new CraneRequestSignal();
            var reqAck = crane.RequestSignal;

            addr = startAddress + 1;//D6002

            reqAck.EmptyRetrievalReq_LF = new Bit(_mplc, $"D{addr}.1");
            reqAck.DoubleStorageReq_LF = new Bit(_mplc, $"D{addr}.2");

            reqAck.ScanCompleteReq = new Bit(_mplc, $"D{addr}.4");
            reqAck.EQInlineInterlockErrReq_LF = new Bit(_mplc, $"D{addr}.5");
            reqAck.EQInlineInterlockErrReq_RF = new Bit(_mplc, $"D{addr}.6");
            reqAck.EmptyRetrievalReq_RF = new Bit(_mplc, $"D{addr}.7");
            reqAck.DoubleStorageReq_RF = new Bit(_mplc, $"D{addr}.8");
            reqAck.TransferRequestWrongReq_LF = new Bit(_mplc, $"D{addr}.9");

            reqAck.IDReadErrorReq_RF = new Bit(_mplc, $"D{addr}.B");
            reqAck.IDMismatchReq_RF = new Bit(_mplc, $"D{addr}.C");
            reqAck.IDReadErrorReq_LF = new Bit(_mplc, $"D{addr}.D");
            reqAck.IDMismatchReq_LF = new Bit(_mplc, $"D{addr}.E");

            addr = startAddress + 2;//D6003
            reqAck.TransferRequestWrongReq_RF = new Bit(_mplc, $"D{addr}.F");

            addr = startAddress + 4;//D6005
            reqAck.IOInlineInterlockErrReq_LF = new Bit(_mplc, $"D{addr}.2");
            reqAck.IOInlineInterlockErrReq_RF = new Bit(_mplc, $"D{addr}.3");
        }

        private void MappingCraneSRI(CraneSignal crane, int startAddress)
        {
            var addr = startAddress;//D6001
            crane.SRI = new CraneSRISignal();
            var sri = crane.SRI;

            addr = startAddress + 4;//D6005

            //sri.SafetyDoorClosed = new Bit(_mplc, $"D{addr}.7");
            sri.TheAMSwitchIsAuto_RM = new Bit(_mplc, $"D{addr}.8");
            sri.EMO = new Bit(_mplc, $"D{addr}.9");
            sri.HIDPowerOn = new Bit(_mplc, $"D{addr}.A");
            //sri.HIDPowerOn_Crane2 = new Bit(_mplc, $"D{addr}.B");
            sri.NoError = new Bit(_mplc, $"D{addr}.C");
            sri.MainCircuitOnEnable = new Bit(_mplc, $"D{addr}.D");
        }

        private void MappingCraneMotor(CraneSignal crane, int startAddress)
        {
            var addr = startAddress;//D6001
            crane.Motor = new CraneMotorSignal();
            var motor = crane.Motor;

            motor.Motor_01_Travel1 = new Word(_mplc, $"D{addr + 49}");//D6050
            motor.Motor_02_Travel2 = new Word(_mplc, $"D{addr + 50}");//D6051
            motor.Motor_03_Travel3 = new Word(_mplc, $"D{addr + 51}");//D6052
            motor.Motor_04_Travel4 = new Word(_mplc, $"D{addr + 52}");//D6053
            motor.Motor_05_Lifter1 = new Word(_mplc, $"D{addr + 53}");//D6054
            motor.Motor_06_Lifter2 = new Word(_mplc, $"D{addr + 54}");//D6055
            motor.Motor_07_Lifter3 = new Word(_mplc, $"D{addr + 55}");//D6056
            motor.Motor_08_Lifter4 = new Word(_mplc, $"D{addr + 56}");//D6057
            motor.Motor_09_Rotate1 = new Word(_mplc, $"D{addr + 57}");//D6058
            motor.Motor_10_Fork1 = new Word(_mplc, $"D{addr + 58}");//D6059
            motor.Motor_11_Fork2 = new Word(_mplc, $"D{addr + 59}");//D6060

            motor.Motor_01_Travel1_ErrorCode = new Word(_mplc, $"D{addr + 69}");//D6070
            motor.Motor_02_Travel2_ErrorCode = new Word(_mplc, $"D{addr + 70}");//D6071
            motor.Motor_03_Travel3_ErrorCode = new Word(_mplc, $"D{addr + 71}");//D6072
            motor.Motor_04_Travel4_ErrorCode = new Word(_mplc, $"D{addr + 72}");//D6073
            motor.Motor_05_Lifter1_ErrorCode = new Word(_mplc, $"D{addr + 73}");//D6074
            motor.Motor_06_Lifter2_ErrorCode = new Word(_mplc, $"D{addr + 74}");//D6075
            motor.Motor_07_Lifter3_ErrorCode = new Word(_mplc, $"D{addr + 75}");//D6076
            motor.Motor_08_Lifter4_ErrorCode = new Word(_mplc, $"D{addr + 76}");//D6077
            motor.Motor_09_Rotate1_ErrorCode = new Word(_mplc, $"D{addr + 77}");//D6078
            motor.Motor_10_Fork1_ErrorCode = new Word(_mplc, $"D{addr + 78}");//D6079
            motor.Motor_11_Fork2_ErrorCode = new Word(_mplc, $"D{addr + 79}");//D6080
        }

        private void MappingCraneLeftFork(CraneSignal crane, int startAddress)
        {
            var addr = startAddress;//D6001
            crane.LeftFork = new ForkSignal();
            var fork = crane.LeftFork;

            addr = startAddress;//D6001
            fork.CSTPresence = new Bit(_mplc, $"D{addr}.5");

            fork.Rised = new Bit(_mplc, $"D{addr}.A");
            fork.Downed = new Bit(_mplc, $"D{addr}.B");

            addr = startAddress + 2;//D6003
            fork.Forking1 = new Bit(_mplc, $"D{addr}.8");
            fork.Forking2 = new Bit(_mplc, $"D{addr}.9");
            fork.Cycle1 = new Bit(_mplc, $"D{addr}.A");
            fork.Cycle2 = new Bit(_mplc, $"D{addr}.B");

            addr = startAddress + 3;//D6004
            fork.LoadPresenceSensor = new Bit(_mplc, $"D{addr}.8");
            fork.ForkHomePosition = new Bit(_mplc, $"D{addr}.9");
            fork.Forking = new Bit(_mplc, $"D{addr}.E");

            addr = startAddress + 4;//D6005
            fork.Idle = new Bit(_mplc, $"D{addr}.4");

            addr = startAddress;//D6001
            fork.CompletedCode = new Word(_mplc, $"D{addr + 29}");//D6030

            fork.ForkCounter = new Word(_mplc, $"D{addr + 36}");//D6037

            fork.CompletedCommand = new Word(_mplc, $"D{addr + 47}");//D6048
            fork.CurrentCommand = new Word(_mplc, $"D{addr + 48}");//D6049

            //D6101~D6110
            fork.TrackingCstId = new WordBlock(_mplc, $"D{addr + 100}", 10);//D6101
            //D6111~D6120
            fork.BCRResultCstId = new WordBlock(_mplc, $"D{addr + 110}", 10);//D6111
        }

        private void MappingCraneRightFork(CraneSignal crane, int startAddress)
        {
            var addr = startAddress;//D6001
            crane.RightFork = new ForkSignal();
            var fork = crane.RightFork;

            addr = startAddress;//D6001
            fork.CSTPresence = new Bit(_mplc, $"D{addr}.6");

            addr = startAddress + 2;//D6003
            fork.Rised = new Bit(_mplc, $"D{addr}.0");
            fork.Downed = new Bit(_mplc, $"D{addr}.1");

            fork.Forking1 = new Bit(_mplc, $"D{addr}.7");
            fork.Forking2 = new Bit(_mplc, $"D{addr}.C");
            fork.Cycle1 = new Bit(_mplc, $"D{addr}.D");
            fork.Cycle2 = new Bit(_mplc, $"D{addr}.E");

            addr = startAddress + 3;//D6004
            fork.LoadPresenceSensor = new Bit(_mplc, $"D{addr}.3");
            fork.ForkHomePosition = new Bit(_mplc, $"D{addr}.4");
            fork.Forking = new Bit(_mplc, $"D{addr}.B");

            addr = startAddress + 4;//D6005
            fork.Idle = new Bit(_mplc, $"D{addr}.5");

            addr = startAddress;//D6001
            fork.CompletedCode = new Word(_mplc, $"D{addr + 30}");//D6031

            fork.ForkCounter = new Word(_mplc, $"D{addr + 44}");//D6045

            fork.CompletedCommand = new Word(_mplc, $"D{addr + 45}");//D6046
            fork.CurrentCommand = new Word(_mplc, $"D{addr + 46}");//D6047

            //D6081~D6090
            fork.TrackingCstId = new WordBlock(_mplc, $"D{addr + 80}", 10);//D6081
            //D6091~D6100
            fork.BCRResultCstId = new WordBlock(_mplc, $"D{addr + 90}", 10);//D6091
        }

        #endregion Crane

        #region IO

        private void MappingIoPorts()
        {
            const int ioS2MStartAddress = 5081;
            const int ioM2SStartAddress = 6401;

            int addr = 0;
            for (int i = 0; i < MaximumIoPortNumber; i++)
            {
                var io = new IOPortSignal(i + 1);
                //D6401
                addr = ioM2SStartAddress + i * 160;
                MappingIoPortSRI(io, addr);
                MappingIoPortStatus(io, addr);
                MappingIoPortStages(io, addr);
                MappingIoPortVehicles(io, addr);

                //D5081
                addr = ioS2MStartAddress + i * 4;
                MappingIoPortController(io, addr);
                _ioPorts.Add(io.Id, io);
            }
        }

        private void MappingIoPortController(IOPortSignal io, int startAddress)
        {
            io.Controller = new IOPortControllerSignal();
            var ctrl = io.Controller;

            var addr = startAddress;//D5081
            ctrl.Word1 = new Word(_mplc, $"D{addr}");
            ctrl.ReserveToMGV = new Bit(_mplc, $"D{addr}.0");
            ctrl.FaultReset = new Bit(_mplc, $"D{addr}.1");
            ctrl.BuzzerStop = new Bit(_mplc, $"D{addr}.2");
            ctrl.Run = new Bit(_mplc, $"D{addr}.3");
            ctrl.Stop = new Bit(_mplc, $"D{addr}.4");
            ctrl.IDReadCommand = new Bit(_mplc, $"D{addr}.5");
            ctrl.DoorOpenOHS = new Bit(_mplc, $"D{addr}.6");
            ctrl.MoveBack = new Bit(_mplc, $"D{addr}.7");

            ctrl.DoorOpenMGV = new Bit(_mplc, $"D{addr}.A");
            ctrl.AreaSensorToIOPort = new Bit(_mplc, $"D{addr}.B");
            ctrl.RequestMGVMode = new Bit(_mplc, $"D{addr}.C");
            ctrl.RequestAGVMode = new Bit(_mplc, $"D{addr}.D");
            ctrl.RequestInputMode = new Bit(_mplc, $"D{addr}.E");
            ctrl.RequestOutputMode = new Bit(_mplc, $"D{addr}.F");

            addr = startAddress + 1;//D5082
            ctrl.PcErrorIndex = new Word(_mplc, $"D{addr}");

            addr = startAddress + 2;//D5083
            ctrl.Word3 = new Word(_mplc, $"D{addr}");
            ctrl.BCRDisable_P1 = new Bit(_mplc, $"D{addr}.0");

            addr = startAddress + 3;//D5084
            ctrl.Word4 = new Word(_mplc, $"D{addr}");
            io._vehicles[1].Controller.HomeReturn = new Bit(_mplc, $"D{addr}.0");
            io._vehicles[2].Controller.HomeReturn = new Bit(_mplc, $"D{addr}.1");
            io._vehicles[3].Controller.HomeReturn = new Bit(_mplc, $"D{addr}.2");
            io._vehicles[4].Controller.HomeReturn = new Bit(_mplc, $"D{addr}.3");
            io._vehicles[5].Controller.HomeReturn = new Bit(_mplc, $"D{addr}.4");
            io._vehicles[1].Controller.Run = new Bit(_mplc, $"D{addr}.5");
            io._vehicles[2].Controller.Run = new Bit(_mplc, $"D{addr}.6");
            io._vehicles[3].Controller.Run = new Bit(_mplc, $"D{addr}.7");
            io._vehicles[4].Controller.Run = new Bit(_mplc, $"D{addr}.8");
            io._vehicles[5].Controller.Run = new Bit(_mplc, $"D{addr}.9");
            io._vehicles[1].Controller.ErrorReset = new Bit(_mplc, $"D{addr}.A");
            io._vehicles[2].Controller.ErrorReset = new Bit(_mplc, $"D{addr}.B");
            io._vehicles[3].Controller.ErrorReset = new Bit(_mplc, $"D{addr}.C");
            io._vehicles[4].Controller.ErrorReset = new Bit(_mplc, $"D{addr}.D");
            io._vehicles[5].Controller.ErrorReset = new Bit(_mplc, $"D{addr}.E");
        }

        private void MappingIoPortSRI(IOPortSignal io, int startAddress)
        {
            io.SRI = new IOSRISignal();
            var sri = io.SRI;
            var addr = startAddress;//D6401
            addr = startAddress + 3;//D6404
            sri.AutoManualSwitchIsAuto = new Bit(_mplc, $"D{addr}.6");
            sri.SafetyDoorClosed = new Bit(_mplc, $"D{addr}.7");
            sri.EMO = new Bit(_mplc, $"D{addr}.8");

            sri.MainCircuitOnEnable = new Bit(_mplc, $"D{addr}.A");
        }

        private void MappingIoPortStatus(IOPortSignal io, int startAddress)
        {
            var addr = startAddress;//D6401
            io.Run = new Bit(_mplc, $"D{addr}.0");
            io.Down = new Bit(_mplc, $"D{addr}.1");
            io.Fault = new Bit(_mplc, $"D{addr}.2");
            io.InMode = new Bit(_mplc, $"D{addr}.3");
            io.OutMode = new Bit(_mplc, $"D{addr}.4");
            io.PortModeChangeable = new Bit(_mplc, $"D{addr}.5");
            io.ForkSensor = new Bit(_mplc, $"D{addr}.6");
            io.FFUError = new Bit(_mplc, $"D{addr}.7");
            io.WaitIn = new Bit(_mplc, $"D{addr}.8");
            io.WaitOut = new Bit(_mplc, $"D{addr}.9");

            io.AutoManualMode = new Bit(_mplc, $"D{addr}.B");
            io.LoadOK = new Bit(_mplc, $"D{addr}.C");
            io.UnloadOK = new Bit(_mplc, $"D{addr}.D");

            addr = startAddress + 1;// D6402
            io.BCRReadDone = new Bit(_mplc, $"D{addr}.9");

            io.CSTTransferComplete_Req = new Bit(_mplc, $"D{addr}.E");
            io.CSTRemoveCheck_Req = new Bit(_mplc, $"D{addr}.F");

            addr = startAddress + 2;// D6403
            io.PLCBatteryLow_CPU = new Bit(_mplc, $"D{addr}.0");

            io.DoorOpenLimit_MGV = new Bit(_mplc, $"D{addr}.8");
            io.GlassDetection_MGV = new Bit(_mplc, $"D{addr}.9");

            io.Ready_CraneSide = new Bit(_mplc, $"D{addr}.C");
            io.TRRequest_CraneSide = new Bit(_mplc, $"D{addr}.D");
            io.Busy_CraneSide = new Bit(_mplc, $"D{addr}.E");
            io.Complete_CraneSide = new Bit(_mplc, $"D{addr}.F");

            addr = startAddress + 3;//D6404
            io.RunEnable = new Bit(_mplc, $"D{addr}.0");

            addr = startAddress + 4;//D6405
            io.ErrorCode = new Word(_mplc, $"D{addr}");

            addr = startAddress + 5;//D6406
            io.CSTAttribute = new Word(_mplc, $"D{addr}");

            addr = startAddress + 56;//D6457
            io.BCRReadResult = new WordBlock(_mplc, $"D{addr}", 10);

            addr = startAddress + 71; //D6472
            io.CountOfTransfer = new Word(_mplc, $"D{addr}");

            addr = startAddress + 72; //D6473
            io.ErrorIndex = new Word(_mplc, $"D{addr}");

            addr = startAddress + 78; //D6479
            io.IF_Signal1 = new Word(_mplc, $"D{addr}");
            io.IF_Signal2 = new Word(_mplc, $"D{addr + 1}");
            io.IF_Signal3 = new Word(_mplc, $"D{addr + 2}");
            io.IF_Signal4 = new Word(_mplc, $"D{addr + 3}");
            io.IF_Signal5 = new Word(_mplc, $"D{addr + 4}");

            io.L_REQ = new Bit(_mplc, $"D{addr}.0");
            io.U_REQ = new Bit(_mplc, $"D{addr}.1");
            io.Ready = new Bit(_mplc, $"D{addr}.2");
            io.CARRIER = new Bit(_mplc, $"D{addr}.3");
            io.PError = new Bit(_mplc, $"D{addr}.4");
            io.Spare = new Bit(_mplc, $"D{addr}.5");
            io.POnline = new Bit(_mplc, $"D{addr}.6");
            io.PEStop = new Bit(_mplc, $"D{addr}.7");
            io.Transferring_FromSTK = new Bit(_mplc, $"D{addr}.8");
            io.TR_REQ_FromSTK = new Bit(_mplc, $"D{addr}.9");
            io.BUSY_FromSTK = new Bit(_mplc, $"D{addr}.A");
            io.COMPLETE_FromSTK = new Bit(_mplc, $"D{addr}.B");
            io.CRANE_1_FromSTK = new Bit(_mplc, $"D{addr}.C");
            io.CRANE_2_FromSTK = new Bit(_mplc, $"D{addr}.D");
            io.AError_FromSTK = new Bit(_mplc, $"D{addr}.E");
            io.ForkNumber_FromSTK = new Bit(_mplc, $"D{addr}.F");
        }

        private void MappingIoPortStages(IOPortSignal io, int startAddress)
        {
            for (int i = 0; i < 5; i++)
            {
                var stage = new IOStageSignal(i + 1);
                var addr = startAddress + 1;//D6402
                stage.LoadPresence = new Bit(_mplc, $"D{addr}.{i:X1}");
                addr = startAddress + 6;//D6407
                stage.CarrierId = new WordBlock(_mplc, $"D{(addr + i * 10)}", 10);

                io._stages.Add(stage.Id, stage);
            }
        }

        private void MappingIoPortVehicles(IOPortSignal io, int startAddress)
        {
            var addr = startAddress; //D6401
            for (int i = 0; i < 5; i++)
            {
                var vehicle = new IOVehicleSignal(i + 1);
                vehicle.Active = new Bit(_mplc, $"D{addr + 66 + i}.0");//D6467
                vehicle.LoadPresence = new Bit(_mplc, $"D{addr + 66 + i}.1");
                vehicle.Error = new Bit(_mplc, $"D{addr + 66 + i}.2");
                vehicle.HomePosition = new Bit(_mplc, $"D{addr + 66 + i}.3");
                vehicle.HPReturn = new Bit(_mplc, $"D{addr + 66 + i}.4");
                vehicle.Auto = new Bit(_mplc, $"D{addr + 66 + i}.5");

                vehicle.CurrentLocation_P1 = new Bit(_mplc, $"D{addr + 66 + i}.8");
                vehicle.CurrentLocation_P2 = new Bit(_mplc, $"D{addr + 66 + i}.9");
                vehicle.CurrentLocation_P3 = new Bit(_mplc, $"D{addr + 66 + i}.A");
                vehicle.CurrentLocation_P4 = new Bit(_mplc, $"D{addr + 66 + i}.B");
                vehicle.CurrentLocation_P5 = new Bit(_mplc, $"D{addr + 66 + i}.C");

                vehicle.RealTimePosition = new Word(_mplc, $"D{addr + 73 + i}");//D6474

                vehicle.CarrierId = new WordBlock(_mplc, $"D{addr + 83 + i * 10}", 10);//D6484

                vehicle.MileageTravel = new Word(_mplc, $"D{addr + 133 + i}");//D6534

                io._vehicles.Add(vehicle.Id, vehicle);
            }
        }

        #endregion IO

        #region EQ

        private void MappingEqPorts()
        {
            const int eqM2SStartAddres_ex = 10401;
            int addr_ex = eqM2SStartAddres_ex;
            const int eqM2SStartAddress_ex_W_CSTID = 0x2900; // W2900~W2BFF = 1~48 Port
            int addrW = eqM2SStartAddress_ex_W_CSTID;

            const int eqM2SStartAddress = 10411;
            int addr = eqM2SStartAddress;
            Stocker.EQCommon = new EQCommonSignal();
            Stocker.EQCommon.PLCBatteryLow_CPU = new Bit(_mplc, $"D{addr}.0");
            Stocker.EQCommon.PLCBatteryLow_SRAM = new Bit(_mplc, $"D{addr}.1");

            addr++; // D10412
            for (int i = 0; i < MaximunEqPortNumber; i++)
            {
                var eq = new EQPortSignal(i + 1);
                eq.L_REQ = new Bit(_mplc, $"D{(addr + i * 4)}.0");
                eq.U_REQ = new Bit(_mplc, $"D{(addr + i * 4)}.1");
                eq.Ready = new Bit(_mplc, $"D{(addr + i * 4)}.2");
                eq.Carrier = new Bit(_mplc, $"D{(addr + i * 4)}.3");
                eq.PError = new Bit(_mplc, $"D{(addr + i * 4)}.4");
                eq.Spare = new Bit(_mplc, $"D{(addr + i * 4)}.5");
                eq.POnline = new Bit(_mplc, $"D{(addr + i * 4)}.6");
                eq.PEStop = new Bit(_mplc, $"D{(addr + i * 4)}.7");
                eq.Transferring_FromSTK = new Bit(_mplc, $"D{(addr + i * 4)}.8");
                eq.TR_REQ_FromSTK = new Bit(_mplc, $"D{(addr + i * 4)}.9");
                eq.BUSY_FromSTK = new Bit(_mplc, $"D{(addr + i * 4)}.A");
                eq.COMPLETE_FromSTK = new Bit(_mplc, $"D{(addr + i * 4)}.B");
                eq.CRANE_1_FromSTK = new Bit(_mplc, $"D{(addr + i * 4)}.C");
                eq.CRANE_2_FromSTK = new Bit(_mplc, $"D{(addr + i * 4)}.D");
                eq.AError_FromSTK = new Bit(_mplc, $"D{(addr + i * 4)}.E");
                eq.ForkNumber_FromSTK = new Bit(_mplc, $"D{(addr + i * 4)}.F");

                var paddr = addr_ex + i / 16;
                var paddr_bit = i % 16;
                eq.PriorityUp = new Bit(_mplc, $"D{paddr}.{paddr_bit:X}");

                if (i < 48)
                {
                    eq.CarrierId = new WordBlock(_mplc, $"W{addrW + i * 16:X4}", 10);
                }
                else
                {
                    eq.CarrierId = new WordBlock(_mplc, $"W{addrW + 47 * 16:X4}", 10);// Only Show Port 48
                }

                _eqPorts.Add(eq.Id, eq);
            }
        }

        #endregion EQ
    }
}
