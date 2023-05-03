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
    public class clsPositionReport8F_Proc
    {
        private System.Timers.Timer timRead = new System.Timers.Timer();
        public clsPositionReport8F_Proc()
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
                int CvIdxTotal = clsLiteOnCV.GetConveyorController_8F().BufferCount;

                for (int i = 0; i < CvIdxTotal; i++)
                {
                    if (clsLiteOnCV.GetConveyorController_8F().IsConnected)
                    {
                        ConveyorInfo buffer = i switch
                        {
                            0 => ConveyorDef.LO2_01,
                            1 => ConveyorDef.LO2_02,
                            2 => ConveyorDef.LO2_03,

                            _ => ConveyorDef.LO2_04,

                        };

                        var cv = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                        if (cv.Presence && !string.IsNullOrEmpty(cv.CommandID) && !cv.TransferReportNotice_PC )
                        {
                            string sCmdSno = cv.CommandID;
                            string sBcr = cv.GetTrayID.Trim();

                            string sCurLoc = buffer.BufferName;

                            //應WCS 要求 , 這四節不要上報 transferreport
                            //if (i == 3)
                            //{
                            //    goto label;
                            //}

                            ////呼叫API
                            //POSITION_REPORTInfo POSITION_REPORTInfo = new POSITION_REPORTInfo()
                            //{
                            //    jobId = sCmdSno,
                            //    transactionId = "POSITION_REPORT",
                            //    carrierType = "BIN",
                            //    id = "",
                            //    position = sCurLoc
                            //}; 

                            //clsWmsApi.GetApiProcess().GetPositionReport().FunReport(POSITION_REPORTInfo);

                            //clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <PositionReport>  => 任務號 : ({sCmdSno})  ");

                            //label:
                            if (cv.WriteTransferReport(1) )
                            {
                                if (!clsTask.FunUpdateTaskCurrLoc(sCmdSno, sCurLoc))
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <任務號> {sCmdSno} => 命令更新失敗！ ");
                                    continue;
                                }
                                clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <任務號> {sCmdSno} => 命令更新成功！ ");
                            }

                            continue;

                        }
                        else if (!cv.Presence && string.IsNullOrEmpty(cv.CommandID) && cv.TransferReportNotice_PC  )
                        {
                            //清掉API上報
                            if (!cv.WriteTransferReport(0) )
                            {
                                clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <PositionReport>  => 更新失敗！  ");
                            }
                        }
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
