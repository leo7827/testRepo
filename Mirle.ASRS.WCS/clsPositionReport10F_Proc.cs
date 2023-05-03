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
    public class clsPositionReport10F_Proc
    {
        private System.Timers.Timer timRead = new System.Timers.Timer();
        public clsPositionReport10F_Proc()
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
                int CvIdxTotal = clsLiteOnCV.GetConveyorController_10F().BufferCount;

                for (int i = 0; i < CvIdxTotal; i++)
                {
                    if (clsLiteOnCV.GetConveyorController_10F().IsConnected)
                    {
                        ConveyorInfo buffer = i switch
                        {
                            //bcr 處不要上報 position 
                            0 => ConveyorDef.LO1_01,
                            1 => ConveyorDef.LO1_02,
                            2 => ConveyorDef.LO1_03,
                            3 => ConveyorDef.LO1_04,
                            4 => ConveyorDef.LO1_05,
                            5 => ConveyorDef.LO1_06,
                            6 => ConveyorDef.LO1_07,

                            _ => ConveyorDef.LO1_08,

                        };

                        var cv = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);
                        if (cv.Presence && !string.IsNullOrEmpty(cv.CommandID) && !cv.TransferReportNotice_PC )
                        {
                            string sCmdSno = cv.CommandID;
                            string sBcr = cv.GetTrayID.Trim();

                            string sCurLoc = buffer.BufferName; 

                            //應WCS 要求 , 這八節不要上報 transferreport
                            //呼叫API
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

                           
                            if (cv.WriteTransferReport(1))
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
                            if (!cv.WriteTransferReport(0))
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
