using System.IO;

namespace LogUtilities
{
    public class FileLog : BaseLog
    {
        private readonly FileStream _fileStream;
        
        public FileLog(string path, string prefix = null, string datetimeFormat = "s", string separator = " | ")
        {
            _fileStream = new FileStream(path, FileMode.Append);
            LogStream = new LogStream(_fileStream, prefix, datetimeFormat, separator);
        }

        public new void Dispose()
        {
            LogStream.Dispose();
            _fileStream.Dispose();
        }
    }
}