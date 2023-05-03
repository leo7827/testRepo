using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using CsvHelper;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using Mirle.MPLC.FileData;
using Mirle.MPLC.DataType;
using Mirle.MPLCViewer.Config;
using Mirle.MPLCViewer.Model;
using Mirle.MPLCViewer.Model.CsvFile;
using Mirle.Extensions;

namespace Mirle.MPLCViewer.View
{
    public partial class frmChartView : Form
    {
        private readonly MPLCViewerINI _config;
        private readonly FileReader _fReader;
        private FileDataViewer _dataView;
        private int progressCount = 0;
        private Dictionary<string, List<ChartItem>> _chartItemsMap;

        [Obsolete]
        public frmChartView(MPLCViewerINI config, FileReader fReader)
        {
            _config = config;
            _fReader = fReader;
            InitializeComponent();
            tableLayoutPanel1.GetType()
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(tableLayoutPanel1, true, null);
        }

        private void dateTimePickerBegin_ValueChanged(object sender, EventArgs e)
        {
            //if (dateTimePickerEnd.Value < dateTimePickerBegin.Value)
            //{
            //    dateTimePickerEnd.Value = dateTimePickerBegin.Value;
            //}
        }

        private void dateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            //if (dateTimePickerEnd.Value < dateTimePickerBegin.Value)
            //{
            //    dateTimePickerBegin.Value = dateTimePickerEnd.Value;
            //}
        }

