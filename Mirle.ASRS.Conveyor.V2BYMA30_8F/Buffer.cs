using Mirle.ASRS.Conveyor.V2BYMA30_8F.Events;
using Mirle.ASRS.Conveyor.V2BYMA30_8F.Signal;
using Mirle.MPLC.DataType;
using System;
using System.Reflection;
using Mirle.ASRS.Conveyor.V2BYMA30_8F.Service;
using System.Threading.Tasks;
using Mirle.Extensions;
using Mirle.Def;

namespace Mirle.ASRS.Conveyor.V2BYMA30_8F
{
    public class Buffer
    {
        public delegate void AlarmBitEventHandler(object sender, AlarmBitEventArgs e);
        public delegate void AlarmBitEventHandler_2(object sender, AlarmBitEventArgs e);
        public delegate void NoticeReleaseEventHandler(object sender, ReqAckEventArgs e);
        public delegate void BufferStatusEventHandler(object sender, BufferEventArgs e);
        public delegate void BufferPresenceEventHandler(object sender, BufferEventArgs e);

        public event AlarmBitEventHandler OnAlarmBitChanged;
        public event AlarmBitEventHandler OnAlarmBitChanged_2;
        public event NoticeReleaseEventHandler OnIniatlNotice;
        public event BufferStatusEventHandler OnStatusChanged;
        public event BufferPresenceEventHandler OnPresenceChanged;

        private readonly bool[] _AlarmBit = new bool[16];
        private readonly bool[] _AlarmBit_2 = new bool[16];
        private bool _InitalReport = false;
        private DateTime _nextCanPortModeChangeTime = DateTime.Now;
        private clsEnum.WmsApi.EqSts _lastStatus = clsEnum.WmsApi.EqSts.StockOutOnly;
        private bool _lastPresence = false;
        private bool _transferReportNotice_PC = false;  //貨物定位通知
        private bool _bcrCheckRequestNotice_PC = false;  //是否已經上報bcr req
        public BufferSignal Signal { get; }

        public bool TransferReportNotice_PC
        {
            get { return _transferReportNotice_PC; }
        }

        public bool BcrCheckRequestNotice_PC
        {
            get { return _bcrCheckRequestNotice_PC; }
        }

        public string CommandID
        {
            get
            {
                if (Signal.CommandID.GetValue() == 0) return "";
                else return Signal.CommandID.GetValue().ToString().PadLeft(5, '0');
            }
        }

        public string CommandID_PC
        {
            get
            {
                if (Signal.Controller.CommandID.GetValue() == 0) return "";
                else return Signal.Controller.CommandID.GetValue().ToString().PadLeft(5, '0');
            }
        }

    

        public int RollNotice_PC => Signal.Controller.RollNotice.GetValue();

        public int CarrierTypeNotice_PC => Signal.Controller.CarrierTypeNotice.GetValue();

        public int CommandMode => Signal.CommandMode.GetValue();
        public int CommandMode_PC => Signal.Controller.CommandMode.GetValue();
        public int PathNotice => Signal.PathNotice.GetValue();
        public int PathNotice_PC => Signal.Controller.PathNotice.GetValue();
        public int Ready => Signal.Ready.GetValue();

        public int CarrierType => Signal.CarrierType.GetValue();
        public int RollNotice => Signal.RollNotice.GetValue();
        public int ReadBcrAck => Signal.AckSignal.ReadBcrAck.GetValue();
        public int ReadBcrReq_PC => Signal.RequestController.ReadBcrReq.GetValue();
        //public int BcrCheckRequestNotice_PC => Signal.Controller.BcrCheckRequestNotice.GetValue();

        //public int TransferReportNotice_PC => Signal.Controller.TransferReportNotice.GetValue();

        public int InitialNotice => Signal.AckSignal.InitalAck.GetValue();
        public int InitialNotice_PC => Signal.RequestController.InitalReq.GetValue();

        //V 1.0.0.5
        public bool IniACK => Signal.AckSignal.IniSignalACK.IniACK.IsOn();

        public bool PathFinishACK => Signal.AckSignal.IniSignalACK.PathFinishACK.IsOn();

        public bool IniReq_PC => Signal.RequestController.IniSignalReq.IniReq.IsOn();
        
        public bool PathFinish_PC => Signal.RequestController.IniSignalReq.PathFinish.IsOn();

