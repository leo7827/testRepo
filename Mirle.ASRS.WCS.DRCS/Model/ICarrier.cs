namespace Mirle.ASRS.WCS.DRCS.Model
{
    public interface ICarrier
    {
        string CarrierId { get; }
        ILocation CurrentLocation { get; }

        void UpdateCurrentLocation(ILocation location);
    }
}