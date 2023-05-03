namespace Mirle.R46YP320.STK.TaskService
{
    public class Carrier
    {
        public Carrier(string carrierId)
        {
            CarrierId = carrierId;
        }

        public string CarrierId { get; private set; }

        public bool IsUnknown()
        {
            return CarrierId.StartsWith("UNKNOWN");
        }

        public bool IsUnknownDuplicate()
        {
            return CarrierId.StartsWith("UNKNOWNDUP");
        }
    }
}
