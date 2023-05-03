using System;
using System.Drawing;
using System.Windows.Forms;

using Mirle.Extensions;
using Mirle.Stocker.R46YP320.Signal;

namespace Mirle.Stocker.R46YP320.View
{
    public partial class MonitorCrane : Form
    {
        private readonly Crane _crane;
        private readonly MPLCViewController _ctrl;
        private readonly CraneSignal _craneSignal;

        public MonitorCrane(Crane crane, MPLCViewController mplcViewController)
        {
            InitializeComponent();
            _crane = crane;
            _ctrl = mplcViewController;
            this._craneSignal = crane.Signal;
        }

        private void MonitorCrane_Load(object sender, EventArgs e)
        {
            Mapping();

            this.RefreshTimer.Enabled = true;
        }

        private void Mapping()
        {
            _ctrl.MappingControlAndSignal(lblRM1_CSTOnCrane_LF, _craneSignal.LeftFork.CSTPresence);
            _ctrl.MappingControlAndSignal(lblRM1_CSTOnCrane_RF, _craneSignal.RightFork.CSTPresence);
            _ctrl.MappingControlAndSignal(butRM1_HPReturn, _craneSignal.Controller.HomeReturn);
            _ctrl.MappingControlAndSignal(butRM1_Run, _craneSignal.Controller.Run);
            _ctrl.MappingControlAndSignal(butRM1_BuzzerStop, _craneSignal.Controller.BuzzerStop);
            _ctrl.MappingControlAndSignal(butRM1_Stop, _craneSignal.Controller.Stop);
            _ctrl.MappingControlAndSignal(butRM1_FaultReset, _craneSignal.Controller.FaultReset);
            _ctrl.MappingControlAndSignal(lblTransferNo_RM1, _craneSignal.Controller.TransferNo);
            _ctrl.MappingControlAndSignal(lblD0013_RM1, _craneSignal.Controller.FromLocation);
            _ctrl.MappingControlAndSignal(lblD0014_RM1, _craneSignal.Controller.ToLocation);
            _ctrl.MappingControlAndSignal(lblD0016_18_RM1, _craneSignal.Controller.CmdCstId);
            _ctrl.MappingControlAndSignal(lblAxisSpeed_RM1, _craneSignal.Controller.TravelAxisSpeed);
            _ctrl.MappingControlAndSignal(lblRM1_CommandBuffer1, _craneSignal.CommandBuffer1);
            _ctrl.MappingControlAndSignal(lblRM1_CommandBuffer2, _craneSignal.CommandBuffer2);
            _ctrl.MappingControlAndSignal(lblRM1_CommandBuffer3, _craneSignal.CommandBuffer3);

            _ctrl.MappingControlAndSignal(lblRM1_InService, _craneSignal.InService);
            _ctrl.MappingControlAndSignal(lblRM1_Run, _craneSignal.Run);
            _ctrl.MappingControlAndSignal(lblRM1_Idle, _craneSignal.Idle);
            _ctrl.MappingControlAndSignal(lblRM1_Active, _craneSignal.Active);
            _ctrl.MappingControlAndSignal(lblRM1_Escape, _craneSignal.Escape);
            _ctrl.MappingControlAndSignal(lblRM1_Error, _craneSignal.Error);
            _ctrl.MappingControlAndSignal(lblRM1_ForkIdle_LF, _craneSignal.LeftFork.Idle);
            _ctrl.MappingControlAndSignal(lblRM1_C1_LF, _craneSignal.LeftFork.Cycle1);
            _ctrl.MappingControlAndSignal(lblRM1_F1_LF, _craneSignal.LeftFork.Forking1);
            _ctrl.MappingControlAndSignal(lblRM1_ForkRaised_LF, _craneSignal.LeftFork.Rised);
            _ctrl.MappingControlAndSignal(lblRM1_C2_LF, _craneSignal.LeftFork.Cycle2);
            _ctrl.MappingControlAndSignal(lblRM1_F2_LF, _craneSignal.LeftFork.Forking2);
            _ctrl.MappingControlAndSignal(lblRM1_ForkDowned_LF, _craneSignal.LeftFork.Downed);
            _ctrl.MappingControlAndSignal(lblRM1_Forking_LF, _craneSignal.LeftFork.Forking);
            _ctrl.MappingControlAndSignal(lblRM1_ForkHP_LF, _craneSignal.LeftFork.ForkHomePosition);
            _ctrl.MappingControlAndSignal(lblRM1_LoadSensorON_LF, _craneSignal.LeftFork.LoadPresenceSensor);
            _ctrl.MappingControlAndSignal(lblRM1_LoadPresenceOn_LF, _craneSignal.LeftFork.CSTPresence);
            _ctrl.MappingControlAndSignal(lblRM1_CurrentTransferNo_LF, _craneSignal.LeftFork.CurrentCommand);
            _ctrl.MappingControlAndSignal(lblRM1_CompletedCode_LF, _craneSignal.LeftFork.CompletedCode);
            _ctrl.MappingControlAndSignal(lblRM1_CompletedTransferNo_LF, _craneSignal.LeftFork.CompletedCommand);

            _ctrl.MappingControlAndSignal(lblRM1_ForkIdle_RF, _craneSignal.RightFork.Idle);
            _ctrl.MappingControlAndSignal(lblRM1_C1_RF, _craneSignal.RightFork.Cycle1);
            _ctrl.MappingControlAndSignal(lblRM1_F1_RF, _craneSignal.RightFork.Forking1);
            _ctrl.MappingControlAndSignal(lblRM1_ForkRaised_RF, _craneSignal.RightFork.Rised);
            _ctrl.MappingControlAndSignal(lblRM1_C2_RF, _craneSignal.RightFork.Cycle2);
            _ctrl.MappingControlAndSignal(lblRM1_F2_RF, _craneSignal.RightFork.Forking2);
            _ctrl.MappingControlAndSignal(lblRM1_ForkDowned_RF, _craneSignal.RightFork.Downed);
            _ctrl.MappingControlAndSignal(lblRM1_Forking_RF, _craneSignal.RightFork.Forking);
            _ctrl.MappingControlAndSignal(lblRM1_ForkHP_RF, _craneSignal.RightFork.ForkHomePosition);
            _ctrl.MappingControlAndSignal(lblRM1_LoadSensorON_RF, _craneSignal.RightFork.LoadPresenceSensor);
            _ctrl.MappingControlAndSignal(lblRM1_LoadPresenceOn_RF, _craneSignal.RightFork.CSTPresence);
            _ctrl.MappingControlAndSignal(lblRM1_CurrentTransferNo_RF, _craneSignal.RightFork.CurrentCommand);
            _ctrl.MappingControlAndSignal(lblRM1_CompletedCode_RF, _craneSignal.RightFork.CompletedCode);
            _ctrl.MappingControlAndSignal(lblRM1_CompletedTransferNo_RF, _craneSignal.RightFork.CompletedCommand);

            _ctrl.MappingControlAndSignal(lblRM1_RunEnable, _craneSignal.RunEnable);
            _ctrl.MappingControlAndSignal(lblRM1_ReadyReceiveNewCommand, _craneSignal.ReadyToReceiveNewCommand);
            _ctrl.MappingControlAndSignal(lblRM1_HPReturn, _craneSignal.HPReturn);
            _ctrl.MappingControlAndSignal(lblRM1_TransferCommandReceived, _craneSignal.TransferCommandReceived);
            _ctrl.MappingControlAndSignal(lblRM1_ForkatBank1, _craneSignal.ForkAtBank1);
            _ctrl.MappingControlAndSignal(lblRM1_ForkatBank2, _craneSignal.ForkAtBank2);
            _ctrl.MappingControlAndSignal(lblRM1_Rotating, _craneSignal.Rotating);
            _ctrl.MappingControlAndSignal(lblRM1_RotateHP, _craneSignal.RotateHomePosition);
            _ctrl.MappingControlAndSignal(lblRM1_Liftering, _craneSignal.LifterActing);
            _ctrl.MappingControlAndSignal(lblRM1_LifterHP, _craneSignal.LifterHomePosition);
            _ctrl.MappingControlAndSignal(lblRM1_Traceling, _craneSignal.TravelMoving);
            _ctrl.MappingControlAndSignal(lblRM1_TracelHP, _craneSignal.TravelHomePosition);
            _ctrl.MappingControlAndSignal(lblRM1_InterferenceWaiting, _craneSignal.Dual_InterferenceWaiting);
            _ctrl.MappingControlAndSignal(lblRM1_InterventionEntry, _craneSignal.Dual_InterventionEntry);
            _ctrl.MappingControlAndSignal(lblRM1_HandoffReserved, _craneSignal.Dual_HandOffReserved);
            _ctrl.MappingControlAndSignal(lblRM1_CraneLocationUpdated, _craneSignal.LocationUpdated);
            _ctrl.MappingControlAndSignal(lblRM1_DualCraneComuErr, _craneSignal.Dual_DualCraneCommunicationErr);
            _ctrl.MappingControlAndSignal(lblRM1_SingleCraneMode, _craneSignal.SingleCraneMode);
            _ctrl.MappingControlAndSignal(lblRM1FBCRReadReqOn, _craneSignal.RequestSignal.ScanCompleteReq);
            _ctrl.MappingControlAndSignal(lblRM1_CSTIDBCRResult_LF, _craneSignal.LeftFork.BCRResultCstId);
            _ctrl.MappingControlAndSignal(lblRM1_CSTIDBCRResult_RF, _craneSignal.RightFork.BCRResultCstId);
            _ctrl.MappingControlAndSignal(lblRM1_CSTIDTracking_LF, _craneSignal.LeftFork.TrackingCstId);
            _ctrl.MappingControlAndSignal(lblRM1_CSTIDTracking_RF, _craneSignal.RightFork.TrackingCstId);
            _ctrl.MappingControlAndSignal(lblRM1_CurrentPosition, _craneSignal.CurrentPosition);
            _ctrl.MappingControlAndSignal(lblRM1_CurrentBayLevel, _craneSignal.CurrentBay);
            _ctrl.MappingControlAndSignal(lblRM1_RMLocation, _craneSignal.Location);
            _ctrl.MappingControlAndSignal(lblRM1_EvacuationPosition, _craneSignal.EvacuationPositon);
            _ctrl.MappingControlAndSignal(lblRM1_InterferenceRange, _craneSignal.InterferenceRange);
            _ctrl.MappingControlAndSignal(lblRM1_T1, _craneSignal.T1);
            _ctrl.MappingControlAndSignal(lblRM1_T2, _craneSignal.T2);
            _ctrl.MappingControlAndSignal(lblRM1_T3, _craneSignal.T3);
            _ctrl.MappingControlAndSignal(lblRM1_T4, _craneSignal.T4);

            _ctrl.MappingControlAndSignal(lblRM1_ErrorIndex1_PLC, _craneSignal.ErrorIndex);
            _ctrl.MappingControlAndSignal(lblRM1_ErrorIndex1_PC, _craneSignal.Controller.PcErrorIndex);
            _ctrl.MappingControlAndSignal(lblRM1_ErrorCode, _craneSignal.ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_RotatingCounter, _craneSignal.RotatingCounter);
            _ctrl.MappingControlAndSignal(lblRM1_ForkCounter_LF, _craneSignal.LeftFork.ForkCounter);
            _ctrl.MappingControlAndSignal(lblRM1_ForkCounter_RF, _craneSignal.RightFork.ForkCounter);
            _ctrl.MappingControlAndSignal(lblRM1_MileageOfTravel, _craneSignal.MileageOfTravel);
            _ctrl.MappingControlAndSignal(lblRM1_MileageOfLifter, _craneSignal.MiileageOfLifter);
            _ctrl.MappingControlAndSignal(lblRM1_WrongCommandReasonCode, _craneSignal.WrongCommandReasonCode);
            _ctrl.MappingControlAndSignal(lblRM1_TravelAxisSpeed, _craneSignal.TravelAxisSpeed);
            _ctrl.MappingControlAndSignal(lblRM1_LifterAxisSpeed, _craneSignal.LifterAxisSpeed);
            _ctrl.MappingControlAndSignal(lblRM1_RotateAxisSpeed, _craneSignal.RotateAxisSpeed);
            _ctrl.MappingControlAndSignal(lblRM1_ForkAxisSpeed, _craneSignal.ForkAxisSpeed);
            _ctrl.MappingControlAndSignal(lblRM1_PLCCPUBatteryLow, _craneSignal.PLCBatteryLow_CPU);
            _ctrl.MappingControlAndSignal(lblRM1_DriverBatteryLow, _craneSignal.DriverBatteryLow);
            _ctrl.MappingControlAndSignal(lblRM1_AnyOneFFUisErr, _craneSignal.AnyFFUofCraneIsError);
            _ctrl.MappingControlAndSignal(lblRM1_SRI_MainCircuitOnEnable, _craneSignal.SRI.MainCircuitOnEnable);
            _ctrl.MappingControlAndSignal(lblRM1_SRI_EMO, _craneSignal.SRI.EMO);
            _ctrl.MappingControlAndSignal(lblRM1_SRI_NoError, _craneSignal.SRI.NoError);
            _ctrl.MappingControlAndSignal(lblRM1_SRI_RM1HIDPowerOn, _craneSignal.SRI.HIDPowerOn);

            _ctrl.MappingControlAndSignal(lblRM1EmptyRetrieval_Ack_LF, _craneSignal.Controller.AckSignal.EmptyRetrievalAck_LF);
            _ctrl.MappingControlAndSignal(lblRM1DoubleStorage_Ack_LF, _craneSignal.Controller.AckSignal.DoubleStorageAck_LF);
            _ctrl.MappingControlAndSignal(lblRM1EQInterlockErr_Ack_LF, _craneSignal.Controller.AckSignal.EQInlineInterlockErrAck_LF);
            _ctrl.MappingControlAndSignal(lblRM1IOInterlockErr_Ack_LF, _craneSignal.Controller.AckSignal.IOInlineInterlockErrAck_LF);
            _ctrl.MappingControlAndSignal(lblRM1IDMismatch_Ack_LF, _craneSignal.Controller.AckSignal.IDMismatchAck_LF);
            _ctrl.MappingControlAndSignal(lblRM1IDReadError_Ack_LF, _craneSignal.Controller.AckSignal.IDReadErrorAck_LF);
            _ctrl.MappingControlAndSignal(lblRM1EmptyRetrieval_Ack_RF, _craneSignal.Controller.AckSignal.EmptyRetrievalAck_RF);
            _ctrl.MappingControlAndSignal(lblRM1DoubleStorage_Ack_RF, _craneSignal.Controller.AckSignal.DoubleStorageAck_RF);
            _ctrl.MappingControlAndSignal(lblRM1EQInterlockErr_Ack_RF, _craneSignal.Controller.AckSignal.EQInlineInterlockErrAck_RF);
            _ctrl.MappingControlAndSignal(lblRM1IOInterlockErr_Ack_RF, _craneSignal.Controller.AckSignal.IOInlineInterlockErrAck_RF);
            _ctrl.MappingControlAndSignal(lblRM1IDMismatch_Ack_RF, _craneSignal.Controller.AckSignal.IDMismatchAck_RF);
            _ctrl.MappingControlAndSignal(lblRM1IDReadError_Ack_RF, _craneSignal.Controller.AckSignal.IDReadErrorAck_RF);
            _ctrl.MappingControlAndSignal(lblRM1ScanComplete_Ack, _craneSignal.Controller.AckSignal.ScanCompleteAck);
            _ctrl.MappingControlAndSignal(lblRM1TransferRequestWrong_Ack, _craneSignal.Controller.AckSignal.TransferRequestWrongAck_LF);
            _ctrl.MappingControlAndSignal(lblRM1EmptyRetrieval_Req_LF, _craneSignal.RequestSignal.EmptyRetrievalReq_LF);
            _ctrl.MappingControlAndSignal(lblRM1DoubleStorage_Req_LF, _craneSignal.RequestSignal.DoubleStorageReq_LF);
            _ctrl.MappingControlAndSignal(lblRM1EQInterlockErr_Req_LF, _craneSignal.RequestSignal.EQInlineInterlockErrReq_LF);
            _ctrl.MappingControlAndSignal(lblRM1IOInterlockErr_Req_LF, _craneSignal.RequestSignal.IOInlineInterlockErrReq_LF);
            _ctrl.MappingControlAndSignal(lblRM1IDMismatch_Req_LF, _craneSignal.RequestSignal.IDMismatchReq_LF);
            _ctrl.MappingControlAndSignal(lblRM1IDReadError_Req_LF, _craneSignal.RequestSignal.IDReadErrorReq_LF);
            _ctrl.MappingControlAndSignal(lblRM1EmptyRetrieval_Req_RF, _craneSignal.RequestSignal.EmptyRetrievalReq_RF);
            _ctrl.MappingControlAndSignal(lblRM1DoubleStorage_Req_RF, _craneSignal.RequestSignal.DoubleStorageReq_RF);
            _ctrl.MappingControlAndSignal(lblRM1EQInterlockErr_Req_RF, _craneSignal.RequestSignal.EQInlineInterlockErrReq_RF);
            _ctrl.MappingControlAndSignal(lblRM1IOInterlockErr_Req_RF, _craneSignal.RequestSignal.IOInlineInterlockErrReq_RF);
            _ctrl.MappingControlAndSignal(lblRM1IDMismatch_Req_RF, _craneSignal.RequestSignal.IDMismatchReq_RF);
            _ctrl.MappingControlAndSignal(lblRM1IDReadError_Req_RF, _craneSignal.RequestSignal.IDReadErrorReq_RF);
            _ctrl.MappingControlAndSignal(lblRM1ScanComplete_Req, _craneSignal.RequestSignal.ScanCompleteReq);
            _ctrl.MappingControlAndSignal(lblRM1TransferRequestWrong_Req, _craneSignal.RequestSignal.TransferRequestWrongReq_LF);

            _ctrl.MappingControlAndSignal(lblRM1_Motor01, _craneSignal.Motor.Motor_01_Travel1);
            _ctrl.MappingControlAndSignal(lblRM1_Motor02, _craneSignal.Motor.Motor_02_Travel2);
            _ctrl.MappingControlAndSignal(lblRM1_Motor03, _craneSignal.Motor.Motor_03_Travel3);
            _ctrl.MappingControlAndSignal(lblRM1_Motor04, _craneSignal.Motor.Motor_04_Travel4);
            _ctrl.MappingControlAndSignal(lblRM1_Motor05, _craneSignal.Motor.Motor_05_Lifter1);
            _ctrl.MappingControlAndSignal(lblRM1_Motor06, _craneSignal.Motor.Motor_06_Lifter2);
            _ctrl.MappingControlAndSignal(lblRM1_Motor07, _craneSignal.Motor.Motor_07_Lifter3);
            _ctrl.MappingControlAndSignal(lblRM1_Motor08, _craneSignal.Motor.Motor_08_Lifter4);
            _ctrl.MappingControlAndSignal(lblRM1_Motor09, _craneSignal.Motor.Motor_09_Rotate1);
            _ctrl.MappingControlAndSignal(lblRM1_Motor10, _craneSignal.Motor.Motor_10_Fork1);
            _ctrl.MappingControlAndSignal(lblRM1_Motor11, _craneSignal.Motor.Motor_11_Fork2);
            _ctrl.MappingControlAndSignal(lblRM1_Motor01_ErrorCode, _craneSignal.Motor.Motor_01_Travel1_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor02_ErrorCode, _craneSignal.Motor.Motor_02_Travel2_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor03_ErrorCode, _craneSignal.Motor.Motor_03_Travel3_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor04_ErrorCode, _craneSignal.Motor.Motor_04_Travel4_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor05_ErrorCode, _craneSignal.Motor.Motor_05_Lifter1_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor06_ErrorCode, _craneSignal.Motor.Motor_06_Lifter2_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor07_ErrorCode, _craneSignal.Motor.Motor_07_Lifter3_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor08_ErrorCode, _craneSignal.Motor.Motor_08_Lifter4_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor09_ErrorCode, _craneSignal.Motor.Motor_09_Rotate1_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor10_ErrorCode, _craneSignal.Motor.Motor_10_Fork1_ErrorCode);
            _ctrl.MappingControlAndSignal(lblRM1_Motor11_ErrorCode, _craneSignal.Motor.Motor_11_Fork2_ErrorCode);
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            ResfreshCraneControllerPanel();
            ResfreshState1Panel();
            ResfreshState2Panel();
            ResfreshReqAckPanel();
            ResfreshMotorPanel();
        }

