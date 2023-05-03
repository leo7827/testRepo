using Mirle.DataBase;
using Mirle.Def;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.DB.Proc
{
    public class clsL2LCount
    {
        private Fun.clsL2LCount L2LCount = new Fun.clsL2LCount();
        private clsDbConfig _config = new clsDbConfig();
        public clsL2LCount(clsDbConfig config)
        {
            _config = config;
        }

      　
    }
}
