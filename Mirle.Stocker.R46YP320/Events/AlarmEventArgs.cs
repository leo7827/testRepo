using System;

namespace Mirle.Stocker.R46YP320.Events
{
    public class AlarmEventArgs : EventArgs
    {
        public int AlarmIndex { get; }
        public int AlarmCode { get; }

        public AlarmEventArgs(int alarmIndex, int alarmCode)
        {
            AlarmIndex = alarmIndex;
            AlarmCode = alarmCode;
        }
    }
}