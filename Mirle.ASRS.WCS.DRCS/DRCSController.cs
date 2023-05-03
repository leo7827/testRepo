using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirle.ASRS.WCS.DRCS.Define;
using Mirle.ASRS.WCS.DRCS.Manager;
using Mirle.ASRS.WCS.DRCS.Model;
using Mirle.ASRS.WCS.DRCS.Service;

namespace Mirle.ASRS.WCS.DRCS
{
    public class DRCSController
    {
        private readonly Dictionary<string, IDevice> _Devices = new Dictionary<string, IDevice>();

        private readonly RouteService _RouteService;

        public IEnumerable<IDevice> Devices => _Devices.Values;

        public DRCSController()
        {
            _RouteService = new RouteService(new MessageService());
        }

        public void AddDevice(IEnumerable<IDevice> devices)
        {
            foreach (var value in devices)
            {
                _Devices.Add(value.DeviceID, value);
            }
        }

        public bool ContainsDevice(string deviceId)
        {
            return _Devices.ContainsKey(deviceId);
        }

        public bool TryGetDevice(string deviceId, out IDevice device)
        {
            return _Devices.TryGetValue(deviceId, out device);
        }
    }
}
