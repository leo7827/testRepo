using Mirle.Def;
using Mirle.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.DB.Object.Table
{
    public class clsTask
    {
        
        public static int CheckHasTaskCmd(string CommandID, ref TaskInfo task) => clsDB_Proc.GetDB_Object().GetTask().CheckHasTaskCmd(CommandID, ref task);
       
        public static bool FunInsertTaskCmd(string CommandID, string DeviceID, clsEnum.TaskMode mode, string sFrom, string sTo, int ForkNo)
        {
            return true;
            //return clsDB_Proc.GetDB_Object().GetTask().FunInsertTaskCmd(CommandID, DeviceID, mode, sFrom, sTo, ForkNo);
        }
        public static bool FunUpdateTaskCmd(string sCmdSno, string sSts, string sCurLoc, string sRemark, ref string strErrMsg) => clsDB_Proc.GetDB_Object().GetTask().FunUpdateTaskCmd(sCmdSno, sSts, sCurLoc, sRemark, ref strErrMsg);

        public static bool FunDeleteTask(string sCmdSno) => clsDB_Proc.GetDB_Object().GetTask().FunDeleteTask(sCmdSno);
        public static bool FunUpdateTaskCurrLoc(string sCmdSno, string sCurrLoc) => clsDB_Proc.GetDB_Object().GetTask().FunUpdateTaskCurrLoc(sCmdSno, sCurrLoc);

        public static bool FunInsTaskHisAndDle(string sCmdSno) => clsDB_Proc.GetDB_Object().GetTask().FunDelTaskCmd_Proc(sCmdSno);

        /// <summary>
        /// 更新入庫之目的
        /// </summary>
        /// <param name="sCmdSno"></param>
        /// <param name="sBuffer"></param>
        /// <returns></returns>
        public static bool FunUpdateTaskCmdDes(string sCmdSno, string sBuffer) => clsDB_Proc.GetDB_Object().GetTask().FunUpdateTaskCmdDes(sCmdSno, sBuffer);


        public static bool FunUpdateTaskPrty( string sDes , string sCmdSno) => clsDB_Proc.GetDB_Object().GetTask().FunUpdateTaskPrty(sDes , sCmdSno);
    }
}
