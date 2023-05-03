using System;
using System.Collections.Generic;
using System.Linq;
using Mirle.Def;
using Mirle.Def.V2BYMA30;
using Mirle.Route;
using Mirle.Structure;

namespace Mirle.LiteOn.V2BYMA30
{
    public class LiteOnLocation
    {
        private static RouteController routeService = new RouteController();
        private static LiteOnLocationInfo[] STK = new LiteOnLocationInfo[4];

        public static LiteOnLocationInfo GetMicronLocationById(int StockerID)
        {
            return STK[StockerID - 1];
        }

        public static RouteController GetRouteService()
        {
            return routeService;
        }

        public static void FunMapPort(List<Element_Port>[] elements, string[] StockerID)
        {
            try
            {
                var ListResult = elements;
                #region 建立Location資訊
                for (int i = 0; i < ListResult.Length; i++)
                {
                    if (ListResult[i][0].DeviceID == StockerID[0])
                    {
                        #region Stocker1
                        STK[0] = new LiteOnLocationInfo(ListResult[i].Count);
                        for (int j = 0; j < ListResult[i].Count; j++)
                        {
                            STK[0].GetLocations[j] = new Location(ListResult[i][j].DeviceID, ListResult[i][j].HostPortID,
                                Def.Location.GetLocationTypesByPortType(ListResult[i][j].PortType));
                        }
                        #endregion Stocker1
                    }
                    else if (ListResult[i][0].DeviceID == StockerID[1])
                    {
                        #region Stocker2
                        STK[1] = new LiteOnLocationInfo(ListResult[i].Count);
                        for (int j = 0; j < ListResult[i].Count; j++)
                        {
                            STK[1].GetLocations[j] = new Location(ListResult[i][j].DeviceID, ListResult[i][j].HostPortID,
                                Def.Location.GetLocationTypesByPortType(ListResult[i][j].PortType));
                        }
                        #endregion Stocker2
                    }
                    else if (ListResult[i][0].DeviceID == StockerID[2])
                    {
                        #region Stocker3
                        STK[2] = new LiteOnLocationInfo(ListResult[i].Count);
                        for (int j = 0; j < ListResult[i].Count; j++)
                        {
                            STK[2].GetLocations[j] = new Location(ListResult[i][j].DeviceID, ListResult[i][j].HostPortID,
                                Def.Location.GetLocationTypesByPortType(ListResult[i][j].PortType));
                        }
                        #endregion Stocker3
                    }
                    else if (ListResult[i][0].DeviceID == StockerID[3])
                    {
                        #region Stocker4
                        STK[3] = new LiteOnLocationInfo(ListResult[i].Count);
                        for (int j = 0; j < ListResult[i].Count; j++)
                        {
                            STK[3].GetLocations[j] = new Location(ListResult[i][j].DeviceID, ListResult[i][j].HostPortID,
                                Def.Location.GetLocationTypesByPortType(ListResult[i][j].PortType));
                        }
                        #endregion Stocker4
                    }
                    else { }
                }
                #endregion 建立Location資訊
                FunAddInternalPath();
                FunAddExternalPath();
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        private static void FunAddInternalPath()
        {
            try
            {
                //var LI2_01 = STK[0].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LI2_01.ToString());
                //ConveyorDef.LI2_01.bufferLocation = LI2_01;

                //var LI2_02 = STK[0].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LI2_02.ToString());
                //ConveyorDef.LI2_01.bufferLocation = LI2_02;

                //var LI2_03 = STK[0].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LI2_03.ToString());
                //ConveyorDef.LI2_04.bufferLocation = LI2_03;

                //var LI2_04 = STK[0].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LI2_04.ToString());
                //ConveyorDef.LI2_04.bufferLocation = LI2_04;

                //var LO4_04 = STK[1].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO4_04.ToString());
                //ConveyorDef.LO4_04.bufferLocation = LO4_04;

                //var LO4_02 = STK[1].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO4_02.ToString());
                //ConveyorDef.LO4_02.bufferLocation = LO4_02;

                //var LO5_04 = STK[2].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO5_04.ToString());
                //ConveyorDef.LO5_04.bufferLocation = LO5_04;

                //var LO5_02 = STK[2].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO5_02.ToString());
                //ConveyorDef.LO5_02.bufferLocation = LO5_02;

                //var LO6_04 = STK[3].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO6_04.ToString());
                //ConveyorDef.LO6_04.bufferLocation = LO6_04;

                //var LO6_02 = STK[3].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LO6_02.ToString());
                //ConveyorDef.A1_20.bufferLocation = LO6_02;

                //var A1_01 = STK[0].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_01.ToString());
                //ConveyorDef.A1_01.bufferLocation = A1_01;

                //var A1_02 = STK[0].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_02.ToString());
                //ConveyorDef.A1_02.bufferLocation = A1_02;

                //var A1_04 = STK[0].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_04.ToString());
                //ConveyorDef.A1_04.bufferLocation = A1_04;

                //var A1_05 = STK[0].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_05.ToString());
                //ConveyorDef.A1_05.bufferLocation = A1_05;

                //var A1_07 = STK[1].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_07.ToString());
                //ConveyorDef.A1_07.bufferLocation = A1_07;

                //var A1_08 = STK[1].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_08.ToString());
                //ConveyorDef.A1_08.bufferLocation = A1_08;

                //var A1_10 = STK[1].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_10.ToString());
                //ConveyorDef.A1_10.bufferLocation = A1_10;

                //var A1_11 = STK[1].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_11.ToString());
                //ConveyorDef.A1_11.bufferLocation = A1_11;

                //var A1_13 = STK[2].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_13.ToString());
                //ConveyorDef.A1_13.bufferLocation = A1_13;

                //var A1_14 = STK[2].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_14.ToString());
                //ConveyorDef.A1_14.bufferLocation = A1_14;

                //var A1_16 = STK[2].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_16.ToString());
                //ConveyorDef.A1_16.bufferLocation = A1_16;

                //var A1_17 = STK[2].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_17.ToString());
                //ConveyorDef.A1_17.bufferLocation = A1_17;

                //var A1_19 = STK[3].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_19.ToString());
                //ConveyorDef.A1_19.bufferLocation = A1_19;

                //var A1_20 = STK[3].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_20.ToString());
                //ConveyorDef.A1_20.bufferLocation = A1_20;

                //var A1_22 = STK[3].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_22.ToString());
                //ConveyorDef.A1_22.bufferLocation = A1_22;

                //var A1_23 = STK[3].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.A1_23.ToString());
                //ConveyorDef.A1_23.bufferLocation = A1_23;

               
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        private static void SubAddInternalPath(Location[] device, Location left1, Location left2, Location right1, Location right2)
        {
            try
            {
                var Shelf = device.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.Shelf.ToString());
                var LeftFork = device.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.LeftFork.ToString());
                var RightFork = device.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.RightFork.ToString());
                var Teach = device.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.Teach.ToString());

                Location.AddPath_Single(routeService, LeftFork, left1);
                Location.AddPath_Single(routeService, LeftFork, left2);
                Location.AddPath_Single(routeService, RightFork, left1);
                //Def.Location.AddPath_Single(routeService, RightFork, left2);

                Location.AddPath_Double(routeService, LeftFork, right1);
                Location.AddPath_Double(routeService, LeftFork, right2);
                Location.AddPath_Double(routeService, LeftFork, Shelf);
                Location.AddPath_Double(routeService, LeftFork, Teach);

                Location.AddPath_Double(routeService, RightFork, right1);
                //Def.Location.AddPath_Double(routeService, RightFork, right2);
                Location.AddPath_Double(routeService, RightFork, Shelf);
                Location.AddPath_Double(routeService, RightFork, Teach);
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        private static void FunAddExternalPath()
        {
            //                                     FROM                               TO 
            //routeService.AddDevicePath(ConveyorDef.LO4_02.bufferLocation, ConveyorDef.LI2_01.bufferLocation);   //三樓進電梯
            //routeService.AddDevicePath(ConveyorDef.LO5_02.bufferLocation, ConveyorDef.LI2_01.bufferLocation);   //五樓進電梯
            //routeService.AddDevicePath(ConveyorDef.LO6_02.bufferLocation, ConveyorDef.LI2_01.bufferLocation);   //六樓進電梯
            //routeService.AddDevicePath(ConveyorDef.LO3_03.bufferLocation, ConveyorDef.LI2_01.bufferLocation);   //八樓進電梯
            //routeService.AddDevicePath(ConveyorDef.LI2_04.bufferLocation, ConveyorDef.LO4_04.bufferLocation);   //三樓出電梯
            //routeService.AddDevicePath(ConveyorDef.LI2_04.bufferLocation, ConveyorDef.LO5_04.bufferLocation);   //五樓出電梯
            //routeService.AddDevicePath(ConveyorDef.LI2_04.bufferLocation, ConveyorDef.LO6_04.bufferLocation);   //六樓出電梯
            //routeService.AddDevicePath(ConveyorDef.LI2_04.bufferLocation, ConveyorDef.LO3_01.bufferLocation);   //八樓出電梯


            #region
            //routeService.AddDevicePath(ConveyorDef.A1_01.bufferLocation, ConveyorDef.A1_11.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_01.bufferLocation, ConveyorDef.A1_17.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_01.bufferLocation, ConveyorDef.A1_23.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_02.bufferLocation, ConveyorDef.A1_11.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_02.bufferLocation, ConveyorDef.A1_17.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_02.bufferLocation, ConveyorDef.A1_23.bufferLocation);

            //routeService.AddDevicePath(ConveyorDef.A1_07.bufferLocation, ConveyorDef.A1_05.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_07.bufferLocation, ConveyorDef.A1_17.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_07.bufferLocation, ConveyorDef.A1_23.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_08.bufferLocation, ConveyorDef.A1_05.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_08.bufferLocation, ConveyorDef.A1_17.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_08.bufferLocation, ConveyorDef.A1_23.bufferLocation);

            //routeService.AddDevicePath(ConveyorDef.A1_13.bufferLocation, ConveyorDef.A1_11.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_13.bufferLocation, ConveyorDef.A1_05.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_13.bufferLocation, ConveyorDef.A1_23.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_14.bufferLocation, ConveyorDef.A1_11.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_14.bufferLocation, ConveyorDef.A1_05.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_14.bufferLocation, ConveyorDef.A1_23.bufferLocation);

            //routeService.AddDevicePath(ConveyorDef.A1_19.bufferLocation, ConveyorDef.A1_11.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_19.bufferLocation, ConveyorDef.A1_17.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_19.bufferLocation, ConveyorDef.A1_05.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_20.bufferLocation, ConveyorDef.A1_11.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_20.bufferLocation, ConveyorDef.A1_17.bufferLocation);
            //routeService.AddDevicePath(ConveyorDef.A1_20.bufferLocation, ConveyorDef.A1_05.bufferLocation);
            #endregion
        }

        public static Location GetLocation(int deviceID, LocationDef.Location emuLoc)
        {
            try
            {
                return STK[deviceID - 1].GetLocations.FirstOrDefault(loc => loc.LocationId == emuLoc.ToString());
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return null;
            }
        }

        public static Location GetLocation(string CurDeviceID, string CurLoc)
        {
            try
            {
                return STK[int.Parse(CurDeviceID) - 1].GetLocations.FirstOrDefault(loc => loc.LocationId == CurLoc);
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return null;
            }
        }

        public static Location GetLocation_ByShelf(int StockerID)
        {
            try
            {
                return STK[StockerID - 1].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.Shelf.ToString());
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return null;
            }
        }

        public static Location GetLocation_ByTeach(int StockerID)
        {
            try
            {
                return STK[StockerID - 1].GetLocations.FirstOrDefault(loc => loc.LocationId == LocationDef.Location.Teach.ToString());
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return null;
            }
        }

        public static Location GetLocation_ByStockOutPort(int StockerID)
        {
            try
            {
                LocationDef.Location location;
                switch(StockerID)
                {
                    case 1:
                        location = LocationDef.Location.A1_01;
                        break;
                    case 2:
                        location = LocationDef.Location.A1_07;
                        break;
                    case 3:
                        location = LocationDef.Location.A1_13;
                        break;
                    default:
                        location = LocationDef.Location.A1_19;
                        break;
                }

                return STK[StockerID - 1].GetLocations.FirstOrDefault(loc => loc.LocationId == location.ToString());
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return null;
            }
        }
 

        public static bool GetPath(Location Start, Location End, ref Location Now_Start, ref Location Now_End)
        {
            try
            {
                var path = routeService.GetPath(Start, End);
                if (!path.Any())
                {
                    return false;
                }

                int iRow_Path = 0;
                foreach (Location p in path)
                {
                    if (iRow_Path > 1) break;
                    if (iRow_Path == 0)
                    {
                        Now_Start = p;
                    }
                    else
                    {
                        Now_End = p;
                    }

                    iRow_Path++;
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
    }
}
