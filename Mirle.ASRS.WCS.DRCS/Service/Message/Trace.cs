namespace Mirle.ASRS.WCS.DRCS.Service.Message
{
    public abstract class LogTrace
    {
        protected string _Message = string.Empty;

        public abstract string FileName { get; }

        public string GetMessage()
        {
            return _Message;
        }
    }
}
