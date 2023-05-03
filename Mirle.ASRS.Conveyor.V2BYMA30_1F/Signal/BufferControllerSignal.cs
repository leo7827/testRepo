using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.Signal
{
    public class BufferControllerSignal
    {
        public Word CommandID { get; internal set; }
        public Word CommandMode { get; internal set; }

        public Word PathNotice { get; internal set; }
        public Word TransferReportNotice { get; internal set; }

        public Word BcrCheckRequestNotice { get; internal set; }
        public Word DoorNotice { get; internal set; }

        public Word RollNotice { get; internal set; }

        public Word Pickup_Word { get; internal set; }

        public Word CarrierTypeNotice { get; internal set; }

        public BufferPickupAckControllerSignal Pickup { get; internal set; }
        public BufferPickupAckControllerSignal PickupFinish_Req { get; internal set; }
        /// <summary>
        /// 手動入庫通知 (0: 自動入庫，1: 手動入庫)
        /// </summary>
        public Word Manual_Putaway { get; internal set; }

        public Word CallEleNotice { get; internal set; }
    }
}
