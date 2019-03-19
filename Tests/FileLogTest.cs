using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Xunit;
using LogUtilities;

namespace Tests
{
    public class FileLogTest
    {
        [Fact]
        public void CreateNewFile()
        {
            var filename = GetTempFile();

            using (var fileLog = new FileLog(filename, datetimeFormat: null))
            {
                fileLog.Write("test");
            }

            var text = File.ReadAllText(filename);
            
            Assert.Equal("test", text);
            
            File.Delete(filename);
        }

        [Fact]
        public void AppendToFile()
        {
            var filename = GetTempFile();
            File.WriteAllText(filename, "test");

            using (var fileLog = new FileLog(filename, datetimeFormat: null))
            {
                fileLog.Write("test");
            }
            

            var text = File.ReadAllText(filename);
            
            Assert.Equal("testtest", text);
            
            File.Delete(filename);   
        }

        private static string GetTempFile()
        {
            return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + (new Random()).Next() +".log";
        }
    }
}