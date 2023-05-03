using System;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class AlarmReportEventArgs : EventArgs
    {
        public AlarmReportEventArgs(bool isAlarmSet, int alcd, int alid, string altx)
        {
            this.IsAlarmSet = isAlarmSet;
            this.ALCD = alcd;
            this.ALID = alid;
            this.ALTX = altx;
        }

        public bool IsAlarmSet { get; }
        public int ALCD { get; }
        public int ALID { get; }
        public string ALTX { get; }
    }
}