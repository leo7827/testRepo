using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Config.Net;
using Mirle.OpcClient;
using Mirle.LiteOn.V2BYMA30;
using Mirle.LiteOn.V2BYMA30.Config;

namespace Mirle.LiteOn.V2BYMA30
{
    public class clsOPCStart
    {
        public readonly OpcClientManager opcManager = new OpcClientManager();
        private Timer OPCTimer = new Timer();
        readonly Dictionary<int, string> ConveyorGroupIndexList = new Dictionary<int, string>();
        private IOPC ini;
        int TimeInterval = 3000;
        public clsOPCStart()
        {
            GetConfig();
        }
        public void Initialize()
        {
            //opcManager.Start();
            OPCTimer.Interval = TimeInterval;
            OPCTimer.Tick += new EventHandler(SendToOpc);
            //OPCTimer.Enabled = true;
        }

        public void Stop()
        {
            opcManager.Stop();
            OPCTimer.Enabled = false;
        }

        public void Start()
        {
            opcManager.Start();
            OPCTimer.Enabled = true;
        }
        public void GetConfig()
        {
            string startpoint = Application.StartupPath + "\\Config\\OPC.ini";
            ini = new ConfigurationBuilder<IOPC>().UseIniFile(startpoint).Build();
            //ini = new ConfigurationBuilder<IOPC>().UseIniFile($@"\Config\OPC.ini").Build();
            clsWriLog.Log.FunWriTraceLog_CV("Start Config: " + startpoint);

            TimeInterval = ini.TimeInterval.TimeInterval > 0 ? ini.TimeInterval.TimeInterval : TimeInterval;
            clsWriLog.Log.FunWriTraceLog_CV("Time Interval: " + ini.TimeInterval.TimeInterval);
            if (ini.Conveyors.E04CV10F == true)
            {
                ConveyorGroupIndexList.Add(1, "1FE04CV");
                clsWriLog.Log.FunWriTraceLog_CV("Load: 1FE04CV");
            }

            if (ini.Conveyors.E04Lift == true)
            {
                ConveyorGroupIndexList.Add(2, "8FE04Lift");
                clsWriLog.Log.FunWriTraceLog_CV("Load: 8FE04Lift");
            }
            if (ini.Conveyors.E04CV8F == true)
            {
                ConveyorGroupIndexList.Add(3, "8FE04CV");
                clsWriLog.Log.FunWriTraceLog_CV("Load: 8FE04CV");
            }
        }

        public void SendToOpc(object sender, EventArgs e)
        {
            OPCTimer.Enabled = false;
            try
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (ConveyorGroupIndexList.ContainsKey(i))
                    {
                        string sendValue = "";
                        switch (i)
                        { 
                            case 1:
                                sendValue = string.Join(",", clsLiteOnCV.GetConveyorController_10F().OpcData);
                                opcManager.Add(ConveyorGroupIndexList[i], sendValue);
                                break;

                            case 2:
                                sendValue = string.Join(",", clsLiteOnCV.GetConveyorController_Elevator().OpcData);
                                opcManager.Add(ConveyorGroupIndexList[i], sendValue);
                                break;
                            case 3:
                                sendValue = string.Join(",", clsLiteOnCV.GetConveyorController_8F().OpcData);
                                opcManager.Add(ConveyorGroupIndexList[i], sendValue);
                                break;
                            default:
                                break;
                        }


                        //sendValue = string.Join(",", clsLiteOnCV.GetConveyorController().OpcData);
                        //string sendValue = string.Join(",", clsOSMTCStart.GetControllerHost().GetCVCManager(i).OpcData);
                        //opcManager.Add(ConveyorGroupIndexList[i], sendValue);
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
                OPCTimer.Enabled = true;
            }
        }
    }
}
