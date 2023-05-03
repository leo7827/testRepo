using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Config.Net;
using Mirle.LCS.LCSShareMemory;
using Mirle.LCS.View;
using Mirle.R46YP320.STK.TaskService;
using Mirle.STKC;
using Mirle.STKC.R46YP320;
using Mirle.Stocker;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.TraceLog;
using Mirle.Stocker.TaskControl.TraceLog.Format;
using Unity;

namespace Mirle.R46YP320.STK
{
    public partial class TaskAgent : Form
    {
        private DateTime _FocusedLeaveTime = DateTime.Now;
        private bool _FocusedFlag = false;
        private STKCHost _stkcHost;
        private Stocker.TaskControl.Info.LCSINI _Config;
        private IStocker _stocker;
        private LoggerService _loggerService;
        private TaskController _taskController;
        private UnityContainer _unityContainer;
        private Form stkcView;
        private LCSWatchDog _lcsWatchDog;
        private MCSStocker _mcsStocker;
        private readonly IDictionary<string, string> _TaskCannotExecuteReason = new Dictionary<string, string>();
        private readonly IDictionary<string, string> _UpdateFailReason = new Dictionary<string, string>();

        public TaskAgent()
        {
            InitializeComponent();
        }

        private async void Agent_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text += $" (V.{Application.ProductVersion})";

                _Config = new ConfigurationBuilder<Stocker.TaskControl.Info.LCSINI>().UseIniFile(@".\Config\LCS.ini").Build();
                _loggerService = new LoggerService(_Config.SystemConfig.StockerID);
                _lcsWatchDog = new LCSWatchDog(_Config.SystemConfig.StockerID);
                _lcsWatchDog.SetTaskAgent(LCSWatchDog.WatchDogStatus.Run);

                _loggerService.WriteLogTrace(new StockerMPLCEventTrace(0, $"TaskAgent (V.{Application.ProductVersion}) Program Start !"));

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                await Task.Run(() =>
                {
                    _stkcHost = new STKCHost(new InitialTrace());
                    _stocker = _stkcHost.GetSTKCManager().GetStockerController().GetStocker();
                });
                stkcView = _stkcHost.GetMainView();
                stkcView.Show();
                stkcView.Hide();
                var sim = _stkcHost.GetSimMainView();
                if(sim != null) sim.Show();
                Application.DoEvents();
                stopwatch.Stop();
                var ms1 = stopwatch.ElapsedMilliseconds;

                stopwatch.Restart();
             
                _taskController = new TaskController(_Config, _stocker, _loggerService);
                //Application.DoEvents();
                _taskController.Start();
                stopwatch.Stop();
                var ms2 = stopwatch.ElapsedMilliseconds;

                stopwatch.Restart();
                _mcsStocker = new MCSStocker(_stocker, _taskController.StockerInfo(), _taskController.DataCollectionEvents(), _loggerService, _stkcHost.GetAlarmService());
                stopwatch.Stop();
                var ms3 = stopwatch.ElapsedMilliseconds;

                var taskInfo = new TaskInfo(_Config);
                _unityContainer = new UnityContainer();
                _unityContainer.RegisterInstance(taskInfo);
                _unityContainer.RegisterInstance(_taskController.DataCollectionEvents());
                _unityContainer.RegisterInstance(new RepositoriesService(taskInfo, _loggerService));
                _unityContainer.RegisterInstance(_stocker);
                //Application.DoEvents();
                timerUIRefresh.Enabled = true;

