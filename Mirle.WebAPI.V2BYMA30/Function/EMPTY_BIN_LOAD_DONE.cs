using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Mirle.Def;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Newtonsoft.Json;

namespace Mirle.WebAPI.V2BYMA30.Function
{
    public class EMPTY_BIN_LOAD_DONE
    {
        private WebApiConfig _config = new WebApiConfig();
        public EMPTY_BIN_LOAD_DONE(WebApiConfig Config)
        {
            _config = Config;
        }

        public bool funReport(EMPTY_BIN_LOAD_DONEInfo_WCS info, ref EMPTY_BIN_LOAD_DONEInfo_WMS response)
        {
            try
            {
                string strJson = JsonConvert.SerializeObject(info);
                clsWriLog.Log.FunWriTraceLog_CV(strJson);
                string sLink = $"http://{_config.IP}/LIFT4C/EMPTY_BIN_LOAD_DONE";
                clsWriLog.Log.FunWriTraceLog_CV($"URL: {sLink}");
                string re = clsTool.HttpPost(sLink, strJson);
                clsWriLog.Log.FunWriTraceLog_CV(re);
                response = (EMPTY_BIN_LOAD_DONEInfo_WMS)Newtonsoft.Json.Linq.JObject.Parse(re).ToObject(typeof(EMPTY_BIN_LOAD_DONEInfo_WMS));

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
