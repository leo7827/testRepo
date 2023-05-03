using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.Event.V2BYMA30.Models
{
    public class ReturnBUFFER_STATUS_QUERYInfo
    {
        public string jobId { get; set; }
        public string transactionId { get; set; }

        public string bufferId { get; set; }
        public string isLoad { get; set; }
        public string ready { get; set; }
        public string isEmpty { get; set; }

        public string stbSts { get; set; }
        public string returnCode { get; set; }

        public string returnComment { get; set; }
    }
}
