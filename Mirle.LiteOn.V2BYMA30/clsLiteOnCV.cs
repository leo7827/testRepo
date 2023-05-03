using Mirle.Def.V2BYMA30;
using System.Collections.Generic;
using Mirle.Structure;
using Mirle.Def;
using Mirle.ASRS.Conveyor.V2BYMA30_8F;
using Mirle.ASRS.Conveyor.V2BYMA30_8F.Signal;
using Mirle.ASRS.Conveyor.V2BYMA30_8F.View;
using Mirle.ASRS.Conveyor.V2BYMA30_10F; 
using System.Linq;
using System.Data;
using System.Windows.Forms;


namespace Mirle.LiteOn.V2BYMA30 
{
    public class clsLiteOnCV
    {
        private static ASRS.Conveyor.V2BYMA30_8F.ConveyorController conveyorController_8F; 
        private static ASRS.Conveyor.V2BYMA30_10F.ConveyorController conveyorController_10F;
        private static ASRS.Conveyor.V2BYMA30_Elevator.ConveyorController conveyorController_Elevator;
        public static readonly Dictionary<int, ConveyorInfo[]> CV_Group = new Dictionary<int, ConveyorInfo[]>();
        public static readonly Dictionary<int, string[]> CV_Alarm = new Dictionary<int, string[]>();

        private static ASRS.Conveyor.V2BYMA30_8F.View.MainView _mainView_8F;         
        private static ASRS.Conveyor.V2BYMA30_10F.View.MainView _mainView_10F;
        private static ASRS.Conveyor.V2BYMA30_Elevator.View.MainView _mainView_Elevator;
        // private static CVCHost controllerHost;

        public static void FunInitalCVController(clsPlcConfig CV , int iPLC)
        {
            switch (iPLC)
            { 
                case 1:
                    var Config_CV_8F = new ASRS.Conveyor.V2BYMA30_8F.ConveyorConfig("CV_8F", CV.MPLCNo, CV.InMemorySimulator, CV.UseMCProtocol);
                    Config_CV_8F.SetIPAddress(CV.MPLCIP);
                    Config_CV_8F.SetPort(CV.MPLCPort);
                    conveyorController_8F = new ASRS.Conveyor.V2BYMA30_8F.ConveyorController(Config_CV_8F);
                    conveyorController_8F.Start();
                    //_mainView_1F = new Mirle.ASRS.Conveyor.V2BYMA30_1F.View.MainView(conveyorController_8F);
                    break;
                
                case 2:
                    var Config_CV_10F = new ASRS.Conveyor.V2BYMA30_10F.ConveyorConfig("CV_10F", CV.MPLCNo, CV.InMemorySimulator, CV.UseMCProtocol);
                    Config_CV_10F.SetIPAddress(CV.MPLCIP);
                    Config_CV_10F.SetPort(CV.MPLCPort);
                    conveyorController_10F = new ASRS.Conveyor.V2BYMA30_10F.ConveyorController(Config_CV_10F);
                    conveyorController_10F.Start();
                    //_mainView_8F = new Mirle.ASRS.Conveyor.V2BYMA30_8F.View.MainView(conveyorController_8F);
                    break;
                case 3:
                    var Config_CV_Ele = new ASRS.Conveyor.V2BYMA30_Elevator.ConveyorConfig("CV_Elevator", CV.MPLCNo, CV.InMemorySimulator, CV.UseMCProtocol);
                    Config_CV_Ele.SetIPAddress(CV.MPLCIP);
                    Config_CV_Ele.SetPort(CV.MPLCPort);
                    conveyorController_Elevator = new ASRS.Conveyor.V2BYMA30_Elevator.ConveyorController(Config_CV_Ele);
                    conveyorController_Elevator.Start();
                    //_mainView_Elevator = new Mirle.ASRS.Conveyor.V2BYMA30_Elevator.View.MainView(conveyorController_Elevator);
                    break;
            }

           
          
        }


        //public static string GetCVAlarm(int bufferIndex, int alarmBit)
        //{
        //    string[] alarm = CV_Alarm[bufferIndex];
        //    return alarm[alarmBit];
        //}

