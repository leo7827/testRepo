using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.V2BYMA30.ReportInfo
{
    public class CMD_DESTINATION_CHECKInfo
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "CMD_DESTINATION_CHECK";
        public string location { get; set; }
    }
}
