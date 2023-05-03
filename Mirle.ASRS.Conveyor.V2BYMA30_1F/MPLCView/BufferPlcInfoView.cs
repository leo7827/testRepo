using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Linq;
using Mirle.ASRS.Conveyor.V2BYMA30_1F.Signal;
using System.Windows.Forms;
using Mirle.MPLC;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.MPLCView
{
    public partial class BufferPlcInfoView : Form
    {
        private ConveyorController _conveyorController;

        public BufferPlcInfoView(ConveyorController conveyorController)
        {
            InitializeComponent();
            _conveyorController = conveyorController;
        }

        private void BufferPlcInfoView_Load(object sender, EventArgs e)
        {
            comboBoxBufferIndex.Items.Clear();
            for (int i = 1; i <= SignalMapper.BufferCount; i++)
            {
                comboBoxBufferIndex.Items.Add($"{i}:A1-{i.ToString().PadLeft(2, '0')}");
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
                if (comboBoxBufferIndex.SelectedIndex != -1)
                {
                    int StnIdx = Convert.ToInt32(comboBoxBufferIndex.Text.Split(':')[0]);
                    clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).Auto, ref label_Auto);
                    clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).Manual, ref label_Manual);
                    clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).Error, ref label_Error);
                    clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).InMode, ref label_InMode);
                    clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).OutMode, ref lblOutMode);
                    clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).Position, ref label_Position);
                    clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).Presence, ref label_Load);
                    clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).Finish, ref label_Finish);
                    clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).EMO, ref label_EMO);
                    label_Command.Text = _conveyorController.GetBuffer(StnIdx).CommandID;
                    label_Ready.Text = _conveyorController.GetBuffer(StnIdx).Ready.ToString();
                    label_ReadAck.Text = _conveyorController.GetBuffer(StnIdx).ReadBcrAck.ToString();
                    label_Mode.Text = _conveyorController.GetBuffer(StnIdx).CommandMode.ToString();
                    label_Path.Text = _conveyorController.GetBuffer(StnIdx).PathNotice.ToString();
                    label_Initial.Text = _conveyorController.GetBuffer(StnIdx).InitialNotice.ToString();
                    lblTrayID.Text = _conveyorController.GetBuffer(StnIdx).GetTrayID;

                    /*
                    for (int i = 0; i < 16; i++)
                    {
                        Label LabelControl = Controls.Find("lblPickupFinishAck_" + i, true).FirstOrDefault() as Label;
                        clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).Signal.PickupFinish_Ack.PickupBit[i].Checked.IsOn(),
                            ref LabelControl);

                        LabelControl = Controls.Find("lblPickupFinishReq_" + i, true).FirstOrDefault() as Label;
                        clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).Signal.Controller.PickupFinish_Req.PickupBit[i].Checked.IsOn(),
                            ref LabelControl);

                        LabelControl = Controls.Find("lblPickup_" + i, true).FirstOrDefault() as Label;
                        clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).Signal.Controller.Pickup.PickupBit[i].Checked.IsOn(),
                            ref LabelControl);

                        if (i < 8)
                        {
                            LabelControl = Controls.Find("lblLittle_" + (i + 1), true).FirstOrDefault() as Label;

                            try
                            {
                                if (!string.IsNullOrWhiteSpace(_conveyorController.GetBuffer(StnIdx).GetLittleThingIdByLocation(i + 1)))
                                    LabelControl.Text = _conveyorController.GetBuffer(StnIdx).GetLittleThingIdByLocation(i + 1);
                                else LabelControl.Text = "";
                            }
                            catch
                            {
                                LabelControl.Text = "";
                            }

                            if (i < 2)
                            {
                                LabelControl = Controls.Find("lblFobBcr_" + (i + 1), true).FirstOrDefault() as Label;
                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(_conveyorController.GetBuffer(StnIdx).GetFobIdByLocation(i + 1)))
                                        LabelControl.Text = _conveyorController.GetBuffer(StnIdx).GetFobIdByLocation(i + 1);
                                    else LabelControl.Text = "";
                                }
                                catch
                                {
                                    LabelControl.Text = "";
                                }
                            }
                        }
                    }
                    */
                    lblCmd_PC.Text = _conveyorController.GetBuffer(StnIdx).CommandID_PC;
                    lblMode_PC.Text = _conveyorController.GetBuffer(StnIdx).CommandMode_PC.ToString();
                    lblRead_Req.Text = _conveyorController.GetBuffer(StnIdx).ReadBcrReq_PC.ToString();
                    lblPath_PC.Text = _conveyorController.GetBuffer(StnIdx).PathNotice_PC.ToString();
                    lblInitial_Req.Text = _conveyorController.GetBuffer(StnIdx).InitialNotice_PC.ToString();
                    //clsTool.Signal_Show(_conveyorController.GetBuffer(StnIdx).IsManualPutaway, ref lblManualPutaway);
                }
            }
            finally
            {
                refreshTimer.Enabled = true;
            }
        }
    }
}
