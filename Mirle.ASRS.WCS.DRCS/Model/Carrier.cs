using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.WCS.DRCS.Model
{
    public class Carrier : ICarrier
    {
        public string CarrierId { get; }
        public ILocation Source { get; }
        public ILocation Destination { get; }
        public ILocation NextDestination { get; }
        public ILocation CurrentLocation { get; private set; }

        public Carrier(string carrierId, ILocation source, ILocation destination)
        {
            CarrierId = carrierId;
            Source = source;
            Destination = destination;
            CurrentLocation = source;
        }

        public Carrier(string carrierId, ILocation source, ILocation destination, ILocation nextDestination) : this(carrierId, source, destination)
        {
            NextDestination = nextDestination;
        }

        public void UpdateCurrentLocation(ILocation location)
        {
            CurrentLocation = location;
        }
    }
}
