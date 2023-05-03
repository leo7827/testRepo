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
    public class clsPLCModeChange8F_Proc
    {
        private System.Timers.Timer timRead = new System.Timers.Timer();
        public clsPLCModeChange8F_Proc()
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
                int iModeStatus = clsLiteOnCV.GetConveyorController_8F().Signal.ModeStatus.GetValue();
                int iModeStatus_PC = clsLiteOnCV.GetConveyorController_8F().Signal.Controller.ModeChange.GetValue();

                int iModeStatus_Ele = clsLiteOnCV.GetConveyorController_Elevator().Signal.ModeStatus.GetValue();
                int iModeStatus_PC_Ele = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.ModeChange.GetValue();

                //int iCurrentFloor = clsLiteOnCV.CheckElevatorFloor();
                bool bAgvmode = clsLiteOnCV.GetConveyorController_Elevator().Signal.EleStatus.AgvMode.IsOn();
                bool bFlag = false;
                bool bFlag_Ele = false;

                //這邊看要不要在卡電梯的狀態防呆?  然後卡10樓的狀態
                if (bAgvmode && iModeStatus ==1 && iModeStatus_PC ==0)
                {
                    //bFlag = clsLiteOnCV.GetConveyorController_Elevator().WriteModeChange(1);
                    bFlag = clsLiteOnCV.GetConveyorController_8F().WriteModeChange(1);

                    if (bFlag)
                    {
                        //create 
                        return;
                    }

                }

                // if (plc.ModeStatus.GetValue() == 0 && ctrl.ModeChange.GetValue() ==1 )
                if (bAgvmode && iModeStatus == 0 && iModeStatus_PC == 1 && iModeStatus_Ele ==0  && iModeStatus_PC_Ele ==0 )
                {

                    //對電梯改寫狀態
                    //bFlag = clsLiteOnCV.GetConveyorController_Elevator().WriteModeChange(1);
                    bFlag_Ele = clsLiteOnCV.GetConveyorController_Elevator().WriteModeChange(1);
                    if (bFlag)
                    {
                        //create 
                        return;
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
