using System;

namespace Mirle.Stocker.R46YP320.Events
{
    public class IOStageEventArgs : EventArgs
    {
        public int IOPortId { get; }
        public int StageId { get; }
        public bool SignalIsOn { get; set; }
        public string CstId { get; set; }
        public string BoxId { get; set; }

        public IOStageEventArgs(int stageId, int ioPortId)
        {
            IOPortId = ioPortId;
            StageId = stageId;
        }
    }
}