                WindowState = FormWindowState.Minimized;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "Initial TaskAgent Fail!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Dispose();
                this.Close();
                Environment.Exit(Environment.ExitCode);
            }
        }

        private void timerUIRefresh_Tick(object sender, EventArgs e)
        {
            timerUIRefresh.Stop();

            _lcsWatchDog.SetTaskAgent(LCSWatchDog.WatchDogStatus.Run);
            if (_lcsWatchDog.IsNeedToStopAllService)
            {
                _lcsWatchDog.StopAllServiceACK();

                _loggerService.WriteLogTrace(new StockerMPLCEventTrace(0, $"TaskAgent (V.{Application.ProductVersion}) Program Close By LCSC UI !"));
                Task.Delay(1000).Wait();
                this.Dispose();
                this.Close();
                Environment.Exit(Environment.ExitCode);
            }


            lblNowDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            lblSTKID.Text = _Config.SystemConfig.StockerID;

            lblCommunicationState.BackColor = Color.Red;
            lblCommunicationState.Text = "Disable";

            switch(_taskController.SelectedEquipmentStatus().SCState)
            {
                case LCS.LCSShareMemory.LCSParameter.SCState.Auto:
                    lblSCState.Text = "Auto";
                    lblSCState.BackColor = Color.Lime;
                    break;

                case LCS.LCSShareMemory.LCSParameter.SCState.SCInit:
                    lblSCState.Text = "SCInit";
                    lblSCState.BackColor = Color.Red;
                    break;

                case LCS.LCSShareMemory.LCSParameter.SCState.Pausing:
                    lblSCState.Text = "Pausing";
                    lblSCState.BackColor = Color.Yellow;
                    break;

                case LCS.LCSShareMemory.LCSParameter.SCState.Paused:
                    lblSCState.Text = "Paused";
                    lblSCState.BackColor = Color.Yellow;
                    break;

                case LCS.LCSShareMemory.LCSParameter.SCState.None:
                default:
                    lblSCState.Text = "None";
                    lblSCState.BackColor = Color.Red;
                    break;
            }

            lsbTaskTrace.Items.AddRange(_loggerService.GetNewTaskTrace().ToArray());
            lsbSECSTrace.Items.AddRange(_loggerService.GetNewSECSTrace().ToArray());

            while(lsbTaskTrace.Items.Count > 300)
                lsbTaskTrace.Items.RemoveAt(0);
            while(lsbSECSTrace.Items.Count > 300)
                lsbSECSTrace.Items.RemoveAt(0);

            if(!_FocusedFlag && _FocusedLeaveTime.AddSeconds(30) < DateTime.Now)
            {
                lsbTaskTrace.SelectedIndex = lsbTaskTrace.Items.Count - 1;
                lsbSECSTrace.SelectedIndex = lsbSECSTrace.Items.Count - 1;
            }

            try
            {
                //lsbTransfer.Items.Clear();
                //var canNotTransferList = TaskProcessService.DicCanNotTransferReason;
                //foreach (var item in canNotTransferList)
                //{
                //    lsbTransfer.Items.Add($"CommandID : {item.Key}, Reason : {item.Value}");
                //}
                var tmp = _taskController.StockerInfo().GetTaskCannotExecuteReason();
                if (tmp.Any())
                {
                    foreach (var item in tmp)
                    {
                        if (_TaskCannotExecuteReason.ContainsKey(item.Key))
                        {
                            if ($"{item.Key} => {item.Value}" != $"{item.Key} => {_TaskCannotExecuteReason[item.Key]}")
                            {
                                lsbTaskReason.Items.Remove($"{item.Key} => {_TaskCannotExecuteReason[item.Key]}");
                                lsbTaskReason.Items.Add($"{item.Key} => {item.Value}");

                                _TaskCannotExecuteReason[item.Key] = item.Value;
                            }
                            else if (!lsbTaskReason.Items.Contains($"{item.Key} => {item.Value}"))
                            {
                                lsbTaskReason.Items.Add($"{item.Key} => {item.Value}");
                                _TaskCannotExecuteReason.Add(item.Key, item.Value);
                            }
                        }
                        else
                        {
                            lsbTaskReason.Items.Add($"{item.Key} => {item.Value}");
                            _TaskCannotExecuteReason.Add(item.Key, item.Value);
                        }
                    }
                }
                else
                {
                    _TaskCannotExecuteReason.Clear();
                    lsbTaskReason.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }
            timerUIRefresh.Start();
        }

        private void ListBoxMouseEnter(object sender, EventArgs e)
        {
            _FocusedFlag = true;
        }

        private void ListBoxMouseLeave(object sender, EventArgs e)
        {
            _FocusedFlag = false;
            _FocusedLeaveTime = DateTime.Now;
        }

        private async void butRestartSECSDriver_Click(object sender, EventArgs e)
        {
           
        }

        private async void butSenS1F1_Test_Click(object sender, EventArgs e)
        {
            
        }

        private async void butEnableCommunication_Test_Click(object sender, EventArgs e)
        {
            
        }

        private void butOnLineRemote_Test_Click(object sender, EventArgs e)
        {
           
        }

        private void butOnLineLocal_Test_Click(object sender, EventArgs e)
        {
            
        }

        private void butOffLine_Test_Click(object sender, EventArgs e)
        {
            
        }

        private void butAuto_Click(object sender, EventArgs e)
        {
            _taskController.SelectedEquipmentStatus().GetLCSParameter().AutoRequest();
        }

        private void butPause_Click(object sender, EventArgs e)
        {
            _taskController.SelectedEquipmentStatus().GetLCSParameter().PauseRequest();
        }

        private void butExitProgram_Click(object sender, EventArgs e)
        {
            CloseProgram frmCloseProgram = new CloseProgram();
            DialogResult Result = frmCloseProgram.ShowDialog();

            if(Result != DialogResult.OK)
            { return; }

            try
            {
                _loggerService.WriteLogTrace(new StockerMPLCEventTrace(0, $"TaskAgent (V.{Application.ProductVersion}) Program Close !"));
                Task.Delay(1000).Wait();
                this.Dispose();
                this.Close();
                Environment.Exit(Environment.ExitCode);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void TaskAgent_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            if(this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                nicTaskAgent.Visible = true;
            }
        }

        private void TaskAgent_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Alt && e.KeyCode == Keys.F4)
                e.Handled = true;
        }

        private void tsmiShow_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            nicTaskAgent.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            CloseProgram frmCloseProgram = new CloseProgram();
            DialogResult Result = frmCloseProgram.ShowDialog();

            if(Result != DialogResult.OK)
                return;

            try
            {
                _loggerService.WriteLogTrace(new StockerMPLCEventTrace(0, $"TaskAgent (V.{Application.ProductVersion}) Program Close !"));
                Task.Delay(1000).Wait();
                this.Dispose();
                this.Close();
                Environment.Exit(Environment.ExitCode);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void TaskAgent_Resize(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                nicTaskAgent.Visible = true;
            }
        }

        private void cmsShowIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            nicTaskAgent.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }

        private void butSTKC_Click(object sender, EventArgs e)
        {
            if(stkcView == null)
                stkcView = _stkcHost.GetMainView();
            stkcView.Show();
            stkcView.Activate();
        }
    }
}
