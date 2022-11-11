using PlexShareWhiteboard.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.WhiteboardTests.Server
{
    public class ServerCommunicatorTests
    {
        [Fact]
        public void Instance_Always_ReturnsSameInstance()
        {
            ServerCommunicator comm1 = ServerCommunicator.Instance;
            ServerCommunicator comm2 = ServerCommunicator.Instance;
            Assert.Equal(comm1, comm2);
        }
    }
}
