using System.Collections.Generic;
using LiveCharts.Defaults;
using Mirle.MPLC;
using Mirle.MPLC.DataType;

namespace Mirle.MPLCViewer.Model
{
    public class SeriesItem
    {
        public string Name { get; set; }
        public int OrderIndex { get; set; }
        public bool IsEnable { get; set; } = true;
        public List<DateTimePoint> Points { get; } = new List<DateTimePoint>();
        public IDataType DataType { get; set; }
    }
}
