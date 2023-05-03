using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirle.Def;

namespace Mirle.ASRS.Conveyor.V2BYMA30_3F.Events
{
    public class BufferEventArgs : EventArgs
    {
        public int BufferIndex { get; }
        public clsEnum.WmsApi.EqSts NewStatus { get; set; }
        public bool Presence { get; set; }

        public BufferEventArgs(int bufferIndex)
        {
            BufferIndex = bufferIndex;
        }
    }
}
