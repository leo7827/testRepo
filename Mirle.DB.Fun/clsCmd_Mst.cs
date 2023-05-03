using System;
using Mirle.Def;
using System.Data;
using Mirle.LiteOn.V2BYMA30;
using Mirle.Structure;
using Mirle.DataBase;
using Mirle.Def.V2BYMA30;
using System.Collections.Generic;

namespace Mirle.DB.Fun
{
    public class clsCmd_Mst
    {
        private clsTool tool = new clsTool();

        public int FunGetCommandCountByStockOut(int[] NotGoodID, string sStnNo, ref int iCount, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {


                string strSql = $"SELECT COUNT (*) AS ICOUNT FROM CMD_MST where CmdMode = '{clsConstValue.CmdMode.StockOut}' ";
                strSql += $" and StnNo = '{sStnNo}' and CmdSts = '{clsConstValue.CmdSts.strCmd_Running}' ";
                strSql += $" and CurLoc not in ('{LocationDef.LocationIn.A1_04.ToString()}', " +
                    $"'{LocationDef.LocationIn.A1_05.ToString()}', '{LocationDef.LocationIn.A1_10.ToString()}', " +
                    $"'{LocationDef.LocationIn.A1_11.ToString()}', '{LocationDef.LocationIn.A1_16.ToString()}', " +
                    $"'{LocationDef.LocationIn.A1_17.ToString()}', '{LocationDef.LocationIn.A1_22.ToString()}', " +
                    $"'{LocationDef.LocationIn.A1_23.ToString()}') ";
                strSql += " and CurDeviceID not in (";
                for (int i = 0; i < NotGoodID.Length; i++)
                {
                    if (i == 0) strSql += $"'{NotGoodID[i]}'";
                    else strSql += $",'{NotGoodID[i]}'";
                }
                strSql += ")";

                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success) iCount = int.Parse(dtTmp.Rows[0]["ICOUNT"].ToString());
                else clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");

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
                dtTmp.Dispose();
            }
        }


