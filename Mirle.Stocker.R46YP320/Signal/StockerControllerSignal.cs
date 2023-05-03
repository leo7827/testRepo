using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class StockerControllerSignal
    {
        public WordBlock STKID { get; internal set; }
        public Bit StockerFull { get; internal set; }
        public Bit MCSOnline { get; internal set; }

        public Bit Heartbeat { get; internal set; }
        public Bit DatetimeCalibration { get; internal set; }
        public Bit CstIdSetCommand { get; internal set; }
        public Bit SpeedMode { get; internal set; }

        public Word CstIdSetCommand_Type { get; internal set; }
        public Bit CstIdSetCommand_TypeCrane1 { get; internal set; }
        public Bit CstIdSetCommand_TypeCrane2 { get; internal set; }
        public Bit CstIdSetCommand_TypePort { get; internal set; }

        public Word PortNo { get; internal set; }

        public Word LocationPosition { get; internal set; }

        public WordBlock SystemTimeCalibration { get; internal set; }

        public Bit EQSimulation_TrReq { get; internal set; }
        public Bit EQSimulation_Busy { get; internal set; }
        public Bit EQSimulation_Complete { get; internal set; }
        public Word EQSimulationSignal { get; internal set; }

        public WordBlock CSTID { get; internal set; }

        public Word ShareArea_StartBay { get; internal set; }
        public Word ShareArea_EndBay { get; internal set; }
        public Word HandOff_StartBay { get; internal set; }
        public Word HandOff_EndBay { get; internal set; }

        public Word MPLC_IF_T1 { get; internal set; }
        public Word MPLC_IF_T2 { get; internal set; }
        public Word MPLC_IF_T3 { get; internal set; }
        public Word MPLC_IF_T4 { get; internal set; }
        public Word MPLC_IF_T5 { get; internal set; }
        public Word MPLC_IF_T6 { get; internal set; }

        public Word RM1_EscapePosition_Row { get; internal set; }
        public Word RM1_EscapePosition_Bay { get; internal set; }
        public Word RM1_EscapePosition_Level { get; internal set; }
        public Word RM2_EscapePosition_Row { get; internal set; }
        public Word RM2_EscapePosition_Bay { get; internal set; }
        public Word RM2_EscapePosition_Level { get; internal set; }

    }
}