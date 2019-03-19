using System;
using System.IO;
using System.Threading;
using LogUtilities;
using Xunit;

namespace Tests
{
    public class RotatingFileLogTest
    {
        [Fact]
        public void RotateBySize()
        {
            var filename = GetTempFile();

            using (var log = new RotatingFileLog(filename, datetimeFormat: null))
            {
                log.MaxSize = 2;
                log.Write("A");
                log.Write("B");
                log.Write("C");
            }
            
            Assert.True(File.Exists(filename));
            Assert.True(File.Exists(filename + ".1"));
            Assert.Equal("AB", File.ReadAllText(filename + ".1"));
            Assert.Equal("C", File.ReadAllText(filename));
            
            File.Delete(filename);
            File.Delete(filename + ".1");
        }

        [Fact]
        public void RotateByAge()
        {
            var filename = GetTempFile();

            using (var log = new RotatingFileLog(filename, datetimeFormat: null))
            {
                log.MaxAge = 2;
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
        
        private static string GetTempFile()
        {
            return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + (new Random()).Next() +".log";
        }
    }
}