using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Mirle.DataBase;
using Mirle.LCS.Models;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.Stocker;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Stocker.TaskControl.Module;
using Mirle.Stocker.TaskControl.TraceLog;
using Mirle.Stocker.TaskControl.TraceLog.Format;
using static Mirle.R46YP320.STK.DaifukuSpec;

namespace Mirle.R46YP320.STK.TaskService
{
    public class AlarmProcessService : AlarmProcessModule
    {
        private readonly DataCollectionEventsService _DataCollectionEventsService;
        private readonly RepositoriesService _Repositories;

        public AlarmProcessService(TaskInfo taskInfo, IStocker stocker, DataCollectionEventsModule dataCollectionEventsService, LoggerService loggerService) : base(taskInfo, stocker, loggerService)
        {
            _DataCollectionEventsService = (DataCollectionEventsService)dataCollectionEventsService;
            _Repositories = new RepositoriesService(taskInfo, loggerService);
        }

        protected override void AlarmScenariosProcess()
        {
            string message = string.Empty;
            string defaultMessage = "AlarmReport";
            int iResult = ErrorCode.Initial;
            try
            {
                using (DB _db = GetDB())
                {
                    IEnumerable<AlarmData> alarmDatas = _Repositories.GetAlarmData(_db);
                    foreach (AlarmData data in alarmDatas)
                    {
                        if (!data.ReportEnable)
                            continue;

                        if (data.ReportFlag)
                            continue;

                        if (data.AlarmLevel == (int)AlarmLevel.Alarm)
                        {
                            if (data.AlarmSts == (int)AlarmStates.Set)
                            {
                                iResult = _Repositories.UpdateAlarmData(_db, data.EQID, data.AlarmType, data.AlarmCode, data.AlarmSts, data.STRDT);
                                if (iResult != ErrorCode.Success)
                                {
                                    message = defaultMessage + $", Update AlarmSet Fail, Result:{iResult}";
                                    _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                                    return;
                                }
                                message = defaultMessage + $", Update AlarmSet Success";
                                _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                            }
                            else if (data.AlarmSts == (int)AlarmStates.Cleared)
                            {
                                iResult = _Repositories.UpdateAlarmData(_db, data.EQID, data.AlarmType, data.AlarmCode, data.AlarmSts, data.STRDT);
                                if (iResult != ErrorCode.Success)
                                {
                                    message = defaultMessage + $", Update AlarmClear Fail, Result:{iResult}";
                                    _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                                    return;
                                }
                                message = defaultMessage + $", Update AlarmClear Success";
                                _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                            }
                        }
                        else if (data.AlarmLevel == (int)AlarmLevel.UnitAlarm)
                        {
                            if (data.AlarmSts == (int)AlarmStates.Set)
                            {
                                iResult = _Repositories.UpdateAlarmData(_db, data.EQID, data.AlarmType, data.AlarmCode, data.AlarmSts, data.STRDT);
                                if (iResult != ErrorCode.Success)
                                {
                                    message = defaultMessage + $", Update UnitAlarmSet Fail, Result:{iResult}";
                                    _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                                    return;
                                }
                                message = defaultMessage + $", Update UnitAlarmSet Success";
                                _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                            }
                            else if (data.AlarmSts == (int)AlarmStates.Cleared)
                            {
                                iResult = _Repositories.UpdateAlarmData(_db, data.EQID, data.AlarmType, data.AlarmCode, data.AlarmSts, data.STRDT);
                                if (iResult != ErrorCode.Success)
                                {
                                    message = defaultMessage + $", Update UnitAlarmClear Fail, Result:{iResult}";
                                    _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                                    return;
                                }
                                message = defaultMessage + $", Update UnitAlarmClear Success";
                                _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                            }
                        }
                        else if (data.AlarmLevel == (int)AlarmLevel.Warning)
                        {
                            iResult = _Repositories.UpdateAlarmData(_db, data.EQID, data.AlarmType, data.AlarmCode, data.AlarmSts, data.STRDT);
                            if (iResult != ErrorCode.Success)
                            {
                                message = defaultMessage + $", Update UnitAlarmSet/Clear Fail, Result:{iResult}";
                                _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                                return;
                            }
                            message = defaultMessage + $", Update UnitAlarmSet/Clear Success";
                            _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType, data.AlarmLevel, data.AlarmCode, data.AlarmID, message));

                            //_SECSEventService.SendAlarmSet(Convert.ToInt32(data.AlarmID), data.AlarmDesc, data.EQID, VIDEnums.StockerUnitState.ErrorUnit);
                            //Task.Delay(200).Wait();
                            //_SECSEventService.SendAlarmClear(Convert.ToInt32(data.AlarmID), data.AlarmDesc, data.EQID, VIDEnums.StockerUnitState.ErrorUnit);
                        }
                    }

                    //FFUAlarm

                    if (_TaskInfo.Config.FFUConfig.Enable == 1)
                    {
                        var alarmDatas_FFU = _Repositories.GetFFUAlarmData(_db);
                        foreach (var data in alarmDatas_FFU)
                        {
                            if (!data.ReportEnable)
                                continue;

                            if (data.ReportFlag)
                                continue;

                            if (data.AlarmLevel == (int)AlarmLevel.UnitAlarm)
                            {
                                if (data.AlarmSts == (int)AlarmStates.Set)
                                {
                                    iResult = _Repositories.UpdateAlarmData_FFU(_db, data.EQID, data.AlarmCode,
                                        data.AlarmSts, data.STRDT);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage + $", Update FFU UnitAlarmSet Fail, Result:{iResult}";
                                        _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, 4,
                                            data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                                        return;
                                    }

                                    message = defaultMessage + $", Update FFU UnitAlarmSet Success";
                                    _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, 4, data.AlarmLevel,
                                        data.AlarmCode, data.AlarmID, message));
                                }
                                else if (data.AlarmSts == (int)AlarmStates.Cleared)
                                {
                                    iResult = _Repositories.UpdateAlarmData_FFU(_db, data.EQID, data.AlarmCode,
                                        data.AlarmSts, data.STRDT);
                                    if (iResult != ErrorCode.Success)
                                    {
                                        message = defaultMessage +
                                                  $", Update FFU UnitAlarmClear Fail, Result:{iResult}";
                                        _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, 4,
                                            data.AlarmLevel, data.AlarmCode, data.AlarmID, message));
                                        return;
                                    }

