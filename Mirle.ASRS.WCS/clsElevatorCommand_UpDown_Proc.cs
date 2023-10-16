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
using System.Collections;

namespace Mirle.ASRS.WCS
{
    /// <summary>
    /// 負責電梯的上下移動 
    /// </summary>
    public class clsElevatorCommand_UpDown_Proc
    {
        private System.Timers.Timer timRead = new System.Timers.Timer();
        private int iCount_AutoClear = 0;

        public clsElevatorCommand_UpDown_Proc()
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
                //挑選的順序是: 同樓層有貨先搬 -> 其他樓層有貨再預約
                int iFROM = 0;
                int iTO = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.Path.GetValue();   //電梯的目的樓層
                int iCurrentFloor = clsLiteOnCV.GetConveyorController_Elevator().Signal.Floor.GetValue();  //電梯當下的樓層 
                int iDoorStatus = clsLiteOnCV.GetConveyorController_Elevator().Signal.DoorStatus.GetValue();
                string sCmdSno = "";
                bool bFlag = false;
                bool bFlag10F = false;
                bool bFlag8F = false;

                //int iCount_AutoClear = 0;

                if (!clsLiteOnCV.CheckElevatorIsAgv())
                {
                    return;
                }

                if (iCurrentFloor == 0)
                {
                    return;
                }

                //如果狀態不對 ,  10秒後自動清掉 PC 樓層訊號
                if (iDoorStatus == 2 && iTO != 0)
                {
                    if (iCount_AutoClear > 20)
                    {
                        clsLiteOnCV.GetConveyorController_Elevator().SetFloor(0);
                        iCount_AutoClear = 0;
                        clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  通知樓層PC清值 => 寫值成功！ ");
                        return;
                    }
                    else
                    {
                        iCount_AutoClear++;
                        return;
                    }
                }

                //電梯在動時先不派工
                if (iTO != 0)
                {
                    return;
                }

                if (iDoorStatus != 1)
                {
                    return;
                }
               

                #region  CV條件

                ConveyorInfo buffer1 = new ConveyorInfo();  //buffer 滾入1
                ConveyorInfo buffer2 = new ConveyorInfo();  //buffer 滾入2
                ConveyorInfo buffer3 = new ConveyorInfo();  //buffer 滾出1
                ConveyorInfo buffer4 = new ConveyorInfo();  //buffer 滾出2

                ConveyorInfo buffer_8F_In1 = ConveyorDef.LO2_02;  //8F buffer 滾入的第一節
                ConveyorInfo buffer_8F_In2 = ConveyorDef.LO2_01;  //8F buffer 滾入的第二節
                ConveyorInfo buffer_10F_In1 = ConveyorDef.LO1_04;  //10F buffer 滾入的第一節
                ConveyorInfo buffer_10F_In2 = ConveyorDef.LO1_03;  //10F buffer 滾入的第二節

                ConveyorInfo buffer_8F_Out1 = ConveyorDef.LO2_03;  //8F buffer 滾出的第一節
                ConveyorInfo buffer_8F_Out2 = ConveyorDef.LO2_04;  //8F buffer 滾出的第二節
                ConveyorInfo buffer_10F_Out1 = ConveyorDef.LO1_05;  //10F buffer 滾出的第一節
                ConveyorInfo buffer_10F_Out2 = ConveyorDef.LO1_06;  //10F buffer 滾出的第二節

