using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.V2BYMA30.ReportInfo
{
    public class BCR_CHECK_REQUESTInfo
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "BCR_CHECK_REQUEST";
        public string barcode { get; set; }
        public string carrierType { get; set; } 

        public string location { get; set; }
    }
}
