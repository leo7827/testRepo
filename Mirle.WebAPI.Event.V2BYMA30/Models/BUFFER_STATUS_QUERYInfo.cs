using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.Event.V2BYMA30.Models
{
    public class BUFFER_STATUS_QUERYInfo
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "BUFFER_STATUS_QUERY";

        public string bufferId { get; set; }
    }
}
