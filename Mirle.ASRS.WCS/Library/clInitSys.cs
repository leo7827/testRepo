using System;
using System.Windows.Forms;
using Mirle.Def;
using Config.Net;
using Mirle.Def.V2BYMA30;
using Mirle.DataBase;
using Mirle.Structure;
using System.Text;
using System.Runtime.InteropServices;
using Mirle.LiteOn.V2BYMA30;

namespace Mirle.ASRS.WCS
{
    public class clInitSys
    {
        public static clsDbConfig DbConfig = new clsDbConfig();
        public static clsDbConfig DbConfig_WMS = new clsDbConfig();
        public static clsPlcConfig[] CV_Config = new clsPlcConfig[5];
        public static clsPlcConfig CV_Config_8F = new clsPlcConfig(); 
        public static clsPlcConfig CV_Config_10F = new clsPlcConfig();
        public static clsPlcConfig CV_Config_Ele = new clsPlcConfig();

        public static WebApiConfig WcsApi_Config = new WebApiConfig();
        public static WebApiConfig LocalApi_Config = new WebApiConfig();
        public static OEEParamConfig OEEparamConfig = new OEEParamConfig();
        public static ASRSINI lcsini;
        public static string[] gsStockerID = new string[4];
        public static int L2L_MaxCount = 5;
        

