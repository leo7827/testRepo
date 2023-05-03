using System;
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
    public class clsAlarm_Proc
    {
        private System.Timers.Timer timRead = new System.Timers.Timer();

        public clsAlarm_Proc()
        {
            timRead.Elapsed += new System.Timers.ElapsedEventHandler(timRead_Elapsed);
            timRead.Enabled = false; timRead.Interval = 500;
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

                int iFloor = 0;

                for (int i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        case 0:
                            iErrorCode = clsLiteOnCV.GetConveyorController_8F().Signal.ErrorCode.GetValue();
                            iErrorIndex = clsLiteOnCV.GetConveyorController_8F().Signal.ErrorIndex.GetValue();
                            iErrorStatus = clsLiteOnCV.GetConveyorController_8F().Signal.ErrorStatus.GetValue();
                            iErrorIndexPC = clsLiteOnCV.GetConveyorController_8F().Signal.Controller.ErrorIndex.GetValue();
                            iFloor = 0;
                            break;
                        case 1:
                            //iErrorCode = clsLiteOnCV.GetConveyorController_10F().Signal.ErrorCode.GetValue();
                            //iErrorIndex = clsLiteOnCV.GetConveyorController_10F().Signal.ErrorIndex.GetValue();
                            //iErrorStatus = clsLiteOnCV.GetConveyorController_10F().Signal.ErrorStatus.GetValue();
                            //iErrorIndexPC = clsLiteOnCV.GetConveyorController_10F().Signal.Controller.ErrorIndex.GetValue();
                            //iFloor = 1;
                            break;
                     
                        default:
                            //iErrorCode = clsLiteOnCV.GetConveyorController_Elevator().Signal.ErrorCode.GetValue();
                            //iErrorIndex = clsLiteOnCV.GetConveyorController_Elevator().Signal.ErrorIndex.GetValue();
                            //iErrorStatus = clsLiteOnCV.GetConveyorController_Elevator().Signal.ErrorStatus.GetValue();
                            //iErrorIndexPC = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.ErrorIndex.GetValue();
                            //iFloor = 2;
                            break;


                    }



                    if (iErrorIndex != 0 && iErrorIndex != iErrorIndexPC)
                    {

                        //呼叫API
                        ALARM_HAPPEN_REPORTInfo alarmReportInfo = new ALARM_HAPPEN_REPORTInfo()
                        {
                            jobId = "",
                            deviceId = "LIFT4C",
                            alarmCode = iErrorCode.ToString(),
                            alarmDef = "test",
                            bufferId = "",
                            status = (iErrorStatus == 1) ? "Happen" : "Clear",
                            happenTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                        };

                        clsWmsApi.GetApiProcess().GetAlarmReport().FunReport(alarmReportInfo);

                        //用buffer 去分類 , 待格式出來再做
                        switch (iFloor)
                        {
                            case 0:
                                clsLiteOnCV.GetConveyorController_8F().WriteErrorIndex(iErrorIndex);
                                break;
                            case 1:
                                clsLiteOnCV.GetConveyorController_10F().WriteErrorIndex(iErrorIndex);
                                break;
                            default:
                                clsLiteOnCV.GetConveyorController_Elevator().WriteErrorIndex(iErrorIndex);
                                break;
                        }


                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {alarmReportInfo.bufferId} <TransferReport> =>  <AlarmName> {alarmReportInfo.alarmDef} , <Status>{alarmReportInfo.status}");
                    }
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
