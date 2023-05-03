using Mirle.Def;
using Mirle.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.DB.Object.Table
{
    public class clsCmd_Mst
    {
        public static int FunCheckHasCommand(string sLoc, ref CmdMstInfo cmd) => clsDB_Proc.GetDB_Object().GetCmd_Mst().FunCheckHasCommand(sLoc, ref cmd);

        public static int FunGetTaskInfo_Grid(ref DataTable dtTmp) => clsDB_Proc.GetDB_Object().GetTask().FunGetTaskInfo_Grid(ref dtTmp);
        public static int FunCheckHasCommand(string sLoc, string sCmdSts, ref DataTable dtTmp) => clsDB_Proc.GetDB_Object().GetCmd_Mst().FunCheckHasCommand(sLoc, sCmdSts, ref dtTmp);
        public static int FunGetCommandCountByStockOut(int[] NotGoodID, string sStnNo, ref int iCount)
           => clsDB_Proc.GetDB_Object().GetCmd_Mst().FunGetCommandCountByStockOut(NotGoodID, sStnNo, ref iCount);
        public static int FunGetCmdMst_Grid(ref DataTable dtTmp) => clsDB_Proc.GetDB_Object().GetCmd_Mst().FunGetCmdMst_Grid(ref dtTmp);
      
        public static int FunFunGetPortTicketID(string sStnNo, ref string sTicketId) => clsDB_Proc.GetDB_Object().GetCmd_Mst().FunGetPortTicketID(sStnNo, ref sTicketId);
        public static bool FunUpdateRemark(string sCmdSno, string sRemark) => clsDB_Proc.GetDB_Object().GetCmd_Mst().FunUpdateRemark(sCmdSno, sRemark);
      
    }
}
