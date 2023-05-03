using System.Collections.Generic;
using Mirle.ASRS.WCS.DRCS.Define;

namespace Mirle.ASRS.WCS.DRCS.Model
{
    public interface IDevice
    {
        string DeviceID { get; }
        DeviceTypes DeviceType { get; }
        int DeviceTypeIndex { get; }
        IEnumerable<IPort> Ports { get; }
        IEnumerable<IShelf> Shelfs { get; }

        void AddPort(IPort port);
        void AddShelf(IShelf shelf);
        bool ContainsPort(string hostPortId);
        bool ContainsShelf(string shelfId);
        bool TryGetPort(string hostPortId, out IPort port);
        bool TryGetShelf(string shelfId, out IShelf shelf);
    }
}