        private void ResfreshCraneControllerPanel()
        {
            if (_craneSignal.HPReturn.IsOn())
            {
                lblRM1Sts.Text = "HOMEACTION";
                lblRM1Sts.BackColor = Color.SkyBlue;
            }
            else
            {
                switch (_crane.Status)
                {
                    case StockerEnums.CraneStatus.BUSY:
                        lblRM1Sts.Text = "BUSY";
                        lblRM1Sts.BackColor = Color.Yellow;
                        break;

                    case StockerEnums.CraneStatus.HOMEACTION:
                        lblRM1Sts.Text = "HOMEACTION";
                        lblRM1Sts.BackColor = Color.SkyBlue;
                        break;

                    case StockerEnums.CraneStatus.IDLE:
                        lblRM1Sts.Text = "IDLE";
                        lblRM1Sts.BackColor = Color.Lime;
                        break;

                    case StockerEnums.CraneStatus.MAINTAIN:
                        lblRM1Sts.Text = "MAINTEN";
                        lblRM1Sts.BackColor = Color.Orange;
                        break;

                    case StockerEnums.CraneStatus.STOP:
                        lblRM1Sts.Text = "STOP";
                        lblRM1Sts.BackColor = Color.Red;
                        break;

                    case StockerEnums.CraneStatus.WAITINGHOMEACTION:
                        lblRM1Sts.Text = "WAITHOME";
                        lblRM1Sts.BackColor = Color.Fuchsia;
                        break;

                    //V1.6.0.14-2
                    case StockerEnums.CraneStatus.ESCAPE:
                        lblRM1Sts.Text = "ESCAPE";
                        lblRM1Sts.BackColor = Color.DeepSkyBlue;
                        break;

                    //V3.12.1607.16-3
                    case StockerEnums.CraneStatus.Waiting:
                        lblRM1Sts.Text = "BUSY";
                        lblRM1Sts.BackColor = Color.DarkSeaGreen;
                        break;

                    default:
                        lblRM1Sts.Text = "None";
                        lblRM1Sts.BackColor = Color.Red;
                        break;
                }
            }

            lblRM1_CSTOnCrane_LF.BackColor = _craneSignal.LeftFork.CSTPresence.IsOn() ? Color.Lime : Color.Gainsboro;
            lblRM1_CSTOnCrane_RF.BackColor = _craneSignal.RightFork.CSTPresence.IsOn() ? Color.Lime : Color.Gainsboro;
            butRM1_HPReturn.BackColor = _craneSignal.Controller.HomeReturn.IsOn() ? Color.Lime : Color.Gainsboro;
            butRM1_Run.BackColor = _craneSignal.Controller.Run.IsOn() ? Color.Lime : Color.Gainsboro;
            butRM1_BuzzerStop.BackColor = _craneSignal.Controller.BuzzerStop.IsOn() ? Color.Lime : Color.Gainsboro;
            butRM1_Stop.BackColor = _craneSignal.Controller.Stop.IsOn() ? Color.Lime : Color.Gainsboro;
            butRM1_FaultReset.BackColor = _craneSignal.Controller.FaultReset.IsOn() ? Color.Lime : Color.Gainsboro;

            lblTransferNo_RM1.Text = _craneSignal.Controller.TransferNo.GetValue().ToString("D5");

            if (_craneSignal.Controller.CmdType_TransferWithoutIDRead.IsOn())
            {
                lblD0012_RM1.Text = "Transfder";
            }
            else if (_craneSignal.Controller.CmdType_Transfer.IsOn())
            {
                lblD0012_RM1.Text = "TransfderScan";
            }
            else if (_craneSignal.Controller.CmdType_Move.IsOn())
            {
                lblD0012_RM1.Text = "Move";
            }
            else if (_craneSignal.Controller.CmdType_Scan.IsOn())
            {
                lblD0012_RM1.Text = "Scan";
            }
            else
            {
                lblD0012_RM1.Text = "0";
            }

            lblD0013_RM1.Text = _craneSignal.Controller.FromLocation.GetValue().ToString("D5");
            lblD0014_RM1.Text = _craneSignal.Controller.ToLocation.GetValue().ToString("D5");
            lblD0016_18_RM1.Text = _craneSignal.Controller.CmdCstId.GetData().ToASCII();
            lblAxisSpeed_RM1.Text = _craneSignal.Controller.TravelAxisSpeed.GetValue() + "-" +
                                    _craneSignal.Controller.LifterAxisSpeed.GetValue() + "-" +
                                    _craneSignal.Controller.RotateAxisSpeed.GetValue() + "-" +
                                    _craneSignal.Controller.ForkAxisSpeed.GetValue();
            lblRM1_CommandBuffer1.Text = _craneSignal.CommandBuffer1.GetValue().ToString("D5");
            lblRM1_CommandBuffer2.Text = _craneSignal.CommandBuffer2.GetValue().ToString("D5");
            lblRM1_CommandBuffer3.Text = _craneSignal.CommandBuffer3.GetValue().ToString("D5");
        }