        /// <summary>
        /// 手動入庫通知 (0: 自動入庫，1: 手動入庫)
        /// </summary>
        //public int ManualPutawayNotice => Signal.Controller.Manual_Putaway.GetValue();
        public bool IsManualPutaway => Signal.Controller.Manual_Putaway.GetValue() == 1;
        public bool InMode => Signal.StatusSignal.InMode.IsOn();
        public bool OutMode => Signal.StatusSignal.OutMode.IsOn();
        public bool Position => Signal.StatusSignal.Position.IsOn();
        public bool Presence => Signal.StatusSignal.Presence.IsOn();
        public bool Error => Signal.StatusSignal.Error.IsOn();
        public bool Auto => Signal.StatusSignal.Auto.IsOn();
        public bool Manual => Signal.StatusSignal.Manual.IsOn();
        public bool EMO => Signal.StatusSignal.EMO.IsOn();
        public bool Finish => Signal.StatusSignal.Finish.IsOn();
        public bool Cylinder => Signal.StatusSignal.Cylinder.IsOn();

        public bool Empty => Signal.StatusSignal.Empty.IsOn();

       
        public bool WriteTransferReport(int Value)
        {
            if (Value == 1)
            {
                _transferReportNotice_PC = true;
                return true;
            }
            else
            {
                _transferReportNotice_PC = false;
                return true;
            }

        }

        public bool WriteBcrSetReadReq(int Value)
        {
            if (Value == 1)
            {
                _bcrCheckRequestNotice_PC = true;
                return true;
            }
            else
            {
                _bcrCheckRequestNotice_PC = false;
                return true;
            }

        }

        public string GetTrayID
        {
            get
            {
                try
                {
                    if (Signal.BCRResultSignal.TrayID.ID != null)
                    {
                        return Signal.BCRResultSignal.TrayID.ID.GetData().ToASCII();
                    }
                    else
                    {
                        return "";
                    }
                }
                catch
                {
                    return "";
                }
            }
        }

        public string GetFobIdByLocation(int loc)
        {
            try
            {
                if (Signal.BCRResultSignal.GetFobBcrByNum(loc).ID != null)
                {
                    return Signal.BCRResultSignal.GetFobBcrByNum(loc).ID.GetData().ToASCII();
                }
                else return "";
            }
            catch
            {
                return "";
            }
        }

        public string GetLittleThingIdByLocation(int loc)
        {
            try
            {
                if (Signal.BCRResultSignal.GetLittleThingBcrByNum(loc).ID != null)
                {
                    return Signal.BCRResultSignal.GetLittleThingBcrByNum(loc).ID.GetData().ToASCII();
                }
                else return "";
            }
            catch
            {
                return "";
            }
        }

        private readonly LoggerService _LoggerService;

        public Buffer(BufferSignal signal, string ConveyorId)
        {
            _LoggerService = new LoggerService(ConveyorId);
            Signal = signal;
        }

        public clsEnum.WmsApi.EqSts Status
        {
            get
            {
                if (!Auto || Error) return clsEnum.WmsApi.EqSts.Down;
                else return clsEnum.WmsApi.EqSts.Run;
            }
        }

        public Task<bool> SetPickupBitAsync(int bit, bool bFlag)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (bFlag) Signal.Controller.Pickup.PickupBit[bit].Checked.SetOn();
                    else Signal.Controller.Pickup.PickupBit[bit].Checked.SetOff();

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> SetPickupFinishBitAsync(int bit)
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.Controller.PickupFinish_Req.PickupBit[bit].Checked.SetOn();

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        /// <summary>
        /// 通知滾動
        /// </summary>
        /// <returns></returns>
        public Task<bool> InfoRolling()
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.Controller.RollNotice.SetValue(1);

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> SetManualPutaway()
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.Controller.Manual_Putaway.SetValue(1);

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }
        
