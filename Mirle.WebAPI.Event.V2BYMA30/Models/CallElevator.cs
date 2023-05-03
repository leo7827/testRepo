using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.WebAPI.Event.V2BYMA30.Models
{
    public class CallElevator
    {
        public string HostName { get; set; }
        public string JobId { get; set; }
        public string TransactionID { get; set; } = "";
        public string CmdSno { get; set; }

        public int EquNo { get; set; }

        public int FloorFrom { get; set; }

        public int FloorTo { get; set; }
    }
}
