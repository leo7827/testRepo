﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.Event.V2BYMA30.Models
{
    public class EMPTY_BIN_LOAD_Done_INFO
    {
        public string jobId { get; set; }
        public string transactionId { get; set; } = "EMPTY_BIN_LOAD_Done";

        public string location { get; set; }
    }
}
