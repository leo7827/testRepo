using Mirle.Def;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Mirle.ASRS.Conveyor.V2BYMA30_3F;

namespace Mirle.WebAPI.V2BYMA30.Function
{
    public class CV_BUFFER_QUERY
    {
        private WebApiConfig _config = new WebApiConfig();
        public CV_BUFFER_QUERY(WebApiConfig Config)
        {
            _config = Config;
        }

        //public bool FunReport(ref ConveyorController conveyorController)
        //{
        //    try
        //    {
        //        string sLink = $"http://{_config.IP}/WCS/CV_BUFFER_QUERY";
        //        string re = clsTool.HttpPost(sLink);
        //        conveyorController = (ConveyorController)Newtonsoft.Json.Linq.JObject.Parse(re).ToObject(typeof(ConveyorController));
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
