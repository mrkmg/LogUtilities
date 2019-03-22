using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LogUtilities
{
    public abstract class AbstractLogStream : Stream
    {
        /// <summary>
        ///     The encoding to use when writing strings.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        ///     Which format to use when prepending a datetime to the beginning of the line.
        ///     <see cref="System.DateTime.ToString(string)" />
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        ///     A prefix to prepend to each line.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        ///     A string to separate blocks in the line. Also used to separate the datetime and prefix.
        /// </summary>
        public string Separator { get; set; } = " | ";

        /// <summary>
        ///     Write a chunk of text.
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            var bytes = Encoding.GetBytes(text);
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///     Write a line of text.
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        /// <summary>
        ///     Write blocks of texts separated by <see cref="Separator" />
        /// </summary>
        /// <param name="textBlocks">Blocks of text to write.</param>
        public void Write(params string[] textBlocks)
        {
            Write(JoinBlocks(textBlocks));
        }

        /// <summary>
        ///     Write blocks of texts separated by <see cref="Separator" />, along with a line terminator.
        /// </summary>
        /// <param name="textBlocks">Blocks of text to write.</param>
        public void WriteLine(params string[] textBlocks)
        {
            WriteLine(JoinBlocks(textBlocks));
        }

        /// <summary>
        ///     Finishes a line.
        /// </summary>
        public void WriteLine()
        {
            Write(Environment.NewLine);
        }

        private string JoinBlocks(string[] textBlocks)
        {
            return textBlocks.Aggregate((l, r) => l + Separator + r);
        }
    }
}