using System;
using Mirle.Def;
using Mirle.LiteOn.V2BYMA30;
using System.Windows.Forms;
using Mirle.Structure;
using Mirle.Grid.V2BYMA30;
using Mirle.Structure.Info;
using System.Collections.Generic;
using System.Linq;
using Mirle.Def.V2BYMA30;
using System.Data;
using Mirle.DataBase;
using Mirle.DB.Object.Table;

namespace Mirle.DB.Object
{
    public class clsDB_Proc
    {
        private static Proc.clsHost wcs;
        public static void Initial(clsDbConfig dbConfig)
        {
            //wcs = new Proc.clsHost(dbConfig,  Application.StartupPath + "\\Sqlite\\LCSCODE.DB");
        }
        public static void Initial(string DB_Server_Sqlite, string DB_Name_Sqlite)
        {
            wcs = new Proc.clsHost(DB_Server_Sqlite, DB_Name_Sqlite);
        }

        public static Proc.clsHost GetDB_Object() => wcs;
        public static bool DBConn => Proc.clsHost.IsConn;
        public static bool WMS_DBConn => WMS.Proc.clsHost.IsConn;
     

       
    }
}
