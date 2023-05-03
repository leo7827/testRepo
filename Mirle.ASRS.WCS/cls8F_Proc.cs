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
using Mirle.ASRS.Conveyor.V2BYMA30_8F.Signal;

namespace Mirle.ASRS.WCS
{
    public class cls8F_Proc
    {
        private System.Timers.Timer timRead = new System.Timers.Timer();

        public cls8F_Proc()
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
                TaskInfo task = new TaskInfo();
                string sRemark = " ";
                string sRemark_Pre = " ";
                int iFROM = 0;
                int iTO = 0;
                string sCmdSno = "";
                string sBcr = "";
                string strErrMsg = "";
                string sNewSts = "";
                string sMode = "";
                string sSts = "";
                int iFloor = 0;
                int iEleIdleSts = 0;
                int iPath = 0;
                int iDoorStatus = 0;
                int iDoorStatus_PC = 0; //通知開門訊號
                int iCurrentFloor = clsLiteOnCV.CheckElevatorFloor();  //電梯當下的樓層
                int iElePlatformOnSts = clsLiteOnCV.CheckElevatorPlatformOn() ? 1 : 0;      //平台已經伸出
                int iElePlatformOffSts = clsLiteOnCV.CheckElevatorPlatformOff() ? 1 : 0;    //平台已經縮回

                ConveyorInfo buffer1 = ConveyorDef.LO2_01;  //
                ConveyorInfo buffer2 = ConveyorDef.LO2_02;  //buffer 滾出buffer
                ConveyorInfo buffer3 = ConveyorDef.LO2_03;  //buffer 滾入buffer
                ConveyorInfo buffer4 = ConveyorDef.LO2_04;  //bcr掃描


                if (!clsLiteOnCV.GetConveyorController_8F().IsConnected)
                {
                    return;
                }


                for (int i = 1; i <= SignalMapper.BufferCount; i++)
                {
                    switch (i)
                    {
                        //LO2-01
                        case 1:
                            
                            break;
                        //LO2-02
                        //電梯抵達8樓 , 閒置 , 有空位 , 有ready 訊號, 開門 , 通知滾動
                        case 2:
                            if (iCurrentFloor != 8)
                            { continue; }

                            var cv2 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer2.Index);
                            var cvEle1 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(ConveyorDef.LI1_02.Index);
                            var cvEle2 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(ConveyorDef.LI1_01.Index);
                            sCmdSno = cv2.CommandID;

                            iPath = ( cvEle1.Presence || !string.IsNullOrEmpty(cvEle1.CommandID)) ? ConveyorDef.LI1_01.Path : ConveyorDef.LI1_02.Path;
                            iFloor = clsLiteOnCV.GetConveyorController_Elevator().Signal.Floor.GetValue();
                            iEleIdleSts = clsLiteOnCV.GetConveyorController_Elevator().Idle ? 1 : 0;
                            iDoorStatus = clsLiteOnCV.GetConveyorController_Elevator().Signal.DoorStatus.GetValue();
                            iDoorStatus_PC = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.DoorNoticce.GetValue();
                            sNewSts = "1";
                            //iPath = 11;
                            //iFloor = 8;
                            //iEleIdleSts =  1 ;
                            //iDoorStatus = 2;

                            // V 1.0.0.2 平台分成兩步驟
                            // 先對電梯平台預約   
                            if (iFloor == 8 && iDoorStatus == 2 && string.IsNullOrEmpty(cvEle2.CommandID) && iDoorStatus_PC == 0 && cvEle2.Ready == (int)clsEnum.Ready.OUT &&
                                    cv2.Presence && !string.IsNullOrEmpty(sCmdSno) && cv2.RollNotice_PC == 0 && cv2.RollNotice == 0 && cv2.Ready == (int)clsEnum.Ready.IN &&
                                    iElePlatformOnSts == 0
                                    )
                            {
                                //if (cv2.WriteRolling().Result && cvEle1.WriteCommandAndSetPathCarrierTypeAndRoll(sCmdSno, sMode, iCarrierType, iPath).Result)
                                if (cvEle2.WriteCommandAndRolling(sCmdSno, 1, iPath).Result)
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer2.BufferName}   => 通知電梯平台滾動成功！ ");
                                    continue;
                                }
                            }

