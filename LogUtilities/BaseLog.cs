using System;
using System.Text;

namespace LogUtilities
{
    public abstract class BaseLog : ILog
    {
        protected LogStream LogStream;
        
        public void Dispose()
        {
            LogStream?.Dispose();
        }
        
        public void WriteLine(string line)
        {
            Write(line + Environment.NewLine);
        }

        public void Write(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            LogStream?.Write(bytes, 0, bytes.Length);
        }
    }
}