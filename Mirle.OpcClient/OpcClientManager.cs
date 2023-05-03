using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using OPCWrite;
using System.Timers;
using System.Threading;

namespace Mirle.OpcClient
{
    public class OpcClientManager : IDisposable
    {
        private readonly ConcurrentQueue<OpcItemValue> _opcValues = new ConcurrentQueue<OpcItemValue>();
        private readonly System.Timers.Timer _timer = new System.Timers.Timer();
        private readonly string _iniFilePath = string.Empty;

        private DateTime _lastReconnectTime = DateTime.Now;
        private bool _runFlag = true;
        private OPCWriteClass _opcWriteClass;

        public OpcClientManager()
        {
            _iniFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}OPC-UA.INI";
            clsWriLog.Log.FunWriTraceLog_CV($"StartUp:{_iniFilePath}");
            _opcWriteClass = new OPCWriteClass(_iniFilePath);

            _timer.Elapsed += WriteOpcProcess;
            _timer.Interval = 500;
        }

        private void WriteOpcProcess(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            try
            {
                clsWriLog.Log.FunWriTraceLog_CV("Inserting: ");
                if (_opcWriteClass != null)
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"Server Status: {_opcWriteClass.ServerStatus()}");
                    if (_opcWriteClass.ServerStatus() == OPCDA.OpcServerState.Running)
                    {
                        clsWriLog.Log.FunWriTraceLog_CV("OpcServerRunning: ");
                        List<OpcItemValue> values = new List<OpcItemValue>();
                        while (_opcValues.TryDequeue(out var value))
                        {
                            values.Add(value);
                            if (values.Count > 10)
                            {
                                break;
                            }
                        }
                        clsWriLog.Log.FunWriTraceLog_CV("Write: " + values.ToArray());
                        _opcWriteClass.Write(values.ToArray());
                    }
                }

                Reconnect();
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
            }
            finally
            {
                if (_runFlag)
                {
                    _timer.Start();
                }
            }
        }
        private void Reconnect()
        {
            if (_opcWriteClass is null)
            {
                if (DateTime.Now > _lastReconnectTime.AddSeconds(30))
                {
                    _opcWriteClass = new OPCWriteClass(_iniFilePath);
                    _lastReconnectTime = DateTime.Now;
                }
            }
            else if (_opcWriteClass.ServerStatus() == OPCDA.OpcServerState.Failed)
            {
                _opcWriteClass = null;
                SpinWait.SpinUntil(() => false, 3000);

                if (DateTime.Now > _lastReconnectTime.AddSeconds(30))
                {
                    _opcWriteClass = new OPCWriteClass(_iniFilePath);
                    _lastReconnectTime = DateTime.Now;
                }
            }
        }

        public void Start()
        {
            _timer.Start();
            _runFlag = true;
        }

        public void Stop()
        {
            _runFlag = false;
        }

        public void Add(string tag, string value)
        {
            _opcValues.Enqueue(new OpcItemValue(tag, value));
        }

        #region Dispose
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                }
                _disposedValue = true;
            }
        }

        ~OpcClientManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose
    }
}
