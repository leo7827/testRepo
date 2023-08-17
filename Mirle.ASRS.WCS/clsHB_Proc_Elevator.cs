using System;
using System.Linq;
using Mirle.Def;
using Mirle.Def.V2BYMA30;
using Mirle.DB.Object;
using Mirle.Structure;
using Mirle.LiteOn.V2BYMA30;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using System.Collections.Generic;
using Mirle.DB.Object.Table;
using Mirle.DataBase;
using System.Threading;


namespace Mirle.ASRS.WCS
{
    public class clsHB_Proc_Elevator
    {
        private System.Timers.Timer timRead = new System.Timers.Timer();
        public bool bFlag_Happen = false;  // 
        public bool bFlag_Clear = true;  // 

        public clsHB_Proc_Elevator()
        {
            timRead.Elapsed += new System.Timers.ElapsedEventHandler(timRead_Elapsed);
            timRead.Enabled = false; timRead.Interval = 1000;
        }
        public void subStart()
        {
            timRead.Enabled = true;
        }
        private void timRead_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            timRead.Enabled = false;


            try
            {

                if (!clsLiteOnCV.GetConveyorController_Elevator().IsConnected && !bFlag_Happen)
                {

                    //呼叫API
                    ALARM_HAPPEN_REPORTInfo alarmReportInfo = new ALARM_HAPPEN_REPORTInfo()
                    {
                        jobId = "123",
                        deviceId = clsConstValue.deviceID.E04,
                        alarmCode = clsConstValue.AlarmCode.HeartBeat,  //代表通訊異常
                        alarmDef = "Lift4",
                        bufferId = "TEST",
                        status = "1",  //happen
                        happenTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    clsWmsApi.GetApiProcess().GetAlarmReport().FunReport(alarmReportInfo);

                    clsWriLog.Log.FunWriTraceLog_CV($"<DeviceId> {alarmReportInfo.deviceId} <AlarmReport> =>  <AlarmCode> {alarmReportInfo.alarmCode} , <Status>{alarmReportInfo.status}");

                    bFlag_Happen = true;
                    bFlag_Clear = false;

                    SpinWait.SpinUntil(() => false, 1000);
                    //Thread.Sleep(1000);
                }


                if (clsLiteOnCV.GetConveyorController_Elevator().IsConnected && !bFlag_Clear)
                {
                    //呼叫API
                    ALARM_HAPPEN_REPORTInfo alarmReportInfo = new ALARM_HAPPEN_REPORTInfo()
                    {
                        jobId = "123",
                        deviceId = clsConstValue.deviceID.E04,
                        alarmCode = clsConstValue.AlarmCode.HeartBeat,
                        alarmDef = "Lift4",
                        bufferId = "TEST",
                        status = "0",  //clear
                        happenTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    clsWmsApi.GetApiProcess().GetAlarmReport().FunReport(alarmReportInfo);

                    clsWriLog.Log.FunWriTraceLog_CV($"<DeviceId> {alarmReportInfo.deviceId} <AlarmReport> =>  <AlarmCode> {alarmReportInfo.alarmCode} , <Status>{alarmReportInfo.status}");

                    bFlag_Happen = false;
                    bFlag_Clear = true;

                    SpinWait.SpinUntil(() => false, 1000);
                }

            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                timRead.Enabled = true;
            }
        }

    }
}
