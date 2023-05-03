using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Config.Net;
using Mirle.MPLC;
using Mirle.MPLC.FileData;
using Mirle.MPLCViewer.Config;

namespace Mirle.MPLCViewer.View
{
    public partial class frmMain : Form
    {
        private MPLCViewerINI _config;

        private FileReader _fReader;

        private frmChartView _frmChartView;

        private frmFindView _frmFindView;

        private int lastTT = 0;
        private IMPLCViewController _mplcViewController;

        private string[] _filePaths;

        public frmMain()
        {
            InitializeComponent();
        }

        private void butLoadFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog objOFD = new OpenFileDialog();
            objOFD.Multiselect = true;
            objOFD.Filter = "Log (*.Txt;*.Log)|*.Txt;*.Log|" +
                            "All files (*.*)|*.*";
            objOFD.Title = "Open STKC PLC Raw Data";
            objOFD.ShowDialog();

            LoadNewRawDataFiles(objOFD.FileNames);
            _filePaths = objOFD.FileNames;
        }

        [Obsolete]
        private void butShowTimeChartView_Click(object sender, EventArgs e)
        {
            if (_frmChartView == null || _frmChartView.IsDisposed)
            {
                _frmChartView = new frmChartView(_config, _fReader);
            }

            _frmChartView.Show();
            _frmChartView.Focus();
        }

        private void butStart_Click(object sender, EventArgs e)
        {
            if (this.lstRawData.Items.Count > 0)
            {
                timerPlayer.Interval = Convert.ToInt32(dudInterval.Text);
                if (butStart.Tag.ToString() == "Start")
                {
                    timerPlayer.Enabled = true;
                    butStart.Tag = "Stop";
                    butStart.Image = imageList1.Images[1];
                }
                else
                {
                    timerPlayer.Enabled = false;
                    butStart.Tag = "Start";
                    butStart.Image = imageList1.Images[0];
                }
            }
        }

        private void buttonTogleHideList_Click(object sender, EventArgs e)
        {
            if (tlp2Main.ColumnStyles[1].Width == 0)
            {
                tlp2Main.ColumnStyles[1].SizeType = SizeType.Absolute;
                tlp2Main.ColumnStyles[1].Width = 200;
            }
            else
            {
                tlp2Main.ColumnStyles[1].SizeType = SizeType.Absolute;
                tlp2Main.ColumnStyles[1].Width = 0;
            }
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            if (_frmFindView == null || _frmFindView.IsDisposed)
            {
                _frmFindView = new frmFindView(_config, this, _fReader, _mplcViewController, _filePaths);
            }

            _frmFindView.Show();
            _frmFindView.WindowState = FormWindowState.Normal;
            _frmFindView.Focus();
            _frmFindView.TopMost = true;
        }

        private void dudInterval_SelectedItemChanged(object sender, EventArgs e)
        {
            timerPlayer.Interval = Convert.ToInt32(dudInterval.Text);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            _config = new ConfigurationBuilder<MPLCViewerINI>().UseIniFile(@".\Config\MPLCViewer.ini").Build();

            _mplcViewController = null;
            if (string.IsNullOrEmpty(_config.LogViewer.DefaultDLL) == false)
            {
                _mplcViewController = LoadMPLCViewControllerByFile(_config.LogViewer.DefaultDLL);
            }

            if (_mplcViewController == null)
            {
                _mplcViewController = LoadAnyMPLCViewController();
            }
            if (_mplcViewController == null)
            {
                MessageBox.Show("No Exist MPLC View Controller!!", "Load DLL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(Environment.ExitCode);
            }

            _fReader = _mplcViewController.GetFileReader();
            this.Text = "Mirle LCS MPLC LogViewer - " + _mplcViewController.Title;

            var child = _mplcViewController.DefaultView();
            this.MonitorPanel.Controls.Clear();
            child.TopLevel = false;
            child.Dock = System.Windows.Forms.DockStyle.Fill;//適應窗體大小
            child.FormBorderStyle = FormBorderStyle.None;//隱藏右上角的按鈕
            child.Parent = this.MonitorPanel;
            this.MonitorPanel.Controls.Add(child);
            child.Show();
        }

        private IMPLCViewController LoadAnyMPLCViewController()
        {
            var dllFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll").ToList();
            foreach (var dllFile in dllFiles)
            {
                var mplcViewController = LoadMPLCViewControllerByFile(dllFile);
                if (mplcViewController != null) return mplcViewController;
            }

            return null;
        }

        private IMPLCViewController LoadMPLCViewControllerByFile(string dllFile)
        {
            try
            {
                var assembly = Assembly.Load(AssemblyName.GetAssemblyName(dllFile));
                if (assembly != null)
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (!type.IsInterface && !type.IsAbstract)
                        {
                            if (type.GetInterface(typeof(IMPLCViewController).FullName) != null)
                            {
                                return (IMPLCViewController)Activator.CreateInstance(type);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
            }

            return null;
        }

        private void lstRawData_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] filePaths)
            {
                LoadNewRawDataFiles(filePaths);
            }
        }

