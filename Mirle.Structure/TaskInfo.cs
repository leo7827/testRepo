using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.Structure
{
    public class TaskInfo
    {
        public string CmdSno { get; set; }
        public string CmdSts { get; set; }
        public string Prty { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string CmdMode { get; set; }

        /// <summary>
        /// 命令產生時間
        /// </summary>
        public string CrtDate { get; set; }
        /// <summary>
        /// 命令送出時間
        /// </summary>
        public string ExpDate { get; set; }
        public string EndDate { get; set; }

        public string CarrierType { get; set; }

        /// <summary>
        /// 當前位置
        /// </summary>
        public string CurLoc { get; set; }
        public string Remark { get; set; }

    }
}
