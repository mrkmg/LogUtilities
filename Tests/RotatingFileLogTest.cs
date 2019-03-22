using System;
using System.IO;
using System.Threading;
using LogUtilities;
using Xunit;

namespace Tests
{
    public class RotatingFileLogTest
    {
        private static string GetTempFile()
        {
            return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + new Random().Next() + ".log";
        }

        [Fact]
        public void RotateByAge()
        {
            var filename = GetTempFile();

            using (var log = new RotatingFileLog(filename) {MaxAge = 2})
            {
                log.Write("A");
                Thread.Sleep(3000);
                log.Write("B");
            }

            Assert.True(File.Exists(filename));
            Assert.True(File.Exists(filename + ".1"));
            Assert.Equal("A", File.ReadAllText(filename + ".1"));
            Assert.Equal("B", File.ReadAllText(filename));

            File.Delete(filename);
            File.Delete(filename + ".1");
        }

        [Fact]
        public void RotateBySize()
        {
            var filename = GetTempFile();

            using (var log = new RotatingFileLog(filename) {MaxSize = 100})
            {
                log.Write(new string('A', 75));
                log.Write(new string('B', 50));
            }

            Assert.True(File.Exists(filename));
            Assert.True(File.Exists(filename + ".1"));
            Assert.Equal(new string('A', 75) + new string('B', 25), File.ReadAllText(filename + ".1"));
            Assert.Equal(new string('B', 25), File.ReadAllText(filename));

            File.Delete(filename);
            File.Delete(filename + ".1");
        }

        [Fact]
        public void DoesWorkWithEmbedded()
        {
            var filename = GetTempFile();

            using (var rotateLog = new RotatingFileLog(filename) {Prefix = "Rotate"})
            {
                using (var innerLog = new LogStream(rotateLog) {Prefix = "Inner"})
                {
                    innerLog.Write("test");
                }
            }
            
            Assert.True(File.Exists(filename));
            Assert.Equal("Rotate | Inner | test", File.ReadAllText(filename));
            File.Delete(filename);
        }
    }
}