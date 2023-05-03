using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mirle.DB.Fun;
using Mirle.Def;
using Mirle.DataBase; 
using Mirle.LiteOn.V2BYMA30;
using Mirle.Def.V2BYMA30;
using Mirle.Grid.V2BYMA30;
using Mirle.Structure;
using Mirle.Structure.Info;
using System.Linq;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using System.Data;


namespace Mirle.DB.Proc
{
    public class clsProc
    {
        private clsPortDef PortDef = new clsPortDef();
        private Fun.clsCmd_Mst CMD_MST = new Fun.clsCmd_Mst();
        private Fun.clsTask TaskTable = new Fun.clsTask();
        private Fun.clsSno SNO = new Fun.clsSno();
        private Fun.clsLocMst LocMst = new Fun.clsLocMst();
        private Fun.clsProc proc;
        private Fun.clsL2LCount L2LCount = new Fun.clsL2LCount();
        private Fun.clsAlarmData alarmData = new Fun.clsAlarmData();
        private Fun.clsCmd_Mst_His CMD_MST_HIS = new Fun.clsCmd_Mst_His();
        private Fun.clsUnitStsLog unitStsLog = new Fun.clsUnitStsLog();

        public List<Element_Port>[] GetLstPort()
        {
            return PortDef.GetLstPort();
        }

        private clsDbConfig _config = new clsDbConfig();
        private clsDbConfig _config_WMS = new clsDbConfig();
        private clsDbConfig _config_Sqlite = new clsDbConfig();
        private static OEEParamConfig _config_OEEParam = new OEEParamConfig();
        public clsProc(clsDbConfig config_Sqlite)
        {
            //_config = config;
            _config_Sqlite = config_Sqlite;
            proc = new Fun.clsProc(_config_WMS);
        }

        public Fun.clsProc GetFunProcess()
        {
            return proc;
        }

        public static OEEParamConfig Getconfig_OEEParam()
        {
            return _config_OEEParam;
        }

