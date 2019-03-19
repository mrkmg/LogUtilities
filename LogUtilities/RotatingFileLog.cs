using System;
using System.IO;
using System.Text;

namespace LogUtilities
{
    public class RotatingFileLog : ILog
    {
        private LogStream _logStream;
        private FileStream _fileStream;
        private DateTime _currentLogCreateDate;
        private string _path;
        private string _prefix;
        private string _datetimeFormat;
        private string _separator;

        /// <summary>
        /// Maximum size of a file in bytes.
        /// 
        /// Set to -1 for infinite.
        /// </summary>
        public long MaxSize = -1;
        
        /// <summary>
        /// Maximum age of a file in seconds.
        /// 
        /// Set to -1 for infinite.
        /// </summary>
        public double MaxAge = -1;
        
        /// <summary>
        /// Maximum number of files to keep.
        ///
        /// Set to -1 for infinite.
        /// </summary>
        public int MaxFiles = -1;
        
        public RotatingFileLog(string path, string prefix = null, string datetimeFormat = "s", string separator = " | ")
        {
            _path = path;
            _prefix = prefix;
            _datetimeFormat = datetimeFormat;
            _separator = separator;
            
            SetupLogger();
        }
        
        public void Dispose()
        {
            _logStream?.Dispose();
        }
        
        public void WriteLine(string line)
        {
            Write(line + Environment.NewLine);
        }

        public void Write(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            var isTooLong = MaxSize > 0 && _logStream.Length + bytes.Length > MaxSize;
            var isTooOld = MaxAge > 0 && (DateTime.Now - _currentLogCreateDate).TotalSeconds > MaxAge;

            if (isTooLong || isTooOld)
            {
                SwapLog();
            }
            
            _logStream?.Write(bytes, 0, bytes.Length);
        }

        private void SetupLogger()
        {
            _fileStream = new FileStream(_path, FileMode.Append);
            _logStream = new LogStream(_fileStream, _prefix, _datetimeFormat, _separator);
            _currentLogCreateDate = File.GetCreationTime(_path);
        }

        private void SwapLog()
        {
            _logStream.Dispose();
            _fileStream.Dispose();
            RotateFiles();
            SetupLogger();
        }

        private void RotateFiles()
        {
            var index = 0;
            while (File.Exists(GetPathForIndex(index + 1)))
            {
                index++;
            }

            if (MaxFiles > 0)
            {
                while (index > MaxFiles)
                {
                    File.Delete(GetPathForIndex(index));
                    index--;
                }
            }
            
            while (index > 0)
            {
                File.Move(GetPathForIndex(index), GetPathForIndex(index + 1));
                index--;
            }
            
            File.Move(_path, GetPathForIndex(1));
        }

        private string GetPathForIndex(int index) => _path + "." + index;
    }
}