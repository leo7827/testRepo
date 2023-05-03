using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Mirle.Def;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Newtonsoft.Json;

namespace Mirle.WebAPI.V2BYMA30.Function
{
    public class CV_RECEIVE_NEW_BIN_CMD
    {
        private WebApiConfig _config = new WebApiConfig();
        public CV_RECEIVE_NEW_BIN_CMD(WebApiConfig Config)
        {
            _config = Config;
        }

        public bool funReport(CV_RECEIVE_NEW_BIN_CMDInfo_WCS info, ref CV_RECEIVE_NEW_BIN_CMDInfo_WMS response)
        {
            try
            {
                string strJson = JsonConvert.SerializeObject(info);
                clsWriLog.Log.FunWriTraceLog_CV(strJson);
                string sLink = $"http://{_config.IP}/LIFT4C/CV_RECEIVE_NEW_BIN_CMD";
                clsWriLog.Log.FunWriTraceLog_CV($"URL: {sLink}");
                string re = clsTool.HttpPost(sLink, strJson);
                clsWriLog.Log.FunWriTraceLog_CV(re);
                response = (CV_RECEIVE_NEW_BIN_CMDInfo_WMS)Newtonsoft.Json.Linq.JObject.Parse(re).ToObject(typeof(CV_RECEIVE_NEW_BIN_CMDInfo_WMS));

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
