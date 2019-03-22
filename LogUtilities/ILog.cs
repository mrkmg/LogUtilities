using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LogUtilities
{
    public interface ILog
    {
        /// <summary>
        /// The encoding to use when writing strings.
        /// </summary>
        Encoding Encoding { get; set; }
        
        /// <summary>
        /// Which format to use when prepending a datetime to the beginning of the line.
        ///
        /// <see cref="System.DateTime.ToString(string)"/>
        /// </summary>
        string DateTimeFormat { get; set; }
        
        /// <summary>
        /// A prefix to prepend to each line.
        /// </summary>
        string Prefix { get; set; }
        
        /// <summary>
        /// A string to separate blocks in the line. Also used to separate the datetime and prefix.
        /// </summary>
        string Separator { get; set; }

        /// <summary>
        ///     Write a chunk of text.
        ///     Does not write the prefix.
        /// </summary>
        /// <param name="text"></param>
        void Write(string text);

        /// <summary>
        ///     Write a line of text.
        ///     Automatically writes the datetime and prefix to the line.
        /// </summary>
        /// <param name="text"></param>
        void WriteLine(string text);

        /// <summary>
        /// Write blocks of texts separated by <see cref="Separator"/>
        /// </summary>
        /// <param name="textBlocks">Blocks of text to write.</param>
        void Write(IEnumerable<string> textBlocks);

        /// <summary>
        /// Write blocks of texts separated by <see cref="Separator"/>, along with a line terminator.
        /// </summary>
        /// <param name="textBlocks">Blocks of text to write.</param>
        void WriteLine(IEnumerable<string> textBlocks);
        
        /// <summary>
        /// Finishes a line.
        /// </summary>
        void WriteLine();
    }
}