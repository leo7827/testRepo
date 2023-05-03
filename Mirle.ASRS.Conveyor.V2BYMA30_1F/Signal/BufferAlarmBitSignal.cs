using Mirle.MPLC.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.Signal
{
    public class BufferAlarmBitSignal
    {
        public BitSignal[] AlarmBit = new BitSignal[16];
        public BufferAlarmBitSignal()
        {
            for (int i = 0; i < AlarmBit.Length; i++)
            {
                AlarmBit[i] = new BitSignal();
            }
        }
    }
}
