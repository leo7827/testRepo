using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class CraneControllerSignal
    {
        public Word D5021 { get; internal set; }
        public Word D5022 { get; internal set; }
        //Crane Controller
        public CraneAckSignal AckSignal { get; internal set; }

        public Bit FaultReset { get; internal set; }
        public Bit BuzzerStop { get; internal set; }
        public Bit Run { get; internal set; }
        public Bit Stop { get; internal set; }
        public Bit StepStop { get; set; }

        public Bit CommandAbort { get; internal set; }

        //Command Write Area
        public Bit CmdType_TransferWithoutIDRead { get; internal set; }

        public Bit CmdType_Transfer { get; internal set; }
        public Bit CmdType_Move { get; internal set; }
        public Bit CmdType_Scan { get; internal set; }

        public Bit HomeReturn { get; internal set; }
        public Bit UseLeftFork { get; internal set; }
        public Bit UseRightFork { get; internal set; }

        public WordBlock CommandData { get; internal set; }
        public Word TransferNo { get; internal set; }
        public DWord FromLocation { get; internal set; }
        public DWord ToLocation { get; internal set; }
        public Word BatchId { get; internal set; }
        public WordBlock CmdCstId { get; internal set; }

        public Word PcErrorIndex { get; internal set; }
        public Word StopRetryTimes { get; internal set; }

        public WordBlock AxisSpeed { get; internal set; }
        public Word TravelAxisSpeed { get; internal set; }
        public Word LifterAxisSpeed { get; internal set; }
        public Word RotateAxisSpeed { get; internal set; }
        public Word ForkAxisSpeed { get; internal set; }
        public Word NextStation { get; internal set; }
    }
}