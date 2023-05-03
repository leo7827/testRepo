using System;
using Mirle.MPLC;
using System.Drawing;
using System.Reflection;
using Mirle.ASRS.Conveyor.V2BYMA30_Elevator.Service;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mirle.ASRS.Conveyor.V2BYMA30_Elevator.View
{
    public partial class MainView : Form
    {
        private LoggerService _loggerService = new LoggerService("CV");
        private ConveyorController conveyorController;
        private MonitorLayout _monitorLayout;
        private BufferPlcInfoView _bufferPlcInfoView;
        public MainView(ConveyorController controller)
        {
            InitializeComponent();
            conveyorController = controller;
            _monitorLayout = new MonitorLayout(controller);
            _bufferPlcInfoView = new BufferPlcInfoView(controller, _loggerService);
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            subInitialLayout();

            butMain_Layout.PerformClick();
            //Start Timer
            timMainProc.Interval = 300;
            timMainProc.Enabled = true;
        }

        public MonitorLayout GetMonitor()
        {
            return _monitorLayout;
        }

        private void timMainProc_Tick(object sender, EventArgs e)
        {
            timMainProc.Enabled = false;
            try
            {
                //Check PLC
                if (conveyorController.IsConnected)
                {
                    lblPLCConnSts.BackColor = Color.Lime;
                }
                else
                {
                    lblPLCConnSts.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                _loggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                timMainProc.Enabled = true;
            }
        }

        private void ChangeSubForm(Form subForm)
        {
            try
            {
                butMain_Layout.BackColor = subForm == _monitorLayout ? Color.Aqua : Color.Gainsboro;
                butMain_BufferPLCInfo.BackColor = subForm == _bufferPlcInfoView ? Color.Aqua : Color.Gainsboro;

                var children = splitContainer1.Panel1.Controls;
                foreach (Control c in children)
                {
                    if (c is Form)
                    {
                        var thisChild = c as Form;
                        //thisChild.Hide();
                        splitContainer1.Panel1.Controls.Remove(thisChild);
                        thisChild.Width = 0;
                    }
                }

                Form newChild = subForm;

                if (newChild != null)
                {
                    newChild.TopLevel = false;
                    newChild.Dock = DockStyle.Fill;//適應窗體大小
                    newChild.FormBorderStyle = FormBorderStyle.None;//隱藏右上角的按鈕
                    newChild.Parent = splitContainer1.Panel1;
                    splitContainer1.Panel1.Controls.Add(newChild);
                    newChild.Show();
                }
            }
            catch (Exception ex)
            {
                _loggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void subInitialLayout()
        {
            butMain_Layout.BackColor = Color.Aqua;
            butMain_BufferPLCInfo.BackColor = Color.Gainsboro;
        }

        private void butMain_Layout_Click(object sender, EventArgs e)
        {
            try
            {
                if (_monitorLayout == null)
                {
                    _monitorLayout = new MonitorLayout(conveyorController);
                }

                ChangeSubForm(_monitorLayout);
            }
            catch (Exception ex)
            {
                _loggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void butMain_BufferPLCInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bufferPlcInfoView == null)
                {
                    _bufferPlcInfoView = new BufferPlcInfoView(conveyorController, _loggerService);
                }

                ChangeSubForm(_bufferPlcInfoView);
            }
            catch (Exception ex)
            {
                _loggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
