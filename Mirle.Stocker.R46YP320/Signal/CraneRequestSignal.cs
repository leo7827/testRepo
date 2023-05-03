using Mirle.MPLC.DataType;

namespace Mirle.Stocker.R46YP320.Signal
{
    public class CraneRequestSignal
    {
       
        public Bit EmptyRetrievalReq_LF { get; internal set; }
        public Bit DoubleStorageReq_LF { get; internal set; }
        
        public Bit ScanCompleteReq { get; internal set; }
        public Bit EQInlineInterlockErrReq_LF { get; internal set; }
        public Bit EQInlineInterlockErrReq_RF { get; internal set; }
        public Bit EmptyRetrievalReq_RF { get; internal set; }
        public Bit DoubleStorageReq_RF { get; internal set; }
        public Bit TransferRequestWrongReq_LF { get; internal set; }
        public Bit TransferRequestWrongReq_RF { get; internal set; }

        public Bit IDReadErrorReq_RF { get; internal set; }
        public Bit IDMismatchReq_RF { get; internal set; }
        public Bit IDReadErrorReq_LF { get; internal set; }
        public Bit IDMismatchReq_LF { get; internal set; }

        public Bit IOInlineInterlockErrReq_LF { get; internal set; }
        public Bit IOInlineInterlockErrReq_RF { get; internal set; }
    }
}