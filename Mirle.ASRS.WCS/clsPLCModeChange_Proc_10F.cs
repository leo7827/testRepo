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
    public class clsPLCModeChange_Proc_10F
    {
        public bool b10F_PC = false;
        private System.Timers.Timer timRead = new System.Timers.Timer();
        public clsPLCModeChange_Proc_10F()
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
                //電梯的訊號
                int iModeStatus_Ele = clsLiteOnCV.GetConveyorController_Elevator().Signal.ModeStatus.GetValue();
                int iModeStatus_PC_Ele = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.ModeChange.GetValue();
                int iDoorStatus = clsLiteOnCV.GetConveyorController_Elevator().Signal.DoorStatus.GetValue();

                int iElePlatformOnSts = clsLiteOnCV.CheckElevatorPlatformOn() ? 1 : 0;      //平台已經伸出
                bool bEleElevatorIsUp = clsLiteOnCV.CheckElevatorIsUp();
                bool bEleElevatorIsDown = clsLiteOnCV.CheckElevatorIsDown();

                //各樓層                

                int iModeStatus_10F = clsLiteOnCV.GetConveyorController_10F().Signal.ModeStatus.GetValue();
                int iModeStatus_PC_10F = clsLiteOnCV.GetConveyorController_10F().Signal.Controller.ModeChange.GetValue();

                if (iDoorStatus != 1)
                {
                    return;
                }

                if (bEleElevatorIsUp || bEleElevatorIsDown)
                {
                    return;
                }

                //10樓交握
                if (iModeStatus_10F == 1 && iModeStatus_PC_10F == 0 && !b10F_PC)
                {
                    clsLiteOnCV.GetConveyorController_10F().WriteElevatorMode(1);
                    b10F_PC = true;
                    return;
                }

                //電梯交握
                if (b10F_PC && iModeStatus_Ele == 0 && iModeStatus_PC_Ele == 0)
                {
                    clsLiteOnCV.GetConveyorController_Elevator().WriteElevatorMode(1);
                    clsWriLog.Log.FunWriTraceLog_CV($"<10F手動切換電梯模式> ！ ");
                    b10F_PC = false;
                    return;
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
