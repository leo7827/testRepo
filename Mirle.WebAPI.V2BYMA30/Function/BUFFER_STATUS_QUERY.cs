using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirle.Def;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Newtonsoft.Json;

namespace Mirle.WebAPI.V2BYMA30.Function
{
    public class BUFFER_STATUS_QUERY
    {
        private WebApiConfig _config = new WebApiConfig();
        public BUFFER_STATUS_QUERY(WebApiConfig Config)
        {
            _config = Config;
        }

        public bool FunReport(BUFFER_STATUS_QUERYInfo info, ref BUFFER_STATUS_QUERYInfo_Response response)
        {
            try
            {
                string strJson = JsonConvert.SerializeObject(info);
                clsWriLog.Log.FunWriTraceLog_CV(strJson);
                string sLink = $"http://{_config.IP}/LIFT4C/BUFFER_STATUS_QUERY/";
                clsWriLog.Log.FunWriTraceLog_CV($"URL: {sLink}");
                string re = clsTool.HttpPost(sLink, strJson);
                clsWriLog.Log.FunWriTraceLog_CV(re);
                response = (BUFFER_STATUS_QUERYInfo_Response)Newtonsoft.Json.Linq.JObject.Parse(re).ToObject(typeof(BUFFER_STATUS_QUERYInfo_Response));
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
