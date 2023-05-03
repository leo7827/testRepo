using Mirle.DataBase;
using Mirle.LCS.Enums;
using Mirle.LCS.Models;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.Repositories;
using Mirle.Stocker.TaskControl.TraceLog;
using Mirle.Structure.Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Mirle.R46YP320.STK.TaskService
{
    public class RepositoriesService : RepositoriesModule
    {
        protected readonly TaskInfo _TaskInfo;
        private string _stockerId;

        public RepositoriesService(TaskInfo taskInfo, LoggerService loggerService) : base(taskInfo, loggerService)
        {
            _TaskInfo = taskInfo;
            _stockerId = _TaskInfo.Config.STKCConfig.StockerID;
        }

        #region For Inital
        private static DB GetDB(LCSINI config)
        {
            DB db = new DB(config.Database.DBMS);
            db.DBServer = config.Database.DbServer;
            db.FODBServer = config.Database.FODbServer;
            db.DBPort = config.Database.DBPort;
            db.DBName = config.Database.DataSource;
            db.DBPassword = config.Database.DbPswd;
            db.DBUser = config.Database.DbUser;
            return db;
        }
        private DB GetDB()
        {
            DB db = new DB(_TaskInfo.Config.Database.DBMS);
            db.DBServer = _TaskInfo.Config.Database.DbServer;
            db.FODBServer = _TaskInfo.Config.Database.FODbServer;
            db.DBPort = _TaskInfo.Config.Database.DBPort;
            db.DBName = _TaskInfo.Config.Database.DataSource;
            db.DBPassword = _TaskInfo.Config.Database.DbPswd;
            db.DBUser = _TaskInfo.Config.Database.DbUser;
            db.Open();
            return db;
        }

        public DB GetSQLite()
        {
            DB db = new DB(DB.DataBaseType.SQLite);
            db.DBName = Application.StartupPath + @"\LCSCODE.DB";

            return db;
        }

        public IEnumerable<Mirle.Structure.Info.CraneInfo> GetCurrentCraneStates()
        {
            List<Mirle.Structure.Info.CraneInfo> cranes = new List<Mirle.Structure.Info.CraneInfo>();
            var craneCount = _TaskInfo.Config.STKCConfig.ControlMode == (int)LCSEnums.ControlMode.Single ? 1 : 2;
            for (int i = 1; i <= craneCount; i++)
            {
                Mirle.Structure.Info.CraneInfo crane = new Mirle.Structure.Info.CraneInfo();
                crane.StockerCraneID = _TaskInfo.GetCraneInfo(i).CraneID;
                crane.StockerCraneNo = i;
                crane.CraneTransferState = getCraneTransferState(i);
                cranes.Add(crane);
            }
            return cranes;
        }

        private VIDEnums.CraneTransferState getCraneTransferState(int CraneNo)
        {
            var craneStatus = getCraneStatus();
            var craneTransferState = VIDEnums.CraneTransferState.CraneInService;
            if (craneStatus == "0")
            {
                craneTransferState = VIDEnums.CraneTransferState.CraneInService;
            }
            else if (craneStatus == "1" && CraneNo == 2)
            {
                craneTransferState = VIDEnums.CraneTransferState.CraneOutOfService;
            }
            else if (craneStatus == "1" && CraneNo == 1)
            {
                craneTransferState = VIDEnums.CraneTransferState.CraneInService;
            }
            else if (craneStatus == "2" && CraneNo == 2)
            {
                craneTransferState = VIDEnums.CraneTransferState.CraneInService;
            }
            else if (craneStatus == "2" && CraneNo == 1)
            {
                craneTransferState = VIDEnums.CraneTransferState.CraneOutOfService;
            }
            return craneTransferState;
        }

        private string getCraneStatus()
        {
            string strEM = string.Empty;
            string strValue = "0";
            DataTable table = new DataTable();
            string strSQL = "SELECT VALUE FROM PARAMETERS WHERE TYPE='STOCKERSTATUS' or TYPE='StockerStatus'";
            using (DB _SQLite = GetSQLite())
            {
                if (_SQLite.GetDataTable(strSQL, ref table, ref strEM) == ErrorCode.Success)
                {
                    DataRow[] tableRows = table.Select();
                    strValue = tableRows[0]["Value"].ToString();
                }
            }
            return strValue;
        }

        public string GetLimitBay(bool isOnlyCrane1Mode, bool isOnlyCrane2Mode)
        {
            if (isOnlyCrane1Mode)
            {
                string strEM = string.Empty;
                string strValue = "0";
                DataTable table = new DataTable();
                string strSQL = $"SELECT HandOffStart_C1 FROM CRANEFUN WHERE STOCKERID='{_stockerId}'";
                using (DB _SQLite = GetSQLite())
                {
                    if (_SQLite.GetDataTable(strSQL, ref table, ref strEM) == ErrorCode.Success)
                    {
                        DataRow[] tableRows = table.Select();
                        //strValue = tableRows[0]["HandOffStart_C1"].ToString();
                        strValue = (Convert.ToInt32(tableRows[0]["HandOffStart_C1"]) - 1).ToString();
                    }
                }
                return strValue;
            }
            else if (isOnlyCrane2Mode)
            {
                string strEM = string.Empty;
                string strValue = "0";
                DataTable table = new DataTable();
                string strSQL = $"SELECT HandOffStart_C2 FROM CRANEFUN WHERE STOCKERID='{_stockerId}'";
                using (DB _SQLite = GetSQLite())
                {
                    if (_SQLite.GetDataTable(strSQL, ref table, ref strEM) == ErrorCode.Success)
                    {
                        DataRow[] tableRows = table.Select();
                        //strValue = tableRows[0]["HandOffStart_C2"].ToString();
                        strValue = (Convert.ToInt32(tableRows[0]["HandOffStart_C2"]) + 1).ToString();
                    }
                }
                return strValue;
            }
            return "";
        }

        private string funGetCraneSpeedType()
        {
            string strSQL = string.Empty;
            string strValue = string.Empty;
            string strEM = string.Empty;
            DataTable table = new DataTable();

            try
            {
                strSQL = "SELECT Value FROM Parameters WHERE Type='CraneSpeedType'";
                using (DB _SQLite = GetSQLite())
                {
                    if (_SQLite.GetDataTable(strSQL, ref table, ref strEM) == ErrorCode.Success)
                    {
                        DataRow[] tableRows = table.Select();
                        strValue = tableRows[0]["Value"].ToString();
                    }
                }
                return strValue;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return "0";
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public static IEnumerable<ShelfDef> GetCraneInfo(LCSINI config, LoggerService loggerService)
        {
            DataTable table = new DataTable();
            try
            {
                using (DB _db = GetDB(config))
                {
                    var stockerId = config.SystemConfig.StockerID;
                    string SQL = "SELECT * FROM SHELFDEF";
                    SQL += $" WHERE STOCKERID='{stockerId}'";
                    SQL += $" AND SHELFTYPE='{(int)ShelfType.Crane}'";
                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        List<ShelfDef> lst = new List<ShelfDef>();
                        foreach (DataRow row in table.Rows)
                        {
                            ShelfDef shelf = new ShelfDef();
                            shelf.ShelfID = Convert.ToString(row["SHELFID"]);
                            shelf.Stage = Convert.ToInt32(row["STAGE"]);
                            shelf.Bank_X = Convert.ToString(row["BANK_X"]);
                            shelf.Bay_Y = Convert.ToString(row["BAY_Y"]);
                            shelf.Level_Z = Convert.ToString(row["LEVEL_Z"]);
                            shelf.LocateCraneNo = Convert.ToInt32(row["LOCATECRANENO"]);
                            shelf.ShelfType = Convert.ToInt32(row["SHELFTYPE"]);
                            shelf.ShelfState = Convert.ToChar(row["SHELFSTATE"]);
                            shelf.ZoneID = Convert.ToString(row["ZONEID"]);
                            shelf.Enable = Convert.ToString(row["ENABLE"]) == "Y";
                            shelf.EmptyBlockFlag = Convert.ToString(row["EMPTYBLOCKFLAG"]) == "Y";
                            shelf.HoldState = Convert.ToInt32(row["HOLDSTATE"]);
                            shelf.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                            shelf.ChargeLoc = Convert.ToString(row["CHARGELOC"]) == "Y";
                            shelf.SelectPriority = Convert.ToInt32(row["SELECTPRIORITY"]);
                            lst.Add(shelf);
                        }
                        return lst;
                    }
                    else
                        return new List<ShelfDef>();
                }
            }
            catch (Exception ex)
            {
                loggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<ShelfDef>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<ShelfDef> GetCraneInfo(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM SHELFDEF";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND SHELFTYPE='{(int)ShelfType.Crane}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<ShelfDef> lst = new List<ShelfDef>();
                    foreach (DataRow row in table.Rows)
                    {
                        ShelfDef shelf = new ShelfDef();
                        shelf.ShelfID = Convert.ToString(row["SHELFID"]);
                        shelf.Stage = Convert.ToInt32(row["STAGE"]);
                        shelf.Bank_X = Convert.ToString(row["BANK_X"]);
                        shelf.Bay_Y = Convert.ToString(row["BAY_Y"]);
                        shelf.Level_Z = Convert.ToString(row["LEVEL_Z"]);
                        shelf.LocateCraneNo = Convert.ToInt32(row["LOCATECRANENO"]);
                        shelf.ShelfType = Convert.ToInt32(row["SHELFTYPE"]);
                        shelf.ShelfState = Convert.ToChar(row["SHELFSTATE"]);
                        shelf.ZoneID = Convert.ToString(row["ZONEID"]);
                        shelf.Enable = Convert.ToString(row["ENABLE"]) == "Y";
                        shelf.EmptyBlockFlag = Convert.ToString(row["EMPTYBLOCKFLAG"]) == "Y";
                        shelf.HoldState = Convert.ToInt32(row["HOLDSTATE"]);
                        shelf.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                        shelf.ChargeLoc = Convert.ToString(row["CHARGELOC"]) == "Y";
                        shelf.SelectPriority = Convert.ToInt32(row["SELECTPRIORITY"]);
                        lst.Add(shelf);
                    }
                    return lst;
                }
                else
                    return new List<ShelfDef>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<ShelfDef>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public static IEnumerable<PortDef> GetPortDef(LCSINI config, LoggerService loggerService)
        {
            DataTable table = new DataTable();
            try
            {
                using (DB _db = GetDB(config))
                {
                    var stockerId = config.SystemConfig.StockerID;
                    string SQL = "SELECT PORTDEF.*, SHELFDEF.ENABLE, SHELFDEF.LOCATECRANENO";
                    SQL += " FROM PORTDEF, SHELFDEF";
                    SQL += " WHERE PORTDEF.STOCKERID=SHELFDEF.STOCKERID";
                    SQL += $" AND PORTDEF.STOCKERID='{ stockerId}'";
                    SQL += " AND PORTDEF.SHELFID=SHELFDEF.SHELFID";
                    SQL += " AND SHELFDEF.STAGE=1";
                    SQL += " ORDER BY PORTDEF.PLCPORTID";
                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        List<PortDef> lst = new List<PortDef>();
                        foreach (DataRow row in table.Rows)
                        {
                            PortDef portDef = new PortDef();
                            portDef.PLCPortID = Convert.ToInt32(row["PLCPORTID"]);
                            portDef.HostEQPortID = Convert.ToString(row["HOSTEQPORTID"]);
                            portDef.ShelfID = Convert.ToString(row["SHELFID"]);
                            portDef.PortType = Convert.ToInt32(row["PORTTYPE"]);
                            portDef.PortLocationType = Convert.ToInt32(row["PORTLOCATIONTYPE"]);
                            portDef.PortTypeIndex = Convert.ToInt32(row["PORTTYPEINDEX"]);
                            portDef.SourceWeighted = Convert.ToInt32(row["SOURCEWEIGHTED"]);
                            portDef.DestWeighted = Convert.ToInt32(row["DESTWEIGHTED"]);
                            portDef.TimeOutForAutoUD = Convert.ToInt32(row["TIMEOUTFORAUTOUD"]);
                            portDef.TimeOutForAutoInZone = Convert.ToString(row["TIMEOUTFORAUTOINZONE"]);
                            portDef.AlternateToZone = Convert.ToString(row["ALTERNATETOZONE"]);
                            portDef.Stage = Convert.ToInt32(row["STAGE"]);
                            portDef.Vehicles = Convert.ToInt32(row["VEHICLES"]);
                            portDef.IgnoreModeChange = Convert.ToString(row["IGNOREMODECHANGE"]) == "Y";
                            portDef.ReportMCSFlag = Convert.ToString(row["REPORTMCSFLAG"]) == "Y";
                            portDef.ReportStage = Convert.ToInt32(row["REPORTSTAGE"]);
                            portDef.NetHStnNo = Convert.ToInt32(row["NETHSTNNO"]);
                            portDef.AreaSensorStnNo = Convert.ToInt32(row["AREASENSORSTNNO"]);
                            portDef.PortEnable = Convert.ToString(row["ENABLE"]) == "Y";
                            lst.Add(portDef);
                        }
                        return lst;
                    }
                    else
                        return new List<PortDef>();
                }
            }
            catch (Exception ex)
            {
                loggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<PortDef>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        #endregion For Inital    

        #region Transfer
        public bool ExistsIntoShareArea(int mainCrane, IEnumerable<CommandTrace> commandTrace, int shareAreaStartBay, int shareAreaEndBay)
        {
            try
            {
                foreach (CommandTrace trace in commandTrace)
                {
                    int iCrane = trace.ExecCrane;
                    if (iCrane != mainCrane)
                        continue;
                    if (string.IsNullOrWhiteSpace(trace.ExecTaskNo) || trace.ExecTaskState > (int)TaskState.Transferring)
                        continue;

                    if (trace.ExecDestBay >= shareAreaStartBay && trace.ExecDestBay <= shareAreaEndBay)
                        return true;
                }
                return false;
            }
            catch
            { return false; }
        }
        public IEnumerable<CommandTrace> GetOptimalTaskByNearOrder(int mainCrane, int currentBay, IEnumerable<CommandTrace> commandTrace, int handoffStart, int handoffEnd)
        {
            try
            {
                List<CommandTrace> newCommandTrace = new List<CommandTrace>();
                foreach (CommandTrace trace in commandTrace)
                {
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState != (int)TaskState.UpdateOK)
                        continue;

                    if (string.IsNullOrWhiteSpace(trace.ExecTaskNo))
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState > (int)TaskState.Transferring)
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }

                    trace.Distance = Math.Abs(mainCrane == 1 ? handoffStart - trace.NextDestBay : handoffEnd - trace.NextDestBay);
                    newCommandTrace.Add(trace);
                }
                return newCommandTrace.OrderBy(x => x.Distance).ToList();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return commandTrace;
            }
        }
        public IEnumerable<CommandTrace> GetOptimalTaskByOutShareArea(int mainCrane, IEnumerable<CommandTrace> commandTrace, int shareAreaStartBay, int shareAreaEndBay)
        {
            try
            {
                List<CommandTrace> newCommandTrace = new List<CommandTrace>();
                foreach (CommandTrace trace in commandTrace)
                {
                    int iCrane = trace.NextCrane;
                    if (iCrane != mainCrane)
                        continue;

                    if (!string.IsNullOrWhiteSpace(trace.NextDest) && trace.NextDestBay > 0)
                    {
                        if (trace.NextSourceBay >= shareAreaStartBay && trace.NextSourceBay <= shareAreaEndBay)
                        {
                            //if (trace.NextDestBay < shareAreaStartBay || trace.NextDestBay > shareAreaEndBay)
                            //{
                            trace.Distance = Math.Abs(mainCrane == 1 ? shareAreaStartBay - trace.NextDestBay : shareAreaEndBay - trace.NextDestBay);
                            newCommandTrace.Add(trace);
                            //}
                        }
                        //ShareArea 外的 命令 Priority大於 900的 優先處理 
                        else if (trace.MainPriority > 900)
                        {
                            newCommandTrace.Add(trace);
                        }

                        //else
                        //{
                        //    if ((trace.NextDestBay < shareAreaStartBay || trace.NextDestBay > shareAreaEndBay))
                        //    {
                        //        trace.Distance = Math.Abs(mainCrane == 1 ? shareAreaStartBay - trace.NextSourceBay : shareAreaEndBay - trace.NextSourceBay);
                        //        newCommandTrace.Add(trace);
                        //    }
                        //}
                    }
                }
                return newCommandTrace.OrderByDescending(x => x.MainPriority).OrderBy(x => x.Distance).ToList();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return commandTrace;
            }
        }
        public IEnumerable<CommandTrace> GetOptimalTaskByOutShareArea2(int mainCrane, IEnumerable<CommandTrace> commandTrace, int shareAreaStartBay, int shareAreaEndBay)
        {
            try
            {
                List<CommandTrace> newCommandTrace = new List<CommandTrace>();
                foreach (CommandTrace trace in commandTrace)
                {
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState != (int)TaskState.UpdateOK)
                        continue;

                    if (string.IsNullOrWhiteSpace(trace.ExecTaskNo))
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState > (int)TaskState.Transferring)
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }
                    if ((trace.NextDestBay < shareAreaStartBay || trace.NextDestBay > shareAreaEndBay) && (trace.NextSourceBay < shareAreaStartBay || trace.NextSourceBay > shareAreaEndBay))
                    {
                        trace.Distance = Math.Abs(mainCrane == 1 ? shareAreaStartBay - trace.NextDestBay : shareAreaEndBay - trace.NextDestBay);
                        newCommandTrace.Add(trace);
                    }
                }
                return newCommandTrace.Where(row => row.NextCrane == mainCrane).OrderBy(x => x.Distance).ToList();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return commandTrace;
            }
        }
        public IEnumerable<CommandTrace> GetOptimalTaskByIntoShareArea(int mainCrane, IEnumerable<CommandTrace> commandTrace, int shareAreaStartBay, int shareAreaEndBay)
        {
            try
            {
                List<CommandTrace> newCommandTrace = new List<CommandTrace>();
                foreach (CommandTrace trace in commandTrace)
                {
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState != (int)TaskState.UpdateOK)
                        continue;

                    if (string.IsNullOrWhiteSpace(trace.ExecTaskNo))
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState > (int)TaskState.Transferring)
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }
                    if ((trace.NextDestBay > shareAreaStartBay && trace.NextDestBay < shareAreaEndBay) || (trace.NextSourceBay > shareAreaStartBay && trace.NextSourceBay < shareAreaEndBay) || (trace.NextDestBay == 0 && string.IsNullOrWhiteSpace(trace.NextDest)))
                    {
                        trace.Distance = Math.Abs(mainCrane == 1 ? shareAreaStartBay - trace.NextDestBay : shareAreaEndBay - trace.NextDestBay);
                        newCommandTrace.Add(trace);
                    }
                }
                return newCommandTrace.Where(row => row.NextCrane == mainCrane).OrderBy(x => x.Distance).ToList();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return commandTrace;
            }
        }
        public IEnumerable<CommandTrace> GetOptimalTaskByReverseOrder(int mainCrane, int currentBay, IEnumerable<CommandTrace> commandTrace, int handoffStart, int handoffEnd)
        {
            try
            {
                List<CommandTrace> newCommandTrace = new List<CommandTrace>();
                foreach (CommandTrace trace in commandTrace)
                {
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState != (int)TaskState.UpdateOK)
                        continue;

                    if (string.IsNullOrWhiteSpace(trace.ExecTaskNo))
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState > (int)TaskState.Transferring)
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }

                    if ((mainCrane == 1 && currentBay < handoffStart) || (mainCrane == 2 && currentBay > handoffEnd))
                    {
                        trace.Distance = Math.Abs((mainCrane == 1 ? currentBay - trace.NextDestBay : currentBay - trace.NextDestBay));
                        newCommandTrace.Add(trace);
                    }
                    else
                    {
                        trace.Distance = 100;
                        newCommandTrace.Add(trace);
                    }
                }
                return newCommandTrace.OrderBy(x => x.Distance).ToList();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return commandTrace;
            }
        }
        public IEnumerable<CommandTrace> GetOptimalTaskByPriority(int mainCrane, int currentBay, IEnumerable<CommandTrace> commandTrace)
        {
            try
            {
                List<CommandTrace> newCommandTrace = new List<CommandTrace>();
                foreach (CommandTrace trace in commandTrace)
                {
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState != (int)TaskState.UpdateOK)
                        continue;

                    if (string.IsNullOrWhiteSpace(trace.ExecTaskNo))
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }
                    if (!string.IsNullOrWhiteSpace(trace.ExecTaskNo) && trace.ExecTaskState > (int)TaskState.Transferring)
                    {
                        if (trace.NextCrane != mainCrane)
                            continue;
                    }
                    trace.Distance = Math.Abs(currentBay - trace.NextSourceBay);
                    newCommandTrace.Add(trace);
                }
                return newCommandTrace.OrderByDescending(x => x.MainPriority).ThenBy(x => x.Distance).ToList();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return commandTrace;
            }
        }

        public bool ExistsCommandOnShareArea(int mainCrane, IEnumerable<CommandTrace> commandTrace, int shareAreaStartBay, int shareAreaEndBay)
        {
            try
            {
                foreach (CommandTrace trace in commandTrace)
                {
                    int iCrane = trace.NextCrane;
                    if (iCrane != mainCrane)
                        continue;

                    if (!string.IsNullOrWhiteSpace(trace.NextDest) && trace.NextDestBay > 0)
                    {
                        if (trace.NextSourceBay >= shareAreaStartBay && trace.NextSourceBay <= shareAreaEndBay)
                        {
                            //if (trace.NextDestBay < shareAreaStartBay || trace.NextDestBay > shareAreaEndBay)
                            return true;
                        }
                        //else
                        //{
                        //    if ((trace.NextDestBay < shareAreaStartBay || trace.NextDestBay > shareAreaEndBay))
                        //        return true;
                        //}
                    }
                }
                return false;
            }
            catch
            { return false; }
        }

        public IEnumerable<CommandTrace> GetCommandTrace(DB _db)
        {
            DataTable table = new DataTable();
            List<CommandTrace> commandTraces = new List<CommandTrace>();
            try
            {
                string strSQL = "SELECT TRANSFERCMD.CRANENO, ISNULL(TRANSFERCMD.FORKNUMBER, 0) FORKNUMBER, TRANSFERCMD.COMMANDID, TRANSFERCMD.CSTID, TRANSFERCMD.TRANSFERMODE, TRANSFERCMD.QUEUEDT, TRANSFERCMD.PRIORITY,";
                strSQL += " TRANSFERCMD.SOURCE SOURCE, TRANSFERCMD.SOURCEBAY, TRANSFERCMD.DESTINATION DESTINATION, TRANSFERCMD.DESTINATIONBAY, TRANSFERCMD.USERID,";
                strSQL += " TRANSFERCMD.TRAVELAXISSPEED, TRANSFERCMD.LIFTERAXISSPEED, TRANSFERCMD.ROTATEAXISSPEED, TRANSFERCMD.FORKAXISSPEED, TRANSFERCMD.BCRREADFLAG, ISNULL(TRANSFERCMD.BATCHID, '') BATCHID,";
                strSQL += " ISNULL(TASK.CRANENO, 0) TCRANENO, ISNULL(TASK.TASKNO, '') TTASKNO, ISNULL(TASK.TRANSFERMODE, 0) TTRANSFERMODE, TASK.TASKSTATE TTASKSTATE,";
                strSQL += " ISNULL(TASK.SOURCE, '') TSOURCE, ISNULL(TASK.SOURCEBAY, 0) TSOURCEBAY, ISNULL(TASK.DESTINATION, '') TDESTINATION, ISNULL(TASK.DESTINATIONBAY, 0) TDESTINATIONBAY,";
                strSQL += " TASK.CSTONDT, TASK.CSTTAKEOFFDT, TASK.FINISHLOCATION TFINISHLOC, ISNULL(TASK.FORKNUMBER, 0) TFORKNUMBER";
                strSQL += " FROM TRANSFERCMD LEFT JOIN TASK ON TRANSFERCMD.STOCKERID=TASK.STOCKERID AND TRANSFERCMD.COMMANDID=TASK.COMMANDID";
                strSQL += $" WHERE TRANSFERCMD.STOCKERID='{_stockerId}'";
                strSQL += $" AND TRANSFERCMD.TRANSFERSTATE IN ('{(int)TransferState.Queue}', '{(int)TransferState.Complete}')";
                strSQL += "	AND TRANSFERCMD.ABORTFLAG<>'Y' AND TRANSFERCMD.CANCELFLAG<>'Y'";
                strSQL += $" AND (TASK.TASKSTATE='{(int)TaskState.UpdateOK}' OR TASK.TASKSTATE IS NULL)";
                strSQL += " ORDER BY TRANSFERCMD.PRIORITY, TRANSFERCMD.QUEUEDT";

                string recCommandID = string.Empty;
                if (_db.GetDataTable(strSQL, ref table) == ErrorCode.Success)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        CommandTrace commandTrace = new CommandTrace();
                        if (!string.IsNullOrWhiteSpace(recCommandID) && recCommandID == commandTrace.CommandID)
                            continue;

                        commandTrace.UserID = Convert.ToString(row["USERID"]);
                        commandTrace.CommandID = Convert.ToString(row["COMMANDID"]);
                        commandTrace.CarrierID = Convert.ToString(row["CSTID"]);
                        commandTrace.BatchID = Convert.ToString(row["BATCHID"]);
                        commandTrace.MainCrane = Convert.ToInt32(row["CRANENO"]);
                        commandTrace.MainFork = Convert.ToInt32(row["FORKNUMBER"]);
                        commandTrace.MainTransferMode = Convert.ToInt32(row["TRANSFERMODE"]);
                        commandTrace.MainSource = Convert.ToString(row["SOURCE"]);
                        commandTrace.MainSourceBay = Convert.ToInt32(row["SOURCEBAY"]);
                        commandTrace.MainDest = Convert.ToString(row["DESTINATION"]);
                        commandTrace.MainDestBay = Convert.ToInt32(row["DESTINATIONBAY"]);
                        commandTrace.MainPriority = Convert.ToInt32(row["PRIORITY"]);
                        commandTrace.BCRReadFlag = Convert.ToString(row["BCRReadFlag"]) == "Y";
                        commandTrace.MainCraneSpeed.TravelaxisSpeed = Convert.ToInt32(row["TRAVELAXISSPEED"]);
                        commandTrace.MainCraneSpeed.LifteraxisSpeed = Convert.ToInt32(row["LIFTERAXISSPEED"]);
                        commandTrace.MainCraneSpeed.RotateaxisSpeed = Convert.ToInt32(row["ROTATEAXISSPEED"]);
                        commandTrace.MainCraneSpeed.ForkaxisSpeed = Convert.ToInt32(row["FORKAXISSPEED"]);

                        commandTrace.ExecTaskNo = Convert.ToString(row["TTASKNO"]);
                        if (string.IsNullOrWhiteSpace(commandTrace.ExecTaskNo) || (commandTrace.ExecTaskState == (int)TaskState.UpdateOK && string.IsNullOrWhiteSpace(commandTrace.ExecFinishLoc)))
                        {
                            #region 預計先產生的Task
                            if (commandTrace.MainTransferMode == (int)TransferMode.MOVE)
                            {
                                #region MOVE

                                #region Check
                                if (!GetShelfInfoByShelfID(_db, commandTrace.MainDest, out VShelfInfo dest))
                                {
                                    if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.MainDest), out dest))
                                    {
                                        //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                        continue;
                                    }
                                }
                                #endregion Check

                                commandTrace.NextTransferMode = (int)TransferMode.MOVE;
                                commandTrace.NextTransferModeType = ((int)TransferModeType.Move).ToString();
                                commandTrace.NextCrane = dest.LocateCraneNo == 3 ? commandTrace.MainCrane > 0 && commandTrace.MainCrane != 3 ? commandTrace.MainCrane : new Random().Next(1, 2) : dest.LocateCraneNo;
                                commandTrace.NextSource = string.Empty;
                                commandTrace.NextSourceBay = 0;
                                commandTrace.NextSourceBank = 0;
                                if (dest.ShelfType == (int)ShelfType.Port)
                                    commandTrace.NextDest = dest.PLCPortID.ToString();
                                else
                                    commandTrace.NextDest = dest.ShelfID;
                                commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                commandTrace.MainDestBay = Convert.ToInt32(dest.Bank_X);
                                #endregion MOVE
                            }
                            else if (commandTrace.MainTransferMode == (int)TransferMode.FROM)
                            {
                                #region FROM

                                #region Check
                                if (!GetShelfInfoByShelfID(_db, commandTrace.MainSource, out VShelfInfo source))
                                {
                                    if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.MainSource), out source))
                                    {
                                        ///UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                        continue;
                                    }
                                }
                                if (source.CSTID != commandTrace.CarrierID)
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                if (!GetShelfInfoByShelfID(_db, commandTrace.MainDest, out VShelfInfo dest))
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                if (dest.ShelfType != (int)ShelfType.Crane)
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                #endregion Check

                                if (source.LocateCraneNo == (int)LocateCraneNo.Both || (source.LocateCraneNo == dest.LocateCraneNo && source.LocateCraneNo != (int)LocateCraneNo.Both))
                                {
                                    commandTrace.NextTransferMode = (int)TransferMode.FROM;
                                    commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                    if (source.ShelfType == (int)ShelfType.Port)
                                        commandTrace.NextSource = source.PLCPortID.ToString();
                                    else
                                        commandTrace.NextSource = source.ShelfID;
                                    commandTrace.NextSourceBay = Convert.ToInt32(source.Bay_Y);
                                    commandTrace.NextSourceBank = Convert.ToInt32(source.Bank_X);
                                    commandTrace.NextCrane = dest.LocateCraneNo;
                                    commandTrace.NextDest = dest.ShelfID;
                                    commandTrace.NextDestBay = 0;
                                    commandTrace.MainDestBay = 0;
                                }
                                else if (source.LocateCraneNo != dest.LocateCraneNo && source.LocateCraneNo != (int)LocateCraneNo.Both)
                                {
                                    commandTrace.NextTransferMode = (int)TransferMode.FROM_TO;
                                    commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                    if (source.ShelfType == (int)ShelfType.Port)
                                        commandTrace.NextSource = source.PLCPortID.ToString();
                                    else
                                        commandTrace.NextSource = source.ShelfID;
                                    commandTrace.NextSourceBay = Convert.ToInt32(source.Bay_Y);
                                    commandTrace.NextSourceBank = Convert.ToInt32(source.Bank_X);
                                    commandTrace.NextCrane = source.LocateCraneNo;
                                    commandTrace.NextDest = string.Empty;
                                    commandTrace.NextDestBay = 0;
                                    commandTrace.MainDestBay = 0;
                                }
                                else
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                #endregion FROM
                            }
                            else if (commandTrace.MainTransferMode == (int)TransferMode.TO)
                            {
                                #region TO

                                #region Check
                                if (!GetShelfInfoByShelfID(_db, commandTrace.MainSource, out VShelfInfo source))
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                if (source.ShelfType != (int)ShelfType.Crane)
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                if (source.CSTID != commandTrace.CarrierID)
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                if (!GetShelfInfoByShelfID(_db, commandTrace.MainDest, out VShelfInfo dest))
                                {
                                    if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.MainDest), out dest))
                                    {
                                        //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                        continue;
                                    }
                                }
                                #endregion Check

                                if ((source.LocateCraneNo == dest.LocateCraneNo && source.LocateCraneNo != (int)LocateCraneNo.Both) || dest.LocateCraneNo == (int)LocateCraneNo.Both)
                                {
                                    commandTrace.NextTransferMode = (int)TransferMode.TO;
                                    commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                    commandTrace.NextCrane = source.LocateCraneNo;
                                    commandTrace.NextSource = source.ShelfID;
                                    commandTrace.NextSourceBay = 0;
                                    commandTrace.NextSourceBank = 0;
                                    if (dest.ShelfType == (int)ShelfType.Port)
                                        commandTrace.NextDest = dest.PLCPortID.ToString();
                                    else
                                        commandTrace.NextDest = dest.ShelfID;
                                    commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                    commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                }
                                else if (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo != (int)LocateCraneNo.Both)
                                {
                                    commandTrace.NextTransferMode = (int)TransferMode.TO;
                                    commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                    commandTrace.NextCrane = source.LocateCraneNo;
                                    commandTrace.NextSource = source.ShelfID;
                                    commandTrace.NextSourceBay = 0;
                                    commandTrace.NextSourceBank = 0;
                                    commandTrace.NextDest = string.Empty;
                                    commandTrace.NextDestBay = 0;
                                    commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                }
                                else
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                #endregion TO
                            }
                            else if (commandTrace.MainTransferMode == (int)TransferMode.SCAN)
                            {
                                #region SCAN

                                #region Check
                                if (!GetShelfInfoByShelfID(_db, commandTrace.MainSource, out VShelfInfo source))
                                {
                                    if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.MainSource), out source))
                                    {
                                        //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                        continue;
                                    }
                                }
                                #endregion Check

                                commandTrace.NextTransferMode = (int)TransferMode.SCAN;
                                commandTrace.NextTransferModeType = ((int)TransferModeType.Scan).ToString();
                                commandTrace.NextCrane = source.LocateCraneNo == 3 ? commandTrace.MainCrane > 0 && commandTrace.MainCrane != 3 ? commandTrace.MainCrane : new Random().Next(1, 2) : source.LocateCraneNo;
                                commandTrace.NextSource = source.ShelfID;
                                commandTrace.NextSourceBay = Convert.ToInt32(source.Bay_Y);
                                commandTrace.NextSourceBank = Convert.ToInt32(source.Bank_X);
                                commandTrace.NextDest = string.Empty;
                                commandTrace.NextDestBay = 0;
                                commandTrace.NextDestBank = 0;
                                #endregion SCAN
                            }
                            else if (commandTrace.MainTransferMode == (int)TransferMode.FROM_TO)
                            {
                                #region FROM_TO

                                #region Check
                                if (!GetShelfInfoByShelfID(_db, commandTrace.MainSource, out VShelfInfo source))
                                {
                                    if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.MainSource), out source))
                                    {
                                        //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                        continue;
                                    }
                                }
                                if (source.CSTID != commandTrace.CarrierID)
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                if (!GetShelfInfoByShelfID(_db, commandTrace.MainDest, out VShelfInfo dest))
                                {
                                    if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.MainDest), out dest))
                                    {
                                        //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                        continue;
                                    }
                                }
                                #endregion Check

                                commandTrace.NextTransferMode = (int)TransferMode.FROM_TO;
                                commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                if (source.ShelfType == (int)ShelfType.Port)
                                {
                                    commandTrace.NextSource = source.PLCPortID.ToString();
                                }
                                else
                                    commandTrace.NextSource = source.ShelfID;
                                commandTrace.NextSourceBay = Convert.ToInt32(source.Bay_Y);
                                commandTrace.NextSourceBank = Convert.ToInt32(source.Bank_X);
                                if ((source.LocateCraneNo == dest.LocateCraneNo && source.LocateCraneNo != (int)LocateCraneNo.Both) || (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo == (int)LocateCraneNo.Both))
                                {
                                    commandTrace.NextCrane = source.LocateCraneNo;
                                    if (dest.ShelfType == (int)ShelfType.Port)
                                        commandTrace.NextDest = dest.PLCPortID.ToString();
                                    else
                                        commandTrace.NextDest = dest.ShelfID;
                                    commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                    commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                }
                                else if (source.LocateCraneNo != dest.LocateCraneNo && source.LocateCraneNo == (int)LocateCraneNo.Both)
                                {
                                    commandTrace.NextCrane = dest.LocateCraneNo;
                                    if (dest.ShelfType == (int)ShelfType.Port)
                                        commandTrace.NextDest = dest.PLCPortID.ToString();
                                    else
                                        commandTrace.NextDest = dest.ShelfID;
                                    commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                    commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                }
                                else if (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo != (int)LocateCraneNo.Both)
                                {
                                    commandTrace.NextCrane = source.LocateCraneNo;
                                    commandTrace.NextDest = string.Empty;
                                    commandTrace.NextDestBay = 0;
                                    commandTrace.NextDestBank = 0;
                                }
                                else if (source.LocateCraneNo == dest.LocateCraneNo && dest.LocateCraneNo == (int)LocateCraneNo.Both)
                                {
                                    commandTrace.NextCrane = commandTrace.MainCrane > 0 && commandTrace.MainCrane != 3 ? commandTrace.MainCrane : new Random().Next(1, 2);
                                    commandTrace.NextDest = dest.ShelfID;
                                    commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                    commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                }
                                else
                                {
                                    //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                    continue;
                                }
                                #endregion FROM_TO
                            }
                            else
                            {
                                //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                continue;
                            }
                            #endregion 預計先產生的Task
                        }
                        else
                        {
                            commandTrace.ExecTaskState = Convert.ToInt32(row["TTASKSTATE"]);
                            commandTrace.ExecTransferMode = Convert.ToInt32(row["TTRANSFERMODE"]);
                            commandTrace.ExecCrane = Convert.ToInt32(row["TCRANENO"]);
                            commandTrace.ExecFork = Convert.ToInt32(row["TFORKNUMBER"]);
                            commandTrace.ExecSource = Convert.ToString(row["TSOURCE"]);
                            commandTrace.ExecSourceBay = Convert.ToInt32(row["TSOURCEBAY"]);
                            commandTrace.ExecDest = Convert.ToString(row["TDESTINATION"]);
                            commandTrace.ExecDestBay = Convert.ToInt32(row["TDESTINATIONBAY"]);
                            if (commandTrace.ExecTaskState == (int)TaskState.UpdateOK)
                            {
                                commandTrace.ExecFinishLoc = Convert.ToString(row["TFINISHLOC"]);
                                if (commandTrace.ExecTaskState == (int)TaskState.UpdateOK && string.IsNullOrWhiteSpace(commandTrace.ExecFinishLoc))
                                {
                                    #region 重新執行
                                    if (commandTrace.ExecTransferMode == (int)TransferMode.MOVE)
                                    {
                                        #region MOVE

                                        #region Check
                                        if (!GetShelfInfoByShelfID(_db, commandTrace.ExecDest, out VShelfInfo dest))
                                        {
                                            if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.ExecDest), out dest))
                                            {
                                                //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                                continue;
                                            }
                                        }
                                        #endregion Check
                                        commandTrace.NextFork = commandTrace.MainFork;
                                        commandTrace.NextTransferMode = (int)TransferMode.MOVE;
                                        commandTrace.NextTransferModeType = ((int)TransferModeType.Move).ToString();
                                        commandTrace.NextCrane = dest.LocateCraneNo == 3 ? commandTrace.MainCrane > 0 && commandTrace.MainCrane != 3 ? commandTrace.MainCrane : new Random().Next(1, 2) : dest.LocateCraneNo;
                                        commandTrace.NextSource = string.Empty;
                                        commandTrace.NextSourceBay = 0;
                                        commandTrace.NextSourceBank = 0;
                                        if (dest.ShelfType == (int)ShelfType.Port)
                                            commandTrace.NextDest = dest.PLCPortID.ToString();
                                        else
                                            commandTrace.NextDest = dest.ShelfID;
                                        commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                        commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                        #endregion MOVE
                                    }
                                    else if (commandTrace.ExecTransferMode == (int)TransferMode.FROM)
                                    {
                                        #region FROM

                                        #region Check
                                        if (!GetShelfInfoByShelfID(_db, commandTrace.ExecSource, out VShelfInfo source))
                                        {
                                            if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.ExecSource), out source))
                                            {
                                                //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                                continue;
                                            }
                                        }
                                        if (source.CSTID != commandTrace.CarrierID)
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        if (!GetShelfInfoByShelfID(_db, commandTrace.ExecDest, out VShelfInfo dest))
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        if (dest.ShelfType != (int)ShelfType.Crane)
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        #endregion Check

                                        if (source.LocateCraneNo == (int)LocateCraneNo.Both || (source.LocateCraneNo == dest.LocateCraneNo && source.LocateCraneNo != (int)LocateCraneNo.Both))
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextTransferMode = (int)TransferMode.FROM;
                                            commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                            if (source.ShelfType == (int)ShelfType.Port)
                                                commandTrace.NextSource = source.PLCPortID.ToString();
                                            else
                                                commandTrace.NextSource = source.ShelfID;
                                            commandTrace.NextSourceBay = Convert.ToInt32(source.Bay_Y);
                                            commandTrace.NextSourceBank = Convert.ToInt32(source.Bank_X);
                                            commandTrace.NextCrane = dest.LocateCraneNo;
                                            commandTrace.NextDest = dest.ShelfID;
                                            commandTrace.NextDestBay = 0;
                                            commandTrace.NextDestBank = 0;
                                        }
                                        else if (source.LocateCraneNo != dest.LocateCraneNo && source.LocateCraneNo != (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextTransferMode = (int)TransferMode.FROM_TO;
                                            commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                            if (source.ShelfType == (int)ShelfType.Port)
                                                commandTrace.NextSource = source.PLCPortID.ToString();
                                            else
                                                commandTrace.NextSource = source.ShelfID;
                                            commandTrace.NextSourceBay = Convert.ToInt32(source.Bay_Y);
                                            commandTrace.NextSourceBank = Convert.ToInt32(source.Bank_X);
                                            commandTrace.NextCrane = source.LocateCraneNo;
                                            commandTrace.NextDest = string.Empty;
                                            commandTrace.NextDestBay = 0;
                                            commandTrace.NextDestBank = 0;
                                        }
                                        else
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        #endregion FROM
                                    }
                                    else if (commandTrace.ExecTransferMode == (int)TransferMode.TO)
                                    {
                                        #region TO

                                        #region Check
                                        if (!GetShelfInfoByShelfID(_db, commandTrace.ExecSource, out VShelfInfo source))
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        if (source.ShelfType != (int)ShelfType.Crane)
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        if (source.CSTID != commandTrace.CarrierID)
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        if (!GetShelfInfoByShelfID(_db, commandTrace.ExecDest, out VShelfInfo dest))
                                        {
                                            if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.ExecDest), out dest))
                                            {
                                                //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                                continue;
                                            }
                                        }
                                        #endregion Check

                                        if ((source.LocateCraneNo == dest.LocateCraneNo && source.LocateCraneNo != (int)LocateCraneNo.Both) || dest.LocateCraneNo == (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextTransferMode = (int)TransferMode.TO;
                                            commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                            commandTrace.NextCrane = source.LocateCraneNo;
                                            commandTrace.NextSource = source.ShelfID;
                                            commandTrace.NextSourceBay = 0;
                                            commandTrace.NextSourceBank = 0;
                                            if (dest.ShelfType == (int)ShelfType.Port)
                                                commandTrace.NextDest = dest.PLCPortID.ToString();
                                            else
                                                commandTrace.NextDest = dest.ShelfID;
                                            commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                            commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                        }
                                        else if (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo != (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextTransferMode = (int)TransferMode.FROM_TO;
                                            commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                            commandTrace.NextCrane = source.LocateCraneNo;
                                            commandTrace.NextSource = source.ShelfID;
                                            commandTrace.NextSourceBay = 0;
                                            commandTrace.NextSourceBank = 0;
                                            commandTrace.NextDest = string.Empty;
                                            commandTrace.NextDestBay = 0;
                                            commandTrace.NextDestBank = 0;
                                        }
                                        else
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        #endregion TO
                                    }
                                    else if (commandTrace.ExecTransferMode == (int)TransferMode.SCAN)
                                    {
                                        #region SCAN

                                        #region Check
                                        if (!GetShelfInfoByShelfID(_db, commandTrace.ExecSource, out VShelfInfo source))
                                        {
                                            if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.ExecSource), out source))
                                            {
                                                //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                                continue;
                                            }
                                        }
                                        #endregion Check
                                        commandTrace.NextFork = commandTrace.MainFork;
                                        commandTrace.NextTransferMode = (int)TransferMode.SCAN;
                                        commandTrace.NextTransferModeType = ((int)TransferModeType.Scan).ToString();
                                        commandTrace.NextCrane = source.LocateCraneNo == 3 ? commandTrace.MainCrane > 0 && commandTrace.MainCrane != 3 ? commandTrace.MainCrane : new Random().Next(1, 2) : source.LocateCraneNo;
                                        commandTrace.NextSource = source.ShelfID;
                                        commandTrace.NextSourceBay = Convert.ToInt32(source.Bay_Y);
                                        commandTrace.NextSourceBank = Convert.ToInt32(source.Bank_X);
                                        commandTrace.NextDest = string.Empty;
                                        commandTrace.NextDestBay = 0;
                                        commandTrace.NextDestBank = 0;
                                        #endregion SCAN
                                    }
                                    else if (commandTrace.ExecTransferMode == (int)TransferMode.FROM_TO)
                                    {
                                        #region FROM_TO

                                        #region Check
                                        if (!GetShelfInfoByShelfID(_db, commandTrace.ExecSource, out VShelfInfo source))
                                        {
                                            if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.ExecSource), out source))
                                            {
                                                //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                                continue;
                                            }
                                        }
                                        if (source.CSTID != commandTrace.CarrierID)
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        if (!GetShelfInfoByShelfID(_db, commandTrace.ExecDest, out VShelfInfo dest))
                                        {
                                            if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.ExecDest), out dest))
                                            {
                                                //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                                continue;
                                            }
                                        }
                                        #endregion Check

                                        commandTrace.NextTransferMode = (int)TransferMode.FROM_TO;
                                        commandTrace.NextTransferModeType = source.ShelfType.ToString() + dest.ShelfType.ToString();
                                        if (source.ShelfType == (int)ShelfType.Port)
                                        {
                                            commandTrace.NextSource = source.PLCPortID.ToString();
                                        }
                                        else
                                            commandTrace.NextSource = source.ShelfID;
                                        commandTrace.NextSourceBay = Convert.ToInt32(source.Bay_Y);
                                        commandTrace.NextSourceBank = Convert.ToInt32(source.Bank_X);
                                        if ((source.LocateCraneNo == dest.LocateCraneNo && source.LocateCraneNo != (int)LocateCraneNo.Both) || (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo == (int)LocateCraneNo.Both))
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextCrane = source.LocateCraneNo;
                                            if (dest.ShelfType == (int)ShelfType.Port)
                                                commandTrace.NextDest = dest.PLCPortID.ToString();
                                            else
                                                commandTrace.NextDest = dest.ShelfID;
                                            commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                            commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                        }
                                        else if (source.LocateCraneNo != dest.LocateCraneNo && source.LocateCraneNo == (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextCrane = dest.LocateCraneNo;
                                            if (dest.ShelfType == (int)ShelfType.Port)
                                                commandTrace.NextDest = dest.PLCPortID.ToString();
                                            else
                                                commandTrace.NextDest = dest.ShelfID;
                                            commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                            commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                        }
                                        else if (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo != (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextCrane = source.LocateCraneNo;
                                            commandTrace.NextDest = string.Empty;
                                            commandTrace.NextDestBay = 0;
                                            commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                        }
                                        else if (source.LocateCraneNo == dest.LocateCraneNo && dest.LocateCraneNo == (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextCrane = commandTrace.MainCrane > 0 && commandTrace.MainCrane != 3 ? commandTrace.MainCrane : new Random().Next(1, 2);
                                            commandTrace.NextDest = dest.ShelfID;
                                            commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                            commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                        }
                                        else
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                        #endregion FROM_TO
                                    }
                                    else
                                    {
                                        //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                        continue;
                                    }
                                    #endregion 重新執行
                                }
                                else if (!string.IsNullOrWhiteSpace(commandTrace.ExecFinishLoc) && commandTrace.ExecFinishLoc != commandTrace.MainDest)
                                {
                                    #region 未到達目的地，產生新Task

                                    #region Check
                                    if (!GetShelfInfoByShelfID(_db, commandTrace.ExecFinishLoc, out VShelfInfo source))
                                    {
                                        if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.ExecFinishLoc), out source))
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                    }
                                    if (source.CSTID != commandTrace.CarrierID)
                                    {
                                        //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                        continue;
                                    }
                                    if (!GetShelfInfoByShelfID(_db, commandTrace.MainDest, out VShelfInfo dest))
                                    {
                                        if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(commandTrace.MainDest), out dest))
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                    }
                                    #endregion Check

                                    if (source.ShelfType == (int)ShelfType.Crane)
                                    {
                                        commandTrace.NextFork = commandTrace.MainFork;
                                        commandTrace.NextTransferMode = (int)TransferMode.TO;
                                        commandTrace.NextCrane = source.LocateCraneNo;
                                        commandTrace.NextSource = source.ShelfID;
                                        commandTrace.NextSourceBay = 0;
                                        commandTrace.NextSourceBank = 0;
                                        if (source.LocateCraneNo == dest.LocateCraneNo || (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo == (int)LocateCraneNo.Both))
                                        {
                                            if (dest.ShelfType == (int)ShelfType.Port)
                                                commandTrace.NextDest = dest.PLCPortID.ToString();
                                            else
                                                commandTrace.NextDest = dest.ShelfID;
                                            commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                            commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                        }
                                        else if (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo != (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextDest = string.Empty;
                                            commandTrace.NextDestBay = 0;
                                            commandTrace.NextDestBank = 0;
                                        }
                                        else
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        commandTrace.NextTransferMode = (int)TransferMode.FROM_TO;
                                        commandTrace.NextSource = source.ShelfID;
                                        commandTrace.NextSourceBay = Convert.ToInt32(source.Bay_Y);
                                        commandTrace.NextSourceBank = Convert.ToInt32(source.Bank_X);
                                        if ((source.LocateCraneNo == dest.LocateCraneNo && source.LocateCraneNo != (int)LocateCraneNo.Both) || (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo == (int)LocateCraneNo.Both))
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            if (dest.ShelfType == (int)ShelfType.Port)
                                                commandTrace.NextDest = dest.PLCPortID.ToString();
                                            else
                                                commandTrace.NextDest = dest.ShelfID;
                                            commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                            commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                            commandTrace.NextCrane = source.LocateCraneNo;
                                        }
                                        else if (source.LocateCraneNo != dest.LocateCraneNo && source.LocateCraneNo == (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            if (dest.ShelfType == (int)ShelfType.Port)
                                                commandTrace.NextDest = dest.PLCPortID.ToString();
                                            else
                                                commandTrace.NextDest = dest.ShelfID;
                                            commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                            commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                            commandTrace.NextCrane = dest.LocateCraneNo;
                                        }
                                        else if (source.LocateCraneNo == dest.LocateCraneNo && dest.LocateCraneNo == (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextCrane = commandTrace.MainCrane > 0 && commandTrace.MainCrane != 3 ? commandTrace.MainCrane : new Random().Next(1, 2);
                                            commandTrace.NextDest = dest.ShelfID;
                                            commandTrace.NextDestBay = Convert.ToInt32(dest.Bay_Y);
                                            commandTrace.NextDestBank = Convert.ToInt32(dest.Bank_X);
                                        }
                                        else if (source.LocateCraneNo != dest.LocateCraneNo && dest.LocateCraneNo != (int)LocateCraneNo.Both)
                                        {
                                            commandTrace.NextFork = commandTrace.MainFork;
                                            commandTrace.NextCrane = source.LocateCraneNo;
                                            commandTrace.NextDest = string.Empty;
                                            commandTrace.NextDestBay = 0;
                                            commandTrace.NextDestBank = 0;
                                        }
                                        else
                                        {
                                            //UpdateTransferState(_db, commandTrace.CommandID, TransferState.CommandFormatError);
                                            continue;
                                        }
                                    }

                                    commandTrace.NextCraneSpeed = GetCraneSpeed(_db, commandTrace.NextCrane, commandTrace.MainCraneSpeed);
                                    #endregion 未到達目的地，產生新Task
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        }
                        commandTrace.NextCraneSpeed = GetCraneSpeed(_db, commandTrace.NextCrane, commandTrace.MainCraneSpeed);
                        commandTraces.Add(commandTrace);
                    }
                }
                return commandTraces.OrderByDescending(row => row.MainPriority).ThenBy(row => row.QueueDT).ToList();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<CommandTrace>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        #region ExistsTransferCmd
        public bool ExistsTransferCmdByCarrierID(DB _db, string carrierID)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERSTATE <='{(int)TransferState.Complete}'";
                SQL += $" AND CSTID='{carrierID}'";
                return _db.GetDataTable(SQL, ref table) == ErrorCode.Success;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public bool ExistsTransferCmdByCommandID(DB _db, string commandID)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERSTATE <='{(int)TransferState.Complete}'";
                SQL += $" AND COMMANDID='{commandID}'";
                return _db.GetDataTable(SQL, ref table) == ErrorCode.Success;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public bool ExistsDuplicate(DB _db, string bcrReplyCSTID, string source, out VShelfInfo duplicateInfo, out string commandID)
        {
            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND CSTID='{bcrReplyCSTID}'";
                SQL += $" AND SHELFID<>'{source}'";
                SQL += $" ORDER BY STAGE";
                if (_db.GetDataTable(SQL, ref table1) == ErrorCode.Success)
                {
                    duplicateInfo = new VShelfInfo();
                    duplicateInfo.CarrierLoc = Convert.ToString(table1.Rows[0]["CarrierLoc"]);
                    duplicateInfo.ZoneID = Convert.ToString(table1.Rows[0]["ZoneID"]);
                    duplicateInfo.Bank_X = Convert.ToString(table1.Rows[0]["Bank_X"]);
                    duplicateInfo.Bay_Y = Convert.ToString(table1.Rows[0]["Bay_Y"]);
                    duplicateInfo.Level_Z = Convert.ToString(table1.Rows[0]["Level_Z"]);
                    duplicateInfo.LocateCraneNo = Convert.ToInt32(table1.Rows[0]["LOCATECRANENO"]);
                    duplicateInfo.Enable = Convert.ToString(table1.Rows[0]["Enable"]) == "Y";
                    duplicateInfo.EmptyBlockFlag = Convert.ToString(table1.Rows[0]["EmptyBlockFlag"]) == "Y";
                    duplicateInfo.HoldState = Convert.ToInt32(table1.Rows[0]["HoldState"]);
                    duplicateInfo.BCRReadFlag = Convert.ToString(table1.Rows[0]["BCRReadFlag"]) == "Y";
                    duplicateInfo.ShelfID = Convert.ToString(table1.Rows[0]["SHELFID"]);
                    duplicateInfo.ShelfType = Convert.ToInt32(table1.Rows[0]["SHELFTYPE"]);
                    duplicateInfo.ChargeLoc = Convert.ToString(table1.Rows[0]["ChargeLoc"]) == "Y";
                    duplicateInfo.SelectPriority = Convert.ToInt32(table1.Rows[0]["SELECTPRIORITY"]);
                    duplicateInfo.ShelfState = Convert.ToChar(table1.Rows[0]["ShelfState"]);
                    duplicateInfo.HostEQPort = Convert.ToString(table1.Rows[0]["HostEQPortID"]);
                    duplicateInfo.Stage = Convert.ToInt32(table1.Rows[0]["STAGE"]);
                    duplicateInfo.Vehicles = Convert.ToInt32(table1.Rows[0]["Vehicles"]);
                    duplicateInfo.PortType = Convert.ToInt32(table1.Rows[0]["PortType"]);
                    duplicateInfo.PortLocationType = Convert.ToInt32(table1.Rows[0]["PortLocationType"]);
                    duplicateInfo.PLCPortID = Convert.ToInt32(table1.Rows[0]["PLCPortID"]);
                    duplicateInfo.PortTypeIndex = Convert.ToInt32(table1.Rows[0]["PortTypeIndex"]);
                    duplicateInfo.StageCount = Convert.ToInt32(table1.Rows[0]["StageCount"]);
                    duplicateInfo.CSTID = Convert.ToString(table1.Rows[0]["CSTID"]);
                    duplicateInfo.LotID = Convert.ToString(table1.Rows[0]["LotID"]);
                    duplicateInfo.EmptyCST = Convert.ToString(table1.Rows[0]["EmptyCST"]);
                    duplicateInfo.CSTType = Convert.ToString(table1.Rows[0]["CSTType"]);
                    duplicateInfo.CSTState = Convert.ToInt32(table1.Rows[0]["CSTState"]);
                    duplicateInfo.CSTInDT = Convert.ToString(table1.Rows[0]["CSTInDT"]);
                    duplicateInfo.StoreDT = Convert.ToString(table1.Rows[0]["StoreDT"]);

                    SQL = "SELECT * FROM TRANSFERCMD";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    SQL += $" AND CSTID='{bcrReplyCSTID}'";
                    if (_db.GetDataTable(SQL, ref table2) == ErrorCode.Success)
                        commandID = Convert.ToString(table2.Rows[0]["COMMANDID"]);
                    else
                        commandID = string.Empty;
                    return true;
                }
                else
                {
                    commandID = string.Empty;
                    duplicateInfo = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                commandID = string.Empty;
                duplicateInfo = null;
                return false;
            }
            finally
            {
                table1?.Clear();
                table1?.Dispose();
                table2?.Clear();
                table2?.Dispose();
            }
        }

        public bool ExistsTransferCmd(DB _db, string commandID, string carrierID, out string recommandID, out string recarrierID)
        {
            DataTable table = new DataTable();
            recommandID = string.Empty;
            recarrierID = string.Empty;
            try
            {
                string SQL = "SELECT * FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERSTATE<='{(int)TransferState.Complete}'";
                SQL += $" AND (COMMANDID='{commandID}'";
                SQL += $" OR CSTID='{carrierID}')";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    recommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    recarrierID = Convert.ToString(table.Rows[0]["CSTID"]);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        #endregion ExistsTransferCmd

        #region GetTransfer
        public IEnumerable<Stocker.TaskControl.Info.TransferCommand> GetAllTransferCmd(DB _db, TransferState transferState = TransferState.Complete)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERSTATE<='{(int)transferState}'";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<Stocker.TaskControl.Info.TransferCommand> transfers = new List<Stocker.TaskControl.Info.TransferCommand>();
                    for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                    {
                        Stocker.TaskControl.Info.TransferCommand transfer = new Stocker.TaskControl.Info.TransferCommand();
                        transfer.CommandID = Convert.ToString(table.Rows[iRow]["COMMANDID"]);
                        transfer.CSTID = Convert.ToString(table.Rows[iRow]["CSTID"]);
                        transfer.CraneNo = Convert.ToInt32(table.Rows[iRow]["CRANENO"]);
                        transfer.TransferState = Convert.ToInt32(table.Rows[iRow]["TRANSFERSTATE"]);
                        transfer.TransferMode = Convert.ToInt32(table.Rows[iRow]["TRANSFERMODE"]);
                        transfer.HostSource = Convert.ToString(table.Rows[iRow]["HOSTSOURCE"]);
                        transfer.Source = Convert.ToString(table.Rows[iRow]["SOURCE"]);
                        transfer.SourceBay = Convert.ToInt32(table.Rows[iRow]["SOURCEBAY"]);
                        transfer.HostDestination = Convert.ToString(table.Rows[iRow]["HOSTDESTINATION"]);
                        transfer.Destination = Convert.ToString(table.Rows[iRow]["DESTINATION"]);
                        transfer.DestinationBay = Convert.ToInt32(table.Rows[iRow]["DESTINATIONBAY"]);
                        transfer.NextDest = Convert.ToString(table.Rows[iRow]["NEXTDEST"]);
                        transfer.TravelAxisSpeed = Convert.ToInt32(table.Rows[iRow]["TRAVELAXISSPEED"]);
                        transfer.LifterAxisSpeed = Convert.ToInt32(table.Rows[iRow]["LIFTERAXISSPEED"]);
                        transfer.RotateAxisSpeed = Convert.ToInt32(table.Rows[iRow]["ROTATEAXISSPEED"]);
                        transfer.ForkAxisSpeed = Convert.ToInt32(table.Rows[iRow]["FORKAXISSPEED"]);
                        transfer.LotID = Convert.ToString(table.Rows[iRow]["LOTID"]);
                        transfer.EmptyCST = Convert.ToString(table.Rows[iRow]["EMPTYCST"]);
                        transfer.CSTType = Convert.ToString(table.Rows[iRow]["CSTTYPE"]);
                        transfer.UserID = Convert.ToString(table.Rows[iRow]["USERID"]);
                        transfer.BCRReadFlag = Convert.ToString(table.Rows[iRow]["BCRREADFLAG"]);
                        transfer.BCRReplyCSTID = Convert.ToString(table.Rows[iRow]["BCRREPLYCSTID"]);
                        transfer.HostPriority = Convert.ToInt32(table.Rows[iRow]["HOSTPRIORITY"]);
                        transfer.Priority = Convert.ToInt32(table.Rows[iRow]["PRIORITY"]);
                        transfer.QueueDT = Convert.ToString(table.Rows[iRow]["QUEUEDT"]);
                        transfer.InitialDT = Convert.ToString(table.Rows[iRow]["INITIALDT"]);
                        transfer.ActiveDT = Convert.ToString(table.Rows[iRow]["ACTIVEDT"]);
                        transfer.StorageAltDT = Convert.ToString(table.Rows[iRow]["STORAGEALTDT"]);
                        transfer.FinishDT = Convert.ToString(table.Rows[iRow]["FINISHDT"]);
                        transfer.CurrentPosition = Convert.ToString(table.Rows[iRow]["CURRENTPOSITION"]);
                        transfer.FinishLocation = Convert.ToString(table.Rows[iRow]["FINISHLOCATION"]);
                        transfer.CompleteCode = Convert.ToString(table.Rows[iRow]["COMPLETECODE"]);
                        transfer.AbortFlag = Convert.ToString(table.Rows[iRow]["ABORTFLAG"]) == "Y";
                        transfer.CancelFlag = Convert.ToString(table.Rows[iRow]["CANCELFLAG"]) == "Y";
                        transfer.RetryFlag = Convert.ToString(table.Rows[iRow]["RETRYFLAG"]) == "Y";
                        transfer.BatchID = Convert.ToString(table.Rows[iRow]["BatchID"]);
                        transfers.Add(transfer);
                    }
                    return transfers;
                }
                else
                    return new List<Stocker.TaskControl.Info.TransferCommand>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<Stocker.TaskControl.Info.TransferCommand>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public Stocker.TaskControl.Info.TransferCommand GetTransferCmdByCommandID(DB _db, string commandID, int TRANSFERSTATE = (int)TransferState.Complete)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERSTATE <'{TRANSFERSTATE}'";
                if (!string.IsNullOrWhiteSpace(commandID))
                    SQL += $" AND COMMANDID='{commandID}'";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    Stocker.TaskControl.Info.TransferCommand transfer = new Stocker.TaskControl.Info.TransferCommand();
                    transfer.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    transfer.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    transfer.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                    transfer.TransferState = Convert.ToInt32(table.Rows[0]["TRANSFERSTATE"]);
                    transfer.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                    transfer.HostSource = Convert.ToString(table.Rows[0]["HOSTSOURCE"]);
                    transfer.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                    transfer.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                    transfer.HostDestination = Convert.ToString(table.Rows[0]["HOSTDESTINATION"]);
                    transfer.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                    transfer.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                    transfer.NextDest = Convert.ToString(table.Rows[0]["NEXTDEST"]);
                    transfer.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    transfer.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    transfer.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    transfer.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                    transfer.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                    transfer.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                    transfer.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                    transfer.UserID = Convert.ToString(table.Rows[0]["USERID"]);
                    transfer.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]);
                    transfer.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                    transfer.HostPriority = Convert.ToInt32(table.Rows[0]["HOSTPRIORITY"]);
                    transfer.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                    transfer.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                    transfer.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                    transfer.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                    transfer.StorageAltDT = Convert.ToString(table.Rows[0]["STORAGEALTDT"]);
                    transfer.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                    transfer.CurrentPosition = Convert.ToString(table.Rows[0]["CURRENTPOSITION"]);
                    transfer.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                    transfer.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                    transfer.AbortFlag = Convert.ToString(table.Rows[0]["ABORTFLAG"]) == "Y";
                    transfer.CancelFlag = Convert.ToString(table.Rows[0]["CANCELFLAG"]) == "Y";
                    transfer.RetryFlag = Convert.ToString(table.Rows[0]["RETRYFLAG"]) == "Y";
                    transfer.ForkNumber = Convert.ToInt32(table.Rows[0]["ForkNumber"]);
                    transfer.BatchID = Convert.ToString(table.Rows[0]["BatchID"]);
                    return transfer;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return null;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public Stocker.TaskControl.Info.TransferCommand GetTransferCmdByCommandID(string commandID)
        {
            using (DB _db = GetDB(_TaskInfo.Config))
            {
                DataTable table = new DataTable();
                try
                {
                    string SQL = "SELECT * FROM TRANSFERCMD";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    SQL += $" AND COMMANDID='{commandID}'";

                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        Stocker.TaskControl.Info.TransferCommand transfer = new Stocker.TaskControl.Info.TransferCommand();
                        transfer.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                        transfer.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                        transfer.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                        transfer.TransferState = Convert.ToInt32(table.Rows[0]["TRANSFERSTATE"]);
                        transfer.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                        transfer.HostSource = Convert.ToString(table.Rows[0]["HOSTSOURCE"]);
                        transfer.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                        transfer.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                        transfer.HostDestination = Convert.ToString(table.Rows[0]["HOSTDESTINATION"]);
                        transfer.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                        transfer.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                        transfer.NextDest = Convert.ToString(table.Rows[0]["NEXTDEST"]);
                        transfer.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                        transfer.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                        transfer.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                        transfer.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                        transfer.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                        transfer.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                        transfer.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                        transfer.UserID = Convert.ToString(table.Rows[0]["USERID"]);
                        transfer.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]);
                        transfer.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                        transfer.HostPriority = Convert.ToInt32(table.Rows[0]["HOSTPRIORITY"]);
                        transfer.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                        transfer.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                        transfer.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                        transfer.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                        transfer.StorageAltDT = Convert.ToString(table.Rows[0]["STORAGEALTDT"]);
                        transfer.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                        transfer.CurrentPosition = Convert.ToString(table.Rows[0]["CURRENTPOSITION"]);
                        transfer.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                        transfer.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                        transfer.AbortFlag = Convert.ToString(table.Rows[0]["ABORTFLAG"]) == "Y";
                        transfer.CancelFlag = Convert.ToString(table.Rows[0]["CANCELFLAG"]) == "Y";
                        transfer.RetryFlag = Convert.ToString(table.Rows[0]["RETRYFLAG"]) == "Y";
                        transfer.ForkNumber = Convert.ToInt32(table.Rows[0]["ForkNumber"]);
                        transfer.BatchID = Convert.ToString(table.Rows[0]["BatchID"]);
                        return transfer;
                    }
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                    return null;
                }
                finally
                {
                    table?.Clear();
                    table?.Dispose();
                }
            }
        }
        public bool GetTransferCmdByCommandID(DB _db, string commandID, out Stocker.TaskControl.Info.TransferCommand transfer)
        {
            DataTable table = new DataTable();
            transfer = null;
            try
            {
                string SQL = "SELECT * FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERSTATE <='{(int)TransferState.Complete}'";
                if (!string.IsNullOrWhiteSpace(commandID))
                    SQL += $" AND COMMANDID='{commandID}'";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    transfer = new Stocker.TaskControl.Info.TransferCommand();
                    transfer.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    transfer.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    transfer.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                    transfer.TransferState = Convert.ToInt32(table.Rows[0]["TRANSFERSTATE"]);
                    transfer.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                    transfer.HostSource = Convert.ToString(table.Rows[0]["HOSTSOURCE"]);
                    transfer.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                    transfer.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                    transfer.HostDestination = Convert.ToString(table.Rows[0]["HOSTDESTINATION"]);
                    transfer.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                    transfer.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                    transfer.NextDest = Convert.ToString(table.Rows[0]["NEXTDEST"]);
                    transfer.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    transfer.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    transfer.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    transfer.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                    transfer.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                    transfer.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                    transfer.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                    transfer.UserID = Convert.ToString(table.Rows[0]["USERID"]);
                    transfer.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]);
                    transfer.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                    transfer.HostPriority = Convert.ToInt32(table.Rows[0]["HOSTPRIORITY"]);
                    transfer.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                    transfer.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                    transfer.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                    transfer.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                    transfer.StorageAltDT = Convert.ToString(table.Rows[0]["STORAGEALTDT"]);
                    transfer.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                    transfer.CurrentPosition = Convert.ToString(table.Rows[0]["CURRENTPOSITION"]);
                    transfer.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                    transfer.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                    transfer.AbortFlag = Convert.ToString(table.Rows[0]["ABORTFLAG"]) == "Y";
                    transfer.CancelFlag = Convert.ToString(table.Rows[0]["CANCELFLAG"]) == "Y";
                    transfer.RetryFlag = Convert.ToString(table.Rows[0]["RETRYFLAG"]) == "Y";
                    transfer.ForkNumber = Convert.ToInt32(table.Rows[0]["ForkNumber"]);
                    transfer.BatchID = Convert.ToString(table.Rows[0]["BatchID"]);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public bool GetTransferCmdByCarrierID(DB _db, string carrierID, out Stocker.TaskControl.Info.TransferCommand transfer)
        {
            DataTable table = new DataTable();
            transfer = null;
            try
            {
                string SQL = "SELECT * FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND CSTID='{carrierID}'";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    transfer = new Stocker.TaskControl.Info.TransferCommand();
                    transfer.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    transfer.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    transfer.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                    transfer.TransferState = Convert.ToInt32(table.Rows[0]["TRANSFERSTATE"]);
                    transfer.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                    transfer.HostSource = Convert.ToString(table.Rows[0]["HOSTSOURCE"]);
                    transfer.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                    transfer.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                    transfer.HostDestination = Convert.ToString(table.Rows[0]["HOSTDESTINATION"]);
                    transfer.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                    transfer.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                    transfer.NextDest = Convert.ToString(table.Rows[0]["NEXTDEST"]);
                    transfer.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    transfer.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    transfer.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    transfer.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                    transfer.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                    transfer.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                    transfer.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                    transfer.UserID = Convert.ToString(table.Rows[0]["USERID"]);
                    transfer.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]);
                    transfer.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                    transfer.HostPriority = Convert.ToInt32(table.Rows[0]["HOSTPRIORITY"]);
                    transfer.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                    transfer.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                    transfer.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                    transfer.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                    transfer.StorageAltDT = Convert.ToString(table.Rows[0]["STORAGEALTDT"]);
                    transfer.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                    transfer.CurrentPosition = Convert.ToString(table.Rows[0]["CURRENTPOSITION"]);
                    transfer.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                    transfer.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                    transfer.AbortFlag = Convert.ToString(table.Rows[0]["ABORTFLAG"]) == "Y";
                    transfer.CancelFlag = Convert.ToString(table.Rows[0]["CANCELFLAG"]) == "Y";
                    transfer.RetryFlag = Convert.ToString(table.Rows[0]["RETRYFLAG"]) == "Y";
                    transfer.ForkNumber = Convert.ToInt32(table.Rows[0]["ForkNumber"]);
                    transfer.BatchID = Convert.ToString(table.Rows[0]["BatchID"]);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public void UpdateTransferCmdDest(string commandID, string dest)
        {
            using (DB _db = GetDB(_TaskInfo.Config))
            {
                string SQL = "UPDATE TRANSFERCMD SET";
                SQL += $" DESTINATION='{dest}'";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND COMMANDID='{commandID}'";
                _db.ExecuteSQL(SQL);
            }
        }

        public Stocker.TaskControl.Info.TransferCommand GetTransferCmdByCarrierID(string carrierID)
        {
            DataTable table = new DataTable();
            try
            {
                using (DB _db = GetDB(_TaskInfo.Config))
                {
                    string SQL = "SELECT * FROM TRANSFERCMD";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    SQL += $" AND TRANSFERSTATE <='{(int)TransferState.Complete}'";
                    SQL += $" AND CSTID='{carrierID}'";

                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        Stocker.TaskControl.Info.TransferCommand transfer = new Stocker.TaskControl.Info.TransferCommand();
                        transfer.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                        transfer.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                        transfer.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                        transfer.TransferState = Convert.ToInt32(table.Rows[0]["TRANSFERSTATE"]);
                        transfer.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                        transfer.HostSource = Convert.ToString(table.Rows[0]["HOSTSOURCE"]);
                        transfer.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                        transfer.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                        transfer.HostDestination = Convert.ToString(table.Rows[0]["HOSTDESTINATION"]);
                        transfer.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                        transfer.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                        transfer.NextDest = Convert.ToString(table.Rows[0]["NEXTDEST"]);
                        transfer.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                        transfer.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                        transfer.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                        transfer.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                        transfer.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                        transfer.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                        transfer.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                        transfer.UserID = Convert.ToString(table.Rows[0]["USERID"]);
                        transfer.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]);
                        transfer.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                        transfer.HostPriority = Convert.ToInt32(table.Rows[0]["HOSTPRIORITY"]);
                        transfer.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                        transfer.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                        transfer.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                        transfer.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                        transfer.StorageAltDT = Convert.ToString(table.Rows[0]["STORAGEALTDT"]);
                        transfer.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                        transfer.CurrentPosition = Convert.ToString(table.Rows[0]["CURRENTPOSITION"]);
                        transfer.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                        transfer.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                        transfer.AbortFlag = Convert.ToString(table.Rows[0]["ABORTFLAG"]) == "Y";
                        transfer.CancelFlag = Convert.ToString(table.Rows[0]["CANCELFLAG"]) == "Y";
                        transfer.RetryFlag = Convert.ToString(table.Rows[0]["RETRYFLAG"]) == "Y";
                        return transfer;
                    }
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return null;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        #endregion GetTransfer

        public int UpdateTransferStateToTransferring(DB _db, string commandID, ref string strEM, string initialDT = "")
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "UPDATE TRANSFERCMD SET";
            if (!string.IsNullOrWhiteSpace(initialDT))
                SQL += $" INITIALDT='{initialDT}',";
            SQL += $" TRANSFERSTATE='{(int)TransferState.Transferring}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL, ref strEM);
        }
        public int UpdateTransferStateToInitialize(DB _db, string commandID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "UPDATE TRANSFERCMD SET";
            SQL += $" TRANSFERSTATE='{(int)TransferState.Initialize}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }

        public int UpdateTransferBatchIDForLCS(DB _db, string commandID, string BatchID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "UPDATE TRANSFERCMD SET";
            SQL += $" BatchID='{BatchID}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }

        public int UpdateTransferState(DB _db, string commandID, TransferState state)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "UPDATE TRANSFERCMD SET";
            SQL += $" TRANSFERSTATE='{(int)state}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }

        public int UpdateTransferStateToAborting(DB _db, string commandID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "UPDATE TRANSFERCMD SET";
            SQL += $" TRANSFERSTATE='{(int)TransferState.Aborting}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }

        public int UpdateTransferStatToUpdateFaile(string commandID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            using (DB _db = GetDB(_TaskInfo.Config))
            {
                string SQL = "UPDATE TRANSFERCMD SET";
                SQL += $" TRANSFERSTATE='{(int)TransferState.UpdateFail}'";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND COMMANDID='{commandID}'";
                return _db.ExecuteSQL(SQL);
            }
        }

        public int AbortTransfer(DB _db, string commandID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "UPDATE TRANSFERCMD SET";
            SQL += $" ABORTFLAG='Y'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            //SQL += $" AND TRANSFERSTATE='{(int)TransferState.Aborting}'";
            SQL += $" AND ABORTFLAG='N'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }

        public int CancelTransfer(DB _db, string commandID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "UPDATE TRANSFERCMD SET";
            SQL += $" CANCELFLAG='Y'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND TRANSFERSTATE='{(int)TransferState.Queue}'";
            SQL += $" AND CANCELFLAG='N'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }

        public int InsertTransferToHistory(DB _db, string commandID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "INSERT INTO HISTRANSFERCMD";
            SQL += $" SELECT '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', * FROM TRANSFERCMD";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }
        public int DeleteTransfer(DB _db, string commandID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;
            string SQL = "DELETE FROM TRANSFERCMD";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }
        public int UpdateTransferState(DB _db, UpdateCommandInfo info, TransferState newTransferState)
        {
            if (info == null)
                return ErrorCode.Exception;

            string SQL = string.Empty;
            try
            {
                SQL = "UPDATE TRANSFERCMD SET";
                SQL += $" TRANSFERCMD.TRANSFERSTATE='{(int)newTransferState}',";
                SQL += " TRANSFERCMD.CURRENTPOSITION=TASK.FINISHLOCATION,";
                SQL += " TRANSFERCMD.FINISHLOCATION=TASK.FINISHLOCATION,";
                SQL += " TRANSFERCMD.BCRREPLYCSTID=TASK.BCRREPLYCSTID,";
                SQL += " TRANSFERCMD.FINISHDT=TASK.FINISHDT,";
                SQL += " TRANSFERCMD.COMPLETECODE=TASK.COMPLETECODE,";
                SQL += " TRANSFERCMD.ACTIVEDT=(CASE TRANSFERCMD.ACTIVEDT WHEN '' THEN TASK.ACTIVEDT ELSE TRANSFERCMD.ACTIVEDT END)";
                SQL += " FROM TRANSFERCMD, TASK";
                SQL += " WHERE TRANSFERCMD.STOCKERID=TASK.STOCKERID";
                SQL += " AND TRANSFERCMD.COMMANDID=TASK.COMMANDID";
                SQL += $" AND TRANSFERCMD.STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERCMD.COMMANDID='{info.CommandID}'";
                SQL += $" AND TASK.TASKNO='{info.TaskNo}'";
                SQL += $" AND TASK.TASKSTATE='{(int)TaskState.UpdateOK}'";
                return _db.ExecuteSQL(SQL);
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return ErrorCode.Exception;
            }
        }
        public int UpdateTransferStateToCancelOK(DB _db, UpdateCommandInfo info)
        {
            if (info == null)
                return ErrorCode.Exception;

            string SQL = string.Empty;
            try
            {
                SQL = "UPDATE TRANSFERCMD SET";
                SQL += $" TRANSFERSTATE='{(int)TransferState.UpdateOK_Cancel}'";
                SQL += " FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERCMD.COMMANDID='{info.CommandID}'";
                return _db.ExecuteSQL(SQL);
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return ErrorCode.Exception;
            }
        }
        public int UpdateMCSPriority(DB _db, string commandID, int priority)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            if (priority <= 0)
                return ErrorCode.Exception;

            string SQL = "UPDATE TRANSFERCMD SET";
            SQL += $" HOSTPRIORITY='{priority}',";
            SQL += $" PRIORITY='{priority * 10}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            SQL += $" AND TRANSFERSTATE='{(int)TransferState.Queue}'";
            return _db.ExecuteSQL(SQL);
        }

        public int UpdateLCSPriority(DB _db, string commandID, int priority)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            if (priority <= 0)
                return ErrorCode.Exception;

            string SQL = "UPDATE TRANSFERCMD SET";
            SQL += $" PRIORITY='{priority}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }

        #endregion Transfer

        #region Task
        public string GetScanCommandID(DB _db) => $"SCAN{DateTime.Now.ToString("yyyyMMddHHmmss")}" + funSeqNo(_db, "CMD");
        public string GetTransferCommandID(DB _db) => $"MANUAL{DateTime.Now.ToString("yyyyMMddHHmmss")}" + funSeqNo(_db, "CMD");
        public string GetBatchID(DB _db) => funSeqNo(_db, "BATCHID");

        private string funSeqNo(DB _DB, string strSNOTYPE)
        {
            string strSQL = string.Empty;
            string strCmdSno = string.Empty;
            DataTable table = new DataTable();
            int intNewCmdSno = 0;
            int intTimes = 0;
            int intCmdSno = 0;
            string strEM = string.Empty;

            try
            {
                do
                {
                    intTimes++;

                    strSQL = "SELECT SNO FROM SnoCtrl";
                    strSQL += " WHERE SNOTYPE='" + strSNOTYPE + "'";

                    if (_DB.GetDataTable(strSQL, ref table) == ErrorCode.Success)
                    {
                        DataRow[] tableRows = table.Select();
                        strCmdSno = tableRows[0]["SNO"].ToString();
                        int.TryParse(strCmdSno, out intCmdSno);
                        if (intCmdSno >= 999 || intCmdSno == 0)
                            intNewCmdSno = 1;
                        else
                            intNewCmdSno = intCmdSno + 1;

                        strSQL = "UPDATE SnoCtrl SET";
                        strSQL += " SNO='" + intNewCmdSno + "',";
                        strSQL += " TrnDT=CONVERT(VARCHAR(20), GETDATE(), 120)";
                        strSQL += " WHERE SNOTYPE='" + strSNOTYPE + "'";
                        strSQL += " AND SUBSTRING(TRNDT,1,10)=CONVERT(VARCHAR(10), GETDATE(), 120)";
                        strSQL += " AND SNO='" + strCmdSno + "'";

                        if (_DB.ExecuteSQL(strSQL, ref strEM) == ErrorCode.Success)
                            return (strCmdSno.PadLeft(3, "0"[0]));
                        else
                        {
                            strSQL = "UPDATE SnoCtrl SET";
                            strSQL += " SNO='" + intNewCmdSno + "',";
                            strSQL += " TrnDT=CONVERT(VARCHAR(20), GETDATE(), 120)";
                            strSQL += " WHERE SNOTYPE='" + strSNOTYPE + "'";
                            strSQL += " AND SNO='" + strCmdSno + "'";

                            if (_DB.ExecuteSQL(strSQL, ref strEM) == ErrorCode.Success)
                                return (strCmdSno.PadLeft(3, "0"[0]));

                        }
                    }
                    else
                    {
                        strSQL = "UPDATE SnoCtrl SET";
                        strSQL += " SNO='2',";
                        strSQL += " TrnDT=CONVERT(VARCHAR(23), GETDATE(), 120)";
                        strSQL += " WHERE SNOTYPE='" + strSNOTYPE + "'";

                        if (_DB.ExecuteSQL(strSQL, ref strEM) != ErrorCode.Success)
                        {
                            strSQL = "INSERT INTO SnoCtrl (TrnDT, SNOTYPE, SNO) VALUES (";
                            strSQL += "CONVERT(VARCHAR(23), GETDATE(), 120), ";
                            strSQL += "'" + strSNOTYPE + "', ";
                            strSQL += "'2')";
                            _DB.ExecuteSQL(strSQL, ref strEM);
                        }
                        return "001";
                    }
                } while (intTimes < 10);

                return string.Empty;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return string.Empty;
            }
        }

        public string GetMultiTaskNo(DB _db, int iCrane, int iFork, string strSNOTYPE)
        {
            DataTable table = new DataTable();
            try
            {
                string sType = strSNOTYPE.ToUpper().Trim() + iCrane.ToString() + iFork.ToString();
                string SQL = "Select SNO from SnoCtrl where SNOTYPE='" + sType + "'";
                string err = string.Empty;
                if (_db.GetDataTable(SQL, ref table, ref err) == ErrorCode.Success)
                {
                    int Sno = Convert.ToInt32(table.Rows[0]["SNO"]);
                    var newSno = Sno + 1;
                    SQL = "UPDATE SNOCTRL SET";
                    if (newSno >= 1000)
                    {
                        SQL += " SNO='1',";
                        newSno = 1;
                    }
                    else
                        SQL += $" SNO='{newSno}',";

                    SQL += $" TRNDT='{DateTime.Now.ToString("yyyy-MM-dd")}'";
                    SQL += $" WHERE SNOTYPE='{sType}' and SNO = '{Sno}'";
                    if (_db.ExecuteSQL(SQL, ref err) == ErrorCode.Success)
                        return $"{System.DateTime.Now:yyMMdd}{iCrane}{iFork}{newSno:D3}";
                }
                else
                {
                    SQL = "INSERT INTO SNOCTRL (TrnDT, SNOTYPE, SNO) VALUES (";
                    SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd")}', ";
                    SQL += $"'{sType}', ";
                    SQL += "'1')";
                    _db.ExecuteSQL(SQL);
                    return $"{System.DateTime.Now:yyMMdd}{iCrane}{iFork}001";
                }

                return "";
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return string.Empty;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public bool ExistsPortTaskExecutting(DB _db, int PLCPortID)
        {
            string SQL = string.Empty;
            DataTable table = new DataTable();
            try
            {
                SQL = "SELECT * FROM TASK";
                SQL += $" WHERE (SOURCE = '{PLCPortID}'";
                SQL += $" AND (TRANSFERMODE = '{(int)TransferMode.FROM_TO}' OR TRANSFERMODE = '{(int)TransferMode.FROM}'))";
                SQL += $" OR (DESTINATION = '{PLCPortID}'";
                SQL += $" AND (TRANSFERMODE = '{(int)TransferMode.FROM_TO}' OR TRANSFERMODE = '{(int)TransferMode.TO}'))";
                SQL += $" AND TASKSTATE < '{(int)TaskState.Complete}'";
                _db.GetDataTable(SQL, ref table);
                return table.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public int ExistsTaskExecutting(DB _db, int craneNo)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TASK";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND CRANENO ='{craneNo}'";
                //SQL += $" AND FORKNUMBER ='{forkNumber}'";
                SQL += $" AND TASKSTATE<'{(int)TaskState.Complete}'";
                _db.GetDataTable(SQL, ref table);
                return table.Rows.Count;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return 0;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public IEnumerable<TaskCmd> GetTaskByCommandID(DB _db, string commandID)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TASK";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND COMMANDID ='{commandID}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<TaskCmd> lstShelfDef = new List<TaskCmd>();
                    foreach (DataRow row in table.Rows)
                    {
                        TaskCmd taskCmd = new TaskCmd();
                        taskCmd.CommandID = Convert.ToString(row["COMMANDID"]);
                        taskCmd.TaskNo = Convert.ToString(row["TASKNO"]);
                        taskCmd.CraneNo = Convert.ToInt32(row["CRANENO"]);
                        taskCmd.ForkNumber = Convert.ToInt32(row["FORKNUMBER"]);
                        taskCmd.CSTID = Convert.ToString(row["CSTID"]);
                        taskCmd.TaskState = Convert.ToInt32(row["TASKSTATE"]);
                        taskCmd.CompleteCode = Convert.ToString(row["COMPLETECODE"]);
                        taskCmd.TransferMode = Convert.ToInt32(row["TRANSFERMODE"]);
                        taskCmd.Source = Convert.ToString(row["SOURCE"]);
                        taskCmd.SourceBay = Convert.ToInt32(row["SOURCEBAY"]);
                        taskCmd.Destination = Convert.ToString(row["DESTINATION"]);
                        taskCmd.DestinationBay = Convert.ToInt32(row["DESTINATIONBAY"]);
                        taskCmd.AtSourceDT = Convert.ToString(row["ATSOURCEDT"]);
                        taskCmd.AtDestinationDT = Convert.ToString(row["ATDESTINATIONDT"]);
                        taskCmd.TravelAxisSpeed = Convert.ToInt32(row["TRAVELAXISSPEED"]);
                        taskCmd.LifterAxisSpeed = Convert.ToInt32(row["LIFTERAXISSPEED"]);
                        taskCmd.RotateAxisSpeed = Convert.ToInt32(row["ROTATEAXISSPEED"]);
                        taskCmd.ForkAxisSpeed = Convert.ToInt32(row["FORKAXISSPEED"]);
                        taskCmd.LotID = Convert.ToString(row["LOTID"]);
                        taskCmd.EmptyCST = Convert.ToString(row["EMPTYCST"]);
                        taskCmd.CSTType = Convert.ToString(row["CSTTYPE"]);
                        taskCmd.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                        taskCmd.BCRReadDT = Convert.ToString(row["BCRREADDT"]);
                        taskCmd.BCRReplyCSTID = Convert.ToString(row["BCRREPLYCSTID"]);
                        taskCmd.BCRReadStatus = Convert.ToInt32(row["BCRREADSTATUS"]);
                        taskCmd.Priority = Convert.ToInt32(row["PRIORITY"]);
                        taskCmd.QueueDT = Convert.ToString(row["QUEUEDT"]);
                        taskCmd.InitialDT = Convert.ToString(row["INITIALDT"]);
                        taskCmd.WaitingDT = Convert.ToString(row["WAITINGDT"]);
                        taskCmd.ActiveDT = Convert.ToString(row["ACTIVEDT"]);
                        taskCmd.FinishDT = Convert.ToString(row["FINISHDT"]);
                        taskCmd.FinishLocation = Convert.ToString(row["FINISHLOCATION"]);
                        taskCmd.C1StartDT = Convert.ToString(row["C1STARTDT"]);
                        taskCmd.CSTOnDT = Convert.ToString(row["CSTONDT"]);
                        taskCmd.CSTTakeOffDT = Convert.ToString(row["CSTTAKEOFFDT"]);
                        taskCmd.C2StartDT = Convert.ToString(row["C2STARTDT"]);
                        taskCmd.T1 = Convert.ToInt32(row["T1"]);
                        taskCmd.T2 = Convert.ToInt32(row["T2"]);
                        taskCmd.T3 = Convert.ToInt32(row["T3"]);
                        taskCmd.T4 = Convert.ToInt32(row["T4"]);
                        taskCmd.F1StartDT = Convert.ToString(row["F1STARTDT"]);
                        taskCmd.F2StartDT = Convert.ToString(row["F2STARTDT"]);
                        taskCmd.NextDest = Convert.ToInt32(row["NEXTDEST"]);
                        lstShelfDef.Add(taskCmd);
                    }
                    return lstShelfDef;
                }
                else
                    return new List<TaskCmd>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<TaskCmd>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public IEnumerable<TaskCmd> GetTaskByCommandID(string commandID, int craneNo)
        {
            using (DB _db = GetDB())
            {
                DataTable table = new DataTable();
                try
                {
                    string SQL = "SELECT * FROM TASK";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    SQL += $" AND COMMANDID ='{commandID}'";
                    SQL += $" AND CraneNo ='{craneNo}'";
                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        List<TaskCmd> lstShelfDef = new List<TaskCmd>();
                        foreach (DataRow row in table.Rows)
                        {
                            TaskCmd taskCmd = new TaskCmd();
                            taskCmd.CommandID = Convert.ToString(row["COMMANDID"]);
                            taskCmd.TaskNo = Convert.ToString(row["TASKNO"]);
                            taskCmd.CraneNo = Convert.ToInt32(row["CRANENO"]);
                            taskCmd.ForkNumber = Convert.ToInt32(row["FORKNUMBER"]);
                            taskCmd.CSTID = Convert.ToString(row["CSTID"]);
                            taskCmd.TaskState = Convert.ToInt32(row["TASKSTATE"]);
                            taskCmd.CompleteCode = Convert.ToString(row["COMPLETECODE"]);
                            taskCmd.TransferMode = Convert.ToInt32(row["TRANSFERMODE"]);
                            taskCmd.Source = Convert.ToString(row["SOURCE"]);
                            taskCmd.SourceBay = Convert.ToInt32(row["SOURCEBAY"]);
                            taskCmd.Destination = Convert.ToString(row["DESTINATION"]);
                            taskCmd.DestinationBay = Convert.ToInt32(row["DESTINATIONBAY"]);
                            taskCmd.AtSourceDT = Convert.ToString(row["ATSOURCEDT"]);
                            taskCmd.AtDestinationDT = Convert.ToString(row["ATDESTINATIONDT"]);
                            taskCmd.TravelAxisSpeed = Convert.ToInt32(row["TRAVELAXISSPEED"]);
                            taskCmd.LifterAxisSpeed = Convert.ToInt32(row["LIFTERAXISSPEED"]);
                            taskCmd.RotateAxisSpeed = Convert.ToInt32(row["ROTATEAXISSPEED"]);
                            taskCmd.ForkAxisSpeed = Convert.ToInt32(row["FORKAXISSPEED"]);
                            taskCmd.LotID = Convert.ToString(row["LOTID"]);
                            taskCmd.EmptyCST = Convert.ToString(row["EMPTYCST"]);
                            taskCmd.CSTType = Convert.ToString(row["CSTTYPE"]);
                            taskCmd.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                            taskCmd.BCRReadDT = Convert.ToString(row["BCRREADDT"]);
                            taskCmd.BCRReplyCSTID = Convert.ToString(row["BCRREPLYCSTID"]);
                            taskCmd.BCRReadStatus = Convert.ToInt32(row["BCRREADSTATUS"]);
                            taskCmd.Priority = Convert.ToInt32(row["PRIORITY"]);
                            taskCmd.QueueDT = Convert.ToString(row["QUEUEDT"]);
                            taskCmd.InitialDT = Convert.ToString(row["INITIALDT"]);
                            taskCmd.WaitingDT = Convert.ToString(row["WAITINGDT"]);
                            taskCmd.ActiveDT = Convert.ToString(row["ACTIVEDT"]);
                            taskCmd.FinishDT = Convert.ToString(row["FINISHDT"]);
                            taskCmd.FinishLocation = Convert.ToString(row["FINISHLOCATION"]);
                            taskCmd.C1StartDT = Convert.ToString(row["C1STARTDT"]);
                            taskCmd.CSTOnDT = Convert.ToString(row["CSTONDT"]);
                            taskCmd.CSTTakeOffDT = Convert.ToString(row["CSTTAKEOFFDT"]);
                            taskCmd.C2StartDT = Convert.ToString(row["C2STARTDT"]);
                            taskCmd.T1 = Convert.ToInt32(row["T1"]);
                            taskCmd.T2 = Convert.ToInt32(row["T2"]);
                            taskCmd.T3 = Convert.ToInt32(row["T3"]);
                            taskCmd.T4 = Convert.ToInt32(row["T4"]);
                            taskCmd.F1StartDT = Convert.ToString(row["F1STARTDT"]);
                            taskCmd.F2StartDT = Convert.ToString(row["F2STARTDT"]);
                            taskCmd.NextDest = Convert.ToInt32(row["NEXTDEST"]);
                            lstShelfDef.Add(taskCmd);
                        }
                        return lstShelfDef;
                    }
                    else
                        return new List<TaskCmd>();
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                    return new List<TaskCmd>();
                }
                finally
                {
                    table?.Clear();
                    table?.Dispose();
                }
            }
        }
        public bool GetTask(DB _db, string commandID, string taskNo, int craneNo, out TaskCmd task)
        {
            DataTable table = new DataTable();
            task = null;
            try
            {
                string SQL = "SELECT * FROM TASK";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND COMMANDID ='{commandID}'";
                SQL += $" AND TASKNO ='{taskNo}'";
                SQL += $" AND CRANENO ='{craneNo}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    task = new TaskCmd();
                    task.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    task.TaskNo = Convert.ToString(table.Rows[0]["TASKNO"]);
                    task.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                    task.ForkNumber = Convert.ToInt32(table.Rows[0]["FORKNUMBER"]);
                    task.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    task.TaskState = Convert.ToInt32(table.Rows[0]["TASKSTATE"]);
                    task.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                    task.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                    task.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                    task.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                    task.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                    task.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                    task.AtSourceDT = Convert.ToString(table.Rows[0]["ATSOURCEDT"]);
                    task.AtDestinationDT = Convert.ToString(table.Rows[0]["ATDESTINATIONDT"]);
                    task.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    task.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    task.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    task.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                    task.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                    task.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                    task.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                    task.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]) == "Y";
                    task.BCRReadDT = Convert.ToString(table.Rows[0]["BCRREADDT"]);
                    task.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                    task.BCRReadStatus = Convert.ToInt32(table.Rows[0]["BCRREADSTATUS"]);
                    task.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                    task.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                    task.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                    task.WaitingDT = Convert.ToString(table.Rows[0]["WAITINGDT"]);
                    task.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                    task.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                    task.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                    task.C1StartDT = Convert.ToString(table.Rows[0]["C1STARTDT"]);
                    task.CSTOnDT = Convert.ToString(table.Rows[0]["CSTONDT"]);
                    task.CSTTakeOffDT = Convert.ToString(table.Rows[0]["CSTTAKEOFFDT"]);
                    task.C2StartDT = Convert.ToString(table.Rows[0]["C2STARTDT"]);
                    task.T1 = Convert.ToInt32(table.Rows[0]["T1"]);
                    task.T2 = Convert.ToInt32(table.Rows[0]["T2"]);
                    task.T3 = Convert.ToInt32(table.Rows[0]["T3"]);
                    task.T4 = Convert.ToInt32(table.Rows[0]["T4"]);
                    task.F1StartDT = Convert.ToString(table.Rows[0]["F1STARTDT"]);
                    task.F2StartDT = Convert.ToString(table.Rows[0]["F2STARTDT"]);
                    task.NextDest = Convert.ToInt32(table.Rows[0]["NEXTDEST"]);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public bool GetTask(DB _db, string commandID, string taskNo, out TaskCmd task)
        {
            DataTable table = new DataTable();
            task = null;
            try
            {
                string SQL = "SELECT * FROM TASK";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND COMMANDID ='{commandID}'";
                SQL += $" AND TASKNO ='{taskNo}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    task = new TaskCmd();
                    task.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    task.TaskNo = Convert.ToString(table.Rows[0]["TASKNO"]);
                    task.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                    task.ForkNumber = Convert.ToInt32(table.Rows[0]["FORKNUMBER"]);
                    task.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    task.TaskState = Convert.ToInt32(table.Rows[0]["TASKSTATE"]);
                    task.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                    task.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                    task.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                    task.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                    task.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                    task.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                    task.AtSourceDT = Convert.ToString(table.Rows[0]["ATSOURCEDT"]);
                    task.AtDestinationDT = Convert.ToString(table.Rows[0]["ATDESTINATIONDT"]);
                    task.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    task.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    task.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    task.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                    task.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                    task.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                    task.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                    task.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]) == "Y";
                    task.BCRReadDT = Convert.ToString(table.Rows[0]["BCRREADDT"]);
                    task.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                    task.BCRReadStatus = Convert.ToInt32(table.Rows[0]["BCRREADSTATUS"]);
                    task.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                    task.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                    task.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                    task.WaitingDT = Convert.ToString(table.Rows[0]["WAITINGDT"]);
                    task.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                    task.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                    task.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                    task.C1StartDT = Convert.ToString(table.Rows[0]["C1STARTDT"]);
                    task.CSTOnDT = Convert.ToString(table.Rows[0]["CSTONDT"]);
                    task.CSTTakeOffDT = Convert.ToString(table.Rows[0]["CSTTAKEOFFDT"]);
                    task.C2StartDT = Convert.ToString(table.Rows[0]["C2STARTDT"]);
                    task.T1 = Convert.ToInt32(table.Rows[0]["T1"]);
                    task.T2 = Convert.ToInt32(table.Rows[0]["T2"]);
                    task.T3 = Convert.ToInt32(table.Rows[0]["T3"]);
                    task.T4 = Convert.ToInt32(table.Rows[0]["T4"]);
                    task.F1StartDT = Convert.ToString(table.Rows[0]["F1STARTDT"]);
                    task.F2StartDT = Convert.ToString(table.Rows[0]["F2STARTDT"]);
                    task.NextDest = Convert.ToInt32(table.Rows[0]["NEXTDEST"]);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public bool GetTask(DB _db, int craneNo, string taskNo, out TaskCmd task)
        {
            DataTable table = new DataTable();
            task = null;
            try
            {
                string SQL = "SELECT * FROM TASK";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TASKNO ='{taskNo}'";
                SQL += $" AND CRANENO ='{craneNo}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    task = new TaskCmd();
                    task.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    task.TaskNo = Convert.ToString(table.Rows[0]["TASKNO"]);
                    task.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                    task.ForkNumber = Convert.ToInt32(table.Rows[0]["FORKNUMBER"]);
                    task.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    task.TaskState = Convert.ToInt32(table.Rows[0]["TASKSTATE"]);
                    task.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                    task.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                    task.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                    task.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                    task.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                    task.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                    task.AtSourceDT = Convert.ToString(table.Rows[0]["ATSOURCEDT"]);
                    task.AtDestinationDT = Convert.ToString(table.Rows[0]["ATDESTINATIONDT"]);
                    task.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    task.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    task.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    task.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                    task.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                    task.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                    task.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                    task.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]) == "Y";
                    task.BCRReadDT = Convert.ToString(table.Rows[0]["BCRREADDT"]);
                    task.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                    task.BCRReadStatus = Convert.ToInt32(table.Rows[0]["BCRREADSTATUS"]);
                    task.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                    task.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                    task.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                    task.WaitingDT = Convert.ToString(table.Rows[0]["WAITINGDT"]);
                    task.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                    task.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                    task.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                    task.C1StartDT = Convert.ToString(table.Rows[0]["C1STARTDT"]);
                    task.CSTOnDT = Convert.ToString(table.Rows[0]["CSTONDT"]);
                    task.CSTTakeOffDT = Convert.ToString(table.Rows[0]["CSTTAKEOFFDT"]);
                    task.C2StartDT = Convert.ToString(table.Rows[0]["C2STARTDT"]);
                    task.T1 = Convert.ToInt32(table.Rows[0]["T1"]);
                    task.T2 = Convert.ToInt32(table.Rows[0]["T2"]);
                    task.T3 = Convert.ToInt32(table.Rows[0]["T3"]);
                    task.T4 = Convert.ToInt32(table.Rows[0]["T4"]);
                    task.F1StartDT = Convert.ToString(table.Rows[0]["F1STARTDT"]);
                    task.F2StartDT = Convert.ToString(table.Rows[0]["F2STARTDT"]);
                    task.NextDest = Convert.ToInt32(table.Rows[0]["NEXTDEST"]);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public TaskCmd GetTask(DB _db, string commandID, string taskNo, int craneNo)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TASK";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND COMMANDID ='{commandID}'";
                SQL += $" AND TASKNO ='{taskNo}'";
                SQL += $" AND CRANENO ='{craneNo}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    TaskCmd taskCmd = new TaskCmd();
                    taskCmd.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    taskCmd.TaskNo = Convert.ToString(table.Rows[0]["TASKNO"]);
                    taskCmd.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                    taskCmd.ForkNumber = Convert.ToInt32(table.Rows[0]["FORKNUMBER"]);
                    taskCmd.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    taskCmd.TaskState = Convert.ToInt32(table.Rows[0]["TASKSTATE"]);
                    taskCmd.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                    taskCmd.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                    taskCmd.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                    taskCmd.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                    taskCmd.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                    taskCmd.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                    taskCmd.AtSourceDT = Convert.ToString(table.Rows[0]["ATSOURCEDT"]);
                    taskCmd.AtDestinationDT = Convert.ToString(table.Rows[0]["ATDESTINATIONDT"]);
                    taskCmd.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    taskCmd.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    taskCmd.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    taskCmd.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                    taskCmd.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                    taskCmd.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                    taskCmd.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                    taskCmd.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]) == "Y";
                    taskCmd.BCRReadDT = Convert.ToString(table.Rows[0]["BCRREADDT"]);
                    taskCmd.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                    taskCmd.BCRReadStatus = Convert.ToInt32(table.Rows[0]["BCRREADSTATUS"]);
                    taskCmd.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                    taskCmd.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                    taskCmd.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                    taskCmd.WaitingDT = Convert.ToString(table.Rows[0]["WAITINGDT"]);
                    taskCmd.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                    taskCmd.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                    taskCmd.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                    taskCmd.C1StartDT = Convert.ToString(table.Rows[0]["C1STARTDT"]);
                    taskCmd.CSTOnDT = Convert.ToString(table.Rows[0]["CSTONDT"]);
                    taskCmd.CSTTakeOffDT = Convert.ToString(table.Rows[0]["CSTTAKEOFFDT"]);
                    taskCmd.C2StartDT = Convert.ToString(table.Rows[0]["C2STARTDT"]);
                    taskCmd.T1 = Convert.ToInt32(table.Rows[0]["T1"]);
                    taskCmd.T2 = Convert.ToInt32(table.Rows[0]["T2"]);
                    taskCmd.T3 = Convert.ToInt32(table.Rows[0]["T3"]);
                    taskCmd.T4 = Convert.ToInt32(table.Rows[0]["T4"]);
                    taskCmd.F1StartDT = Convert.ToString(table.Rows[0]["F1STARTDT"]);
                    taskCmd.F2StartDT = Convert.ToString(table.Rows[0]["F2STARTDT"]);
                    taskCmd.NextDest = Convert.ToInt32(table.Rows[0]["NEXTDEST"]);
                    return taskCmd;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return null;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public TaskCmd GetTask(DB _db, string commandID, string taskNo)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TASK";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND COMMANDID ='{commandID}'";
                SQL += $" AND TASKNO ='{taskNo}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    TaskCmd taskCmd = new TaskCmd();
                    taskCmd.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    taskCmd.TaskNo = Convert.ToString(table.Rows[0]["TASKNO"]);
                    taskCmd.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                    taskCmd.ForkNumber = Convert.ToInt32(table.Rows[0]["FORKNUMBER"]);
                    taskCmd.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    taskCmd.TaskState = Convert.ToInt32(table.Rows[0]["TASKSTATE"]);
                    taskCmd.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                    taskCmd.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                    taskCmd.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                    taskCmd.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                    taskCmd.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                    taskCmd.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                    taskCmd.AtSourceDT = Convert.ToString(table.Rows[0]["ATSOURCEDT"]);
                    taskCmd.AtDestinationDT = Convert.ToString(table.Rows[0]["ATDESTINATIONDT"]);
                    taskCmd.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    taskCmd.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    taskCmd.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    taskCmd.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                    taskCmd.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                    taskCmd.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                    taskCmd.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                    taskCmd.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]) == "Y";
                    taskCmd.BCRReadDT = Convert.ToString(table.Rows[0]["BCRREADDT"]);
                    taskCmd.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                    taskCmd.BCRReadStatus = Convert.ToInt32(table.Rows[0]["BCRREADSTATUS"]);
                    taskCmd.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                    taskCmd.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                    taskCmd.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                    taskCmd.WaitingDT = Convert.ToString(table.Rows[0]["WAITINGDT"]);
                    taskCmd.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                    taskCmd.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                    taskCmd.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                    taskCmd.C1StartDT = Convert.ToString(table.Rows[0]["C1STARTDT"]);
                    taskCmd.CSTOnDT = Convert.ToString(table.Rows[0]["CSTONDT"]);
                    taskCmd.CSTTakeOffDT = Convert.ToString(table.Rows[0]["CSTTAKEOFFDT"]);
                    taskCmd.C2StartDT = Convert.ToString(table.Rows[0]["C2STARTDT"]);
                    taskCmd.T1 = Convert.ToInt32(table.Rows[0]["T1"]);
                    taskCmd.T2 = Convert.ToInt32(table.Rows[0]["T2"]);
                    taskCmd.T3 = Convert.ToInt32(table.Rows[0]["T3"]);
                    taskCmd.T4 = Convert.ToInt32(table.Rows[0]["T4"]);
                    taskCmd.F1StartDT = Convert.ToString(table.Rows[0]["F1STARTDT"]);
                    taskCmd.F2StartDT = Convert.ToString(table.Rows[0]["F2STARTDT"]);
                    taskCmd.NextDest = Convert.ToInt32(table.Rows[0]["NEXTDEST"]);
                    return taskCmd;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return null;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public TaskCmd GetTask(DB _db, string taskNo)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TASK";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TASKNO ='{taskNo}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    TaskCmd taskCmd = new TaskCmd();
                    taskCmd.CommandID = Convert.ToString(table.Rows[0]["COMMANDID"]);
                    taskCmd.TaskNo = Convert.ToString(table.Rows[0]["TASKNO"]);
                    taskCmd.CraneNo = Convert.ToInt32(table.Rows[0]["CRANENO"]);
                    taskCmd.ForkNumber = Convert.ToInt32(table.Rows[0]["FORKNUMBER"]);
                    taskCmd.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    taskCmd.TaskState = Convert.ToInt32(table.Rows[0]["TASKSTATE"]);
                    taskCmd.CompleteCode = Convert.ToString(table.Rows[0]["COMPLETECODE"]);
                    taskCmd.TransferMode = Convert.ToInt32(table.Rows[0]["TRANSFERMODE"]);
                    taskCmd.Source = Convert.ToString(table.Rows[0]["SOURCE"]);
                    taskCmd.SourceBay = Convert.ToInt32(table.Rows[0]["SOURCEBAY"]);
                    taskCmd.Destination = Convert.ToString(table.Rows[0]["DESTINATION"]);
                    taskCmd.DestinationBay = Convert.ToInt32(table.Rows[0]["DESTINATIONBAY"]);
                    taskCmd.AtSourceDT = Convert.ToString(table.Rows[0]["ATSOURCEDT"]);
                    taskCmd.AtDestinationDT = Convert.ToString(table.Rows[0]["ATDESTINATIONDT"]);
                    taskCmd.TravelAxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    taskCmd.LifterAxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    taskCmd.RotateAxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    taskCmd.ForkAxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);
                    taskCmd.LotID = Convert.ToString(table.Rows[0]["LOTID"]);
                    taskCmd.EmptyCST = Convert.ToString(table.Rows[0]["EMPTYCST"]);
                    taskCmd.CSTType = Convert.ToString(table.Rows[0]["CSTTYPE"]);
                    taskCmd.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRREADFLAG"]) == "Y";
                    taskCmd.BCRReadDT = Convert.ToString(table.Rows[0]["BCRREADDT"]);
                    taskCmd.BCRReplyCSTID = Convert.ToString(table.Rows[0]["BCRREPLYCSTID"]);
                    taskCmd.BCRReadStatus = Convert.ToInt32(table.Rows[0]["BCRREADSTATUS"]);
                    taskCmd.Priority = Convert.ToInt32(table.Rows[0]["PRIORITY"]);
                    taskCmd.QueueDT = Convert.ToString(table.Rows[0]["QUEUEDT"]);
                    taskCmd.InitialDT = Convert.ToString(table.Rows[0]["INITIALDT"]);
                    taskCmd.WaitingDT = Convert.ToString(table.Rows[0]["WAITINGDT"]);
                    taskCmd.ActiveDT = Convert.ToString(table.Rows[0]["ACTIVEDT"]);
                    taskCmd.FinishDT = Convert.ToString(table.Rows[0]["FINISHDT"]);
                    taskCmd.FinishLocation = Convert.ToString(table.Rows[0]["FINISHLOCATION"]);
                    taskCmd.C1StartDT = Convert.ToString(table.Rows[0]["C1STARTDT"]);
                    taskCmd.CSTOnDT = Convert.ToString(table.Rows[0]["CSTONDT"]);
                    taskCmd.CSTTakeOffDT = Convert.ToString(table.Rows[0]["CSTTAKEOFFDT"]);
                    taskCmd.C2StartDT = Convert.ToString(table.Rows[0]["C2STARTDT"]);
                    taskCmd.T1 = Convert.ToInt32(table.Rows[0]["T1"]);
                    taskCmd.T2 = Convert.ToInt32(table.Rows[0]["T2"]);
                    taskCmd.T3 = Convert.ToInt32(table.Rows[0]["T3"]);
                    taskCmd.T4 = Convert.ToInt32(table.Rows[0]["T4"]);
                    taskCmd.F1StartDT = Convert.ToString(table.Rows[0]["F1STARTDT"]);
                    taskCmd.F2StartDT = Convert.ToString(table.Rows[0]["F2STARTDT"]);
                    taskCmd.NextDest = Convert.ToInt32(table.Rows[0]["NEXTDEST"]);
                    return taskCmd;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return null;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public IEnumerable<TaskCmd> GetAllTask(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TASK";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<TaskCmd> lstShelfDef = new List<TaskCmd>();
                    foreach (DataRow row in table.Rows)
                    {
                        TaskCmd taskCmd = new TaskCmd();
                        taskCmd.CommandID = Convert.ToString(row["COMMANDID"]);
                        taskCmd.TaskNo = Convert.ToString(row["TASKNO"]);
                        taskCmd.CraneNo = Convert.ToInt32(row["CRANENO"]);
                        taskCmd.ForkNumber = Convert.ToInt32(row["FORKNUMBER"]);
                        taskCmd.CSTID = Convert.ToString(row["CSTID"]);
                        taskCmd.TaskState = Convert.ToInt32(row["TASKSTATE"]);
                        taskCmd.CompleteCode = Convert.ToString(row["COMPLETECODE"]);
                        taskCmd.TransferMode = Convert.ToInt32(row["TRANSFERMODE"]);
                        taskCmd.Source = Convert.ToString(row["SOURCE"]);
                        taskCmd.SourceBay = Convert.ToInt32(row["SOURCEBAY"]);
                        taskCmd.Destination = Convert.ToString(row["DESTINATION"]);
                        taskCmd.DestinationBay = Convert.ToInt32(row["DESTINATIONBAY"]);
                        taskCmd.AtSourceDT = Convert.ToString(row["ATSOURCEDT"]);
                        taskCmd.AtDestinationDT = Convert.ToString(row["ATDESTINATIONDT"]);
                        taskCmd.TravelAxisSpeed = Convert.ToInt32(row["TRAVELAXISSPEED"]);
                        taskCmd.LifterAxisSpeed = Convert.ToInt32(row["LIFTERAXISSPEED"]);
                        taskCmd.RotateAxisSpeed = Convert.ToInt32(row["ROTATEAXISSPEED"]);
                        taskCmd.ForkAxisSpeed = Convert.ToInt32(row["FORKAXISSPEED"]);
                        taskCmd.LotID = Convert.ToString(row["LOTID"]);
                        taskCmd.EmptyCST = Convert.ToString(row["EMPTYCST"]);
                        taskCmd.CSTType = Convert.ToString(row["CSTTYPE"]);
                        taskCmd.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                        taskCmd.BCRReadDT = Convert.ToString(row["BCRREADDT"]);
                        taskCmd.BCRReplyCSTID = Convert.ToString(row["BCRREPLYCSTID"]);
                        taskCmd.BCRReadStatus = Convert.ToInt32(row["BCRREADSTATUS"]);
                        taskCmd.Priority = Convert.ToInt32(row["PRIORITY"]);
                        taskCmd.QueueDT = Convert.ToString(row["QUEUEDT"]);
                        taskCmd.InitialDT = Convert.ToString(row["INITIALDT"]);
                        taskCmd.WaitingDT = Convert.ToString(row["WAITINGDT"]);
                        taskCmd.ActiveDT = Convert.ToString(row["ACTIVEDT"]);
                        taskCmd.FinishDT = Convert.ToString(row["FINISHDT"]);
                        taskCmd.FinishLocation = Convert.ToString(row["FINISHLOCATION"]);
                        taskCmd.C1StartDT = Convert.ToString(row["C1STARTDT"]);
                        taskCmd.CSTOnDT = Convert.ToString(row["CSTONDT"]);
                        taskCmd.CSTTakeOffDT = Convert.ToString(row["CSTTAKEOFFDT"]);
                        taskCmd.C2StartDT = Convert.ToString(row["C2STARTDT"]);
                        taskCmd.T1 = Convert.ToInt32(row["T1"]);
                        taskCmd.T2 = Convert.ToInt32(row["T2"]);
                        taskCmd.T3 = Convert.ToInt32(row["T3"]);
                        taskCmd.T4 = Convert.ToInt32(row["T4"]);
                        taskCmd.F1StartDT = Convert.ToString(row["F1STARTDT"]);
                        taskCmd.F2StartDT = Convert.ToString(row["F2STARTDT"]);
                        taskCmd.NextDest = Convert.ToInt32(row["NEXTDEST"]);
                        lstShelfDef.Add(taskCmd);
                    }
                    return lstShelfDef;
                }
                else
                    return new List<TaskCmd>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<TaskCmd>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public int InsertTaskToHistory(DB _db, string commandID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "INSERT INTO HISTASK";
            SQL += $" SELECT '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', * FROM TASK";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID ='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }

        public int DeleteTask(DB _db, string commandID)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "DELETE FROM TASK";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID ='{commandID}'";
            return _db.ExecuteSQL(SQL);
        }
        public int DeleteTask(DB _db, string commandID, string taskNo)
        {
            if (string.IsNullOrWhiteSpace(commandID))
                return ErrorCode.Exception;

            string SQL = "DELETE FROM TASK";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{commandID}'";
            SQL += $" AND TASKNO<>'{commandID}'";
            return _db.ExecuteSQL(SQL);
        }
        public int UpdateTaskState(DB _db, UpdateCommandInfo info)
        {
            if (info == null)
                return ErrorCode.Exception;

            string SQL = "UPDATE TASK SET";
            SQL += $" TASKSTATE='{(int)TaskState.UpdateOK}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{info.CommandID}'";
            SQL += $" AND TASKNO='{info.TaskNo}'";
            return _db.ExecuteSQL(SQL);
        }

        public int UpdateTaskStateAllTask(DB _db, UpdateCommandInfo info)
        {
            if (info == null)
                return ErrorCode.Exception;

            string SQL = "UPDATE TASK SET";
            SQL += $" TASKSTATE='{(int)TaskState.UpdateOK}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND COMMANDID='{info.CommandID}'";
            return _db.ExecuteSQL(SQL);
        }
        public IEnumerable<UpdateCommandInfo> GetUpdateCommandInfo(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT TRANSFERCMD.COMMANDID, TRANSFERCMD.TRANSFERMODE, TRANSFERCMD.TRANSFERSTATE, TRANSFERCMD.BCRREADFLAG,";
                SQL += " TRANSFERCMD.CSTID, TRANSFERCMD.EMPTYCST, TRANSFERCMD.CSTTYPE, TRANSFERCMD.LOTID, TRANSFERCMD.HOSTPRIORITY,";
                SQL += " TRANSFERCMD.HOSTSOURCE, TRANSFERCMD.SOURCE, TRANSFERCMD.HOSTDESTINATION, TRANSFERCMD.DESTINATION,";
                SQL += " TRANSFERCMD.NEXTDEST, TRANSFERCMD.CURRENTPOSITION, TRANSFERCMD.FINISHLOCATION,";
                SQL += " TRANSFERCMD.ABORTFLAG, TRANSFERCMD.CANCELFLAG, TRANSFERCMD.RETRYFLAG,";
                SQL += " TASK.TASKNO, TASK.TASKSTATE, TASK.CRANENO, TASK.FORKNUMBER, TASK.SOURCE TASK_SOURCE, TASK.DESTINATION TASK_DESTINATION,";
                SQL += " TASK.COMPLETECODE TASK_COMPLETECODE, TASK.BCRREPLYCSTID TASK_BCRREPLYCSTID, TASK.FINISHLOCATION TASK_FINISHLOCATION,";
                SQL += " TASK.QUEUEDT TASK_QUEUEDT, TASK.INITIALDT TASK_INITIALDT, TASK.ACTIVEDT TASK_ACTIVEDT,";
                SQL += " TASK.CSTONDT TASK_CSTONDT, TASK.CSTTAKEOFFDT TASK_CSTTAKEOFFDT, TASK.FINISHDT TASK_FINISHDT";
                SQL += " FROM TASK LEFT JOIN TRANSFERCMD ON TASK.COMMANDID=TRANSFERCMD.COMMANDID";
                SQL += $" AND TASK.TASKSTATE='{(int)TaskState.Complete}'";
                //SQL += " AND (DATEDIFF(MINUTE, CONVERT(DATETIME, TASK.FINISHDT), GETDATE()) > 1 OR DATEDIFF(SECOND, CONVERT(DATETIME, TASK.FINISHDT), GETDATE()) > 1)";
                SQL += $" WHERE TRANSFERCMD.STOCKERID='{_stockerId}'";
                SQL += " ORDER BY TASK.TASKNO";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<UpdateCommandInfo> lstShelfDef = new List<UpdateCommandInfo>();
                    foreach (DataRow row in table.Rows)
                    {
                        UpdateCommandInfo commandInfo = new UpdateCommandInfo();
                        commandInfo.CommandID = Convert.ToString(row["COMMANDID"]);
                        commandInfo.TransferMode = Convert.ToInt32(row["TRANSFERMODE"]);
                        commandInfo.TransferState = Convert.ToInt32(row["TRANSFERSTATE"]);
                        commandInfo.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                        commandInfo.CarrierID = Convert.ToString(row["CSTID"]);
                        commandInfo.EmptyCST = Convert.ToString(row["EMPTYCST"]);
                        commandInfo.CSTType = Convert.ToString(row["CSTTYPE"]);
                        commandInfo.LotID = Convert.ToString(row["LOTID"]);
                        commandInfo.HostPriority = Convert.ToInt32(row["HOSTPRIORITY"]);
                        commandInfo.HostSource = Convert.ToString(row["HOSTSOURCE"]);
                        commandInfo.Source = Convert.ToString(row["SOURCE"]);
                        commandInfo.HostDestination = Convert.ToString(row["HOSTDESTINATION"]);
                        commandInfo.Destination = Convert.ToString(row["DESTINATION"]);
                        commandInfo.NextDest = Convert.ToString(row["NEXTDEST"]);
                        commandInfo.CurrentPosition = Convert.ToString(row["CURRENTPOSITION"]);
                        commandInfo.FinishLocation = Convert.ToString(row["FINISHLOCATION"]);

                        commandInfo.TaskNo = Convert.ToString(row["TaskNo"]);
                        commandInfo.TaskState = Convert.ToInt32(row["TaskState"]);
                        commandInfo.Task_CraneNo = Convert.ToInt32(row["CraneNo"]);
                        commandInfo.Task_ForkNumber = Convert.ToInt32(row["FORKNUMBER"]);
                        commandInfo.Task_Source = Convert.ToString(row["Task_Source"]);
                        commandInfo.Task_Destination = Convert.ToString(row["Task_Destination"]);
                        commandInfo.Task_CompleteCode = Convert.ToString(row["Task_CompleteCode"]);
                        commandInfo.Task_BCRReplyCSTID = Convert.ToString(row["Task_BCRReplyCSTID"]);
                        commandInfo.Task_FinishLocation = Convert.ToString(row["Task_FinishLocation"]);

                        commandInfo.Task_QueueDT = Convert.ToString(row["Task_QueueDT"]);
                        commandInfo.Task_InitialDT = Convert.ToString(row["Task_InitialDT"]);
                        commandInfo.Task_ActiveDT = Convert.ToString(row["Task_ActiveDT"]);
                        commandInfo.Task_CSTOnDT = Convert.ToString(row["Task_CSTOnDT"]);
                        commandInfo.Task_CSTTakeOffDT = Convert.ToString(row["Task_CSTTakeOffDT"]);
                        commandInfo.Task_FinishDT = Convert.ToString(row["Task_FinishDT"]);

                        commandInfo.AbortFlag = Convert.ToString(row["ABORTFLAG"]) == "Y";
                        commandInfo.CancelFlag = Convert.ToString(row["CANCELFLAG"]) == "Y";
                        commandInfo.RetryFlag = Convert.ToString(row["RETRYFLAG"]) == "Y";
                        lstShelfDef.Add(commandInfo);
                    }
                    return lstShelfDef;
                }
                else
                    return new List<UpdateCommandInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<UpdateCommandInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<UpdateCommandInfo> GetUpdateCommandInfoByCancel(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERSTATE='{(int)TransferState.Queue}'";
                SQL += $" AND CANCELFLAG='Y'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<UpdateCommandInfo> lstShelfDef = new List<UpdateCommandInfo>();
                    foreach (DataRow row in table.Rows)
                    {
                        UpdateCommandInfo commandInfo = new UpdateCommandInfo();
                        commandInfo.CommandID = Convert.ToString(row["COMMANDID"]);
                        commandInfo.TransferMode = Convert.ToInt32(row["TRANSFERMODE"]);
                        commandInfo.TransferState = Convert.ToInt32(row["TRANSFERSTATE"]);
                        commandInfo.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                        commandInfo.CarrierID = Convert.ToString(row["CSTID"]);
                        commandInfo.EmptyCST = Convert.ToString(row["EMPTYCST"]);
                        commandInfo.CSTType = Convert.ToString(row["CSTTYPE"]);
                        commandInfo.LotID = Convert.ToString(row["LOTID"]);
                        commandInfo.HostPriority = Convert.ToInt32(row["HOSTPRIORITY"]);
                        commandInfo.HostSource = Convert.ToString(row["HOSTSOURCE"]);
                        commandInfo.Source = Convert.ToString(row["SOURCE"]);
                        commandInfo.HostDestination = Convert.ToString(row["HOSTDESTINATION"]);
                        commandInfo.Destination = Convert.ToString(row["DESTINATION"]);
                        commandInfo.NextDest = Convert.ToString(row["NEXTDEST"]);
                        commandInfo.CurrentPosition = Convert.ToString(row["CURRENTPOSITION"]);
                        commandInfo.FinishLocation = Convert.ToString(row["FINISHLOCATION"]);
                        lstShelfDef.Add(commandInfo);
                    }
                    return lstShelfDef;
                }
                else
                    return new List<UpdateCommandInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<UpdateCommandInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<UpdateCommandInfo> GetUpdateCommandInfoByAbort(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = $"SELECT DISTINCT TRANSFERCMD.COMMANDID, TRANSFERCMD.* FROM TRANSFERCMD " +
                    $"WHERE TRANSFERCMD.STOCKERID='{_stockerId}' AND TRANSFERCMD.ABORTFLAG='Y'" +
                    $"AND COMPLETECODE != 'EC' and COMPLETECODE !='E2'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<UpdateCommandInfo> lstShelfDef = new List<UpdateCommandInfo>();
                    foreach (DataRow row in table.Rows)
                    {
                        UpdateCommandInfo commandInfo = new UpdateCommandInfo();
                        commandInfo.CommandID = Convert.ToString(row["COMMANDID"]);
                        commandInfo.TransferMode = Convert.ToInt32(row["TRANSFERMODE"]);
                        commandInfo.TransferState = Convert.ToInt32(row["TRANSFERSTATE"]);
                        commandInfo.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                        commandInfo.CarrierID = Convert.ToString(row["CSTID"]);
                        commandInfo.EmptyCST = Convert.ToString(row["EMPTYCST"]);
                        commandInfo.CSTType = Convert.ToString(row["CSTTYPE"]);
                        commandInfo.LotID = Convert.ToString(row["LOTID"]);
                        commandInfo.HostPriority = Convert.ToInt32(row["HOSTPRIORITY"]);
                        commandInfo.HostSource = Convert.ToString(row["HOSTSOURCE"]);
                        commandInfo.Source = Convert.ToString(row["SOURCE"]);
                        commandInfo.HostDestination = Convert.ToString(row["HOSTDESTINATION"]);
                        commandInfo.Destination = Convert.ToString(row["DESTINATION"]);
                        commandInfo.NextDest = Convert.ToString(row["NEXTDEST"]);
                        commandInfo.CurrentPosition = Convert.ToString(row["CURRENTPOSITION"]);
                        commandInfo.FinishLocation = Convert.ToString(row["FINISHLOCATION"]);
                        commandInfo.Task_CompleteCode = Convert.ToString(row["CompleteCode"]);
                        lstShelfDef.Add(commandInfo);
                    }
                    return lstShelfDef;
                }
                else
                    return new List<UpdateCommandInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<UpdateCommandInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<UpdateCommandInfo> GetUpdateCommandInfoByDelete(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM TRANSFERCMD";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND DELETEFLAG='{(char)Enable.Enable}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<UpdateCommandInfo> lstShelfDef = new List<UpdateCommandInfo>();
                    foreach (DataRow row in table.Rows)
                    {
                        UpdateCommandInfo commandInfo = new UpdateCommandInfo();
                        commandInfo.CommandID = Convert.ToString(row["COMMANDID"]);
                        commandInfo.TransferMode = Convert.ToInt32(row["TRANSFERMODE"]);
                        commandInfo.TransferState = Convert.ToInt32(row["TRANSFERSTATE"]);
                        commandInfo.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                        commandInfo.CarrierID = Convert.ToString(row["CSTID"]);
                        commandInfo.EmptyCST = Convert.ToString(row["EMPTYCST"]);
                        commandInfo.CSTType = Convert.ToString(row["CSTTYPE"]);
                        commandInfo.LotID = Convert.ToString(row["LOTID"]);
                        commandInfo.HostPriority = Convert.ToInt32(row["HOSTPRIORITY"]);
                        commandInfo.HostSource = Convert.ToString(row["HOSTSOURCE"]);
                        commandInfo.Source = Convert.ToString(row["SOURCE"]);
                        commandInfo.HostDestination = Convert.ToString(row["HOSTDESTINATION"]);
                        commandInfo.Destination = Convert.ToString(row["DESTINATION"]);
                        commandInfo.NextDest = Convert.ToString(row["NEXTDEST"]);
                        commandInfo.CurrentPosition = Convert.ToString(row["CURRENTPOSITION"]);
                        commandInfo.FinishLocation = Convert.ToString(row["FINISHLOCATION"]);
                        lstShelfDef.Add(commandInfo);
                    }
                    return lstShelfDef;
                }
                else
                    return new List<UpdateCommandInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<UpdateCommandInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public UpdateCommandInfo GetCommandInfoByCommandID(DB _db, string commandID)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT TRANSFERCMD.COMMANDID, TRANSFERCMD.TRANSFERMODE, TRANSFERCMD.TRANSFERSTATE, TRANSFERCMD.BCRREADFLAG,";
                SQL += " TRANSFERCMD.CSTID, TRANSFERCMD.EMPTYCST, TRANSFERCMD.CSTTYPE, TRANSFERCMD.LOTID, TRANSFERCMD.HOSTPRIORITY,";
                SQL += " TRANSFERCMD.HOSTSOURCE, TRANSFERCMD.SOURCE, TRANSFERCMD.HOSTDESTINATION, TRANSFERCMD.DESTINATION,";
                SQL += " TRANSFERCMD.NEXTDEST, TRANSFERCMD.CURRENTPOSITION, TRANSFERCMD.FINISHLOCATION,";
                SQL += " TRANSFERCMD.ABORTFLAG, TRANSFERCMD.CANCELFLAG, TRANSFERCMD.RETRYFLAG,";
                SQL += " TASK.TASKNO, TASK.TASKSTATE, TASK.CRANENO, TASK.FORKNUMBER, TASK.SOURCE TASK_SOURCE, TASK.DESTINATION TASK_DESTINATION,";
                SQL += " TASK.COMPLETECODE TASK_COMPLETECODE, TASK.BCRREPLYCSTID TASK_BCRREPLYCSTID, TASK.FINISHLOCATION TASK_FINISHLOCATION,";
                SQL += " TASK.QUEUEDT TASK_QUEUEDT, TASK.INITIALDT TASK_INITIALDT, TASK.ACTIVEDT TASK_ACTIVEDT,";
                SQL += " TASK.CSTONDT TASK_CSTONDT, TASK.CSTTAKEOFFDT TASK_CSTTAKEOFFDT, TASK.FINISHDT TASK_FINISHDT";
                SQL += " FROM TASK LEFT JOIN TRANSFERCMD ON TASK.COMMANDID=TRANSFERCMD.COMMANDID";
                SQL += $" WHERE TRANSFERCMD.STOCKERID='{_stockerId}'";
                SQL += $" AND TRANSFERCMD.COMMANDID='{commandID}'";
                SQL += " ORDER BY TASK.TASKNO";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    var row = table.Rows[0];
                    UpdateCommandInfo commandInfo = new UpdateCommandInfo();
                    commandInfo.CommandID = Convert.ToString(row["COMMANDID"]);
                    commandInfo.TransferMode = Convert.ToInt32(row["TRANSFERMODE"]);
                    commandInfo.TransferState = Convert.ToInt32(row["TRANSFERSTATE"]);
                    commandInfo.BCRReadFlag = Convert.ToString(row["BCRREADFLAG"]) == "Y";
                    commandInfo.CarrierID = Convert.ToString(row["CSTID"]);
                    commandInfo.EmptyCST = Convert.ToString(row["EMPTYCST"]);
                    commandInfo.CSTType = Convert.ToString(row["CSTTYPE"]);
                    commandInfo.LotID = Convert.ToString(row["LOTID"]);
                    commandInfo.HostPriority = Convert.ToInt32(row["HOSTPRIORITY"]);
                    commandInfo.HostSource = Convert.ToString(row["HOSTSOURCE"]);
                    commandInfo.Source = Convert.ToString(row["SOURCE"]);
                    commandInfo.HostDestination = Convert.ToString(row["HOSTDESTINATION"]);
                    commandInfo.Destination = Convert.ToString(row["DESTINATION"]);
                    commandInfo.NextDest = Convert.ToString(row["NEXTDEST"]);
                    commandInfo.CurrentPosition = Convert.ToString(row["CURRENTPOSITION"]);
                    commandInfo.FinishLocation = Convert.ToString(row["FINISHLOCATION"]);

                    commandInfo.TaskNo = Convert.ToString(row["TaskNo"]);
                    commandInfo.TaskState = Convert.ToInt32(row["TaskState"]);
                    commandInfo.Task_CraneNo = Convert.ToInt32(row["CraneNo"]);
                    commandInfo.Task_ForkNumber = Convert.ToInt32(row["FORKNUMBER"]);
                    commandInfo.Task_Source = Convert.ToString(row["Task_Source"]);
                    commandInfo.Task_Destination = Convert.ToString(row["Task_Destination"]);
                    commandInfo.Task_CompleteCode = Convert.ToString(row["Task_CompleteCode"]);
                    commandInfo.Task_BCRReplyCSTID = Convert.ToString(row["Task_BCRReplyCSTID"]);
                    commandInfo.Task_FinishLocation = Convert.ToString(row["Task_FinishLocation"]);

                    commandInfo.Task_QueueDT = Convert.ToString(row["Task_QueueDT"]);
                    commandInfo.Task_InitialDT = Convert.ToString(row["Task_InitialDT"]);
                    commandInfo.Task_ActiveDT = Convert.ToString(row["Task_ActiveDT"]);
                    commandInfo.Task_CSTOnDT = Convert.ToString(row["Task_CSTOnDT"]);
                    commandInfo.Task_CSTTakeOffDT = Convert.ToString(row["Task_CSTTakeOffDT"]);
                    commandInfo.Task_FinishDT = Convert.ToString(row["Task_FinishDT"]);

                    commandInfo.AbortFlag = Convert.ToString(row["ABORTFLAG"]) == "Y";
                    commandInfo.CancelFlag = Convert.ToString(row["CANCELFLAG"]) == "Y";
                    commandInfo.RetryFlag = Convert.ToString(row["RETRYFLAG"]) == "Y";
                    return commandInfo;
                }
                else
                    return new UpdateCommandInfo();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new UpdateCommandInfo();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public int InsertFrom(DB _db, CommandTrace trace, string taskNo, int forkNumber)
        {
            string SQL = "INSERT INTO TASK (STOCKERID, COMMANDID, TASKNO, CRANENO, FORKNUMBER, CSTID,";
            SQL += " TRANSFERMODE, TRANSFERMODETYPE, SOURCE, SOURCEBAY, DESTINATION, DESTINATIONBAY,";
            SQL += " TRAVELAXISSPEED, LIFTERAXISSPEED, ROTATEAXISSPEED, FORKAXISSPEED, ";
            SQL += " USERID, BCRREADFLAG, PRIORITY, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{trace.CommandID}', ";
            SQL += $"'{taskNo}', ";
            SQL += $"'{trace.NextCrane}', ";
            SQL += $"'{forkNumber}', ";
            SQL += $"'{trace.CarrierID}', ";
            SQL += $"'{(int)TransferMode.FROM}', ";
            SQL += $"'{(int)TransferModeType.ShelfToRM}', ";
            SQL += $"'{trace.NextSource}', ";
            SQL += $"'{trace.NextSourceBay}', ";
            SQL += $"'{_TaskInfo.GetCraneInfo(trace.NextCrane, forkNumber).CraneShelfID}', ";
            SQL += $"'0', ";
            SQL += $"'{trace.NextCraneSpeed.TravelaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.LifteraxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.RotateaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.ForkaxisSpeed}', ";
            SQL += $"'{trace.UserID}', ";
            SQL += $"'{(trace.NextTransferMode == (int)TransferMode.SCAN ? "Y" : trace.BCRReadFlag ? "Y" : "N")}', ";
            SQL += $"'{trace.MainPriority}', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }

        public int InsertTo(DB _db, CommandTrace trace, string taskNo, int forkNumber, VShelfInfo dest)
        {
            string SQL = "INSERT INTO TASK (STOCKERID, COMMANDID, TASKNO, CRANENO, FORKNUMBER, CSTID,";
            SQL += " TRANSFERMODE, TRANSFERMODETYPE, SOURCE, SOURCEBAY, DESTINATION, DESTINATIONBAY,";
            SQL += " TRAVELAXISSPEED, LIFTERAXISSPEED, ROTATEAXISSPEED, FORKAXISSPEED, ";
            SQL += " USERID, BCRREADFLAG, PRIORITY, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{trace.CommandID}', ";
            SQL += $"'{(taskNo)}', ";
            SQL += $"'{trace.NextCrane}', ";
            SQL += $"'{forkNumber}', ";
            SQL += $"'{trace.CarrierID}', ";
            SQL += $"'{(int)TransferMode.TO}', ";
            SQL += $"'{(int)TransferModeType.RMToShelf}', ";
            SQL += $"'{_TaskInfo.GetCraneInfo(trace.NextCrane, forkNumber).CraneShelfID}', ";
            SQL += $"'0', ";
            SQL += $"'{(dest.ShelfType == (int)ShelfType.Port ? dest.PLCPortID.ToString() : dest.ShelfID)}', ";
            SQL += $"'{Convert.ToInt32(dest.Bay_Y)}', ";
            SQL += $"'{trace.NextCraneSpeed.TravelaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.LifteraxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.RotateaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.ForkaxisSpeed}', ";
            SQL += $"'{trace.UserID}', ";
            SQL += $"'N', ";
            SQL += $"'{trace.MainPriority}', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }

        public int InsertTaskByTwinFork(DB _db, CommandTrace trace, string[] taskNo, VShelfInfo dest, int forkNumber)
        {
            if (trace == null)
                return ErrorCode.Exception;
            //if (dest == null)
            //    return ErrorCode.Exception;
            if (taskNo.Length == 0)
                return ErrorCode.Exception;

            string SQL = string.Empty;
            if (trace.MainTransferMode == (int)TransferMode.FROM_TO || trace.MainTransferMode == (int)TransferMode.FROM || trace.MainTransferMode == (int)TransferMode.SCAN)
            {
                SQL = "INSERT INTO TASK (STOCKERID, COMMANDID, TASKNO, CRANENO, FORKNUMBER, CSTID,";
                SQL += " TRANSFERMODE, TRANSFERMODETYPE, SOURCE, SOURCEBAY, DESTINATION, DESTINATIONBAY,";
                SQL += " TRAVELAXISSPEED, LIFTERAXISSPEED, ROTATEAXISSPEED, FORKAXISSPEED, ";
                SQL += " USERID, BCRREADFLAG, PRIORITY, QUEUEDT) VALUES (";
                SQL += $"'{_stockerId}', ";
                SQL += $"'{trace.CommandID}', ";
                SQL += $"'{taskNo[0]}', ";
                SQL += $"'{trace.NextCrane}', ";
                SQL += $"'{forkNumber}', ";
                SQL += $"'{trace.CarrierID}', ";
                SQL += $"'{(int)TransferMode.FROM}', ";
                SQL += $"'{(int)TransferModeType.ShelfToRM}', ";
                SQL += $"'{trace.NextSource}', ";
                SQL += $"'{trace.NextSourceBay}', ";
                SQL += $"'{_TaskInfo.GetCraneInfo(trace.NextCrane, forkNumber).CraneShelfID}', ";
                SQL += $"'0', ";
                SQL += $"'{trace.NextCraneSpeed.TravelaxisSpeed}', ";
                SQL += $"'{trace.NextCraneSpeed.LifteraxisSpeed}', ";
                SQL += $"'{trace.NextCraneSpeed.RotateaxisSpeed}', ";
                SQL += $"'{trace.NextCraneSpeed.ForkaxisSpeed}', ";
                SQL += $"'{trace.UserID}', ";
                SQL += $"'{(trace.NextTransferMode == (int)TransferMode.SCAN ? "Y" : trace.BCRReadFlag ? "Y" : "N")}', ";
                SQL += $"'{trace.MainPriority}', ";
                SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                _db.ExecuteSQL(SQL);
            }

            if (trace.MainTransferMode == (int)TransferMode.FROM_TO || trace.MainTransferMode == (int)TransferMode.TO || trace.MainTransferMode == (int)TransferMode.MOVE)
            {
                if (trace.MainTransferMode == (int)TransferMode.TO && taskNo.Length > 1)
                {
                    SQL = "INSERT INTO TASK (STOCKERID, COMMANDID, TASKNO, CRANENO, FORKNUMBER, CSTID,";
                    SQL += " TRANSFERMODE, TRANSFERMODETYPE, SOURCE, SOURCEBAY, DESTINATION, DESTINATIONBAY,";
                    SQL += " TRAVELAXISSPEED, LIFTERAXISSPEED, ROTATEAXISSPEED, FORKAXISSPEED, ";
                    SQL += " USERID, BCRREADFLAG, PRIORITY, QUEUEDT) VALUES (";
                    SQL += $"'{_stockerId}', ";
                    SQL += $"'{trace.CommandID}', ";
                    SQL += $"'{taskNo[0]}', ";
                    SQL += $"'{trace.NextCrane}', ";
                    SQL += $"'{forkNumber}', ";
                    SQL += $"'{trace.CarrierID}', ";
                    SQL += $"'{(int)TransferMode.FROM}', ";
                    SQL += $"'{(int)TransferModeType.ShelfToRM}', ";
                    SQL += $"'{trace.NextSource}', ";
                    SQL += $"'{trace.NextSourceBay}', ";
                    SQL += $"'{_TaskInfo.GetCraneInfo(trace.NextCrane, forkNumber).CraneShelfID}', ";
                    SQL += $"'0', ";
                    SQL += $"'{trace.NextCraneSpeed.TravelaxisSpeed}', ";
                    SQL += $"'{trace.NextCraneSpeed.LifteraxisSpeed}', ";
                    SQL += $"'{trace.NextCraneSpeed.RotateaxisSpeed}', ";
                    SQL += $"'{trace.NextCraneSpeed.ForkaxisSpeed}', ";
                    SQL += $"'{trace.UserID}', ";
                    SQL += $"'{(trace.NextTransferMode == (int)TransferMode.SCAN ? "Y" : trace.BCRReadFlag ? "Y" : "N")}', ";
                    SQL += $"'{trace.MainPriority}', ";
                    SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                    _db.ExecuteSQL(SQL);
                }
                SQL = "INSERT INTO TASK (STOCKERID, COMMANDID, TASKNO, CRANENO, FORKNUMBER, CSTID,";
                SQL += " TRANSFERMODE, TRANSFERMODETYPE, SOURCE, SOURCEBAY, DESTINATION, DESTINATIONBAY,";
                SQL += " TRAVELAXISSPEED, LIFTERAXISSPEED, ROTATEAXISSPEED, FORKAXISSPEED, ";
                SQL += " USERID, BCRREADFLAG, PRIORITY, QUEUEDT) VALUES (";
                SQL += $"'{_stockerId}', ";
                SQL += $"'{trace.CommandID}', ";
                SQL += $"'{(taskNo.Length > 1 ? taskNo[1] : taskNo[0])}', ";
                SQL += $"'{trace.NextCrane}', ";
                SQL += $"'{forkNumber}', ";
                SQL += $"'{trace.CarrierID}', ";
                SQL += $"'{(int)TransferMode.TO}', ";
                SQL += $"'{(int)TransferModeType.RMToShelf}', ";
                SQL += $"'{_TaskInfo.GetCraneInfo(trace.NextCrane, forkNumber).CraneShelfID}', ";
                SQL += $"'0', ";
                SQL += $"'{(dest.ShelfType == (int)ShelfType.Port ? dest.PLCPortID.ToString() : dest.ShelfID)}', ";
                SQL += $"'{Convert.ToInt32(dest.Bay_Y)}', ";
                SQL += $"'{trace.NextCraneSpeed.TravelaxisSpeed}', ";
                SQL += $"'{trace.NextCraneSpeed.LifteraxisSpeed}', ";
                SQL += $"'{trace.NextCraneSpeed.RotateaxisSpeed}', ";
                SQL += $"'{trace.NextCraneSpeed.ForkaxisSpeed}', ";
                SQL += $"'{trace.UserID}', ";
                SQL += $"'N', ";
                SQL += $"'{trace.MainPriority}', ";
                SQL += $"'{DateTime.Now.AddMilliseconds(100).ToString("yyyy-MM-dd HH:mm:ss.fff")}')";

                return _db.ExecuteSQL(SQL);
            }
            return 0;
        }

        public int InsertTask(DB _db, CommandTrace trace, string taskNo, int forkNumber, VShelfInfo dest)
        {
            if (trace == null)
                return ErrorCode.Exception;
            if (dest == null)
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(taskNo))
                return ErrorCode.Exception;

            string SQL = "INSERT INTO TASK (STOCKERID, COMMANDID, TASKNO, CRANENO, FORKNUMBER, CSTID,";
            SQL += " TRANSFERMODE, TRANSFERMODETYPE, SOURCE, SOURCEBAY, DESTINATION, DESTINATIONBAY,";
            SQL += " TRAVELAXISSPEED, LIFTERAXISSPEED, ROTATEAXISSPEED, FORKAXISSPEED, ";
            SQL += " USERID, BCRREADFLAG, PRIORITY, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{trace.CommandID}', ";
            SQL += $"'{taskNo}', ";
            SQL += $"'{trace.NextCrane}', ";
            SQL += $"'{forkNumber}', ";
            SQL += $"'{trace.CarrierID}', ";
            SQL += $"'{trace.NextTransferMode}', ";
            SQL += $"'{trace.NextTransferModeType}', ";
            SQL += $"'{trace.NextSource}', ";
            SQL += $"'{trace.NextSourceBay}', ";
            SQL += $"'{(dest.ShelfType == (int)ShelfType.Port ? dest.PLCPortID.ToString() : dest.ShelfID)}', ";
            SQL += $"'{Convert.ToInt32(dest.Bay_Y)}', ";
            SQL += $"'{trace.NextCraneSpeed.TravelaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.LifteraxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.RotateaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.ForkaxisSpeed}', ";
            SQL += $"'{trace.UserID}', ";
            SQL += $"'{(trace.NextTransferMode == (int)TransferMode.SCAN ? "Y" : trace.BCRReadFlag ? "Y" : "N")}', ";
            SQL += $"'{trace.MainPriority}', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }

        public int InsertTask(DB _db, CommandTrace trace, string taskNo, int forkNumber, string cstType = "")
        {
            if (trace == null)
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(taskNo))
                return ErrorCode.Exception;

            string SQL = "INSERT INTO TASK (STOCKERID, COMMANDID, TASKNO, CRANENO, FORKNUMBER, CSTID,";
            SQL += " TRANSFERMODE, TRANSFERMODETYPE, SOURCE, SOURCEBAY, DESTINATION, DESTINATIONBAY,";
            SQL += " TRAVELAXISSPEED, LIFTERAXISSPEED, ROTATEAXISSPEED, FORKAXISSPEED, ";
            SQL += " USERID, CSTTYPE, BCRREADFLAG, PRIORITY, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{trace.CommandID}', ";
            SQL += $"'{taskNo}', ";
            SQL += $"'{trace.NextCrane}', ";
            SQL += $"'{forkNumber}', ";
            SQL += $"'{trace.CarrierID}', ";
            SQL += $"'{trace.NextTransferMode}', ";
            SQL += $"'{trace.NextTransferModeType}', ";
            SQL += $"'{trace.NextSource}', ";
            SQL += $"'{trace.NextSourceBay}', ";
            SQL += $"'{trace.NextDest}', ";
            SQL += $"'{trace.NextDestBay}', ";
            SQL += $"'{trace.NextCraneSpeed.TravelaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.LifteraxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.RotateaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.ForkaxisSpeed}', ";
            SQL += $"'{trace.UserID}', ";
            SQL += $"'{cstType}', ";
            SQL += $"'{(trace.NextTransferMode == (int)TransferMode.SCAN ? "Y" : trace.BCRReadFlag ? "Y" : "N")}', ";
            SQL += $"'{trace.MainPriority}', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }

        public int InsertTaskForMove(DB _db, CommandTrace trace, string taskNo, int forkNumber = 1)
        {
            if (trace == null)
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(taskNo))
                return ErrorCode.Exception;

            string SQL = "INSERT INTO TASK (STOCKERID, COMMANDID, TASKNO, CRANENO, FORKNUMBER, CSTID,";
            SQL += " TRANSFERMODE, TRANSFERMODETYPE, SOURCE, SOURCEBAY, DESTINATION, DESTINATIONBAY,";
            SQL += " TRAVELAXISSPEED, LIFTERAXISSPEED, ROTATEAXISSPEED, FORKAXISSPEED, ";
            SQL += " USERID, BCRREADFLAG, PRIORITY, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{trace.CommandID}', ";
            SQL += $"'{taskNo}', ";
            SQL += $"'{trace.NextCrane}', ";
            SQL += $"'{forkNumber}', ";
            SQL += $"'', ";
            SQL += $"'{(int)TransferMode.MOVE}', ";
            SQL += $"'{((int)TransferModeType.Move).ToString()}', ";
            SQL += $"'0', ";
            SQL += $"'0', ";
            SQL += $"'{trace.NextDest}', ";
            SQL += $"'{trace.NextDestBay}', ";
            SQL += $"'{trace.NextCraneSpeed.TravelaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.LifteraxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.RotateaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.ForkaxisSpeed}', ";
            SQL += $"'{trace.UserID}', ";
            SQL += $"'N', ";
            SQL += $"'{trace.MainPriority}', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }

        public int InsertTaskForScan(DB _db, CommandTrace trace, string taskNo, int forkNumber = 1)
        {
            if (trace == null)
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(taskNo))
                return ErrorCode.Exception;

            string SQL = "INSERT INTO TASK (STOCKERID, COMMANDID, TASKNO, CRANENO, FORKNUMBER, CSTID,";
            SQL += " TRANSFERMODE, TRANSFERMODETYPE, SOURCE, SOURCEBAY, DESTINATION, DESTINATIONBAY,";
            SQL += " TRAVELAXISSPEED, LIFTERAXISSPEED, ROTATEAXISSPEED, FORKAXISSPEED, ";
            SQL += " USERID, BCRREADFLAG, PRIORITY, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{trace.CommandID}', ";
            SQL += $"'{taskNo}', ";
            SQL += $"'{trace.NextCrane}', ";
            SQL += $"'{forkNumber}', ";
            SQL += $"'{trace.CarrierID}', ";
            SQL += $"'{(int)TransferMode.SCAN}', ";
            SQL += $"'{((int)TransferModeType.Scan).ToString()}', ";
            SQL += $"'{trace.NextSource}', ";
            SQL += $"'{trace.NextSourceBay}', ";
            SQL += $"'0', ";
            SQL += $"'0', ";
            SQL += $"'{trace.NextCraneSpeed.TravelaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.LifteraxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.RotateaxisSpeed}', ";
            SQL += $"'{trace.NextCraneSpeed.ForkaxisSpeed}', ";
            SQL += $"'{trace.UserID}', ";
            SQL += $"'Y', ";
            SQL += $"'{trace.MainPriority}', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }
        #endregion Task

        #region ShelfDef/CarrierData
        public IEnumerable<VShelfInfo> GetAllSharePortByCraneNo(DB _db, int craneNo)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND SHELFTYPE='{(int)ShelfType.Port}'";
                SQL += $" AND PORTLOCATIONTYPE='{(int)PortLocationType.SharePort}'";
                SQL += $" AND STAGE='1'";
                SQL += $" AND LOCATECRANENO='{craneNo}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<VShelfInfo> shelfInfos = new List<VShelfInfo>();
                    for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                    {
                        VShelfInfo info = new VShelfInfo();
                        info.CarrierLoc = Convert.ToString(table.Rows[iRow]["CarrierLoc"]);
                        info.ZoneID = Convert.ToString(table.Rows[iRow]["ZoneID"]);
                        info.Bank_X = Convert.ToString(table.Rows[iRow]["Bank_X"]);
                        info.Bay_Y = Convert.ToString(table.Rows[iRow]["Bay_Y"]);
                        info.Level_Z = Convert.ToString(table.Rows[iRow]["Level_Z"]);
                        info.LocateCraneNo = Convert.ToInt32(table.Rows[iRow]["LOCATECRANENO"]);
                        info.Enable = Convert.ToString(table.Rows[iRow]["Enable"]) == "Y";
                        info.EmptyBlockFlag = Convert.ToString(table.Rows[iRow]["EmptyBlockFlag"]) == "Y";
                        info.HoldState = Convert.ToInt32(table.Rows[iRow]["HoldState"]);
                        info.BCRReadFlag = Convert.ToString(table.Rows[iRow]["BCRReadFlag"]) == "Y";
                        info.ShelfID = Convert.ToString(table.Rows[iRow]["SHELFID"]);
                        info.ShelfType = Convert.ToInt32(table.Rows[iRow]["SHELFTYPE"]);
                        info.ChargeLoc = Convert.ToString(table.Rows[iRow]["ChargeLoc"]) == "Y";
                        info.SelectPriority = Convert.ToInt32(table.Rows[iRow]["SELECTPRIORITY"]);
                        info.ShelfState = Convert.ToChar(table.Rows[iRow]["ShelfState"]);
                        info.HostEQPort = Convert.ToString(table.Rows[iRow]["HostEQPortID"]);
                        info.Stage = Convert.ToInt32(table.Rows[iRow]["STAGE"]);
                        info.Vehicles = Convert.ToInt32(table.Rows[iRow]["Vehicles"]);
                        info.PortType = Convert.ToInt32(table.Rows[iRow]["PortType"]);
                        info.PortLocationType = Convert.ToInt32(table.Rows[iRow]["PortLocationType"]);
                        info.PLCPortID = Convert.ToInt32(table.Rows[iRow]["PLCPortID"]);
                        info.PortTypeIndex = Convert.ToInt32(table.Rows[iRow]["PortTypeIndex"]);
                        info.StageCount = Convert.ToInt32(table.Rows[iRow]["StageCount"]);
                        info.CSTID = Convert.ToString(table.Rows[iRow]["CSTID"]);
                        info.LotID = Convert.ToString(table.Rows[iRow]["LotID"]);
                        info.EmptyCST = Convert.ToString(table.Rows[iRow]["EmptyCST"]);
                        info.CSTType = Convert.ToString(table.Rows[iRow]["CSTType"]);
                        info.CSTState = Convert.ToInt32(table.Rows[iRow]["CSTState"]);
                        info.CSTInDT = Convert.ToString(table.Rows[iRow]["CSTInDT"]);
                        info.StoreDT = Convert.ToString(table.Rows[iRow]["StoreDT"]);
                        shelfInfos.Add(info);
                    }
                    return shelfInfos;
                }
                else
                    return new List<VShelfInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<VShelfInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public bool GetOutputSharePort(DB _db, int craneNo, out VShelfInfo info)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND SHELFTYPE='{(int)ShelfType.Port}'";
                SQL += $" AND PORTLOCATIONTYPE='{(int)PortLocationType.SharePort}'";
                SQL += $" AND DIRECTION='{(int)Direction.OnlyOutput}'";
                SQL += $" AND STAGE='1'";
                SQL += $" AND LOCATECRANENO='{craneNo}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    info = new VShelfInfo();
                    info.CarrierLoc = Convert.ToString(table.Rows[0]["CarrierLoc"]);
                    info.ZoneID = Convert.ToString(table.Rows[0]["ZoneID"]);
                    info.Bank_X = Convert.ToString(table.Rows[0]["Bank_X"]);
                    info.Bay_Y = Convert.ToString(table.Rows[0]["Bay_Y"]);
                    info.Level_Z = Convert.ToString(table.Rows[0]["Level_Z"]);
                    info.LocateCraneNo = Convert.ToInt32(table.Rows[0]["LOCATECRANENO"]);
                    info.Enable = Convert.ToString(table.Rows[0]["Enable"]) == "Y";
                    info.EmptyBlockFlag = Convert.ToString(table.Rows[0]["EmptyBlockFlag"]) == "Y";
                    info.HoldState = Convert.ToInt32(table.Rows[0]["HoldState"]);
                    info.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRReadFlag"]) == "Y";
                    info.ShelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                    info.ShelfType = Convert.ToInt32(table.Rows[0]["SHELFTYPE"]);
                    info.ChargeLoc = Convert.ToString(table.Rows[0]["ChargeLoc"]) == "Y";
                    info.SelectPriority = Convert.ToInt32(table.Rows[0]["SELECTPRIORITY"]);
                    info.ShelfState = Convert.ToChar(table.Rows[0]["ShelfState"]);
                    info.HostEQPort = Convert.ToString(table.Rows[0]["HostEQPortID"]);
                    info.Stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                    info.Vehicles = Convert.ToInt32(table.Rows[0]["Vehicles"]);
                    info.PortType = Convert.ToInt32(table.Rows[0]["PortType"]);
                    info.PortLocationType = Convert.ToInt32(table.Rows[0]["PortLocationType"]);
                    info.PLCPortID = Convert.ToInt32(table.Rows[0]["PLCPORTID"]);
                    info.PortTypeIndex = Convert.ToInt32(table.Rows[0]["PortTypeIndex"]);
                    info.StageCount = Convert.ToInt32(table.Rows[0]["StageCount"]);
                    info.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    info.LotID = Convert.ToString(table.Rows[0]["LotID"]);
                    info.EmptyCST = Convert.ToString(table.Rows[0]["EmptyCST"]);
                    info.CSTType = Convert.ToString(table.Rows[0]["CSTType"]);
                    info.CSTState = Convert.ToInt32(table.Rows[0]["CSTState"]);
                    info.CSTInDT = Convert.ToString(table.Rows[0]["CSTInDT"]);
                    info.StoreDT = Convert.ToString(table.Rows[0]["StoreDT"]);
                    return true;
                }
                info = null;
                return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                info = null;
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public bool GetHandoffShelf(DB _db, VShelfInfo source, VShelfInfo tmpHandOff, out VShelfInfo info)
        {
            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();
            try
            {
                string SQL = "SELECT * FROM ZONEDEF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND ZONETYPE='{(int)ZoneType.Handoff}'";
                if (_db.GetDataTable(SQL, ref table1) == ErrorCode.Success)
                {
                    string handoffZoneID = Convert.ToString(table1.Rows[0]["ZONEID"]);
                    SQL = "SELECT * FROM V_SHELF";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    SQL += $" AND ZONEID='{handoffZoneID}'";
                    SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                    SQL += $" AND SHELFSTATE='{(char)ShelfState.EmptyShelf}'";
                    SQL += $" AND ENABLE='{(char)ShelfEnable.Enable}'";

                    if (!string.IsNullOrWhiteSpace(tmpHandOff.ShelfID))
                        SQL += $" AND SHELFID <> '{tmpHandOff.ShelfID}'";

                    if (Convert.ToInt32(source.Bank_X) == 2)
                        SQL += " ORDER BY SELECTPRIORITY DESC, BANK_X DESC,";
                    else
                        SQL += " ORDER BY SELECTPRIORITY DESC, BANK_X,";
                    if (source.LocateCraneNo == 2)
                        SQL += " BAY_Y,";
                    else
                        SQL += " BAY_Y DESC,";
                    SQL += " ABS(CAST(LEVEL_Z AS INT)-1)";

                    if (_db.GetDataTable(SQL, ref table2) == ErrorCode.Success)
                    {
                        info = new VShelfInfo();
                        info.CarrierLoc = Convert.ToString(table2.Rows[0]["CarrierLoc"]);
                        info.ZoneID = Convert.ToString(table2.Rows[0]["ZoneID"]);
                        info.Bank_X = Convert.ToString(table2.Rows[0]["Bank_X"]);
                        info.Bay_Y = Convert.ToString(table2.Rows[0]["Bay_Y"]);
                        info.Level_Z = Convert.ToString(table2.Rows[0]["Level_Z"]);
                        info.LocateCraneNo = Convert.ToInt32(table2.Rows[0]["LOCATECRANENO"]);
                        info.Enable = Convert.ToString(table2.Rows[0]["Enable"]) == "Y";
                        info.EmptyBlockFlag = Convert.ToString(table2.Rows[0]["EmptyBlockFlag"]) == "Y";
                        info.HoldState = Convert.ToInt32(table2.Rows[0]["HoldState"]);
                        info.BCRReadFlag = Convert.ToString(table2.Rows[0]["BCRReadFlag"]) == "Y";
                        info.ShelfID = Convert.ToString(table2.Rows[0]["SHELFID"]);
                        info.ShelfType = Convert.ToInt32(table2.Rows[0]["SHELFTYPE"]);
                        info.ChargeLoc = Convert.ToString(table2.Rows[0]["ChargeLoc"]) == "Y";
                        info.SelectPriority = Convert.ToInt32(table2.Rows[0]["SELECTPRIORITY"]);
                        info.ShelfState = Convert.ToChar(table2.Rows[0]["ShelfState"]);
                        info.HostEQPort = Convert.ToString(table2.Rows[0]["HostEQPortID"]);
                        info.Stage = Convert.ToInt32(table2.Rows[0]["STAGE"]);
                        info.Vehicles = Convert.ToInt32(table2.Rows[0]["Vehicles"]);
                        info.PortType = Convert.ToInt32(table2.Rows[0]["PortType"]);
                        info.PortLocationType = Convert.ToInt32(table2.Rows[0]["PortLocationType"]);
                        info.PLCPortID = Convert.ToInt32(table2.Rows[0]["PLCPortID"]);
                        info.PortTypeIndex = Convert.ToInt32(table2.Rows[0]["PortTypeIndex"]);
                        info.StageCount = Convert.ToInt32(table2.Rows[0]["StageCount"]);
                        info.CSTID = Convert.ToString(table2.Rows[0]["CSTID"]);
                        info.LotID = Convert.ToString(table2.Rows[0]["LotID"]);
                        info.EmptyCST = Convert.ToString(table2.Rows[0]["EmptyCST"]);
                        info.CSTType = Convert.ToString(table2.Rows[0]["CSTType"]);
                        info.CSTState = Convert.ToInt32(table2.Rows[0]["CSTState"]);
                        info.CSTInDT = Convert.ToString(table2.Rows[0]["CSTInDT"]);
                        info.StoreDT = Convert.ToString(table2.Rows[0]["StoreDT"]);
                        return true;
                    }
                }
                info = null;
                return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                info = null;
                return false;
            }
            finally
            {
                table1?.Clear();
                table1?.Dispose();
                table2?.Clear();
                table2?.Dispose();
            }
        }

        public bool GetHandoffShelf(DB _db, VShelfInfo source, int forkNumber, out VShelfInfo info)
        {
            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();
            try
            {
                string SQL = "SELECT * FROM ZONEDEF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND ZONETYPE='{(int)ZoneType.Handoff}'";
                if (_db.GetDataTable(SQL, ref table1) == ErrorCode.Success)
                {
                    string handoffZoneID = Convert.ToString(table1.Rows[0]["ZONEID"]);
                    SQL = "SELECT * FROM V_SHELF";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    SQL += $" AND ZONEID='{handoffZoneID}'";
                    SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                    SQL += $" AND SHELFSTATE='{(char)ShelfState.EmptyShelf}'";
                    SQL += $" AND ENABLE='{(char)ShelfEnable.Enable}'";
                    //if (Convert.ToInt32(source.Bank_X) == 1)
                    //{
                    //    SQL += " ORDER BY SELECTPRIORITY DESC, BANK_X,";
                    //    if ((source.LocateCraneNo == 1 && forkNumber == 1) || (source.LocateCraneNo == 2 && forkNumber == 1))
                    //        SQL += " BAY_Y,";
                    //    else
                    //        SQL += " BAY_Y DESC,";
                    //}
                    //else
                    //{
                    SQL += " ORDER BY SELECTPRIORITY DESC, BANK_X DESC,";
                    if ((source.LocateCraneNo == 1 && forkNumber == 1) || (source.LocateCraneNo == 2 && forkNumber == 1))
                        SQL += " BAY_Y DESC,";
                    else
                        SQL += " BAY_Y,";
                    //}

                    SQL += " ABS(CAST(LEVEL_Z AS INT)-1)";

                    if (_db.GetDataTable(SQL, ref table2) == ErrorCode.Success)
                    {
                        info = new VShelfInfo();
                        info.CarrierLoc = Convert.ToString(table2.Rows[0]["CarrierLoc"]);
                        info.ZoneID = Convert.ToString(table2.Rows[0]["ZoneID"]);
                        info.Bank_X = Convert.ToString(table2.Rows[0]["Bank_X"]);
                        info.Bay_Y = Convert.ToString(table2.Rows[0]["Bay_Y"]);
                        info.Level_Z = Convert.ToString(table2.Rows[0]["Level_Z"]);
                        info.LocateCraneNo = Convert.ToInt32(table2.Rows[0]["LOCATECRANENO"]);
                        info.Enable = Convert.ToString(table2.Rows[0]["Enable"]) == "Y";
                        info.EmptyBlockFlag = Convert.ToString(table2.Rows[0]["EmptyBlockFlag"]) == "Y";
                        info.HoldState = Convert.ToInt32(table2.Rows[0]["HoldState"]);
                        info.BCRReadFlag = Convert.ToString(table2.Rows[0]["BCRReadFlag"]) == "Y";
                        info.ShelfID = Convert.ToString(table2.Rows[0]["SHELFID"]);
                        info.ShelfType = Convert.ToInt32(table2.Rows[0]["SHELFTYPE"]);
                        info.ChargeLoc = Convert.ToString(table2.Rows[0]["ChargeLoc"]) == "Y";
                        info.SelectPriority = Convert.ToInt32(table2.Rows[0]["SELECTPRIORITY"]);
                        info.ShelfState = Convert.ToChar(table2.Rows[0]["ShelfState"]);
                        info.HostEQPort = Convert.ToString(table2.Rows[0]["HostEQPortID"]);
                        info.Stage = Convert.ToInt32(table2.Rows[0]["STAGE"]);
                        info.Vehicles = Convert.ToInt32(table2.Rows[0]["Vehicles"]);
                        info.PortType = Convert.ToInt32(table2.Rows[0]["PortType"]);
                        info.PortLocationType = Convert.ToInt32(table2.Rows[0]["PortLocationType"]);
                        info.PLCPortID = Convert.ToInt32(table2.Rows[0]["PLCPortID"]);
                        info.PortTypeIndex = Convert.ToInt32(table2.Rows[0]["PortTypeIndex"]);
                        info.StageCount = Convert.ToInt32(table2.Rows[0]["StageCount"]);
                        info.CSTID = Convert.ToString(table2.Rows[0]["CSTID"]);
                        info.LotID = Convert.ToString(table2.Rows[0]["LotID"]);
                        info.EmptyCST = Convert.ToString(table2.Rows[0]["EmptyCST"]);
                        info.CSTType = Convert.ToString(table2.Rows[0]["CSTType"]);
                        info.CSTState = Convert.ToInt32(table2.Rows[0]["CSTState"]);
                        info.CSTInDT = Convert.ToString(table2.Rows[0]["CSTInDT"]);
                        info.StoreDT = Convert.ToString(table2.Rows[0]["StoreDT"]);
                        return true;
                    }
                }
                info = null;
                return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                info = null;
                return false;
            }
            finally
            {
                table1?.Clear();
                table1?.Dispose();
                table2?.Clear();
                table2?.Dispose();
            }
        }

        public IEnumerable<VShelfInfo> GetShortestPath(DB _db, VShelfInfo source, string carrierLoc)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT (";
                SQL += $" ABS(CAST(BAY_Y AS INT)-{source.Bay_Y}) +";
                SQL += $" ABS(CAST(LEVEL_Z AS INT)-{source.Level_Z}) +";
                SQL += $" ABS(CAST(BANK_X AS INT)-{source.Bank_X}))";
                SQL += $" DIS, * FROM V_SHELF";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND (CARRIERLOC='{carrierLoc}'";
                SQL += $" OR ZONEID='{carrierLoc }')";
                SQL += $" AND STAGE='1'";
                SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                SQL += $" AND SHELFSTATE='{(char)ShelfState.EmptyShelf}'";
                SQL += $" AND ENABLE='{(char)ShelfEnable.Enable}'";
                SQL += $" ORDER BY DIS,";
                SQL += $" CASE WHEN CAST(LEVEL_Z AS INT) <> {source.Level_Z} THEN 0 ELSE 1 END,";
                SQL += $" CASE WHEN CAST(BAY_Y AS INT) <> {source.Bank_X} THEN 1 ELSE 0 END,";
                SQL += $" SELECTPRIORITY DESC";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<VShelfInfo> infos = new List<VShelfInfo>();
                    for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                    {
                        VShelfInfo info = new VShelfInfo();
                        info.CarrierLoc = Convert.ToString(table.Rows[iRow]["CarrierLoc"]);
                        info.ZoneID = Convert.ToString(table.Rows[iRow]["ZoneID"]);
                        info.Bank_X = Convert.ToString(table.Rows[iRow]["Bank_X"]);
                        info.Bay_Y = Convert.ToString(table.Rows[iRow]["Bay_Y"]);
                        info.Level_Z = Convert.ToString(table.Rows[iRow]["Level_Z"]);
                        info.LocateCraneNo = Convert.ToInt32(table.Rows[iRow]["LOCATECRANENO"]);
                        info.Enable = Convert.ToString(table.Rows[iRow]["Enable"]) == "Y";
                        info.EmptyBlockFlag = Convert.ToString(table.Rows[iRow]["EmptyBlockFlag"]) == "Y";
                        info.HoldState = Convert.ToInt32(table.Rows[iRow]["HoldState"]);
                        info.BCRReadFlag = Convert.ToString(table.Rows[iRow]["BCRReadFlag"]) == "Y";
                        info.ShelfID = Convert.ToString(table.Rows[iRow]["SHELFID"]);
                        info.ShelfType = Convert.ToInt32(table.Rows[iRow]["SHELFTYPE"]);
                        info.ChargeLoc = Convert.ToString(table.Rows[iRow]["ChargeLoc"]) == "Y";
                        info.SelectPriority = Convert.ToInt32(table.Rows[iRow]["SELECTPRIORITY"]);
                        info.ShelfState = Convert.ToChar(table.Rows[iRow]["ShelfState"]);
                        info.HostEQPort = Convert.ToString(table.Rows[iRow]["HostEQPortID"]);
                        info.Stage = Convert.ToInt32(table.Rows[iRow]["STAGE"]);
                        info.Vehicles = Convert.ToInt32(table.Rows[iRow]["Vehicles"]);
                        info.PortType = Convert.ToInt32(table.Rows[iRow]["PortType"]);
                        info.PortLocationType = Convert.ToInt32(table.Rows[iRow]["PortLocationType"]);
                        info.PLCPortID = Convert.ToInt32(table.Rows[iRow]["PLCPortID"]);
                        info.PortTypeIndex = Convert.ToInt32(table.Rows[iRow]["PortTypeIndex"]);
                        info.StageCount = Convert.ToInt32(table.Rows[iRow]["StageCount"]);
                        info.CSTID = Convert.ToString(table.Rows[iRow]["CSTID"]);
                        info.LotID = Convert.ToString(table.Rows[iRow]["LotID"]);
                        info.EmptyCST = Convert.ToString(table.Rows[iRow]["EmptyCST"]);
                        info.CSTType = Convert.ToString(table.Rows[iRow]["CSTType"]);
                        info.CSTState = Convert.ToInt32(table.Rows[iRow]["CSTState"]);
                        info.CSTInDT = Convert.ToString(table.Rows[iRow]["CSTInDT"]);
                        info.StoreDT = Convert.ToString(table.Rows[iRow]["StoreDT"]);
                        infos.Add(info);
                    }
                    return infos;
                }
                else
                    return new List<VShelfInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<VShelfInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public IEnumerable<VShelfInfo> GetShortestPath(DB _db, int craneNo, int shareAreaBay)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = $"SELECT (ABS(CAST(BAY_Y AS INT)-{shareAreaBay})) DIS, * FROM V_SHELF";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND STAGE='1'";
                SQL += $" AND SHELFTYPE IN('{(int)ShelfType.Shelf}', '{(int)ShelfType.Port}')";
                SQL += craneNo == 1 ? $" AND CAST(BAY_Y AS INT)<'{shareAreaBay}'" : $" AND CAST(BAY_Y AS INT)>'{shareAreaBay}'";
                SQL += craneNo == 1 ? " ORDER BY BAY_Y DESC," : " ORDER BY BAY_Y,";
                SQL += $" DIS DESC,";
                SQL += $" SELECTPRIORITY DESC";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<VShelfInfo> infos = new List<VShelfInfo>();
                    for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                    {
                        VShelfInfo info = new VShelfInfo();
                        info.CarrierLoc = Convert.ToString(table.Rows[iRow]["CarrierLoc"]);
                        info.ZoneID = Convert.ToString(table.Rows[iRow]["ZoneID"]);
                        info.Bank_X = Convert.ToString(table.Rows[iRow]["Bank_X"]);
                        info.Bay_Y = Convert.ToString(table.Rows[iRow]["Bay_Y"]);
                        info.Level_Z = Convert.ToString(table.Rows[iRow]["Level_Z"]);
                        info.LocateCraneNo = Convert.ToInt32(table.Rows[iRow]["LOCATECRANENO"]);
                        info.Enable = Convert.ToString(table.Rows[iRow]["Enable"]) == "Y";
                        info.EmptyBlockFlag = Convert.ToString(table.Rows[iRow]["EmptyBlockFlag"]) == "Y";
                        info.HoldState = Convert.ToInt32(table.Rows[iRow]["HoldState"]);
                        info.BCRReadFlag = Convert.ToString(table.Rows[iRow]["BCRReadFlag"]) == "Y";
                        info.ShelfID = Convert.ToString(table.Rows[iRow]["SHELFID"]);
                        info.ShelfType = Convert.ToInt32(table.Rows[iRow]["SHELFTYPE"]);
                        info.ChargeLoc = Convert.ToString(table.Rows[iRow]["ChargeLoc"]) == "Y";
                        info.SelectPriority = Convert.ToInt32(table.Rows[iRow]["SELECTPRIORITY"]);
                        info.ShelfState = Convert.ToChar(table.Rows[iRow]["ShelfState"]);
                        info.HostEQPort = Convert.ToString(table.Rows[iRow]["HostEQPortID"]);
                        info.Stage = Convert.ToInt32(table.Rows[iRow]["STAGE"]);
                        info.Vehicles = Convert.ToInt32(table.Rows[iRow]["Vehicles"]);
                        info.PortType = Convert.ToInt32(table.Rows[iRow]["PortType"]);
                        info.PortLocationType = Convert.ToInt32(table.Rows[iRow]["PortLocationType"]);
                        info.PLCPortID = Convert.ToInt32(table.Rows[iRow]["PLCPortID"]);
                        info.PortTypeIndex = Convert.ToInt32(table.Rows[iRow]["PortTypeIndex"]);
                        info.StageCount = Convert.ToInt32(table.Rows[iRow]["StageCount"]);
                        info.CSTID = Convert.ToString(table.Rows[iRow]["CSTID"]);
                        info.LotID = Convert.ToString(table.Rows[iRow]["LotID"]);
                        info.EmptyCST = Convert.ToString(table.Rows[iRow]["EmptyCST"]);
                        info.CSTType = Convert.ToString(table.Rows[iRow]["CSTType"]);
                        info.CSTState = Convert.ToInt32(table.Rows[iRow]["CSTState"]);
                        info.CSTInDT = Convert.ToString(table.Rows[iRow]["CSTInDT"]);
                        info.StoreDT = Convert.ToString(table.Rows[iRow]["StoreDT"]);
                        infos.Add(info);
                    }
                    return infos;
                }
                else
                    return new List<VShelfInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<VShelfInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public bool GetAlternateShelf(DB _db, VShelfInfo source, out VShelfInfo dest)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND SHELFID<>'{source.ShelfID}'";
                SQL += $" AND ZONEID<>'{source.ZoneID}'";
                SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                SQL += $" AND SHELFSTATE='{(char)ShelfState.EmptyShelf}'";
                SQL += $" AND ENABLE='{(char)ShelfEnable.Enable}'";
                SQL += $" ORDER BY ABS(CAST(BAY_Y AS INT)-{Convert.ToInt32(source.Bay_Y)}),";
                if (Convert.ToInt32(source.Bank_X) == 2)
                    SQL += " BANK_X DESC,";
                else
                    SQL += " BANK_X,";
                SQL += $" ABS(CAST(LEVEL_Z AS INT)-{Convert.ToInt32(source.Level_Z)})";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    dest = new VShelfInfo();
                    dest.CarrierLoc = Convert.ToString(table.Rows[0]["CarrierLoc"]);
                    dest.ZoneID = Convert.ToString(table.Rows[0]["ZoneID"]);
                    dest.Bank_X = Convert.ToString(table.Rows[0]["Bank_X"]);
                    dest.Bay_Y = Convert.ToString(table.Rows[0]["Bay_Y"]);
                    dest.Level_Z = Convert.ToString(table.Rows[0]["Level_Z"]);
                    dest.LocateCraneNo = Convert.ToInt32(table.Rows[0]["LOCATECRANENO"]);
                    dest.Enable = Convert.ToString(table.Rows[0]["Enable"]) == "Y";
                    dest.EmptyBlockFlag = Convert.ToString(table.Rows[0]["EmptyBlockFlag"]) == "Y";
                    dest.HoldState = Convert.ToInt32(table.Rows[0]["HoldState"]);
                    dest.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRReadFlag"]) == "Y";
                    dest.ShelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                    dest.ShelfType = Convert.ToInt32(table.Rows[0]["SHELFTYPE"]);
                    dest.ChargeLoc = Convert.ToString(table.Rows[0]["ChargeLoc"]) == "Y";
                    dest.SelectPriority = Convert.ToInt32(table.Rows[0]["SELECTPRIORITY"]);
                    dest.ShelfState = Convert.ToChar(table.Rows[0]["ShelfState"]);
                    dest.HostEQPort = Convert.ToString(table.Rows[0]["HostEQPortID"]);
                    dest.Stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                    dest.Vehicles = Convert.ToInt32(table.Rows[0]["Vehicles"]);
                    dest.PortType = Convert.ToInt32(table.Rows[0]["PortType"]);
                    dest.PortLocationType = Convert.ToInt32(table.Rows[0]["PortLocationType"]);
                    dest.PLCPortID = Convert.ToInt32(table.Rows[0]["PLCPortID"]);
                    dest.PortTypeIndex = Convert.ToInt32(table.Rows[0]["PortTypeIndex"]);
                    dest.StageCount = Convert.ToInt32(table.Rows[0]["StageCount"]);
                    dest.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    dest.LotID = Convert.ToString(table.Rows[0]["LotID"]);
                    dest.EmptyCST = Convert.ToString(table.Rows[0]["EmptyCST"]);
                    dest.CSTType = Convert.ToString(table.Rows[0]["CSTType"]);
                    dest.CSTState = Convert.ToInt32(table.Rows[0]["CSTState"]);
                    dest.CSTInDT = Convert.ToString(table.Rows[0]["CSTInDT"]);
                    dest.StoreDT = Convert.ToString(table.Rows[0]["StoreDT"]);
                    return true;
                }
                dest = null;
                return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                dest = null;
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public bool GetAlternateShelf(DB _db, int craneNo, VShelfInfo source, out VShelfInfo dest, bool islimitbay = false)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND SHELFID<>'{source.ShelfID}'";
                SQL += $" AND LOCATECRANENO='{craneNo}'";
                SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                SQL += $" AND SHELFSTATE='{(char)ShelfState.EmptyShelf}'";
                SQL += $" AND ENABLE='{(char)ShelfEnable.Enable}'";

                if (islimitbay)
                {
                    SQL += $" AND BAY_Y !='{source.Bay_Y}'";
                }

                SQL += $" ORDER BY ABS(CAST(BAY_Y AS INT)-{Convert.ToInt32(source.Bay_Y)}),";
                if (Convert.ToInt32(source.Bank_X) == 2)
                    SQL += " BANK_X DESC,";
                else
                    SQL += " BANK_X,";
                SQL += $" ABS(CAST(LEVEL_Z AS INT)-{Convert.ToInt32(source.Level_Z)})";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    dest = new VShelfInfo();
                    dest.CarrierLoc = Convert.ToString(table.Rows[0]["CarrierLoc"]);
                    dest.ZoneID = Convert.ToString(table.Rows[0]["ZoneID"]);
                    dest.Bank_X = Convert.ToString(table.Rows[0]["Bank_X"]);
                    dest.Bay_Y = Convert.ToString(table.Rows[0]["Bay_Y"]);
                    dest.Level_Z = Convert.ToString(table.Rows[0]["Level_Z"]);
                    dest.LocateCraneNo = Convert.ToInt32(table.Rows[0]["LOCATECRANENO"]);
                    dest.Enable = Convert.ToString(table.Rows[0]["Enable"]) == "Y";
                    dest.EmptyBlockFlag = Convert.ToString(table.Rows[0]["EmptyBlockFlag"]) == "Y";
                    dest.HoldState = Convert.ToInt32(table.Rows[0]["HoldState"]);
                    dest.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRReadFlag"]) == "Y";
                    dest.ShelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                    dest.ShelfType = Convert.ToInt32(table.Rows[0]["SHELFTYPE"]);
                    dest.ChargeLoc = Convert.ToString(table.Rows[0]["ChargeLoc"]) == "Y";
                    dest.SelectPriority = Convert.ToInt32(table.Rows[0]["SELECTPRIORITY"]);
                    dest.ShelfState = Convert.ToChar(table.Rows[0]["ShelfState"]);
                    dest.HostEQPort = Convert.ToString(table.Rows[0]["HostEQPortID"]);
                    dest.Stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                    dest.Vehicles = Convert.ToInt32(table.Rows[0]["Vehicles"]);
                    dest.PortType = Convert.ToInt32(table.Rows[0]["PortType"]);
                    dest.PortLocationType = Convert.ToInt32(table.Rows[0]["PortLocationType"]);
                    dest.PLCPortID = Convert.ToInt32(table.Rows[0]["PLCPortID"]);
                    dest.PortTypeIndex = Convert.ToInt32(table.Rows[0]["PortTypeIndex"]);
                    dest.StageCount = Convert.ToInt32(table.Rows[0]["StageCount"]);
                    dest.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    dest.LotID = Convert.ToString(table.Rows[0]["LotID"]);
                    dest.EmptyCST = Convert.ToString(table.Rows[0]["EmptyCST"]);
                    dest.CSTType = Convert.ToString(table.Rows[0]["CSTType"]);
                    dest.CSTState = Convert.ToInt32(table.Rows[0]["CSTState"]);
                    dest.CSTInDT = Convert.ToString(table.Rows[0]["CSTInDT"]);
                    dest.StoreDT = Convert.ToString(table.Rows[0]["StoreDT"]);
                    return true;
                }
                dest = null;
                return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                dest = null;
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public bool GetAlternateShelf(int craneNo, int currentBay, VShelfInfo source, string destZoneID, out VShelfInfo dest)
        {
            DataTable table = new DataTable();
            try
            {
                using (var _db = _TaskInfo.GetDB())
                {
                    string SQL = "SELECT * FROM V_SHELF";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    SQL += $" AND LOCATECRANENO='{craneNo}'";
                    SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                    SQL += $" AND SHELFSTATE='{(char)ShelfState.EmptyShelf}'";
                    SQL += $" AND ENABLE='{(char)ShelfEnable.Enable}'";
                    if (!string.IsNullOrWhiteSpace(destZoneID))
                        SQL += $" AND ZONEID='{destZoneID}'";
                    if (currentBay > 0)
                        SQL += $" ORDER BY ABS(CAST(BAY_Y AS INT)-{Convert.ToInt32(currentBay)}), SELECTPRIORITY DESC";
                    else if (source != null)
                    {
                        if (Convert.ToInt32(source.Bank_X) == 2)
                            SQL += $" ORDER BY ABS(CAST(BAY_Y AS INT)-{Convert.ToInt32(source.Bay_Y)}),";
                        else
                            SQL += $" ORDER BY ABS(CAST(BAY_Y AS INT)-{Convert.ToInt32(source.Bay_Y)}) DESC,";
                        if (Convert.ToInt32(source.Bank_X) == 2)
                            SQL += " BANK_X DESC,";
                        else
                            SQL += " BANK_X,";
                        SQL += $" ABS(CAST(LEVEL_Z AS INT)-{Convert.ToInt32(source.Level_Z)}), SELECTPRIORITY DESC";
                    }
                    else
                    {
                        if (craneNo == 1)
                        {
                            SQL += $" ORDER BY BAY_Y DESC, SELECTPRIORITY DESC";
                        }
                        else
                        {
                            SQL += $" ORDER BY BAY_Y, SELECTPRIORITY DESC";
                        }
                    }
                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        dest = new VShelfInfo();
                        dest.CarrierLoc = Convert.ToString(table.Rows[0]["CarrierLoc"]);
                        dest.ZoneID = Convert.ToString(table.Rows[0]["ZoneID"]);
                        dest.Bank_X = Convert.ToString(table.Rows[0]["Bank_X"]);
                        dest.Bay_Y = Convert.ToString(table.Rows[0]["Bay_Y"]);
                        dest.Level_Z = Convert.ToString(table.Rows[0]["Level_Z"]);
                        dest.LocateCraneNo = Convert.ToInt32(table.Rows[0]["LOCATECRANENO"]);
                        dest.Enable = Convert.ToString(table.Rows[0]["Enable"]) == "Y";
                        dest.EmptyBlockFlag = Convert.ToString(table.Rows[0]["EmptyBlockFlag"]) == "Y";
                        dest.HoldState = Convert.ToInt32(table.Rows[0]["HoldState"]);
                        dest.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRReadFlag"]) == "Y";
                        dest.ShelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                        dest.ShelfType = Convert.ToInt32(table.Rows[0]["SHELFTYPE"]);
                        dest.ChargeLoc = Convert.ToString(table.Rows[0]["ChargeLoc"]) == "Y";
                        dest.SelectPriority = Convert.ToInt32(table.Rows[0]["SELECTPRIORITY"]);
                        dest.ShelfState = Convert.ToChar(table.Rows[0]["ShelfState"]);
                        dest.HostEQPort = Convert.ToString(table.Rows[0]["HostEQPortID"]);
                        dest.Stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                        dest.Vehicles = Convert.ToInt32(table.Rows[0]["Vehicles"]);
                        dest.PortType = Convert.ToInt32(table.Rows[0]["PortType"]);
                        dest.PortLocationType = Convert.ToInt32(table.Rows[0]["PortLocationType"]);
                        dest.PLCPortID = Convert.ToInt32(table.Rows[0]["PLCPortID"]);
                        dest.PortTypeIndex = Convert.ToInt32(table.Rows[0]["PortTypeIndex"]);
                        dest.StageCount = Convert.ToInt32(table.Rows[0]["StageCount"]);
                        dest.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                        dest.LotID = Convert.ToString(table.Rows[0]["LotID"]);
                        dest.EmptyCST = Convert.ToString(table.Rows[0]["EmptyCST"]);
                        dest.CSTType = Convert.ToString(table.Rows[0]["CSTType"]);
                        dest.CSTState = Convert.ToInt32(table.Rows[0]["CSTState"]);
                        dest.CSTInDT = Convert.ToString(table.Rows[0]["CSTInDT"]);
                        dest.StoreDT = Convert.ToString(table.Rows[0]["StoreDT"]);
                        return true;
                    }
                    dest = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                dest = null;
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public bool GetAlternateShelf(DB _db, VShelfInfo source, string destZoneID, int shareAreaBayStart, int shareAreaBayEnd, out VShelfInfo dest)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND ZONEID='{destZoneID}'";
                SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                SQL += $" AND SHELFSTATE='{(char)ShelfState.EmptyShelf}'";
                SQL += $" AND ENABLE='{(char)ShelfEnable.Enable}'";
                if (shareAreaBayStart < shareAreaBayEnd)
                {
                    SQL += $" AND (CAST(BAY_Y AS INT)<'{shareAreaBayStart}'";
                    SQL += $" OR CAST(BAY_Y AS INT)>'{shareAreaBayEnd}')";
                }
                else
                {
                    SQL += $" AND CAST(BAY_Y AS INT)<='{shareAreaBayStart}'";
                    SQL += $" AND CAST(BAY_Y AS INT)>='{shareAreaBayEnd}'";
                }
                //if (Convert.ToInt32(source.Bank_X) == 2)
                SQL += $" ORDER BY ABS(CAST(BAY_Y AS INT)-{Convert.ToInt32(source.Bay_Y)}),";
                //else
                //    SQL += $" ORDER BY ABS(CAST(BAY_Y AS INT)-{Convert.ToInt32(source.Bay_Y)}) DESC,";
                if (Convert.ToInt32(source.Bank_X) == 2)
                    SQL += " BANK_X DESC,";
                else
                    SQL += " BANK_X,";
                SQL += $" ABS(CAST(LEVEL_Z AS INT)-{Convert.ToInt32(source.Level_Z)})";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    dest = new VShelfInfo();
                    dest.CarrierLoc = Convert.ToString(table.Rows[0]["CarrierLoc"]);
                    dest.ZoneID = Convert.ToString(table.Rows[0]["ZoneID"]);
                    dest.Bank_X = Convert.ToString(table.Rows[0]["Bank_X"]);
                    dest.Bay_Y = Convert.ToString(table.Rows[0]["Bay_Y"]);
                    dest.Level_Z = Convert.ToString(table.Rows[0]["Level_Z"]);
                    dest.LocateCraneNo = Convert.ToInt32(table.Rows[0]["LOCATECRANENO"]);
                    dest.Enable = Convert.ToString(table.Rows[0]["Enable"]) == "Y";
                    dest.EmptyBlockFlag = Convert.ToString(table.Rows[0]["EmptyBlockFlag"]) == "Y";
                    dest.HoldState = Convert.ToInt32(table.Rows[0]["HoldState"]);
                    dest.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRReadFlag"]) == "Y";
                    dest.ShelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                    dest.ShelfType = Convert.ToInt32(table.Rows[0]["SHELFTYPE"]);
                    dest.ChargeLoc = Convert.ToString(table.Rows[0]["ChargeLoc"]) == "Y";
                    dest.SelectPriority = Convert.ToInt32(table.Rows[0]["SELECTPRIORITY"]);
                    dest.ShelfState = Convert.ToChar(table.Rows[0]["ShelfState"]);
                    dest.HostEQPort = Convert.ToString(table.Rows[0]["HostEQPortID"]);
                    dest.Stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                    dest.Vehicles = Convert.ToInt32(table.Rows[0]["Vehicles"]);
                    dest.PortType = Convert.ToInt32(table.Rows[0]["PortType"]);
                    dest.PortLocationType = Convert.ToInt32(table.Rows[0]["PortLocationType"]);
                    dest.PLCPortID = Convert.ToInt32(table.Rows[0]["PLCPortID"]);
                    dest.PortTypeIndex = Convert.ToInt32(table.Rows[0]["PortTypeIndex"]);
                    dest.StageCount = Convert.ToInt32(table.Rows[0]["StageCount"]);
                    dest.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    dest.LotID = Convert.ToString(table.Rows[0]["LotID"]);
                    dest.EmptyCST = Convert.ToString(table.Rows[0]["EmptyCST"]);
                    dest.CSTType = Convert.ToString(table.Rows[0]["CSTType"]);
                    dest.CSTState = Convert.ToInt32(table.Rows[0]["CSTState"]);
                    dest.CSTInDT = Convert.ToString(table.Rows[0]["CSTInDT"]);
                    dest.StoreDT = Convert.ToString(table.Rows[0]["StoreDT"]);
                    return true;
                }
                dest = null;
                return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                dest = null;
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public bool GetAlternateShelf(DB _db, int craneNo, int bay, out VShelfInfo dest)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND LOCATECRANENO='{craneNo}'";
                SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                SQL += $" AND SHELFSTATE='{(char)ShelfState.EmptyShelf}'";
                SQL += $" AND ENABLE='{(char)ShelfEnable.Enable}'";
                SQL += $" ORDER BY ABS(CAST(BAY_Y AS INT)-{Convert.ToInt32(bay)})";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    dest = new VShelfInfo();
                    dest.CarrierLoc = Convert.ToString(table.Rows[0]["CarrierLoc"]);
                    dest.ZoneID = Convert.ToString(table.Rows[0]["ZoneID"]);
                    dest.Bank_X = Convert.ToString(table.Rows[0]["Bank_X"]);
                    dest.Bay_Y = Convert.ToString(table.Rows[0]["Bay_Y"]);
                    dest.Level_Z = Convert.ToString(table.Rows[0]["Level_Z"]);
                    dest.LocateCraneNo = Convert.ToInt32(table.Rows[0]["LOCATECRANENO"]);
                    dest.Enable = Convert.ToString(table.Rows[0]["Enable"]) == "Y";
                    dest.EmptyBlockFlag = Convert.ToString(table.Rows[0]["EmptyBlockFlag"]) == "Y";
                    dest.HoldState = Convert.ToInt32(table.Rows[0]["HoldState"]);
                    dest.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRReadFlag"]) == "Y";
                    dest.ShelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                    dest.ShelfType = Convert.ToInt32(table.Rows[0]["SHELFTYPE"]);
                    dest.ChargeLoc = Convert.ToString(table.Rows[0]["ChargeLoc"]) == "Y";
                    dest.SelectPriority = Convert.ToInt32(table.Rows[0]["SELECTPRIORITY"]);
                    dest.ShelfState = Convert.ToChar(table.Rows[0]["ShelfState"]);
                    dest.HostEQPort = Convert.ToString(table.Rows[0]["HostEQPortID"]);
                    dest.Stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                    dest.Vehicles = Convert.ToInt32(table.Rows[0]["Vehicles"]);
                    dest.PortType = Convert.ToInt32(table.Rows[0]["PortType"]);
                    dest.PortLocationType = Convert.ToInt32(table.Rows[0]["PortLocationType"]);
                    dest.PLCPortID = Convert.ToInt32(table.Rows[0]["PLCPortID"]);
                    dest.PortTypeIndex = Convert.ToInt32(table.Rows[0]["PortTypeIndex"]);
                    dest.StageCount = Convert.ToInt32(table.Rows[0]["StageCount"]);
                    dest.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    dest.LotID = Convert.ToString(table.Rows[0]["LotID"]);
                    dest.EmptyCST = Convert.ToString(table.Rows[0]["EmptyCST"]);
                    dest.CSTType = Convert.ToString(table.Rows[0]["CSTType"]);
                    dest.CSTState = Convert.ToInt32(table.Rows[0]["CSTState"]);
                    dest.CSTInDT = Convert.ToString(table.Rows[0]["CSTInDT"]);
                    dest.StoreDT = Convert.ToString(table.Rows[0]["StoreDT"]);
                    return true;
                }
                dest = null;
                return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                dest = null;
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public bool GetAlternatePort(DB _db, int craneNo, out VShelfInfo dest)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND LOCATECRANENO='{craneNo}'";
                SQL += $" AND SHELFTYPE='{(int)ShelfType.Port}'";
                SQL += $" AND PORTTYPE='{(int)PortType.IO}'";
                SQL += $" AND ENABLE='{(char)ShelfEnable.Enable}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    dest = new VShelfInfo();
                    dest.CarrierLoc = Convert.ToString(table.Rows[0]["CarrierLoc"]);
                    dest.ZoneID = Convert.ToString(table.Rows[0]["ZoneID"]);
                    dest.Bank_X = Convert.ToString(table.Rows[0]["Bank_X"]);
                    dest.Bay_Y = Convert.ToString(table.Rows[0]["Bay_Y"]);
                    dest.Level_Z = Convert.ToString(table.Rows[0]["Level_Z"]);
                    dest.LocateCraneNo = Convert.ToInt32(table.Rows[0]["LOCATECRANENO"]);
                    dest.Enable = Convert.ToString(table.Rows[0]["Enable"]) == "Y";
                    dest.EmptyBlockFlag = Convert.ToString(table.Rows[0]["EmptyBlockFlag"]) == "Y";
                    dest.HoldState = Convert.ToInt32(table.Rows[0]["HoldState"]);
                    dest.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRReadFlag"]) == "Y";
                    dest.ShelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                    dest.ShelfType = Convert.ToInt32(table.Rows[0]["SHELFTYPE"]);
                    dest.ChargeLoc = Convert.ToString(table.Rows[0]["ChargeLoc"]) == "Y";
                    dest.SelectPriority = Convert.ToInt32(table.Rows[0]["SELECTPRIORITY"]);
                    dest.ShelfState = Convert.ToChar(table.Rows[0]["ShelfState"]);
                    dest.HostEQPort = Convert.ToString(table.Rows[0]["HostEQPortID"]);
                    dest.Stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                    dest.Vehicles = Convert.ToInt32(table.Rows[0]["Vehicles"]);
                    dest.PortType = Convert.ToInt32(table.Rows[0]["PortType"]);
                    dest.PortLocationType = Convert.ToInt32(table.Rows[0]["PortLocationType"]);
                    dest.PLCPortID = Convert.ToInt32(table.Rows[0]["PLCPortID"]);
                    dest.PortTypeIndex = Convert.ToInt32(table.Rows[0]["PortTypeIndex"]);
                    dest.StageCount = Convert.ToInt32(table.Rows[0]["StageCount"]);
                    dest.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    dest.LotID = Convert.ToString(table.Rows[0]["LotID"]);
                    dest.EmptyCST = Convert.ToString(table.Rows[0]["EmptyCST"]);
                    dest.CSTType = Convert.ToString(table.Rows[0]["CSTType"]);
                    dest.CSTState = Convert.ToInt32(table.Rows[0]["CSTState"]);
                    dest.CSTInDT = Convert.ToString(table.Rows[0]["CSTInDT"]);
                    dest.StoreDT = Convert.ToString(table.Rows[0]["StoreDT"]);
                    return true;
                }
                dest = null;
                return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                dest = null;
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        #region GetShelfInfo

        public List<TwoShelf> GetTwoShelfInfoHandOff(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = $"select v1.SHELFID as left_fork, v2.SHELFID as right_fork from V_SHELF v1 " +
                              "left join V_SHELF v2 on v1.BAY_Y + 1 = v2.BAY_Y and v1.LEVEL_Z = v2.LEVEL_Z and v2.SHELFSTATE = 'N' " +
                              "where v1.Enable = 'Y' and v2.Enable = 'Y' and v1.SHELFTYPE = 1 and v2.SHELFTYPE = 1 and v1.SHELFSTATE = 'N' and v1.BANK_X = 1 and v2.BANK_X = 1 and  v1.ZONEID IN (SELECT ZONEID FROM ZONEDEF WHERE ZONETYPE='9') and v1.ZONEID = v2.ZONEID " +
                              "union all " +
                              "select v1.SHELFID as left_fork, v2.SHELFID as right_fork from V_SHELF v1 " +
                              "left join V_SHELF v2 on v1.BAY_Y = v2.BAY_Y + 1 and v1.LEVEL_Z = v2.LEVEL_Z and v2.SHELFSTATE = 'N' " +
                              "where v1.Enable = 'Y' and v2.Enable = 'Y' and v1.SHELFTYPE = 1 and v2.SHELFTYPE = 1 and v1.SHELFSTATE = 'N' and v1.BANK_X = 2 and v2.BANK_X = 2 and  v1.ZONEID IN (SELECT ZONEID FROM ZONEDEF WHERE ZONETYPE='9') and v1.ZONEID = v2.ZONEID ";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<TwoShelf> shelfInfos = new List<TwoShelf>();
                    for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                    {
                        TwoShelf info = new TwoShelf();
                        info.LeftForkShelfID = Convert.ToString(table.Rows[iRow]["left_fork"]);
                        info.RightForkShelfID = Convert.ToString(table.Rows[iRow]["right_fork"]);
                        shelfInfos.Add(info);
                    }
                    return shelfInfos;
                }
                else
                    return new List<TwoShelf>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<TwoShelf>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public List<TwoShelf> GetTwoShelfInfo(DB _db, IEnumerable<CommandTrace> commandTraces, IEnumerable<TwoShelf> shelfInfos, string Type)
        {
            var table = new DataTable();
            try
            {
                //取得現在Command中, 是否有相鄰的, 有的話就Add
                var twoShelves = new List<TwoShelf>();
                foreach (var item in commandTraces)
                {
                    if (!GetShelfInfo(_db, Type, item, out var shelfInfo))
                        continue;

                    //在所有有相鄰位置的 shelf 中, 找 LeftForkShelfID =  source.ShelfID
                    var LeftForkShelf = shelfInfos.Where(i => i.LeftForkShelfID == shelfInfo.ShelfID);
                    if (LeftForkShelf.Count() > 0)
                    {
                        using (var db = GetDB())
                        {
                            if (shelfInfo.ShelfType == (int)ShelfType.Port && Type == "Source")
                            {
                                int delay = _TaskInfo.Config.Refresh.CmdDelayTime;
                                if (delay >= 0)
                                {
                                    if (delay < 200)
                                    {
                                        delay = 200;
                                    }
                                    else if (delay > 5000)
                                    {
                                        delay = 5000;
                                    }
                                    SpinWait.SpinUntil(() => false, delay);
                                }
                            }

                            //找其他Command中, 有沒有 Source.ShelfID = RightForkShelfID
                            foreach (var item2 in GetCommandTrace(db).Where(i => i.CommandID != item.CommandID))
                            {
                                //確認 第二筆 的 ShelfInfo
                                if (!GetShelfInfo(_db, Type, item2, out var shelfInfo2))
                                    continue;

                                //取得右Fork的ShelfID
                                var RightForkShelfID = LeftForkShelf.FirstOrDefault().RightForkShelfID;

                                if (shelfInfo2.ShelfID == RightForkShelfID)
                                {
                                    var info = new TwoShelf
                                    {
                                        LeftForkShelfID = LeftForkShelf.FirstOrDefault().LeftForkShelfID,
                                        RightForkShelfID = LeftForkShelf.FirstOrDefault().RightForkShelfID
                                    };
                                    twoShelves.Add(info);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        //如果左Fork沒有找到 試著找 右Fork 的
                        var RightForkShelf = shelfInfos.Where(i => i.RightForkShelfID == shelfInfo.ShelfID);
                        if (RightForkShelf.Count() > 0)
                        {
                            using (var db = GetDB())
                            {
                                //找其他Command中, 有沒有 Source.ShelfID = RightForkShelfID
                                foreach (var item2 in GetCommandTrace(db).Where(i => i.CommandID != item.CommandID))
                                {
                                    //確認 第二筆 的 ShelfInfo
                                    if (!GetShelfInfo(_db, Type, item2, out var shelfInfo2))
                                        continue;

                                    //取得左Fork的ShelfID
                                    var LeftForkShelfID = RightForkShelf.FirstOrDefault().LeftForkShelfID;

                                    if (shelfInfo2.ShelfID == LeftForkShelfID)
                                    {
                                        var info = new TwoShelf
                                        {
                                            LeftForkShelfID = RightForkShelf.FirstOrDefault().LeftForkShelfID,
                                            RightForkShelfID = RightForkShelf.FirstOrDefault().RightForkShelfID
                                        };
                                        twoShelves.Add(info);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }

                return twoShelves;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<TwoShelf>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public List<TwoShelf> GetAllTwoShelfInfo()
        {
            var table = new DataTable();
            try
            {
                using (DB db = GetDB())
                {
                    //Bank1 union all Bank2
                    var SQL = $"select " +
                              $"CASE v1.SHELFTYPE " +
                              $"WHEN 1 THEN CONVERT(VARCHAR , v1.SHELFID) " +
                              $"WHEN 2 THEN CONVERT(VARCHAR, v1.PLCPORTID ) END as left_fork,  " +
                              $"CASE v2.SHELFTYPE " +
                              $"WHEN 1 THEN CONVERT(VARCHAR, v2.SHELFID) " +
                              $"WHEN 2 THEN CONVERT(VARCHAR, v2.PLCPORTID ) END as right_fork " +
                              $"from V_SHELF v1 " +
                              $"left join V_SHELF v2 on v1.BAY_Y + 1 = v2.BAY_Y and v1.LEVEL_Z = v2.LEVEL_Z " +
                              $"where v1.BANK_X = 1 and v2.BANK_X = 1 and((v1.shelfType = 1 and v1.ZONEID = v2.ZoneID) or(v1.shelfType = 2 and v1.shelfType = v2.shelfType))  and v1.stage = 1 and v2.stage = 1 and v1.ENABLE = 'Y' and v2.ENABLE = 'Y' " +
                              $"union all " +
                              $"select CASE v1.SHELFTYPE " +
                              $"WHEN 1 THEN CONVERT(VARCHAR , v1.SHELFID) " +
                              $"WHEN 2 THEN CONVERT(VARCHAR, v1.PLCPORTID ) END as left_fork, " +
                              $"CASE v2.SHELFTYPE " +
                              $"WHEN 1 THEN CONVERT(VARCHAR, v2.SHELFID) " +
                              $"WHEN 2 THEN CONVERT(VARCHAR, v2.PLCPORTID ) END as right_fork from V_SHELF v1 " +
                              $"left join V_SHELF v2 on v1.BAY_Y = v2.BAY_Y + 1 and v1.LEVEL_Z = v2.LEVEL_Z " +
                              $"where v1.BANK_X = 2 and v2.BANK_X = 2 and((v1.shelfType = 1 and v1.ZONEID = v2.ZoneID) or(v1.shelfType = 2 and v1.shelfType = v2.shelfType)) and v1.stage = 1 and v2.stage = 1 and v1.ENABLE = 'Y' and v2.ENABLE = 'Y'";

                    if (db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                        return GetAllShelfInfoPair(table);
                    else
                        return new List<TwoShelf>();
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<TwoShelf>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        private bool GetShelfInfo(DB _db, string Type, CommandTrace item, out VShelfInfo shelfInfo)
        {
            if (Type == "Source")
            {
                //確認Source的ShelfInfo
                if (!GetShelfInfoByShelfID(_db, item.NextSource, out shelfInfo))
                {
                    if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(item.NextSource), out shelfInfo))
                    {
                        return false;
                    }
                }
            }
            else
            {
                //確認Dest的ShelfInfo
                if (!GetShelfInfoByShelfID(_db, item.NextDest, out shelfInfo))
                {
                    if (!GetShelfInfoByPLCPortID(_db, Convert.ToInt32(item.NextDest), out shelfInfo))
                    {
                        return false;
                    }
                }
            }

            if (shelfInfo.ShelfType == (int)ShelfType.Crane)
            {
                return false;
            }

            return true;
        }


        private List<TwoShelf> GetAllShelfInfoPair(DataTable table)
        {
            List<TwoShelf> shelfInfos = new List<TwoShelf>();
            for (int iRow = 0; iRow < table.Rows.Count; iRow++)
            {
                TwoShelf info = new TwoShelf();
                info.LeftForkShelfID = Convert.ToString(table.Rows[iRow]["left_fork"]);
                info.RightForkShelfID = Convert.ToString(table.Rows[iRow]["right_fork"]);
                shelfInfos.Add(info);
            }
            return shelfInfos;
        }

        public IEnumerable<VShelfInfo> GetAllShelfInfoOnHandOff(DB _db, bool CSTIDisEmpty = false, ShelfState ShelfState = ShelfState.Stored, bool Enable = true)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND ZONEID IN (SELECT ZONEID FROM ZONEDEF WHERE ZONETYPE='9')";
                SQL += CSTIDisEmpty ? $" AND CSTID = ''" : $" AND CSTID <> ''";
                SQL += $" AND ShelfState='{(char)ShelfState}'";
                SQL += Enable ? $" AND Enable='Y'" : $" AND Enable != 'Y'";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<VShelfInfo> shelfInfos = new List<VShelfInfo>();
                    for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                    {
                        VShelfInfo info = new VShelfInfo();
                        info.CarrierLoc = Convert.ToString(table.Rows[iRow]["CarrierLoc"]);
                        info.ZoneID = Convert.ToString(table.Rows[iRow]["ZoneID"]);
                        info.Bank_X = Convert.ToString(table.Rows[iRow]["Bank_X"]);
                        info.Bay_Y = Convert.ToString(table.Rows[iRow]["Bay_Y"]);
                        info.Level_Z = Convert.ToString(table.Rows[iRow]["Level_Z"]);
                        info.LocateCraneNo = Convert.ToInt32(table.Rows[iRow]["LOCATECRANENO"]);
                        info.Enable = Convert.ToString(table.Rows[iRow]["Enable"]) == "Y";
                        info.EmptyBlockFlag = Convert.ToString(table.Rows[iRow]["EmptyBlockFlag"]) == "Y";
                        info.HoldState = Convert.ToInt32(table.Rows[iRow]["HoldState"]);
                        info.BCRReadFlag = Convert.ToString(table.Rows[iRow]["BCRReadFlag"]) == "Y";
                        info.ShelfID = Convert.ToString(table.Rows[iRow]["SHELFID"]);
                        info.ShelfType = Convert.ToInt32(table.Rows[iRow]["SHELFTYPE"]);
                        info.ChargeLoc = Convert.ToString(table.Rows[iRow]["ChargeLoc"]) == "Y";
                        info.SelectPriority = Convert.ToInt32(table.Rows[iRow]["SELECTPRIORITY"]);
                        info.ShelfState = Convert.ToChar(table.Rows[iRow]["ShelfState"]);
                        info.HostEQPort = Convert.ToString(table.Rows[iRow]["HostEQPortID"]);
                        info.Stage = Convert.ToInt32(table.Rows[iRow]["STAGE"]);
                        info.Vehicles = Convert.ToInt32(table.Rows[iRow]["Vehicles"]);
                        info.PortType = Convert.ToInt32(table.Rows[iRow]["PortType"]);
                        info.PortLocationType = Convert.ToInt32(table.Rows[iRow]["PortLocationType"]);
                        info.PLCPortID = Convert.ToInt32(table.Rows[iRow]["PLCPortID"]);
                        info.PortTypeIndex = Convert.ToInt32(table.Rows[iRow]["PortTypeIndex"]);
                        info.StageCount = Convert.ToInt32(table.Rows[iRow]["StageCount"]);
                        info.CSTID = Convert.ToString(table.Rows[iRow]["CSTID"]);
                        info.LotID = Convert.ToString(table.Rows[iRow]["LotID"]);
                        info.EmptyCST = Convert.ToString(table.Rows[iRow]["EmptyCST"]);
                        info.CSTType = Convert.ToString(table.Rows[iRow]["CSTType"]);
                        info.CSTState = Convert.ToInt32(table.Rows[iRow]["CSTState"]);
                        info.CSTInDT = Convert.ToString(table.Rows[iRow]["CSTInDT"]);
                        info.StoreDT = Convert.ToString(table.Rows[iRow]["StoreDT"]);
                        shelfInfos.Add(info);
                    }
                    return shelfInfos;
                }
                else
                    return new List<VShelfInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<VShelfInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public IEnumerable<VShelfInfo> GetShelfInfoLikeCarrierID(DB _db, string carrierID)
        {
            List<VShelfInfo> lstShelfDef = new List<VShelfInfo>();
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND CSTID LIKE '%{carrierID}%'";

                if (!string.IsNullOrEmpty(carrierID) && _db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<VShelfInfo> shelfInfos = new List<VShelfInfo>();
                    for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                    {
                        VShelfInfo info = new VShelfInfo();
                        info.CarrierLoc = Convert.ToString(table.Rows[iRow]["CarrierLoc"]);
                        info.ZoneID = Convert.ToString(table.Rows[iRow]["ZoneID"]);
                        info.Bank_X = Convert.ToString(table.Rows[iRow]["Bank_X"]);
                        info.Bay_Y = Convert.ToString(table.Rows[iRow]["Bay_Y"]);
                        info.Level_Z = Convert.ToString(table.Rows[iRow]["Level_Z"]);
                        info.LocateCraneNo = Convert.ToInt32(table.Rows[iRow]["LOCATECRANENO"]);
                        info.Enable = Convert.ToString(table.Rows[iRow]["Enable"]) == "Y";
                        info.EmptyBlockFlag = Convert.ToString(table.Rows[iRow]["EmptyBlockFlag"]) == "Y";
                        info.HoldState = Convert.ToInt32(table.Rows[iRow]["HoldState"]);
                        info.BCRReadFlag = Convert.ToString(table.Rows[iRow]["BCRReadFlag"]) == "Y";
                        info.ShelfID = Convert.ToString(table.Rows[iRow]["SHELFID"]);
                        info.ShelfType = Convert.ToInt32(table.Rows[iRow]["SHELFTYPE"]);
                        info.ChargeLoc = Convert.ToString(table.Rows[iRow]["ChargeLoc"]) == "Y";
                        info.SelectPriority = Convert.ToInt32(table.Rows[iRow]["SELECTPRIORITY"]);
                        info.ShelfState = Convert.ToChar(table.Rows[iRow]["ShelfState"]);
                        info.HostEQPort = Convert.ToString(table.Rows[iRow]["HostEQPortID"]);
                        info.Stage = Convert.ToInt32(table.Rows[iRow]["STAGE"]);
                        info.Vehicles = Convert.ToInt32(table.Rows[iRow]["Vehicles"]);
                        info.PortType = Convert.ToInt32(table.Rows[iRow]["PortType"]);
                        info.PortLocationType = Convert.ToInt32(table.Rows[iRow]["PortLocationType"]);
                        info.PLCPortID = Convert.ToInt32(table.Rows[iRow]["PLCPortID"]);
                        info.PortTypeIndex = Convert.ToInt32(table.Rows[iRow]["PortTypeIndex"]);
                        info.StageCount = Convert.ToInt32(table.Rows[iRow]["StageCount"]);
                        info.CSTID = Convert.ToString(table.Rows[iRow]["CSTID"]);
                        info.LotID = Convert.ToString(table.Rows[iRow]["LotID"]);
                        info.EmptyCST = Convert.ToString(table.Rows[iRow]["EmptyCST"]);
                        info.CSTType = Convert.ToString(table.Rows[iRow]["CSTType"]);
                        info.CSTState = Convert.ToInt32(table.Rows[iRow]["CSTState"]);
                        info.CSTInDT = Convert.ToString(table.Rows[iRow]["CSTInDT"]);
                        info.StoreDT = Convert.ToString(table.Rows[iRow]["StoreDT"]);
                        shelfInfos.Add(info);
                    }
                    return shelfInfos;
                }
                else
                    return new List<VShelfInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<VShelfInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        public IEnumerable<VShelfInfo> GetAllShelfInfo(DB _db) => GetAllShelfInfo(_db, string.Empty, string.Empty, string.Empty, 0, string.Empty);
        public IEnumerable<VShelfInfo> GetAllShelfInfoByHostEQPortID(DB _db, string hostEQPortID) => GetAllShelfInfo(_db, string.Empty, string.Empty, hostEQPortID, 0, string.Empty);
        public IEnumerable<VShelfInfo> GetAllShelfInfoByHostEQPortID(string hostEQPortID) => GetAllShelfInfo(string.Empty, string.Empty, hostEQPortID, 0, string.Empty);
        public IEnumerable<VShelfInfo> GetAllShelfInfoByShelfID(DB _db, string shelfID) => GetAllShelfInfo(_db, string.Empty, shelfID, string.Empty, 0, string.Empty);
        public IEnumerable<VShelfInfo> GetAllShelfInfoByCarrierLoc(DB _db, string carrierLoc) => GetAllShelfInfo(_db, carrierLoc, string.Empty, string.Empty, 0, string.Empty);
        public IEnumerable<VShelfInfo> GetAllShelfInfoByZoneID(DB _db, string zoneID) => GetAllShelfInfo(_db, string.Empty, string.Empty, string.Empty, 0, zoneID);
        public IEnumerable<VShelfInfo> GetAllShelfInfoByPLCPortID(DB _db, int plcPortID) => GetAllShelfInfo(_db, string.Empty, string.Empty, string.Empty, plcPortID, string.Empty);
        public bool GetShelfInfoByHostEQPortID(DB _db, string hostEQPortID, string carrierID, out VShelfInfo info) => (info = GetShelfInfo(_db, string.Empty, string.Empty, hostEQPortID, 0, string.Empty, carrierID)) != null;
        public bool GetShelfInfoByHostEQPortID(string hostEQPortID, string carrierID, out VShelfInfo info) => (info = GetShelfInfo(string.Empty, string.Empty, hostEQPortID, 0, string.Empty, carrierID)) != null;
        public bool GetShelfInfoByHostEQPortID(DB _db, string hostEQPortID, out VShelfInfo info) => (info = GetShelfInfo(_db, string.Empty, string.Empty, hostEQPortID, 0, string.Empty, string.Empty)) != null;
        public bool GetShelfInfoByShelfID(DB _db, string shelfID, string carrierID, out VShelfInfo info) => (info = GetShelfInfo(_db, string.Empty, shelfID, string.Empty, 0, string.Empty, carrierID)) != null;
        public bool GetShelfInfoByShelfID(DB _db, string shelfID, out VShelfInfo info) => (info = GetShelfInfo(_db, string.Empty, shelfID, string.Empty, 0, string.Empty, string.Empty)) != null;
        public bool GetShelfInfoByCarrierLoc(DB _db, string carrierLoc, string carrierID, out VShelfInfo info) => (info = GetShelfInfo(_db, carrierLoc, string.Empty, string.Empty, 0, string.Empty, carrierID)) != null;
        public bool GetShelfInfoByCarrierLoc(DB _db, string carrierLoc, out VShelfInfo info) => (info = GetShelfInfo(_db, carrierLoc, string.Empty, string.Empty, 0, string.Empty, string.Empty)) != null;
        public bool GetShelfInfoByZoneID(DB _db, string zoneID, string carrierID, out VShelfInfo info) => (info = GetShelfInfo(_db, string.Empty, string.Empty, string.Empty, 0, zoneID, carrierID)) != null;
        public bool GetShelfInfoByZoneID(DB _db, string zoneID, out VShelfInfo info) => (info = GetShelfInfo(_db, string.Empty, string.Empty, string.Empty, 0, zoneID, string.Empty)) != null;
        public bool GetShelfInfoByCarrierID(DB _db, string carrierID, out VShelfInfo info) => (info = GetShelfInfo(_db, string.Empty, string.Empty, string.Empty, 0, string.Empty, carrierID)) != null;
        public bool GetShelfInfoByPLCPortID(DB _db, int plcPortID, out VShelfInfo info) => (info = GetShelfInfo(_db, string.Empty, string.Empty, string.Empty, plcPortID, string.Empty, string.Empty)) != null;
        public VShelfInfo GetShelfInfoByHostEQPortID(DB _db, string hostEQPortID, string carrierID) => GetShelfInfo(_db, string.Empty, string.Empty, hostEQPortID, 0, string.Empty, carrierID);
        public VShelfInfo GetShelfInfoByHostEQPortID(DB _db, string hostEQPortID) => GetShelfInfo(_db, string.Empty, string.Empty, hostEQPortID, 0, string.Empty, string.Empty);
        public VShelfInfo GetShelfInfoByHostEQPortID(string hostEQPortID) => GetShelfInfo(string.Empty, string.Empty, hostEQPortID, 0, string.Empty, string.Empty);
        public VShelfInfo GetShelfInfoByShelfID(DB _db, string shelfID, string carrierID) => GetShelfInfo(_db, string.Empty, shelfID, string.Empty, 0, string.Empty, carrierID);
        public VShelfInfo GetShelfInfoByShelfID(string shelfID) => GetShelfInfo(string.Empty, shelfID, string.Empty, 0, string.Empty, string.Empty);
        public VShelfInfo GetShelfInfoByShelfID(DB _db, string shelfID) => GetShelfInfo(_db, string.Empty, shelfID, string.Empty, 0, string.Empty, string.Empty);
        public VShelfInfo GetShelfInfoByCarrierLoc(DB _db, string carrierLoc, string carrierID) => GetShelfInfo(_db, carrierLoc, string.Empty, string.Empty, 0, string.Empty, carrierID);
        public VShelfInfo GetShelfInfoByCarrierLoc(DB _db, string carrierLoc) => GetShelfInfo(_db, carrierLoc, string.Empty, string.Empty, 0, string.Empty, string.Empty);
        public VShelfInfo GetShelfInfoByZoneID(DB _db, string zoneID, string carrierID) => GetShelfInfo(_db, string.Empty, string.Empty, string.Empty, 0, zoneID, carrierID);
        public VShelfInfo GetShelfInfoByZoneID(DB _db, string zoneID) => GetShelfInfo(_db, string.Empty, string.Empty, string.Empty, 0, zoneID, string.Empty);
        public VShelfInfo GetShelfInfoByCarrierID(DB _db, string carrierID) => GetShelfInfo(_db, string.Empty, string.Empty, string.Empty, 0, string.Empty, carrierID);
        public VShelfInfo GetShelfInfoByPLCPortID(DB _db, int plcPortID) => GetShelfInfo(_db, string.Empty, string.Empty, string.Empty, plcPortID, string.Empty, string.Empty);
        public VShelfInfo GetShelfInfoByPLCPortID(int plcPortID) => GetShelfInfo(string.Empty, string.Empty, string.Empty, plcPortID, string.Empty, string.Empty);
        private VShelfInfo GetShelfInfo(DB _db, string carrierLoc, string shelfID, string hostEQPortID, int plcPortID, string zoneID, string carrierID)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                if (!string.IsNullOrWhiteSpace(carrierLoc))
                {
                    SQL += $" AND CARRIERLOC='{carrierLoc}'";
                    SQL += " AND STAGE='1'";
                }
                else if (!string.IsNullOrWhiteSpace(shelfID))
                {
                    SQL += $" AND SHELFID='{shelfID}'";
                    SQL += " AND STAGE='1'";
                }
                else if (!string.IsNullOrWhiteSpace(hostEQPortID))
                {
                    SQL += $" AND HOSTEQPORTID like '{hostEQPortID}%'";
                    SQL += " AND STAGE='1'";
                }
                else if (!string.IsNullOrWhiteSpace(zoneID))
                {
                    SQL += $" AND ZONEID='{zoneID}'";
                    SQL += " AND STAGE='1'";
                }
                else if (plcPortID > 0)
                {
                    SQL += $" AND PLCPORTID='{plcPortID}'";
                    SQL += " AND STAGE='1'";
                }
                if (!string.IsNullOrWhiteSpace(carrierID))
                    SQL += $" AND CSTID='{carrierID}'";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    VShelfInfo info = new VShelfInfo();
                    info.CarrierLoc = Convert.ToString(table.Rows[0]["CarrierLoc"]);
                    info.ZoneID = Convert.ToString(table.Rows[0]["ZoneID"]);
                    info.Bank_X = Convert.ToString(table.Rows[0]["Bank_X"]);
                    info.Bay_Y = Convert.ToString(table.Rows[0]["Bay_Y"]);
                    info.Level_Z = Convert.ToString(table.Rows[0]["Level_Z"]);
                    info.LocateCraneNo = Convert.ToInt32(table.Rows[0]["LOCATECRANENO"]);
                    info.Enable = Convert.ToString(table.Rows[0]["Enable"]) == "Y";
                    info.EmptyBlockFlag = Convert.ToString(table.Rows[0]["EmptyBlockFlag"]) == "Y";
                    info.HoldState = Convert.ToInt32(table.Rows[0]["HoldState"]);
                    info.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRReadFlag"]) == "Y";
                    info.ShelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                    info.ShelfType = Convert.ToInt32(table.Rows[0]["SHELFTYPE"]);
                    info.ChargeLoc = Convert.ToString(table.Rows[0]["ChargeLoc"]) == "Y";
                    info.SelectPriority = Convert.ToInt32(table.Rows[0]["SELECTPRIORITY"]);
                    info.ShelfState = Convert.ToChar(table.Rows[0]["ShelfState"]);
                    info.HostEQPort = Convert.ToString(table.Rows[0]["HostEQPortID"]);
                    info.Stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                    info.Vehicles = Convert.ToInt32(table.Rows[0]["Vehicles"]);
                    info.PortType = Convert.ToInt32(table.Rows[0]["PortType"]);
                    info.PortLocationType = Convert.ToInt32(table.Rows[0]["PortLocationType"]);
                    info.PLCPortID = Convert.ToInt32(table.Rows[0]["PLCPORTID"]);
                    info.PortTypeIndex = Convert.ToInt32(table.Rows[0]["PortTypeIndex"]);
                    info.StageCount = Convert.ToInt32(table.Rows[0]["StageCount"]);
                    info.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                    info.LotID = Convert.ToString(table.Rows[0]["LotID"]);
                    info.EmptyCST = Convert.ToString(table.Rows[0]["EmptyCST"]);
                    info.CSTType = Convert.ToString(table.Rows[0]["CSTType"]);
                    info.CSTState = Convert.ToInt32(table.Rows[0]["CSTState"]);
                    info.CSTInDT = Convert.ToString(table.Rows[0]["CSTInDT"]);
                    info.StoreDT = Convert.ToString(table.Rows[0]["StoreDT"]);
                    return info;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return null;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        private VShelfInfo GetShelfInfo(string carrierLoc, string shelfID, string hostEQPortID, int plcPortID, string zoneID, string carrierID)
        {
            using (DB _db = GetDB())
            {
                DataTable table = new DataTable();
                try
                {
                    string SQL = "SELECT * FROM V_SHELF ";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    if (!string.IsNullOrWhiteSpace(carrierLoc))
                    {
                        SQL += $" AND CARRIERLOC='{carrierLoc}'";
                        SQL += " AND STAGE='1'";
                    }
                    else if (!string.IsNullOrWhiteSpace(shelfID))
                    {
                        SQL += $" AND SHELFID='{shelfID}'";
                        SQL += " AND STAGE='1'";
                    }
                    else if (!string.IsNullOrWhiteSpace(hostEQPortID))
                    {
                        SQL += $" AND HOSTEQPORTID like '{hostEQPortID}%'";
                        SQL += " AND STAGE='1'";
                    }
                    else if (!string.IsNullOrWhiteSpace(zoneID))
                    {
                        SQL += $" AND ZONEID='{zoneID}'";
                        SQL += " AND STAGE='1'";
                    }
                    else if (plcPortID > 0)
                    {
                        SQL += $" AND PLCPORTID='{plcPortID}'";
                        SQL += " AND STAGE='1'";
                    }
                    if (!string.IsNullOrWhiteSpace(carrierID))
                        SQL += $" AND CSTID='{carrierID}'";

                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        VShelfInfo info = new VShelfInfo();
                        info.CarrierLoc = Convert.ToString(table.Rows[0]["CarrierLoc"]);
                        info.ZoneID = Convert.ToString(table.Rows[0]["ZoneID"]);
                        info.Bank_X = Convert.ToString(table.Rows[0]["Bank_X"]);
                        info.Bay_Y = Convert.ToString(table.Rows[0]["Bay_Y"]);
                        info.Level_Z = Convert.ToString(table.Rows[0]["Level_Z"]);
                        info.LocateCraneNo = Convert.ToInt32(table.Rows[0]["LOCATECRANENO"]);
                        info.Enable = Convert.ToString(table.Rows[0]["Enable"]) == "Y";
                        info.EmptyBlockFlag = Convert.ToString(table.Rows[0]["EmptyBlockFlag"]) == "Y";
                        info.HoldState = Convert.ToInt32(table.Rows[0]["HoldState"]);
                        info.BCRReadFlag = Convert.ToString(table.Rows[0]["BCRReadFlag"]) == "Y";
                        info.ShelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                        info.ShelfType = Convert.ToInt32(table.Rows[0]["SHELFTYPE"]);
                        info.ChargeLoc = Convert.ToString(table.Rows[0]["ChargeLoc"]) == "Y";
                        info.SelectPriority = Convert.ToInt32(table.Rows[0]["SELECTPRIORITY"]);
                        info.ShelfState = Convert.ToChar(table.Rows[0]["ShelfState"]);
                        info.HostEQPort = Convert.ToString(table.Rows[0]["HostEQPortID"]);
                        info.Stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                        info.Vehicles = Convert.ToInt32(table.Rows[0]["Vehicles"]);
                        info.PortType = Convert.ToInt32(table.Rows[0]["PortType"]);
                        info.PortLocationType = Convert.ToInt32(table.Rows[0]["PortLocationType"]);
                        info.PLCPortID = Convert.ToInt32(table.Rows[0]["PLCPORTID"]);
                        info.PortTypeIndex = Convert.ToInt32(table.Rows[0]["PortTypeIndex"]);
                        info.StageCount = Convert.ToInt32(table.Rows[0]["StageCount"]);
                        info.CSTID = Convert.ToString(table.Rows[0]["CSTID"]);
                        info.LotID = Convert.ToString(table.Rows[0]["LotID"]);
                        info.EmptyCST = Convert.ToString(table.Rows[0]["EmptyCST"]);
                        info.CSTType = Convert.ToString(table.Rows[0]["CSTType"]);
                        info.CSTState = Convert.ToInt32(table.Rows[0]["CSTState"]);
                        info.CSTInDT = Convert.ToString(table.Rows[0]["CSTInDT"]);
                        info.StoreDT = Convert.ToString(table.Rows[0]["StoreDT"]);
                        return info;
                    }
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                    return null;
                }
                finally
                {
                    table?.Clear();
                    table?.Dispose();
                }
            }
        }
        private IEnumerable<VShelfInfo> GetAllShelfInfo(DB _db, string carrierLoc, string shelfID, string hostEQPortID, int plcPortID, string zoneID)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_SHELF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                if (!string.IsNullOrWhiteSpace(carrierLoc))
                {
                    SQL += $" AND CARRIERLOC='{carrierLoc}'";
                }
                if (!string.IsNullOrWhiteSpace(shelfID))
                {
                    SQL += $" AND SHELFID='{shelfID}'";
                    SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                }
                else if (!string.IsNullOrWhiteSpace(hostEQPortID))
                {
                    SQL += $" AND HOSTEQPORTID like '{hostEQPortID}%'";
                    SQL += $" AND SHELFTYPE='{(int)ShelfType.Port}'";
                }
                else if (!string.IsNullOrWhiteSpace(carrierLoc))
                    SQL += $" AND CARRIERLOC='{carrierLoc}'";
                else if (!string.IsNullOrWhiteSpace(zoneID))
                    SQL += $" AND ZONEID='{zoneID}'";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<VShelfInfo> shelfInfos = new List<VShelfInfo>();
                    for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                    {
                        VShelfInfo info = new VShelfInfo();
                        info.CarrierLoc = Convert.ToString(table.Rows[iRow]["CarrierLoc"]);
                        info.ZoneID = Convert.ToString(table.Rows[iRow]["ZoneID"]);
                        info.Bank_X = Convert.ToString(table.Rows[iRow]["Bank_X"]);
                        info.Bay_Y = Convert.ToString(table.Rows[iRow]["Bay_Y"]);
                        info.Level_Z = Convert.ToString(table.Rows[iRow]["Level_Z"]);
                        info.LocateCraneNo = Convert.ToInt32(table.Rows[iRow]["LOCATECRANENO"]);
                        info.Enable = Convert.ToString(table.Rows[iRow]["Enable"]) == "Y";
                        info.EmptyBlockFlag = Convert.ToString(table.Rows[iRow]["EmptyBlockFlag"]) == "Y";
                        info.HoldState = Convert.ToInt32(table.Rows[iRow]["HoldState"]);
                        info.BCRReadFlag = Convert.ToString(table.Rows[iRow]["BCRReadFlag"]) == "Y";
                        info.ShelfID = Convert.ToString(table.Rows[iRow]["SHELFID"]);
                        info.ShelfType = Convert.ToInt32(table.Rows[iRow]["SHELFTYPE"]);
                        info.ChargeLoc = Convert.ToString(table.Rows[iRow]["ChargeLoc"]) == "Y";
                        info.SelectPriority = Convert.ToInt32(table.Rows[iRow]["SELECTPRIORITY"]);
                        info.ShelfState = Convert.ToChar(table.Rows[iRow]["ShelfState"]);
                        info.HostEQPort = Convert.ToString(table.Rows[iRow]["HostEQPortID"]);
                        info.Stage = Convert.ToInt32(table.Rows[iRow]["STAGE"]);
                        info.Vehicles = Convert.ToInt32(table.Rows[iRow]["Vehicles"]);
                        info.PortType = Convert.ToInt32(table.Rows[iRow]["PortType"]);
                        info.PortLocationType = Convert.ToInt32(table.Rows[iRow]["PortLocationType"]);
                        info.PLCPortID = Convert.ToInt32(table.Rows[iRow]["PLCPortID"]);
                        info.PortTypeIndex = Convert.ToInt32(table.Rows[iRow]["PortTypeIndex"]);
                        info.StageCount = Convert.ToInt32(table.Rows[iRow]["StageCount"]);
                        info.CSTID = Convert.ToString(table.Rows[iRow]["CSTID"]);
                        info.LotID = Convert.ToString(table.Rows[iRow]["LotID"]);
                        info.EmptyCST = Convert.ToString(table.Rows[iRow]["EmptyCST"]);
                        info.CSTType = Convert.ToString(table.Rows[iRow]["CSTType"]);
                        info.CSTState = Convert.ToInt32(table.Rows[iRow]["CSTState"]);
                        info.CSTInDT = Convert.ToString(table.Rows[iRow]["CSTInDT"]);
                        info.StoreDT = Convert.ToString(table.Rows[iRow]["StoreDT"]);
                        shelfInfos.Add(info);
                    }
                    return shelfInfos;
                }
                else
                    return new List<VShelfInfo>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<VShelfInfo>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }
        private IEnumerable<VShelfInfo> GetAllShelfInfo(string carrierLoc, string shelfID, string hostEQPortID, int plcPortID, string zoneID)
        {
            using (DB _db = GetDB())
            {
                DataTable table = new DataTable();
                try
                {
                    string SQL = "SELECT * FROM V_SHELF ";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    if (!string.IsNullOrWhiteSpace(carrierLoc))
                    {
                        SQL += $" AND CARRIERLOC='{carrierLoc}'";
                    }
                    if (!string.IsNullOrWhiteSpace(shelfID))
                    {
                        SQL += $" AND SHELFID='{shelfID}'";
                        SQL += $" AND SHELFTYPE='{(int)ShelfType.Shelf}'";
                    }
                    else if (!string.IsNullOrWhiteSpace(hostEQPortID))
                    {
                        SQL += $" AND HOSTEQPORTID like '{hostEQPortID}%'";
                        SQL += $" AND SHELFTYPE='{(int)ShelfType.Port}'";
                    }
                    else if (!string.IsNullOrWhiteSpace(carrierLoc))
                        SQL += $" AND CARRIERLOC='{carrierLoc}'";
                    else if (!string.IsNullOrWhiteSpace(zoneID))
                        SQL += $" AND ZONEID='{zoneID}'";

                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        List<VShelfInfo> shelfInfos = new List<VShelfInfo>();
                        for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                        {
                            VShelfInfo info = new VShelfInfo();
                            info.CarrierLoc = Convert.ToString(table.Rows[iRow]["CarrierLoc"]);
                            info.ZoneID = Convert.ToString(table.Rows[iRow]["ZoneID"]);
                            info.Bank_X = Convert.ToString(table.Rows[iRow]["Bank_X"]);
                            info.Bay_Y = Convert.ToString(table.Rows[iRow]["Bay_Y"]);
                            info.Level_Z = Convert.ToString(table.Rows[iRow]["Level_Z"]);
                            info.LocateCraneNo = Convert.ToInt32(table.Rows[iRow]["LOCATECRANENO"]);
                            info.Enable = Convert.ToString(table.Rows[iRow]["Enable"]) == "Y";
                            info.EmptyBlockFlag = Convert.ToString(table.Rows[iRow]["EmptyBlockFlag"]) == "Y";
                            info.HoldState = Convert.ToInt32(table.Rows[iRow]["HoldState"]);
                            info.BCRReadFlag = Convert.ToString(table.Rows[iRow]["BCRReadFlag"]) == "Y";
                            info.ShelfID = Convert.ToString(table.Rows[iRow]["SHELFID"]);
                            info.ShelfType = Convert.ToInt32(table.Rows[iRow]["SHELFTYPE"]);
                            info.ChargeLoc = Convert.ToString(table.Rows[iRow]["ChargeLoc"]) == "Y";
                            info.SelectPriority = Convert.ToInt32(table.Rows[iRow]["SELECTPRIORITY"]);
                            info.ShelfState = Convert.ToChar(table.Rows[iRow]["ShelfState"]);
                            info.HostEQPort = Convert.ToString(table.Rows[iRow]["HostEQPortID"]);
                            info.Stage = Convert.ToInt32(table.Rows[iRow]["STAGE"]);
                            info.Vehicles = Convert.ToInt32(table.Rows[iRow]["Vehicles"]);
                            info.PortType = Convert.ToInt32(table.Rows[iRow]["PortType"]);
                            info.PortLocationType = Convert.ToInt32(table.Rows[iRow]["PortLocationType"]);
                            info.PLCPortID = Convert.ToInt32(table.Rows[iRow]["PLCPortID"]);
                            info.PortTypeIndex = Convert.ToInt32(table.Rows[iRow]["PortTypeIndex"]);
                            info.StageCount = Convert.ToInt32(table.Rows[iRow]["StageCount"]);
                            info.CSTID = Convert.ToString(table.Rows[iRow]["CSTID"]);
                            info.LotID = Convert.ToString(table.Rows[iRow]["LotID"]);
                            info.EmptyCST = Convert.ToString(table.Rows[iRow]["EmptyCST"]);
                            info.CSTType = Convert.ToString(table.Rows[iRow]["CSTType"]);
                            info.CSTState = Convert.ToInt32(table.Rows[iRow]["CSTState"]);
                            info.CSTInDT = Convert.ToString(table.Rows[iRow]["CSTInDT"]);
                            info.StoreDT = Convert.ToString(table.Rows[iRow]["StoreDT"]);
                            shelfInfos.Add(info);
                        }
                        return shelfInfos;
                    }
                    else
                        return new List<VShelfInfo>();
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                    return new List<VShelfInfo>();
                }
                finally
                {
                    table?.Clear();
                    table?.Dispose();
                }
            }
        }
        #endregion GetShelfInfo

        public bool ExistsCarrier(DB _db, string carrierID, out string shelfID, out int stage)
        {
            DataTable table = new DataTable();
            shelfID = string.Empty;
            stage = 0;
            try
            {
                string SQL = "SELECT * FROM CASSETTEDATA";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND CSTID='{carrierID}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    shelfID = Convert.ToString(table.Rows[0]["SHELFID"]);
                    stage = Convert.ToInt32(table.Rows[0]["STAGE"]);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                shelfID = string.Empty;
                stage = 0;
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        #region Update/Insert/Delete CarrierData
        public int InsertCarrierDataToHistory(DB _db, string carrierID)
        {
            if (string.IsNullOrWhiteSpace(carrierID))
                return ErrorCode.Exception;

            string SQL = "INSERT INTO HISCASSETTEDATA";
            SQL += $" SELECT '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', * FROM CASSETTEDATA";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND CSTID ='{carrierID}'";
            return _db.ExecuteSQL(SQL);
        }
        public int InsertCarrierDataToHistory(DB _db, string carrierID, string shelfID, ref string strEM)
        {
            if (string.IsNullOrWhiteSpace(carrierID))
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(shelfID))
                return ErrorCode.Exception;

            string SQL = "INSERT INTO HISCASSETTEDATA";
            SQL += $" SELECT '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', * FROM CASSETTEDATA";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND CSTID ='{carrierID}'";
            SQL += $" AND SHELFID='{shelfID}'";
            return _db.ExecuteSQL(SQL, ref strEM);
        }
        public int DeleteCarrierData(DB _db, string carrierID, string shelfID, ref string strEM)
        {
            if (string.IsNullOrWhiteSpace(carrierID))
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(shelfID))
                return ErrorCode.Exception;

            string SQL = "DELETE FROM CASSETTEDATA";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND CSTID ='{carrierID}'";
            SQL += $" AND SHELFID='{shelfID}'";
            return _db.ExecuteSQL(SQL, ref strEM);
        }

        public int InsertCarrierData(DB _db, string carrierID, string shelfID, CarrierState carrierState, ref string strEM)
            => InsertCarrierData(_db, carrierID, shelfID, 1, carrierState, ref strEM);
        public int InsertCarrierData(DB _db, string carrierID, string shelfID, int stage, CarrierState carrierState, ref string strEM)
        {
            if (string.IsNullOrWhiteSpace(carrierID))
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(shelfID))
                return ErrorCode.Exception;
            if (stage <= 0)
                return ErrorCode.Exception;

            string SQL = "INSERT INTO CASSETTEDATA (STOCKERID ,CSTID ,SHELFID ,STAGE ,CSTSTATE, CSTTYPE, CSTINDT, STOREDT ,WAITOUTOPDT ,WAITOUTLPDT ,TRNDT) VALUES (";
            SQL += $" '{_stockerId}',";
            SQL += $" '{carrierID}',";
            SQL += $" '{shelfID}',";
            SQL += $" '{stage}',";
            SQL += $" '{(carrierState == CarrierState.WaitOutLP ? (int)CarrierState.WaitOut : (int)carrierState)}', ";
            SQL += $" '', ";
            SQL += $" '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',";
            if (carrierState == CarrierState.None ||
                carrierState == CarrierState.Transfering ||
                carrierState == CarrierState.StoreCompleted ||
                carrierState == CarrierState.StoreAlternate ||
                carrierState == CarrierState.Installed)
            {
                SQL += $" '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',";
            }
            else
                SQL += " '',";
            if (carrierState == CarrierState.WaitOut)
                SQL += $" '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',";
            else
                SQL += " '',";
            if (carrierState == CarrierState.WaitOutLP)
                SQL += $" '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',";
            else
                SQL += " '',";
            SQL += $" '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL, ref strEM);
        }

        public int UpdateCarrierData(DB _db, string carrierID, string newShelfID, int newStage, CarrierState carrierState, ref string strEM)
        {
            if (string.IsNullOrWhiteSpace(carrierID))
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(newShelfID))
                return ErrorCode.Exception;
            if (newStage <= 0)
                return ErrorCode.Exception;

            string SQL = "UPDATE CASSETTEDATA SET";
            SQL += $" SHELFID='{newShelfID}',";
            SQL += $" STAGE='{newStage.ToString()}',";
            if (carrierState != CarrierState.None)
            {
                if (carrierState == CarrierState.WaitIn)
                {
                    SQL += $" CSTSTATE='{(int)carrierState}',";
                    SQL += $" CSTINDT='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',";
                }
                else if (carrierState == CarrierState.WaitOut)
                {
                    SQL += $" CSTSTATE='{(int)carrierState}',";
                    SQL += $" WAITOUTOPDT='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} ',";
                }
                else if (carrierState == CarrierState.WaitOutLP)
                {
                    SQL += $" CSTSTATE='{(int)CarrierState.WaitOut}',";
                    SQL += $" WAITOUTLPDT='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} ',";
                }
                else
                {
                    SQL += $" CSTSTATE='{(int)carrierState}',";
                    SQL += $" STOREDT='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',";
                }
            }
            SQL += $" TRNDT='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND CSTID ='{carrierID}'";
            int iResult = _db.ExecuteSQL(SQL, ref strEM);
            if (iResult == ErrorCode.NoDataUpdate)
                return InsertCarrierData(_db, carrierID, newShelfID, newStage, carrierState, ref strEM);
            else
                return iResult;
        }

        public int UpdateCarrierData(DB _db, string carrierID, string newShelfID, CarrierState carrierState, ref string strEM) => UpdateCarrierData(_db, carrierID, newShelfID, 1, carrierState, ref strEM);
        #endregion Update/Insert/Delete CarrierData

        #endregion ShelfDef/CarrierData

        public ZoneCapacitys GetZoneCapacity(DB _db, string zoneID)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_ZONECAPACITY";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND ZONEID='{zoneID}'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    ZoneCapacitys capacitys = new ZoneCapacitys();
                    capacitys.ZoneID = Convert.ToString(table.Rows[0]["ZONEID"]);
                    capacitys.ZoneType = Convert.ToInt32(table.Rows[0]["ZONETYPE"]);
                    capacitys.ZoneCapacity = Convert.ToInt32(table.Rows[0]["ZONECAPACITY"]);
                    capacitys.TotalZoneSize = Convert.ToInt32(table.Rows[0]["TOTALZONESIZE"]);
                    capacitys.ZoneSize = Convert.ToInt32(table.Rows[0]["ZONESIZE"]);
                    return capacitys;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return null;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public string GetAbnormalCSTID(AbnormalCSTIDType abnormalCSTIDType, string hostEQPort, string CarrierID, string ShelfID = "")
        {
            string strNewCSTID = string.Empty;
            string strTrnDate = DateTime.Now.ToString("yyMMddHHmmssfff");

            if (abnormalCSTIDType == AbnormalCSTIDType.Failure && (CarrierID.StartsWith("UNKNOWN-") || CarrierID.Contains("UNKNOWNDUP") || CarrierID.Contains("UNKNOWNDBS") || CarrierID.Contains("UNKNOWNEMP")))
                return CarrierID;
            else
            {
                switch (abnormalCSTIDType)
                {
                    case AbnormalCSTIDType.Failure:
                        var oldCST = string.IsNullOrWhiteSpace(CarrierID) ? "000000" : CarrierID;
                        strNewCSTID = $"UNKNOWN-" + oldCST + "-" + hostEQPort + "-" + strTrnDate;
                        break;

                    case AbnormalCSTIDType.Duplicate:
                        strNewCSTID = $"UNKNOWNDUP-" + CarrierID + "-" + strTrnDate;
                        break;
                    case AbnormalCSTIDType.DoubleStorage:
                        strNewCSTID = $"UNKNOWNDBS-" + _stockerId + ShelfID + "-" + strTrnDate;
                        break;
                    case AbnormalCSTIDType.EmptyRetrieval:
                    case AbnormalCSTIDType.NoCarrier:
                        if (CarrierID.StartsWith("UNKNOWNEMP-"))
                        {
                            strNewCSTID = CarrierID;
                        }
                        else
                        {
                            //只要不是同異常的UNK, 再次異常的話, 中間的CSTID改成 "UNKNOW"
                            if (!CarrierID.StartsWith("UNKNOWNEMP") && CarrierID.StartsWith("UNK"))
                                CarrierID = "UNKNOW-";
                            strNewCSTID = "UNKNOWNEMP-" + CarrierID + "-" + strTrnDate;
                        }
                        break;
                    //strNewCSTID = $"UNKNOWNEMP-" + CarrierID + "-" + strTrnDate;
                    //break;
                    default:
                        strNewCSTID = CarrierID;
                        break;
                }
            }
            return strNewCSTID;
        }

        public CraneSpeed GetCraneSpeed(DB _db, int craneNo, CraneSpeed mainSpeed)
        {
            DataTable table = new DataTable();
            try
            {
                var baseType = funGetCraneSpeedType();
                var cmdCount = GetAllTransferCmd(_db).Count();
                var typeValue = baseType == "0" ? DateTime.Now.Hour : cmdCount;
                string SQL = "SELECT * FROM CRANESPEED";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND BASETYPE='{baseType}'";
                SQL += $" AND CRANENO='{craneNo}'";
                SQL += $" AND TYPEVALUE>={typeValue} Order by TYPEVALUE";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    CraneSpeed craneSpeed = new CraneSpeed();
                    craneSpeed.TravelaxisSpeed = Convert.ToInt32(table.Rows[0]["TRAVELAXISSPEED"]);
                    craneSpeed.LifteraxisSpeed = Convert.ToInt32(table.Rows[0]["LIFTERAXISSPEED"]);
                    craneSpeed.RotateaxisSpeed = Convert.ToInt32(table.Rows[0]["ROTATEAXISSPEED"]);
                    craneSpeed.ForkaxisSpeed = Convert.ToInt32(table.Rows[0]["FORKAXISSPEED"]);

                    //MCS 下的 Speed 設定為0
                    if (mainSpeed.TravelaxisSpeed == 0)
                    {
                        //所以參照 RMSpeed
                        mainSpeed.TravelaxisSpeed = craneSpeed.TravelaxisSpeed;
                        mainSpeed.LifteraxisSpeed = craneSpeed.LifteraxisSpeed;
                        mainSpeed.RotateaxisSpeed = craneSpeed.RotateaxisSpeed;
                        mainSpeed.ForkaxisSpeed = craneSpeed.ForkaxisSpeed;
                    }
                    else
                    {
                        //手動命令 會有初始值
                        //如果初始值 > 40 就取 mainSpeed 或 craneSpeed 兩個其中一個比較大的 
                        //如果初始值 < 40 就取 40 
                        //mainSpeed.TravelaxisSpeed = mainSpeed.TravelaxisSpeed > 40 ?
                        //                mainSpeed.TravelaxisSpeed > craneSpeed.TravelaxisSpeed ?
                        //                mainSpeed.TravelaxisSpeed : craneSpeed.TravelaxisSpeed : 40;
                        //mainSpeed.LifteraxisSpeed = mainSpeed.LifteraxisSpeed > 40 ?
                        //                mainSpeed.LifteraxisSpeed > craneSpeed.LifteraxisSpeed ?
                        //                mainSpeed.LifteraxisSpeed : craneSpeed.LifteraxisSpeed : 40;

                        //mainSpeed.RotateaxisSpeed = mainSpeed.RotateaxisSpeed > 40 ?
                        //                mainSpeed.RotateaxisSpeed > craneSpeed.RotateaxisSpeed ?
                        //                mainSpeed.RotateaxisSpeed : craneSpeed.RotateaxisSpeed : 40;

                        //mainSpeed.ForkaxisSpeed = mainSpeed.ForkaxisSpeed > 40 ?
                        //                mainSpeed.ForkaxisSpeed > craneSpeed.ForkaxisSpeed ?
                        //                mainSpeed.ForkaxisSpeed : craneSpeed.ForkaxisSpeed : 40;
                    }

                    return mainSpeed;
                }
                else
                {
                    CraneSpeed craneSpeed = new CraneSpeed();
                    mainSpeed.TravelaxisSpeed = mainSpeed.TravelaxisSpeed > 40 ? mainSpeed.TravelaxisSpeed > craneSpeed.TravelaxisSpeed ? craneSpeed.TravelaxisSpeed : mainSpeed.TravelaxisSpeed : 40;
                    mainSpeed.LifteraxisSpeed = mainSpeed.LifteraxisSpeed > 40 ? mainSpeed.LifteraxisSpeed > craneSpeed.LifteraxisSpeed ? craneSpeed.LifteraxisSpeed : mainSpeed.LifteraxisSpeed : 40;
                    mainSpeed.RotateaxisSpeed = mainSpeed.RotateaxisSpeed > 40 ? mainSpeed.RotateaxisSpeed > craneSpeed.RotateaxisSpeed ? craneSpeed.RotateaxisSpeed : mainSpeed.RotateaxisSpeed : 40;
                    mainSpeed.ForkaxisSpeed = mainSpeed.ForkaxisSpeed > 40 ? mainSpeed.ForkaxisSpeed > craneSpeed.ForkaxisSpeed ? craneSpeed.ForkaxisSpeed : mainSpeed.ForkaxisSpeed : 40;
                    return mainSpeed;
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new CraneSpeed();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public bool IsHandOff(DB _db, string zoneID)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM ZONEDEF ";
                SQL += $" WHERE STOCKERID='{_stockerId}'";
                SQL += $" AND ZONEID='{zoneID}'";
                return _db.GetDataTable(SQL, ref table) == ErrorCode.Success;
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public int UpdateAlarmData(DB _db, string eqID, int alarmType, string alarmCode, int alarmSts, string strDT)
        {
            if (string.IsNullOrWhiteSpace(eqID))
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(alarmCode))
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(strDT))
                return ErrorCode.Exception;

            string SQL = "UPDATE ALARMDATA SET";
            SQL += $" REPORTFLAG='{(char)ShelfEnable.Enable}'";
            SQL += $" WHERE STOCKERID='{_stockerId}'";
            SQL += $" AND EQID='{eqID}'";
            SQL += $" AND ALARMTYPE='{alarmType}'";
            SQL += $" AND ALARMCODE='{alarmCode}'";
            SQL += $" AND ALARMSTS='{alarmSts}'";
            SQL += $" AND STRDT='{strDT}'";
            return _db.ExecuteSQL(SQL);
        }

        public int UpdateAlarmData_FFU(DB _db, string eqID, string alarmCode, int alarmSts, string strDT)
        {
            if (string.IsNullOrWhiteSpace(eqID))
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(alarmCode))
                return ErrorCode.Exception;
            if (string.IsNullOrWhiteSpace(strDT))
                return ErrorCode.Exception;

            string SQL = "UPDATE FFUALARMDATA SET";
            SQL += $" REPORTFLAG='{(char)ShelfEnable.Enable}'";
            SQL += $" WHERE EQID='{eqID}'";
            SQL += $" AND ALARMCODE='{alarmCode}'";
            SQL += $" AND ALARMSTS='{alarmSts}'";
            SQL += $" AND STRDT='{strDT}'";
            return _db.ExecuteSQL(SQL);
        }

        public IEnumerable<AlarmData> GetCurrentAlarmSet(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT ALARMDEF.ALARMTYPE, ALARMDEF.ALARMLEVEL, ALARMDATA.ALARMSTS, ALARMDEF.ALARMCODE, ALARMDEF.ALARMID,";
                SQL += " ALARMDEF.ALARMDESC_EN, ALARMDATA.EQID, ALARMDEF.REPORTENABLE, ALARMDATA.REPORTFLAG, ALARMDATA.COMMANDID, ALARMDATA.STRDT";
                SQL += " FROM ALARMDATA, ALARMDEF";
                SQL += " WHERE ALARMDATA.STOCKERID=ALARMDEF.STOCKERID";
                SQL += " AND ALARMDATA.ALARMCODE=ALARMDEF.ALARMCODE";
                SQL += " AND ALARMDATA.ALARMTYPE=ALARMDEF.ALARMTYPE";
                SQL += " AND ALARMDATA.REPORTFLAG='Y'";
                SQL += " AND ALARMDATA.ALARMSTS='0'";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<AlarmData> lst = new List<AlarmData>();
                    foreach (DataRow row in table.Rows)
                    {
                        AlarmData portDef = new AlarmData();
                        portDef.AlarmType = Convert.ToInt32(row["ALARMTYPE"]);
                        portDef.AlarmLevel = Convert.ToInt32(row["ALARMLEVEL"]);
                        portDef.AlarmSts = Convert.ToInt32(row["ALARMSTS"]);
                        portDef.AlarmCode = Convert.ToString(row["ALARMCODE"]);
                        portDef.AlarmID = Convert.ToString(row["ALARMID"]);
                        portDef.AlarmDesc = Convert.ToString(row["ALARMDESC_EN"]);
                        portDef.EQID = Convert.ToString(row["EQID"]);
                        portDef.ReportEnable = Convert.ToString(row["REPORTENABLE"]) == "Y";
                        portDef.ReportFlag = Convert.ToString(row["REPORTFLAG"]) == "Y";
                        portDef.CommandID = Convert.ToString(row["COMMANDID"]);
                        portDef.STRDT = Convert.ToString(row["STRDT"]);
                        lst.Add(portDef);
                    }
                    return lst;
                }
                else
                    return new List<AlarmData>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<AlarmData>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<AlarmData> GetAlarmData(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT ALARMDATA.ALARMTYPE, ALARMDEF.ALARMLEVEL, ALARMDATA.ALARMSTS, ALARMDATA.ALARMCODE, ALARMDEF.ALARMID,";
                SQL += " ALARMDEF.ALARMDESC_EN, ALARMDATA.EQID, ALARMDEF.REPORTENABLE, ALARMDATA.REPORTFLAG, ALARMDATA.COMMANDID, ALARMDATA.STRDT";
                SQL += " FROM ALARMDATA LEFT JOIN ALARMDEF";
                SQL += " ON ALARMDATA.STOCKERID=ALARMDEF.STOCKERID";
                SQL += " AND ALARMDATA.ALARMCODE=ALARMDEF.ALARMCODE";
                SQL += " AND ALARMDATA.ALARMTYPE=ALARMDEF.ALARMTYPE";
                SQL += " WHERE ALARMDATA.REPORTFLAG='N'";
                SQL += " AND ALARMDATA.ALARMCODE!='0000'";
                SQL += " ORDER BY ALARMDATA.STRDT";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<AlarmData> lst = new List<AlarmData>();
                    foreach (DataRow row in table.Rows)
                    {
                        AlarmData portDef = new AlarmData();
                        portDef.AlarmType = Convert.ToInt32(row["ALARMTYPE"]);
                        string alarmLevel = Convert.ToString(row["ALARMLEVEL"]);

                        if (portDef.AlarmType == (int)AlarmTypes.Stocker)
                            portDef.AlarmLevel = string.IsNullOrWhiteSpace(alarmLevel) ? (int)AlarmLevel.Alarm : Convert.ToInt32(alarmLevel);
                        else
                            portDef.AlarmLevel = string.IsNullOrWhiteSpace(alarmLevel) ? (int)AlarmLevel.UnitAlarm : Convert.ToInt32(alarmLevel);

                        portDef.AlarmSts = Convert.ToInt32(row["ALARMSTS"]);
                        portDef.AlarmCode = Convert.ToString(row["ALARMCODE"]);
                        string alarmID = Convert.ToString(row["ALARMID"]);
                        portDef.AlarmID = string.IsNullOrWhiteSpace(alarmID) ? "99" + Convert.ToInt32(portDef.AlarmCode, 16).ToString().PadLeft(5, '0') : alarmID;

                        portDef.EQID = Convert.ToString(row["EQID"]);
                        string ReportEnable = Convert.ToString(row["REPORTENABLE"]);
                        portDef.ReportEnable = string.IsNullOrWhiteSpace(ReportEnable) ? true : ReportEnable == "Y";
                        portDef.ReportFlag = Convert.ToString(row["REPORTFLAG"]) == "Y";
                        portDef.CommandID = Convert.ToString(row["COMMANDID"]);
                        portDef.STRDT = Convert.ToString(row["STRDT"]);

                        string strALARMDESC_EN = Convert.ToString(row["ALARMDESC_EN"]);
                        if (string.IsNullOrWhiteSpace(alarmLevel))
                        {
                            portDef.AlarmDesc = "Not Report MCS AlarmCode:" + portDef.AlarmCode + " Undefined !!";
                            _LoggerService.WriteLogTrace(portDef.CommandID, "", "", portDef.AlarmDesc);
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(strALARMDESC_EN))
                            {
                                portDef.AlarmDesc = "Not Report MCS AlarmCode:" + portDef.AlarmCode + " English Description Undefined !!";
                                _LoggerService.WriteLogTrace(portDef.CommandID, "", "", portDef.AlarmDesc);
                            }
                            else
                            {
                                portDef.AlarmDesc = strALARMDESC_EN;
                            }
                        }

                        lst.Add(portDef);
                    }
                    return lst;
                }
                else
                    return new List<AlarmData>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<AlarmData>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<AlarmData> GetFFUAlarmData(DB _db)
        {
            var table = new DataTable();
            try
            {
                string SQL = "SELECT ALARMDEF.ALARMLEVEL, FFUALARMDATA.ALARMSTS, FFUALARMDATA.ALARMCODE, ALARMDEF.ALARMID,";
                SQL += " ALARMDEF.ALARMDESC_EN, FFUALARMDATA.EQID, ALARMDEF.REPORTENABLE, FFUALARMDATA.REPORTFLAG, FFUALARMDATA.STRDT";
                SQL += " FROM FFUALARMDATA LEFT JOIN ALARMDEF";
                SQL += " ON FFUALARMDATA.ALARMCODE = ALARMDEF.ALARMCODE";
                SQL += " WHERE FFUALARMDATA.REPORTFLAG='N'";
                SQL += " AND ALARMDEF.ALARMTYPE='4'";
                SQL += " ORDER BY FFUALARMDATA.STRDT";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    var lst = new List<AlarmData>();
                    foreach (DataRow row in table.Rows)
                    {
                        var portDef = new AlarmData();
                        string alarmLevel = Convert.ToString(row["ALARMLEVEL"]);
                        if (portDef.AlarmType == (int)AlarmTypes.Stocker)
                        {
                            portDef.AlarmLevel = string.IsNullOrWhiteSpace(alarmLevel) ? (int)AlarmLevel.Alarm : Convert.ToInt32(alarmLevel);
                        }
                        else
                        {
                            portDef.AlarmLevel = string.IsNullOrWhiteSpace(alarmLevel) ? (int)AlarmLevel.UnitAlarm : Convert.ToInt32(alarmLevel);
                        }
                        portDef.AlarmSts = Convert.ToInt32(row["ALARMSTS"]);
                        portDef.AlarmCode = Convert.ToString(row["ALARMCODE"]);
                        string alarmID = Convert.ToString(row["ALARMID"]);
                        portDef.AlarmID = string.IsNullOrWhiteSpace(alarmID) ? "99" + Convert.ToInt32(portDef.AlarmCode, 16).ToString().PadLeft(5, '0') : alarmID;
                        string strALARMDESC_EN = Convert.ToString(row["ALARMDESC_EN"]);
                        if (string.IsNullOrWhiteSpace(alarmLevel))
                        {
                            portDef.AlarmDesc = "Not Report MCS AlarmCode:" + portDef.AlarmCode + " Undefined !!";
                            _LoggerService.WriteLogTrace(portDef.CommandID, "", "", portDef.AlarmDesc);
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(strALARMDESC_EN))
                            {
                                portDef.AlarmDesc = "Not Report MCS AlarmCode:" + portDef.AlarmCode + " English Description Undefined !!";
                                _LoggerService.WriteLogTrace(portDef.CommandID, "", "", portDef.AlarmDesc);
                            }
                            else
                            {
                                portDef.AlarmDesc = strALARMDESC_EN;
                            }
                        }
                        portDef.EQID = Convert.ToString(row["EQID"]);
                        string ReportEnable = Convert.ToString(row["REPORTENABLE"]);
                        portDef.ReportEnable = string.IsNullOrWhiteSpace(ReportEnable) ? true : ReportEnable == "Y";
                        portDef.ReportFlag = Convert.ToString(row["REPORTFLAG"]) == "Y";
                        portDef.STRDT = Convert.ToString(row["STRDT"]);
                        lst.Add(portDef);
                    }
                    return lst;
                }
                else
                    return new List<AlarmData>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<AlarmData>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<AlarmData> GetAlarmDef(DB _db, int alarmCode)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = string.Empty;
                if (alarmCode == 0)
                { SQL = "Select ALARMCODE, ALARMID, ALARMDESC_EN, ReportEnable From AlarmDef"; }
                else
                { SQL = "Select ALARMCODE, ALARMID, ALARMDESC_EN, ReportEnable From AlarmDef where AlarmID in (" + alarmCode.ToString() + ")"; }

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<AlarmData> lst = new List<AlarmData>();
                    foreach (DataRow row in table.Rows)
                    {
                        AlarmData alarmData = new AlarmData();
                        alarmData.ReportEnable = Convert.ToString(row["ReportEnable"]) == "Y" ? true : false;
                        alarmData.AlarmCode = Convert.ToString(row["ALARMCODE"]);
                        alarmData.AlarmID = Convert.ToString(row["ALARMID"]);
                        alarmData.AlarmDesc = Convert.ToString(row["ALARMDESC_EN"]);
                        lst.Add(alarmData);
                    }
                    return lst;
                }
                else
                    return new List<AlarmData>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<AlarmData>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<PortDef> GetAllPortDef(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT PORTDEF.*, SHELFDEF.ENABLE, SHELFDEF.LOCATECRANENO";
                SQL += " FROM PORTDEF, SHELFDEF";
                SQL += " WHERE PORTDEF.STOCKERID=SHELFDEF.STOCKERID";
                SQL += $" AND PORTDEF.STOCKERID='{_stockerId}'";
                SQL += " AND PORTDEF.SHELFID=SHELFDEF.SHELFID";
                SQL += " AND SHELFDEF.STAGE=1";
                SQL += " ORDER BY PORTDEF.PLCPORTID";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<PortDef> lst = new List<PortDef>();
                    foreach (DataRow row in table.Rows)
                    {
                        PortDef portDef = new PortDef();
                        portDef.PLCPortID = Convert.ToInt32(row["PLCPORTID"]);
                        portDef.HostEQPortID = Convert.ToString(row["HOSTEQPORTID"]);
                        portDef.ShelfID = Convert.ToString(row["SHELFID"]);
                        portDef.PortType = Convert.ToInt32(row["PORTTYPE"]);
                        portDef.PortLocationType = Convert.ToInt32(row["PORTLOCATIONTYPE"]);
                        portDef.PortTypeIndex = Convert.ToInt32(row["PORTTYPEINDEX"]);
                        portDef.SourceWeighted = Convert.ToInt32(row["SOURCEWEIGHTED"]);
                        portDef.DestWeighted = Convert.ToInt32(row["DESTWEIGHTED"]);
                        portDef.TimeOutForAutoUD = Convert.ToInt32(row["TIMEOUTFORAUTOUD"]);
                        portDef.TimeOutForAutoInZone = Convert.ToString(row["TIMEOUTFORAUTOINZONE"]);
                        portDef.AlternateToZone = Convert.ToString(row["ALTERNATETOZONE"]);
                        portDef.Stage = Convert.ToInt32(row["STAGE"]);
                        portDef.Vehicles = Convert.ToInt32(row["VEHICLES"]);
                        portDef.IgnoreModeChange = Convert.ToString(row["IGNOREMODECHANGE"]) == "Y";
                        portDef.ReportMCSFlag = Convert.ToString(row["REPORTMCSFLAG"]) == "Y";
                        portDef.ReportStage = Convert.ToInt32(row["REPORTSTAGE"]);
                        portDef.NetHStnNo = Convert.ToInt32(row["NETHSTNNO"]);
                        portDef.AreaSensorStnNo = Convert.ToInt32(row["AREASENSORSTNNO"]);
                        portDef.PRESENTON_INSCST = Convert.ToString(row["PRESENTON_INSCST"]) == "Y";
                        portDef.PRESENTOFF_DELCST = Convert.ToString(row["PRESENTOFF_DELCST"]) == "Y";
                        portDef.PortEnable = Convert.ToString(row["ENABLE"]) == "Y";
                        lst.Add(portDef);
                    }
                    return lst;
                }
                else
                    return new List<PortDef>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<PortDef>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<PortDef> GetAllPortDef()
        {
            using (DB _db = GetDB())
            {
                DataTable table = new DataTable();
                try
                {
                    string SQL = "SELECT PORTDEF.*, SHELFDEF.ENABLE, SHELFDEF.LOCATECRANENO";
                    SQL += " FROM PORTDEF, SHELFDEF";
                    SQL += " WHERE PORTDEF.STOCKERID=SHELFDEF.STOCKERID";
                    SQL += $" AND PORTDEF.STOCKERID='{_stockerId}'";
                    SQL += " AND PORTDEF.SHELFID=SHELFDEF.SHELFID";
                    SQL += " AND SHELFDEF.STAGE=1";
                    SQL += " ORDER BY PORTDEF.PLCPORTID";
                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        List<PortDef> lst = new List<PortDef>();
                        foreach (DataRow row in table.Rows)
                        {
                            PortDef portDef = new PortDef();
                            portDef.PLCPortID = Convert.ToInt32(row["PLCPORTID"]);
                            portDef.HostEQPortID = Convert.ToString(row["HOSTEQPORTID"]);
                            portDef.ShelfID = Convert.ToString(row["SHELFID"]);
                            portDef.PortType = Convert.ToInt32(row["PORTTYPE"]);
                            portDef.PortLocationType = Convert.ToInt32(row["PORTLOCATIONTYPE"]);
                            portDef.PortTypeIndex = Convert.ToInt32(row["PORTTYPEINDEX"]);
                            portDef.SourceWeighted = Convert.ToInt32(row["SOURCEWEIGHTED"]);
                            portDef.DestWeighted = Convert.ToInt32(row["DESTWEIGHTED"]);
                            portDef.TimeOutForAutoUD = Convert.ToInt32(row["TIMEOUTFORAUTOUD"]);
                            portDef.TimeOutForAutoInZone = Convert.ToString(row["TIMEOUTFORAUTOINZONE"]);
                            portDef.AlternateToZone = Convert.ToString(row["ALTERNATETOZONE"]);
                            portDef.Stage = Convert.ToInt32(row["STAGE"]);
                            portDef.Vehicles = Convert.ToInt32(row["VEHICLES"]);
                            portDef.IgnoreModeChange = Convert.ToString(row["IGNOREMODECHANGE"]) == "Y";
                            portDef.ReportMCSFlag = Convert.ToString(row["REPORTMCSFLAG"]) == "Y";
                            portDef.ReportStage = Convert.ToInt32(row["REPORTSTAGE"]);
                            portDef.NetHStnNo = Convert.ToInt32(row["NETHSTNNO"]);
                            portDef.AreaSensorStnNo = Convert.ToInt32(row["AREASENSORSTNNO"]);
                            portDef.PortEnable = Convert.ToString(row["ENABLE"]) == "Y";
                            lst.Add(portDef);
                        }
                        return lst;
                    }
                    else
                        return new List<PortDef>();
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                    return new List<PortDef>();
                }
                finally
                {
                    table?.Clear();
                    table?.Dispose();
                }
            }
        }

        public IEnumerable<ZoneDef> GetAllZoneDef(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM ZONEDEF";
                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<ZoneDef> lst = new List<ZoneDef>();
                    foreach (DataRow row in table.Rows)
                    {
                        ZoneDef zoneDef = new ZoneDef();
                        zoneDef.ZoneID = Convert.ToString(row["ZoneID"]);
                        zoneDef.ZoneName = Convert.ToString(row["ZoneName"]);
                        zoneDef.HighWaterMark = Convert.ToInt32(row["HighWaterMark"]);
                        zoneDef.LowWaterMark = Convert.ToInt32(row["LowWaterMark"]);
                        zoneDef.ZoneType = Convert.ToInt32(row["ZoneType"]);
                        zoneDef.InventoryCheck = Convert.ToString(row["InventoryCheck"]);
                        zoneDef.SourceWeighted = Convert.ToInt32(row["SourceWeighted"]);
                        zoneDef.DestWeighted = Convert.ToInt32(row["DESTWEIGHTED"]);
                        lst.Add(zoneDef);
                    }
                    return lst;
                }
                else
                    return new List<ZoneDef>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<ZoneDef>();
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public IEnumerable<ZoneDef> GetAllZoneDef()
        {
            using (DB _db = GetDB())
            {
                DataTable table = new DataTable();
                try
                {
                    string SQL = "SELECT * FROM ZONEDEF";
                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        List<ZoneDef> lst = new List<ZoneDef>();
                        foreach (DataRow row in table.Rows)
                        {
                            ZoneDef zoneDef = new ZoneDef();
                            zoneDef.ZoneID = Convert.ToString(row["ZoneID"]);
                            zoneDef.ZoneName = Convert.ToString(row["ZoneName"]);
                            zoneDef.HighWaterMark = Convert.ToInt32(row["HighWaterMark"]);
                            zoneDef.LowWaterMark = Convert.ToInt32(row["LowWaterMark"]);
                            zoneDef.ZoneType = Convert.ToInt32(row["ZoneType"]);
                            zoneDef.InventoryCheck = Convert.ToString(row["InventoryCheck"]);
                            zoneDef.SourceWeighted = Convert.ToInt32(row["SourceWeighted"]);
                            zoneDef.DestWeighted = Convert.ToInt32(row["DESTWEIGHTED"]);
                            lst.Add(zoneDef);
                        }
                        return lst;
                    }
                    else
                        return new List<ZoneDef>();
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                    return new List<ZoneDef>();
                }
                finally
                {
                    table?.Clear();
                    table?.Dispose();
                }
            }
        }

        public IEnumerable<ZoneCapacitys> GetAllZoneCapacity(DB _db)
        {
            DataTable table = new DataTable();
            try
            {
                string SQL = "SELECT * FROM V_ZONECAPACITY";
                SQL += $" WHERE STOCKERID='{_stockerId}'";

                if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                {
                    List<ZoneCapacitys> lst = new List<ZoneCapacitys>();
                    foreach (DataRow row in table.Rows)
                    {
                        ZoneCapacitys zoneCapacity = new ZoneCapacitys();
                        zoneCapacity.ZoneID = Convert.ToString(row["ZONEID"]);
                        zoneCapacity.ZoneType = Convert.ToInt32(row["ZONETYPE"]);
                        zoneCapacity.ZoneCapacity = Convert.ToInt32(row["ZONECAPACITY"]);
                        zoneCapacity.TotalZoneSize = Convert.ToInt32(row["TOTALZONESIZE"]);
                        zoneCapacity.ZoneSize = Convert.ToInt32(row["ZONESIZE"]);
                        lst.Add(zoneCapacity);
                    }
                    return lst;
                }
                else
                    return new List<ZoneCapacitys>();
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                return new List<ZoneCapacitys>();
            }
            finally
            {
                if (table != null)
                {
                    table.Clear();
                    table.Dispose();
                    table = null;
                }
            }
        }

        public int InsertTransferCMD(DB _db, string commandID, int hostPriority, int priority, string carrierID, VShelfInfo source, VShelfInfo dest, int nextDest, string userID = "MCS", string BatchID = "")
        {
            string SQL = "INSERT INTO TRANSFERCMD (STOCKERID, COMMANDID, CSTID, CRANENO, TRANSFERMODE, TRANSFERMODETYPE, HOSTSOURCE, SOURCE, SOURCEBAY, HOSTDESTINATION, DESTINATION, DESTINATIONBAY,";
            SQL += " USERID, BCRREADFLAG, HOSTPRIORITY, PRIORITY, NEXTDEST, BATCHID, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{commandID}', ";
            SQL += $"'{carrierID}', ";
            SQL += $"'{source.LocateCraneNo}', ";
            SQL += $"'{(int)TransferMode.FROM_TO}', ";
            SQL += $"'{source.ShelfType.ToString() + dest.ShelfType.ToString()}', ";
            SQL += $"'{source.ZoneID}', ";
            if (source.ShelfType == (int)ShelfType.Port)
                SQL += $"'{source.PLCPortID.ToString()}', ";
            else
                SQL += $"'{source.ShelfID}', ";
            SQL += $"'{Convert.ToInt32(source.Bay_Y)}', ";
            SQL += $"'{dest.ZoneID}', ";
            if (dest.ShelfType == (int)ShelfType.Port)
                SQL += $"'{dest.PLCPortID.ToString()}', ";
            else
                SQL += $"'{dest.ShelfID}', ";
            SQL += $"'{Convert.ToInt32(dest.Bay_Y)}', ";
            SQL += $"'{userID}', ";
            SQL += $"'{(carrierID.Contains("UNK") ? _TaskInfo.Config.SystemConfig.UnknowCSTNeedScan : (source.BCRReadFlag ? "Y" : "N"))}', ";
            SQL += $"'{hostPriority}', ";
            SQL += $"'{priority}', ";
            SQL += $"'{nextDest}', ";
            SQL += $"'{BatchID}', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }
        public int InsertScanCMD(DB _db, string commandID, string carrierID, VShelfInfo source, string byWho)
        {
            string SQL = "INSERT INTO TRANSFERCMD (STOCKERID, COMMANDID, CSTID, CRANENO, TRANSFERMODE, TRANSFERMODETYPE, HOSTSOURCE, SOURCE, SOURCEBAY, HOSTDESTINATION, DESTINATION, DESTINATIONBAY,";
            SQL += " USERID, BCRREADFLAG, HOSTPRIORITY, PRIORITY, NEXTDEST, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{commandID}', ";
            SQL += $"'{carrierID}', ";
            SQL += $"'{source.LocateCraneNo}', ";
            SQL += $"'{(int)TransferMode.SCAN}', ";
            SQL += $"'{((int)TransferModeType.Scan).ToString()}', ";
            SQL += $"'{source.ZoneID}', ";
            if (source.ShelfType == (int)ShelfType.Port)
                SQL += $"'{source.PLCPortID.ToString()}', ";
            else
                SQL += $"'{source.ShelfID}', ";
            SQL += $"'{Convert.ToInt32(source.Bay_Y)}', ";
            SQL += "'0', ";
            SQL += "'0', ";
            SQL += "'0', ";
            SQL += $"'{byWho}', ";
            SQL += "'Y', ";
            SQL += "'99', ";
            SQL += "'99', ";
            SQL += "'0', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }

        public int InsertEscapeTransferCMD(DB _db, int iRM, VShelfInfo dest)
        {
            string SQL = "INSERT INTO TRANSFERCMD (STOCKERID, COMMANDID, CSTID, CRANENO, TRANSFERMODE, TRANSFERMODETYPE, HOSTSOURCE, SOURCE, SOURCEBAY, HOSTDESTINATION, DESTINATION, DESTINATIONBAY,";
            SQL += " USERID, BCRREADFLAG, HOSTPRIORITY, PRIORITY, NEXTDEST, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{GetTransferCommandID(_db)}', ";
            SQL += $"'', ";
            SQL += $"'{iRM}', ";
            SQL += $"'{(int)TransferMode.MOVE}', ";
            SQL += $"'{(int)TransferModeType.Move}', ";
            SQL += $"'0', ";
            SQL += $"'0', ";
            SQL += "'0', ";
            SQL += $"'{dest.ZoneID}', ";
            if (dest.ShelfType == (int)ShelfType.Port)
                SQL += $"'{dest.PLCPortID}', ";
            else
                SQL += $"'{dest.ShelfID}', ";
            SQL += $"'{dest.Bay_Y}', ";
            SQL += "'LCS', ";
            SQL += "'N', ";
            SQL += $"'99', ";
            SQL += $"'9999', ";
            SQL += $"'0', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }
        public int InsertDepositTransferCMD(DB _db, string commandID, int hostPriority, int priority, string carrierID, VShelfInfo source, VShelfInfo dest, int nextDest, int forkNumber, string userID = "MCS", string BatchID = "")
        {
            string SQL = "INSERT INTO TRANSFERCMD (STOCKERID, COMMANDID, CSTID, CRANENO, FORKNUMBER, TRANSFERMODE, TRANSFERMODETYPE, HOSTSOURCE, SOURCE, SOURCEBAY, HOSTDESTINATION, DESTINATION, DESTINATIONBAY,";
            SQL += " USERID, BCRREADFLAG, HOSTPRIORITY, PRIORITY, NEXTDEST, BATCHID, QUEUEDT) VALUES (";
            SQL += $"'{_stockerId}', ";
            SQL += $"'{commandID}', ";
            SQL += $"'{carrierID}', ";
            SQL += $"'{source.LocateCraneNo}', ";
            SQL += $"'{forkNumber}', ";
            SQL += $"'{(int)TransferMode.TO}', ";
            SQL += $"'{source.ShelfType.ToString() + dest.ShelfType.ToString()}', ";
            SQL += $"'{source.ZoneID}', ";
            SQL += $"'{source.ShelfID}', ";
            SQL += "'0', ";
            SQL += $"'{dest.ZoneID}', ";
            if (dest.ShelfType == (int)ShelfType.Port)
                SQL += $"'{dest.PLCPortID}', ";
            else
                SQL += $"'{dest.ShelfID}', ";
            SQL += $"'{Convert.ToInt32(dest.Bay_Y)}', ";
            SQL += $"'{userID}', ";
            SQL += "'N', ";
            SQL += $"'{hostPriority}', ";
            SQL += $"'{priority}', ";
            SQL += $"'{nextDest}', ";
            SQL += $"'{BatchID}', ";
            SQL += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
            return _db.ExecuteSQL(SQL);
        }

        public void UpdatePriority()
        {
            //每次加1
            //大於900 的不往上加
            using (DB _db = GetDB())
            {
                string strSQL = $"UPDATE TRANSFERCMD ";
                strSQL += $" SET Priority = Priority + 1";
                strSQL += $" WHERE STOCKERID='{_stockerId}'";
                strSQL += $" AND TransferState = 0 AND Priority < 900";
                _db.ExecuteSQL(strSQL);
            }
        }

        public int ComputeTaskPriority(int MCS_Priority, VShelfInfo Source, VShelfInfo Dest)
        {
            int iMCS_Priority = MCS_Priority * 10;
            //Source
            if (Source.ShelfType == (int)ShelfType.Shelf)
            {
                var SourceZone = GetAllZoneDef().Where(i => i.ZoneID == Source.ZoneID);
                if (SourceZone.Count() > 0)
                {
                    iMCS_Priority += SourceZone.FirstOrDefault().SourceWeighted;
                }
            }
            else if (Source.ShelfType == (int)ShelfType.Port)
            {
                var SourcePort = GetAllPortDef().Where(i => i.PLCPortID == Source.PLCPortID);
                if (SourcePort.Count() > 0)
                {
                    iMCS_Priority += SourcePort.FirstOrDefault().SourceWeighted;
                }
            }

            //Dest
            if (Dest.ShelfType == (int)ShelfType.Shelf)
            {
                var DestZone = GetAllZoneDef().Where(i => i.ZoneID == Dest.ZoneID);
                if (DestZone.Count() > 0)
                {
                    iMCS_Priority += DestZone.FirstOrDefault().DestWeighted;
                }
            }
            else if (Dest.ShelfType == (int)ShelfType.Port)
            {
                var DestPort = GetAllPortDef().Where(i => i.PLCPortID == Dest.PLCPortID);
                if (DestPort.Count() > 0)
                {
                    iMCS_Priority += DestPort.FirstOrDefault().DestWeighted;
                }
            }

            return Math.Min(900, iMCS_Priority);
        }

        public bool CreateTransferCommand(string defaultMessage, string commandId, VShelfInfo info, VShelfInfo dest, int priority)
        {
            string strEM = "";
            string message;
            using (var db = _TaskInfo.GetDB())
            {
                #region Begin

                int iResult = db.CommitCtrl(DB.TransactionType.Begin);
                if (iResult != ErrorCode.Success)
                {
                    message = $"{defaultMessage}, Begin Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);
                    return false;
                }

                message = $"{defaultMessage}, Begin Success";
                _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);

                #endregion Begin

                #region BookingSource

                iResult = UpdateShelfDef(db, info.ShelfID,
                    ShelfState.StorageOutReserved, ref strEM);
                if (iResult != ErrorCode.Success)
                {
                    db.CommitCtrl(DB.TransactionType.Rollback);

                    message = $"{defaultMessage}, Booking Source Fail, Message:{strEM}";
                    _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);
                    return false;
                }

                message = $"{defaultMessage}, Booking Source Success";
                _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);

                #endregion BookingSource

                #region BookingDestination

                iResult = UpdateShelfDef(db, dest.ShelfID,
                    ShelfState.StorageInReserved, ref strEM);
                if (iResult != ErrorCode.Success)
                {
                    db.CommitCtrl(DB.TransactionType.Rollback);

                    message = $"{defaultMessage}, Booking Destination Fail, Message:{strEM}";
                    _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);
                    return false;
                }

                message = $"{defaultMessage}, Booking Destination Success";
                _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);

                #endregion BookingDestination

                #region InsertTransferCMD

                iResult = InsertTransferCMD(db, commandId, priority, priority * 10, info.CSTID,
                    info, dest, 0, "LCS");
                if (iResult != ErrorCode.Success)
                {
                    db.CommitCtrl(DB.TransactionType.Rollback);

                    message = $"{defaultMessage}, Insert Transfer CMD Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);
                    return false;
                }

                message = $"{defaultMessage}, Insert Transfer CMD Success";
                _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);

                #endregion InsertTransferCMD

                #region Commit

                iResult = db.CommitCtrl(DB.TransactionType.Commit);
                if (iResult != ErrorCode.Success)
                {
                    db.CommitCtrl(DB.TransactionType.Rollback);

                    message = $"{defaultMessage}, Commit Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);
                    return false;
                }

                message = $"{defaultMessage}, Commit Success";
                _LoggerService.WriteLogTrace(commandId, "", info.CSTID, message);

                #endregion Commit
            }

            return true;
        }

        public bool CreatePortCommand(string defaultMessage, string commandId, string carrierId, PortDef def,
            VShelfInfo dest, VShelfInfo portInfo, int priority)
        {
            string strEM = "";
            string message;
            using (var db = GetDB())
            {
                #region Begin

                var iResult = db.CommitCtrl(DB.TransactionType.Begin);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Begin Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                    return false;
                }

                message = defaultMessage + $", Begin Success";
                _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                #endregion Begin

                #region BookingDestination

                iResult = UpdateShelfDef(db, dest.ShelfID, ShelfState.StorageInReserved, ref strEM);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Booking Destination Fail, Message:{strEM}";
                    _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                    db.CommitCtrl(DB.TransactionType.Rollback);
                    return false;
                }

                message = defaultMessage + $", Booking Destination Success";
                _LoggerService.WriteLogTrace(commandId, "", carrierId, message);

                #endregion BookingDestination

                #region InsertDepositTransferCMD

                iResult = InsertTransferCMD(db, commandId, priority, priority * 10, carrierId, portInfo, dest, 0, "LCS");
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Insert Transfer Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                    db.CommitCtrl(DB.TransactionType.Rollback);
                    return false;
                }

                message = defaultMessage + $", Insert Transfer Success";
                _LoggerService.WriteLogTrace(commandId, "", carrierId, message);

                #endregion InsertDepositTransferCMD

                #region Commit

                iResult = db.CommitCtrl(DB.TransactionType.Commit);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Commit Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                    return false;
                }

                message = defaultMessage + $", Commit Success";
                _LoggerService.WriteLogTrace(commandId, "", carrierId, message);

                #endregion Commit
            }

            return true;
        }

        public bool CreateToCommand(string defaultMessage, string commandId, string carrierId, int iRM, int iFork,
            VShelfInfo dest, int priority, VShelfInfo carrierInfo)
        {
            string strEM = "";
            string message;
            using (var db = GetDB())
            {
                #region Begin

                var iResult = db.CommitCtrl(DB.TransactionType.Begin);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Begin Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                    return false;
                }

                message = defaultMessage + $", Begin Success";
                _LoggerService.WriteLogTrace(commandId, "", carrierId, message);

                #endregion Begin

                #region BookingDestination

                iResult = UpdateShelfDef(db, dest.ShelfID,
                    ShelfState.StorageInReserved, ref strEM);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Booking Destination Fail, Message:{strEM}";
                    _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                    db.CommitCtrl(DB.TransactionType.Rollback);
                    return false;
                }

                message = defaultMessage + $", Booking Destination Success";
                _LoggerService.WriteLogTrace(commandId, "", carrierId, message);

                #endregion BookingDestination

                #region InsertDepositTransferCMD

                iResult = InsertDepositTransferCMD(db, commandId, priority, priority * 10, carrierId, carrierInfo, dest, 0, iFork, "LCS");
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Insert Deposit Transfer Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                    db.CommitCtrl(DB.TransactionType.Rollback);
                    return false;
                }

                message = defaultMessage + $", Insert Deposit Transfer Success";
                _LoggerService.WriteLogTrace(commandId, "", carrierId, message);

                #endregion InsertDepositTransferCMD

                #region Commit

                iResult = db.CommitCtrl(DB.TransactionType.Commit);
                if (iResult != ErrorCode.Success)
                {
                    message = defaultMessage + $", Commit Fail, Result:{iResult}";
                    _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                    return false;
                }

                message = defaultMessage + $", Commit Success";
                _LoggerService.WriteLogTrace(commandId, "", carrierId, message);
                #endregion Commit
            }

            return true;
        }

        public bool GetCassetteData(string shelfID, out IEnumerable<CassetteData> cassetteDatas)
        {
            DataTable table = new DataTable();
            try
            {
                using (DB _db = _TaskInfo.GetDB())
                {
                    string SQL = "SELECT * FROM CASSETTEDATA";
                    SQL += $" WHERE STOCKERID='{_stockerId}'";
                    SQL += $" AND SHELFID='{shelfID}'";
                    if (_db.GetDataTable(SQL, ref table) == ErrorCode.Success)
                    {
                        List<CassetteData> datas = new List<CassetteData>();
                        for (int iRow = 0; iRow < table.Rows.Count; iRow++)
                        {
                            CassetteData info = new CassetteData();
                            info.CSTID = Convert.ToString(table.Rows[iRow]["CSTID"]);
                            info.ShelfID = Convert.ToString(table.Rows[iRow]["SHELFID"]);
                            info.Stage = Convert.ToInt32(table.Rows[iRow]["STAGE"]);
                            info.CSTState = Convert.ToInt32(table.Rows[iRow]["CSTState"]);
                            info.LotID = Convert.ToString(table.Rows[iRow]["LotID"]);
                            info.EmptyCST = Convert.ToString(table.Rows[iRow]["EmptyCST"]);
                            info.CSTType = Convert.ToString(table.Rows[iRow]["CSTType"]);
                            info.CSTInDT = Convert.ToString(table.Rows[iRow]["CSTInDT"]);
                            info.StoreDT = Convert.ToString(table.Rows[iRow]["StoreDT"]);
                            info.WaitOutLPDT = Convert.ToString(table.Rows[iRow]["WaitOutLPDT"]);
                            info.WaitOutOPDT = Convert.ToString(table.Rows[iRow]["WaitOutOPDT"]);
                            datas.Add(info);
                        }
                        cassetteDatas = datas;
                        return true;
                    }
                    else
                    {
                        cassetteDatas = new List<CassetteData>();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}");
                cassetteDatas = new List<CassetteData>();
                return false;
            }
            finally
            {
                table?.Clear();
                table?.Dispose();
            }
        }

        public bool DeleteTransferAndTask(DB db, string commandId, string taskNo, string carrierId, string defaultMessage)
        {
            string message;
            #region Begin
            var iResult = db.CommitCtrl(DB.TransactionType.Begin);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Begin Fail, Result:{iResult}";
                _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
                return false;
            }
            message = defaultMessage + ", Begin Success";
            _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
            #endregion Begin

            #region InsertTransferToHistory
            iResult = InsertTransferToHistory(db, commandId);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Insert HisTransfer Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }
            message = defaultMessage + $", Insert HisTransfer Success";
            _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
            #endregion InsertTransferToHistory

            #region DeleteTransfer
            iResult = DeleteTransfer(db, commandId);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Delete Transfer Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
                db.CommitCtrl(DB.TransactionType.Rollback);
                return false;
            }
            message = defaultMessage + $", Delete Transfer Success";
            _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
            #endregion DeleteTransfer

            #region InsertTaskToHistory
            iResult = InsertTaskToHistory(db, commandId);
            if (iResult == ErrorCode.Success)
            {
                message = defaultMessage + $", CommandID:{commandId}, Insert HisTask Success";
                _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
            }
            #endregion InsertTaskToHistory

            #region DeleteTask
            iResult = DeleteTask(db, commandId);
            if (iResult == ErrorCode.Success)
            {
                message = defaultMessage + $", Delete Task Success";
                _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
            }
            #endregion DeleteTask

            #region Commit  
            iResult = db.CommitCtrl(DB.TransactionType.Commit);
            if (iResult != ErrorCode.Success)
            {
                message = defaultMessage + $", Commit Fail, Result={iResult}";
                _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
                return false;
            }
            message = defaultMessage + ", Commit Success";
            _LoggerService.WriteLogTrace(commandId, taskNo, carrierId, message);
            #endregion Commit
            return true;
        }

        public int InsertIdReadErrorLog(string CarrierLoc, string cstId)
        {
            if (string.IsNullOrWhiteSpace(CarrierLoc))
                return ErrorCode.Exception;

            using (var db = GetDB())
            {
                string SQL = "INSERT INTO DATACOLLECTION (NAME, VALUE, TIME) VALUES (";
                SQL += $" 'DEVICE_{CarrierLoc}',";
                SQL += $" '{1}',";
                SQL += $" '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                return db.ExecuteSQL(SQL);
            }
        }

        public void UpdateFork(string traceCommandId, int fork = 0)
        {
            using (DB _db = GetDB())
            {
                string strSQL = $"UPDATE TRANSFERCMD ";
                strSQL += $" SET ForkNumber = {fork}";
                strSQL += $" WHERE STOCKERID='{_stockerId}' AND COMMANDID = '{traceCommandId}'";
                _db.ExecuteSQL(strSQL);
            }
        }

        public void DeleteHistory(DB db)
        {
            string SQL = "DELETE FROM HISTASK WHERE HISDT < (CONVERT([VARCHAR],GETDATE()-180,(121)))";
            db.ExecuteSQL(SQL);
            SQL = "DELETE FROM HISTRANSFERCMD WHERE HISDT < (CONVERT([VARCHAR], GETDATE() - 180, (121)))";
            db.ExecuteSQL(SQL);
            SQL = "DELETE FROM HISCASSETTEDATA WHERE HISDT < (CONVERT([VARCHAR], GETDATE() - 180, (121)))";
            db.ExecuteSQL(SQL);
            SQL = "DELETE FROM UNITSTSLOG WHERE STRDT < (CONVERT([VARCHAR], GETDATE() - 180, (121)))";
            db.ExecuteSQL(SQL);
            SQL = "DELETE FROM ALARMDATA WHERE STRDT < (CONVERT([VARCHAR], GETDATE() - 180, (121)))";
            db.ExecuteSQL(SQL);
            SQL = "DELETE FROM USEROPLOG WHERE ACTIONTIME < (CONVERT([VARCHAR], GETDATE() - 180, (121)))";
            db.ExecuteSQL(SQL);
            SQL = "DELETE FROM CASSETTESTAYTIMEHISTORY WHERE STOREDT < (CONVERT([VARCHAR], GETDATE() - 180, (121)))";
            db.ExecuteSQL(SQL);
        }
    }
}
