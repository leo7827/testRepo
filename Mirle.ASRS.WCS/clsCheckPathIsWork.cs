using System;
using Mirle.Def.V2BYMA30;
using System.Linq;
using Mirle.Def;
using Mirle.LiteOn.V2BYMA30;

namespace Mirle.ASRS.WCS
{
    public class clsCheckPathIsWork
    {
        //private bool[] bLoad_Left_Pre = new bool[4];
        //private bool[] bLoad_Right_Pre = new bool[4];

        private System.Timers.Timer timRead = new System.Timers.Timer();
        public clsCheckPathIsWork()
        {
            timRead.Elapsed += new System.Timers.ElapsedEventHandler(timRead_Elapsed);
            timRead.Enabled = false; timRead.Interval = 100;
        }

        public void subStart()
        {
            timRead.Enabled = true;
        }

        private void timRead_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            timRead.Enabled = false;
            try
            {
                SubEnablePath(true);

                //for (int i = 0; i < 4; i++)
                //{
                //    var crane = clsMicronStocker.GetStockerById(i + 1).GetCraneById(1);
                //    var LeftHasCarrier = (crane.GetForkById(1).HasCarrier || !crane.GetForkById(1).GetConfig().Enable);
                //    var RightHasCarrier = (crane.GetForkById(2).HasCarrier || !crane.GetForkById(2).GetConfig().Enable);

                //    if (RightHasCarrier != bLoad_Right_Pre[i])
                //    {
                //        if (RightHasCarrier)
                //        {
                //            SubEnablePath(i + 1, clsEnum.Fork.Right, false);
                //        }
                //        else
                //        {
                //            SubEnablePath(i + 1, clsEnum.Fork.Right, true);
                //        }

                //        bLoad_Right_Pre[i] = RightHasCarrier;
                //    }

                //    if (LeftHasCarrier != bLoad_Left_Pre[i])
                //    {
                //        if(LeftHasCarrier)
                //        {
                //            SubEnablePath(i + 1, clsEnum.Fork.Left, false);
                //        }
                //        else
                //        {
                //            SubEnablePath(i + 1, clsEnum.Fork.Left, true);
                //        }

                //        bLoad_Left_Pre[i] = LeftHasCarrier;
                //    }
                //}
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                timRead.Enabled = true;
            }
        }

        private void SubEnablePath( bool enable)
        {
            //3樓
            var LO4_02 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO4_02.ToString());
            var LO4_04 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO4_04.ToString());

            //5樓
            var LO5_02 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO5_02.ToString());
            var LO5_04 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO5_04.ToString());

            //6樓
            var LO6_02 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO6_02.ToString());
            var LO6_04 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO6_04.ToString());

            //8樓
            var LO3_01 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO3_01.ToString());
            var LO3_04 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO3_04.ToString());
            
            //電梯
            var LI2_01 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LI2_01.ToString());
            //var LI2_02 = MicronLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LI2_02.ToString());
            //var LI2_03 = MicronLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LI2_03.ToString());
            var LI2_04 = LiteOnLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LI2_04.ToString());

            LiteOnLocation.GetRouteService().EnalePath(LO4_02, LI2_01, enable);
            LiteOnLocation.GetRouteService().EnalePath(LI2_01, LO3_01, enable);
            LiteOnLocation.GetRouteService().EnalePath(LO3_04, LI2_04, enable);
            LiteOnLocation.GetRouteService().EnalePath(LI2_04, LO4_04, enable);

            //var A1_04 = MicronLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_04.ToString());
            //var A1_05 = MicronLocation.GetMicronLocationById(1).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_05.ToString());
            //var A1_10 = MicronLocation.GetMicronLocationById(2).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_10.ToString());
            //var A1_11 = MicronLocation.GetMicronLocationById(2).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_11.ToString());
            //var A1_16 = MicronLocation.GetMicronLocationById(3).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_16.ToString());
            //var A1_17 = MicronLocation.GetMicronLocationById(3).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_17.ToString());
            //var A1_22 = MicronLocation.GetMicronLocationById(4).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_22.ToString());
            //var A1_23 = MicronLocation.GetMicronLocationById(4).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_23.ToString());

            //var Shelf = MicronLocation.GetMicronLocationById(StockerID).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.Shelf.ToString());
            //if (fork == clsEnum.Fork.Right)
            //{
            //    var RightFork = MicronLocation.GetMicronLocationById(StockerID).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.RightFork.ToString());
            //    MicronLocation.GetRouteService().EnalePath(Shelf, RightFork, enable);
            //    switch (StockerID)
            //    {
            //        case 1:
            //            MicronLocation.GetRouteService().EnalePath(A1_04, RightFork, enable);
            //            break;
            //        case 2:
            //            MicronLocation.GetRouteService().EnalePath(A1_10, RightFork, enable);
            //            break;
            //        case 3:
            //            MicronLocation.GetRouteService().EnalePath(A1_16, RightFork, enable);
            //            break;
            //        default:
            //            MicronLocation.GetRouteService().EnalePath(A1_22, RightFork, enable);
            //            break;
            //    }
            //}
            //else
            //{
            //    var LeftFork = MicronLocation.GetMicronLocationById(StockerID).GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LeftFork.ToString());
            //    MicronLocation.GetRouteService().EnalePath(Shelf, LeftFork, enable);
            //    switch (StockerID)
            //    {
            //        case 1:
            //            MicronLocation.GetRouteService().EnalePath(A1_04, LeftFork, enable);
            //            MicronLocation.GetRouteService().EnalePath(A1_05, LeftFork, enable);
            //            break;
            //        case 2:
            //            MicronLocation.GetRouteService().EnalePath(A1_10, LeftFork, enable);
            //            MicronLocation.GetRouteService().EnalePath(A1_11, LeftFork, enable);
            //            break;
            //        case 3:
            //            MicronLocation.GetRouteService().EnalePath(A1_16, LeftFork, enable);
            //            MicronLocation.GetRouteService().EnalePath(A1_17, LeftFork, enable);
            //            break;
            //        default:
            //            MicronLocation.GetRouteService().EnalePath(A1_22, LeftFork, enable);
            //            MicronLocation.GetRouteService().EnalePath(A1_23, LeftFork, enable);
            //            break;
            //    }
            //}
        }
    }
}
