using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareWhiteboard.Server;

namespace PlexShareTests.WhiteboardTests.Server
{

    public class ServerSideTests
    {
        [Fact]
        public void Instance_Always_ReturnsSameInstance()
        {
            ServerSide server1 = ServerSide.Instance;
            ServerSide server2 = ServerSide.Instance;
            Assert.Equal(server1, server2);
        }

    }
}
