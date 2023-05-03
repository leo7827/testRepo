using Mirle.ASRS.WCS.DRCS.Define;

namespace Mirle.ASRS.WCS.DRCS.Model
{
    public class Shelf : IShelf
    {
        public string DeviceID { get; }
        public string LocationId { get; }
        public string ShelfId { get; }
        public LocationTypes LocationTypes => LocationTypes.Shelf;

        public Shelf(string deviceID, string locatioId, string shelfId)
        {
            DeviceID = deviceID;
            LocationId = locatioId;
            ShelfId = shelfId;
        }
    }
}
