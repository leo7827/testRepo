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
    public class clsManualOperate_Proc
    {
        /// <summary>
        /// 電梯當下非AGV模式 , 就清掉自身PC 值
        /// </summary>
        private System.Timers.Timer timRead = new System.Timers.Timer();

        public clsManualOperate_Proc()
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
                int iDoorStatus_PC = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.DoorNoticce.GetValue();
                int iTO = clsLiteOnCV.GetConveyorController_Elevator().Signal.Controller.Path.GetValue();   //電梯的目的樓層      

                if (!clsLiteOnCV.GetConveyorController_Elevator().IsConnected)
                {
                    return;
                }

                if (clsLiteOnCV.CheckElevatorIsAgv())
                {
                    return;
                }

                if (iDoorStatus_PC != 0)
                {
                    clsLiteOnCV.GetConveyorController_Elevator().WriteDoorIndex(clsConstValue.DoorStatus.Ini);
                    clsWriLog.Log.FunWriTraceLog_CV($"非AGV 模式 <Elevator>  <通知關門清值> => 寫值成功！ ");
                    return;
                }

                if (iTO != 0)
                {
                    clsLiteOnCV.GetConveyorController_Elevator().SetFloor(0);
                    clsWriLog.Log.FunWriTraceLog_CV($"非AGV 模式 <Elevator>  <通知樓層清值> => 寫值成功！ ");
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
