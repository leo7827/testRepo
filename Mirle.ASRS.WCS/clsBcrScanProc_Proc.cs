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
    public class clsBcrScanProc_Proc
    {
        /// <summary>
        /// 掃碼時,上報
        /// </summary>
        private System.Timers.Timer timRead = new System.Timers.Timer();
        public clsBcrScanProc_Proc()
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
                for (int i = 0; i < 5; i++)
                {
                    //if (clsLiteOnCV.GetConveyorController_3F().IsConnected)
                    //{
                    ConveyorInfo buffer = i switch
                    {
                        0 => ConveyorDef.LO1_05,
                  

                        _ => ConveyorDef.LO1_05,
                    };


                    switch (i)
                    {
                        case 0:
                            //var cv0 = clsLiteOnCV.GetConveyorController_3F().GetBuffer(buffer.Index);

                            //if (cv0.Presence && cv0.ReadBcrAck == 1 && cv0.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv0.CommandID) && cv0.BcrCheckRequestNotice_PC == 0 && cv0.Ready == (int)clsEnum.Ready.IN)
                            //{
                            //    string sCmdSno = cv0.CommandID;
                            //    string sBcr = cv0.GetTrayID.Trim();
                            //    string sDestination = "";

                            //    呼叫API
                            //    BCR_CHECK_REQUESTInfo_Response response = new BCR_CHECK_REQUESTInfo_Response();

                            //    BCR_CHECK_REQUESTInfo bcr_check_requestInfo = new BCR_CHECK_REQUESTInfo()
                            //    {
                            //        jobId = sCmdSno,
                            //        transactionId = "BCR_CHECK_REQUEST",
                            //        barcode = sBcr,
                            //        location = buffer.BufferName,
                            //    };
                            //    clsWmsApi.GetApiProcess().GetBCR_CHECK_REQUEST().funReport(bcr_check_requestInfo, ref response);

                            //    if (response.returnCode != clsConstValue.ApiReturnCode.Success)
                            //    {
                            //        clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報失敗！ ");
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        if (cv0.WriteBcrSetReadReq(1).Result)
                            //        {
                            //            clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報成功！ ");
                            //            continue;

                            //        }

                            //    }
                            //}
                            break;
                        case 1:
                            //var cv1 = clsLiteOnCV.GetConveyorController_5F().GetBuffer(buffer.Index);
                            //if (cv1.Presence && cv1.ReadBcrAck == 1 && cv1.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv1.CommandID) && cv1.BcrCheckRequestNotice_PC == 0 && cv1.Ready == (int)clsEnum.Ready.IN)
                            //{
                            //    string sCmdSno = cv1.CommandID;
                            //    string sBcr = cv1.GetTrayID.Trim();
                            //    //string sDestination = "";

                            //    //呼叫API 
                            //    BCR_CHECK_REQUESTInfo_Response response = new BCR_CHECK_REQUESTInfo_Response();

                            //    BCR_CHECK_REQUESTInfo bcr_check_requestInfo = new BCR_CHECK_REQUESTInfo()
                            //    {
                            //        jobId = sCmdSno,
                            //        transactionId = "BCR_CHECK_REQUEST",
                            //        barcode = sBcr,
                            //        location = buffer.BufferName,
                            //    };
                            //    clsWmsApi.GetApiProcess().GetBCR_CHECK_REQUEST().funReport(bcr_check_requestInfo, ref response);

                            //    if (response.returnCode != clsConstValue.ApiReturnCode.Success)
                            //    {
                            //        clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報失敗！ ");
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        if (cv1.WriteBcrSetReadReq(1).Result)
                            //        {
                            //            clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報成功！ ");
                            //            continue;

                            //        }

                            //    }
                            //}
                            break;
                        case 2:
                            //var cv2 = clsLiteOnCV.GetConveyorController_6F().GetBuffer(buffer.Index);
                            //if (cv2.Presence && cv2.ReadBcrAck == 1 && cv2.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv2.CommandID) && cv2.BcrCheckRequestNotice_PC == 0 && cv2.Ready == (int)clsEnum.Ready.IN)
                            //{
                            //    string sCmdSno = cv2.CommandID;
                            //    string sBcr = cv2.GetTrayID.Trim();
                            //    //string sDestination = "";

                            //    //呼叫API 
                            //    BCR_CHECK_REQUESTInfo_Response response = new BCR_CHECK_REQUESTInfo_Response();

                            //    BCR_CHECK_REQUESTInfo bcr_check_requestInfo = new BCR_CHECK_REQUESTInfo()
                            //    {
                            //        jobId = sCmdSno,
                            //        transactionId = "BCR_CHECK_REQUEST",
                            //        barcode = sBcr,
                            //        location = buffer.BufferName,
                            //    };
                            //    clsWmsApi.GetApiProcess().GetBCR_CHECK_REQUEST().funReport(bcr_check_requestInfo, ref response);

                            //    if (response.returnCode != clsConstValue.ApiReturnCode.Success)
                            //    {
                            //        clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報失敗！ ");
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        if (cv2.WriteBcrSetReadReq(1).Result)
                            //        {
                            //            clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報成功！ ");
                            //            continue;

                            //        }

                            //    }
                            //}
                            break;

                        default:
                            var cv3 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                            if (cv3.Presence && cv3.ReadBcrAck == 1 && cv3.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv3.CommandID) && !cv3.BcrCheckRequestNotice_PC  && cv3.Ready == (int)clsEnum.Ready.IN)
                            {
                                string sCmdSno = cv3.CommandID;
                                string sBcr = cv3.GetTrayID.Trim();

                                //呼叫API 
                                BCR_CHECK_REQUESTInfo_Response response = new BCR_CHECK_REQUESTInfo_Response();

                                BCR_CHECK_REQUESTInfo bcr_check_requestInfo = new BCR_CHECK_REQUESTInfo()
                                {
                                    jobId = sCmdSno,
                                    transactionId = "BCR_CHECK_REQUEST",
                                    barcode = sBcr,
                                    location = buffer.BufferName,
                                };
                                clsWmsApi.GetApiProcess().GetBCR_CHECK_REQUEST().funReport(bcr_check_requestInfo, ref response);

                                if (response.returnCode != clsConstValue.ApiReturnCode.Success)
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報失敗！ ");
                                    continue;
                                }
                                else
                                {
                                    if (cv3.WriteBcrSetReadReq(1) )
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報成功！ ");
                                        continue;

                                    }

                                }
                            }
                            break;
                    }

                //}
                    
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