                ConveyorInfo buffer_Elevator_LI1_03 = ConveyorDef.LI1_03;  //電梯 入庫第一節
                ConveyorInfo buffer_Elevator_LI1_04 = ConveyorDef.LI1_04;  //電梯 入庫第二節 
                ConveyorInfo buffer_Elevator_LI1_02 = ConveyorDef.LI1_02;  //電梯 出庫第一節
                ConveyorInfo buffer_Elevator_LI1_01 = ConveyorDef.LI1_01;  //電梯 出庫第二節

                
                var cv_8F_In1 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_8F_In1.Index);
                var cv_8F_In2 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_8F_In2.Index);
                var cv_10F_In1 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_10F_In1.Index);                
                var cv_10F_In2 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_10F_In2.Index);

                var cv_8F_Out1 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_8F_Out1.Index);
                var cv_8F_Out2 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_8F_Out2.Index);
                var cv_10F_Out1 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_10F_Out1.Index);                
                var cv_10F_Out2 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_10F_Out2.Index);
               
                var cv_Elevator_LI1_03 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Elevator_LI1_03.Index);  //In
                var cv_Elevator_LI1_04 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Elevator_LI1_04.Index);  //In2
                var cv_Elevator_LI1_02 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Elevator_LI1_02.Index);  //Out
                var cv_Elevator_LI1_01 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Elevator_LI1_01.Index);  //Out2

                //電梯內有貨物要出到10樓
                //if ( (cv_Elevator_Out2.Presence || cv_Elevator_Out1.Presence) && !cv_10F_Out1.Presence)
                if (iCurrentFloor == 10 &&(cv_Elevator_LI1_01.Presence || cv_Elevator_LI1_02.Presence))
                {
                    if (( ChkEleCmd() + Chk10FCmd()) > 6)
                    {
                        bFlag = false;
                    }
                    else
                    {
                        bFlag = true;
                    }
                }

                //10樓有貨物要進電梯
                //if (cv_10F_In1.Presence && !cv_Elevator_In2.Presence)
                if (iCurrentFloor == 10 && !cv_Elevator_LI1_04.Presence && cv_10F_In1.Presence)
                {
                    if ((ChkEleCmd() + Chk10FCmd()) > 6)
                    {
                        bFlag = false;
                    }
                    else
                    {
                        bFlag = true;
                    }                    
                }

                //電梯內有貨物要出到八樓
                //if ( (cv_Elevator_In1.Presence || cv_Elevator_In2.Presence) && !cv_8F_Out1.Presence)
                if (iCurrentFloor == 8 && (cv_Elevator_LI1_03.Presence || cv_Elevator_LI1_04.Presence))
                {
                    if ((ChkEleCmd() + Chk8FCmd()) > 6)
                    {
                        bFlag = false;
                    }
                    else
                    {
                        bFlag = true;
                    }                   
                }

                //八樓有貨物要進電梯
                //if (cv_8F_In1.Presence && !cv_Elevator_Out2.Presence)
                if (iCurrentFloor == 8 && !cv_Elevator_LI1_01.Presence && cv_8F_In1.Presence)
                {
                    if ((ChkEleCmd() + Chk8FCmd()) > 6)
                    {
                        bFlag = false;
                    }
                    else
                    {
                        bFlag = true;
                    }                     
                }
                #endregion
                 
               
 
                /*
                //電梯閒置時 ,  當下樓層的conveyor先找看有無貨物 , 若有就不移動
                clsLiteOnCV.ReportCurrentConveyor(iCurrentFloor, ref buffer1, ref buffer2);
 
                switch (iCurrentFloor)
                {
                    case 10:
                        var cv10F_1 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer1.Index);
                        var cv10F_2 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer2.Index);
                        if (cv10F_1.Presence || cv10F_2.Presence)
                        {
                            bFlag = true;
                        }
                        var cv10F_3 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Elevator_LI1_02.Index);
                        var cv10F_4 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Elevator_LI1_01.Index);
                        if (cv10F_3.Presence && cv10F_4.Presence)
                        {
                            bFlag = true;
                        }

                        //bFlag = true;
                        break;
                    case 8:
                        var cv8F_1 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer1.Index);
                        var cv8F_2 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer2.Index);

                        if (cv8F_1.Presence || cv8F_2.Presence)
                        {
                            bFlag = true;
                        }
                        var cv8F_3 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Elevator_LI1_03.Index);
                        var cv8F_4 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Elevator_LI1_04.Index);
                        if (cv8F_3.Presence && cv8F_4.Presence)
                        {
                            bFlag = true;
                        }
                        break;
                    default:
                       

                        //bFlag = false;
                        break;
                }
                */

                if (bFlag)
                {
                    return;
                } 

                switch (iCurrentFloor)
                {          
                    //移動到10樓
                    case 8:
                        if ((cv_Elevator_LI1_01.Presence || cv_Elevator_LI1_02.Presence) || !string.IsNullOrEmpty(cv_10F_In1.CommandID) )
                        {
                            clsLiteOnCV.GetConveyorController_Elevator().SetFloor(clsConstValue.FloorPath.Floor_10);
                            clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知移動> {10} F => 寫值成功！ ");
                            iCount_AutoClear = 0;
                        } 

                        break;

                    //移動到八樓
                    case 10:                    
                        if ((cv_Elevator_LI1_03.Presence || cv_Elevator_LI1_04.Presence) || !string.IsNullOrEmpty(cv_8F_In1.CommandID) )
                        {
                            clsLiteOnCV.GetConveyorController_Elevator().SetFloor(clsConstValue.FloorPath.Floor_8);
                            clsWriLog.Log.FunWriTraceLog_CV($"<Elevator>  <通知移動> {8} F => 寫值成功！ ");
                            iCount_AutoClear = 0;
                        }

                        break;

                    default:
                        break;
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

        private int Chk8FCmd()
        {
            int iCount = 0;

            ConveyorInfo buffer_8F_1 = ConveyorDef.LO2_01;
            ConveyorInfo buffer_8F_2 = ConveyorDef.LO2_02;
            ConveyorInfo buffer_8F_3 = ConveyorDef.LO2_03;
            ConveyorInfo buffer_8F_4 = ConveyorDef.LO2_04;

            var cvr_CV_8F_1 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_8F_1.Index);  //LO2_01
            var cvr_CV_8F_2 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_8F_2.Index);  //LO2_02
            var cvr_CV_8F_3 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_8F_3.Index);  //LO2_03
            var cvr_CV_8F_4 = clsLiteOnCV.GetConveyorController_8F().GetBuffer(buffer_8F_4.Index);  //LO2_04

            if (cvr_CV_8F_1.Presence && !string.IsNullOrEmpty(cvr_CV_8F_1.CommandID))
            {
                iCount++;
            }

            if (cvr_CV_8F_2.Presence && !string.IsNullOrEmpty(cvr_CV_8F_2.CommandID))
            {
                iCount++;
            }

            if (cvr_CV_8F_3.Presence && !string.IsNullOrEmpty(cvr_CV_8F_3.CommandID))
            {
                iCount++;
            }

            if (cvr_CV_8F_4.Presence && !string.IsNullOrEmpty(cvr_CV_8F_4.CommandID))
            {
                iCount++;
            }

            return iCount;
        }

        private int Chk10FCmd()
        {
            int iCount = 0;

            ConveyorInfo buffer_10F_1 = ConveyorDef.LO1_03;
            ConveyorInfo buffer_10F_2 = ConveyorDef.LO1_04;
            ConveyorInfo buffer_10F_3 = ConveyorDef.LO1_05;
            ConveyorInfo buffer_10F_4 = ConveyorDef.LO1_06;

            var cvr_CV_10F_1 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_10F_1.Index);  //LO1_03
            var cvr_CV_10F_2 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_10F_2.Index);  //LO1_04
            var cvr_CV_10F_3 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_10F_3.Index);  //LO1_05
            var cvr_CV_10F_4 = clsLiteOnCV.GetConveyorController_10F().GetBuffer(buffer_10F_4.Index);  //LO1_06

            if (cvr_CV_10F_1.Presence && !string.IsNullOrEmpty(cvr_CV_10F_1.CommandID))
            {
                iCount++;
            }

            if (cvr_CV_10F_2.Presence && !string.IsNullOrEmpty(cvr_CV_10F_2.CommandID))
            {
                iCount++;
            }

            if (cvr_CV_10F_3.Presence && !string.IsNullOrEmpty(cvr_CV_10F_3.CommandID))
            {
                iCount++;
            }

            if (cvr_CV_10F_4.Presence && !string.IsNullOrEmpty(cvr_CV_10F_4.CommandID))
            {
                iCount++;
            }

            return iCount;
        }

        private int ChkEleCmd()
        {
            int iCount = 0;

            ConveyorInfo buffer_Ele_01 = ConveyorDef.LI1_01;
            ConveyorInfo buffer_Ele_02 = ConveyorDef.LI1_02;
            ConveyorInfo buffer_Ele_03 = ConveyorDef.LI1_03;
            ConveyorInfo buffer_Ele_04 = ConveyorDef.LI1_04;

            var cvr_Ele_01 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_01.Index);  //LI1_01
            var cvr_Ele_02 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_02.Index);  //LI1_02
            var cvr_Ele_03 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_03.Index);  //LI1_03
            var cvr_Ele_04 = clsLiteOnCV.GetConveyorController_Elevator().GetBuffer(buffer_Ele_04.Index);  //LI1_04

            if (cvr_Ele_01.Presence && !string.IsNullOrEmpty(cvr_Ele_01.CommandID))
            {
                iCount++;
            }

            if (cvr_Ele_02.Presence && !string.IsNullOrEmpty(cvr_Ele_02.CommandID))
            {
                iCount++;
            }

            if (cvr_Ele_03.Presence && !string.IsNullOrEmpty(cvr_Ele_03.CommandID))
            {
                iCount++;
            }

            if (cvr_Ele_04.Presence && !string.IsNullOrEmpty(cvr_Ele_04.CommandID))
            {
                iCount++;
            }

            return iCount;
        }

    }
}
