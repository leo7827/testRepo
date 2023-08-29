using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Linq;
using Mirle.ASRS.Conveyor.V2BYMA30_Elevator.Signal;
using Mirle.ASRS.Conveyor.V2BYMA30_Elevator.Service;
using System.Windows.Forms;
using Mirle.MPLC;

namespace Mirle.ASRS.Conveyor.V2BYMA30_Elevator.View
{
    public partial class BufferPlcInfoView : Form
    {
        private ConveyorController conveyorController;
        private readonly LoggerService _LoggerService;
        public BufferPlcInfoView()
        {
            InitializeComponent();
        }   

        public BufferPlcInfoView(ConveyorController controller, LoggerService Log)
        {
            InitializeComponent();

            conveyorController = controller;
            _LoggerService = Log;
        }

        private void BufferPlcInfoView_Load(object sender, EventArgs e)
        {
            comboBoxBufferIndex.Items.Clear();
            for (int i = 1; i <= SignalMapper.BufferCount; i++)
            {
                comboBoxBufferIndex.Items.Add($"{i}:LI1-{i.ToString().PadLeft(2, '0')}");
            }

            #region 調整Combobox寬度
            int cWidth = 0;
            Graphics g = comboBoxBufferIndex.CreateGraphics();
            for (int i = 0; i < comboBoxBufferIndex.Items.Count; i++)
            {
                int cTemp = (int)g.MeasureString(comboBoxBufferIndex.Items[i].ToString(), comboBoxBufferIndex.Font).Width;
                if (cTemp > cWidth)
                    cWidth = cTemp;
            }
            comboBoxBufferIndex.DropDownWidth = cWidth;
            #endregion 調整Combobox寬度

            refreshTimer.Enabled = true;
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            refreshTimer.Enabled = false;
            try
            {
                if (conveyorController.IsConnected)
                {
                    if (comboBoxBufferIndex.SelectedIndex != -1)
                    {
                        int StnIdx = Convert.ToInt32(comboBoxBufferIndex.Text.Split(':')[0]);
                        clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).Auto, ref label_Auto);
                        clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).Manual, ref label_Manual);
                        clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).Error, ref label_Error);
                        clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).InMode, ref label_InMode);
                        clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).OutMode, ref lblOutMode);
                        clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).Position, ref label_Position);
                        clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).Presence, ref label_Load);
                        clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).Finish, ref label_Finish);
                        clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).EMO, ref label_EMO);
                        label_Command.Text = conveyorController.GetBuffer(StnIdx).CommandID;
                        label_Ready.Text = conveyorController.GetBuffer(StnIdx).Ready.ToString();
                        label_ReadAck.Text = conveyorController.GetBuffer(StnIdx).ReadBcrAck.ToString();
                        label_Mode.Text = conveyorController.GetBuffer(StnIdx).CommandMode.ToString();
                        label_Roll.Text = conveyorController.GetBuffer(StnIdx).RollNotice.ToString();
                        label_Initial.Text = conveyorController.GetBuffer(StnIdx).InitialNotice.ToString();
                        lblTrayID.Text = conveyorController.GetBuffer(StnIdx).GetTrayID;
                        lblFobBcr_1.Text = conveyorController.GetBuffer(StnIdx).PathNotice.ToString(); 
                        //lblFobBcr_1.Text = conveyorController.Signal.Floor.GetValue().ToString();                         
                        lblDoorSts.Text = conveyorController.Signal.DoorStatus.GetValue().ToString();
                        lblFloor.Text = conveyorController.Signal.Floor.GetValue().ToString();

                        clsTool.Signal_Show(conveyorController.Idle, ref lbl_Idle);
                        clsTool.Signal_Show(conveyorController.Running, ref lbl_Busy);
                        clsTool.Signal_Show(conveyorController.Up, ref lbl_Up);
                        clsTool.Signal_Show(conveyorController.Down, ref lbl_Down);
                        clsTool.Signal_Show(conveyorController.AgvMode, ref lbl_Agv);
                        clsTool.Signal_Show(conveyorController.PlatformOn, ref lbl_Platform);

                        lblCmd_PC.Text = conveyorController.GetBuffer(StnIdx).CommandID_PC;
                        lblMode_PC.Text = conveyorController.GetBuffer(StnIdx).CommandMode_PC.ToString();
                        lblRead_Req.Text = conveyorController.GetBuffer(StnIdx).ReadBcrReq_PC.ToString();
                        lblRoll_PC.Text = conveyorController.GetBuffer(StnIdx).RollNotice_PC.ToString();
                        lblInitial_Req.Text = conveyorController.GetBuffer(StnIdx).InitialNotice_PC.ToString();

                        lblPathNotice_PC.Text = conveyorController.GetBuffer(StnIdx).PathNotice_PC.ToString();
                        //lblBcrCheckRequestNotice_PC.Text = conveyorController.GetBuffer(StnIdx).BcrCheckRequestNotice_PC.ToString();
                        lblTransferReportNotice_PC.Text = conveyorController.GetBuffer(StnIdx).TransferReportNotice_PC.ToString();
                        lblElevatorNotice_PC.Text = conveyorController.Signal.Controller.Path.GetValue().ToString();
                        lblDoorNotice_PC.Text = conveyorController.Signal.Controller.DoorNoticce.GetValue().ToString();
                        //clsTool.Signal_Show(conveyorController.GetBuffer(StnIdx).IsManualPutaway, ref lblManualPutaway);

                        lblLittle_4.Text = conveyorController.Signal.Controller.ErrorIndex.GetValue().ToString();
                        lblLittle_8.Text = conveyorController.Signal.ErrorIndex.GetValue().ToString();
                    }
                }
            }
            finally
            {
                refreshTimer.Enabled = true;
            }
        }

        private void btnInitial_Ack_Click(object sender, EventArgs e)
        {
            btnInitial_Ack.Enabled = false;
            try
            {
                if (comboBoxBufferIndex.SelectedIndex > -1 &&
                    conveyorController.IsConnected)
                {
                    int StnIdx = Convert.ToInt32(comboBoxBufferIndex.Text.Split(':')[0]);
                    conveyorController.GetBuffer(StnIdx).NoticeInital();

                    _LoggerService.WriteLog($"手動按下CV初始通知按鈕： <Buffer> {comboBoxBufferIndex.Text.Split(':')[1]}");
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                btnInitial_Ack.Enabled = true;
            }
        }

        private void btnInitial_PcToPlc_Click(object sender, EventArgs e)
        {
            btnInitial_PcToPlc.Enabled = false;
            try
            {
                if (comboBoxBufferIndex.SelectedIndex > -1 &&
                    conveyorController.IsConnected)
                { 
                    int StnIdx = Convert.ToInt32(comboBoxBufferIndex.Text.Split(':')[0]);
                    //conveyorController.GetBuffer(StnIdx).NoticeInital();
                    conveyorController.GetBuffer(StnIdx).WriteCommandAsync("00000", 0, 0);
                    //conveyorController.GetBuffer(StnIdx).SetReadReq(0);
                    conveyorController.GetBuffer(StnIdx).InfoRolling(0);
                    //conveyorController.GetBuffer(StnIdx).WriteBcrSetReadReq(0);
                    conveyorController.GetBuffer(StnIdx).WriteCarrierType(0);
                    conveyorController.GetBuffer(StnIdx).WriteTransferReport(0);
                    _LoggerService.WriteLog($"手動按下CV初始PC -> PLC按鈕：<Buffer> {comboBoxBufferIndex.Text.Split(':')[1]}");
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                btnInitial_PcToPlc.Enabled = true;
            }
        }

        private void btnInitial_System_Click(object sender, EventArgs e)
        {
            btnInitial_System.Enabled = false;
            try
            {
                conveyorController.Floor_Ini();
                conveyorController.Door_Ini();
                _LoggerService.WriteLog($"手動按下電梯初始通知");
            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                btnInitial_System.Enabled = true;
            }
        }

        private void btnTo8F_Click(object sender, EventArgs e)
        {
            conveyorController.SetFloor(8);
            _LoggerService.WriteLog($"手動按下8樓移動通知");
        }

        private void btnTo10F_Click(object sender, EventArgs e)
        {
            conveyorController.SetFloor(10);
            _LoggerService.WriteLog($"手動按下10樓移動通知");
        }

        private void btnOpenDoor_Click(object sender, EventArgs e)
        {
            conveyorController.WriteDoorIndex(2);
            _LoggerService.WriteLog($"手動按下開門通知");
        }

        private void btnCloseDoor_Click(object sender, EventArgs e)
        {
            conveyorController.WriteDoorIndex(1);
            _LoggerService.WriteLog($"手動按下關門通知");
        }
    }
}
