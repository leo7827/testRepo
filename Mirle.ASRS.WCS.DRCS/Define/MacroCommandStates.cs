using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.WCS.DRCS.Define
{
    public enum MacroCommandStates
    {
        Queue = 0,
        Initialize = 1,
        Transferring = 2,
        Resume = 3,
        TransferComplete = 4,
    }
}