        private void ResfreshState1Panel()
        {
            lblRM1_InService.BackColor = _craneSignal.InService.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Run.BackColor = _craneSignal.Run.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Idle.BackColor = _craneSignal.Idle.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Active.BackColor = _craneSignal.Active.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Escape.BackColor = _craneSignal.Escape.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Error.BackColor = _craneSignal.Error.IsOn() ? Color.Yellow : Color.White;

            lblRM1_ForkIdle_LF.BackColor = _craneSignal.LeftFork.Idle.IsOn() ? Color.Yellow : Color.White;
            lblRM1_C1_LF.BackColor = _craneSignal.LeftFork.Cycle1.IsOn() ? Color.Yellow : Color.White;
            lblRM1_F1_LF.BackColor = _craneSignal.LeftFork.Forking1.IsOn() ? Color.Yellow : Color.White;
            lblRM1_ForkRaised_LF.BackColor = _craneSignal.LeftFork.Rised.IsOn() ? Color.Yellow : Color.White;
            lblRM1_C2_LF.BackColor = _craneSignal.LeftFork.Cycle2.IsOn() ? Color.Yellow : Color.White;
            lblRM1_F2_LF.BackColor = _craneSignal.LeftFork.Forking2.IsOn() ? Color.Yellow : Color.White;
            lblRM1_ForkDowned_LF.BackColor = _craneSignal.LeftFork.Downed.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Forking_LF.BackColor = _craneSignal.LeftFork.Forking.IsOn() ? Color.Yellow : Color.White;
            lblRM1_ForkHP_LF.BackColor = _craneSignal.LeftFork.ForkHomePosition.IsOn() ? Color.Yellow : Color.White;
            lblRM1_LoadSensorON_LF.BackColor = _craneSignal.LeftFork.LoadPresenceSensor.IsOn() ? Color.Yellow : Color.White;
            lblRM1_LoadPresenceOn_LF.BackColor = _craneSignal.LeftFork.CSTPresence.IsOn() ? Color.YellowGreen : Color.White;

            lblRM1_CurrentTransferNo_LF.Text = _craneSignal.LeftFork.CurrentCommand.GetValue().ToString("D5");
            lblRM1_CompletedCode_LF.Text = _craneSignal.LeftFork.CompletedCode.GetValue().ToString("X");
            lblRM1_CompletedTransferNo_LF.Text = _craneSignal.LeftFork.CompletedCommand.GetValue().ToString("D5");

            lblRM1_ForkIdle_RF.BackColor = _craneSignal.RightFork.Idle.IsOn() ? Color.Yellow : Color.White;
            lblRM1_C1_RF.BackColor = _craneSignal.RightFork.Cycle1.IsOn() ? Color.Yellow : Color.White;
            lblRM1_F1_RF.BackColor = _craneSignal.RightFork.Forking1.IsOn() ? Color.Yellow : Color.White;
            lblRM1_ForkRaised_RF.BackColor = _craneSignal.RightFork.Rised.IsOn() ? Color.Yellow : Color.White;
            lblRM1_C2_RF.BackColor = _craneSignal.RightFork.Cycle2.IsOn() ? Color.Yellow : Color.White;
            lblRM1_F2_RF.BackColor = _craneSignal.RightFork.Forking2.IsOn() ? Color.Yellow : Color.White;
            lblRM1_ForkDowned_RF.BackColor = _craneSignal.RightFork.Downed.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Forking_RF.BackColor = _craneSignal.RightFork.Forking.IsOn() ? Color.Yellow : Color.White;
            lblRM1_ForkHP_RF.BackColor = _craneSignal.RightFork.ForkHomePosition.IsOn() ? Color.Yellow : Color.White;
            lblRM1_LoadSensorON_RF.BackColor = _craneSignal.RightFork.LoadPresenceSensor.IsOn() ? Color.Yellow : Color.White;
            lblRM1_LoadPresenceOn_RF.BackColor = _craneSignal.RightFork.CSTPresence.IsOn() ? Color.YellowGreen : Color.White;

            lblRM1_CurrentTransferNo_RF.Text = _craneSignal.RightFork.CurrentCommand.GetValue().ToString("D5");
            lblRM1_CompletedCode_RF.Text = _craneSignal.RightFork.CompletedCode.GetValue().ToString("X");
            lblRM1_CompletedTransferNo_RF.Text = _craneSignal.RightFork.CompletedCommand.GetValue().ToString("D5");

            lblRM1_RunEnable.BackColor = _craneSignal.RunEnable.IsOn() ? Color.Yellow : Color.White;
            lblRM1_ReadyReceiveNewCommand.BackColor = _craneSignal.ReadyToReceiveNewCommand.IsOn() ? Color.Yellow : Color.White;
            lblRM1_HPReturn.BackColor = _craneSignal.HPReturn.IsOn() ? Color.Yellow : Color.White;
            lblRM1_TransferCommandReceived.BackColor = _craneSignal.TransferCommandReceived.IsOn() ? Color.Yellow : Color.White;
            //lblRM1_Retry.BackColor = _craneSignal.Retry.IsOn() ? Color.Yellow : Color.White;
            lblRM1_ForkatBank1.BackColor = _craneSignal.ForkAtBank1.IsOn() ? Color.Yellow : Color.White;
            lblRM1_ForkatBank2.BackColor = _craneSignal.ForkAtBank2.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Rotating.BackColor = _craneSignal.Rotating.IsOn() ? Color.Yellow : Color.White;
            lblRM1_RotateHP.BackColor = _craneSignal.RotateHomePosition.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Liftering.BackColor = _craneSignal.LifterActing.IsOn() ? Color.Yellow : Color.White;
            lblRM1_LifterHP.BackColor = _craneSignal.LifterHomePosition.IsOn() ? Color.Yellow : Color.White;
            lblRM1_Traceling.BackColor = _craneSignal.TravelMoving.IsOn() ? Color.Yellow : Color.White;
            lblRM1_TracelHP.BackColor = _craneSignal.TravelHomePosition.IsOn() ? Color.Yellow : Color.White;
            lblRM1_InterferenceWaiting.BackColor = _craneSignal.Dual_InterferenceWaiting.IsOn() ? Color.Yellow : Color.White;
            lblRM1_InterventionEntry.BackColor = _craneSignal.Dual_InterventionEntry.IsOn() ? Color.Yellow : Color.White;
            lblRM1_HandoffReserved.BackColor = _craneSignal.Dual_HandOffReserved.IsOn() ? Color.Yellow : Color.White;
            //lblRM1_TransferPositionCorrection.BackColor = _craneSignal.TransferPositionCorrection.IsOn() ? Color.Yellow : Color.White;
            lblRM1_CraneLocationUpdated.BackColor = _craneSignal.LocationUpdated.IsOn() ? Color.Yellow : Color.White;
            lblRM1_DualCraneComuErr.BackColor = _craneSignal.Dual_DualCraneCommunicationErr.IsOn() ? Color.Yellow : Color.White;
            //lblRM1_InterferenceZonePermit.BackColor = _craneSignal.Dual_InterferenceZonePermit.IsOn() ? Color.Yellow : Color.White;
            //lblRM1_InterferenceZoneRequest.BackColor = _craneSignal.Dual_InterferenceZoneRequest.IsOn() ? Color.Yellow : Color.White;
            lblRM1_SingleCraneMode.BackColor = _craneSignal.SingleCraneMode.IsOn() ? Color.Yellow : Color.White;

            lblRM1FBCRReadReqOn.BackColor = _craneSignal.RequestSignal.ScanCompleteReq.IsOn() ? Color.Lime : Color.White;
            lblRM1_CSTIDBCRResult_LF.Text = _craneSignal.LeftFork.BCRResultCstId.GetData().ToASCII();
            lblRM1_CSTIDBCRResult_RF.Text = _craneSignal.RightFork.BCRResultCstId.GetData().ToASCII();
            lblRM1_CSTIDTracking_LF.Text = _craneSignal.LeftFork.TrackingCstId.GetData().ToASCII();
            lblRM1_CSTIDTracking_RF.Text = _craneSignal.RightFork.TrackingCstId.GetData().ToASCII();
            lblRM1_CurrentPosition.Text = _craneSignal.CurrentPosition.GetValue().ToString();
            lblRM1_CurrentBayLevel.Text = _craneSignal.CurrentBay.GetValue() + "-" + _craneSignal.CurrentLevel.GetValue();
            lblRM1_RMLocation.Text = _craneSignal.Location.GetValue().ToString("D5");
            //lblRM1_CurrentSpeed.Text = _craneSignal.CurrentSpeed.GetValue().ToString();
            lblRM1_EvacuationPosition.Text = _craneSignal.EvacuationPositon.GetValue().ToString();
            lblRM1_InterferenceRange.Text = _craneSignal.InterferenceRange.GetValue().ToString();

            lblRM1_T1.Text = System.Math.Round((Convert.ToDouble(_craneSignal.T1.GetValue())) / 10, 1).ToString();
            lblRM1_T2.Text = System.Math.Round((Convert.ToDouble(_craneSignal.T2.GetValue())) / 10, 1).ToString();
            lblRM1_T3.Text = System.Math.Round((Convert.ToDouble(_craneSignal.T3.GetValue())) / 10, 1).ToString();
            lblRM1_T4.Text = System.Math.Round((Convert.ToDouble(_craneSignal.T4.GetValue())) / 10, 1).ToString();
        }

