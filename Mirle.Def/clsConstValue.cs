﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.Def
{
    public class clsConstValue
    {
        public const string BcrError = "ERROR1";

        public const string CarrierNoCmd = "FAILID";

        public class ApiReturnCode
        {
            public const string Success = "200";
            public const string Fail = "500";
        }

        public class OEECountMethod
        {
            public const string lot = "lot";
            public const string bin = "bin";
        }

        public class STKC_FinishLoc
        {
            public const string LeftFork = "0001001";
            public const string RightFork = "0001002";
        }

        public class CmdSts
        {
            /// <summary>
            /// 命令完成带过帐
            /// </summary>
            public const string strCmd_Finished = "7";
            /// <summary>
            /// 命令取消带过帐
            /// </summary>
            public const string strCmd_Cancel = "8";
            public const string strCmd_Initial = "0";
            /// <summary>
            /// 執行中
            /// </summary>
            public const string strCmd_Running = "1";
        }

        public class CarrierType
        {
            public const int BIN = 1;
            public const int MAG = 2;
        }
        public class FloorPath
        {
            public const int Floor_10 = 10;           
            public const int Floor_8 = 8;
        }

        public class DoorStatus
        {
            public const int Ini = 0;
            public const int Close = 1;
            public const int Open = 2; 
        }


        public class CmdMode
        {
            public const string StockIn = "1";
            public const string StockOut = "2";
            public const string Cycle = "3";
            /// <summary>
            /// 站對站
            /// </summary>
            public const string S2S = "4";
            /// <summary>
            /// 庫對庫
            /// </summary>
            public const string L2L = "5";
            /// <summary>
            /// 置物
            /// </summary>
            public const string Deposit = "9";
        }

        public class LocSts
        {
            /// <summary>
            /// 空儲位
            /// </summary>
            public const string Empty = "NONE";
            /// <summary>
            /// 空料盒
            /// </summary>
            public const string EmptyBox = "EMPTY";
            /// <summary>
            /// 入庫預約
            /// </summary>
            public const string IN = "INT";
            /// <summary>
            /// 出庫預約
            /// </summary>
            public const string OUT = "OUT";
            /// <summary>
            /// 禁用
            /// </summary>
            public const string Block = "DISABLE";
            public const string Normal = "NORMAL";
            public const string Abnormal = "ABNORMAL";
            /// <summary>
            /// 滿板
            /// </summary>
            public const string Full = "FULL";
            /// <summary>
            /// 不滿板
            /// </summary>
            public const string NotFull = "NOTFULL";
        }
        public class AlarmCode
        {
            public const string HeartBeat = "20000";
        }

        public class deviceID
        {
            public const string E1 = "E1";

            public const string E2 = "E2";

            public const string E801 = "E801";

            public const string E802 = "E802";

            public const string E803 = "E803";

            public const string E804 = "E804";

            public const string E805 = "E805";

            public const string E806 = "E806";


            public const string B800 = "B1";

            public const string M800 = "M1";

            public const string E04 = "LI1";

            public const string E05 = "LI2";

            public const string E04_10F = "LO1";

            public const string E04_8F = "LO2";

            public const string E05_8F = "LO3";

            public const string E05_3F = "LO4";

            public const string E05_5F = "LO5";

            public const string E05_6F = "LO6";

            public const string AGV_3F = "A1";

            public const string AGV_5F = "A2";

            public const string AGV_6F = "A3";

            public const string A4 = "A4";

            //8F整理區
            public const string S0 = "S0";

            //線邊倉
            public const string SMT_123_F = "S1";

            public const string SMT_123_B = "S2";

            public const string SMT_456_F = "S3";

            public const string SMT_456_B = "S4";

            public const string SMT_789_F = "S5";

            public const string SMT_789_B = "S6";
        }
        public class CompleteCode
        {
            public const string PickProc_T2TimeOut = "C0";             //取物 T2 time out
            public const string PickProc_EQLReqOn = "C1";               //取物檢測到EQ L-REQ訊號ON
            public const string PickProc_EQUReqOff = "C2";             //取物檢測到EQ U-REQ訊號OFF
            public const string PickProc_EQReadyOn = "C3";              //取物檢測到EQ Ready訊號ON
            public const string PickProc_EQOnlineOffBeforePick = "C4"; //RM取物前行程EQ Online信號中斷
            public const string PickProc_EQRYOff = "C5";                //RM取物Tr-on檢測到EQ RY信號中斷
            public const string PickProc_EQNoCST = "C6";               //EQ Port站口無物
            public const string PickProc_EQNotForkAccess = "C7";       //EQ Port不允許Fork存取
            public const string PickProc_T5TimeOut = "C8";             //取物 T5 time out
            public const string PickProc_EQUReqOnAfterRMFinish = "C9";  //RM取物完檢測到EQ U-REQ訊號ON
            public const string PickProc_EQOnLineOffAfterRMFinish = "CA";   //RM取物完檢測到EQ Online OFF
            public const string DeposProc_T2TimeOut = "D0";         //置物 T2 time out
            public const string DeposProc_EQLReqOff = "D1";         //置物檢測到EQ L-REQ訊號OFF
            public const string DeposProc_EQUReqOn = "D2";              //置物檢測到EQ U-REQ訊號ON
            public const string DeposProc_EQReadyOn = "D3";         //置物檢測到EQ Ready訊號ON
            public const string DeposProc_EQOnlineOffBeforeDepos = "D4";    //RM置物前行程EQ Online信號中斷
            public const string DeposProc_EQRYOffBeforeDepos = "D5";    //RM置物Tr-on檢測到EQ RY信號中斷
            public const string DeposProc_EQHaveCST = "D6";         //EQ Port站口有物
            public const string DeposProc_EQNotForkAccess = "D7";       //EQ Port不允許Fork存置
            public const string DeposProc_T5TimeOut = "D8";         //置物 T5 time out
            public const string DeposProc_EQLReqOnAfterDepos = "D9";    //RM置物完檢測到EQ L-REQ訊號ON
            public const string DeposProc_EQOnlineOffAfterDepos = "DA"; //RM置物完檢測到EQ Online OFF
            public const string InlineInterlockError_OnLine = "E0"; //Inline Interlock Error(On-Line)
            public const string TransferRequestWrong = "E1";            //Transfer Request Wrong.
            public const string EmptyRetrieval = "E2";                  //儲位空出庫
            public const string ScanIDReadError = "E3";                 //Scan ID Read Error
            public const string IDMismatch = "E4";                      //ID Mismatch
            public const string ScanNoResponse = "E5";                  //Scan No Response
            public const string NoCST = "E6";                           //檢知無CST
            public const string IDReadError = "E7";                     //ID Read Error
            public const string NoResponse = "E8";                      //No Response
            public const string FromCommandAbout = "E9";                //From Command about
            public const string MoveScanCommandAbout = "EA";            //Move/Scan command about
            public const string CassetteTypeMissmach = "EB";            //Cassette Type MissMach
            public const string DoubleStorage = "EC";                   //Double storage
            public const string InlineInterlockError_LD = "ED";     //Inline Interlock Error(LD)
            public const string InlineInterlockError_ULD = "EE";        //Inline Interlock Error(ULD)
            public const string HMIUserForceAbortCommand = "EF";        //HMI User Force Abort Command
            public const string HMIUserForceFinishCommand = "FF";       //HMI User Force Finish Command
            public const string Success_FromReturnCodeAck = "91";       //From Return code Ack
            public const string Success_ToReturnCode = "92";            //To Return code
            public const string Success_CraneIsRunningRetryMoving = "94";   //Crane is running retry moving
            public const string Success_ScanComplete = "97";            //Scan complete
            public const string Success_IdleTimeOutReset = "99";            //Idle Timeout Reset Abnormal complete
            public const string Success_AbortDuringCycle1 = "93";                   //AbortCMD in Cycle1

            public const string DeposProc_Obstruction = "DB";          //RM置物前發生 Obstruction
            public const string PickProc_Obstruction = "CB";            //RM取物前發生 Obstruction

            public const string PickupCycle_Error = "F1";
            public const string DepositCycle_Error = "F2";

            // Form STKC
            //retry
            public const string CannotRetrieveFromSourcePortFromSTKC_P0 = "P0";    //STKC 來源Port無法取物
            public const string CannotDepositToDestinationPortFromSTKC_P1 = "P1";  //STKC 目的Port無法置物


            //abort 
            public const string CannotRetrieveHasCarrierOnCraneFromSTKC_P2 = "P2"; //STKC 車上有物無法取物
            public const string CannotDepositNoCarrierOnCraneFromSTKC_P3 = "P3";   //STKC 車上無物無法置物

            //complete
            public const string CannotScanHasCarrierOnCraneFromSTKC_P4 = "P4";     //STKC 車上有物無法Scan

            public const string CannotExcuteFromSTKC = "PD";            //STKC 判斷地上盤無法執行該命令
            public const string CommandTimeoutFromSTKC = "PE";          //STKC 下命令給地上盤命令後，地上盤未有反應或執行超過10分鐘
        }
    }
}
