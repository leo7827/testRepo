using Mirle.DataBase;
using Mirle.Def;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.DB.Fun
{
    public class clsEQ_Alarm
    {

        public bool FunUpdEQ_Alarm(int cvBuffer, string alarm, clsEnum.AlarmSts alarmSts,DataBase.DB db)
        {
            try
            {
                var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var update = $"UPDATE EQ_Alarm SET EndDT='{now}', Flag = 'Y' " +
                    $"WHERE CVNO = '{cvBuffer}' AND Alarm = '{alarm}' AND Flag = 'N'";
                string strEM = "";
                if (db.ExecuteSQL(update, ref strEM) == DBResult.Success) return true;
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{update} => {strEM}");
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

        public bool FunInsEQ_Alarm(int cvBuffer, string ERRCODE, clsEnum.AlarmSts alarmSts,DataBase.DB db)
        {
            try
            {
                var sql = $"insert into EQ_Alarm(PLCNO,CVNO,ERRCODE,AlarmSts,StrDT,EndDT,Flag) values ( 0,'{cvBuffer}','{ERRCODE}' , " +
                    $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', ' ' , 'N')";
                string strEM = "";
                if (db.ExecuteSQL(sql, ref strEM) == DBResult.Success) return true;
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{sql} => {strEM}");
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
