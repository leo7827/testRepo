using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class CraneSignal
    {
        public int Id { get; }

        public CraneSignal(int id)
        {
            this.Id = id;
        }

        public CraneControllerSignal Controller { get; internal set; }

        public ForkSignal LeftFork { get; internal set; }
        public ForkSignal RightFork { get; internal set; }

        
        public CraneRequestSignal RequestSignal { get; internal set; }
        public CraneSRISignal SRI { get; internal set; }
        public CraneMotorSignal Motor { get; internal set; }

        //CraneStatus1
        public Bit InService { get; internal set; }

        public Bit Run { get; internal set; }
        public Bit Error { get; internal set; }
        public Bit Idle { get; internal set; }
        public Bit Active { get; internal set; }
        public Bit TransferCommandReceived { get; internal set; }
        public Bit HPReturn { get; internal set; }
        public Bit Escape { get; internal set; }
        public Bit Approach { get; set; }
        public Bit ForkAtBank1 { get; internal set; }
        public Bit ForkAtBank2 { get; internal set; }

        //CraneStatus2
        public Bit LocationUpdated { get; internal set; }

        //CraneStatus3

        public Bit Dual_DualCraneCommunicationErr { get; internal set; }
        public Bit SingleCraneMode { get; internal set; }
        public Bit Dual_InterferenceWaiting { get; internal set; }
        public Bit Dual_HandOffReserved { get; internal set; }
        public Bit Dual_InterventionEntry { get; internal set; }

        //CraneSensorSignals
        public Bit PLCBatteryLow_CPU { get; internal set; }

        public Bit DriverBatteryLow { get; internal set; }

        public Bit TravelHomePosition { get; internal set; }
        public Bit LifterHomePosition { get; internal set; }
        public Bit RotateHomePosition { get; internal set; }

        public Bit AnyFFUofCraneIsError { get; internal set; }

        public Bit TravelMoving { get; internal set; }
        public Bit LifterActing { get; internal set; }

        public Bit Rotating { get; internal set; }

        public Bit RunEnable { get; internal set; }
        public Bit ReadyToReceiveNewCommand { get; internal set; }

        public Bit HomeLost { get; internal set; }

        public Word Location { get; internal set; }
        public Word CurrentPosition { get; internal set; }
        public Word CSTAttribute { get; internal set; }

        public Word ErrorCode { get; internal set; }
        public Word T1 { get; internal set; }
        public Word T2 { get; internal set; }
        public Word T3 { get; internal set; }
        public Word T4 { get; internal set; }
        public Word EvacuationPositon { get; internal set; }
        public Word ErrorIndex { get; internal set; }
        public Word InterferenceRange { get; internal set; }
        public DWord MileageOfTravel { get; internal set; }
        public Word MiileageOfLifter { get; internal set; }
        public Word RotatingCounter { get; internal set; }
        public Word WrongCommandReasonCode { get; internal set; }
        public Word CurrentBay { get; internal set; }
        public Word CurrentLevel { get; internal set; }
        public Word TravelAxisSpeed { get; internal set; }
        public Word LifterAxisSpeed { get; internal set; }
        public Word RotateAxisSpeed { get; internal set; }
        public Word ForkAxisSpeed { get; internal set; }

        public Word CraneToCraneDistance { get; set; }
        public Word EscapePosition_Row;
        public Word EscapePosition_Bay;
        public Word EscapePosition_Level;
        

        public Word CommandBuffer1 { get; internal set; }
        public Word CommandBuffer2 { get; internal set; }
        public Word CommandBuffer3 { get; internal set; }
    }
}