        private void ResfreshState2Panel()
        {
            lblRM1_ErrorIndex1_PLC.Text = _craneSignal.ErrorIndex.GetValue().ToString();
            lblRM1_ErrorIndex1_PC.Text = _craneSignal.Controller.PcErrorIndex.GetValue().ToString();
            lblRM1_ErrorCode.Text = _craneSignal.ErrorCode.GetValue().ToString("X4");
            lblRM1_RotatingCounter.Text = _craneSignal.RotatingCounter.GetValue().ToString();
            lblRM1_ForkCounter_LF.Text = _craneSignal.LeftFork.ForkCounter.GetValue().ToString();
            lblRM1_ForkCounter_RF.Text = _craneSignal.RightFork.ForkCounter.GetValue().ToString();
            lblRM1_MileageOfTravel.Text = _craneSignal.MileageOfTravel.GetValue().ToString();
            lblRM1_MileageOfLifter.Text = _craneSignal.MiileageOfLifter.GetValue().ToString();
            lblRM1_WrongCommandReasonCode.Text = _craneSignal.WrongCommandReasonCode.GetValue().ToString();
            lblRM1_TravelAxisSpeed.Text = _craneSignal.TravelAxisSpeed.GetValue().ToString();
            lblRM1_LifterAxisSpeed.Text = _craneSignal.LifterAxisSpeed.GetValue().ToString();
            lblRM1_RotateAxisSpeed.Text = _craneSignal.RotateAxisSpeed.GetValue().ToString();
            lblRM1_ForkAxisSpeed.Text = _craneSignal.ForkAxisSpeed.GetValue().ToString();

            lblRM1_PLCCPUBatteryLow.BackColor = _craneSignal.PLCBatteryLow_CPU.IsOn() ? Color.Yellow : Color.White;
            lblRM1_DriverBatteryLow.BackColor = _craneSignal.DriverBatteryLow.IsOn() ? Color.Yellow : Color.White;
            //lblRM1_PLCSRAMBatteryLow.BackColor = _craneSignal.PLCBatteryLow_SRAM.IsOn() ? Color.Yellow : Color.White;
            lblRM1_AnyOneFFUisErr.BackColor = _craneSignal.AnyFFUofCraneIsError.IsOn() ? Color.Yellow : Color.White;

            //lblRM1_SRI_AMSwitchofMPLCisAuto_HP.BackColor = _craneSignal.SRI.TheAMSwitchIsAuto_MPLC_HP.IsOn() ? Color.Yellow : Color.White;
            //lblRM1_SRI_AMSwitchofRMPLCisAuto_HP.BackColor = _craneSignal.SRI.TheAMSwitchIsAuto_RMPLC_HP.IsOn() ? Color.Yellow : Color.White;
            //lblRM1_SRI_AMSwitchofMPLCisAuto_OP.BackColor = _craneSignal.SRI.TheAMSwitchIsAuto_MPLC_OP.IsOn() ? Color.Yellow : Color.White;
            //lblRM1_SRI_AMSwitchofRMPLCisAuto_OP.BackColor = _craneSignal.SRI.TheAMSwitchIsAuto_RMPLC_OP.IsOn() ? Color.Yellow : Color.White;

            //lblRM1_SRI_SafetyDoorClosed.BackColor = _craneSignal.SRI.SafetyDoorClosed.IsOn() ? Color.Yellow : Color.White;
            lblRM1_SRI_MainCircuitOnEnable.BackColor = _craneSignal.SRI.MainCircuitOnEnable.IsOn() ? Color.Yellow : Color.White;
            lblRM1_SRI_EMO.BackColor = _craneSignal.SRI.EMO.IsOn() ? Color.Yellow : Color.White;
            lblRM1_SRI_NoError.BackColor = _craneSignal.SRI.NoError.IsOn() ? Color.Yellow : Color.White;

            lblRM1_SRI_RM1HIDPowerOn.BackColor = _craneSignal.SRI.HIDPowerOn.IsOn() ? Color.Yellow : Color.White;
            //lblRM1_SRI_RM2HIDPowerOn.BackColor = _craneSignal.SRI.HIDPowerOn_Crane2.IsOn() ? Color.Yellow : Color.White;
        }

