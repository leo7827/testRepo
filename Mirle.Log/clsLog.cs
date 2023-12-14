using System;

using Mirle.Logger;

namespace Mirle.WriLog
{
    public class clsLog
    {
        private string sFileName = "";
        private bool bIsByHour = false;
        public clsLog(string FileName, bool IsByHour)
        {
            sFileName = FileName;
            bIsByHour = IsByHour;
        }

        /// <summary>
        /// For LOG
        /// </summary>
        private Log gobjLog = new Log();
        /// <summary>
        /// 記錄Exception Try Catch
        /// </summary>
        /// <param name="strFunSubName">Function Sub Name</param>
        /// <param name="strMsg">Message</param>
        /// <remarks></remarks>
        public void subWriteExLog(string strFunSubName, string strMsg)
        {
            gobjLog.WriteLogFile($"{sFileName}_Exception.log", strFunSubName.PadRight(60, ' ') + ":" + strMsg);
        }

        /// <summary>
        /// Write Trace Log (周邊)
        /// </summary>
        /// <param name="sValue">Trace Msg</param>
        public void FunWriTraceLog_CV(string sValue)
        {
            try
            {
                if (bIsByHour)
                    gobjLog.WriteLogFile($"{sFileName}_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", sValue);
                else
                    gobjLog.WriteLogFile($"{sFileName}.log", sValue);
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
            }
        }
    }
}
