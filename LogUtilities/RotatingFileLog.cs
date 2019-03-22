using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogUtilities
{
    public class RotatingFileLog : Stream, ILog
    {
        private readonly LogStream _logStream;
        private readonly MemoryStream _memoryStream;
        private readonly string _path;
        private DateTime _currentLogCreateDate;
        private FileStream _fileStream;

        /// <summary>
        /// AutoFlush the log to disk after every write.
        /// </summary>
        public bool AutoFlush = true;

        /// <summary>
        ///     Maximum age of a file in seconds.
        ///     Set to -1 for infinite.
        /// </summary>
        public double MaxAge = -1;

        /// <summary>
        ///     Maximum number of files to keep.
        ///     Set to -1 for infinite.
        /// </summary>
        public int MaxFiles = -1;

        /// <summary>
        ///     Maximum size of a file in bytes.
        ///     Set to -1 for infinite.
        /// </summary>
        public long MaxSize = -1;

        public RotatingFileLog(string path)
        {
            _path = path;
            _memoryStream = new MemoryStream();
            _logStream = new LogStream(_memoryStream);
            CreateFile();
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => _logStream.CanWrite;

        public override long Length => _logStream.Length;

        public override long Position
        {
            get => _logStream.Position;
            set => _logStream.Position = value;
        }

        public Encoding Encoding
        {
            get => _logStream.Encoding;
            set => _logStream.Encoding = value;
        }

        public string DateTimeFormat
        {
            get => _logStream.DateTimeFormat;
            set => _logStream.DateTimeFormat = value;
        }

        public string Prefix
        {
            get => _logStream.Prefix;
            set => _logStream.Prefix = value;
        }

        public string Separator
        {
            get => _logStream.Separator;
            set => _logStream.Separator = value;
        }

        public void WriteLine(string line)
        {
            Write(line + Environment.NewLine);
        }

        public void Write(IEnumerable<string> textBlocks) => _logStream.Write(textBlocks);

        public void WriteLine(IEnumerable<string> textBlocks) => _logStream.WriteLine(textBlocks);
        public void WriteLine()
        {
            _logStream.WriteLine();
        }


        public void Write(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            Write(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            Flush();
            _logStream.Dispose();
            _fileStream.Dispose();
            _memoryStream.Dispose();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _logStream.Write(buffer, offset, count);
            if (AutoFlush) Flush();
        }

        public override void Flush()
        {
            _logStream.Flush();

            if (MaxAge > 0 && (DateTime.Now - _currentLogCreateDate).TotalSeconds > MaxAge) SwapLog();

            if (MaxSize > 0 && _memoryStream.Length + _fileStream.Length > MaxSize)
            {
                _memoryStream.Position = 0;
                while (_memoryStream.Length - _memoryStream.Position > 0)
                {
                    var amountToCopy = (int) Math.Min(_memoryStream.Length - _memoryStream.Position,
                        MaxSize - _fileStream.Length);
                    var bytesToCopy = new byte[amountToCopy];
                    _memoryStream.Read(bytesToCopy, 0, amountToCopy);
                    _fileStream.Write(bytesToCopy, 0, amountToCopy);
                    _fileStream.Flush();

                    if (_fileStream.Length == MaxSize) SwapLog();
                }
            }
            else
            {
                _memoryStream.WriteTo(_fileStream);
                _fileStream.Flush();
            }

            _memoryStream.Position = 0;
            _memoryStream.SetLength(0);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Rotating File Logs cannot be Read");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Rotating File Logs cannot be Seeked");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Rotating File Logs cannot have their length changed.");
        }

        private void CreateFile()
        {
            _fileStream = new FileStream(_path, FileMode.Append);
            _currentLogCreateDate = File.GetCreationTime(_path);
        }

        private void SwapLog()
        {
            _logStream.Dispose();
            _fileStream.Dispose();
            RotateFiles();
            CreateFile();
        }

        /**
         * TODO: Make this better
         */
        private void RotateFiles()
        {
            var index = 0;
            while (File.Exists(GetPathForIndex(index + 1))) index++;

            if (MaxFiles > 0)
                while (index > MaxFiles)
                {
                    File.Delete(GetPathForIndex(index));
                    index--;
                }

            while (index > 0)
            {
                File.Move(GetPathForIndex(index), GetPathForIndex(index + 1));
                index--;
            }

            File.Move(_path, GetPathForIndex(1));
        }

        private string GetPathForIndex(int index)
        {
            return _path + "." + index;
        }
    }
}