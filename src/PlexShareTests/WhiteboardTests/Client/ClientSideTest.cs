using PlexShareWhiteboard.Client;
using PlexShareWhiteboard.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.WhiteboardTests.Client
{
    internal class ClientSideTest
    {
        public void Instance_Always_ReturnsSameInstance()
        {
            ClientSide client1 = ClientSide.Instance;
            ClientSide client2 = ClientSide.Instance;
            Assert.Equal(client1, client2);
        }
    }
}
