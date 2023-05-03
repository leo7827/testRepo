using System;
using Mirle.DataBase;
using System.Linq;
using System.Data;
using Mirle.Def;
using Mirle.Structure;
using Mirle.LiteOn.V2BYMA30; 

namespace Mirle.DB.Proc
{
    public class clsTask
    {
        private Fun.clsTask task = new Fun.clsTask();
        private Fun.clsCmd_Mst cmd_Mst = new Fun.clsCmd_Mst();
        private Fun.clsLocMst locMst = new Fun.clsLocMst();
        private Fun.clsProc proc;
        private Fun.clsSno SNO = new Fun.clsSno();
        private clsDbConfig _config = new clsDbConfig();
        private clsDbConfig _config_Sqlite = new clsDbConfig();
        public clsTask(clsDbConfig config_Sqlite)
        {
            //_config = config;
            _config_Sqlite = config_Sqlite;
            //proc = new Fun.clsProc(config);
        }


        public int FunGetTaskInfo_Grid(ref DataTable dtTmp)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        return task.FunGetTask_Grid(ref dtTmp, db);
                    }
                    else return iRet;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
        }


        public int CheckHasTaskCmd(string CommandID, ref TaskInfo taskinfo)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        return task.CheckHasTaskCmd(CommandID, ref taskinfo, db);
                    }

                    return iRet;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
        }

        public bool FunDeleteTask(string sCmdSno)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        iRet = task.FunSelectTaskCmdByCommandID(sCmdSno, db);
                        if (iRet == DBResult.Success)
                        {
                            return task.FunDelTaskCmd(sCmdSno, db);
                        }
                        else
                        {
                            if (iRet == DBResult.NoDataSelect) return true;
                            else return false;
                        }
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }
        public bool FunDelTaskCmd_Proc(string sCmdSno)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        iRet = task.FunSelectTaskCmdByCommandID(sCmdSno, db);
                        if(iRet == DBResult.Success)
                        {
                            task.FunInsertHisTask(sCmdSno, db);
                            return task.FunDelTaskCmd(sCmdSno, db);
                        }
                        else
                        {
                            if (iRet == DBResult.NoDataSelect) return true;
                            else return false;
                        }
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

   
        public void FunCheckAndDeleteTaskCmd(string sCmdSno, int StockerID)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        if (task.CheckHasTaskCmd(sCmdSno, db) == DBResult.Success)
                        {
                            task.FunInsertHisTask(sCmdSno, db);

                            Location loc = LiteOnLocation.GetLocation_ByStockOutPort(StockerID);
                            if (loc == null)
                            {
                                clsWriLog.Log.FunWriTraceLog_CV($"Error: <CmdSno> {sCmdSno} => 取得目的站Location失敗！");
                                return;
                            }
                            else
                            {
                                if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success) return;

                                if (!cmd_Mst.FunUpdateCurLoc(sCmdSno, loc.DeviceId, loc.LocationId, db))
                                {
                                    db.TransactionCtrl(TransactionTypes.Rollback);
                                    return;
                                }

                                if(!task.FunDelTaskCmd(sCmdSno, db))
                                {
                                    db.TransactionCtrl(TransactionTypes.Rollback);
                                    return;
                                }

                                db.TransactionCtrl(TransactionTypes.Commit);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
            }
        }

        public bool FunDelHisTask(double dblDay)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        return task.FunDelHisTask(dblDay, db);
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

       
        public bool FunUpdateTaskPrty( string sDes ,string sCmdSno)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        return task.FunUpdateTaskPrty( sDes, sCmdSno, db);
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunUpdateTaskCmdDes(string sCmdSno, string sBuffer)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        return task.FunUpdateTaskCmdDes(sCmdSno, sBuffer, db);
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunUpdateTaskCmd(string sCmdSno , string sSts, string sCurLoc, string sRemark, ref string strErrMsg)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        if (string.IsNullOrWhiteSpace(sCmdSno)) return false;

                        return task.UpdateByTaskNo(sCmdSno, sSts, sCurLoc, sRemark, ref strErrMsg, db);
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunInsertTaskCmd(TaskInfo stuTask, ref string strErrMsg)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {                       
                        if (string.IsNullOrWhiteSpace(stuTask.CmdSno)) return false;
                       
                        return task.FunInsertTaskCmd(stuTask, ref strErrMsg, db);
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunUpdateTaskCurrLoc(string sCmdSno, string sCurrLoc)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        return task.FunUpdateTaskCurrLoc(sCmdSno, sCurrLoc, db);
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }
    }
}
