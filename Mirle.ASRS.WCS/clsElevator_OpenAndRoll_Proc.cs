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
    public class clsElevator_OpenAndRoll_Proc
    {
        /// <summary>
        /// 控制電梯開關門 .通知滾動
        /// </summary>
        private System.Timers.Timer timRead = new System.Timers.Timer();

        public clsElevator_OpenAndRoll_Proc()
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
                TaskInfo task = new TaskInfo();      
                string sCmdSno = "";
                string sBcr = "";              
                string sMode = "";              
                string sFrom = "";
                string sTo = "";          
                int iPath = 0;
                int iRet = 0;

                int iDoorStatus = clsLiteOnCV.GetConveyorController_Elevator().Signal.DoorStatus.GetValue();
                int iDoorStatus_PC = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.DoorNoticce.GetValue();
                int iCurrentFloor = clsLiteOnCV.CheckElevatorFloor();  //電梯當下的樓層

                ConveyorInfo buffer_Ele_LI1_04 = ConveyorDef.LI1_04;  //電梯buffer 滾入
                ConveyorInfo buffer_Ele_LI1_03 = ConveyorDef.LI1_03;  //電梯buffer 滾出
                ConveyorInfo buffer_Ele_LI1_02 = ConveyorDef.LI1_02;  //電梯buffer 滾入的第二節
                ConveyorInfo buffer_Ele_LI1_01 = ConveyorDef.LI1_01;  //電梯buffer 滾出的第二節

                ConveyorInfo buffer_CV_10F_1 = ConveyorDef.LO1_03;  //平面buffer 滾入第二節
                ConveyorInfo buffer_CV_10F_2 = ConveyorDef.LO1_04;  //平面buffer 滾入
                ConveyorInfo buffer_CV_10F_3 = ConveyorDef.LO1_05;  //平面buffer 滾出

                ConveyorInfo buffer_CV_8F_1 = ConveyorDef.LO2_01;  //平面buffer 滾入第二節
                ConveyorInfo buffer_CV_8F_2 = ConveyorDef.LO2_02;  //平面buffer 滾入
                ConveyorInfo buffer_CV_8F_3 = ConveyorDef.LO2_03;  // 平面buffer 滾出

                if (!clsLiteOnCV.GetConveyorController_Elevator().IsConnected)
                {
                    return;
                }

                if (!clsLiteOnCV.CheckElevatorIsAgv())
                {
                    return;
                }

                for (int i = 1; i <= 2; i++)
                {
                    switch (i)
                    {
                        case 1:
                            if (iCurrentFloor != 8)
                            { continue; }

                            var cvr_Ele_LI1_01 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_LI1_01.Index);
                            var cvr_Ele_LI1_02 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_LI1_02.Index);  
                            var cvr_Ele_LI1_03 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_LI1_03.Index);
                            var cvr_Ele_LI1_04 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_LI1_04.Index);

                            var cvr_CV_In_8F_1 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_CV_8F_1.Index);  //LO2_01
                            var cvr_CV_In_8F_2 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_CV_8F_2.Index);  //LO2_02
                            var cvr_CV_In_8F_3 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_CV_8F_3.Index);  //LO2_03

                            if (iDoorStatus_PC == 0)
                            {
                                // 電梯關閉 , 裡面面有貨物要出去 , 通知開門
                                if (iDoorStatus == 1  && (cvr_Ele_LI1_03.Presence || cvr_Ele_LI1_04.Presence))   //&& cvr_CV_In_8F_3.Presence
                                {
                                    clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Open);
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知開門> {8} F , 情境1 => 寫值成功！ ");
                                    continue;
                                }

                                // 電梯關閉 , 外面有貨物等待進來 , 通知開門
                                if (iDoorStatus == 1 && cvr_CV_In_8F_2.Presence && (!cvr_Ele_LI1_01.Presence || !cvr_Ele_LI1_02.Presence))
                                {
                                    clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Open);
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知開門> {8} F , 情境2 => 寫值成功！ ");
                                    continue;
                                }

                                //電梯內兩節都有貨物就關門  且沒有貨物要出去 
                                if (iDoorStatus == 2 && cvr_Ele_LI1_01.Presence && cvr_Ele_LI1_02.Presence && string.IsNullOrEmpty(cvr_Ele_LI1_03.CommandID) && string.IsNullOrEmpty(cvr_Ele_LI1_04.CommandID) )
                                {
                                    clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Close);
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知關門> {8} F , 情境3 => 寫值成功！ ");
                                    continue;
                                }

                                //門開 , 電梯已經沒有要執行的命令時就通知關門  
                                //else if (iDoorStatus == 2 && string.IsNullOrEmpty(cvr_Ele_LI1_04.CommandID) && !cvr_Ele_LI1_03.Presence && !cvr_Ele_LI1_04.Presence && !cvr_CV_In_8F_2.Presence)  //&& cvr_Ele_LI1_02.Presence
                                else if (iDoorStatus == 2 && string.IsNullOrEmpty(cvr_Ele_LI1_04.CommandID) && string.IsNullOrEmpty(cvr_Ele_LI1_03.CommandID)  && !cvr_Ele_LI1_03.Presence && !cvr_Ele_LI1_04.Presence 
                                        && !cvr_CV_In_8F_2.Presence && !cvr_CV_In_8F_1.Presence)  
                                {      
                                    clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Close);
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知關門> {8} F , 情境4 => 寫值成功！ ");
                                    continue;

                                }

                                // (滾進電梯的單板情境) 若是門開 且靠近電梯2個CV都沒貨了, 就通知關門  
                                else if (iDoorStatus == 2 && !cvr_CV_In_8F_1.Presence && !cvr_CV_In_8F_2.Presence && !cvr_Ele_LI1_01.Presence && !cvr_Ele_LI1_04.Presence 
                                    && string.IsNullOrEmpty(cvr_Ele_LI1_03.CommandID) && string.IsNullOrEmpty(cvr_Ele_LI1_04.CommandID)
                                    )
                                {
                                    //檢查第二格是不是要到這個樓層
                                    //if (cvr_Ele_LI1_03.Presence && !ChkFloorByCmd(cvr_Ele_LI1_03.CommandID, iCurrentFloor))
                                    if (!cvr_Ele_LI1_03.Presence && string.IsNullOrEmpty(cvr_Ele_LI1_03.CommandID))
                                    {
                                        clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Close);
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知關門> {8} F , 情境5 => 寫值成功！ ");
                                        continue;
                                    }
                                    else
                                    {
                                        goto label_8_1;
                                    }

                                }

                            label_8_1:
                                //(沒有貨物要滾出.滾入) 若是門開 且2個CV都沒貨了, 就通知關門   
                                if (iDoorStatus == 2 && !cvr_CV_In_8F_1.Presence && !cvr_CV_In_8F_2.Presence && !cvr_Ele_LI1_04.Presence && string.IsNullOrEmpty(cvr_Ele_LI1_04.CommandID))
                                {
                                    //檢查第二格是不是要到這個樓層
                                    //if (cvr_Ele_LI1_03.Presence && !ChkFloorByCmd(cvr_Ele_LI1_03.CommandID, iCurrentFloor
                                    if (!cvr_Ele_LI1_03.Presence && string.IsNullOrEmpty(cvr_Ele_LI1_03.CommandID))
                                    {
                                        clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Close);
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知關門> {8} F , 情境6 => 寫值成功！ ");
                                        continue;
                                    }
                                    else
                                    {
                                        goto label_8_2;
                                    }

                                }

                            label_8_2:
                                //電梯內只有一版 , 且電梯開門 , 貨物已經在指定樓層 , 通知往前滾動
                                if (iDoorStatus == 2 && !cvr_Ele_LI1_04.Presence && cvr_Ele_LI1_03.Presence && cvr_Ele_LI1_04.RollNotice_PC == 0 && cvr_Ele_LI1_03.RollNotice_PC ==0)  // && !cvr_CV_In_8F_3.Presence 
                                {
                                    if (cvr_Ele_LI1_03.InfoRolling().Result)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer_Ele_LI1_03.BufferName} <通知滾動>   => 寫值成功！ ");
                                        continue;
                                    }

                                }

                                //電梯內只有一版 , 且電梯關門 , 通知往前滾動
                                else if (iDoorStatus == 1 && !cvr_Ele_LI1_01.Presence && cvr_Ele_LI1_02.Presence && cvr_Ele_LI1_02.RollNotice_PC == 0)
                                {
                                    if (cvr_Ele_LI1_02.InfoRolling().Result)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer_Ele_LI1_02.BufferName} <通知滾動>   => 寫值成功！ ");
                                        continue;
                                    }

                                }
                            }

                            break;
                            

                        default:
                            if (iCurrentFloor != 10)
                            { continue; }

                            var cvr_CV_In_10F_1 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_CV_10F_1.Index);  //LO1_03
                            var cvr_CV_In_10F_2 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_CV_10F_2.Index);  //LO1_04
                            var cvr_CV_In_10F_3 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_CV_10F_3.Index);  //LO1_05

                            cvr_Ele_LI1_01 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_LI1_01.Index);
                            cvr_Ele_LI1_02 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_LI1_02.Index);
                            cvr_Ele_LI1_03 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_LI1_03.Index);
                            cvr_Ele_LI1_04 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_LI1_04.Index);


                            if (iDoorStatus_PC == 0)
                            {
                                // 電梯關閉 , 裡面面有貨物要出去 , 通知開門
                                if (iDoorStatus == 1 &&  (cvr_Ele_LI1_01.Presence || cvr_Ele_LI1_02.Presence)) //cvr_CV_In_10F_3.Presence &&
                                {
                                    clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Open);
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知開門> {10} F , 情境1 => 寫值成功！ ");
                                    continue;
                                }

                                // 電梯關閉 , 外面有貨物等待進來 , 通知開門
                                if (iDoorStatus == 1 && cvr_CV_In_10F_2.Presence && (!cvr_Ele_LI1_03.Presence || !cvr_Ele_LI1_04.Presence))
                                {
                                    clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Open);
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知開門> {10} F , 情境2 => 寫值成功！ ");
                                    continue;
                                }

                                //電梯內兩節都有貨物就關門  且沒有貨物要出去
                                if (iDoorStatus == 2 && cvr_Ele_LI1_03.Presence && cvr_Ele_LI1_04.Presence && string.IsNullOrEmpty(cvr_Ele_LI1_01.CommandID) && string.IsNullOrEmpty(cvr_Ele_LI1_02.CommandID))
                                {
                                    clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Close);
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知關門> {10} F , 情境3 => 寫值成功！ ");
                                    continue;
                                }

                                //門開 , 電梯已經沒有要執行的命令時就通知關門
                                //else if (iDoorStatus == 2 && string.IsNullOrEmpty(cvr_Ele_LI1_04.CommandID) && !cvr_Ele_LI1_01.Presence && !cvr_Ele_LI1_02.Presence && !cvr_CV_In_10F_2.Presence)
                                else if (iDoorStatus == 2 && string.IsNullOrEmpty(cvr_Ele_LI1_02.CommandID) && string.IsNullOrEmpty(cvr_Ele_LI1_01.CommandID) && !cvr_Ele_LI1_01.Presence && !cvr_Ele_LI1_02.Presence
                                        && !cvr_CV_In_10F_2.Presence && !cvr_CV_In_10F_1.Presence)
                                {
                                    clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Close);
                                    clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知關門> {10} F , 情境4 => 寫值成功！ ");
                                    continue;

                                }

                                // (滾進電梯的單板情境) 若是門開 且靠近電梯2個CV都沒貨了, 就通知關門  
                                else if (iDoorStatus == 2 && !cvr_CV_In_10F_1.Presence && !cvr_CV_In_10F_2.Presence && !cvr_Ele_LI1_01.Presence && !cvr_Ele_LI1_04.Presence)
                                {
                                    //檢查第二格是不是要到這個樓層
                                    //if (cvr_Ele_LI1_03.Presence && !ChkFloorByCmd(cvr_Ele_LI1_02.CommandID, iCurrentFloor))
                                    if (!cvr_Ele_LI1_02.Presence && string.IsNullOrEmpty(cvr_Ele_LI1_02.CommandID))
                                    {
                                        clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Close);
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知關門> {10} F , 情境5 => 寫值成功！ ");
                                        continue;
                                    }
                                    else
                                    {
                                        goto label_10_1;
                                    }

                                }

                            label_10_1:
                                //(沒有貨物要滾出.滾入) 若是門開 且2個CV都沒貨了, 就通知關門   
                                if (iDoorStatus == 2 && !cvr_CV_In_10F_1.Presence && !cvr_CV_In_10F_2.Presence && !cvr_Ele_LI1_01.Presence)
                                {
                                    //檢查第二格是不是要到這個樓層
                                    //if (cvr_Ele_LI1_03.Presence && !ChkFloorByCmd(cvr_Ele_LI1_02.CommandID, iCurrentFloor))
                                    if (!cvr_Ele_LI1_02.Presence && string.IsNullOrEmpty(cvr_Ele_LI1_02.CommandID))
                                    {
                                        clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Close);
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知關門> {10} F , 情境6 => 寫值成功！ ");
                                        continue;
                                    }
                                    else
                                    {
                                        goto label_10_2;
                                    }

                                }

                            label_10_2:
                                //電梯內只有一版 , 且電梯開門 , 貨物已經在指定樓層 , 通知往前滾動
                                if (iDoorStatus == 2 && !cvr_Ele_LI1_01.Presence && cvr_Ele_LI1_02.Presence && cvr_Ele_LI1_01.RollNotice_PC == 0 && cvr_Ele_LI1_02.RollNotice_PC == 0) //&& !cvr_CV_In_10F_3.Presence 
                                {
                                    if (cvr_Ele_LI1_02.InfoRolling().Result)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer_Ele_LI1_03.BufferName} <通知滾動>   => 寫值成功！ ");
                                        continue;
                                    }

                                }

                                //電梯內只有一版 , 且電梯關門 , 通知往前滾動
                                else if (iDoorStatus == 1 && !cvr_Ele_LI1_04.Presence && cvr_Ele_LI1_03.Presence && cvr_Ele_LI1_03.RollNotice_PC == 0)
                                {
                                    if (cvr_Ele_LI1_03.InfoRolling().Result)
                                    {
                                        clsWriLog.Log.FunWriTraceLog_CV($"<Buffer> {buffer_Ele_LI1_02.BufferName} <通知滾動>   => 寫值成功！ ");
                                        continue;
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

        private string GetCmdSnoByFloor(int iFloor)
        {
            string sCmdSno = "";
            ConveyorInfo buffer_CV_Out_10F = ConveyorDef.LO1_05;
            ConveyorInfo buffer_CV_Out_8F = ConveyorDef.LO2_03;

            switch (iFloor)
            {
                case 10:
                    var cvr_CV_Out_10F = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_CV_Out_10F.Index);
                    sCmdSno = cvr_CV_Out_10F.CommandID;
                    break;
                
                default:
                    var cvr_CV_Out_8F = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_CV_Out_8F.Index);
                    sCmdSno = cvr_CV_Out_8F.CommandID;
                    break;
             
            }

            return sCmdSno;
        }

        private int GetCarrierTypeByFloor(int iFloor)
        {
            int iCarrierType = 0;
            ConveyorInfo buffer_CV_Out_10F = ConveyorDef.LO1_05;           
            ConveyorInfo buffer_CV_Out_8F = ConveyorDef.LO2_03;

            switch (iFloor)
            {
                case 10:
                    var cvr_CV_Out_10F = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_CV_Out_10F.Index);
                    iCarrierType = cvr_CV_Out_10F.CarrierType;
                    break;
               
                default:
                    var cvr_CV_Out_8F = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_CV_Out_8F.Index);
                    iCarrierType = cvr_CV_Out_8F.CarrierType;
                    break;
            }

            return iCarrierType;
        }


         
        private bool ChkReadyStsByFloor(int iFloor)
        {
            bool bFlag = false;
          
            ConveyorInfo buffer_CV_Out_10F = ConveyorDef.LO1_05;            
            ConveyorInfo buffer_CV_Out_8F = ConveyorDef.LO2_03;

            switch (iFloor)
            {
                case 10:
                    var cvr_CV_Out_3F = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_CV_Out_10F.Index);
                    if (!cvr_CV_Out_3F.Presence && cvr_CV_Out_3F.RollNotice_PC == 0 && cvr_CV_Out_3F.RollNotice == 0 && cvr_CV_Out_3F.Ready == (int)clsEnum.Ready.IN)
                    {
                        bFlag = true;
                    }
                   
                    break;
                
                default:
                    var cvr_CV_Out_8F = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_CV_Out_8F.Index);
                    if (!cvr_CV_Out_8F.Presence && cvr_CV_Out_8F.RollNotice_PC == 0 && cvr_CV_Out_8F.RollNotice == 0 && cvr_CV_Out_8F.Ready == (int)clsEnum.Ready.IN)
                    {
                        bFlag = true;
                    }
                    break;
            }

            return bFlag;
        }

        private bool ChkFloorByCmd(string sCmdSno, int iFloor)
        {
            TaskInfo task = new TaskInfo();
            int iRet = 0;
            int iTo = 0;

            iRet = clsTask.CheckHasTaskCmd(sCmdSno, ref task);
            if (iRet != DBResult.Success)
            {
                //clsWriLog.Log.FunWriTraceLog_CV($"NG: <Buffer> {buffer_Ele_In.BufferName} <Cmd> {sCmdSno} => 取得命令資料失敗！");
                return false;
            }
            iTo = clsLiteOnCV.GetFloorByBufferID(task.Destination);

            if (iTo == iFloor)
            {
                return true;
            }
            else
            { return false; }

        }
    }
}
