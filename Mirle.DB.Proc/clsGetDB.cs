﻿using Mirle.Def;
using Mirle.DataBase;

namespace Mirle.DB.Proc
{
    public class clsGetDB
    {
        public static Sqlite GetSqliteDB(clsDbConfig _config)
        {
            var db = new Sqlite(_config);
            db.Open();
            return db;
        }


        public static SqlServer GetDB(clsDbConfig _config)
        {
            var db = new SqlServer(_config);
            return db;
        }

        //public static int FunDbOpen(SqlServer db)
        public static int FunDbOpen(Sqlite db)
        {
            string strEM = "";
            return FunDbOpen(db, ref strEM);
        }

        //public static int FunDbOpen(SqlServer db, ref string strEM)
        public static int FunDbOpen(Sqlite db, ref string strEM)
        {
            int iRet = db.Open(ref strEM);
            clsHost.IsConn = db.IsConnected;
            if (iRet != DBResult.Success)
            {
                clsWriLog.Log.FunWriTraceLog_CV($"資料庫開啟失敗！=> {strEM}");
            }

            return iRet;
        }
    }
}
