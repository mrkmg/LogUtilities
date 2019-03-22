using System;
using System.IO;
using System.Threading;
using LogUtilities;
using Xunit;

namespace Tests
{
    public class RotatingFileLogStreamTest : IDisposable
    {
        public RotatingFileLogStreamTest()
        {
            _testDirectory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + new Random().Next();
            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            foreach (var file in Directory.GetFiles(_testDirectory)) File.Delete(file);

            Directory.Delete(_testDirectory);
        }

        private readonly string _testDirectory;

        private string GetTempFilePath()
        {
            return _testDirectory + Path.DirectorySeparatorChar + new Random().Next() + ".log";
        }

        [Fact]
        public void DoesWorkWithEmbedded()
        {
            var filename = GetTempFilePath();

            using (var rotateLog = new RotatingFileLogStream(filename) {Prefix = "Rotate"})
            {
                using (var innerLog = new LogStream(rotateLog) {Prefix = "Inner"})
                {
                    innerLog.Write("test");
                }
            }

            Assert.True(File.Exists(filename));
            Assert.Equal("Rotate | Inner | test", File.ReadAllText(filename));
        }

        [Fact]
        public void RemovesExtraFile()
        {
            var filename = GetTempFilePath();

            using (var log = new RotatingFileLogStream(filename) {MaxSize = 1, MaxFiles = 2})
            {
                log.Write("ABCD");
            }

            Assert.True(File.Exists(filename));
            Assert.True(File.Exists(filename + ".1"));
            Assert.True(File.Exists(filename + ".2"));
            Assert.False(File.Exists(filename + ".3"));
        }

        [Fact]
        public void RotateByAge()
        {
            var filename = GetTempFilePath();

            using (var log = new RotatingFileLogStream(filename) {MaxAge = 2})
            {
                log.Write("A");
                Thread.Sleep(3000);
                log.Write("B");
            }

            Assert.True(File.Exists(filename));
            Assert.True(File.Exists(filename + ".1"));
            Assert.Equal("A", File.ReadAllText(filename + ".1"));
            Assert.Equal("B", File.ReadAllText(filename));
        }

        [Fact]
        public void RotateBySize()
        {
            var filename = GetTempFilePath();

            using (var log = new RotatingFileLogStream(filename) {MaxSize = 100})
            {
                log.Write(new string('A', 75));
                log.Write(new string('B', 50));
            }

            Assert.True(File.Exists(filename));
            Assert.True(File.Exists(filename + ".1"));
            Assert.Equal(new string('A', 75) + new string('B', 25), File.ReadAllText(filename + ".1"));
            Assert.Equal(new string('B', 25), File.ReadAllText(filename));
        }
    }
}