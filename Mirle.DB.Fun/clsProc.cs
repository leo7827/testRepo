using System;
using Mirle.Def;
using Mirle.Def.V2BYMA30;
using Mirle.DataBase;
using Mirle.LiteOn.V2BYMA30;
using Mirle.Structure.Info;
using Mirle.Structure;
using System.Windows.Forms;
using Mirle.Grid.V2BYMA30;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Mirle.WebAPI.V2BYMA30;


namespace Mirle.DB.Fun
{
    public class clsProc
    {
        private clsCmd_Mst CMD_MST = new clsCmd_Mst();
        private clsTask TaskTable = new clsTask();
        private clsSno SNO = new clsSno();
        private clsLocMst locMst = new clsLocMst();
        private WMS.Proc.clsHost wms;
        private clsL2LCount L2LCount = new clsL2LCount();
        public clsProc(clsDbConfig dbConfig_WMS)
        {
            wms = new WMS.Proc.clsHost(dbConfig_WMS);
        }

        public WMS.Proc.clsHost GetWMS_DBObject()
        {
            return wms;
        }

      
    }
}