        //API
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString
            (string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static void FunLoadIniSys()
        {
            try
            {
                lcsini = new ConfigurationBuilder<ASRSINI>()
                   .UseIniFile("Config\\ASRS.ini")
                   .Build();

                FunDbConfig(lcsini);     
                FunApiConfig(lcsini);             
                FunPlcConfig(lcsini);                           
                FunLoadCVAlarmIni();
            }
            catch(Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                MessageBox.Show("找不到.ini資料，請洽系統管理人員 !!", "MIRLE", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(0);
            }
        }

        private static void FunDbConfig(ASRSINI lcsini)
        {
            DbConfig.CommandTimeOut = lcsini.Database.CommandTimeOut;
            DbConfig.ConnectTimeOut = lcsini.Database.ConnectTimeOut;
            DbConfig.DbName = lcsini.Database.DataBase;
            DbConfig.DbPassword = lcsini.Database.DbPswd;
            DbConfig.DbServer = lcsini.Database.DbServer;
            DbConfig.DBType = (DBTypes)Enum.Parse(typeof(DBTypes), lcsini.Database.DBMS);
            DbConfig.DbUser = lcsini.Database.DbUser;
            DbConfig.FODBServer = lcsini.Database.FODbServer;
            DbConfig.WriteLog = true;

            //DbConfig_WMS.CommandTimeOut = lcsini.Database_WMS.CommandTimeOut;
            //DbConfig_WMS.ConnectTimeOut = lcsini.Database_WMS.ConnectTimeOut;
            //DbConfig_WMS.DbName = lcsini.Database_WMS.DataBase;
            //DbConfig_WMS.DbPassword = lcsini.Database_WMS.DbPswd;
            //DbConfig_WMS.DbServer = lcsini.Database_WMS.DbServer;
            //DbConfig_WMS.DBType = (DBTypes)Enum.Parse(typeof(DBTypes), lcsini.Database_WMS.DBMS);
            //DbConfig_WMS.DbUser = lcsini.Database_WMS.DbUser;
            //DbConfig_WMS.FODBServer = lcsini.Database_WMS.FODbServer;
            //DbConfig_WMS.WriteLog = true;
        }

       

        private static void FunApiConfig(ASRSINI lcsini)
        {
            WcsApi_Config.IP = lcsini.WCS_API.IP;
            LocalApi_Config.IP = lcsini.Local_API.IP;
        }


        private static void FunPlcConfig(ASRSINI lcsini)
        {
            //for (int i= 1; i <6;i++)
            //{
            //    CV_Config[i].InMemorySimulator = lcsini.CV.InMemorySimulator switch
            //    {
            //        1 => true,
            //        _ => false
            //    };

            //    CV_Config[i].MPLCIP = lcsini.CV.MPLCIP;
            //    CV_Config[i].MPLCNo = lcsini.CV.MPLCNo;
            //    CV_Config[i].MPLCPort = lcsini.CV.MPLCPort;
            //    CV_Config[i].MPLCTimeout = lcsini.CV.MPLCTimeout;

            //    CV_Config[i].UseMCProtocol = lcsini.CV.UseMCProtocol switch
            //    {
            //        1 => true,
            //        _ => false
            //    };

            //    CV_Config[i].WritePLCRawData = lcsini.CV.WritePLCRawData switch
            //    {
            //        1 => true,
            //        _ => false
            //    };
            //}
            
            #region 8F
            CV_Config_8F.InMemorySimulator = lcsini.CV_8F.InMemorySimulator switch
            {
                1 => true,
                _ => false
            };

            CV_Config_8F.MPLCIP = lcsini.CV_8F.MPLCIP;
            CV_Config_8F.MPLCNo = lcsini.CV_8F.MPLCNo;
            CV_Config_8F.MPLCPort = lcsini.CV_8F.MPLCPort;
            CV_Config_8F.MPLCTimeout = lcsini.CV_8F.MPLCTimeout;

            CV_Config_8F.UseMCProtocol = lcsini.CV_8F.UseMCProtocol switch
            {
                1 => true,
                _ => false
            };

            CV_Config_8F.WritePLCRawData = lcsini.CV_8F.WritePLCRawData switch
            {
                1 => true,
                _ => false
            };
            #endregion
                                  

            #region 10F
            CV_Config_10F.InMemorySimulator = lcsini.CV_10F.InMemorySimulator switch
            {
                1 => true,
                _ => false
            };

            CV_Config_10F.MPLCIP = lcsini.CV_10F.MPLCIP;
            CV_Config_10F.MPLCNo = lcsini.CV_10F.MPLCNo;
            CV_Config_10F.MPLCPort = lcsini.CV_10F.MPLCPort;
            CV_Config_10F.MPLCTimeout = lcsini.CV_10F.MPLCTimeout;

            CV_Config_10F.UseMCProtocol = lcsini.CV_10F.UseMCProtocol switch
            {
                1 => true,
                _ => false
            };

            CV_Config_10F.WritePLCRawData = lcsini.CV_10F.WritePLCRawData switch
            {
                1 => true,
                _ => false
            };
            #endregion

            #region Elevator
            CV_Config_Ele.InMemorySimulator = lcsini.CV_ELE.InMemorySimulator switch
            {
                1 => true,
                _ => false
            };

            CV_Config_Ele.MPLCIP = lcsini.CV_ELE.MPLCIP;
            CV_Config_Ele.MPLCNo = lcsini.CV_ELE.MPLCNo;
            CV_Config_Ele.MPLCPort = lcsini.CV_ELE.MPLCPort;
            CV_Config_Ele.MPLCTimeout = lcsini.CV_ELE.MPLCTimeout;

            CV_Config_Ele.UseMCProtocol = lcsini.CV_ELE.UseMCProtocol switch
            {
                1 => true,
                _ => false
            };

            CV_Config_Ele.WritePLCRawData = lcsini.CV_ELE.WritePLCRawData switch
            {
                1 => true,
                _ => false
            };
            #endregion 
            
        }

       

        public static void FunLoadCVAlarmIni()
        {
            try
            {
                string strFileName = null;
                strFileName = Application.StartupPath + "\\Config\\CVAlarm.ini";

                if (!System.IO.File.Exists(strFileName))
                {
                    MessageBox.Show("找不到.ini資料，請洽系統管理人員 !!", "MIRLE", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Environment.Exit(0);
                }

                string strAppName = "", strKeyName = "";
                for (int i=1; i <= clsLiteOnCV.GetBufferCount(); i++)
                {
                    strAppName = "A1-" + Convert.ToString(i).PadLeft(2, '0');

                    string[] bit = new string[32];
                    for (int j = 0; j< 32; j++)
                    {
                        strKeyName = "bit" + Convert.ToString(j);
                        try
                        {
                            bit[j] = funReadParam(strFileName, strAppName, strKeyName);
                            bit[j] = "(bit" + j.ToString() + ") " + bit[j];
                        }
                        catch
                        {
                            bit[j] = "";
                        }
                                            
                    }
                    clsLiteOnCV.CV_Alarm.Add(i, bit);
                }
                 
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }


        /// <summary>
        /// 讀取ini檔的單一欄位
        /// </summary>
        /// <param name="sFileName">INI檔檔名</param>
        /// <param name="sAppName">區段名</param>
        /// <param name="sKeyName">KEY名稱</param>
        /// <param name="strDefault">Default</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string funReadParam(string sFileName, string sAppName, string sKeyName, string strDefault = "")
        {
            StringBuilder sResult = new StringBuilder(255);
            int intResult;
            try
            {
                intResult = GetPrivateProfileString(sAppName, sKeyName, strDefault, sResult, sResult.Capacity, sFileName);
                string R = sResult.ToString().Trim();
                return R;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return strDefault;
            }
        }
    }
}
