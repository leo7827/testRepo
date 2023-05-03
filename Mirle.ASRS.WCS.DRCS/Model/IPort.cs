using Mirle.ASRS.WCS.DRCS.Define;

namespace Mirle.ASRS.WCS.DRCS.Model
{
    public interface IPort : ILocation
    {
        string HostPortID { get; }
        int PLCPortID { get; }
        PortTypes PortType { get; }
        int PortTypeIndex { get; }
    }
}