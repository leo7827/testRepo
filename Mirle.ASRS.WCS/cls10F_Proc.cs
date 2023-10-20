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
using Mirle.ASRS.Conveyor.V2BYMA30_10F.Signal;

namespace Mirle.ASRS.WCS
{
    public class cls10F_Proc
    {
        private System.Timers.Timer timRead = new System.Timers.Timer();

        public cls10F_Proc()
        {
            timRead.Elapsed += new System.Timers.ElapsedEventHandler(timRead_Elapsed);
            timRead.Enabled = false; timRead.Interval = 1000;
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

                ConveyorInfo buffer1 = ConveyorDef.LO1_02;  //buffer 入庫掃描
                ConveyorInfo buffer2 = ConveyorDef.LO1_07;  //buffer 出庫掃描
                ConveyorInfo buffer3 = ConveyorDef.LO1_04;  //buffer 滾入電梯
                ConveyorInfo buffer4 = ConveyorDef.LO1_05;  //buffer 滾出電梯


                if (!clsLiteOnCV.GetConveyorController_10F().IsConnected)
                {
                    return;
                }

                for (int i = 1; i <= SignalMapper.BufferCount; i++)
                {
                    switch (i)
                    {
                        //LO1_02  bcr 掃描到貨物就上報
                        case 1:
                            var cv1 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer1.Index);
                            //if (cv1.Presence && cv1.ReadBcrAck == 1 && cv1.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv1.CommandID) && !cv1.BcrCheckRequestNotice_PC && cv1.Ready == (int)clsEnum.Ready.IN)
                            if (cv1.Presence &&  cv1.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv1.CommandID)   && cv1.Ready == (int)clsEnum.Ready.IN)
                            {
                                if ( cv1.ReadBcrAck == 1 && !cv1.BcrCheckRequestNotice_PC)
                                {
                                    sBcr = cv1.GetTrayID.Trim();
                                    if (cv1.WriteBcrSetReadReq(1) && cv1.SetReadReq().Result)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer1.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 通知已讀！ ");
                                        continue;

                                    }
                                }
                                else if (cv1.ReadBcrAck == 0 && cv1.BcrCheckRequestNotice_PC)
                                {
                                    sCmdSno = cv1.CommandID;
                                    sBcr = cv1.GetTrayID.Trim();

                                    BCR_CHECK_REQUESTInfo_Response response = new BCR_CHECK_REQUESTInfo_Response();

                                    BCR_CHECK_REQUESTInfo bcr_check_requestInfo = new BCR_CHECK_REQUESTInfo()
                                    {
                                        jobId = sCmdSno,
                                        transactionId = "BCR_CHECK_REQUEST",
                                        barcode = sBcr,
                                        carrierType = "BIN",
                                        //carrierType = cv1.CarrierType == 1 ? "BIN" : "MAG",
                                        location = buffer1.BufferName,
                                    };
                                    clsWmsApi.GetApiProcess().GetBCR_CHECK_REQUEST().funReport(bcr_check_requestInfo, ref response);

                                    //response.returnCode = clsConstValue.ApiReturnCode.Success;
                                    if (response.returnCode != clsConstValue.ApiReturnCode.Success)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer1.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報失敗！ ");
                                        continue;
                                    }
                                    else
                                    {
                                        if (cv1.WriteBcrSetReadReq(0))
                                        {
                                            clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer1.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報成功！ ");
                                            continue;
                                        }
                                       
                                    }
                                }

