using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config.Net;

namespace Mirle.Def.V2BYMA30
{
    public interface ASRSINI
    {
        [Option(Alias = "Data Base")]
        DatabaseConfig Database { get; }

        [Option(Alias = "WMS Data Base")]
        DatabaseConfig Database_WMS { get; }
 

        [Option(Alias = "WCS API")]
        APIConfig WCS_API { get; }

        [Option(Alias = "Local API")]
        APIConfig Local_API { get; }

       

        [Option(Alias = "CV PLC1")]
        CV_PlcConfig CV_8F { get; }

        [Option(Alias = "CV PLC2")]
        CV_PlcConfig CV_10F { get; }

        [Option(Alias = "CV PLC3")]
        CV_PlcConfig CV_ELE { get; }


    }

    public interface DatabaseConfig
    {
        /// <summary>
        /// MSSQL, Oracle_Oledb, DB2, Odbc, Access, OleDb, Oracle_DB.enuDatabaseType.Oracle_OracleClient, SQLite,
        /// </summary>
        string DBMS { get; }

        string DbServer { get; }
        string FODbServer { get; }
        string DataBase { get; }
        string DbUser { get; }
        string DbPswd { get; }

        [Option(DefaultValue = 1433)]
        int DBPort { get; }

        [Option(DefaultValue = 30)]
        int ConnectTimeOut { get; }

        [Option(DefaultValue = 30)]
        int CommandTimeOut { get; }
    }


    public interface APIConfig
    {
        string IP { get; }
    }

    public interface ForkSetupConfig
    {
        string S1 { get; }
        string S2 { get; }
        string S3 { get; }
        string S4 { get; }
    }

    public interface CV_PlcConfig
    {
        [Option(DefaultValue = 0)]
        int MPLCNo { get; }
        string MPLCIP { get; }

        [Option(DefaultValue = 0)]
        int MPLCPort { get; }

        [Option(DefaultValue = 0)]
        int MPLCTimeout { get; }

        [Option(DefaultValue = 0)]
        int WritePLCRawData { get; }

        [Option(DefaultValue = 0)]
        int UseMCProtocol { get; }

        [Option(DefaultValue = 0)]
        int InMemorySimulator { get; }

        [Option(DefaultValue = 10)]
        int CycleCount_Max { get; }
    }

  

}