        private void frmChartView_Load(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            dateTimePickerBegin.Value = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            dateTimePickerEnd.Value = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            try
            {
                using (var reader = new StreamReader($@".\Config\{_config.LogViewer.ChartItemsCSV}"))
                {
                    IEnumerable<ChartItem> chartItems;
                    using (var csv = new CsvReader(reader))
                    {
                        chartItems = csv.GetRecords<ChartItem>().ToList();
                    }

                    if (chartItems == null || !chartItems.Any()) throw new InvalidOperationException("No Any Chart Item!!");

                    _chartItemsMap = chartItems.GroupBy(i => i.Title).ToDictionary(i => i.Key, i => i.ToList());

                    foreach (var key in _chartItemsMap.Keys)
                    {
                        listBoxItems.Items.Add(key);
                    }
                    if (listBoxItems.Items.Count > 0)
                    {
                        listBoxItems.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chart Items Loading fail!\n{ex.Message}\n{ex.StackTrace}", "Loading Chart Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        [Obsolete]
        private void SetChartAxis(List<SeriesItem> items)
        {
            try
            {
                var labels = new List<string>();
                labels.Add("");
                foreach (var seriesItem in items.OrderByDescending(i => i.OrderIndex))
                {
                    labels.Add(seriesItem.Name);
                    labels.Add("");
                }
                var sectionValue = items.Max(i => i.OrderIndex);

                cartesianChart1.DisableAnimations = true;
                cartesianChart1.Hoverable = false;
                cartesianChart1.DataTooltip = null;
                cartesianChart1.Background = System.Windows.Media.Brushes.White;
                cartesianChart1.Zoom = ZoomingOptions.X;

                cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis()
                {
                    Position = AxisPosition.RightTop,
                    Separator = new Separator { IsEnabled = false },
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.Black,
                    LabelFormatter = val => new System.DateTime((long)val).ToString("HH:mm:ss.fff"),
                });

                cartesianChart1.AxisY.Add(new LiveCharts.Wpf.Axis()
                {
                    Position = AxisPosition.LeftBottom,
                    Separator = new Separator { Step = 1, IsEnabled = true, },
                    FontSize = 16,
                    Foreground = System.Windows.Media.Brushes.Black,
                    //Labels = new[]
                    //{
                    //    "",
                    //    "Online", "",
                    //    "UC_REQ", "",
                    //    "LC_REQ", "",
                    //    "CARRIER", "",
                    //    "Ready", "",
                    //    "U_REQ", "",
                    //    "L_REQ", "",
                    //    "EStop_FromSTK", "",
                    //    "CRANE_2_FromSTK", "",
                    //    "CRANE_1_FromSTK", "",
                    //    "COMPLETE_FromSTK", "",
                    //    "BUSY_FromSTK", "",
                    //    "TR_REQ_FromSTK", "",
                    //    "VALID_FromSTK", "",
                    //},
                    Labels = labels,
                    Sections = new SectionsCollection()
                    {
                        new AxisSection
                        {
                            Label = "A",
                            Value = 0,
                            SectionWidth = sectionValue % 2 == 0 ? sectionValue + 2 : sectionValue + 1, // 16,
                            Fill = new SolidColorBrush
                            {
                                Color = System.Windows.Media.Color.FromRgb(131, 111, 255), Opacity = .2
                            }
                        },
                        new AxisSection
                        {
                            Label = "B",
                            Value = sectionValue % 2 == 0 ? sectionValue + 3 : sectionValue + 2, // 17,
                            SectionWidth = sectionValue + 2, //16,
                            Fill = new SolidColorBrush
                            {
                                Color = System.Windows.Media.Color.FromRgb(255, 182, 193), Opacity = .2
                            }
                        },
                    },
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
            }
        }

        [Obsolete]
        private async void buttonQuery_Click(object sender, EventArgs e)
        {
            try
            {
                buttonQuery.Enabled = false;
                _dataView = _fReader.GetDataView();
                //var ioIndex = comboBox1.SelectedIndex;
                //var ioAddr = 6461 + 112 * ioIndex;

                //var items = new List<SeriesItem>()
                //{
                //    new SeriesItem(){Name = "L_REQ", OrderIndex = 0, DataType = new Bit(_dataView, $"D{ioAddr}.0")},
                //    new SeriesItem(){Name = "U_REQ", OrderIndex = 1, DataType = new Bit(_dataView, $"D{ioAddr}.1")},
                //    new SeriesItem(){Name = "Ready", OrderIndex = 2, DataType = new Bit(_dataView, $"D{ioAddr}.2")},
                //    new SeriesItem(){Name = "CARRIER", OrderIndex = 3, DataType = new Bit(_dataView, $"D{ioAddr}.3")},
                //    new SeriesItem(){Name = "LC_REQ", OrderIndex = 4, DataType = new Bit(_dataView, $"D{ioAddr}.4")},
                //    new SeriesItem(){Name = "UC_REQ", OrderIndex = 5, DataType = new Bit(_dataView, $"D{ioAddr}.5")},
                //    new SeriesItem(){Name = "Online", OrderIndex = 6, DataType = new Bit(_dataView, $"D{ioAddr}.6")},
                //    new SeriesItem(){Name = "EStop", OrderIndex = 7, DataType = new Bit(_dataView, $"D{ioAddr}.7")},

                //    new SeriesItem(){Name = "VALID_FromSTK", OrderIndex = 8, DataType = new Bit(_dataView, $"D{ioAddr}.8")},
                //    new SeriesItem(){Name = "TR_REQ_FromSTK", OrderIndex = 9, DataType = new Bit(_dataView, $"D{ioAddr}.9")},
                //    new SeriesItem(){Name = "BUSY_FromSTK", OrderIndex = 10, DataType = new Bit(_dataView, $"D{ioAddr}.A")},
                //    new SeriesItem(){Name = "COMPLETE_FromSTK", OrderIndex = 11, DataType = new Bit(_dataView, $"D{ioAddr}.B")},
                //    new SeriesItem(){Name = "CRANE_1_FromSTK", OrderIndex = 12, DataType = new Bit(_dataView, $"D{ioAddr}.C")},
                //    new SeriesItem(){Name = "CRANE_2_FromSTK", OrderIndex = 13, DataType = new Bit(_dataView, $"D{ioAddr}.D")},
                //    new SeriesItem(){Name = "EStop_FromSTK", OrderIndex = 14, DataType = new Bit(_dataView, $"D{ioAddr}.E")},
                //};

                _chartItemsMap.TryGetValue(listBoxItems.SelectedItem.ToString(), out var chartItems);
                if (chartItems == null)
                {
                    MessageBox.Show($"Please Select ChartItem", "Query", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var items = new List<SeriesItem>();
                foreach (var chartItem in chartItems)
                {
                    IDataType dataType;
                    switch (chartItem.DataType.ToUpper())
                    {
                        case "BIT":
                            dataType = new Bit(_dataView, chartItem.Address);
                            break;

                        case "WORD":
                            dataType = new Word(_dataView, chartItem.Address);
                            break;

                        case "DWORD":
                            dataType = new DWord(_dataView, chartItem.Address);
                            break;

                        case "WORDBLOCK":
                            var length = chartItem.Length > 0 ? chartItem.Length : 1;
                            dataType = new WordBlock(_dataView, chartItem.Address, length);
                            break;

                        default:
                            continue;
                    }

                    items.Add(new SeriesItem()
                    {
                        Name = chartItem.Name,
                        OrderIndex = chartItem.OrderIndex,
                        DataType = dataType,
                    });
                }

                var maxIndex = items.Max(i => i.OrderIndex) * 2 + 1;

                progressCount = 0;
                timer1.Enabled = true;
                if (dateTimePickerEnd.Value < dateTimePickerBegin.Value)
                {
                    dateTimePickerEnd.Value = dateTimePickerBegin.Value.AddHours(1);
                }

                var stopwatch = Stopwatch.StartNew();

                await Task.Run(() =>
                {
                    var points = _dataView.Query(dateTimePickerBegin.Value, dateTimePickerEnd.Value).ToList();
                    var current = 0;
                    foreach (var point in points)
                    {
                        current++;
                        _dataView.RefreshRawData(point);

                        double signalValue = 0;
                        foreach (var seriesItem in items)
                        {
                            switch (seriesItem.DataType)
                            {
                                case Bit bit:
                                    signalValue = bit.IsOn() ? 1 : 0;
                                    break;

                                case Word word:
                                    signalValue = word.Value == 0 ? 0 : 1;
                                    break;

                                case DWord dWord:
                                    signalValue = dWord.Value == 0 ? 0 : 1;
                                    break;

                                case WordBlock wordBlock:
                                    signalValue = string.IsNullOrEmpty(wordBlock.GetData().ToASCII()) ? 0 : 1;
                                    break;
                            }

                            seriesItem.Points.Add(
                                new DateTimePoint(point, signalValue + maxIndex - seriesItem.OrderIndex * 2));
                        }

                        progressCount = 100 * current / points.Count;
                    }
                });
                stopwatch.Stop();
                Debug.WriteLine("Query: " + stopwatch.ElapsedMilliseconds);

                cartesianChart1.AxisX.Clear();
                cartesianChart1.AxisY.Clear();
                SetChartAxis(items);

                cartesianChart1.Series.Clear();
                foreach (var seriesItem in items)
                {
                    GetSeries(seriesItem.Name, seriesItem.Points);
                }

                progressBar1.Value = 100;
                timer1.Enabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
            }

            buttonQuery.Enabled = true;
        }

        private void GetSeries(string title, List<DateTimePoint> dateTimePoints)
        {
            cartesianChart1.Series.Add(new GStepLineSeries()
            {
                Title = title,
                Values = dateTimePoints.AsGearedValues().WithQuality(Quality.Low),
                PointGeometry = DefaultGeometries.None,
                PointForeground = System.Windows.Media.Brushes.Blue,
            });
        }

        private void TimeChartView_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Hide();
            //e.Cancel = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = progressCount;
        }
    }
}