        private void ResfreshReqAckPanel()
        {
            lblRM1EmptyRetrieval_Ack_LF.BackColor = _craneSignal.Controller.AckSignal.EmptyRetrievalAck_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1DoubleStorage_Ack_LF.BackColor = _craneSignal.Controller.AckSignal.DoubleStorageAck_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1EQInterlockErr_Ack_LF.BackColor = _craneSignal.Controller.AckSignal.EQInlineInterlockErrAck_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IOInterlockErr_Ack_LF.BackColor = _craneSignal.Controller.AckSignal.IOInlineInterlockErrAck_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IDMismatch_Ack_LF.BackColor = _craneSignal.Controller.AckSignal.IDMismatchAck_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IDReadError_Ack_LF.BackColor = _craneSignal.Controller.AckSignal.IDReadErrorAck_LF.IsOn() ? Color.Yellow : Color.White;

            lblRM1EmptyRetrieval_Ack_RF.BackColor = _craneSignal.Controller.AckSignal.EmptyRetrievalAck_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1DoubleStorage_Ack_RF.BackColor = _craneSignal.Controller.AckSignal.DoubleStorageAck_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1EQInterlockErr_Ack_RF.BackColor = _craneSignal.Controller.AckSignal.EQInlineInterlockErrAck_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IOInterlockErr_Ack_RF.BackColor = _craneSignal.Controller.AckSignal.IOInlineInterlockErrAck_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IDMismatch_Ack_RF.BackColor = _craneSignal.Controller.AckSignal.IDMismatchAck_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IDReadError_Ack_RF.BackColor = _craneSignal.Controller.AckSignal.IDReadErrorAck_RF.IsOn() ? Color.Yellow : Color.White;

            lblRM1ScanComplete_Ack.BackColor = _craneSignal.Controller.AckSignal.ScanCompleteAck.IsOn() ? Color.Yellow : Color.White;
            //lblRM1StorageAlt_Ack.BackColor = _craneSignal.Controller.AckStatus.StoredAltRequestAck.IsOn() ? Color.Yellow : Color.White;
            //lblRM1Obstruction_Ack.BackColor = _craneSignal.Controller.AckStatus.Dual_ObstructionAck.IsOn() ? Color.Yellow : Color.White;
            lblRM1TransferRequestWrong_Ack.BackColor = _craneSignal.Controller.AckSignal.TransferRequestWrongAck_LF.IsOn() ? Color.Yellow : Color.White;

            lblRM1EmptyRetrieval_Req_LF.BackColor = _craneSignal.RequestSignal.EmptyRetrievalReq_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1DoubleStorage_Req_LF.BackColor = _craneSignal.RequestSignal.DoubleStorageReq_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1EQInterlockErr_Req_LF.BackColor = _craneSignal.RequestSignal.EQInlineInterlockErrReq_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IOInterlockErr_Req_LF.BackColor = _craneSignal.RequestSignal.IOInlineInterlockErrReq_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IDMismatch_Req_LF.BackColor = _craneSignal.RequestSignal.IDMismatchReq_LF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IDReadError_Req_LF.BackColor = _craneSignal.RequestSignal.IDReadErrorReq_LF.IsOn() ? Color.Yellow : Color.White;

            lblRM1EmptyRetrieval_Req_RF.BackColor = _craneSignal.RequestSignal.EmptyRetrievalReq_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1DoubleStorage_Req_RF.BackColor = _craneSignal.RequestSignal.DoubleStorageReq_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1EQInterlockErr_Req_RF.BackColor = _craneSignal.RequestSignal.EQInlineInterlockErrReq_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IOInterlockErr_Req_RF.BackColor = _craneSignal.RequestSignal.IOInlineInterlockErrReq_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IDMismatch_Req_RF.BackColor = _craneSignal.RequestSignal.IDMismatchReq_RF.IsOn() ? Color.Yellow : Color.White;
            lblRM1IDReadError_Req_RF.BackColor = _craneSignal.RequestSignal.IDReadErrorReq_RF.IsOn() ? Color.Yellow : Color.White;

            lblRM1ScanComplete_Req.BackColor = _craneSignal.RequestSignal.ScanCompleteReq.IsOn() ? Color.Yellow : Color.White;
            //lblRM1StorageAlt_Req.BackColor = _craneSignal.RequestStatus.StoredAltRequestReq.IsOn() ? Color.Yellow : Color.White;
            //lblRM1Obstruction_Req.BackColor = _craneSignal.RequestStatus.Dual_ObstructionReq.IsOn() ? Color.Yellow : Color.White;
            lblRM1TransferRequestWrong_Req.BackColor = _craneSignal.RequestSignal.TransferRequestWrongReq_LF.IsOn() ? Color.Yellow : Color.White;
        }

