﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.Event.V2BYMA30.Models
{
    public class ReturnTRANSFER_COMMAND_REQUEST
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "TRANSFER_COMMAND_REQUEST";
        public string returnCode { get; set; }

        public string returnComment { get; set; }
    }
}