        private void LoadNewRawDataFiles(string[] filePaths)
        {
            try
            {
                if (!filePaths.Any()) return;

                timerRefresh.Enabled = false;
                timerRawCaching.Enabled = false;

                this.textBox1.Text = string.Empty;
                lstRawData.Items.Clear();
                tsslCurrentRowTime.Text = "Current Time : 00:00:00.00000";
                tsslCurrentRow.Text = "Row: 0";
                tsslTotalRow.Text = "Total Row: 0";
                tsslFilePath.Text = "FilePath: ";

                _fReader.ClearFile();
                foreach (var file in filePaths)
                {
                    _fReader.AddFile(file);
                    if (string.IsNullOrEmpty(textBox1.Text))
                    {
                        textBox1.Text = file;
                        tsslFilePath.Text = $"FilePath: {file}";
                    }
                    else
                    {
                        textBox1.Text += Environment.NewLine + file;
                    }
                }

                _fReader.OpenFile();
                var indexes = _fReader.GetDateTimeIndexes().ToList();
                tsslTotalRow.Text = $"Total Row: {indexes.Count}";

                lstRawData.Visible = false;
                var i = 0;
                foreach (var index in indexes)
                {
                    lstRawData.Items.Add($"{i++}-{index:HH:mm:ss.fffff}");
                }

                lstRawData.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                timerRefresh.Enabled = true;
                timerRawCaching.Enabled = true;
            }
        }

        private void lstRawData_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void timerPlayer_Tick(object sender, EventArgs e)
        {
            timerPlayer.Enabled = false;
            if (this.lstRawData.SelectedIndex < this.lstRawData.Items.Count - 1)
            {
                this.lstRawData.SelectedIndex += 1;
                timerPlayer.Enabled = true;
            }
            else
            {
                butStart_Click(this, new EventArgs());
            }
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            int intTT = Convert.ToInt32(lstRawData.SelectedIndex);
            if (lastTT != intTT && lstRawData.Items.Count > 0)
            {
                lastTT = intTT;
                _fReader.Refresh(intTT);

                tsslCurrentRowTime.Text = $"Current Time: {_fReader.CurrentRowTime:HH:mm:ss.fffff}";
                tsslCurrentRow.Text = $"Current Row: {intTT}";
            }
        }

        private async void TimerRawCaching_Tick(object sender, EventArgs e)
        {
            try
            {
                timerRawCaching.Enabled = false;
                var percentage = await Task.Run(() => _fReader.CachingPercentage);
                tspbCaching.Value = (int)percentage;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
            }
            finally
            {
                timerRawCaching.Enabled = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                buttonFind_Click(this, null);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void FindPointByDateTime(DateTime dateTimeIndex)
        {
            foreach (var item in lstRawData.Items)
            {
                if (item.ToString().Contains(dateTimeIndex.ToString("HH:mm:ss.fffff")))
                {
                    lstRawData.SelectedItem = item;
                    break;
                }
            }
        }
    }
}
