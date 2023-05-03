using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class CraneAckSignal
    {

        public Bit EmptyRetrievalAck_LF { get; internal set; }
        public Bit DoubleStorageAck_LF { get; internal set; }

        public Bit ScanCompleteAck { get; internal set; }
        public Bit EQInlineInterlockErrAck_LF { get; internal set; }
        public Bit EQInlineInterlockErrAck_RF { get; internal set; }
        public Bit EmptyRetrievalAck_RF { get; internal set; }
        public Bit DoubleStorageAck_RF { get; internal set; }
        public Bit TransferRequestWrongAck_LF { get; internal set; }
        public Bit TransferRequestWrongAck_RF { get; internal set; }
        public Bit IOInlineInterlockErrAck_LF { get; internal set; }
        public Bit IDReadErrorAck_RF { get; internal set; }
        public Bit IDMismatchAck_RF { get; internal set; }
        public Bit IDReadErrorAck_LF { get; internal set; }
        public Bit IDMismatchAck_LF { get; internal set; }
        public Bit IOInlineInterlockErrAck_RF { get; internal set; }
    }
}