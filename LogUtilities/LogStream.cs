using System;
using System.IO;
using System.Text;

namespace LogUtilities
{
    public class LogStream : Stream
    {
        private readonly Stream _baseStream;
        private string _datetimeFormat;
        private bool _includeDatetime;
        private bool _includePrefix;
        private byte[] _prefix;
        private string _separator;

        public LogStream(Stream baseStream, string prefix = null, string datetimeFormat = "s", string separator = " | ")
        {
            _baseStream = baseStream;
            Initialize(prefix, datetimeFormat, separator);
        }

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => _baseStream.CanWrite;
        public override long Length => _baseStream.Length;

        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var totalLength = count;

            if (_includePrefix) totalLength += _prefix.Length;

            var dateBytes = new byte[0];

            if (_includeDatetime)
            {
                dateBytes = GetDateStampBytes(_datetimeFormat, _separator);
                totalLength += dateBytes.Length;
            }

            var bytes = new byte[totalLength];
            var currentOffset = 0;

            if (_includeDatetime)
            {
                Buffer.BlockCopy(dateBytes, 0, bytes, 0, dateBytes.Length);
                currentOffset += dateBytes.Length;
            }

            if (_includePrefix)
            {
                Buffer.BlockCopy(_prefix, 0, bytes, currentOffset, _prefix.Length);
                currentOffset += _prefix.Length;
            }

            Buffer.BlockCopy(buffer, offset, bytes, currentOffset, count);

            _baseStream.Write(bytes, 0, bytes.Length);
        }

        public override void Close()
        {
            _baseStream.Close();
            base.Close();
        }

        private void Initialize(string prefix, string datetimeFormat, string separator)
        {
            _separator = separator;

            if (prefix != null)
            {
                _includePrefix = true;
                _prefix = Encoding.UTF8.GetBytes(prefix + separator);
            }

            if (datetimeFormat != null)
            {
                _includeDatetime = true;
                _datetimeFormat = datetimeFormat;
            }
        }

        private static byte[] GetDateStampBytes(string format, string separator)
        {
            return Encoding.UTF8.GetBytes(DateTime.Now.ToString(format) + separator);
        }
    }
}