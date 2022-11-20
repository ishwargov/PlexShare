using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareWhiteboard.Client;

namespace PlexShareTests.WhiteboardTests.Client
{
    [Collection("Sequential")]
    public class ClientCommunicatorTests
    {
        [Fact]
        public void Instance_Always_ReturnsSameInstance()
        {
            ClientCommunicator comm1 = ClientCommunicator.Instance;
            ClientCommunicator comm2 = ClientCommunicator.Instance;
            Assert.Equal(comm1, comm2);
        }
        
    }
}