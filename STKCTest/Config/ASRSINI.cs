using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config.Net;

namespace Mirle.ASRS.WCS
{
    public interface ASRSINI
    {
        [Option(Alias = "Data Base")]
        DatabaseConfig Database { get; }

        [Option(Alias = "WMS API")]
        APIConfig WMS_API { get; }

        [Option(Alias = "Device")]
        DeviceConfig Device { get; }

        [Option(Alias = "CV PLC")]
        CV_PlcConfig CV { get; }

        [Option(Alias = "STK Port")]
        StkPortConfig StkPort { get; }

        [Option(Alias = "StnNo")]
        StnNoConfig StnNo { get; }
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

    public interface DeviceConfig
    {
        string StockerID { get; }
        string Speed { get; }
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
    }

    public interface StkPortConfig
    {
        [Option(DefaultValue = 1)]
        int Left1 { get; }

        [Option(DefaultValue = 2)]
        int Left2 { get; }

        [Option(DefaultValue = 3)]
        int Right1 { get; }

        [Option(DefaultValue = 4)]
        int Right2 { get; }
    }

    public interface StnNoConfig
    {
        string A1_41 { get; }
        string A1_42 { get; }
        string A1_43 { get; }
        string A1_44 { get; }
    }
}
