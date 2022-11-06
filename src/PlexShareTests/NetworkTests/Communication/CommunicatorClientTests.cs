/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using Xunit;

namespace PlexShareNetwork.Communication.Test
{
	public class CommunicatorClientTests
	{
        private static readonly CommunicatorClient _clientCommunicator = new();
        private static readonly CommunicatorServer _serverCommunicator = new();

        [Fact]
		public void ClientStartAndStopTest()
		{
            string clientCommunicatorReturn = _clientCommunicator.Start("0", "0");
            Assert.Equal("failure", clientCommunicatorReturn);

            string serverIPAndPort = _serverCommunicator.Start();
			string[] IPAndPort = serverIPAndPort.Split(":");

			clientCommunicatorReturn = _clientCommunicator.Start(IPAndPort[0], IPAndPort[1]);
			Assert.Equal("success", clientCommunicatorReturn);

            _serverCommunicator.Stop();
            _clientCommunicator.Stop();
		}
	}
}
