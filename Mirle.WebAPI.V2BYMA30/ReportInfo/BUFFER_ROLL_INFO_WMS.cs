using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.V2BYMA30.ReportInfo
{
    public class BUFFER_ROLL_INFO_WMS
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "BUFFER_ROLL_INFO";
        public string bufferId { get; set; }

        public string returnCode { get; set; }
        public string returnComment { get; set; }
    }
}
