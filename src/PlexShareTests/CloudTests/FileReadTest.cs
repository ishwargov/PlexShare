using Microsoft.AspNetCore.WebUtilities;
using PlexShareCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareCloudUX;

namespace PlexShareTests.CloudTests
{
    public class FileReadTest
    {
        [Fact]
        public void GetPath()
        {
            string[] lines = FileRead.GetPaths("OfflineSetup_Path.txt");
            Assert.NotNull(lines);
        }

        [Fact]
        public void WrongFileName()
        {
            //Give the wrong filename which does not exist in the directory. 
            string[] lines = FileRead.GetPaths("temp.txt");
            //It should handle filenotfound exception.
            Assert.Equal("File Not Found", lines[0]);
        }
    }
}
