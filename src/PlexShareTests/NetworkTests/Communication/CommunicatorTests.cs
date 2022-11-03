/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using Xunit;

namespace PlexShareNetwork.Communication.Test
{
	public class CommunicatorTest
	{
		[Fact]
		public void ClientAndServerStartAndStopTest()
		{
			// Creating and starting a server
			ICommunicator server = CommunicationFactory.GetCommunicator(false, true);
			string serverIPAndPort = server.Start();
			string[] IPAndPort = serverIPAndPort.Split(":");

			// Starting client1
			ICommunicator client1 = CommunicationFactory.GetCommunicator(true, true);
			string client1Return = client1.Start(IPAndPort[0], IPAndPort[1]);

			// Starting client2
			ICommunicator client2 = CommunicationFactory.GetCommunicator(true, true);
			string client2Return = client2.Start(IPAndPort[0], IPAndPort[1]);

			// Checking whether both clients have successfully started
			Assert.Equal("1", client1Return);
			Assert.Equal("1", client2Return);

			server.Stop();
			client1.Stop();
			client2.Stop();
		}
	}
}
