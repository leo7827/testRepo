using System.Collections.Generic;
using Mirle.DataBase;
using Mirle.Def;

namespace Mirle.DB.Proc
{
    public class clsHost
    {
        private readonly clsProc Process;
        private readonly clsCmd_Mst CMD_MST;
        private readonly clsSno SNO;
        private readonly clsLocMst LocMst;
        private readonly clsTask task;
        private readonly clsAlarmData ALARMDATA;
        private readonly clsCmd_Mst_His CMD_MST_HIS;
        private readonly clsEQ_Alarm EQ_Alarm;
        private readonly clsUnitStsLog unitStsLog;
        private readonly clsUnitModeDef unitModeDef;
        private readonly clsL2LCount L2LCount;
        private static object _Lock = new object();
        private static bool _IsConn = false;
        public static bool IsConn
        {
            get { return _IsConn; }
            set
            {
                lock(_Lock)
                {
                    _IsConn = value;
                }
            }
        }

        private static clsDbConfig _config_Sqlite = new clsDbConfig
        {
            DBType = DBTypes.SQLite
        };

        public clsHost(string DB_Server_Sqlite, string DB_Name_Sqlite)
        {
            _config_Sqlite.DbName = DB_Name_Sqlite;
            _config_Sqlite.DbName = DB_Name_Sqlite;
            Process = new clsProc(_config_Sqlite);
            CMD_MST = new clsCmd_Mst(_config_Sqlite);
            SNO = new clsSno(_config_Sqlite); 

            task = new clsTask( _config_Sqlite);
            //ALARMDATA = new clsAlarmData(config);
            //CMD_MST_HIS = new clsCmd_Mst_His(config);
            //EQ_Alarm = new clsEQ_Alarm(config); 
        }

        public clsProc GetProcess()
        {
            return Process;
        }

        public clsCmd_Mst GetCmd_Mst()
        {
            return CMD_MST;
        }

        public clsLocMst GetLocMst()
        {
            return LocMst;
        }

        public clsTask GetTask()
        {
            return task;
        }

        public clsSno GetSNO()
        {
            return SNO;
        }

        public clsAlarmData GetAlarmData()
        {
            return ALARMDATA;
        }
        public clsCmd_Mst_His GetCmd_Mst_His()
        {
            return CMD_MST_HIS;
        }

        public clsEQ_Alarm GetEQ_Alarm()
        {
            return EQ_Alarm;
        }

        public clsUnitStsLog GetUnitStsLog()
        {
            return unitStsLog;
        }

        public clsUnitModeDef GetUnitModeDef()
        {
            return unitModeDef;
        }

        public clsL2LCount GetL2LCount()
        {
            return L2LCount;
        }

        public List<Element_Port>[] GetLstPort()
        {
            return Process.GetLstPort();
        }
    }
}
