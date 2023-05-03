using System;
using Mirle.Structure.Info;

namespace Mirle.R46YP320.STK.DataCollectionEventArgs
{
    public class UnitAlarmEventArgs : EventArgs
    {
        public UnitAlarmEventArgs(int alarmID, string alarmText, string stockerUnitID, VIDEnums.MainteState mainteState)
        {
            this.AlarmID = alarmID;
            this.AlarmText = alarmText;
            this.StockerUnitID = stockerUnitID;
            this.MainteState = mainteState;
        }

        public int AlarmID { get; }
        public string AlarmText { get; }
        public string StockerUnitID { get; }
        public VIDEnums.MainteState MainteState { get; }
    }
}