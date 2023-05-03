using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.V2BYMA30.ReportInfo
{
    public class ALARM_HAPPEN_REPORTInfo
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "ALARM_HAPPEN_REPORT";

        public string deviceId { get; set; }
        public string alarmCode { get; set; }
        public string alarmDef { get; set; }

        public string bufferId { get; set; }


        public string status { get; set; }
        public string happenTime { get; set; }
    }
}
