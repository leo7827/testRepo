namespace Mirle.Stocker.R46YP320
{
    public static class LCSEnums
    {
        public enum ControlMode
        {
            Single = 0,
            DoubleSingle = 1,
            Dual = 2,
            TwinForkDual = 3,
            TwinFork = 4,
        }

        public enum AvailStatus
        {
            Avail = 0,
            NotAvail = 1,
            None = 3,
        }

        public enum LCSMode
        {
            STKC = 1,
            RGV = 2,
            MultiSTK = 3,
        }
    }
}