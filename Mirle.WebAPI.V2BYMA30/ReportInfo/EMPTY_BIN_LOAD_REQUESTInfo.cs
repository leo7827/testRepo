using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.V2BYMA30.ReportInfo
{
    public class EMPTY_BIN_LOAD_REQUESTInfo
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "EMPTY_BIN_LOAD_REQUEST";
        public string location { get; set; }
        public int reqQty { get; set; }


    }
}
