namespace Mirle.Stocker.R46YP320
{
    public static class StockerEnums
    {
        public enum StockerStatus
        {
            NONE = 0,
            RUN = 1,
            DOWN = 2,
            IDLE = 3,
            MAINTAIN = 4,
        }

        public enum CraneStatus
        {
            NONE = 0,
            WAITINGHOMEACTION = 1,
            HOMEACTION = 2,
            IDLE = 3,
            BUSY = 4,
            STOP = 5,
            MAINTAIN = 6,
            ESCAPE = 7,
            NOSTS = 8,
            Waiting = 9,
            ScheduleDown = 10,
            Failures = 11,
        }

        public enum IOPortStatus
        {
            NONE,
            ERROR,
            NORMAL,
        }

        public enum EQPortStatus
        {
            NONE,
            ERROR,
            NORMAL,
        }

        public enum IOPortVehicleStatus
        {
            NONE = 0,
            ACTIVE = 1,
            ERROR = 2,
            HOMEACTION = 3,
            IDLE = 4,
        }

        public enum PortLoadRequestStatus
        {
            None = 0,
            Load = 1,
            Unload = 2,
        }

        public enum IOPortDirection
        {
            None = 0,
            InMode = 1,
            OutMode = 2,
            ModeChanging = 3,
        }
    }
}