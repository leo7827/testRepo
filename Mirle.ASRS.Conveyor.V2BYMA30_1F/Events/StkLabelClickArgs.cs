using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.Events
{
    public class StkLabelClickArgs : EventArgs
    {
        public int StockerID { get; }

        public StkLabelClickArgs(int ID)
        {
            StockerID = ID;
        }
    }
}
