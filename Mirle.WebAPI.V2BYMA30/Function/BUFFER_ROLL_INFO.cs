using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Mirle.Def;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Newtonsoft.Json;

namespace Mirle.WebAPI.V2BYMA30.Function
{
    public class BUFFER_ROLL_INFO
    {
        private WebApiConfig _config = new WebApiConfig();
        public BUFFER_ROLL_INFO(WebApiConfig Config)
        {
            _config = Config;
        }

        public bool funReport(BUFFER_ROLL_INFO_WCS info, ref BUFFER_ROLL_INFO_WMS response)
        {
            try
            {
                string strJson = JsonConvert.SerializeObject(info);
                clsWriLog.Log.FunWriTraceLog_CV(strJson);
                string sLink = $"http://{_config.IP}/LIFT4C/BUFFER_ROLL_INFO";
                clsWriLog.Log.FunWriTraceLog_CV($"URL: {sLink}");
                string re = clsTool.HttpPost(sLink, strJson);
                clsWriLog.Log.FunWriTraceLog_CV(re);
                response = (BUFFER_ROLL_INFO_WMS)Newtonsoft.Json.Linq.JObject.Parse(re).ToObject(typeof(BUFFER_ROLL_INFO_WMS));

                return true;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }
    }
}
