using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.V2BYMA30.ReportInfo
{
    public class EMPTY_BIN_LOAD_DONEInfo_WMS
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "EMPTY_BIN_LOAD_DONE";

        public string returnCode { get; set; }
        public string returnComment { get; set; }
    }
}
