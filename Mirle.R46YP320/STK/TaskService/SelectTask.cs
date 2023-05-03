using System;
using System.Collections.Generic;
using System.Linq;

using Mirle.LCS.Models;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.TraceLog;
using Mirle.Stocker.TaskControl.TraceLog.Format;

namespace Mirle.R46YP320.STK.TaskService
{
    //V1. 如果可以考慮不同取同放的Case
    //1. By Priority 排序
    //2. 第一筆如果是 From, TO, SCAN, Move 就只取一筆, 另外 如果第一筆在極限區也只取一筆 //目前先取一筆 之後要改再改
    //3. 第一筆如果是 Transfer
    //   (1) 判斷是否有BatchID
    //       有Batch   (1.1)取下筆BatchID的命令, 如果沒有下筆相同BatchId的則選擇continue選下一筆
    //       沒有Batch (1.2)下筆如果有BatchID的則跳過不選, 直到選擇沒有 BatchId 的 TransferCmd
    //   (2) 判斷是否有ForkNumber的問題
    //       第一筆的ForkNumber = 1 第二筆就找ForkNumber = 0 or ForkNumber = 2 OK的
    //       第一筆的ForkNumber = 2 第二筆就找ForkNumber = 0 or ForkNumber = 1 OK的
    //   (3) 判斷是否有極限值的問題
    //       Crane1 的 右Fork 到不了 Bank1的第一個Bay
    //       Crane2 的 左Fork 到不了 Bank1的最後一個Bay
    //       Crane1 的 左Fork 到不了 Bank2的第一個Bay
    //       Crane2 的 右Fork 到不了 Bank2的最後一個Bay
    //   (4) 考慮同取
    //   (5) 考慮同放