        public bool GetDevicePortProc()
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        if (PortDef.FunDevice(db))
                        {
                            PortDef.FunGetAllPort(db);
                            return true;
                        }
                        else return false;
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
           
         
        public bool FunManualRepeatCmd(string sCmdSno, ref string strEM)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        CmdMstInfo cmd = new CmdMstInfo();
                        if (CMD_MST.FunGetCommand(sCmdSno, ref cmd, ref iRet, db))
                        {
                            int iRet_Task = TaskTable.CheckHasTaskCmd(cmd.CmdSno, db);
                            if (iRet_Task == DBResult.Exception)
                            {
                                strEM = "取得Task命令失敗！";
                                return false;
                            }

                            if (iRet_Task == DBResult.Success) TaskTable.FunInsertHisTask(cmd.CmdSno, db);
                            if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                            {
                                strEM = "Error: Begin失敗！";
                                if (strEM != cmd.Remark)
                                {
                                    CMD_MST.FunUpdateRemark(cmd.CmdSno, strEM, db);
                                }

                                return false;
                            }

                            if (!CMD_MST.FunUpdateCmdSts(cmd.CmdSno, clsConstValue.CmdSts.strCmd_Initial, "WCS手動重新執行命令", db))
                            {
                                db.TransactionCtrl(TransactionTypes.Rollback);
                                return false;
                            }

                            if (iRet_Task == DBResult.Success)
                            {
                                if (!TaskTable.FunDelTaskCmd(cmd.CmdSno, db))
                                {
                                    db.TransactionCtrl(TransactionTypes.Rollback);
                                    return false;
                                }
                            }

                            db.TransactionCtrl(TransactionTypes.Commit);
                            return true;
                        }
                        else
                        {
                            strEM = $"<CmdSno> {sCmdSno} => 取得命令資料失敗！";
                            return false;
                        }
                    }
                    else
                    {
                        strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                strEM = ex.Message;
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunManualRepeatCmd(string sCmdSno, string sStnNo, ref string strEM)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        CmdMstInfo cmd = new CmdMstInfo();
                        if (CMD_MST.FunGetCommand(sCmdSno, ref cmd, ref iRet, db))
                        {
                            int iRet_Task = TaskTable.CheckHasTaskCmd(cmd.CmdSno, db);
                            if (iRet_Task == DBResult.Exception)
                            {
                                strEM = "取得Task命令失敗！";
                                return false;
                            }

                            if (iRet_Task == DBResult.Success) TaskTable.FunInsertHisTask(cmd.CmdSno, db);
                            if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                            {
                                strEM = "Error: Begin失敗！";
                                if (strEM != cmd.Remark)
                                {
                                    CMD_MST.FunUpdateRemark(cmd.CmdSno, strEM, db);
                                }

                                return false;
                            }

                            if (!CMD_MST.FunUpdateCmdSts(cmd.CmdSno, clsConstValue.CmdSts.strCmd_Initial, sStnNo, "WCS手動重新執行命令", db))
                            {
                                db.TransactionCtrl(TransactionTypes.Rollback);
                                return false;
                            }

                            if (iRet_Task == DBResult.Success)
                            {
                                if (!TaskTable.FunDelTaskCmd(cmd.CmdSno, db))
                                {
                                    db.TransactionCtrl(TransactionTypes.Rollback);
                                    return false;
                                }
                            }

                            db.TransactionCtrl(TransactionTypes.Commit);
                            return true;
                        }
                        else
                        {
                            strEM = $"<CmdSno> {sCmdSno} => 取得命令資料失敗！";
                            return false;
                        }
                    }
                    else
                    {
                        strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                strEM = ex.Message;
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunManualCommandComplete(string sCmdSno, ref string strEM)
        {
            try
            {
                using (var db = clsGetDB.GetSqliteDB(_config_Sqlite))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        CmdMstInfo cmd = new CmdMstInfo();
                        if (CMD_MST.FunGetCommand(sCmdSno, ref cmd, ref iRet, db))
                        {
                            int iRet_Task = TaskTable.CheckHasTaskCmd(cmd.CmdSno, db);
                            if (iRet_Task == DBResult.Exception)
                            {
                                strEM = "取得Task命令失敗！";
                                return false;
                            }

                            //int iRet_Teach = LocMst.GetTeachLoc_byBoxID(cmd.BoxID, db);
                            //if (iRet_Teach == DBResult.Exception)
                            //{
                            //    strEM = "取得校正儲位資料失敗！";
                            //    return false;
                            //}

                            if (iRet_Task == DBResult.Success) TaskTable.FunInsertHisTask(cmd.CmdSno, db);
                            if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                            {
                                strEM = "Error: Begin失敗！";
                                if (strEM != cmd.Remark)
                                {
                                    CMD_MST.FunUpdateRemark(cmd.CmdSno, strEM, db);
                                }

                                return false;
                            }

                            if (!CMD_MST.FunUpdateCmdSts(cmd.CmdSno, clsConstValue.CmdSts.strCmd_Finished, "WCS命令手動完成", db))
                            {
                                db.TransactionCtrl(TransactionTypes.Rollback);
                                return false;
                            }

                            if (iRet_Task == DBResult.Success)
                            {
                                if (!TaskTable.FunDelTaskCmd(cmd.CmdSno, db))
                                {
                                    db.TransactionCtrl(TransactionTypes.Rollback);
                                    return false;
                                }
                            }

                            //if (iRet_Teach == DBResult.Success)
                            //{
                                //if (!LocMst.FunClearTeachLoc_byBoxID(cmd.BoxID, db))
                                //{
                                //    db.TransactionCtrl(TransactionTypes.Rollback);
                                //    return false;
                                //}
                            //}

                            if (cmd.CmdMode == clsConstValue.CmdMode.StockIn)
                            {
                                //PutAwayCompleteInfo info = new PutAwayCompleteInfo
                                //{
                                //    carrierId = cmd.BoxID,
                                //    isComplete = clsEnum.WmsApi.IsComplete.Y.ToString(),
                                //    jobId = cmd.JobID,
                                //    shelfId = cmd.Loc
                                //};

                                //if (!clsWmsApi.GetApiProcess().GetPutAwayComplete().FunReport(info))
                                //{
                                //    db.TransactionCtrl(TransactionTypes.Rollback);
                                //    return false;
                                //}
                            }
                            else if (cmd.CmdMode == clsConstValue.CmdMode.L2L)
                            {
                                //ShelfCompleteInfo info = new ShelfCompleteInfo
                                //{
                                //    carrierId = cmd.BoxID,
                                //    jobId = cmd.JobID,
                                //    shelfId = cmd.NewLoc
                                //};

                                //if (!clsWmsApi.GetApiProcess().GetShelfComplete().FunReport(info))
                                //{
                                //    db.TransactionCtrl(TransactionTypes.Rollback);
                                //    return false;
                                //}
                            }
                            else
                            {
                                //string sStnNo = "";
                                //if (string.IsNullOrWhiteSpace(cmd.StnNo)) sStnNo = ConveyorDef.A1_41.StnNo;
                                //else sStnNo = cmd.StnNo;

                                //RetrieveCompleteInfo info = new RetrieveCompleteInfo
                                //{
                                //    carrierId = cmd.BoxID,
                                //    isComplete = clsEnum.WmsApi.IsComplete.Y.ToString(),
                                //    jobId = cmd.JobID,
                                //    portId = sStnNo
                                //};

                                //if (!clsWmsApi.GetApiProcess().GetRetrieveComplete().FunReport(info))
                                //{
                                //    db.TransactionCtrl(TransactionTypes.Rollback);
                                //    return false;
                                //}
                            }

                            db.TransactionCtrl(TransactionTypes.Commit);
                            return true;
                        }
                        else
                        {
                            strEM = $"<CmdSno> {sCmdSno} => 取得命令資料失敗！";
                            return false;
                        }
                    }
                    else
                    {
                        strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                strEM = ex.Message;
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

     
    }
}
