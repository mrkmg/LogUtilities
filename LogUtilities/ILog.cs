using System;

namespace LogUtilities
{
    public interface ILog : IDisposable
    {
        void Write(string text);
        void WriteLine(string text);
    }
}