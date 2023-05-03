using System;
using System.Net.Http;
using Newtonsoft.Json;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Mirle.Def;
using System.Net.Http.Headers;
using System.Text;

namespace Mirle.WebAPI.V2BYMA30.Function
{
    public class PositionReport
    {
        private WebApiConfig _config = new WebApiConfig();
        public PositionReport(WebApiConfig Config)
        {
            _config = Config;
        }

        public bool FunReport(POSITION_REPORTInfo info)
        {
            try
            {
                string strJson = JsonConvert.SerializeObject(info);
                clsWriLog.Log.FunWriTraceLog_CV(strJson);
                string sLink = $"http://{_config.IP}/WCS/POSITION_REPORT";
                clsWriLog.Log.FunWriTraceLog_CV($"URL: {sLink}");
                string re = clsTool.HttpPost(sLink, strJson);
                clsWriLog.Log.FunWriTraceLog_CV(re);
                //HttpResponseMessage response = null;
                //using (HttpClient client = new HttpClient())
                //{
                //    using (var request = new HttpRequestMessage(HttpMethod.Post, sLink))
                //    {
                //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //        //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", System.Web.HttpContext.Current.Session["WebApiAccessToken"].ToString());
                //        var data = new StringContent(strJson, Encoding.UTF8, "application/json");
                //        request.Content = data;
                //        response = client.SendAsync(request).Result;
                //        clsWriLog.Log.FunWriTraceLog_CV(response.Content.ReadAsStringAsync().Result);
                //    }

                //    //var result2 = client.PostAsJsonAsync(sLink, info).Result;
                //}

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
