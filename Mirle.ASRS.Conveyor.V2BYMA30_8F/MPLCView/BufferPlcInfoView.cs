using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Linq;
using Mirle.ASRS.Conveyor.V2BYMA30_8F.Signal;
using System.Windows.Forms;
using Mirle.MPLC;

namespace Mirle.ASRS.Conveyor.V2BYMA30_8F.MPLCView
{
    public partial class BufferPlcInfoView : Form
    {
        private ConveyorController conveyorController;

        public BufferPlcInfoView(ConveyorController controller)
        {
            InitializeComponent();
            conveyorController = controller;
        }

        private void BufferPlcInfoView_Load(object sender, EventArgs e)
        {
            comboBoxBufferIndex.Items.Clear();
            for (int i = 1; i <= SignalMapper.BufferCount; i++)
            {
                comboBoxBufferIndex.Items.Add($"{i}:LO2-{i.ToString().PadLeft(2, '0')}");
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
                    //label_Initial.Text = conveyorController.GetBuffer(StnIdx).InitialNotice.ToString();
                    lblTrayID.Text = conveyorController.GetBuffer(StnIdx).GetTrayID;
                    lblFobBcr_1.Text = conveyorController.GetBuffer(StnIdx).CarrierType.ToString();

                    lblCmd_PC.Text = conveyorController.GetBuffer(StnIdx).CommandID_PC;
                    lblMode_PC.Text = conveyorController.GetBuffer(StnIdx).CommandMode_PC.ToString();
                    lblRead_Req.Text = conveyorController.GetBuffer(StnIdx).ReadBcrReq_PC.ToString();
                    lblRoll_PC.Text = conveyorController.GetBuffer(StnIdx).RollNotice_PC.ToString();
                    //lblInitial_Req.Text = conveyorController.GetBuffer(StnIdx).InitialNotice_PC.ToString();

                    lblCarrierType_PC.Text = conveyorController.GetBuffer(StnIdx).CarrierTypeNotice_PC.ToString();
                    lblBcrCheckRequestNotice_PC.Text = conveyorController.GetBuffer(StnIdx).BcrCheckRequestNotice_PC.ToString();
                    lblTransferReportNotice_PC.Text = conveyorController.GetBuffer(StnIdx).TransferReportNotice_PC.ToString();
                }
            }
            finally
            {
                refreshTimer.Enabled = true;
            }
        }
    }
}
