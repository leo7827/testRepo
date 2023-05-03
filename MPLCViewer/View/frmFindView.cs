using CsvHelper;
using Mirle.MPLC;
using Mirle.MPLC.DataType;
using Mirle.MPLC.FileData;
using Mirle.MPLCViewer.Model;
using Mirle.MPLCViewer.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mirle.Extensions;

namespace Mirle.MPLCViewer.View
{
    public partial class frmFindView : Form
    {
        private readonly MPLCViewerINI _config;
        private readonly frmMain _frmMainView;
        private readonly FileReader _fReader;
        private readonly IMPLCViewController _mplcViewController;
        private readonly string[] _filePaths;

        private static string _fileTime = "";

        public frmFindView(MPLCViewerINI config, frmMain frmMainView, FileReader fReader, IMPLCViewController mplcViewController, string[] filePaths)
        {
            _config = config;
            _frmMainView = frmMainView;
            _fReader = fReader;
            _mplcViewController = mplcViewController;
            _filePaths = filePaths;
            InitializeComponent();
        }

        private async void ButtonFind_Click(object sender, EventArgs e)
        {
            buttonFind.Enabled = false;
            TopMost = false;
            bool DontCheckNegative = false;

            try
            {
                var dataView = _fReader.GetDataView();
                var address = textBoxAddress.Text.ToUpper();
                IDataType signal = null;

                if (radioButtonBit.Checked)
                {
                    signal = new Bit(dataView, address);
                    dataGridView1.DataSource = await SearchResults(dataView, signal, DontCheckNegative);
                }
                else if (radioButtonWord.Checked)
                {
                    signal = new Word(dataView, address);
                    dataGridView1.DataSource = await SearchResults(dataView, signal, DontCheckNegative);
                }
                else if (radioButtonDWord.Checked)
                {
                    signal = new DWord(dataView, address);
                    dataGridView1.DataSource = await SearchResults(dataView, signal, DontCheckNegative);
                }
                else if (radioButtonWordBlock.Checked)
                {
                    var wordBlock = new WordBlock(dataView, address, Convert.ToInt32(textBoxLength.Text));
                    dataGridView1.DataSource = await SearchResultsForWordBlock(dataView, wordBlock);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "Find Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonFind.Enabled = true;
                TopMost = true;
            }
        }

        private Task<List<FindReuslt>> SearchResults(FileDataViewer dataView, IDataType signal, bool needCheckNegative)
        {
            return Task.Run(() =>
             {
                 var result = new List<FindReuslt>();
                 var points = dataView.Query(DateTime.MinValue, DateTime.MaxValue).ToList();
                 var lastValue = 0;
                 foreach (var point in points)
                 {
                     dataView.RefreshRawData(point);
                     var actual = Convert.ToInt32(signal.Value);
                     if (needCheckNegative)
                         actual = checkNegative(actual);

                     var excepted = radioButtonHexadecimal.Checked
                         ? textBoxEqualsValue.Text.HexToInt()
                         : Convert.ToInt32(textBoxEqualsValue.Text);
                     var actualString = radioButtonHexadecimal.Checked
                         ? actual.ToString("X") : actual.ToString();

                     if (radioButtonDifferent.Checked && actual != lastValue)
                     {
                         result.Add(new FindReuslt() { Time = point.ToString("HH:mm:ss.fffff"), Value = actualString });
                         lastValue = actual;
                     }
                     else if (radioButtonEquals.Checked && actual == excepted)
                     {
                         result.Add(new FindReuslt() { Time = point.ToString("HH:mm:ss.fffff"), Value = actualString });
                     }
                     else if (radioButtonNotEquals.Checked && actual != excepted)
                     {
                         result.Add(new FindReuslt() { Time = point.ToString("HH:mm:ss.fffff"), Value = actualString });
                     }
                 }

                 ExportResult(result, null);

                 return result;
             });
        }

        private static void ExportResult(List<FindReuslt> result, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "MPLCViewer_SearchResults_" + _fileTime;
                path = $@".\{path}.csv";
            }
            else
            {
                string filder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                path = $@"{filder}\{path}.csv";
            }

            try
            {
                using (var writer = new StreamWriter(path))
                {
                    using (var csv = new CsvWriter(writer))
                    {
                        csv.WriteHeader<FindReuslt>();
                        csv.NextRecord();
                        csv.WriteRecords(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export Result Fail!! : {path}.csv"
                    , "Export Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Task<List<FindReuslt>> SearchResultsForWordBlock(FileDataViewer dataView, WordBlock signal)
        {
            return Task.Run(() =>
            {
                var result = new List<FindReuslt>();
                var points = dataView.Query(DateTime.MinValue, DateTime.MaxValue).ToList();
                var lastValue = string.Empty;
                foreach (var point in points)
                {
                    dataView.RefreshRawData(point);
                    var actual = signal.GetData().ToASCII().ToUpper();

                    if (radioButtonDifferent.Checked && actual != lastValue)
                    {
                        result.Add(new FindReuslt() { Time = point.ToString("HH:mm:ss.fffff"), Value = actual });
                        lastValue = actual;
                    }
                    else if (radioButtonEquals.Checked && actual.Contains(textBoxEqualsValue.Text.ToUpper()))
                    {
                        result.Add(new FindReuslt() { Time = point.ToString("HH:mm:ss.fffff"), Value = actual });
                    }
                    else if (radioButtonNotEquals.Checked && !actual.Contains(textBoxEqualsValue.Text.ToUpper()))
                    {
                        result.Add(new FindReuslt() { Time = point.ToString("HH:mm:ss.fffff"), Value = actual });
                    }
                }

                ExportResult(result, null);

                return result;
            });
        }

        private void FrmFindView_Load(object sender, EventArgs e)
        {
            if (_filePaths != null)
            {
                _fileTime = _filePaths.FirstOrDefault();
                _fileTime = _fileTime.Substring(_fileTime.Length - 14, 10);   //ex:  .....2019091321.log
            }

            if (_config.LogViewer.SearchVibrationEnable == 1)
                buttonExportVibration.Visible = true;
            else
                buttonExportVibration.Visible = false;
        }

        private void TimerUI_Tick(object sender, EventArgs e)
        {
            textBoxLength.Enabled = radioButtonWordBlock.Checked;
            groupBoxOption.Enabled = radioButtonWord.Checked || radioButtonDWord.Checked;
            textBoxEqualsValue.Enabled = !radioButtonDifferent.Checked;
            var signal = _mplcViewController.CurrentFocusedSignal;
            if (signal != null)
            {
                switch (signal)
                {
                    case Bit bit:
                        textBoxAddress.Text = bit.Address;
                        radioButtonBit.Checked = true;
                        break;

                    case Word word:
                        textBoxAddress.Text = word.Address;
                        radioButtonWord.Checked = true;
                        break;

                    case DWord dWord:
                        textBoxAddress.Text = dWord.Address;
                        radioButtonDWord.Checked = true;
                        break;

                    case WordBlock wordBlock:
                        textBoxAddress.Text = wordBlock.Address;
                        textBoxLength.Text = wordBlock.Length.ToString();
                        radioButtonWordBlock.Checked = true;
                        break;
                }
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var rowIndex = e.RowIndex;
                var row = dataGridView1.Rows[rowIndex];
                var s = row.Cells[nameof(FindReuslt.Time)].Value.ToString();
                var dateTime = DateTime.ParseExact(s, "HH:mm:ss.fffff", null);

                _frmMainView.FindPointByDateTime(dateTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
            }
        }

        private async void buttonExportVibration_Click(object sender, EventArgs e)
        {
            buttonFind.Enabled = false;
            TopMost = false;
            radioButtonWord.Checked = true;
            radioButtonDecimal.Checked = true;
            radioButtonNotEquals.Checked = true;
            textBoxEqualsValue.Text = "0";

            string address;
            bool NeedCheckNegative = true;
            string fileName;

            try
            {
                var dataView = _fReader.GetDataView();                
                IDataType signal = null;

                address = _config.LogViewer.VibrationX;
                signal = new Word(dataView, address);
                var reuslt = await SearchResults(dataView, signal, NeedCheckNegative);
                fileName = "Vibration_X_" + _fileTime;                
                ExportResult(reuslt, fileName);

                address = _config.LogViewer.VibrationY;
                signal = new Word(dataView, address);
                reuslt = await SearchResults(dataView, signal, NeedCheckNegative);
                fileName = "Vibration_Y_" + _fileTime;
                ExportResult(reuslt, fileName);

                address = _config.LogViewer.VibrationZ;
                signal = new Word(dataView, address);
                reuslt = await SearchResults(dataView, signal, NeedCheckNegative);
                fileName = "Vibration_Z_" + _fileTime;
                ExportResult(reuslt, fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "Export vibration value fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonFind.Enabled = true;
                TopMost = true;
                MessageBox.Show("The exported file is stored on your desktop.", "Export vibration value succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private int checkNegative(int value)
        {
            if (value > 32767)
                value = value - 65536;
            return value;
        }
    }
}
