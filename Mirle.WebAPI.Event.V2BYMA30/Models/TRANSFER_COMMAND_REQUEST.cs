using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.Event.V2BYMA30.Models
{
    public class TRANSFER_COMMAND_REQUEST
    {      
        public string jobId { get; set; }
        public string transactionId { get; set; } = "TRANSFER_COMMAND_REQUEST";
        public string binId { get; set; }

        public string ioType { get; set; }

        public string source { get; set; }

        public string destination { get; set; }
    }
}
