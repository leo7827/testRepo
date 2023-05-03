using System;
using Mirle.ASRS.Conveyor.V2BYMA30_10F.Events;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Mirle.MPLC;
using Mirle.ASRS.Conveyor.V2BYMA30_10F.View;
using System.Windows.Forms;

namespace Mirle.ASRS.Conveyor.V2BYMA30_10F.MPLCView
{
    public partial class MonitorLayout : Form
    {
        private ConveyorController _conveyorController;
        public delegate void StkLabelClickEventHandler(object sender, StkLabelClickArgs e);
        public event StkLabelClickEventHandler OnStkLabelClick;

        private bool[] reportedFlag = new bool[4];

        public MonitorLayout(ConveyorController conveyorController)
        {
            InitializeComponent();
            _conveyorController = conveyorController;
        }

        private void MonitorMainForm_Load(object sender, EventArgs e)
        {
            refreshTimer.Enabled = true;
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            refreshTimer.Enabled = false;
            try
            {
                for (int i = 1; i <= Signal.SignalMapper.BufferCount; i++)
                {
                    uclBuffer BufferControl = Controls.Find("uc" + i, true).FirstOrDefault() as uclBuffer;
                    BufferControl.Auto = _conveyorController.GetBuffer(i).Auto;
                    BufferControl.bLoad = _conveyorController.GetBuffer(i).Presence;
                    BufferControl.CmdMode = clsTool.funGetEnumValue<uclBuffer.enuCmdMode>(_conveyorController.GetBuffer(i).CommandMode);
                    BufferControl.CmdSno = _conveyorController.GetBuffer(i).CommandID;
                    BufferControl.Error = _conveyorController.GetBuffer(i).Error;
                    BufferControl.PathNotice = _conveyorController.GetBuffer(i).PathNotice;
                    BufferControl.Position = _conveyorController.GetBuffer(i).Position;
                    BufferControl.ReadNotice = _conveyorController.GetBuffer(i).Signal.AckSignal.ReadBcrAck.GetValue();
                    BufferControl.Ready = clsTool.funGetEnumValue<uclBuffer.enuReady>(_conveyorController.GetBuffer(i).Ready);
                    BufferControl.Done = _conveyorController.GetBuffer(i).Signal.StatusSignal.Finish.IsOn();
                    BufferControl.InitialNotice = _conveyorController.GetBuffer(i).Signal.AckSignal.InitalAck.GetValue();

                    //if (i == 4)
                    //{
                    //    ShowImage(ref pic1, i);
                    //}
                    //else if (i == 10)
                    //{
                    //    ShowImage(ref pic2, i);
                    //}
                    //else if (i == 16)
                    //{
                    //    ShowImage(ref pic3, i);
                    //}
                    //else if (i == 22)
                    //{
                    //    ShowImage(ref pic4, i);
                    //}
                    //else { }

                    //if (i == 41) ShowModeImage(ref picMode1, i);
                    //else if (i == 42) ShowModeImage(ref picMode2, i);
                    //else if (i == 43) ShowModeImage(ref picMode3, i);
                    //else if (i == 44) ShowModeImage(ref picMode4, i);
                    //else { }
                }
            }
            finally
            {
                refreshTimer.Enabled = true;
            }
        }

        private void ShowImage(ref PictureBox pic, int BufferIndex)
        {
            if (_conveyorController.GetBuffer(BufferIndex).InMode)
            {
                pic.Image = GetUpImage();
            }
            else if (_conveyorController.GetBuffer(BufferIndex).OutMode)
            {
                pic.Image = GetDownImage();
            }
            else
            {
                pic.Image = GetXImage();
            }
        }

      

        private Image GetUpImage()
        {
            return imageList1.Images[0];
        }

        private Image GetDownImage()
        {
            return imageList1.Images[1];
        }

        private Image GetLeftImage()
        {
            return imageList1.Images[2];
        }

        private Image GetRightImage()
        {
            return imageList1.Images[3];
        }

        private Image GetXImage()
        {
            return imageList1.Images[4];
        }

        private Image GetOnlineImage()
        {
            return imageList1.Images[5];
        }

        private Image GetOfflineImage()
        {
            return imageList1.Images[6];
        }

        private void lblSTK_1_DoubleClick(object sender, EventArgs e)
        {
            if (reportedFlag[0] == false)
            {
                reportedFlag[0] = true;
                OnStkLabelClick?.Invoke(this, new StkLabelClickArgs(1));
                reportedFlag[0] = false;
            }
        }

        private void lblSTK_2_DoubleClick(object sender, EventArgs e)
        {
            if (reportedFlag[1] == false)
            {
                reportedFlag[1] = true;
                OnStkLabelClick?.Invoke(this, new StkLabelClickArgs(2));
                reportedFlag[1] = false;
            }
        }

        private void lblSTK_3_DoubleClick(object sender, EventArgs e)
        {
            if (reportedFlag[2] == false)
            {
                reportedFlag[2] = true;
                OnStkLabelClick?.Invoke(this, new StkLabelClickArgs(3));
                reportedFlag[2] = false;
            }
        }

        private void lblSTK_4_DoubleClick(object sender, EventArgs e)
        {
            if (reportedFlag[3] == false)
            {
                reportedFlag[3] = true;
                OnStkLabelClick?.Invoke(this, new StkLabelClickArgs(4));
                reportedFlag[3] = false;
            }
        }
    }
}
