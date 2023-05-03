using System;
using System.Data;
using Mirle.DB.Object;
using Mirle.Def;
using Mirle.Def.V2BYMA30;
using Mirle.WebAPI.Event.V2BYMA30.Models;
using System.Web.Http;
using Mirle.Structure;
using Mirle.LiteOn.V2BYMA30;
using Newtonsoft.Json;
using Mirle.DataBase;
using Mirle.DB.Object.Table;

namespace Mirle.WebAPI.Event.V2BYMA30
{
    public class WCSController : ApiController
    {
        public WCSController()
        {
        }           


        [Route("LIFT4C/CV_RECEIVE_NEW_BIN_CMD")]
        [HttpPost]
        public IHttpActionResult CV_RECEIVE_NEW_BIN_CMD([FromBody] CV_RECEIVE_NEW_BIN_CMD Body)
        {
            bool bFlag_Fault = false;
            bool bFlag_Auto = false;

            //寫值入周邊及產生命令 
            clsWriLog.Log.FunWriTraceLog_CV($"<CV_RECEIVE_NEW_BIN_CMD> <WCS Send>\n{JsonConvert.SerializeObject(Body)}");
            ReturnCV_RECEIVE_NEW_BIN_CMD rMsg = new ReturnCV_RECEIVE_NEW_BIN_CMD
            {
                jobId = Body.jobId,
                transactionId = "CV_RECEIVE_NEW_BIN_CMD"
            };

            clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}> CV_RECEIVE_NEW_BIN_CMD start!");
            try
            {

                TaskInfo task = new TaskInfo();
                string strEM = "";
                int iMode = Convert.ToInt16(clsLiteOnCV.GetCmdModeByBufferID(Body.bufferId));               
                int iFloor = clsLiteOnCV.GetFloorByBufferID(Body.bufferId);
                bool bFlag = true; 

                ConveyorInfo buffer = clsLiteOnCV.GetBufferByStnNo(Body.bufferId , ref iFloor );

                //檢查站口是否有警報
                switch (iFloor)
                {
                    case 8:
                        var cv8 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                        if (cv8.Error)
                        {
                            bFlag_Fault = true;
                        }
                        break;
                    default:
                        var cv10 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);
                        if (cv10.Error)
                        {
                            bFlag_Fault = true;
                        }
                        break;

                }

                if (bFlag_Fault)
                {
                    rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                    rMsg.returnComment = "不正常";
                    clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>BUFFER_ROLL_INFO Fail!" + " 該 CV 有警報");
                    return Json(rMsg);
                }

                //檢查站口是否是自動
                switch (iFloor)
                {
                    case 8:
                        var cv8 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                        if (cv8.Auto)
                        {
                            bFlag_Auto = true;
                        }
                        break;

                    default:
                        var cv10 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);
                        if (cv10.Auto)
                        {
                            bFlag_Auto = true;
                        }
                        break;

                }

                if (!bFlag_Auto)
                {
                    rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                    rMsg.returnComment = "不正常";
                    clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>BUFFER_ROLL_INFO Fail!" + " 該 CV 非自動");
                    return Json(rMsg);
                }

                if (clsTask.CheckHasTaskCmd(Body.jobId, ref task) != DBResult.Success && !clsLiteOnCV.ChkFinalBcrBufferID(Body.bufferId))
                {
                    //產生新命令
                    task.CmdSno = Body.jobId.PadLeft(5, '0');
                    task.CmdSts = "1";
                    task.Prty = "5";
                    task.Source = Body.bufferId;
                    task.Destination = clsLiteOnCV.GetGesByBufferID(Body.bufferId);
                    task.CmdMode = iMode.ToString();
                    task.CrtDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    task.ExpDate = "";
                    task.EndDate = " ";
                    task.CurLoc = " ";
                    task.Remark = "";
                    task.CarrierType = Body.carrierType;
                    if (!clsDB_Proc.GetDB_Object().GetTask().FunInsertTaskCmd(task, ref strEM))
                    {
                        throw new Exception(strEM);
                    }
                }

