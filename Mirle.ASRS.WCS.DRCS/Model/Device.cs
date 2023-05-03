using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirle.ASRS.WCS.DRCS.Define;

namespace Mirle.ASRS.WCS.DRCS.Model
{
    public class Device : IDevice
    {
        private readonly Dictionary<string, IShelf> _Shelfs = new Dictionary<string, IShelf>();
        private readonly Dictionary<string, IPort> _Ports = new Dictionary<string, IPort>();

        public string DeviceID { get; }
        public DeviceTypes DeviceType { get; }
        public int DeviceTypeIndex { get; }
        public IEnumerable<IShelf> Shelfs => _Shelfs.Values;
        public IEnumerable<IPort> Ports => _Ports.Values;

        public Device(string deviceID, DeviceTypes deviceType, int deviceTypeIndex)
        {
            DeviceID = deviceID;
            DeviceType = deviceType;
            DeviceTypeIndex = deviceTypeIndex;
        }

        public void AddPort(IPort port)
        {
            _Ports.Add(port.LocationId, port);
        }

        public void AddShelf(IShelf shelf)
        {
            _Shelfs.Add(shelf.LocationId, shelf);
        }

        public bool TryGetPort(string hostPortId, out IPort port)
        {
            return _Ports.TryGetValue(hostPortId, out port);
        }

        public bool TryGetShelf(string shelfId, out IShelf shelf)
        {
            return _Shelfs.TryGetValue(shelfId, out shelf);
        }

        public bool ContainsPort(string hostPortId)
        {
            return _Ports.ContainsKey(hostPortId);
        }

        public bool ContainsShelf(string shelfId)
        {
            return _Shelfs.ContainsKey(shelfId);
        }
    }
}
