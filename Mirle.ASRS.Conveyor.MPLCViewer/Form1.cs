using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mirle.MPLC;
using Mirle.MPLC.FileData;
using System.Reflection;

namespace Mirle.ASRS.Conveyor.MPLCViewer
{
    public partial class Form1 : Form
    {
        private FileReader _fReader;
        private IMPLCViewController _mplcViewController;
        private string[] _filePaths;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _mplcViewController = null;
                //_mplcViewController = LoadMPLCViewControllerByFile("Mirle.ASRS.Conveyor.V2BYMA30_8F.dll");
                //_mplcViewController = LoadMPLCViewControllerByFile("Mirle.ASRS.Conveyor.V2BYMA30_10F.dll");
                _mplcViewController = LoadMPLCViewControllerByFile("Mirle.ASRS.Conveyor.V2BYMA30_Elevator.dll");
                if (_mplcViewController == null)
                {
                    MessageBox.Show("No Exist MPLC View Controller!!", "Load DLL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(Environment.ExitCode);
                }

                _fReader = _mplcViewController.GetFileReader();
                this.Text = "Mirle CV MPLC LogViewer - " + _mplcViewController.Title;

                var child = _mplcViewController.DefaultView();
                this.MonitorPanel.Controls.Clear();
                child.TopLevel = false;
                child.Dock = System.Windows.Forms.DockStyle.Fill;//適應窗體大小
                child.FormBorderStyle = FormBorderStyle.None;//隱藏右上角的按鈕
                child.Parent = this.MonitorPanel;
                this.MonitorPanel.Controls.Add(child);
                child.Show();
            }
            catch
            {

            }
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        private void butLoadFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog objOFD = new OpenFileDialog();
            objOFD.Multiselect = true;
            objOFD.Filter = "Log (*.Txt;*.Log)|*.Txt;*.Log|" +
                            "All files (*.*)|*.*";
            objOFD.Title = "Open CV PLC Raw Data";
            objOFD.ShowDialog();

            LoadNewRawDataFiles(objOFD.FileNames);
            _filePaths = objOFD.FileNames;
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

        private void dudInterval_SelectedItemChanged(object sender, EventArgs e)
        {
            timerPlayer.Interval = Convert.ToInt32(dudInterval.Text);
        }

        private void lstRawData_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] filePaths)
            {
                LoadNewRawDataFiles(filePaths);
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

        private int lastTT = 0;
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

        private async void timerRawCaching_Tick(object sender, EventArgs e)
        {
            try
            {
                timerRawCaching.Enabled = false;
                var percentage = await Task.Run(() => _fReader.CachingPercentage);
                tspbCaching.Value = (int)percentage;
            }
            finally
            {
                timerRawCaching.Enabled = true;
            }
        }
    }
}
