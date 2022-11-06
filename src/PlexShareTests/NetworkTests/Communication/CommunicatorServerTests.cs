/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using PlexShareNetwork.Communication;
using PlexShareNetwork;
using Xunit;

namespace PlexShareNetwork.Communication.Test
{
    public class CommunicatorServerTests
    {
        private static readonly CommunicatorServer _serverCommunicator = new();

        [Fact]
        public void ServerStartAndStopTest()
        {
            string serverIPAndPort = _serverCommunicator.Start();
            Assert.True(serverIPAndPort != null);
            _serverCommunicator.Stop();
        }
    }
}
