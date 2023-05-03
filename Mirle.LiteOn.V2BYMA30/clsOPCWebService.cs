using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Config.Net;
using Mirle.OpcClient;
using Mirle.LiteOn.V2BYMA30;
using Mirle.LiteOn.V2BYMA30.Config;
using System.Threading.Tasks;
//using System.Text.Json;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace Mirle.LiteOn.V2BYMA30
{
    public class clsOPCWebService
    {
        public readonly OpcClientManager opcManager = new OpcClientManager();
        private Timer OPCTimer = new Timer();
        //readonly Dictionary<int, string> ConveyorGroupIndexList = new Dictionary<int, string>();
        private Dictionary<string, object> ConveyorGroupIndexList = new Dictionary<string, object>();
        private IOPC ini;
        int TimeInterval = 500;
        private int _serialNumber = 1;

        public clsOPCWebService()
        {
            GetConfig();
        }
        public void Initialize()
        {
            //opcManager.Start();
            OPCTimer.Interval = TimeInterval;
            OPCTimer.Tick += new EventHandler(SendToOpc);
            OPCTimer.Enabled = true;
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

            ConveyorGroupIndexList.Add("1FE04CV", " "); 

            ConveyorGroupIndexList.Add("8FE04Lift", " ");

            ConveyorGroupIndexList.Add("8FE04CV", " ");
        }

        public void SendToOpc(object sender, EventArgs e)
        {
            OPCTimer.Enabled = false;
            try
            {
                string sendValue = "";
                //Dictionary<string, object> ConveyorGroupIndexList = new Dictionary<string, object>();

                if (ConveyorGroupIndexList.ContainsKey("1FE04CV"))
                {
                    sendValue = "";
                    sendValue = string.Join(",", clsLiteOnCV.GetConveyorController_10F().OpcData);
                    ConveyorGroupIndexList["1FE04CV"] = sendValue;
                } 

                if (ConveyorGroupIndexList.ContainsKey("8FE04Lift"))
                {
                    sendValue = "";
                    sendValue = string.Join(",", clsLiteOnCV.GetConveyorController_Elevator().OpcData);
                    ConveyorGroupIndexList["8FE04Lift"] = sendValue;
                }

                if (ConveyorGroupIndexList.ContainsKey("8FE04CV"))
                {
                    sendValue = "";
                    sendValue = string.Join(",", clsLiteOnCV.GetConveyorController_8F().OpcData);
                    ConveyorGroupIndexList["8FE04CV"] = sendValue;
                }
  
                OpcData(ConveyorGroupIndexList);
                 
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

        private void PostAsync2(string url, IDictionary<string, object> Values)
        {
            string serialId = GetSerialId();
            try
            {
                //var options = new JsonSerializerOptions() { WriteIndented = true};
                //string jsonstring = JsonSerializer.Serializ(Values, options);
                string jsonstring = JsonConvert.SerializeObject(Values);

                WebApiSendTrace(serialId, jsonstring);

                using (var client = new HttpClient { BaseAddress = new Uri(url) })
                {
                    using (var contenPost = new StringContent(jsonstring, Encoding.UTF8, "application/json"))
                    {
                        using (var request = client.PostAsync(url, contenPost).GetAwaiter().GetResult())
                        {
                            var respone = request.Content.ReadAsStringAsync().Result;
                            WebApiReceiveTrace(serialId, respone);
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
        }

        private void OpcData(IDictionary<string, object> values)
        {
            var url = $"{Uri.UriSchemeHttp}://172.18.135.29/api/liteon_opc_data.php";
            PostAsync2(url, values);
        }

        //public Task OpcData(IDictionary<string, object> values) 
        //{
        //    return Task.Run(() =>
        //    {
        //        var url = $"{Uri.UriSchemeHttp}://172.18.135.29/api/liteon_opc_data.php";
        //        PostAsync2(url,values);
        //    });
        //}

        private string GetSerialId()
        {
            if (_serialNumber > 9999)
            {
                _serialNumber = 1;
            }

            Random iRan = new Random();
            return $"{iRan.Next(9999)}{_serialNumber++.ToString().PadLeft(4, '0')}";
        }

        private void WebApiSendTrace(string serialNumber, string jsonString)
        {
            try
            {
                clsWriLog.Log.FunWriTraceLog_CV($"Send({serialNumber}){Environment.NewLine}{jsonString}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{ex}");
            }
        }
        private void WebApiReceiveTrace(string serialNumber, string jsonString)
        {
            try
            {
                clsWriLog.Log.FunWriTraceLog_CV($"Receive({serialNumber}){Environment.NewLine}{jsonString}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{ex}");
            }
        }
    }
}
