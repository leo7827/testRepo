using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.DB.Object
{
    public class clsManual
    {      
        public static bool FunManualCommandComplete(string sCmdSno, ref string strEM) => clsDB_Proc.GetDB_Object().GetProcess().FunManualCommandComplete(sCmdSno, ref strEM);
        public static bool FunManualRepeatCmd(string sCmdSno, ref string strEM) => clsDB_Proc.GetDB_Object().GetProcess().FunManualRepeatCmd(sCmdSno, ref strEM);
       
    }
}
