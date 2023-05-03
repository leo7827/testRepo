using Mirle.ASRS.WCS.DRCS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.WCS.DRCS.Service.Message
{
    public class RouteServiceTrace : LogTrace
    {
        public override string FileName => "RouteService";

        public RouteServiceTrace(string message, ILocation node,
        {
            _Message = $"{message}|{node.DeviceID}-{node.LocationId}";
        }

        public RouteServiceTrace(string message, ILocation node, ILocation nextLocation)
        {
            _Message = $"{message}|{node.DeviceID}-{node.LocationId}=>{nextLocation.DeviceID}-{nextLocation.LocationId}";
        }

        public RouteServiceTrace(string message, ILocation node, ILocation nextLocation, uint cost)
        {
            _Message = $"{message}|{node.DeviceID}-{node.LocationId}=>{nextLocation.DeviceID}-{nextLocation.LocationId}|{cost}";
        }
    }
}