                switch (iFloor)
                {
                    case 10:
                        var cv1 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);
                        if (!cv1.WriteCommandAndSetReadReqAsync(Body.jobId, iMode).Result)
                        {
                            bFlag = false;
                        }
                        break;
                 
                    default:
                        var cv8 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                        ////V 1.0.0.4
                        if (!cv8.WriteCommandAndSetReadReqAsync(Body.jobId, iMode  ).Result)
                        {                           
                            bFlag = false;
                        }
                        break;

                }
                 
                if (!bFlag)
                {
                    rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                    rMsg.returnComment = "寫入周邊失敗";

                    //刪除這筆命令 
                    if (clsTask.FunDeleteTask(Body.jobId))
                    {
                        clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>CV_RECEIVE_NEW_BIN_CMD  寫入周邊失敗 !" + " 命令已刪除");
                    }

                    //bFlag = false;
                }
                else
                {

                    //查無此筆命令就新建 && Bcr最後一節不要新建命令
                    /*
                    if (clsTask.CheckHasTaskCmd(Body.jobId, ref task) != DBResult.Success && !clsLiteOnCV.ChkFinalBcrBufferID(Body.bufferId) )
                    {
                        //產生新命令
                        task.CmdSno = Body.jobId.PadLeft(5, '0');
                        task.CmdSts = "1";
                        task.Prty = "5";
                        task.Source = Body.bufferId;
                        task.Destination = clsLiteOnCV.GetGesByBufferID(Body.bufferId);
                        task.CmdMode = iMode.ToString();
                        task.CrtDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        task.ExpDate = "";
                        task.EndDate = " ";
                        task.CurLoc = " ";
                        task.Remark = "";
                        task.CarrierType = Body.carrierType;
                        if (!clsDB_Proc.GetDB_Object().GetTask().FunInsertTaskCmd(task, ref strEM))
                        {
                            throw new Exception(strEM);
                        }
                    }
                    */
                    rMsg.returnCode = clsConstValue.ApiReturnCode.Success;
                    rMsg.returnComment = "正常";
                }

                clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}> CV_RECEIVE_NEW_BIN_CMD end!");
                return Json(rMsg);

            }
            catch (Exception ex)
            {
                rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                rMsg.returnComment = ex.Message;

                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message + "\n(" + ex.StackTrace + ")");
                return Json(rMsg);
            }
            finally
            {

            }
        }




        [Route("LIFT4C/HEALTH_CHECK")]
        [HttpPost]
        public IHttpActionResult HealthCheck([FromBody] HealthCheckInfo Body)
        {

            ReturnHealthCheckInfo rMsg = new ReturnHealthCheckInfo
            {
                jobId = Body.jobId,
                transactionId = "HealthCheck"
            };
            clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>HealthCheck start!");

            try
            {
                //WebApiHost.log.WriteLogFile("API_Trace", $"<{Body}>ASRSHealthCheck End!");
                rMsg.returnCode = clsConstValue.ApiReturnCode.Success;
                rMsg.returnComment = "正常";
                return Json(rMsg);
            }
            catch (Exception ex)
            {
                rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                rMsg.returnComment = ex.Message;

                clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>HealthCheck Fail!" + ex);
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                return Json(rMsg);
            }
        }



        [Route("LIFT4C/BUFFER_STATUS_QUERY")]
        [HttpPost]
        public IHttpActionResult BUFFER_STATUS_QUERY([FromBody] BUFFER_STATUS_QUERYInfo Body)
        {

            ReturnBUFFER_STATUS_QUERYInfo rMsg = new ReturnBUFFER_STATUS_QUERYInfo
            {
                //jobId = Body.jobId,
                transactionId = "BUFFER_STATUS_QUERY",
                bufferId = Body.bufferId
            };
            //clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>BUFFER_STATUS_QUERY start!");
             
            int iFloor = 0;            

            try
            {
                ConveyorInfo buffer = clsLiteOnCV.GetBufferByStnNo(Body.bufferId , ref iFloor );                

                switch (iFloor)
                {
                    case 10:
                        var cv1 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);
                        rMsg.jobId = cv1.Signal.CommandID.GetValue().ToString().PadLeft(5, '0');
                        rMsg.ready = cv1.Ready.ToString();
                        rMsg.isLoad = (cv1.Presence == true) ? "Y" : "N";
                        rMsg.isEmpty = (cv1.Empty == true) ? "Y" : "N";
                        rMsg.stbSts = (cv1.Cylinder == true) ? "1" : "0";
                        break;
                    
                    default:
                        var cv8 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                        rMsg.jobId = cv8.Signal.CommandID.GetValue().ToString().PadLeft(5, '0');
                        rMsg.ready = cv8.Ready.ToString();
                        rMsg.isLoad = (cv8.Presence == true) ? "Y" : "N";
                        rMsg.isEmpty = (cv8.Empty == true) ? "Y" : "N";
                        rMsg.stbSts = (cv8.Cylinder == true) ? "1" : "0";
                        break;

                }

                
                rMsg.returnCode = clsConstValue.ApiReturnCode.Success;
                rMsg.returnComment = "正常";

                clsWriLog.Log.FunWriTraceLog_CV($"<JobId> : <{rMsg.jobId}>BUFFER_STATUS_QUERY ");
                clsWriLog.Log.FunWriTraceLog_CV($"<isLoad> : <{rMsg.isLoad}>BUFFER_STATUS_QUERY ");
                clsWriLog.Log.FunWriTraceLog_CV($"<Ready> : <{rMsg.ready}>BUFFER_STATUS_QUERY ");
                clsWriLog.Log.FunWriTraceLog_CV($"<isEmpty> : <{rMsg.isEmpty}>BUFFER_STATUS_QUERY ");
                clsWriLog.Log.FunWriTraceLog_CV($"<stbSts> : <{rMsg.stbSts}>BUFFER_STATUS_QUERY ");
                clsWriLog.Log.FunWriTraceLog_CV($"<{Body.bufferId}>BUFFER_STATUS_QUERY end!");

                return Json(rMsg);
            }
            catch (Exception ex)
            {
                rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                rMsg.returnComment = ex.Message;

                clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>BUFFER_STATUS_QUERY Fail!" + ex);
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                return Json(rMsg);
            }
        }

        [Route("LIFT4C/BUFFER_ROLL_INFO")]
        [HttpPost]
        public IHttpActionResult BUFFER_ROLL_INFO([FromBody] BUFFER_ROLL_INFO Body)
        {
            bool bFlag_Fault = false;
            bool bFlag_Auto = false;

            ReturnBUFFER_ROLL_INFO rMsg = new ReturnBUFFER_ROLL_INFO
            {
                jobId = Body.jobId,
                transactionId = "BUFFER_ROLL_INFO",
                bufferId = Body.bufferId
            };

            clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>BUFFER_ROLL_INFO start!");
            int iFloor = 0; 

            try
            {
                ConveyorInfo buffer = clsLiteOnCV.GetBufferByStnNo(Body.bufferId, ref iFloor );

                //檢查站口是否有警報
                switch (iFloor)
                {
                    case 8:
                        var cv8 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                        if (cv8.Error)
                        {
                            bFlag_Fault = true;
                        }
                        break; 
                    default:
                        var cv10 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);
                        if (cv10.Error)
                        {
                            bFlag_Fault = true;
                        }
                        break;

                }

                if (bFlag_Fault)
                {
                    rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                    rMsg.returnComment = "不正常";
                    clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>BUFFER_ROLL_INFO Fail!" + " 該 CV 有警報");
                    return Json(rMsg);
                }

                //檢查站口是否是自動
                switch (iFloor)
                {
                    case 8:
                        var cv8 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                        if (cv8.Auto)
                        {
                            bFlag_Auto = true;
                        }
                        break;
                     
                    default:
                        var cv10 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);
                        if (cv10.Auto)
                        {
                            bFlag_Auto = true;
                        }
                        break;

                }

                if (!bFlag_Auto)
                {
                    rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                    rMsg.returnComment = "不正常";
                    clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>BUFFER_ROLL_INFO Fail!" + " 該 CV 非自動");
                    return Json(rMsg);
                }

                switch (iFloor)
                {
                    case 10:
                        var cv10 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);
                        if (!cv10.InfoRolling(1).Result)
                        {
                            rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                            rMsg.returnComment = "不正常";
                            break;
                        }

                        break;
                    default:
                        var cv8 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                        if (!cv8.InfoRolling(1).Result)
                        {
                            rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                            rMsg.returnComment = "不正常";
                            break;
                        }
                        break;

                }


                rMsg.returnCode = clsConstValue.ApiReturnCode.Success;
                rMsg.returnComment = "正常";
                clsWriLog.Log.FunWriTraceLog_CV($"<{Body.jobId}>BUFFER_STATUS_QUERY end!");
                return Json(rMsg);
            }
            catch (Exception ex)
            {
                rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                rMsg.returnComment = ex.Message;

                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message + "\n(" + ex.StackTrace + ")");
                return Json(rMsg);
            }
            finally
            {

            }
        }

        [Route("LIFT4C/EMPTY_BIN_LOAD_DONE")]
        [HttpPost]
        public IHttpActionResult EMPTY_BIN_LOAD_DONE([FromBody] EMPTY_BIN_LOAD_Done_INFO Body)
        {

            ReturnEMPTY_BIN_LOAD_Done_INFO rMsg = new ReturnEMPTY_BIN_LOAD_Done_INFO
            {
                jobId = Body.jobId,
                transactionId = "EMPTY_BIN_LOAD_DONE", 
            };

            clsWriLog.Log.FunWriTraceLog_CV($"<{Body.location}>EMPTY_BIN_LOAD_DONE start!");
            int iFloor = 0;

            try
            { 
                if (Body.location != "LO1-07")
                {
                    rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                    rMsg.returnComment = "不正常";
                    clsWriLog.Log.FunWriTraceLog_CV($"<{Body.location}> EMPTY_BIN_LOAD_DONE 的站點不為 LO1-07!");
                    return Json(rMsg);

                }

                ConveyorInfo buffer = clsLiteOnCV.GetBufferByStnNo(Body.location, ref iFloor); 
                 
                var cv = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer.Index);

                int iCurrentPCValue = cv.CallEmptyQty_PC;
                int iCurrentPLCValue = cv.CallEmptyQty;

                iCurrentPCValue += 1;
                if (!cv.WriteEmptyQty(iCurrentPCValue).Result)
                {
                    rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                    rMsg.returnComment = "不正常";
                    return Json(rMsg);
                }

                rMsg.returnCode = clsConstValue.ApiReturnCode.Success;
                rMsg.returnComment = "正常";
                clsWriLog.Log.FunWriTraceLog_CV($"<{Body.location}> 目前PC 記數 <{iCurrentPCValue}>");
                clsWriLog.Log.FunWriTraceLog_CV($"<{Body.location}>EMPTY_BIN_LOAD_DONE end!");
                return Json(rMsg);
            }
            catch (Exception ex)
            {
                rMsg.returnCode = clsConstValue.ApiReturnCode.Fail;
                rMsg.returnComment = ex.Message;

                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message + "\n(" + ex.StackTrace + ")");
                return Json(rMsg);
            }
            finally
            {

            }
        }

    }
}
