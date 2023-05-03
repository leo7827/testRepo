using System;
using Mirle.Def;
using System.Data; 
using Mirle.Structure;
using Mirle.DataBase;

namespace Mirle.DB.Fun
{
    public class clsCmd_Mst_His
    {
        public bool FunGetCmdBetweenTime(DateTime startTime, DateTime endTime, string eqpId, ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                var strSql = $"select * from CMD_MST_His where ((CrtDate between '{startTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}' and '{endTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}')" +
                    $" or (EndDate between '{startTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}' and '{endTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'))" +
                    $" and EquNO = '{eqpId}'" + 
                    $" order by HisDT asc";
                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success) return true;
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }

        }

        public bool FunGetPortCmdBetweenTime(DateTime startTime, DateTime endTime, string portId, ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                var strSql = $"select * from CMD_MST_His where ((CrtDate between '{startTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}' and '{endTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}')" +
                    $" or (EndDate between '{startTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}' and '{endTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'))" +
                    $" and StnNo = '{portId}'" +
                    $" order by HisDT asc";
                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success) return true;
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }

        }
    }
}
