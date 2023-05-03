using System;
using Mirle.Structure;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Mirle.Def;
using Mirle.WebAPI.V2BYMA30;

namespace Mirle.LiteOn.V2BYMA30
{
    public class clsWmsApi
    {
        private static clsHost report;

        public static void FunInit(WebApiConfig config)
        {
            report = new clsHost(config);

            //for (int i = 0; i < StockerSts.Length; i++)
            //{
            //    StockerSts[i] = clsEnum.WmsApi.EqSts.StockOutOnly;
            //}
        }
         
         
         

        public static clsHost GetApiProcess()
        {
            return report;
        }
    }
}