        public Task<bool> CallEle(int Path)
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.Controller.CallEleNotice.SetValue(1);

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> WriteCommandAndCarrierType(string Command, int commandMode, int iCarrierType)
        {
            return Task.Run(() =>
            {
                try
                {
                    //if (string.IsNullOrWhiteSpace(CommandID)) return false;

                    Signal.Controller.CommandMode.SetValue(commandMode);
                    Signal.Controller.CarrierTypeNotice.SetValue(iCarrierType);
                    Signal.Controller.CommandID.SetValue(Convert.ToInt32(Command));

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }
        public Task<bool> WriteCommandAsync(string Command, int commandMode, int Path)
        {
            return Task.Run(() =>
            {
                try
                {
                    //if (string.IsNullOrWhiteSpace(CommandID)) return false;

                    Signal.Controller.CommandMode.SetValue(commandMode);
                    Signal.Controller.PathNotice.SetValue(Path);
                    Signal.Controller.CommandID.SetValue(Convert.ToInt32(Command));

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }
       

        public Task<bool> WriteCommandAndRolling(string Command, int commandMode)
        {
            return Task.Run(() =>
            {
                try
                {
                    //if (!string.IsNullOrWhiteSpace(CommandID)) return false;

                    Signal.Controller.CommandMode.SetValue(commandMode);
                    Signal.Controller.RollNotice.SetValue(1);
                    Signal.Controller.CommandID.SetValue(Convert.ToInt32(Command));

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> WriteRolling()
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.Controller.RollNotice.SetValue(1);

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }
        public Task<bool> WriteCommandAndSetPathAndRoll(string Command, string CommandMode, int Path)
        {
            return Task.Run(() =>
            {
                try
                {
                    //if (!string.IsNullOrWhiteSpace(CommandID)) return false;

                    Signal.Controller.CommandMode.SetValue(Convert.ToInt32(CommandMode));
                    Signal.Controller.PathNotice.SetValue(Path);
                    Signal.Controller.RollNotice.SetValue(1);
                    Signal.Controller.CommandID.SetValue(Convert.ToInt32(Command));

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> WriteCommandAndSetReadReqAsync(string Command, int commandMode )
        {
            return Task.Run(() =>
            {
                try
                {
                    //if (string.IsNullOrWhiteSpace(CommandID)) return false;

                    Signal.Controller.CommandMode.SetValue(commandMode); 
                    Signal.RequestController.IniSignalReq.PathFinish.SetOn();
                    //Signal.RequestController.ReadBcrReq.SetValue(iReadAck);
                    Signal.Controller.CommandID.SetValue(Convert.ToInt32(Command));

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> WritePathAndReadReqAsync(int Path)
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.Controller.PathNotice.SetValue(Path);
                    Signal.RequestController.ReadBcrReq.SetValue(1);

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        //public Task<bool> WriteTransferReport(int Value)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            Signal.Controller.TransferReportNotice.SetValue(Value);

        //            Task.Delay(500).Wait();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
        //            return false;
        //        }
        //    });
        //}

        //public Task<bool> WriteBcrSetReadReq(int value)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            Signal.Controller.BcrCheckRequestNotice.SetValue(value);

        //            Task.Delay(500).Wait();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
        //            return false;
        //        }
        //    });
        //}

        /// <summary>
        /// 通知初始
        /// </summary>
        /// <returns></returns>
        public Task<bool> NoticeInital()
        {
            return Task.Run(() =>
            {
                try
                {
                    //V 1.0.0.4
                    Signal.RequestController.IniSignalReq.IniReq.SetOn();
                    //Signal.RequestController.InitalReq.SetValue(1);
                    Task.Delay(500).Wait();
                    return true;
                }
                catch(Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> SetReadReq()
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.RequestController.ReadBcrReq.SetValue(1);
                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> SetReadReq(int value)
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.RequestController.ReadBcrReq.SetValue(value);
                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> WriteCarrierType(int iCarrierType)
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.Controller.CarrierTypeNotice.SetValue(iCarrierType);

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }


        /// <summary>
        /// 通知滾動
        /// </summary>
        /// <returns></returns>
        public Task<bool> InfoRolling(int value)
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.Controller.RollNotice.SetValue(value);

                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }

        public Task<bool> PathFinish()
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.RequestController.IniSignalReq.PathFinish.SetOff();
                    Task.Delay(500).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            });
        }
        public void Refresh()
        {
            try
            {
                ClearController();
            }
            catch(Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }

            //CheckReqAckStatus(ref _InitalReport, OnIniatlNotice, Signal.RequestController.InitalReq, Signal.AckSignal.InitalAck);
            CheckStatus();
            CheckPresence();

            for (int i = 0; i < 16; i++)
            {
                CheckAlarmBit(ref _AlarmBit[i], OnAlarmBitChanged, i, Signal.AlarmBitSignal.AlarmBit[i].Checked);
                CheckAlarmBit(ref _AlarmBit_2[i], OnAlarmBitChanged_2, i, Signal.AlarmBitSignal_2.AlarmBit[i].Checked);
            }
        }

        private void CheckStatus()
        {
            var newStatus = this.Status;
            if (_lastStatus != newStatus)
            {
                _lastStatus = newStatus;
                var args = new BufferEventArgs(Signal.BufferIndex) { NewStatus = newStatus };
                OnStatusChanged?.Invoke(this, args);
            }
        }

        private void CheckPresence()
        {
            var newPresence = Presence || !string.IsNullOrWhiteSpace(CommandID);
            if(_lastPresence != newPresence)
            {
                _lastPresence = newPresence;
                var args = new BufferEventArgs(Signal.BufferIndex) { Presence = newPresence };
                OnPresenceChanged?.Invoke(this, args);
            }
        }

        private void ClearController()
        {
            try
            {
                if (Signal.CommandID.GetValue() > 0 && Signal.CommandID.GetValue() == Signal.Controller.CommandID.GetValue())
                {
                    Signal.Controller.CommandID.SetValue(0);
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }

            try
            {
                if (Signal.CommandMode.GetValue() > 0 && Signal.CommandMode.GetValue() == Signal.Controller.CommandMode.GetValue())
                {
                    Signal.Controller.CommandMode.SetValue(0);
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }

            try
            { 
                //荷無 & 有上報的跡象就清掉 bcr request
                if (BcrCheckRequestNotice_PC && Signal.StatusSignal.Presence.IsOff() )
                //if (BcrCheckRequestNotice_PC && Signal.AckSignal.ReadBcrAck.GetValue() == 0 && Signal.RequestController.ReadBcrReq.GetValue() == 0 && Signal.CommandID.GetValue() > 0 && Signal.PathNotice.GetValue() > 0)
                {
                    WriteBcrSetReadReq(0);
                }

            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }
            try
            {
                if (Signal.PathNotice.GetValue() > 0 && Signal.PathNotice.GetValue() == Signal.Controller.PathNotice.GetValue())
                {
                    Signal.Controller.PathNotice.SetValue(0);
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }

            //try
            //{
            //    if (Signal.AckSignal.ReadBcrAck.GetValue() > 0 && Signal.AckSignal.ReadBcrAck.GetValue() == Signal.RequestController.ReadBcrReq.GetValue())
            //    {
            //        Signal.RequestController.ReadBcrReq.SetValue(0);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            //}


            try
            {                
                if (Signal.RollNotice.GetValue() > 0 && Signal.RollNotice.GetValue() == Signal.Controller.RollNotice.GetValue())
                {
                    Signal.Controller.RollNotice.SetValue(0);
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }

            try
            {
                if (Signal.Controller.CarrierTypeNotice.GetValue() != 0 && Signal.CarrierType.GetValue() == Signal.Controller.CarrierTypeNotice.GetValue())
                {
                    Signal.Controller.CarrierTypeNotice.SetValue(0);
                }
            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }

            try
            {
                if (Signal.RequestController.ReadBcrReq.GetValue() != 0 && Signal.AckSignal.ReadBcrAck.GetValue() == 0)
                {
                    Signal.RequestController.ReadBcrReq.SetValue(0);
                }

            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }

            try
            {
                if (Signal.Controller.BcrCheckRequestNotice.GetValue() != 0 && Signal.AckSignal.ReadBcrAck.GetValue() == 0 && Signal.CommandID.GetValue() > 0)
                {
                    Signal.Controller.BcrCheckRequestNotice.SetValue(0);
                }

            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }

            
            try
            {
                if (Signal.AckSignal.IniSignalACK.IniACK.IsOn() && Signal.RequestController.IniSignalReq.IniReq.IsOn())
                {
                    Signal.RequestController.IniSignalReq.IniReq.SetOff();
                }

            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }


            try
            {
                if (Signal.AckSignal.IniSignalACK.PathFinishACK.IsOn() && Signal.RequestController.IniSignalReq.PathFinish.IsOn() )
                {
                    Signal.RequestController.IniSignalReq.PathFinish.SetOff();
                }

            }
            catch (Exception ex)
            {
                _LoggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }

        }

        private void CheckAlarmBit(ref bool reportedFlag, AlarmBitEventHandler eventHandler, int bit, Bit alarmBit)
        {
            if (alarmBit.IsOn())
            {
                if (reportedFlag == false && alarmBit.IsOn())
                {
                    reportedFlag = true;
                    eventHandler?.Invoke(this, new AlarmBitEventArgs(Signal.BufferIndex, bit, true));
                }
            }
            else
            {
                if (reportedFlag && alarmBit.IsOff())
                {
                    reportedFlag = false;
                    eventHandler?.Invoke(this, new AlarmBitEventArgs(Signal.BufferIndex, bit, false));
                }
            }
        }

        private void CheckReqAckStatus(ref bool reportedFlag, NoticeReleaseEventHandler eventHandler, Word request, Word ack)
        {
            if (request.GetValue() == 1 && ack.GetValue() == 1)
            {
                if (reportedFlag == false)
                {
                    reportedFlag = true;
                    var args = new ReqAckEventArgs(Signal.BufferIndex);
                    eventHandler?.Invoke(this, args);
                }

                AckSignal(request, ack);
            }
            else if (request.GetValue() == 0 && ack.GetValue() == 0)
            {
                reportedFlag = false;
            }
        }

        private void AckSignal(Word req, Word ack)
        {
            if (ack.GetValue() > 0)
            {
                req.SetValue(0);
            }
        }
    }
}
