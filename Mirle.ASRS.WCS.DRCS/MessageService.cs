using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mirle.ASRS.WCS.DRCS.Service.Message;
using Mirle.Universal.Archive;
using Mirle.Universal.Logger;

namespace Mirle.ASRS.WCS.DRCS
{
    public class MessageService : IDisposable
    {

        public MessageService()
        {
        }

        public void WriteException(MethodBase methodBase, Exception exception)
        {
            try
            {
                lock (_LogLock)
                {
                    Debug.WriteLine($"{methodBase.DeclaringType.FullName}.{methodBase.Name}: {exception.Message}\n{exception.StackTrace}");
                    _Log.WriteLogFile("WCS_Exception.log", $"{methodBase.DeclaringType.FullName}.{methodBase.Name}: {exception.Message}\n{exception.StackTrace}");
                }
            }
            catch (Exception ex)
            { Debug.WriteLine($"{MethodBase.GetCurrentMethod()}: {ex.Message}\n{ex.StackTrace}"); }
        }

        public void WriteLog(string fileName, string message)
        {
            try
            {
                lock (_LogLock)
                {
                    _Log.WriteLogFile(fileName, message);
                }
            }
            catch (Exception ex)
            { Debug.WriteLine($"{MethodBase.GetCurrentMethod()}: {ex.Message}\n{ex.StackTrace}"); }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                _Log?.Dispose();
                _AutoArchive?.Stop();

                disposedValue = true;
            }
        }

        ~MessageService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
