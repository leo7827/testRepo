using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.Conveyor.Controller
{
    public class CVCHost
    {
        private readonly CVCManager_3F _cvCManager_3F;
        private readonly CVCManager_5F _cvCManager_5F;

        public CVCHost(ConveyorConfig config3F, ConveyorConfig config5F)
        {
            _cvCManager_3F = new CVCManager_3F(config3F);
            _cvCManager_5F = new CVCManager_5F(config5F);
        }


        public CVCManager_3F GetCVCManager_3F()
        {
            return _cvCManager_3F;
        }

        public CVCManager_5F GetCVCManager_5F()
        {
            return _cvCManager_5F;
        }
    }
}
