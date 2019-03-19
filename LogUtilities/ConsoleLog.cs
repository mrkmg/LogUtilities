using System;
using System.IO;
using System.Text;

namespace LogUtilities
{
    public class ConsoleLog : BaseLog
    {
        public ConsoleLog(string prefix = null, string datetimeFormat = "s", string separator = " | ")
        {
            LogStream = new LogStream(Console.OpenStandardOutput(), prefix, datetimeFormat, separator);
        }
    }
}