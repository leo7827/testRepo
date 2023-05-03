using System;
using Mirle.ASRS.Conveyor.V2BYMA30_1F.Events;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Mirle.MPLC;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.View
{
    public partial class MonitorLayout : Form
    {
        private ConveyorController conveyorController;
        //private ConveyorController conveyorController_3F;

        public delegate void StkLabelClickEventHandler(object sender, StkLabelClickArgs e);
        public event StkLabelClickEventHandler OnStkLabelClick;

        public bool[] Online = new bool[4];
        private bool[] reportedFlag = new bool[4];

        public MonitorLayout()
        {
            InitializeComponent();

        }

        public MonitorLayout(ConveyorController controller)
        {
            InitializeComponent();

            conveyorController = controller;
            //conveyorController_3F = controller;
        }

        public void FunSetOnline(int StockerID, bool bFlag)
        {
            CheckBox checkBox = Controls.Find("chkOnline" + StockerID, true).FirstOrDefault() as CheckBox;
            checkBox.Checked = bFlag;
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
                
                //for(int i = 0; i < 4; i++)
                //{
                //    CheckBox checkBox = Controls.Find("chkOnline" + (i+1), true).FirstOrDefault() as CheckBox;
                //    Online[i] = checkBox.Checked;
                //}
                //lblCycleCount.Text = ConveyorController.CycleCount + "/" + ConveyorController.CycleCountMax;
                if (!conveyorController.IsConnected)
                {
                    for (int i = 1; i <= Signal.SignalMapper.BufferCount; i++)
                    {
                        uclBuffer BufferControl = Controls.Find("uc" + i, true).FirstOrDefault() as uclBuffer;
                        BufferControl.funReadPLCError();

                        //if(i < 5)
                        //{
                        //    PictureBox PicControl = Controls.Find("pic" + i, true).FirstOrDefault() as PictureBox;
                        //    PicControl.Image = GetXImage();

                        //    PicControl = Controls.Find("picMode" + i, true).FirstOrDefault() as PictureBox;
                        //    PicControl.Image = GetXImage();
                        //}
                    }
                }
                else
                {
                    for (int i = 1; i <= Signal.SignalMapper.BufferCount; i++)
                    {
                        uclBuffer BufferControl = Controls.Find("uc" + i, true).FirstOrDefault() as uclBuffer;
                        BufferControl.Auto = conveyorController.GetBuffer(i).Auto;
                        BufferControl.bLoad = conveyorController.GetBuffer(i).Presence;
                        BufferControl.CmdMode = clsTool.funGetEnumValue<uclBuffer.enuCmdMode>(conveyorController.GetBuffer(i).CommandMode);
                        BufferControl.CmdSno = conveyorController.GetBuffer(i).CommandID;
                        BufferControl.Error = conveyorController.GetBuffer(i).Error;
                        BufferControl.PathNotice = conveyorController.GetBuffer(i).PathNotice;
                        BufferControl.Position = conveyorController.GetBuffer(i).Position;
                        BufferControl.ReadNotice = conveyorController.GetBuffer(i).Signal.AckSignal.ReadBcrAck.GetValue();
                        BufferControl.Ready = clsTool.funGetEnumValue<uclBuffer.enuReady>(conveyorController.GetBuffer(i).Ready);
                        BufferControl.Done = conveyorController.GetBuffer(i).Signal.StatusSignal.Finish.IsOn();
                        BufferControl.InitialNotice = conveyorController.GetBuffer(i).Signal.AckSignal.InitalAck.GetValue();
                         

                    }
                }
            }
            finally
            {
                refreshTimer.Enabled = true;
            }
        }

        private void ShowImage(ref PictureBox pic, int BufferIndex)
        {
            if (conveyorController.GetBuffer(BufferIndex).InMode)
            {
                pic.Image = GetUpImage();
            }
            else if (conveyorController.GetBuffer(BufferIndex).OutMode)
            {
                pic.Image = GetDownImage();
            }
            else
            {
                pic.Image = GetXImage();
            }
        }

        private void ShowModeImage(ref PictureBox pic, int BufferIndex)
        {
            if (conveyorController.GetBuffer(BufferIndex).Online)
            {
                pic.Image = GetOnlineImage();
            }
            else if (conveyorController.GetBuffer(BufferIndex).Offline)
            {
                pic.Image = GetOfflineImage();
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

        private void chkOnlineUnit_CheckedChanged(object sender, EventArgs e)
        {
            //
        }

    }
}