        public static int GetBufferCount()
        {
            return SignalMapper.BufferCount;
        }


        //public static ConveyorController GetConveyorControllerHost(int iCVNo)
        //{
        //    switch (iCVNo)
        //    {
        //        case 1:
        //            return conveyorController_3F;
        //        case 2:
        //            return conveyorController_5F;
        //        case 3:
        //            return conveyorController_6F;
        //        case 4:
        //            return conveyorController_8F;
        //    }
        
        //}

      
        //public static ASRS.Conveyor.V2BYMA30_1F.ConveyorController GetConveyorController_1F()
        //{
        //    return conveyorController_1F;
        //}

        public static ASRS.Conveyor.V2BYMA30_8F.ConveyorController GetConveyorController_8F()
        {
            return conveyorController_8F;
        }

        public static ASRS.Conveyor.V2BYMA30_10F.ConveyorController GetConveyorController_10F()
        {
            return conveyorController_10F;
        }

        public static ASRS.Conveyor.V2BYMA30_Elevator.ConveyorController GetConveyorController_Elevator()
        {
            return conveyorController_Elevator;
        }

        
        

        /// <summary>
        /// 回報當下的樓層
        /// </summary>
        /// <returns></returns>
        public static int CheckElevatorFloor()
        {
            return conveyorController_Elevator.Signal.Floor.GetValue();
            //return conveyorController_Elevator.GetBuffer(0).GetTrayID;            
        }

        /// <summary>
        /// 指定樓層的外面兩節conveyor
        /// </summary>
        /// <param name="Floor"></param>
        /// <param name="conveyor1"></param>
        /// <param name="conveyor2"></param>
        /// <returns></returns>
        public static bool ReportCurrentConveyor(int Floor, ref ConveyorInfo conveyor1 , ref ConveyorInfo conveyor2)
        {
            switch (Floor)
            {
                case 8:
                    conveyor1 = ConveyorDef.LO2_01;
                    conveyor2 = ConveyorDef.LO2_02;
                    break;            

                case 10:
                    conveyor1 = ConveyorDef.LO1_03;
                    conveyor2 = ConveyorDef.LO1_04;
                    break;
            }
            return true;
        }

        public static bool CheckElevatorIsAgv()
        {
            return conveyorController_Elevator.Signal.EleStatus.AgvMode.IsOn();
        }
        public static bool CheckElevatorPlatformOn()
        {
            return conveyorController_Elevator.Signal.EleStatus.PlatformOn.IsOn();
        }

        public static bool CheckElevatorPlatformOff()
        {
            return conveyorController_Elevator.Signal.EleStatus.PlatformOff.IsOn();
        }

        public static bool CheckElevatorIsIdle()
        {
            return conveyorController_Elevator.Signal.EleStatus.Idle.IsOn(); 
        }

        public static bool CheckElevatorIsBusy()
        {
            return conveyorController_Elevator.Signal.EleStatus.Running.IsOn();        
        }

        public static bool CheckElevatorIsUp()
        {
            return conveyorController_Elevator.Signal.EleStatus.Up.IsOn();       
        }

        public static bool CheckElevatorIsDown()
        {
            return conveyorController_Elevator.Signal.EleStatus.Down.IsOn();            
        }

        public static bool CheckElevatorisEmpty(ConveyorInfo conveyor)
        {
            //ConveyorInfo buffer1 = new ConveyorInfo();
            if (conveyorController_Elevator. GetBuffer(conveyor.Index).Position == true)
            {
                return true;
            }
            else return false;
        } 


