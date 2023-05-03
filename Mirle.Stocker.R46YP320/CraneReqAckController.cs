using System;
using System.Threading.Tasks;
using Mirle.MPLC.DataType;
using Mirle.Stocker.R46YP320.Events;
using Mirle.Stocker.R46YP320.Signal;

namespace Mirle.Stocker.R46YP320
{
    public class CraneReqAckController
    {
        private readonly Crane _crane;
        private CraneRequestSignal _req;
        private CraneAckSignal _ack;

        public CraneReqAckController(Crane crane)
        {
            _crane = crane;
            _req = crane.Signal.RequestSignal;
            _ack = crane.Signal.Controller.AckSignal;
        }

        public event StockerEvents.ReqAckEventHandler OnScanCompleteRequest;

        public event StockerEvents.ReqAckEventHandler OnWrongCommandRequest;

        public event StockerEvents.ReqAckEventHandler OnEmptyRetrievalRequest;

        public event StockerEvents.ReqAckEventHandler OnDoubleStorageRequest;

        public event StockerEvents.ReqAckEventHandler OnEQInterlockErrorRequest;

        public event StockerEvents.ReqAckEventHandler OnIOInterlockErrorRequest;

        public event StockerEvents.ReqAckEventHandler OnIDReadErrorRequest;

        public event StockerEvents.ReqAckEventHandler OnIDMismatchRequest;

        private bool _lastScanCompleteReported = false;

        private bool _lastWrongCommandReported_LF = false;
        private bool _lastEmptyRetrievalReported_LF = false;
        private bool _lastDoubleStorageReported_LF = false;
        private bool _lastEQInterlockReported_LF = false;
        private bool _lastIOInterlockReported_LF = false;
        private bool _lastIDReadErrorReported_LF = false;
        private bool _lastIDMismatchReported_LF = false;

        private bool _lastWrongCommandReported_RF = false;
        private bool _lastEmptyRetrievalReported_RF = false;
        private bool _lastDoubleStorageReported_RF = false;
        private bool _lastEQInterlockReported_RF = false;
        private bool _lastIOInterlockReported_RF = false;
        private bool _lastIDReadErrorReported_RF = false;
        private bool _lastIDMismatchReported_RF = false;

        public void InitailStatus()
        {
            _lastScanCompleteReported = _req.ScanCompleteReq.IsOn() || _ack.ScanCompleteAck.IsOn();
        }

        public void RefreshStatus()
        {
            CheckReqAckStatus(_crane.Id, 0, ref _lastScanCompleteReported, OnScanCompleteRequest, _req.ScanCompleteReq, _ack.ScanCompleteAck);

            CheckReqAckStatus(_crane.Id, 1, ref _lastWrongCommandReported_LF, OnWrongCommandRequest, _req.TransferRequestWrongReq_LF, _ack.TransferRequestWrongAck_LF);
            CheckReqAckStatus(_crane.Id, 1, ref _lastEmptyRetrievalReported_LF, OnEmptyRetrievalRequest, _req.EmptyRetrievalReq_LF, _ack.EmptyRetrievalAck_LF);
            CheckReqAckStatus(_crane.Id, 1, ref _lastDoubleStorageReported_LF, OnDoubleStorageRequest, _req.DoubleStorageReq_LF, _ack.DoubleStorageAck_LF);
            CheckReqAckStatus(_crane.Id, 1, ref _lastEQInterlockReported_LF, OnEQInterlockErrorRequest, _req.EQInlineInterlockErrReq_LF, _ack.EQInlineInterlockErrAck_LF);
            CheckReqAckStatus(_crane.Id, 1, ref _lastIOInterlockReported_LF, OnIOInterlockErrorRequest, _req.IOInlineInterlockErrReq_LF, _ack.IOInlineInterlockErrAck_LF);
            CheckReqAckStatus(_crane.Id, 1, ref _lastIDReadErrorReported_LF, OnIDReadErrorRequest, _req.IDReadErrorReq_LF, _ack.IDReadErrorAck_LF);
            CheckReqAckStatus(_crane.Id, 1, ref _lastIDMismatchReported_LF, OnIDMismatchRequest, _req.IDMismatchReq_LF, _ack.IDMismatchAck_LF);

            CheckReqAckStatus(_crane.Id, 2, ref _lastWrongCommandReported_RF, OnWrongCommandRequest, _req.TransferRequestWrongReq_RF, _ack.TransferRequestWrongAck_RF);
            CheckReqAckStatus(_crane.Id, 2, ref _lastEmptyRetrievalReported_RF, OnEmptyRetrievalRequest, _req.EmptyRetrievalReq_RF, _ack.EmptyRetrievalAck_RF);
            CheckReqAckStatus(_crane.Id, 2, ref _lastDoubleStorageReported_RF, OnDoubleStorageRequest, _req.DoubleStorageReq_RF, _ack.DoubleStorageAck_RF);
            CheckReqAckStatus(_crane.Id, 2, ref _lastEQInterlockReported_RF, OnEQInterlockErrorRequest, _req.EQInlineInterlockErrReq_RF, _ack.EQInlineInterlockErrAck_RF);
            CheckReqAckStatus(_crane.Id, 2, ref _lastIOInterlockReported_RF, OnIOInterlockErrorRequest, _req.IOInlineInterlockErrReq_RF, _ack.IOInlineInterlockErrAck_RF);
            CheckReqAckStatus(_crane.Id, 2, ref _lastIDReadErrorReported_RF, OnIDReadErrorRequest, _req.IDReadErrorReq_RF, _ack.IDReadErrorAck_RF);
            CheckReqAckStatus(_crane.Id, 2, ref _lastIDMismatchReported_RF, OnIDMismatchRequest, _req.IDMismatchReq_RF, _ack.IDMismatchAck_RF);
        }

        private void CheckReqAckStatus(int craneId, int forkId, ref bool reportedFlag, StockerEvents.ReqAckEventHandler eventHandler, Bit requestBit, Bit ackBit)
        {
            if (requestBit.IsOn() != ackBit.IsOn())
            {
                if (reportedFlag == false && requestBit.IsOn())
                {
                    reportedFlag = true;
                    var args = new ReqAckEventArgs(craneId, forkId);
                    eventHandler?.Invoke(this, args);
                }

                AckSignal(requestBit, ackBit);
            }
            else if (requestBit.IsOff() && ackBit.IsOff())
            {
                reportedFlag = false;
            }
        }

        private void AckSignal(Bit req, Bit ack)
        {
            try
            {
                if (req.IsOn())
                {
                    ack.SetOn();
                }
                else
                {
                    ack.SetOff();
                }
                Task.Delay(300).Wait();
            }
            catch (Exception ex) { }
        }
    }
}