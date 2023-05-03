using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.V2BYMA30.ReportInfo
{
    public class BUFFER_STATUS_QUERYInfo_Response
    {
        public string jobId { get; set; }
        public string transactionId { get; set; }

        public string bufferId { get; set; }

        public string ready { get; set; }
        public string returnCode { get; set; }

        public string returnComment { get; set; }
    }
}
