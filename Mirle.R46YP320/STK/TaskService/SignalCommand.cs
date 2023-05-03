using Mirle.DataBase;
using Mirle.Stocker;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.Module;
using Mirle.Stocker.TaskControl.TraceLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.R46YP320.STK.TaskService
{
    public class SignalCommand
    {
        public SignalCommand()
        {
        }

        public DaifukuSpec.ACK.HCACK Signal()
        {
            return DaifukuSpec.ACK.HCACK.Acknowledge;
        }
    }
}