        private void ResfreshMotorPanel()
        {
            lblRM1_Motor01.Text = _craneSignal.Motor.Motor_01_Travel1.GetValue().ToString();
            lblRM1_Motor02.Text = _craneSignal.Motor.Motor_02_Travel2.GetValue().ToString();
            lblRM1_Motor03.Text = _craneSignal.Motor.Motor_03_Travel3.GetValue().ToString();
            lblRM1_Motor04.Text = _craneSignal.Motor.Motor_04_Travel4.GetValue().ToString();
            lblRM1_Motor05.Text = _craneSignal.Motor.Motor_05_Lifter1.GetValue().ToString();
            lblRM1_Motor06.Text = _craneSignal.Motor.Motor_06_Lifter2.GetValue().ToString();
            lblRM1_Motor07.Text = _craneSignal.Motor.Motor_07_Lifter3.GetValue().ToString();
            lblRM1_Motor08.Text = _craneSignal.Motor.Motor_08_Lifter4.GetValue().ToString();
            lblRM1_Motor09.Text = _craneSignal.Motor.Motor_09_Rotate1.GetValue().ToString();
            lblRM1_Motor10.Text = _craneSignal.Motor.Motor_10_Fork1.GetValue().ToString();
            lblRM1_Motor11.Text = _craneSignal.Motor.Motor_11_Fork2.GetValue().ToString();

            lblRM1_Motor01_ErrorCode.Text = _craneSignal.Motor.Motor_01_Travel1_ErrorCode.GetValue().ToString();
            lblRM1_Motor02_ErrorCode.Text = _craneSignal.Motor.Motor_02_Travel2_ErrorCode.GetValue().ToString();
            lblRM1_Motor03_ErrorCode.Text = _craneSignal.Motor.Motor_03_Travel3_ErrorCode.GetValue().ToString();
            lblRM1_Motor04_ErrorCode.Text = _craneSignal.Motor.Motor_04_Travel4_ErrorCode.GetValue().ToString();
            lblRM1_Motor05_ErrorCode.Text = _craneSignal.Motor.Motor_05_Lifter1_ErrorCode.GetValue().ToString();
            lblRM1_Motor06_ErrorCode.Text = _craneSignal.Motor.Motor_06_Lifter2_ErrorCode.GetValue().ToString();
            lblRM1_Motor07_ErrorCode.Text = _craneSignal.Motor.Motor_07_Lifter3_ErrorCode.GetValue().ToString();
            lblRM1_Motor08_ErrorCode.Text = _craneSignal.Motor.Motor_08_Lifter4_ErrorCode.GetValue().ToString();
            lblRM1_Motor09_ErrorCode.Text = _craneSignal.Motor.Motor_09_Rotate1_ErrorCode.GetValue().ToString();
            lblRM1_Motor10_ErrorCode.Text = _craneSignal.Motor.Motor_10_Fork1_ErrorCode.GetValue().ToString();
            lblRM1_Motor11_ErrorCode.Text = _craneSignal.Motor.Motor_11_Fork2_ErrorCode.GetValue().ToString();
        }
    }
}
