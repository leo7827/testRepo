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
    public class clsEMPTY_BIN_LOAD_REQUEST_Proc
    {
        /// <summary>
        ///  呼叫空箱
        /// </summary>
        private System.Timers.Timer timRead = new System.Timers.Timer();
        public clsEMPTY_BIN_LOAD_REQUEST_Proc()
        {
            timRead.Elapsed += new System.Timers.ElapsedEventHandler(timRead_Elapsed);
            timRead.Enabled = false; timRead.Interval = 3000;
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

                for (int i = 0; i < 2; i++)
                {
                    if (clsLiteOnCV.GetConveyorController_10F().IsConnected)
                    {
                        ConveyorInfo buffer = i switch
                        {
                            0 => ConveyorDef.LO1_07,

                            _ => ConveyorDef.LO1_07,

                        };

                        var cv = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);
                        if (  !cv.CallEmpty && cv.CallEmptyQty > 0 && cv.CallEmptyQty_PC == 0)
                        {
                            string sCmdSno = cv.CommandID;
                            string sBcr = cv.GetTrayID.Trim();
                            string sCurLoc = buffer.BufferName;

                            EMPTY_BIN_LOAD_REQUESTInfo_Response response = new EMPTY_BIN_LOAD_REQUESTInfo_Response();
                            //呼叫API
                            EMPTY_BIN_LOAD_REQUESTInfo EMPTY_BIN_LOAD_REQUESTInfo = new EMPTY_BIN_LOAD_REQUESTInfo()
                            {
                                jobId = sCmdSno,
                                transactionId = "EMPTY_BIN_LOAD_REQUEST",
                                location = sCurLoc,
                                reqQty = cv.CallEmptyQty,
                            };

                            clsWmsApi.GetApiProcess().GetCallEmptyReport().FunReport(EMPTY_BIN_LOAD_REQUESTInfo, ref response);
                          
                            if (response.returnCode == clsConstValue.ApiReturnCode.Success)
                            { 
                                if (cv.WriteCallEmptyReport(1)  )//&& cv.WriteEmptyQty(cv.CallEmptyQty).Result)
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <CallEmptyReport> => 呼叫上報成功！ {cv.CallEmptyQty} 箱");
                                }

                            }
                            else
                            {
                                clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <CallEmptyReport> => NG : 呼叫上報失敗！ ");
                            }

                            continue;

                        }


                        //else if (cv.Presence && cv.CallEmpty    )
                        //{
                        //    if (cv.CallEmptyQty < cv.CallEmptyQty_PC)
                        //    {
                        //        int Qty = cv.CallEmptyQty + 1;
                        //        if (cv.WriteEmptyQty(Qty).Result)
                        //        {
                        //            clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <CallEmptyReport> => 更新數目成功！ CallEmptyQty =  {Qty}");
                        //        }
                        //        continue; 
                        //    }
                        //    //清掉API上報                           
                        //    else if (cv.CallEmptyQty == cv.CallEmptyQty_PC)
                        //    {
                        //        if (cv.WriteCallEmptyReport(0))
                        //        {
                        //            clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <CallEmptyReport> => 呼叫清除成功！ ");
                        //        }
                        //        continue;
                        //    } 
                          
                        //}


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
