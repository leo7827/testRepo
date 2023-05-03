using Mirle.Extensions;
using Mirle.Stocker.R46YP320.Signal;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mirle.Stocker.R46YP320.View
{
    public partial class MonitorPort : Form
    {
        private readonly CSOTStocker _stocker;
        private readonly MPLCViewController _ctrl;
        private EQPort _myEqPort;
        private int _eqMaxCount = 0;
        private IOPort _myIoPort;
        private int _ioMaxCount = 0;
        private IOVehicleSignal _myIoVehicleSignal;
        private int _vehicleMaxCount = 0;

        public MonitorPort(CSOTStocker stocker, MPLCViewController mplcViewController)
        {
            InitializeComponent();
            _stocker = stocker;
            _ctrl = mplcViewController;
            _eqMaxCount = SignalMapper4_11.MaximunEqPortNumber;
            _ioMaxCount = SignalMapper4_11.MaximumIoPortNumber;
            _vehicleMaxCount = SignalMapper4_11.MaximumIoPortVehicleNumber;

            for (int i = 1; i <= _eqMaxCount; i++)
            {
                this.cboEQPort.Items.Add(i.ToString());
            }

            for (int i = 1; i <= _ioMaxCount; i++)
            {
                this.cboIOPort.Items.Add(i.ToString());
            }

            for (int i = 1; i <= _vehicleMaxCount; i++)
            {
                this.cboVehicle.Items.Add(i.ToString());
            }
        }

        private void MonitorPort_Load(object sender, EventArgs e)
        {
            this.RefreshTimer.Enabled = true;
        }

        private void cboEQPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            int intEQ = int.Parse(cboEQPort.SelectedItem.ToString());

            if (intEQ > 0 && intEQ <= _eqMaxCount)
            {
                _myEqPort = _stocker.GetEQPortById(intEQ) as EQPort;

                if (_myEqPort != null)
                {
                    _ctrl.MappingControlAndSignal(lblEQ_CSTID, _myEqPort.Signal.CarrierId);
                    _ctrl.MappingControlAndSignal(lblEQ_L_REQ, _myEqPort.Signal.L_REQ);
                    _ctrl.MappingControlAndSignal(lblEQ_U_REQ, _myEqPort.Signal.U_REQ);
                    _ctrl.MappingControlAndSignal(lblEQ_Ready, _myEqPort.Signal.Ready);
                    _ctrl.MappingControlAndSignal(lblEQ_Carrier, _myEqPort.Signal.Carrier);
                    _ctrl.MappingControlAndSignal(lblEQ_PError, _myEqPort.Signal.PError);
                    _ctrl.MappingControlAndSignal(lblEQ_Spare, _myEqPort.Signal.Spare);
                    _ctrl.MappingControlAndSignal(lblEQ_POnline, _myEqPort.Signal.POnline);
                    _ctrl.MappingControlAndSignal(lblEQ_PEStop, _myEqPort.Signal.PEStop);
                    _ctrl.MappingControlAndSignal(lblEQ_Transferring_FromSTK, _myEqPort.Signal.Transferring_FromSTK);
                    _ctrl.MappingControlAndSignal(lblEQ_TR_REQ_FromSTK, _myEqPort.Signal.TR_REQ_FromSTK);
                    _ctrl.MappingControlAndSignal(lblEQ_BUSY_FromSTK, _myEqPort.Signal.BUSY_FromSTK);
                    _ctrl.MappingControlAndSignal(lblEQ_COMPLETE_FromSTK, _myEqPort.Signal.COMPLETE_FromSTK);
                    _ctrl.MappingControlAndSignal(lblEQ_CRANE_1_FromSTK, _myEqPort.Signal.CRANE_1_FromSTK);
                    _ctrl.MappingControlAndSignal(lblEQ_CRANE_2_FromSTK, _myEqPort.Signal.CRANE_2_FromSTK);
                    _ctrl.MappingControlAndSignal(lblEQ_AError_FromSTK, _myEqPort.Signal.AError_FromSTK);
                    _ctrl.MappingControlAndSignal(lblEQ_ForkNumber_FromSTK, _myEqPort.Signal.ForkNumber_FromSTK);
                }
            }
        }

        private void cboIOPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            int intIO = int.Parse(cboIOPort.SelectedItem.ToString());

            if (intIO > 0 && intIO <= this._ioMaxCount)
            {
                this._myIoPort = _stocker.GetIOPortById(intIO) as IOPort;

                if (_myIoPort != null)
                {
                    cboVehicle.SelectedIndex = 0;
                    cboVehicle_SelectedIndexChanged(this, e);

                    var ioSignal = _myIoPort.Signal;
                    _ctrl.MappingControlAndSignal(lblIORun, ioSignal.Run);
                    _ctrl.MappingControlAndSignal(lblIODown, ioSignal.Down);
                    _ctrl.MappingControlAndSignal(lblIOFault, ioSignal.Fault);
                    _ctrl.MappingControlAndSignal(lblIOInMode, ioSignal.InMode);
                    _ctrl.MappingControlAndSignal(lblIOOutMode, ioSignal.OutMode);
                    _ctrl.MappingControlAndSignal(lblIOAMMode, ioSignal.AutoManualMode);
                    _ctrl.MappingControlAndSignal(lblIOLoadOK, ioSignal.LoadOK);
                    _ctrl.MappingControlAndSignal(lblIOUnloadOK, ioSignal.UnloadOK);
                    _ctrl.MappingControlAndSignal(lblIOPLCBatteryLow, ioSignal.PLCBatteryLow_CPU);
                    _ctrl.MappingControlAndSignal(lblDoorOpenLimit_MGV, ioSignal.DoorOpenLimit_MGV);
                    _ctrl.MappingControlAndSignal(lblIOPGlassDetection_MGV, ioSignal.GlassDetection_MGV);
                    _ctrl.MappingControlAndSignal(lblIOReady_FromCrane, ioSignal.Ready_CraneSide);
                    _ctrl.MappingControlAndSignal(lblIOTRRequest_FromCrane, ioSignal.TRRequest_CraneSide);
                    _ctrl.MappingControlAndSignal(lblIOBusy_FromCrane, ioSignal.Complete_CraneSide);
                    _ctrl.MappingControlAndSignal(lblIORunEnable, ioSignal.RunEnable);
                    _ctrl.MappingControlAndSignal(lblIO_SRI_AMSwitchofMPLC, ioSignal.SRI.AutoManualSwitchIsAuto);
                    _ctrl.MappingControlAndSignal(lblIO_SRI_SafetyDoorClose, ioSignal.SRI.SafetyDoorClosed);
                    _ctrl.MappingControlAndSignal(lblIO_SRI_EMO, ioSignal.SRI.EMO);
                    _ctrl.MappingControlAndSignal(lblIO_SRI_MainCircuitOnEnable, ioSignal.SRI.MainCircuitOnEnable);
                    _ctrl.MappingControlAndSignal(lblIO_SRI_InService, ioSignal.Run);
                    _ctrl.MappingControlAndSignal(lblIOPortModeChangeable, ioSignal.PortModeChangeable);
                    _ctrl.MappingControlAndSignal(lblIOBCRReadDone, ioSignal.BCRReadDone);
                    _ctrl.MappingControlAndSignal(lblIOCSTTransferComplete, ioSignal.CSTTransferComplete_Req);
                    _ctrl.MappingControlAndSignal(lblIOCSTRemoveCheck, ioSignal.CSTRemoveCheck_Req);
                    _ctrl.MappingControlAndSignal(lblIOWaitIn, ioSignal.WaitIn);
                    _ctrl.MappingControlAndSignal(lblIOWaitOut, ioSignal.WaitOut);
                    _ctrl.MappingControlAndSignal(lblIOFBCRResultCSTID, ioSignal.BCRReadResult);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition1, ioSignal.Stages.ToList()[0].CarrierId);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition2, ioSignal.Stages.ToList()[1].CarrierId);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition3, ioSignal.Stages.ToList()[2].CarrierId);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition4, ioSignal.Stages.ToList()[3].CarrierId);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition5, ioSignal.Stages.ToList()[4].CarrierId);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition1, ioSignal.Stages.ToList()[0].LoadPresence);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition2, ioSignal.Stages.ToList()[1].LoadPresence);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition3, ioSignal.Stages.ToList()[2].LoadPresence);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition4, ioSignal.Stages.ToList()[3].LoadPresence);
                    _ctrl.MappingControlAndSignal(lblIOLoadPosition5, ioSignal.Stages.ToList()[4].LoadPresence);
                    _ctrl.MappingControlAndSignal(lblIOPLCAlarmIndex, ioSignal.ErrorIndex);
                    _ctrl.MappingControlAndSignal(lblIOAlarmCode, ioSignal.ErrorCode);
                    _ctrl.MappingControlAndSignal(lblIOPCAlarmIndex, ioSignal.Controller.PcErrorIndex);
                    _ctrl.MappingControlAndSignal(lblIFSignal1, ioSignal.IF_Signal1);
                    _ctrl.MappingControlAndSignal(lblIFSignal1_X, ioSignal.IF_Signal1);

                    _ctrl.MappingControlAndSignal(lblIOLReq, ioSignal.L_REQ);
                    _ctrl.MappingControlAndSignal(lblIOUReq, ioSignal.U_REQ);
                    _ctrl.MappingControlAndSignal(lblIOReady, ioSignal.Ready);
                    _ctrl.MappingControlAndSignal(lblIOCarrier, ioSignal.CARRIER);
                    _ctrl.MappingControlAndSignal(lblIOPError, ioSignal.PError);
                    _ctrl.MappingControlAndSignal(lblIOSpare, ioSignal.Spare);
                    _ctrl.MappingControlAndSignal(lblIOOnLine, ioSignal.POnline);
                    _ctrl.MappingControlAndSignal(lblIOPEStop, ioSignal.PEStop);
                    _ctrl.MappingControlAndSignal(lblIOTransferring_FromSTK, ioSignal.Transferring_FromSTK);
                    _ctrl.MappingControlAndSignal(lblIOTR_REQ_FromSTK, ioSignal.TR_REQ_FromSTK);
                    _ctrl.MappingControlAndSignal(lblIOBUSY_FromSTK, ioSignal.BUSY_FromSTK);
                    _ctrl.MappingControlAndSignal(lblIOCOMPLETE_FromSTK, ioSignal.COMPLETE_FromSTK);
                    _ctrl.MappingControlAndSignal(lblIOCRANE_1_FromSTK, ioSignal.CRANE_1_FromSTK);
                    _ctrl.MappingControlAndSignal(lblIOCRANE_2_FromSTK, ioSignal.CRANE_2_FromSTK);
                    _ctrl.MappingControlAndSignal(lblIOAError, ioSignal.AError_FromSTK);
                    _ctrl.MappingControlAndSignal(lblIOForkNumber, ioSignal.ForkNumber_FromSTK);
                }
            }
        }

        private void cboVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            int intVehicle = int.Parse(cboVehicle.SelectedItem.ToString());

            if (intVehicle > 0 && intVehicle <= this._vehicleMaxCount)
            {
                if (this._myIoPort != null)
                {
                    _myIoVehicleSignal = _myIoPort.Signal.Vehicles.ToList()[intVehicle - 1];

                    if (_myIoVehicleSignal != null)
                    {
                        _ctrl.MappingControlAndSignal(lblVehicleCST, _myIoVehicleSignal.CarrierId);
                        _ctrl.MappingControlAndSignal(lblVehicleActive, _myIoVehicleSignal.Active);
                        _ctrl.MappingControlAndSignal(lblVehicleLoaded, _myIoVehicleSignal.LoadPresence);
                        _ctrl.MappingControlAndSignal(lblVehicleError, _myIoVehicleSignal.Error);
                        _ctrl.MappingControlAndSignal(lblVehicleHomePosition, _myIoVehicleSignal.HomePosition);
                        _ctrl.MappingControlAndSignal(lblVehicleHPReturn, _myIoVehicleSignal.HPReturn);
                        _ctrl.MappingControlAndSignal(lblVehicleAuto, _myIoVehicleSignal.Auto);
                        _ctrl.MappingControlAndSignal(lblVehiclePosition1, _myIoVehicleSignal.CurrentLocation_P1);
                        _ctrl.MappingControlAndSignal(lblVehiclePosition2, _myIoVehicleSignal.CurrentLocation_P2);
                        _ctrl.MappingControlAndSignal(lblVehiclePosition3, _myIoVehicleSignal.CurrentLocation_P3);
                        _ctrl.MappingControlAndSignal(lblVehiclePosition4, _myIoVehicleSignal.CurrentLocation_P4);
                        _ctrl.MappingControlAndSignal(lblVehiclePosition5, _myIoVehicleSignal.CurrentLocation_P5);
                    }
                }
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            if (this._myEqPort != null)
            {
                var eqSignal = _myEqPort.Signal;
                lblEQ_CSTID.Text = _myEqPort.Signal.CSTID; 
                lblEQ_L_REQ.BackColor = _myEqPort.Signal.L_REQ.IsOn() ? Color.Yellow : Color.White;
                lblEQ_U_REQ.BackColor = _myEqPort.Signal.U_REQ.IsOn() ? Color.Yellow : Color.White;
                lblEQ_Ready.BackColor = _myEqPort.Signal.Ready.IsOn() ? Color.Yellow : Color.White;
                lblEQ_Carrier.BackColor = _myEqPort.Signal.Carrier.IsOn() ? Color.Yellow : Color.White;
                lblEQ_PError.BackColor = _myEqPort.Signal.PError.IsOn() ? Color.Yellow : Color.White;
                lblEQ_Spare.BackColor = _myEqPort.Signal.Spare.IsOn() ? Color.Yellow : Color.White;
                lblEQ_POnline.BackColor = _myEqPort.Signal.POnline.IsOn() ? Color.Yellow : Color.White;
                lblEQ_PEStop.BackColor = _myEqPort.Signal.PEStop.IsOn() ? Color.Yellow : Color.White;

                lblEQ_Transferring_FromSTK.BackColor = _myEqPort.Signal.Transferring_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblEQ_TR_REQ_FromSTK.BackColor = _myEqPort.Signal.TR_REQ_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblEQ_BUSY_FromSTK.BackColor = _myEqPort.Signal.BUSY_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblEQ_COMPLETE_FromSTK.BackColor = _myEqPort.Signal.COMPLETE_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblEQ_CRANE_1_FromSTK.BackColor = _myEqPort.Signal.CRANE_1_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblEQ_CRANE_2_FromSTK.BackColor = _myEqPort.Signal.CRANE_2_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblEQ_AError_FromSTK.BackColor = _myEqPort.Signal.AError_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblEQ_ForkNumber_FromSTK.BackColor = _myEqPort.Signal.ForkNumber_FromSTK.IsOn() ? Color.Yellow : Color.White;
            }

            if (this._myIoPort != null)
            {
                var ioSignal = _myIoPort.Signal;
                lblIORun.BackColor = ioSignal.Run.IsOn() ? Color.Yellow : Color.White;
                lblIODown.BackColor = ioSignal.Down.IsOn() ? Color.Yellow : Color.White;
                lblIOFault.BackColor = ioSignal.Fault.IsOn() ? Color.Yellow : Color.White;
                lblIOInMode.BackColor = ioSignal.InMode.IsOn() ? Color.Yellow : Color.White;
                lblIOOutMode.BackColor = ioSignal.OutMode.IsOn() ? Color.Yellow : Color.White;
                lblIOAMMode.BackColor = ioSignal.AutoManualMode.IsOn() ? Color.Yellow : Color.White;
                lblIOLoadOK.BackColor = ioSignal.LoadOK.IsOn() ? Color.Yellow : Color.White;
                lblIOUnloadOK.BackColor = ioSignal.UnloadOK.IsOn() ? Color.Yellow : Color.White;
                //lblIOAreaSensorBroken.BackColor = ioSignal.AreaSensorBroken.IsOn() ? Color.Yellow : Color.White;
                //lblIOAreaSensorTrigger.BackColor = ioSignal.AreaSenaorTrigger.IsOn() ? Color.Yellow : Color.White;

                lblIOPLCBatteryLow.BackColor = ioSignal.PLCBatteryLow_CPU.IsOn() ? Color.Yellow : Color.White;
                //lblIOPLCSRAMBatteryLow.BackColor = ioSignal.PLCBatteryLow_SRAM.IsOn() ? Color.Yellow : Color.White;
                lblDoorOpenLimit_MGV.BackColor = ioSignal.DoorOpenLimit_MGV.IsOn() ? Color.Yellow : Color.White;
                lblIOPGlassDetection_MGV.BackColor = ioSignal.GlassDetection_MGV.IsOn() ? Color.Yellow : Color.White;
                lblIOReady_FromCrane.BackColor = ioSignal.Ready_CraneSide.IsOn() ? Color.Yellow : Color.White;
                lblIOTRRequest_FromCrane.BackColor = ioSignal.TRRequest_CraneSide.IsOn() ? Color.Yellow : Color.White;
                lblIOBusy_FromCrane.BackColor = ioSignal.Busy_CraneSide.IsOn() ? Color.Yellow : Color.White;
                lblIOComplete_FromCrane.BackColor = ioSignal.Complete_CraneSide.IsOn() ? Color.Yellow : Color.White;
                lblIORunEnable.BackColor = ioSignal.RunEnable.IsOn() ? Color.Yellow : Color.White;

                lblIO_SRI_AMSwitchofMPLC.BackColor = ioSignal.SRI.AutoManualSwitchIsAuto.IsOn() ? Color.Yellow : Color.White;
                lblIO_SRI_SafetyDoorClose.BackColor = ioSignal.SRI.SafetyDoorClosed.IsOn() ? Color.Yellow : Color.White;
                lblIO_SRI_EMO.BackColor = ioSignal.SRI.EMO.IsOn() ? Color.Yellow : Color.White;
                lblIO_SRI_MainCircuitOnEnable.BackColor = ioSignal.SRI.MainCircuitOnEnable.IsOn() ? Color.Yellow : Color.White;
                //lblIO_SRI_LiftLowerLimit.BackColor = ioSignal.SRI.LiftLowerLimit.IsOn() ? Color.Yellow : Color.White;
                //lblIO_SRI_InService.BackColor = ioSignal.SRI.InService.IsOn() ? Color.Yellow : Color.White;
                lblIO_SRI_InService.BackColor = ioSignal.Run.IsOn() ? Color.Yellow : Color.White;
                lblIOPortModeChangeable.BackColor = ioSignal.PortModeChangeable.IsOn() ? Color.Yellow : Color.White;

                lblIOBCRReadDone.BackColor = ioSignal.BCRReadDone.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTFeed.BackColor = ioSignal.CSTFeed_Req.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTShift1.BackColor = ioSignal.CSTShift_1_Req.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTShift2.BackColor = ioSignal.CSTShift_2_Req.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTShift3.BackColor = ioSignal.CSTShift_3_Req.IsOn() ? Color.Yellow : Color.White;
                lblIOCSTTransferComplete.BackColor = ioSignal.CSTTransferComplete_Req.IsOn() ? Color.Yellow : Color.White;
                lblIOCSTRemoveCheck.BackColor = ioSignal.CSTRemoveCheck_Req.IsOn() ? Color.Yellow : Color.White;

                lblIOWaitIn.BackColor = ioSignal.WaitIn.IsOn() ? Color.Yellow : Color.White;
                lblIOWaitOut.BackColor = ioSignal.WaitOut.IsOn() ? Color.Yellow : Color.White;

                lblIOFBCRResultCSTID.Text = ioSignal.CSTID_BarcodeResultOnP1;
                lblIOLoadPosition1.Text = ioSignal.Stages.ToList()[0].CarrierId.GetData().ToASCII();
                lblIOLoadPosition2.Text = ioSignal.Stages.ToList()[1].CarrierId.GetData().ToASCII();
                lblIOLoadPosition3.Text = ioSignal.Stages.ToList()[2].CarrierId.GetData().ToASCII();
                lblIOLoadPosition4.Text = ioSignal.Stages.ToList()[3].CarrierId.GetData().ToASCII();
                lblIOLoadPosition5.Text = ioSignal.Stages.ToList()[4].CarrierId.GetData().ToASCII();
                lblIOLoadPosition1.BackColor = ioSignal.Stages.ToList()[0].LoadPresence.IsOn() ? Color.Lime : Color.White;
                lblIOLoadPosition2.BackColor = ioSignal.Stages.ToList()[1].LoadPresence.IsOn() ? Color.Lime : Color.White;
                lblIOLoadPosition3.BackColor = ioSignal.Stages.ToList()[2].LoadPresence.IsOn() ? Color.Lime : Color.White;
                lblIOLoadPosition4.BackColor = ioSignal.Stages.ToList()[3].LoadPresence.IsOn() ? Color.Lime : Color.White;
                lblIOLoadPosition5.BackColor = ioSignal.Stages.ToList()[4].LoadPresence.IsOn() ? Color.Lime : Color.White;

                lblIOPLCAlarmIndex.Text = ioSignal.ErrorIndex.GetValue().ToString();
                lblIOAlarmCode.Text = ioSignal.ErrCode_Main.ToString("X2") + ioSignal.ErrCode_Sub.ToString("X2");

                //lblIOBCRRF_Ack.BackColor = ioSignal.Controller.RFRead_Ack.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTFeed_Ack.BackColor = ioSignal.Controller.CSTFeed_Ack.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTShift1_Ack.BackColor = ioSignal.Controller.CSTShift_1_Ack.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTShift2_Ack.BackColor = ioSignal.Controller.CSTShift_2_Ack.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTShift3_Ack.BackColor = ioSignal.Controller.CSTShift_3_Ack.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTTransferComplete_Ack.BackColor = ioSignal.Controller.CSTTransferComplete_Ack.IsOn() ? Color.Yellow : Color.White;
                //lblIOCSTRemoveCheck_Ack.BackColor = ioSignal.Controller.CSTRemoveCheck_Ack.IsOn() ? Color.Yellow : Color.White;
                lblIOPCAlarmIndex.Text = ioSignal.Controller.PcErrorIndex.GetValue().ToString();

                lblIFSignal1.Text = ioSignal.IF_Signal1.GetValue().ToString();

                string strTemp = string.Empty;
                strTemp = Convert.ToString(ioSignal.IF_Signal1.GetValue(), 2).PadLeft(16, Convert.ToChar("0"));
                lblIFSignal1_X.Text = strTemp.Substring(0, 8) + " " + strTemp.Substring(8);

                lblIOLReq.BackColor = ioSignal.L_REQ.IsOn() ? Color.Yellow : Color.White;
                lblIOUReq.BackColor = ioSignal.U_REQ.IsOn() ? Color.Yellow : Color.White;
                lblIOReady.BackColor = ioSignal.Ready.IsOn() ? Color.Yellow : Color.White;
                lblIOCarrier.BackColor = ioSignal.CARRIER.IsOn() ? Color.Yellow : Color.White;
                lblIOPError.BackColor = ioSignal.PError.IsOn() ? Color.Yellow : Color.White;
                lblIOSpare.BackColor = ioSignal.Spare.IsOn() ? Color.Yellow : Color.White;
                lblIOOnLine.BackColor = ioSignal.POnline.IsOn() ? Color.Yellow : Color.White;
                lblIOPEStop.BackColor = ioSignal.PEStop.IsOn() ? Color.Yellow : Color.White;
                lblIOTransferring_FromSTK.BackColor = ioSignal.Transferring_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblIOTR_REQ_FromSTK.BackColor = ioSignal.TR_REQ_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblIOBUSY_FromSTK.BackColor = ioSignal.BUSY_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblIOCOMPLETE_FromSTK.BackColor = ioSignal.COMPLETE_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblIOCRANE_1_FromSTK.BackColor = ioSignal.CRANE_1_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblIOCRANE_2_FromSTK.BackColor = ioSignal.CRANE_2_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblIOAError.BackColor = ioSignal.AError_FromSTK.IsOn() ? Color.Yellow : Color.White;
                lblIOForkNumber.BackColor = ioSignal.ForkNumber_FromSTK.IsOn() ? Color.Yellow : Color.White;

                if (this._myIoVehicleSignal != null)
                {
                    lblVehicleCST.Text = _myIoVehicleSignal.CarrierId.GetData().ToASCII();
                    lblVehicleActive.BackColor = _myIoVehicleSignal.Active.IsOn() ? Color.Yellow : Color.White;
                    lblVehicleLoaded.BackColor = _myIoVehicleSignal.LoadPresence.IsOn() ? Color.Yellow : Color.White;
                    lblVehicleError.BackColor = _myIoVehicleSignal.Error.IsOn() ? Color.Yellow : Color.White;
                    lblVehicleHomePosition.BackColor = _myIoVehicleSignal.HomePosition.IsOn() ? Color.Yellow : Color.White;
                    lblVehicleHPReturn.BackColor = _myIoVehicleSignal.HPReturn.IsOn() ? Color.Yellow : Color.White;
                    lblVehicleAuto.BackColor = _myIoVehicleSignal.Auto.IsOn() ? Color.Yellow : Color.White;

                    lblVehiclePosition1.BackColor = _myIoVehicleSignal.CurrentLocation_P1.IsOn() ? Color.Yellow : Color.White;
                    lblVehiclePosition2.BackColor = _myIoVehicleSignal.CurrentLocation_P2.IsOn() ? Color.Yellow : Color.White;
                    lblVehiclePosition3.BackColor = _myIoVehicleSignal.CurrentLocation_P3.IsOn() ? Color.Yellow : Color.White;
                    lblVehiclePosition4.BackColor = _myIoVehicleSignal.CurrentLocation_P4.IsOn() ? Color.Yellow : Color.White;
                    lblVehiclePosition5.BackColor = _myIoVehicleSignal.CurrentLocation_P5.IsOn() ? Color.Yellow : Color.White;
                }
            }
        }
    }
}
