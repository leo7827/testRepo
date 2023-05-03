﻿using Mirle.Route;

namespace Mirle.Def
{
    public class Location : ILocation
    {
        public Location(string deviceID, string locationId, LocationTypes locationTypes)
        {
            DeviceId = deviceID;
            LocationId = locationId;
            ShelfId = locationId;
            LocationTypes = locationTypes;
        }

        public string DeviceId { get;  }
        public string ShelfId { get; }
        public string LocationId { get; }
        public int Stage { get; } = 1;
        public LocationTypes LocationTypes { get; }

        public static LocationTypes GetLocationTypesByPortType(int PortType)
        {
            switch (PortType)
            {
                case (int)LocationTypes.Conveyor:
                    return LocationTypes.Conveyor;
                case (int)LocationTypes.EQ:
                    return LocationTypes.EQ;
                case (int)LocationTypes.IO:
                    return LocationTypes.IO;
                case (int)LocationTypes.Shelf:
                    return LocationTypes.Shelf;
                default:
                    return LocationTypes.Stocker;
            }
        }

        public static void AddPath_Double(RouteController routeService, Location loc1, Location loc2)
        {
            routeService.AddPath(loc1, loc2);
            routeService.AddPath(loc2, loc1);
        }

        public static void AddPath_Single(RouteController routeService, Location loc1, Location loc2)
        {
            routeService.AddPath(loc1, loc2);
        }
    }
}
