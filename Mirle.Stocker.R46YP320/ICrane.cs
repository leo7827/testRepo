using Mirle.Stocker.R46YP320.Events;
using System.Collections.Generic;

namespace Mirle.Stocker.R46YP320
{
    public interface ICrane
    {
        event StockerEvents.CraneEventHandler OnAvailStatusChanged;

        event StockerEvents.CraneEventHandler OnLocationUpdated;

        event StockerEvents.CraneEventHandler OnStatusChanged;

        event StockerEvents.CraneEventHandler OnTaskCmdWriteToMPLC;

        int Speed { get; set; }
        int CurrentBank { get; }
        int CurrentBay { get; }
        int CurrentLevel { get; }
        int CurrentPosition { get; }
        IEnumerable<IFork> Forks { get; }
        int Id { get; }
        bool IsExecutingHPReturn { get; }

        bool IsHandOffReserved { get; }

        bool IsIdle { get; }

        bool IsInService { get; }

        bool IsKeySwitchIsAuto { get; }

        bool IsSingleCraneMode { get; }

        bool IsAlarm { get; }

        bool CanAutoSetRun { get; set; }

        bool ReadyRecieveNewCommand { get; }

        string Location { get; }

        /// <summary>
        /// 兩隻Fork都荷有
        /// </summary>
        bool TwinForkIsLoad { get; }

        StockerEnums.CraneStatus Status { get; }

        IFork GetForkById(int id);

        IFork GetLeftFork();

        IFork GetRightFork();
    }
}