                                /*
                                sCmdSno = cv1.CommandID;
                                sBcr = cv1.GetTrayID.Trim();

                                BCR_CHECK_REQUESTInfo_Response response = new BCR_CHECK_REQUESTInfo_Response();

                                BCR_CHECK_REQUESTInfo bcr_check_requestInfo = new BCR_CHECK_REQUESTInfo()
                                {
                                    jobId = sCmdSno,
                                    transactionId = "BCR_CHECK_REQUEST",
                                    barcode = sBcr,
                                    carrierType =  "BIN"  ,
                                    //carrierType = cv1.CarrierType == 1 ? "BIN" : "MAG",
                                    location = buffer1.BufferName,
                                };
                                clsWmsApi.GetApiProcess().GetBCR_CHECK_REQUEST().funReport(bcr_check_requestInfo, ref response);

                                //response.returnCode = clsConstValue.ApiReturnCode.Success;
                                if (response.returnCode != clsConstValue.ApiReturnCode.Success)
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer1.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報失敗！ ");
                                    continue;
                                }
                                else
                                {
                                    if (cv1.WriteBcrSetReadReq(1) && cv1.SetReadReq().Result)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer1.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報成功！ ");
                                        continue;

                                    }

                                }
                                */                    

                            }


                            break;
                        //LO1_07    bcr 掃描到貨物就上報                       
                        case 2:

