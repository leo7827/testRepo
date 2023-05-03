using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Mirle.Def;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Newtonsoft.Json;

namespace Mirle.WebAPI.V2BYMA30.Function
{
    public class CallEmptyReport
    {
        private WebApiConfig _config = new WebApiConfig();
        public CallEmptyReport(WebApiConfig Config)
        {
            _config = Config;
        }

        public bool FunReport(EMPTY_BIN_LOAD_REQUESTInfo info, ref EMPTY_BIN_LOAD_REQUESTInfo_Response response)
        {
            try
            {
                string strJson = JsonConvert.SerializeObject(info);
                clsWriLog.Log.FunWriTraceLog_CV(strJson);
                string sLink = $"http://{_config.IP}/WCS/EMPTY_BIN_LOAD_REQUEST";
                clsWriLog.Log.FunWriTraceLog_CV($"URL: {sLink}");
                string re = clsTool.HttpPost(sLink, strJson);
                clsWriLog.Log.FunWriTraceLog_CV(re);
                response = (EMPTY_BIN_LOAD_REQUESTInfo_Response)Newtonsoft.Json.Linq.JObject.Parse(re).ToObject(typeof(EMPTY_BIN_LOAD_REQUESTInfo_Response));

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
