using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.Event.V2BYMA30.Models
{
    public class ReturnCallElevatorInfo
    {
        public string HostName { get; set; }
        public string JobID { get; set; }
        public string TransactionID { get; set; }
        public string ReturnCode { get; set; }

        public string ReturnMSG { get; set; }
    }
}
