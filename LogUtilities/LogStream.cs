using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogUtilities
{
    public class LogStream : AbstractLogStream
    {
        private const int NewLine = 0xA;

        private readonly Stream _baseStream;
        private bool _isNewLine = true;
        
        /// <summary>
        /// Should LogStream dispose the baseStream when it disposes.
        /// </summary>
        public bool AutoDisposeInner;

        public LogStream(Stream baseStream)
        {
            _baseStream = baseStream;
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

        /// <summary>
        /// Create an instance of LogStream which writes to a file.
        /// </summary>
        /// <param name="path">Path to the log file.</param>
        /// <returns>Instance of LogStream</returns>
        public static LogStream ToFile(string path)
        {
            return new LogStream(File.Open(path, FileMode.Append)) {AutoDisposeInner = true};
        }

        /// <summary>
        /// Create an instance of LogStream which outputs to a console
        /// </summary>
        /// <param name="type"><see cref="LogStreamConsoleType"/></param>
        /// <returns>Instance of LogStream</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static LogStream ToConsole(LogStreamConsoleType type = LogStreamConsoleType.Stdout)
        {
            switch (type)
            {
                case LogStreamConsoleType.Stdout:
                    return new LogStream(Console.OpenStandardOutput());
                case LogStreamConsoleType.Stderr:
                    return new LogStream(Console.OpenStandardError());
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public new void Dispose()
        {
            if (AutoDisposeInner) _baseStream.Dispose();
            base.Dispose();
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
            var bufferBlocks = SplitByteArrayByNewLine(buffer);

            foreach (var bufferBlock in bufferBlocks)
            {
                if (_isNewLine) WritePrefix();
                _baseStream.Write(bufferBlock, 0, bufferBlock.Length);
                _isNewLine = bufferBlock[bufferBlock.Length - 1] == NewLine;
            }
        }

        private void WritePrefix()
        {
            if (DateTimeFormat != null)
            {
                var dateBytes = Encoding.GetBytes(DateTime.Now.ToString(DateTimeFormat) + Separator);
                _baseStream.Write(dateBytes, 0, dateBytes.Length);
            }

            if (Prefix != null)
            {
                var prefixBytes = Encoding.GetBytes(Prefix + Separator);
                _baseStream.Write(prefixBytes, 0, prefixBytes.Length);
            }
        }

        private static IEnumerable<byte[]> SplitByteArrayByNewLine(byte[] source)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));

            var currentIdx = 0;
            var lastIdx = 0;

            while (currentIdx < source.Length)
            {
                if (source[currentIdx] == NewLine)
                {
                    var newArray = new byte[1 + currentIdx - lastIdx];
                    Array.Copy(source, lastIdx, newArray, 0, 1 + currentIdx - lastIdx);
                    yield return newArray;
                    lastIdx = currentIdx + 1;
                }

                currentIdx++;
            }

            if (currentIdx != lastIdx)
            {
                var newArray = new byte[currentIdx - lastIdx];
                Array.Copy(source, lastIdx, newArray, 0, currentIdx - lastIdx);
                yield return newArray;
            }
        }
    }

    public enum LogStreamConsoleType
    {
        Stdout,
        Stderr
    }
}