using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.Conveyor.V2BYMA30_8F.Events
{
    public class AlarmEventArgs : EventArgs
    {
        public bool Signal { get; }
        public AlarmEventArgs(bool signal)
        {
            Signal = signal;
        }
    }
}
