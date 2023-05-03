using System;
using System.Linq;
using Mirle.Def;
using Mirle.Def.V2BYMA30;
using Mirle.DB.Object;
using Mirle.Structure;
using System.Threading.Tasks;
using Mirle.DataBase;
using Mirle.DB.Object.Table;
using Mirle.LiteOn.V2BYMA30;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using System.Collections.Generic;

namespace Mirle.ASRS.WCS
{
    public class clsCmdDestinationCheck_Proc
    {
        /// <summary>
        /// 只用在8樓出庫的地方
        /// </summary>
        private System.Timers.Timer timRead = new System.Timers.Timer();       

        public clsCmdDestinationCheck_Proc()
        {
            timRead.Elapsed += new System.Timers.ElapsedEventHandler(timRead_Elapsed);
            timRead.Enabled = false; timRead.Interval = 500;
        }

        public void subStart( )
        {           
            timRead.Enabled = true;
        }

        private void timRead_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            timRead.Enabled = false;
            try
            {
                for (int i = 0; i < 1; i++)
                {
                    if (clsLiteOnCV.GetConveyorController_8F().IsConnected)
                    {
                        ConveyorInfo buffer = i switch
                        {
                            0 => ConveyorDef.LO2_01,                            
                            _ => ConveyorDef.LO2_02,
                        };

                        var cv = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer.Index);
                        if (cv.Presence && !string.IsNullOrWhiteSpace(cv.CommandID) && cv.RollNotice_PC == 0 && cv.RollNotice == 0 && cv.ReadBcrAck == 1 && cv.ReadBcrReq_PC == 0)
                        {
                            //查詢
                            string sCmdSno = cv.CommandID;
                            string sBcr = cv.GetTrayID.Trim();
                            string sRemark = "", strErrMsg = "";
                            string sNewSts = clsConstValue.CmdSts.strCmd_Running;

                            TaskInfo task = new TaskInfo();
                            int iRet = clsTask.CheckHasTaskCmd(sCmdSno, ref task);

                            string sMode = task.CmdMode;
                            string sSts = task.CmdSts;

                            CMD_DESTINATION_CHECKInfo_Response response = new CMD_DESTINATION_CHECKInfo_Response();

                            CMD_DESTINATION_CHECKInfo cmd_destination_checkInfo = new CMD_DESTINATION_CHECKInfo()
                            {
                                jobId = sCmdSno,
                                transactionId = "CMD_DESTINATION_CHECK",
                                location = buffer.BufferName,
                            };
                            clsWmsApi.GetApiProcess().GetCMD_DESTINATION_CHECK().funReport(cmd_destination_checkInfo, ref response);

                            response.toLocation = "LO4-04";
                            //int iPath = Convert.ToInt16(clsLiteOnCV.GetCmdPathByBufferID(response.toLocation));
                            //int iPath = ConveyorDef.B1_009.Path;

                            if (iRet != DBResult.Success && iRet != DBResult.NoDataSelect)
                            {
                                clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer.BufferName} <Cmd> {sCmdSno} => 取得命令資料失敗！");
                                continue;
                            }
                            //else if (sSts == clsConstValue.CmdSts.strCmd_Initial)
                            else
                            {
                                
                                if (cv.WriteCommandAndSetReadReqAsync(sCmdSno,2).Result)
                                {
                                    if (!clsTask.FunUpdateTaskCmd(sCmdSno, sNewSts, buffer.BufferName, sRemark, ref strErrMsg) || !clsTask.FunUpdateTaskCmdDes(sCmdSno, response.toLocation))
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <任務號> {sCmdSno} => 通知前進失敗！ ");
                                        continue;
                                    }
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <任務號> {sCmdSno} => 通知前進！ ()");
                                }
                                else
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer.BufferName} <任務號> {sCmdSno} => 通知前進失敗！ ()");
                                }

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
