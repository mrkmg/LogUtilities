using System;
using System.IO;
using System.Text.RegularExpressions;
using LogUtilities;
using Xunit;

namespace Tests
{
    public class LogStreamTest
    {
        [Fact]
        public void DoesntWritePrefixUntilNewLine()
        {
            using (var memStream = new MemoryStream())
            {
                using (var logStream = new LogStream(memStream) {Prefix = "A"})
                {
                    logStream.Write("Test1");
                    logStream.Write("Test2\n");
                    logStream.Write("Test3");
                    logStream.Write("Test4");
                }

                memStream.Position = 0;
                var test = new StreamReader(memStream).ReadToEnd();

                Assert.Equal("A | Test1Test2\nA | Test3Test4", test);
            }
        }

        [Fact]
        public void EmbeddedLogStreams()
        {
            using (var memStream = new MemoryStream())
            {
                using (var logStreamA = new LogStream(memStream) {Prefix = "A"})
                {
                    using (var logStreamB = new LogStream(logStreamA) {Prefix = "B"})
                    {
                        logStreamB.WriteLine("Test1");
                        logStreamB.WriteLine("Test2");
                    }
                }

                memStream.Position = 0;
                var test = new StreamReader(memStream).ReadToEnd();

                Assert.Equal("A | B | Test1\nA | B | Test2\n", test);
            }
        }

        [Fact]
        public void WriteLineWritesNewLine()
        {
            using (var memStream = new MemoryStream())
            {
                using (var logStream = new LogStream(memStream) {Prefix = "A"})
                {
                    logStream.WriteLine("Test1");
                    logStream.WriteLine("Test2");
                    logStream.WriteLine("Test3");
                    logStream.WriteLine("Test4");
                }

                memStream.Position = 0;
                var test = new StreamReader(memStream).ReadToEnd();

                Assert.Equal("A | Test1\nA | Test2\nA | Test3\nA | Test4\n", test);
            }
        }

        [Fact]
        public void WritesDateAndPrefix()
        {
            using (var memStream = new MemoryStream())
            {
                using (var logStream = new LogStream(memStream) {Prefix = "A", DateTimeFormat = "s"})
                {
                    logStream.Write("Test");
                }

                memStream.Position = 0;
                var test = new StreamReader(memStream).ReadToEnd();

                Assert.Matches(new Regex(@"\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d \| A \| Test"), test);
            }
        }

        [Fact]
        public void WritesDatePrefix()
        {
            using (var memStream = new MemoryStream())
            {
                using (var logStream = new LogStream(memStream) {DateTimeFormat = "s"})
                {
                    logStream.Write("Test");
                }

                memStream.Position = 0;
                var test = new StreamReader(memStream).ReadToEnd();

                Assert.Matches(new Regex(@"\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d \| Test"), test);
            }
        }

        [Fact]
        public void WritesInitialPrefix()
        {
            using (var memStream = new MemoryStream())
            {
                using (var logStream = new LogStream(memStream) {Prefix = "A"})
                {
                    logStream.Write("Test");
                }

                memStream.Position = 0;
                var test = new StreamReader(memStream).ReadToEnd();

                Assert.Equal("A | Test", test);
            }
        }

        [Fact]
        public void WriteFromEnumerable()
        {
            using (var memStream = new MemoryStream())
            {
                using (var logStream = new LogStream(memStream))
                {
                    logStream.Write(new[] {"A", "B", "C"});
                }

                memStream.Position = 0;
                var test = new StreamReader(memStream).ReadToEnd();

                Assert.Equal("A | B | C", test);
            }
        }

        [Fact]
        public void ShouldDisposeInnerStream()
        {
            var memStream = new MemoryStream();
            var logStream = new LogStream(memStream) { AutoDisposeInner = true };
            logStream.Dispose();
            Assert.Throws<ObjectDisposedException>((() => memStream.Write(new[] {(byte) 0}, 0, 1)));
        }
    }
}