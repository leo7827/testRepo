using System;
using System.Collections.Generic;
using System.Linq;
using Mirle.Def;
using Mirle.DataBase;

namespace Mirle.DB.WMS.Proc
{
    public class clsLocMst
    {
        private WMS.Fun.clsLocMst LocMst = new WMS.Fun.clsLocMst();
        private clsDbConfig _config = new clsDbConfig();
        public clsLocMst(clsDbConfig config)
        {
            _config = config;
        }

      
      
    }
}
