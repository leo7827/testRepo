using Config.Net;

namespace Mirle.MPLCViewer.Config
{
    public interface MPLCViewerINI
    {
        [Option(Alias = "LogViewer")]
        LogViewerConfig LogViewer { get; }
    }

    public interface LogViewerConfig
    {
        string DefaultDLL { get; set; }

        string ChartItemsCSV { get; set; }

        [Option(DefaultValue = 0)]
        int SearchVibrationEnable { get;}

        string VibrationX { get; set; }

        string VibrationY { get; set; }

        string VibrationZ { get; set; }
    }
}
