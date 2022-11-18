using Client.Models;
using PlexShareWhiteboard.Client;
using PlexShareWhiteboard.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.WhiteboardTests.Client
{
    [Collection("Sequential")]
    public class ClientSideTests
    {
        ClientSide client;
        public ClientSideTests()
        {
            client = ClientSide.Instance;
        }

        [Fact]
        public void Instance_Always_ReturnsSameInstance()
        {
            ClientSide client1 = ClientSide.Instance;
            ClientSide client2 = ClientSide.Instance;
            Assert.Equal(client1, client2);
        }

        [Fact]
        public void SnapshotNoIncrease_OnSave()
        {
            Assert.Equal(client.OnSaveMessage("1"),1);
            Assert.Equal(client.OnSaveMessage("1"), 2);
        }
    }
}