                            var cv2 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer2.Index);
                            //if (cv2.Presence && cv2.ReadBcrAck == 1 && cv2.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv2.CommandID) && !cv2.BcrCheckRequestNotice_PC  && cv2.Ready == (int)clsEnum.Ready.IN)
                            if (cv2.Presence && cv2.ReadBcrReq_PC == 0 && string.IsNullOrEmpty(cv2.CommandID)  && cv2.Ready == (int)clsEnum.Ready.IN)
                            {
                                if (cv2.ReadBcrAck == 1 && !cv2.BcrCheckRequestNotice_PC)
                                {
                                    if (cv2.WriteBcrSetReadReq(1) && cv2.SetReadReq().Result)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer2.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 通知已讀！ ");
                                        continue;
                                    }
                                }
                                else if (cv2.ReadBcrAck == 0 && cv2.BcrCheckRequestNotice_PC)
                                {
                                    sCmdSno = cv2.CommandID;
                                    sBcr = cv2.GetTrayID.Trim();

                                    BCR_CHECK_REQUESTInfo_Response response = new BCR_CHECK_REQUESTInfo_Response();

                                    BCR_CHECK_REQUESTInfo bcr_check_requestInfo = new BCR_CHECK_REQUESTInfo()
                                    {
                                        jobId = sCmdSno,
                                        transactionId = "BCR_CHECK_REQUEST",
                                        barcode = sBcr,
                                        carrierType = "BIN",
                                        //carrierType = cv2.CarrierType == 1 ? "BIN" : "MAG",                                        
                                        location = buffer2.BufferName,
                                    };
                                    clsWmsApi.GetApiProcess().GetBCR_CHECK_REQUEST().funReport(bcr_check_requestInfo, ref response);

                                    //response.returnCode = clsConstValue.ApiReturnCode.Success;
                                    if (response.returnCode != clsConstValue.ApiReturnCode.Success)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer2.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報失敗！ ");
                                        continue;
                                    }
                                    else
                                    {
                                        if (cv2.WriteBcrSetReadReq(0))                                       
                                        {
                                            clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer2.BufferName} <條碼號> {sBcr} => BCR_CHECK_REQUEST 上報成功！ ");
                                            continue;

                                        }

                                    }
                                } 
                            }


                            break;
                        //LO1_04
                        //電梯抵達10樓 , 閒置 , 有物 , 有ready 訊號 , 開門 , 通知滾動
                        case 3:
                            if (iCurrentFloor != 10)
                            { continue; }
                            var cv3 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer3.Index);
                            var cvEle1 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(ConveyorDef.LI1_03.Index);
                            var cvEle2 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(ConveyorDef.LI1_04.Index);

                            sCmdSno = cv3.CommandID;
                            iFloor = clsLiteOnCV.GetConveyorController_Elevator().Signal.Floor.GetValue();
                            sNewSts = "1";
                            iPath = (cvEle1.Presence || !string.IsNullOrEmpty(cvEle1.CommandID) ) ? ConveyorDef.LI1_04.Path : ConveyorDef.LI1_03.Path;
                            iDoorStatus_PC = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.DoorNoticce.GetValue();
                            iFloor = clsLiteOnCV.GetConveyorController_Elevator().Signal.Floor.GetValue(); 
                            iDoorStatus = clsLiteOnCV.GetConveyorController_Elevator().Signal.DoorStatus.GetValue();

                            //sCmdSno ="12345";
                            //iFloor = 1;
                            //iEleIdleSts = 1; 

                            // V 1.0.0.2 平台分成兩步驟
                            // 先對電梯平台預約   
                            if (iFloor == 10 && iDoorStatus == 2 && string.IsNullOrEmpty(cvEle2.CommandID) && iDoorStatus_PC == 0 && cvEle2.Ready == (int)clsEnum.Ready.OUT &&
                                    cv3.Presence && !string.IsNullOrEmpty(sCmdSno) && cv3.RollNotice_PC == 0 && cv3.RollNotice == 0 && cv3.Ready == (int)clsEnum.Ready.IN &&
                                    iElePlatformOnSts == 0
                                    )
                            {
                                //if (cv2.WriteRolling().Result && cvEle1.WriteCommandAndSetPathCarrierTypeAndRoll(sCmdSno, sMode, iCarrierType, iPath).Result)
                                if (cvEle2.WriteCommandAndRolling(sCmdSno, 1, iPath).Result)
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer3.BufferName}   => 通知電梯平台滾動成功！ ");
                                    continue;
                                }
                            }

                            //平台已伸出
                            if (iFloor == 10 && iDoorStatus == 2 && !string.IsNullOrEmpty(cvEle2.CommandID) && iDoorStatus_PC == 0 && cvEle2.RollNotice != 0 && !cvEle2.Presence &&
                                cv3.Presence && !string.IsNullOrEmpty(sCmdSno) && cv3.RollNotice_PC == 0 && cv3.RollNotice == 0 && cv3.Ready == (int)clsEnum.Ready.IN &&
                                iElePlatformOnSts == 1 && cvEle2.CommandID == sCmdSno
                                )
                            {
                                if (cv3.WriteRolling().Result)
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer3.BufferName}   => 通知10樓平台滾動成功！ ");
                                    continue;
                                }
                            }
                             
                            break;
                        //LO1_05    電梯抵達10樓 , 閒置 , 有物 , 有ready 訊號 , 開門 , 通知滾動
                        case 4:
                            if (iCurrentFloor != 10)
                            { continue; }
                            var cv4 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer4.Index);
                            var cvEle3 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(ConveyorDef.LI1_02.Index);
                            var cvEle4 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(ConveyorDef.LI1_01.Index);

                            sCmdSno = cvEle4.CommandID;
                            iFloor = clsLiteOnCV.GetConveyorController_Elevator().Signal.Floor.GetValue();
                            iDoorStatus_PC = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.DoorNoticce.GetValue();

                            iFloor = clsLiteOnCV.GetConveyorController_Elevator().Signal.Floor.GetValue();
                            iEleIdleSts = clsLiteOnCV.GetConveyorController_Elevator().Idle ? 1 : 0;
                            iDoorStatus = clsLiteOnCV.GetConveyorController_Elevator().Signal.DoorStatus.GetValue();
                            sNewSts = "1";

                            //sCmdSno = "12345";
                            //iFloor = 1;
                            //iEleIdleSts = 1;

                            if (iFloor == 10 && iDoorStatus == 2 && !string.IsNullOrEmpty(sCmdSno) && iDoorStatus_PC == 0 && cvEle4.Ready == (int)clsEnum.Ready.IN
                                 && !cv4.Presence && string.IsNullOrEmpty(cv4.CommandID) && cv4.RollNotice_PC == 0 && cv4.RollNotice == 0 && cv4.Ready == (int)clsEnum.Ready.OUT)
                            {
                                //V 1.0.0.3
                                //if (clsTask.FunUpdateTaskCmd(sCmdSno, sNewSts, buffer4.BufferName, sRemark, ref strErrMsg))
                                //{
                                if (cv4.WriteCommandAndRolling(sCmdSno, 1).Result && cvEle4.InfoRolling(1).Result)
                                {
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer4.BufferName}   => 通知滾動成功！ ");
                                    continue;

                                }
                                //}

                            }
                            break;

                        default:
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