    public class SelectTask
    {
        public string _MinBay;
        public string _MaxBay;
        private readonly LoggerService _LoggerService;
        public SelectTask(string minBay, string maxBay)
        {
            _MinBay = minBay;
            _MaxBay = maxBay;
        }
        public SelectTask(string minBay, string maxBay, LoggerService loggerService)
        {
            _MinBay = minBay;
            _MaxBay = maxBay;
            _LoggerService = loggerService;
        }
        public List<CommandTrace> GetCanExeCommands(IEnumerable<CommandTrace> commandTraces, CommandTrace trace, List<TwoShelf> twoSourceShelves, List<TwoShelf> twoDestShelves, int craneCurrentBay = 0)
        {
            List<CommandTrace> newCommandTrace = null;
            newCommandTrace = new List<CommandTrace>();
            //先取第一筆
            CommandTrace firstCommand = GetFirstCommand(commandTraces, trace);
            CommandTrace secendCommand = null;
            if (firstCommand == null)
                return null;

            //如果第一筆是 null 的就也不取第二筆
            else if (firstCommand != null)
            {
                newCommandTrace.Add(firstCommand);

                ////判斷第一筆是否在極限區, 如果是, 就不要取二筆
                //bool isLimitCommand = IsLimitCommand(isOnlyCrane1Mode, isOnlyCrane2Mode, limitBay, firstCommand);

                //_LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"GetCanExeCommands, IsLimitCommand:{isLimitCommand}"));

                //if (isLimitCommand)
                //    return null;
                //else
                //{
                //    //第一筆如果是 From, SCAN, Move 就只取一筆, 且第二筆要是TO or FROM_TO ///之後要改再改
                //    if (firstCommand.MainTransferMode == (int)TransferMode.FROM_TO || firstCommand.MainTransferMode == (int)TransferMode.TO)
                //    {
                //        //取下筆的時候 避免取到第一筆, 所以先排除第一筆的CommandID
                //        commandTraces = commandTraces.Where(i => i.CommandID != firstCommand.CommandID);
                //        //取第二筆
                //        secendCommand = GetSecendCommand(commandTraces, firstCommand, isOnlyCrane1Mode, isOnlyCrane2Mode, limitBay, _MinBay, _MaxBay, twoSourceShelves, twoDestShelves, craneCurrentBay);
                //        //如果不是 Null 再加入List
                //        if (secendCommand != null)
                //        {
                //            if ((firstCommand.MainTransferMode == (int)TransferMode.FROM_TO && secendCommand.MainTransferMode == (int)TransferMode.TO) ||
                //                (firstCommand.MainTransferMode == (int)TransferMode.TO && secendCommand.MainTransferMode == (int)TransferMode.FROM_TO))
                //            {
                //                return null;
                //            }
                //            newCommandTrace.Add(secendCommand);
                //        }
                //    }
                //    if (newCommandTrace.Count > 1)
                //    {
                //        _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"FirstCommand:{newCommandTrace[0].CommandID}"));
                //        _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"SecendCommand:{newCommandTrace[1].CommandID}"));
                //        //_LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"CheckCommandOrder"));
                //        //newCommandTrace = CheckCommandOrder(newCommandTrace);
                //        //_LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"FirstCommand:{newCommandTrace[0].CommandID}"));
                //        //_LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"SecendCommand:{newCommandTrace[1].CommandID}"));
                //        return newCommandTrace;
                //    }
                //    else
                //    {
                //        newCommandTrace = null;
                //        return null;
                //    }
                //}

                //第一筆如果是 From, SCAN, Move 就只取一筆, 且第二筆要是TO or FROM_TO ///之後要改再改
                if (firstCommand.MainTransferMode == (int)TransferMode.FROM_TO || firstCommand.MainTransferMode == (int)TransferMode.TO)
                {
                    //取下筆的時候 避免取到第一筆, 所以先排除第一筆的CommandID
                    commandTraces = commandTraces.Where(i => i.CommandID != firstCommand.CommandID);
                    //取第二筆
                    secendCommand = GetSecendCommand(commandTraces, firstCommand, twoSourceShelves, twoDestShelves, craneCurrentBay);
                    //如果不是 Null 再加入List
                    if (secendCommand != null)
                    {
                        if ((firstCommand.MainTransferMode == (int)TransferMode.FROM_TO && secendCommand.MainTransferMode == (int)TransferMode.TO) ||
                            (firstCommand.MainTransferMode == (int)TransferMode.TO && secendCommand.MainTransferMode == (int)TransferMode.FROM_TO))
                        {
                            return null;
                        }
                        newCommandTrace.Add(secendCommand);
                    }
                }
                if (newCommandTrace.Count > 1)
                {
                    _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"FirstCommand:{newCommandTrace[0].CommandID}"));
                    _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"SecendCommand:{newCommandTrace[1].CommandID}"));
                    return newCommandTrace;
                }
                else
                {
                    newCommandTrace = null;
                    return null;
                }
            }

            return newCommandTrace;
        }

        private bool IsLimitCommand(bool isOnlyCrane1Mode, bool isOnlyCrane2Mode, string limitBay, CommandTrace firstCommand)
        {
            string destBank = Convert.ToInt16(firstCommand.NextDestBank).ToString();
            string destBay = Convert.ToInt16(firstCommand.NextDestBay + (isOnlyCrane1Mode ? 1 : isOnlyCrane2Mode ? -1 : 0)).ToString();
            bool isTheDestLimitDest =
                IsTheDestLimitDest(
                    isOnlyCrane1Mode,
                    isOnlyCrane2Mode,
                    //firstCommand.MainCrane, firstCommand.MainFork,
                    firstCommand.NextCrane, firstCommand.NextFork,
                    destBank, destBay, _MinBay,
                    _MaxBay, limitBay);
            return isTheDestLimitDest;
        }

        private List<CommandTrace> CheckCommandOrder(IEnumerable<CommandTrace> commandTraces)
        {
            List<CommandTrace> newCommandTrace = new List<CommandTrace>();
            CommandTrace firstCommand = commandTraces.FirstOrDefault();
            CommandTrace secendCommand = commandTraces.ElementAt(1);
            if ((firstCommand.NextSourceBank == 1 && firstCommand.NextSourceBay > secendCommand.NextSourceBay) ||
                (firstCommand.NextSourceBank == 2 && firstCommand.NextSourceBay < secendCommand.NextSourceBay))
            {
                newCommandTrace.Add(secendCommand);
                newCommandTrace.Add(firstCommand);
            }
            else
            {
                newCommandTrace.Add(firstCommand);
                newCommandTrace.Add(secendCommand);
            }

            return newCommandTrace;
        }

        private bool IsSourceLimitCommand(bool isSingleCrane1Mode, bool isSingleCrane2Mode, CommandTrace trace, string limitBay, int fork)
        {
            string sourceBank = Convert.ToInt16(trace.NextSourceBank).ToString();
            string sourceBay = Convert.ToInt16(trace.NextSourceBay + (isSingleCrane1Mode ? 1 : isSingleCrane2Mode ? -1 : 0)).ToString();

            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"Check Crane:{trace.NextCrane}, Fork:{fork}, SourceBay:{sourceBay}"));
            bool isTheSourceLimitDest = IsTheDestLimitDest(isSingleCrane1Mode, isSingleCrane2Mode, trace.NextCrane, fork, sourceBank, sourceBay, _MinBay, _MaxBay, limitBay);
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"Check Source Limit:{isTheSourceLimitDest}"));
            return isTheSourceLimitDest;
        }

        private bool IsDestLimitCommand(bool isSingleCrane1Mode, bool isSingleCrane2Mode, CommandTrace trace, string limitBay, int fork)
        {
            string destBank = Convert.ToInt16(trace.NextDestBank).ToString();
            string destBay = Convert.ToInt16(trace.NextDestBay + (isSingleCrane1Mode ? 1 : isSingleCrane2Mode ? -1 : 0)).ToString();

            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"Check Crane:{trace.NextCrane}, Fork:{fork}, DestBay:{destBay}"));
            bool isTheDestLimitDest = IsTheDestLimitDest(isSingleCrane1Mode, isSingleCrane2Mode, trace.NextCrane, fork, destBank, destBay, _MinBay, _MaxBay, limitBay);
            _LoggerService.WriteLogTrace(new TaskProcessTrace(trace.CommandID, $"Check Dest Limit:{isTheDestLimitDest}"));
            return isTheDestLimitDest;
        }

        private CommandTrace GetFirstCommand(IEnumerable<CommandTrace> commandTraces, CommandTrace first)
        {
            string Bank = GetBank(first);
            string Bay = GetBay(first);
            int frok = first.NextFork == 0 ? 1 : first.NextFork;

            //var isTheSourceLimitDest = IsSourceLimitCommand(isOnlyCrane1Mode, isOnlyCrane2Mode, first, limitBay, 1);
            //var isTheDestLimitDest = IsDestLimitCommand(isOnlyCrane1Mode, isOnlyCrane2Mode, first, limitBay, 1);
            //if (isTheSourceLimitDest || isTheDestLimitDest)
            //{
            //    return null;
            //}

            //第一筆如果是 From, TO, SCAN, Move 就只取一筆
            if (first.MainTransferMode != (int)TransferMode.FROM_TO)
            {
                //判斷是否有BatchID
                //有BatchID
                if (!string.IsNullOrWhiteSpace(first.BatchID))
                {
                    //取下筆BatchID的命令
                    var sccendCommand = commandTraces.Where(i => i.BatchID == first.BatchID && i.CommandID != first.CommandID);
                    if (sccendCommand.Count() > 0)
                    {
                        return first;
                    }
                    else
                    {
                        //如果沒有下筆相同BatchId的則選擇continue選下一筆
                        return null;
                    }
                }
                else
                {
                    return null;
                }

                //極限值的判斷
                //Crane1 的 右Fork 到不了 Bank1的第一個Bay
                //Crane2 的 左Fork 到不了 Bank1的最後一個Bay
                //Crane1 的 左Fork 到不了 Bank2的第一個Bay
                //Crane2 的 右Fork 到不了 Bank2的最後一個Bay

                //string Bank = GetBank(first);

                //string Bay = GetBay(first);

                //bool isTheDestLimitDest = IsTheDestLimitDest(isOnlyCrane1Mode, isOnlyCrane2Mode, first.MainCrane, first.MainFork, Bank, Bay, minBay, maxBay, limitBay);
                //bool isTheDestLimitDest = IsTheDestLimitDest(isOnlyCrane1Mode, isOnlyCrane2Mode, first.NextCrane, first.NextFork, Bank, Bay, minBay, maxBay, limitBay);
                //_LoggerService.WriteLogTrace(new TaskProcessTrace(first.CommandID, $"GetFirstCommand, Check Crane:{first.NextCrane}, Fork:{first.NextFork}, DestBay:{Bay}"));
                //bool isTheDestLimitDest = IsTheDestLimitDest(isOnlyCrane1Mode, isOnlyCrane2Mode, first.NextCrane, frok, Bank, Bay, minBay, maxBay, limitBay);
                //_LoggerService.WriteLogTrace(new TaskProcessTrace(first.CommandID, $"GetFirstCommand, Check Dest Limit:{isTheDestLimitDest}"));
                //if (isTheDestLimitDest)
                //    return null;
                //else
                //{
                //    //TODO : 取最接近Crane的一筆
                //    return first;
                //}
            }
            else
            {
                ////如果命令是Transfer
                ////判斷是否有BatchID
                ////有BatchID
                //if (!string.IsNullOrWhiteSpace(first.BatchID))
                //{
                //    //取下筆BatchID的命令
                //    var sccendCommand = commandTraces.Where(i => i.BatchID == first.BatchID && i.CommandID != first.CommandID);
                //    if (sccendCommand.Count() > 0)
                //    {
                //        return first;
                //    }
                //    else
                //    {
                //        //如果沒有下筆相同BatchId的則選擇continue選下一筆
                //        return null;
                //    }
                //}
                ////沒有BatchID
                //else
                //{
                //    return first;
                //}

                //_LoggerService.WriteLogTrace(new TaskProcessTrace(first.CommandID, $"GetFirstCommand, Check Crane:{first.NextCrane}, Fork:{first.NextFork}, DestBay:{Bay}"));
                //bool isTheDestLimitDest = IsTheDestLimitDest(isOnlyCrane1Mode, isOnlyCrane2Mode, first.NextCrane, frok, Bank, Bay, minBay, maxBay, limitBay);
                //_LoggerService.WriteLogTrace(new TaskProcessTrace(first.CommandID, $"GetFirstCommand, Check Dest Limit:{isTheDestLimitDest}"));
                //if (isTheDestLimitDest)
                //{
                //    return null;
                //}
                //else
                //{
                //    if (!string.IsNullOrWhiteSpace(first.BatchID))
                //    {
                //        //取下筆BatchID的命令
                //        var sccendCommand = commandTraces.Where(i => i.BatchID == first.BatchID && i.CommandID != first.CommandID);
                //        if (sccendCommand.Count() > 0)
                //        {
                //            return first;
                //        }
                //        else
                //        {
                //            //如果沒有下筆相同BatchId的則選擇continue選下一筆
                //            return null;
                //        }
                //    }
                //    //沒有BatchID
                //    else
                //    {
                //        return first;
                //    }
                //}

                if (!string.IsNullOrWhiteSpace(first.BatchID))
                {
                    //取下筆BatchID的命令
                    var sccendCommand = commandTraces.Where(i => i.BatchID == first.BatchID && i.CommandID != first.CommandID);
                    if (sccendCommand.Count() > 0)
                    {
                        return first;
                    }
                    else
                    {
                        //如果沒有下筆相同BatchId的則選擇continue選下一筆
                        return null;
                    }
                }
                //沒有BatchID
                else
                {
                    return first;
                }
            }
        }

        private CommandTrace GetSecendCommand(IEnumerable<CommandTrace> commandTraces, CommandTrace firstCommand, List<TwoShelf> twoSourceShelves, List<TwoShelf> twoDestShelves, int craneCurrentBay = 0)
        {
            CommandTrace secendCommand = null;

            if (string.IsNullOrWhiteSpace(firstCommand.BatchID))
            {
                commandTraces = commandTraces.Where(i => i.MainTransferMode == (int)TransferMode.FROM_TO && string.IsNullOrWhiteSpace(i.BatchID));

                //判斷是否有同取的情況
                secendCommand = GetSourceNearByCommand(firstCommand, commandTraces, twoSourceShelves);
                if (secendCommand != null)
                {
                    //var isTheSourceLimitDest = IsSourceLimitCommand(isOnlyCrane1Mode, isOnlyCrane2Mode, secendCommand, limitBay, 2);
                    //var isTheDestLimitDest = IsDestLimitCommand(isOnlyCrane1Mode, isOnlyCrane2Mode, secendCommand, limitBay, 2);
                    //if (isTheSourceLimitDest || isTheDestLimitDest)
                    //{
                    //    return null;
                    //}
                    //else
                    //{
                        return secendCommand;
                    //}
                }

                //判斷是否有同放的情況
                secendCommand = GetDestNearByCommand(firstCommand, commandTraces, twoDestShelves);
                if (secendCommand != null)
                {
                    //var isTheSourceLimitDest = IsSourceLimitCommand(isOnlyCrane1Mode, isOnlyCrane2Mode, secendCommand, limitBay, 2);
                    //var isTheDestLimitDest = IsDestLimitCommand(isOnlyCrane1Mode, isOnlyCrane2Mode, secendCommand, limitBay, 2);
                    //if (isTheSourceLimitDest || isTheDestLimitDest)
                    //{
                    //    return null;
                    //}
                    //else
                    //{
                        return secendCommand;
                    //}
                }

                var cmd = separateLessThanTogetherSteps(firstCommand, commandTraces, craneCurrentBay);

                foreach (var item in cmd.OrderBy(i => Math.Abs(i.NextSourceBay - craneCurrentBay)))
                {
                    //判斷是否是已指定Fork
                    //如果第一筆指定的Fork = 第二筆指定的Fork 就跳過
                    if (firstCommand.MainFork == item.MainFork && firstCommand.MainFork != 0)
                    {
                        continue;
                    }
                    else
                    {
                        ////極限值的判斷
                        ////Crane1 的 右Fork 到不了 Bank1的第一個Bay
                        ////Crane2 的 左Fork 到不了 Bank1的最後一個Bay
                        ////Crane1 的 左Fork 到不了 Bank2的第一個Bay
                        ////Crane2 的 右Fork 到不了 Bank2的最後一個Bay

                        //string Bank = GetBank(item);

                        //string Bay = GetBay(item);

                        ////bool isTheDestLimitDest = IsTheDestLimitDest(isOnlyCrane1Mode, isOnlyCrane2Mode, item.MainCrane, item.MainFork, Bank, Bay, minBay, maxBay, limitBay);

                        //_LoggerService.WriteLogTrace(new TaskProcessTrace(item.CommandID, $"GetSecendCommand, Check Crane:{item.NextCrane}, Fork:{item.NextFork}, DestBay:{Bay}"));
                        //bool isTheDestLimitDest = IsTheDestLimitDest(isOnlyCrane1Mode, isOnlyCrane2Mode, item.NextCrane, item.NextFork, Bank, Bay, minBay, maxBay, limitBay);
                        //_LoggerService.WriteLogTrace(new TaskProcessTrace(item.CommandID, $"GetSecendCommand, Check Dest Limit:{isTheDestLimitDest}"));
                        //if (isTheDestLimitDest)
                        //    continue;
                        //else
                        //{
                        //    secendCommand = item;
                        //    break;
                        //}
                        secendCommand = item;
                        break;
                    }
                }
            }
            else
            {
                var subCommand = commandTraces.Where(i => i.BatchID == firstCommand.BatchID);
                if (subCommand.Count() > 0)
                {
                    secendCommand = subCommand.FirstOrDefault();

                    //var isTheSourceLimitDest = IsSourceLimitCommand(isOnlyCrane1Mode, isOnlyCrane2Mode, secendCommand, limitBay, 2);
                    //var isTheDestLimitDest = IsDestLimitCommand(isOnlyCrane1Mode, isOnlyCrane2Mode, secendCommand, limitBay, 2);
                    //if (isTheSourceLimitDest || isTheDestLimitDest)
                    //{
                    //return null;
                    //}
                }

            }
            return secendCommand;
        }

        private List<CommandTrace> separateLessThanTogetherSteps(CommandTrace firstCommand, IEnumerable<CommandTrace> commandTraces, int craneCurrentBay)
        {
            var separateLessThanTogetherStepsCmdList = new List<CommandTrace>();
            //如果分開取 比 一起取的步數少 則不列入考慮
            var separateFirstCmdStep = Math.Abs(craneCurrentBay - firstCommand.NextSourceBay) +
                                       Math.Abs(firstCommand.NextSourceBay - firstCommand.NextDestBay);

            foreach (var SecondCommand in commandTraces)
            {
                var separateSecondCmdStep = Math.Abs(firstCommand.NextDestBay - SecondCommand.NextSourceBay) +
                                            Math.Abs(SecondCommand.NextSourceBay - SecondCommand.NextDestBay);

                var separateStep = separateFirstCmdStep + separateSecondCmdStep;

                var step = 0;
                if (Math.Abs(SecondCommand.NextSourceBay - SecondCommand.NextDestBay) > Math.Abs(SecondCommand.NextSourceBay - firstCommand.NextDestBay))
                    step = firstCommand.NextDestBay;
                else
                    step = SecondCommand.NextDestBay;

                var togetherStep = Math.Abs(craneCurrentBay - firstCommand.NextSourceBay) +
                                   Math.Abs(firstCommand.NextSourceBay - SecondCommand.NextSourceBay) +
                                   Math.Abs(SecondCommand.NextSourceBay - step) +
                                   Math.Abs(firstCommand.NextSourceBay - (step == firstCommand.NextDestBay ? SecondCommand.NextDestBay : firstCommand.NextDestBay));

                if (separateStep >= togetherStep)
                {
                    separateLessThanTogetherStepsCmdList.Add(SecondCommand);
                }
            }
            return separateLessThanTogetherStepsCmdList;
        }

        private static string GetBay(CommandTrace command)
        {
            string Bay = "";

            if (command.MainTransferMode == (int)TransferMode.SCAN || command.MainTransferMode == (int)TransferMode.FROM)
            {
                Bay = command.MainSourceBay.ToString();
            }
            else
            {
                Bay = command.MainDestBay.ToString();
            }

            return Bay;
        }

        private static string GetBank(CommandTrace command)
        {
            string Bank = "";
            if (command.MainTransferMode == (int)TransferMode.SCAN || command.MainTransferMode == (int)TransferMode.FROM)
            {
                Bank = command.NextSourceBank.ToString();
            }
            else
            {
                Bank = command.NextDestBank.ToString();
            }

            return Bank;
        }

        private CommandTrace GetDestNearByCommand(CommandTrace firstCommand, IEnumerable<CommandTrace> commandTraces, List<TwoShelf> twoShelves)
        {
            //確認可以同取的位置
            //如果在左邊 直接選
            //如果在右邊 則選左邊的
            CommandTrace secondCommand = null;

            //if (string.IsNullOrWhiteSpace(firstCommand.NextDest))
            //{
            //    foreach (var item in commandTraces.Where(i => i.CommandID != firstCommand.CommandID))
            //    {
            //        if (string.IsNullOrWhiteSpace(item.NextDest))
            //        {
            //            secondCommand = item;
            //            return secondCommand;
            //        }
            //    }
            //}

            foreach (var item in commandTraces.Where(i => i.CommandID != firstCommand.CommandID))
            {
                var firstShelf = twoShelves.Where(i => i.LeftForkShelfID == firstCommand.MainDest);
                //測試同放HandOff
                //var firstShelf = twoShelves.Where(i => i.LeftForkShelfID == firstCommand.NextDest);
                if (firstShelf.Count() > 0)
                {
                    if (item.MainDest == firstShelf.First().RightForkShelfID)
                    //測試同放HandOff
                    //if (item.NextDest == firstShelf.First().RightForkShelfID)
                    {
                        secondCommand = item;
                        break;
                    }
                }

                firstShelf = twoShelves.Where(i => i.RightForkShelfID == firstCommand.MainDest);
                //測試同放HandOff
                //firstShelf = twoShelves.Where(i => i.RightForkShelfID == firstCommand.NextDest);
                if (firstShelf.Count() > 0)
                {
                    if (item.MainDest == firstShelf.First().LeftForkShelfID)
                    //測試同放HandOff
                    //if (item.NextDest == firstShelf.First().LeftForkShelfID)
                    {
                        secondCommand = item;
                        break;
                    }
                }
            }

            return secondCommand;
        }

        public CommandTrace GetSourceNearByCommand(CommandTrace firstCommand, IEnumerable<CommandTrace> commandTraces, List<TwoShelf> twoShelves)
        {
            //確認可以同取的位置
            //如果在左邊 直接選
            //如果在右邊 則選左邊的
            CommandTrace secondCommand = null;

            foreach (var item in commandTraces.Where(i => i.CommandID != firstCommand.CommandID))
            {
                var firstShelf = twoShelves.Where(i => i.LeftForkShelfID == firstCommand.MainSource);
                //var firstShelf = twoShelves.Where(i => i.LeftForkShelfID == firstCommand.NextSource);
                if (firstShelf.Count() > 0)
                {
                    if (item.MainSource == firstShelf.First().RightForkShelfID)
                    //if (item.NextSource == firstShelf.First().RightForkShelfID)
                    {
                        secondCommand = item;
                        break;
                    }
                }

                firstShelf = twoShelves.Where(i => i.RightForkShelfID == firstCommand.MainSource);
                //firstShelf = twoShelves.Where(i => i.RightForkShelfID == firstCommand.NextSource);
                if (firstShelf.Count() > 0)
                {
                    if (item.MainSource == firstShelf.First().LeftForkShelfID)
                    //if (item.NextSource == firstShelf.First().LeftForkShelfID)
                    {
                        secondCommand = item;
                        break;
                    }
                }
            }

            return secondCommand;
        }

        public bool IsTheDestLimitDest(bool isOnlyCrane1Mode, bool isOnlyCrane2Mode, int Crane, int Fork, string destBank, string destBay, string minBay, string maxBay, string limitBay)
        {
            if (isOnlyCrane1Mode)
            {
                return IsTheDestLimitDestwhenCrane1OneMode(Crane, Fork, destBank, destBay, limitBay, minBay);
            }
            else if (isOnlyCrane2Mode)
            {
                return IsTheDestLimitDestwhenCrane2OneMode(Crane, Fork, destBank, destBay, limitBay, maxBay);
            }
            else
            {
                return IsTheDestLimitDestNormal(Crane, Fork, destBank, destBay, maxBay, minBay);
            }
        }

        //極限值的判斷
        //Crane1 的 右Fork 到不了 Bank1的第一個Bay
        //Crane2 的 左Fork 到不了 Bank1的最後一個Bay
        //Crane1 的 左Fork 到不了 Bank2的第一個Bay
        //Crane2 的 右Fork 到不了 Bank2的最後一個Bay

        public bool IsTheDestLimitDestNormal(int Crane, int Fork, string destBank, string destBay, string maxBay, string minBay)
        {
            int intDestBay = Convert.ToInt32(destBay);
            int intMinBay = Convert.ToInt32(minBay);
            int intMaxBay = Convert.ToInt32(maxBay);
            if (Crane == 1)
            {
                if (Fork == 2)
                {
                    if (destBank == "1" && intDestBay <= intMinBay)
                    {
                        return true;
                    }
                }
                else if (Fork == 1)
                {
                    if (destBank == "2" && intDestBay <= intMinBay)
                    {
                        return true;
                    }
                }
            }
            else if (Crane == 2)
            {
                if (Fork == 1)
                {
                    if (destBank == "1" && intDestBay >= intMaxBay)
                    {
                        return true;
                    }
                }
                else if (Fork == 2)
                {
                    if (destBank == "2" && intDestBay >= intMaxBay)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //極限值的判斷
        //Crane1 的 右Fork 到不了 Bank1的第一個Bay
        //Crane1 的 左Fork 到不了 Bank2的第一個Bay

        //Crane1 的 右Fork 到不了 Bank1的最後一個 可執行的Bay
        //Crane1 的 左Fork 到不了 Bank2的最後一個 可執行的Bay
        //Crane2 都當作 極限區
        public bool IsTheDestLimitDestwhenCrane1OneMode(int Crane, int Fork, string destBank, string destBay, string limitBay, string minBay)
        {
            int intDestBay = Convert.ToInt32(destBay);
            int intMinBay = Convert.ToInt32(minBay);
            int intLimitBay = Convert.ToInt32(limitBay);

            if (Crane == 1)
            {
                if (Fork == 2)
                {
                    if ((destBank == "1" && intDestBay <= intMinBay) || (destBank == "2" && intDestBay >= intLimitBay))
                    {
                        return true;
                    }
                }
                else if (Fork == 1)
                {
                    if ((destBank == "2" && intDestBay <= intMinBay) || (destBank == "1" && intDestBay >= intLimitBay))
                    {
                        return true;
                    }
                }
            }
            else if (Crane == 2)
            {
                return true;
            }
            return false;
        }

        //極限值的判斷
        //Crane2 的 右Fork 到不了 Bank1的最前面可以執行的 Bay
        //Crane2 的 左Fork 到不了 Bank2的最前面可以執行的 Bay

        //Crane2 的 右Fork 到不了 Bank1的最後一個Bay
        //Crane2 的 左Fork 到不了 Bank2的最後一個Bay
        //Crane1 都當作 極限區
        public bool IsTheDestLimitDestwhenCrane2OneMode(int Crane, int Fork, string destBank, string destBay, string limitBay, string maxBay)
        {
            int intDestBay = Convert.ToInt32(destBay);
            int intMaxBay = Convert.ToInt32(maxBay);
            int intLimitBay = Convert.ToInt32(limitBay);
            if (Crane == 2)
            {
                if (Fork == 2)
                {
                    if ((destBank == "1" && intDestBay <= intLimitBay) || (destBank == "2" && intDestBay >= intMaxBay))
                    {
                        return true;
                    }
                }
                else if (Fork == 1)
                {
                    if ((destBank == "1" && intDestBay >= intMaxBay) || (destBank == "2" && intDestBay <= intLimitBay))
                    {
                        return true;
                    }
                }
            }
            else if (Crane == 1)
            {
                return true;
            }
            return false;
        }

        public bool IsNearestCmd(CommandTrace firstCommand, List<CommandTrace> commandListWithoutFirstCommand)
        {
            return true;
        }
    }
}