                                    message = defaultMessage + $", Update FFU UnitAlarmClear Success";
                                    _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, data.AlarmType,
                                        data.AlarmLevel, data.AlarmCode, data.AlarmID, message));

                                }
                            }
                            else if (data.AlarmLevel == (int)AlarmLevel.Warning)
                            {
                                iResult = _Repositories.UpdateAlarmData_FFU(_db, data.EQID, data.AlarmCode,
                                    data.AlarmSts, data.STRDT);
                                if (iResult != ErrorCode.Success)
                                {
                                    message = defaultMessage +
                                              $", Update FFU UnitAlarmSet/Clear Fail, Result:{iResult}";
                                    _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, 4, data.AlarmLevel,
                                        data.AlarmCode, data.AlarmID, message));
                                    return;
                                }

                                message = defaultMessage + $", Update FFU UnitAlarmSet/Clear Success";
                                _LoggerService.WriteLogTrace(new AlarmProcessTrace(data.EQID, 4, data.AlarmLevel,
                                    data.AlarmCode, data.AlarmID, message));

                                //_DataCollectionEventsService.OnUnitAlarmSet(this, new UnitAlarmEventArgs(data.EQID, data.AlarmID.ToString(), data.AlarmDesc));
                                //Task.Delay(200).Wait();
                                //_DataCollectionEventsService.OnUnitAlarmCleared(this, new UnitAlarmEventArgs(data.EQID, data.AlarmID.ToString(), data.AlarmDesc));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { _LoggerService.WriteException(MethodBase.GetCurrentMethod().Name, $"{ex.Message}\n{ex.StackTrace}"); }
        }
    }
}
