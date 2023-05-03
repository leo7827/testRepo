namespace Mirle.Stocker.R46YP320.Events
{
    public static class StockerEvents
    {
        public delegate void StockerEventHandler(object sender, StockerEventArgs args);

        public delegate void CraneEventHandler(object sender, CraneEventArgs args);

        public delegate void ForkEventHandler(object sender, ForkEventArgs args);

        public delegate void IOEventHandler(object sender, IOEventArgs args);

        public delegate void IOStageEventHandler(object sender, IOStageEventArgs args);

        public delegate void IOVehicleEventHandler(object sender, IOVehicleEventArgs args);

        public delegate void EQEventHandler(object sender, EQEventArgs args);

        public delegate void ReqAckEventHandler(object sender, ReqAckEventArgs args);

        public delegate void AlarmEventHandler(object sender, AlarmEventArgs args);
    }
}