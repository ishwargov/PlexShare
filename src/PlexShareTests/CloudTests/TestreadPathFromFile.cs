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
    public class TestreadPathFromFile
    {
        [Fact]
        public void GetPath()
        {
            // Create an entity.
            //Logger.LogMessage("Create an entity.");
            /*
            Assert.Equal(1, getEntity?.Count);
            for (int i = 0; i < getEntity?.Count; i++)
            {
                Assert.Equal(postEntity?.SessionId, getEntity[i].SessionId);
            }*/
            //FileRead fileRead = new FileRead;
            string[] lines = FileRead.GetPaths();
            Assert.NotNull(lines);
            //Assert.Equal(postEntity?.Name, getEntity?.Name);
        }
    }
}
