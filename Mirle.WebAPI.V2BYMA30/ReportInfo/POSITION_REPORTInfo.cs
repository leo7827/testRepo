using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.V2BYMA30.ReportInfo
{
    public class POSITION_REPORTInfo
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "POSITION_REPORT";
        public string carrierType { get; set; } = "Bin ";
        public string id { get; set; }
        public string position { get; set; }
    }
}