        public int FunGetTicketIdCmD(string StnNo, ref string sticketId, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = $"select * from CMD_MST where StnNo = '{StnNo}'";
                //strSql += $" and CmdMode = '2' ";
                strSql += $" and ticketId not in ('',' ') ";
                //strSql += $" and ticketId <> '{sticketId}' ";
                strSql += $" and CmdSts = '1' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    sticketId = Convert.ToString(dtTmp.Rows[0]["ticketId"]);
                }
                else
                {
                    if (iRet != DBResult.NoDataSelect)
                    {
                        clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    }
                }
                //給訂單號
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
        public int FunGetTicketIdByBatch(string BatchID, string sCmdSno, ref CmdMstInfo batchCmd, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = $"select * from CMD_MST where BatchID = '{BatchID}' and CmdSno <> '{sCmdSno}'";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    batchCmd = tool.GetCommand(dtTmp);
                    return iRet;
                }
                else if (iRet != DBResult.NoDataSelect)
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    iRet = DBResult.Exception;
                }
                else { }
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
        public int FunGetSmallTicketId(string StnNo, string TicketId, ref string SmallTicketId, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = $"select * from CMD_MST where StnNo = '{StnNo}'";
                strSql += $" and ticketId not in ('',' ') ";
                strSql += $" and CmdSts < '7' ";
                strSql += $" order by PRT, CrtDate asc";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    SmallTicketId = Convert.ToString(dtTmp.Rows[0]["ticketId"]);
                }
                else
                {
                    if (iRet != DBResult.NoDataSelect)
                    {
                        clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    }
                    else SmallTicketId = TicketId;
                }
                //給訂單號
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
        public int FunGetSmallTicketId(string StnNo, string TicketId, ref string SmallTicketId, int[] NotGoodID, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = $"select * from CMD_MST where StnNo = '{StnNo}'";
                strSql += $" and ticketId not in ('',' ') ";
                strSql += $" and CmdSts < '7' ";
                strSql += " and EquNO not in (";
                for (int i = 0; i < NotGoodID.Length; i++)
                {
                    if (i == 0) strSql += $"'{NotGoodID[i]}'";
                    else strSql += $",'{NotGoodID[i]}'";
                }
                strSql += ")";
                strSql += $" order by PRT, CrtDate asc";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    SmallTicketId = Convert.ToString(dtTmp.Rows[0]["ticketId"]);
                }
                else
                {
                    if (iRet != DBResult.NoDataSelect)
                    {
                        clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    }
                    else SmallTicketId = TicketId;
                }
                //給訂單號
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

        public int FunGetPortTicketID(string StnNo, ref string sTicketId, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = $"select * from CMD_MST where StnNo = '{StnNo}'";
                strSql += $" and ticketId not in ('',' ') ";
                strSql += $" and ticketId is not null ";
                strSql += " order by CrtDate asc";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    sTicketId = Convert.ToString(dtTmp.Rows[0]["ticketId"]);
                }
                else
                {
                    if (iRet != DBResult.NoDataSelect)
                    {
                        clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    }
                }
                //給訂單號
                return iRet;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
        }
         
        public int CheckHasNeedL2LCmd(string Loc, ref string sCmdSno, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strSql = $"select * from CMD_MST where Loc = '{Loc}' and NeedShelfToShelf = '{clsEnum.NeedL2L.Y.ToString()}' ";
                strSql += $" and CmdSts < '{clsConstValue.CmdSts.strCmd_Finished}' ";

                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if(iRet == DBResult.Success)
                {
                    sCmdSno = Convert.ToString(dtTmp.Rows[0]["CmdSno"]);
                }
                else if(iRet != DBResult.NoDataSelect)
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    iRet = DBResult.Exception;
                }
                else { }

                return iRet;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }

        /// <summary>
        /// 判斷是否可以過帳
        /// </summary>
        public bool CanPost(CmdMstInfo cmd, string sDeviceID, int fork, DataBase.DB db)
        {
            try
            {
                CmdMstInfo cmd_fork = new CmdMstInfo();
                string sRemark = "";
                int iRet = FunGetCommandByFork(int.Parse(sDeviceID), fork, ref cmd_fork, db);
                if (iRet == DBResult.Exception)
                {
                    sRemark = $"Error: 取得Stocker{sDeviceID}的Fork{fork}命令失敗！";
                    if (sRemark != cmd.Remark)
                    {
                        FunUpdateRemark(cmd.CmdSno, sRemark, db);
                    }

                    return false;
                }
                else if (iRet == DBResult.Success)
                {
                    if (cmd_fork.CmdSno != cmd.CmdSno)
                    {
                        sRemark = $"Error: Stocker{sDeviceID}的Fork{fork}已被其他命令佔據 => <任務號> {cmd_fork.CmdSno} <料盒> {cmd_fork.BoxID}";
                        if (sRemark != cmd.Remark)
                        {
                            FunUpdateRemark(cmd.CmdSno, sRemark, db);
                        }

                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public int FunGetFinishCommand(ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                string strSql = $"select * from CMD_MST where CmdSts in ('{clsConstValue.CmdSts.strCmd_Cancel}', '{clsConstValue.CmdSts.strCmd_Finished}')";
                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet != DBResult.Success && iRet != DBResult.NoDataSelect)
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");

                return iRet;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return DBResult.Exception;
            }
        }

        public int FunGetCommandByFork(int iStockerID, int fork, ref CmdMstInfo cmd, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string sCurLoc = fork == 1 ? LocationDef.Location.LeftFork.ToString() : LocationDef.Location.RightFork.ToString();
                string strEM = "";
                string strSql = "select * from CMD_MST where CurDeviceID = '" + iStockerID + "' ";
                strSql += $" and CurLoc = '{sCurLoc}' and CmdSts < '{clsConstValue.CmdSts.strCmd_Finished}' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if(iRet == DBResult.Success) cmd = tool.GetCommand(dtTmp);
                else if(iRet == DBResult.Exception) clsWriLog.Log.FunWriTraceLog_CV(strSql + " => " + strEM);
                else { }

                return iRet;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public bool FunGetCommand(string sCmdSno, ref CmdMstInfo cmd, ref int iRet, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = "select * from CMD_MST where CmdSno = '" + sCmdSno + "' ";
                iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    cmd = tool.GetCommand(dtTmp);
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
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public bool FunGetCommand_ForPickupQuery(string sCmdSno, ref CmdMstInfo cmd, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = "select * from CMD_MST where CmdSno = '" + sCmdSno + "' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    cmd = tool.GetCommand(dtTmp);
                    return true;
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql + " => " + strEM);
                    if(iRet == DBResult.NoDataSelect)
                    {
                        cmd = new CmdMstInfo(); dtTmp = new DataTable();
                        strSql = "select * from CMD_MST_His where CmdSno = '" + sCmdSno + "' ";
                        strSql += " order by HisDT desc";
                        iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                        if (iRet == DBResult.Success)
                        {
                            cmd = tool.GetCommand(dtTmp);
                            return true;
                        }
                        else
                        {
                            clsWriLog.Log.FunWriTraceLog_CV(strSql + " => " + strEM);
                            return false;
                        }
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public bool FunGetCommand_byJobId(string jobId, ref CmdMstInfo cmd, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = "select * from CMD_MST where JobID = '" + jobId + "' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    cmd = tool.GetCommand(dtTmp);
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
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public int FunGetCommand_byBoxID(string sBoxID, ref CmdMstInfo cmd, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = "select * from CMD_MST where BoxId = '" + sBoxID + "' ";
                //strSql += $" and CmdMode = '{clsConstValue.CmdMode.StockOut}' and StnNo <> '' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    cmd = tool.GetCommand(dtTmp);
                }
                else
                {
                    if (iRet != DBResult.NoDataSelect)
                        clsWriLog.Log.FunWriTraceLog_CV(strSql + " => " + strEM);
                }

                return iRet;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public int FunGetCommand_ByBatchID(string sBatchID, ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                string strEM = "";
                string strSql = "select * from CMD_MST where BatchID = '" + sBatchID + "' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    sBatchID = Convert.ToString(dtTmp.Rows[0]["BatchID"]);
                }
                else if (iRet != DBResult.NoDataSelect)
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    iRet = DBResult.Exception;
                }
                else { }

                return iRet;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }
         
        public int FunGetCommandBatches(ref DataTable Batches, DataBase.DB db)
        {
            try
            {
                string strEM = "";
                string strSql = $"SELECT DISTINCT BatchID from CMD_MST" +
                    $" WHERE BatchID not in ('',' ') and BatchID is not null";
                int iRet = db.GetDataTable(strSql, ref Batches, ref strEM);
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

        public int FunGetCommandBatch_byCmdSno(string sCmdSno, ref string sBatchID, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = "select BatchID from CMD_MST where CmdSno = '" + sCmdSno + "' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    sBatchID = Convert.ToString(dtTmp.Rows[0]["BatchID"]);
                }
                else if (iRet != DBResult.NoDataSelect)
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    iRet = DBResult.Exception;
                }
                else { }

                return iRet;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public bool GetLoadPortCmd_Proc(Location loc, ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                if (FunGetCmdMst(loc, ref dtTmp, db)) return true;
                else
                {
                    dtTmp = new DataTable();
                    if (FunGetCmdMst_FromTask(loc, ref dtTmp, db)) return true;
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

        public bool FunGetCmdMst(Location loc, ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                string strEM = "";
                string strSql = "select * from CMD_MST";
                strSql += $" where CurDeviceID = '{loc.DeviceId}' and CurLoc = '{loc.LocationId}' ";
                if (db.GetDataTable(strSql, ref dtTmp, ref strEM) == DBResult.Success) return true;
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
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

        public int FunGetCmdMst_StockIn_L2L(int StockerID, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strSql = "select CmdSno from CMD_MST";
                strSql += $" where CmdSts < '{clsConstValue.CmdSts.strCmd_Finished}' " +
                    $"and (" +
                    $"(CmdMode = '{clsConstValue.CmdMode.StockIn}' and EquNO = '{StockerID}')" +
                    $" or (CmdMode = '{clsConstValue.CmdMode.L2L}' and SUBSTRING(NewLoc,1,2) in ";
                switch(StockerID)
                {
                    case 1:
                        strSql += " ('01','02','03','04'))";
                        break;
                    case 2:
                        strSql += " ('05','06','07','08'))";
                        break;
                    case 3:
                        strSql += " ('09','10','11','12'))";
                        break;
                    default:
                        strSql += " ('13','14'))";
                        break;
                }

                strSql += ") ";

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

        public int FunGetCmdMst_Grid(ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                string strEM = "";
                string strSql = $"select * from CMD_MST" +
                    $" where CmdSts < '{clsConstValue.CmdSts.strCmd_Finished}' ";
                strSql += " ORDER BY PRT, CrtDate, CmdSno";
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

        public bool FunGetCmdMst_FromTask(Location loc, ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                int fork = 0;
                if (loc.LocationId ==  LocationDef.Location.LeftFork.ToString())
                    fork = 1;
                else fork = 2;

                string strEM = "";
                string strSql = "select * from CMD_MST";
                strSql += $" where CmdSno in (select CommandID from Task where DeviceID = '{loc.DeviceId}' " +
                    $"and ForkNo = {fork}) ";
                if (db.GetDataTable(strSql, ref dtTmp, ref strEM) == DBResult.Success) return true;
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
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

        public int FunCheckHasCommand(string sLoc, ref CmdMstInfo cmd, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = $"select * from CMD_MST where Loc = '{sLoc}' or NewLoc = '{sLoc}' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    cmd = tool.GetCommand(dtTmp);
                }
                else
                {
                    if (iRet != DBResult.NoDataSelect)
                    {
                        clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    }
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

        public int FunCheckHasCommand(string sLoc, string sCmdSts, ref DataTable dtTmp, DataBase.DB db)
        {
            try
            {
                string strEM = "";
                string strSql = $"select * from CMD_MST where Loc = '{sLoc}' or NewLoc = '{sLoc}' ";
                strSql += $" and CmdSts = '{sCmdSts}' ";
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

        public int FunCheckHasCommand(int iStockerID, DataBase.DB db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strSql = $"select * from CMD_MST where CurDeviceID = '{iStockerID}' or EquNO = '{iStockerID}' ";
                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Exception)
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");

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

        public bool FunUpdateRemark(string sCmdSno, string sRemark, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set Remark = N'" + sRemark + $"' where CmdSno = '{sCmdSno}'";

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

        public bool FunUpdateStnNo(string sCmdSno, string sStnNo, string sRemark, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set StnNo = '" + sStnNo + $"', Remark = N'{sRemark}' ";
                strSql += ", ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                strSql += $" where CmdSno = '{sCmdSno}' ";

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

        public bool FunUpdatePry(string sBoxID, string Pry, ref string strEM, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set PRT = '" + Pry + "' ";
                strSql += $" where BoxId = '{sBoxID}' ";

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

        public bool FunUpdateCmdSts(string sCmdSno, string sCmdSts, string sRemark, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set Remark = N'" + sRemark + $"', CmdSts = '{sCmdSts}' ";

                if(sCmdSts == clsConstValue.CmdSts.strCmd_Initial)
                {
                    strSql += ", CurLoc = '', CurDeviceID = '' ";
                }

                if (sCmdSts == clsConstValue.CmdSts.strCmd_Cancel || sCmdSts == clsConstValue.CmdSts.strCmd_Finished)
                {
                    strSql += ", EndDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }
                else
                {
                    strSql += ", ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }

                strSql += $" where CmdSno = '{sCmdSno}' ";

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

        public bool FunUpdateCmdSts(string sCmdSno, string sCmdSts, string sStnNo, string sRemark, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set Remark = N'" + sRemark + $"', CmdSts = '{sCmdSts}' ";
                strSql += $", StnNo = '{sStnNo}' ";

                if (sCmdSts == clsConstValue.CmdSts.strCmd_Initial)
                {
                    strSql += ", CurLoc = '', CurDeviceID = '' ";
                }

                if (sCmdSts == clsConstValue.CmdSts.strCmd_Cancel || sCmdSts == clsConstValue.CmdSts.strCmd_Finished)
                {
                    strSql += ", EndDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }
                else
                {
                    strSql += ", ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }

                strSql += $" where CmdSno = '{sCmdSno}' ";

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

        public bool FunUpdateCmdSts(string sCmdSno, string sCmdSts, clsEnum.Cmd_Abnormal abnormal, string sRemark, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set Remark = N'" + sRemark + $"', CmdSts = '{sCmdSts}' ";
                strSql += $", Cmd_Abnormal = '{abnormal.ToString()}' ";

                if (sCmdSts == clsConstValue.CmdSts.strCmd_Cancel || sCmdSts == clsConstValue.CmdSts.strCmd_Finished)
                {
                    strSql += ", EndDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }
                else
                {
                    strSql += ", ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }

                strSql += $" where CmdSno = '{sCmdSno}' ";

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

        public bool FunUpdateNewLocForL2L(string sCmdSno, string sNewLoc, DataBase.DB db)
        {
            try
            {
                string strSql = $"update CMD_MST set NewLoc = '{sNewLoc}' where CmdSno = '{sCmdSno}' ";
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
         
        public bool FunUpdateCurLoc(string sCmdSno, string sCurDeviceID, string sCurLoc, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set CurDeviceID = '" + sCurDeviceID + $"', CurLoc = '{sCurLoc}', " +
                    $"CmdSts = '{clsConstValue.CmdSts.strCmd_Running}' ";

                strSql += ", ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";

                strSql += $" where CmdSno = '{sCmdSno}' ";

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

        public bool FunUpdateCurLocAndCancelBatch(string sCmdSno, string sCurDeviceID, string sCurLoc, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set CurDeviceID = '" + sCurDeviceID + $"', CurLoc = '{sCurLoc}', " +
                    $"CmdSts = '{clsConstValue.CmdSts.strCmd_Running}' " + ", BatchId = '' ";

                strSql += ", ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";

                strSql += $" where CmdSno = '{sCmdSno}' ";

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

        public bool FunUpdateCmd_ForTeachCmd(string sCmdSno, string sLoc, string sNewLoc, DataBase.DB db)
        {
            try
            {
                string strSql = $"update CMD_MST set Loc = '{sLoc}', NewLoc = '{sNewLoc}' ";
                strSql += $", CmdSts = '{clsConstValue.CmdSts.strCmd_Initial}', " +
                    $"CrtDate = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', ExpDate = '', EndDate= '' ";
                strSql += $", CurLoc = '', CurDeviceID = '' where CmdSno = '{sCmdSno}'";
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

        public bool FunUpdateCurLocForS2S(string sCmdSno, string sCurDeviceID, string sCurLoc, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set CurDeviceID = '" + sCurDeviceID + $"', CurLoc = '{sCurLoc}', " +
                    $"CmdSts = '{clsConstValue.CmdSts.strCmd_Finished}' ";

                strSql += ", ExpDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";

                strSql += $" where CmdSno = '{sCmdSno}' ";

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

        public bool FunCancelBatch(string sCmdMode, string BatchID, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set BatchID = '' ";

                strSql += $" where CmdMode = '{sCmdMode}' and BatchID = '{BatchID}' ";

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

        public bool FunCancelBatch(string sCmdMode, string BatchID, string sRemark, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set BatchID = '', Remark = N'" + sRemark + $"' ";

                strSql += $" where CmdMode = '{sCmdMode}' and BatchID = '{BatchID}' ";

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

        public bool FunCancelBatch(string sCmdSno, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set BatchID = '' ";

                strSql += $" where CmdSno = '{sCmdSno}' ";

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

        public bool FunCancelBatch_ByBatchID(string sBatchID, DataBase.DB db)
        {
            try
            {
                string strSql = "update CMD_MST set BatchID = '' ";

                strSql += $" where BatchID = '{sBatchID}' ";

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

        public bool FunUpdateNeedL2L(string sCmdSno, clsEnum.NeedL2L ans, DataBase.DB db)
        {
            try
            {
                string strSql = $"update CMD_MST set NeedShelfToShelf = '{ans.ToString()}' ";

                strSql += $" where CmdSno = '{sCmdSno}' ";

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

        public bool FunUpdateStnNo_ForCycleRun(DataBase.DB db)
        {
            string strEM = "";
            string strSql = $"update CMD_MST set StnNo = '1,2,3' where CmdMode = '{clsConstValue.CmdMode.StockOut}'";
            int iRet = db.ExecuteSQL(strSql, ref strEM);
            if (iRet == DBResult.Success) return true;
            else
            {
                if(iRet != DBResult.NoDataUpdate)
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                }
                return false;
            }
        }

        public bool FunInsCmdMst(CmdMstInfo stuCmdMst, ref string strErrMsg, DataBase.DB db)
        {
            string sSQL = "";
            try
            {
                sSQL = "INSERT INTO CMD_MST (CMDSNO, CmdSts, PRT, Cmd_Abnormal, StnNo, CmdMode, Iotype, Loc, NewLoc,";
                sSQL += "CrtDate, ExpDate, EndDate, UserID, BoxId, EquNO, CurLoc, CurDeviceID, JobID, BatchID, ZoneID, backupPortId, NeedShelfToShelf, ticketId, manualStockIn) values(";
                sSQL += "'" + stuCmdMst.CmdSno + "', ";
                sSQL += "'" + clsConstValue.CmdSts.strCmd_Initial + "', ";
                sSQL += "'" + stuCmdMst.Prt + "', 'NA', ";
                sSQL += "'" + stuCmdMst.StnNo + "', ";
                sSQL += "'" + stuCmdMst.CmdMode + "', ";
                sSQL += "'" + stuCmdMst.IoType + "', ";
                sSQL += "'" + stuCmdMst.Loc + "', ";
                sSQL += "'" + stuCmdMst.NewLoc + "', ";
                sSQL += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '', '', 'WCS', ";
                sSQL += "'" + stuCmdMst.BoxID + "', ";
                sSQL += "'" + stuCmdMst.EquNo + "', ";
                sSQL += $"'{stuCmdMst.CurLoc}', ";
                sSQL += "'" + stuCmdMst.CurDeviceID + "',";
                sSQL += "'" + stuCmdMst.JobID + "',";
                sSQL += "'" + stuCmdMst.BatchID + "',";
                sSQL += "'" + stuCmdMst.ZoneID + "',";
                sSQL += "'" + stuCmdMst.backupPortId + "',";
                sSQL += $"'{stuCmdMst.NeedShelfToShelf}', '{stuCmdMst.ticketId}', '{stuCmdMst.manualStockIn}')";

                if (db.ExecuteSQL(sSQL, ref strErrMsg) == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(sSQL);
                    return true;
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV(sSQL + " => " + strErrMsg);
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

        public bool FunDelCmdMst(string CommandID, DataBase.DB db)
        {
            try
            {
                string strEM = "";
                string strSQL = "delete from CMD_MST where CmdSno = '" + CommandID + "' ";
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

        public bool FunInsertCMD_MST_His(string sCmdSno, DataBase.DB db)
        {
            try
            {
                string SQL = "INSERT INTO CMD_MST_His ";
                SQL += $" SELECT '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', * FROM CMD_MST ";
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

        public bool FunDelCMD_MST_His(double dblDay, DataBase.DB db)
        {
            try
            {
                string strDelDay = DateTime.Today.Date.AddDays(dblDay * (-1)).ToString("yyyy-MM-dd");
                string strSql = "delete from CMD_MST_His where HisDT <= '" + strDelDay + "' ";

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
