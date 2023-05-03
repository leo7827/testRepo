using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Mirle.Def;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Newtonsoft.Json;


namespace Mirle.WebAPI.V2BYMA30.Function
{
    public class CV_WRITE_CMD
    {
        private WebApiConfig _config = new WebApiConfig();
        public CV_WRITE_CMD(WebApiConfig Config)
        {
            _config = Config;
        }

        //public bool FunReport(CV_WRITE_CMDInfo info)
        //{
        //    try
        //    {
        //        string strJson = JsonConvert.SerializeObject(info);
        //        clsWriLog.Log.FunWriTraceLog_CV(strJson);
        //        string sLink = $"http://{_config.IP}/LIFT5C/CV_WRITE_CMD";
        //        clsWriLog.Log.FunWriTraceLog_CV($"URL: {sLink}");
        //        string re = clsTool.HttpPost(sLink, strJson);
        //        clsWriLog.Log.FunWriTraceLog_CV(re);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        var cmet = System.Reflection.MethodBase.GetCurrentMethod();
        //        clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
        //        return false;
        //    }
        //}
    }
}