                            //平台已伸出
                            if (iFloor == 8 && iDoorStatus == 2 && !string.IsNullOrEmpty(cvEle2.CommandID) && iDoorStatus_PC == 0 && cvEle2.RollNotice != 0 &&
                                cv2.Presence && !string.IsNullOrEmpty(sCmdSno) && cv2.RollNotice_PC == 0 && cv2.RollNotice == 0 && cv2.Ready == (int)clsEnum.Ready.IN &&
                                iElePlatformOnSts == 1
                                )
                            {
                                if (cv2.WriteRolling().Result)
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer2.BufferName}   => 通知8樓平台滾動成功！ ");
                                    continue;
                                }
                            }
                             
                            break;
                        //LO2-03
                        //電梯抵達8樓 , 閒置 , 有物 , 有ready 訊號 , 開門 , 通知滾動
                        case 3:
                            if (iCurrentFloor != 8)
                            { continue; }

                            var cv3 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer3.Index);
                            var cvEle3 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(ConveyorDef.LI1_03.Index);
                            var cvEle4 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(ConveyorDef.LI1_04.Index);
                            sCmdSno = cvEle4.CommandID;
                            //sCmdSno = "12345";
                            iFloor = clsLiteOnCV.GetConveyorController_Elevator().Signal.Floor.GetValue();
                            iEleIdleSts = clsLiteOnCV.GetConveyorController_Elevator().Idle ? 1 : 0;
                            iDoorStatus_PC = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.DoorNoticce.GetValue();
                            sNewSts = "1";

                            iFloor = 8;
                            iEleIdleSts = 1;

                            if ( iFloor == 8 && !string.IsNullOrEmpty(sCmdSno) && iDoorStatus_PC == 0 && cvEle4.Ready == (int)clsEnum.Ready.IN
                                 && !cv3.Presence && string.IsNullOrEmpty(cv3.CommandID) && cv3.RollNotice_PC == 0 && cv3.RollNotice == 0 && cv3.Ready == (int)clsEnum.Ready.OUT)
                            {
                                //V 1.0.0.3
                                //if (clsTask.FunUpdateTaskCmd(sCmdSno, sNewSts, buffer3.BufferName, sRemark, ref strErrMsg))
                                //{
                                    if (cv3.WriteCommandAndRolling(sCmdSno, 1).Result && cvEle4.InfoRolling(1).Result)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer3.BufferName}   => 通知滾動成功！ ");
                                        continue;

                                    }
                                //}
                                   
                            }

                            break;
                        //LO2-04
                        default:
                            var cv4 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer4.Index);
                            //if (cv4.Presence && cv4.ReadBcrAck == 1 && cv4.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv4.CommandID) && !cv4.BcrCheckRequestNotice_PC && cv4.Ready == (int)clsEnum.Ready.IN)
                            if (cv4.Presence && cv4.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv4.CommandID) && cv4.Ready == (int)clsEnum.Ready.IN) 
                            {
                                if (cv4.ReadBcrAck == 1 && !cv4.BcrCheckRequestNotice_PC)
                                {
                                    sBcr = cv4.GetTrayID.Trim();
                                    if (cv4.WriteBcrSetReadReq(1) && cv4.SetReadReq().Result)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer4.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 通知已讀！ ");
                                        continue;
                                    }
                                }
                                else if (cv4.ReadBcrAck == 0 && cv4.BcrCheckRequestNotice_PC)
                                {
                                    sCmdSno = cv4.CommandID;
                                    sBcr = cv4.GetTrayID.Trim();

                                    BCR_CHECK_REQUESTInfo_Response response = new BCR_CHECK_REQUESTInfo_Response();

                                    BCR_CHECK_REQUESTInfo bcr_check_requestInfo = new BCR_CHECK_REQUESTInfo()
                                    {
                                        jobId = sCmdSno,
                                        transactionId = "BCR_CHECK_REQUEST",
                                        barcode = sBcr,
                                        carrierType = "BIN",
                                        //carrierType = cv4.CarrierType == 1 ? "BIN" : "MAG",
                                        location = buffer4.BufferName,
                                    };
                                    clsWmsApi.GetApiProcess().GetBCR_CHECK_REQUEST().funReport(bcr_check_requestInfo, ref response);

                                    //response.returnCode = clsConstValue.ApiReturnCode.Success;
                                    if (response.returnCode != clsConstValue.ApiReturnCode.Success)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer4.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報失敗！ ");
                                        continue;
                                    }
                                    else
                                    {
                                        if (cv4.WriteBcrSetReadReq(0)) 
                                        {
                                            clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer4.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報成功！ ");
                                            continue;

                                        }

                                    }
                                } 

                            }
                            break;
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
