using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    public class RetrieveTransferInfo
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "RETRIEVE_TRANSFER";
        public string carrierId { get; set; }
        public string fromShelfId { get; set; }
        public string toPortId { get; set; }
        public string backupPortId { get; set; }
        public string priority { get; set; }
        public string batchId { get; set; }
    }
}
