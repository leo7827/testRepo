﻿using System;
using System.Linq;
using Mirle.Def;
using Mirle.Def.V2BYMA30;
using Mirle.DB.Object;
using Mirle.Structure;
using Mirle.LiteOn.V2BYMA30;
using Mirle.DataBase;
using Mirle.DB.Object.Table;
using Mirle.WebAPI.V2BYMA30.ReportInfo;

namespace Mirle.ASRS.WCS
{
    public class clsAlarm_Proc_8F
    {
        private System.Timers.Timer timRead = new System.Timers.Timer();

        public clsAlarm_Proc_8F()
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
                int iErrorCode = 0;
                int iErrorIndex = 0;
                int iErrorStatus = 0;
                int iErrorIndexPC = 0;

                iErrorCode = clsLiteOnCV.GetConveyorController_8F().Signal.ErrorCode.GetValue();
                iErrorIndex = clsLiteOnCV.GetConveyorController_8F().Signal.ErrorIndex.GetValue();
                iErrorStatus = clsLiteOnCV.GetConveyorController_8F().Signal.ErrorStatus.GetValue();
                iErrorIndexPC = clsLiteOnCV.GetConveyorController_8F().Signal.Controller.ErrorIndex.GetValue();  

                if (iErrorIndex != 0 && iErrorIndex != iErrorIndexPC)
                {
                    //呼叫API
                    ALARM_HAPPEN_REPORTInfo alarmReportInfo = new ALARM_HAPPEN_REPORTInfo()
                    {
                        jobId = "123",
                        deviceId = clsConstValue.deviceID.E04_8F,
                        alarmCode = iErrorCode.ToString(),
                        alarmDef = "8F",
                        bufferId = "test",
                        status = (iErrorStatus == 1) ? "1" : "0",
                        happenTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    clsWmsApi.GetApiProcess().GetAlarmReport().FunReport(alarmReportInfo);

                    clsLiteOnCV.GetConveyorController_8F().WriteErrorIndex(iErrorIndex); 
            
                    clsWriLog.Log.FunWriTraceLog_CV($"<DeviceId> {alarmReportInfo.deviceId} <AlarmReport> =>  <AlarmCode> {alarmReportInfo.alarmCode} , <Status>{alarmReportInfo.status}");

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
