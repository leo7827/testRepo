using System;
using Mirle.Structure;
using Mirle.Def;
using System.Data;
using Mirle.DataBase; 
using Mirle.LiteOn.V2BYMA30;


namespace Mirle.DB.Fun
{
    public class clsTask
    {
        private clsSno SNO = new clsSno();
        private clsCmd_Mst CMD_MST = new clsCmd_Mst();
        private clsTool tool = new clsTool();


        public int FunGetTask_Grid(ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                string strEM = "";
                string strSql = $"select * from TaskInfo";
                //+ $" where CmdSts < '{clsConstValue.CmdSts.strCmd_Finished}' ";
                strSql += " ORDER BY prty, CrtDate, CmdSno";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet != DBResult.Success && iRet != DBResult.NoDataSelect)
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                }

                return iRet;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
        }

        public int CheckHasTaskCmd(string CommandID, ref TaskInfo task, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strSql = "select * from TaskInfo where CmdSno = '" + CommandID + "' ";

                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);

                if (iRet == DBResult.Success)
                {
                    task = tool.GetCommandbyTask(dtTmp);
                }

                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                }
              

                return iRet;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }
        public bool FunUpdateTaskCurrLoc(string sCmdSno, string sCurrLoc, DataBase.DB db)
        {
            try
            {
                string strSql = "update TaskInfo set CurLoc = '" + sCurrLoc + "' , ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where CmdSno = '" + sCmdSno + "'";

                string strEM = "";
                if (db.ExecuteSQL(strSql, ref strEM) == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql);
                    return true;
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql + " => " + strEM);
                    return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }


        }


        public int CheckHasTaskCmd(string CommandID, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strSql = "select * from TaskInfo where CmdSno = '" + CommandID + "' ";

                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet != DBResult.Success && iRet != DBResult.NoDataSelect)
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                }

                return iRet;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public int FunSelectTaskCmd(string TaskNo, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strSql = $"select * from TaskInfo where CmdSno='{TaskNo}'";
                return db.GetDataTable(strSql, ref dtTmp);
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public int FunSelectTaskCmdByCommandID(string sCmdSno, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strSql = $"select * from TaskInfo where CmdSno='{sCmdSno}'";
                return db.GetDataTable(strSql, ref dtTmp);
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }


        public bool FunUpdateTaskPrty( string sDes, string sCmdSno_Ex, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strSql = "select * from TaskInfo where sTo = '" + sDes + "' and CmdSts ='1' and CurLoc in ('LO4-02','LO5-02','LO6-02','LO3-03')";
                string strEM = "";
                int iPrty = 0;
                int iPrty_New = 0;
                string sCmdSno = "";

                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);

                if (iRet == DBResult.Success)
                {
                    for (int i = 0; i < dtTmp.Rows.Count; i++)
                    {
                        //如果只有一筆命令就無需再調整權重
                        if (dtTmp.Rows.Count <= 1 )
                        {
                            return true;
                        }

                        sCmdSno = dtTmp.Rows[i]["CmdSno"].ToString().PadLeft(5,'0');
                        iPrty = Convert.ToInt16(dtTmp.Rows[i]["prty"]);
                        if (iPrty != 1 )
                        {
                            iPrty_New = iPrty - 1;
                            strSql = "update TaskInfo set prty = N'" + iPrty_New + $"'  where CmdSno = '" + sCmdSno + "'  and CmdSno != '"+ sCmdSno_Ex + "'";
                            if (db.ExecuteSQL(strSql, ref strEM) == DBResult.Success)
                            {
                                clsWriLog.Log.FunWriTraceLog_CV(strSql);
                                //return true;
                            }
                            else
                            {
                                clsWriLog.Log.FunWriTraceLog_CV(strSql + " => " + strEM);
                                //return false;
                            }
                        }

                       
                    }

                    return true;
                }

                else
                {
                    return true;
                }


           
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }


        public bool FunUpdateTaskCmdDes(string sCmdSno, string sBuffer, DataBase.DB db)
        {
            try
            {
                string strSql = "update TaskInfo set sTo = '" + sBuffer + "' , ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where CmdSno = '" + sCmdSno + "'";

                string strEM = "";
                if (db.ExecuteSQL(strSql, ref strEM) == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql);
                    return true;
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql + " => " + strEM);
                    return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool UpdateByTaskNo(string sCmdSno, string sSts , string sCurLoc, string sRemark, ref string strEM, DataBase.DB db)
        {
            try
            {
                string sSQL = "update TaskInfo set "; 
                sSQL += " CmdSts = '" + sSts + "', ";

                if (sSts == clsConstValue.CmdSts.strCmd_Running)
                {
                    sSQL += " ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',  ";
                }
                else
                {
                    sSQL += " EndDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',  ";
                }                
             
                sSQL += " CurLoc = '" + sCurLoc + "' , ";              
                sSQL += " Remark = '" + sRemark + "' ";
                sSQL += " where CmdSno = '"+ sCmdSno + "' ";

                if (db.ExecuteSQL(sSQL, ref strEM) == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(sSQL);
                    return true;
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV(sSQL + " => " + strEM);
                    return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunInsertTaskCmd(TaskInfo task, ref string strEM, DataBase.DB db)
        {
            try
            {
                string sSQL = "insert into TaskInfo(CmdSno, CmdSts, prty , sFrom, sTo, CmdMode, CrtDate, ExpDate, EndDate, CurLoc, Remark ,CarrierType) values (";
                sSQL += "'" + task.CmdSno + "', ";
                sSQL += "'" + task.CmdSts + "', ";
                sSQL += "'" + task.Prty + "' , ";
                sSQL += "'" + task.Source + "', ";
                sSQL += "'" + task.Destination + "', ";
                sSQL += "'" + task.CmdMode + "', ";
                sSQL += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',  ";
                sSQL += "'" + task.ExpDate + "', ";
                sSQL += "'" + task.EndDate + "', ";
                sSQL += "'" + task.CurLoc + "' , ";              
                sSQL += "'" + task.Remark + "' , ";
                sSQL += "'" + task.CarrierType + "' )";
                if (db.ExecuteSQL(sSQL, ref strEM) == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(sSQL);
                    return true;
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV(sSQL + " => " + strEM);
                    return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunDelTaskCmd(string CommandID, DataBase.DB db)
        {
            try
            {
                string strEM = "";
                string strSQL = "delete from TaskInfo where CmdSno = '" + CommandID + "' ";
                int Ret = db.ExecuteSQL(strSQL, ref strEM);
                if (Ret == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSQL); return true;
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSQL + " => " + strEM); return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunInsertHisTask(string sCmdSno, DataBase.DB db)
        {
            try
            {
                string SQL = "INSERT INTO HisTaskInfo ";
                SQL += $" SELECT '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', * FROM TaskInfo ";
                SQL += $" WHERE CmdSno='{sCmdSno}'";

                int iRet = db.ExecuteSQL(SQL);
                if (iRet == DBResult.Success)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunDelHisTask(double dblDay, DataBase.DB db)
        {
            try
            {
                string strDelDay = DateTime.Today.Date.AddDays(dblDay * (-1)).ToString("yyyy-MM-dd");
                string strSql = "delete from HisTaskInfo where HisDT <= '" + strDelDay + "' ";

                int iRet = db.ExecuteSQL(strSql);
                if (iRet == DBResult.Success)
                {
                    return true;
                }
                else return false;
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