        public static ConveyorInfo GetBufferByStnNo(string sStnNo , ref int Floor )
        {
            if (sStnNo == ConveyorDef.LO1_01.BufferName)
            {
                Floor = 10;
                return ConveyorDef.LO1_01;
            }
            else if (sStnNo == ConveyorDef.LO1_02.BufferName)
            {
                Floor = 10;
                //iReadAck = 1;
                return ConveyorDef.LO1_02;
            }
            else if (sStnNo == ConveyorDef.LO1_04.BufferName)
            {
                Floor = 10;
                return ConveyorDef.LO1_04;
            }
            else if (sStnNo == ConveyorDef.LO1_05.BufferName)
            {
                Floor = 10;
                return ConveyorDef.LO1_05;
            }
            else if (sStnNo == ConveyorDef.LO1_07.BufferName)
            {
                Floor = 10;
                //iReadAck = 1;
                return ConveyorDef.LO1_07;
            }
            else if (sStnNo == ConveyorDef.LO1_08.BufferName)
            {
                Floor = 10;
                return ConveyorDef.LO1_08;
            }

            else if (sStnNo == ConveyorDef.LO2_01.BufferName)
            {
                Floor = 8;
                return ConveyorDef.LO2_01;
            }
            else if (sStnNo == ConveyorDef.LO2_02.BufferName)
            {
                Floor = 8;
                return ConveyorDef.LO2_02;
            }
            else if (sStnNo == ConveyorDef.LO2_03.BufferName)
            {
                Floor = 8;
                return ConveyorDef.LO2_03;
            }
            else if (sStnNo == ConveyorDef.LO2_04.BufferName)
            {
                Floor = 8;
                //iReadAck = 1;
                return ConveyorDef.LO2_04;
            }
            else return null;
        }

        public static bool ChkFinalBcrBufferID(string sBufferID)
        {
            if (sBufferID == "LO1-07" || sBufferID == "LO2-04"   )
            {
                return true;
            }
            else
            {
                return false;
            }
        
        }

        public static string GetCmdModeByBufferID(string sBufferID)
        {
            string sCmdMode = "";
            switch (sBufferID)
            {
                //目的
                case "LO1-08":
                case "LO2-01":
                    sCmdMode = clsConstValue.CmdMode.StockOut;
                    break;
                case "LO2-04":              
                    sCmdMode = clsConstValue.CmdMode.StockIn;
                    break;
            
                default:
                    sCmdMode = clsConstValue.CmdMode.StockIn;

                    break;
            }

            return sCmdMode;

        }

        public static int GetCarrieTypeByName(string sCarrierType)
        {
            int iNo = 0;
            switch (sCarrierType)
            {
                case "BIN":
                    iNo = clsConstValue.CarrierType.BIN; 
                    break;
                case "MAG":
                    iNo = clsConstValue.CarrierType.MAG;
                    break;
            }

            return iNo;
        }

        public static int GetFloorByBufferID(string sBufferID)
        {
            int iFloor = 0;
            switch (sBufferID)
            {
                //目的
                case "LO1-02":
                case "LO1-07":
                    iFloor = clsConstValue.FloorPath.Floor_10;
                    break;            
        

                default:
                    iFloor = clsConstValue.FloorPath.Floor_8;
                    break;
            }

            return iFloor;

        }

        public static string GetGesByBufferID(string sBufferID)
        {
            string sDes = "";
            switch (sBufferID)
            {
                //來源
                case "LO1-02":
                    sDes = ConveyorDef.LO2_04.BufferName;                 
                    break;
                default:
                    sDes = ConveyorDef.LO1_08.BufferName;
                    break;
            }

            return sDes;

        }

        public static Form GetMainView(int iPLC)
        {
            //int a = 0;
            Form _mainView = new Form();
            switch (iPLC)
            {
                case 1:
                    _mainView_8F = new Mirle.ASRS.Conveyor.V2BYMA30_8F.View.MainView(conveyorController_8F);
                    _mainView = _mainView_8F;
                    break;
          
                case 2:
                    _mainView_10F = new Mirle.ASRS.Conveyor.V2BYMA30_10F.View.MainView(conveyorController_10F);
                    _mainView = _mainView_10F;
                    break;
                case 3:
                    _mainView_Elevator = new Mirle.ASRS.Conveyor.V2BYMA30_Elevator.View.MainView(conveyorController_Elevator);
                    _mainView = _mainView_Elevator;
                    break;
            }

           

            return _mainView;
        }

        //public static MainView GetMainView_Object()
        //{
        //    return _mainView;
        //}
    }
}
