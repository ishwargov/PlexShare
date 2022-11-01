/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener.
/// </summary>

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Networking.Sockets.Test
{
	[TestClass()]
	public class CommunicatorTest
	{
		[TestMethod()]
		public void ClientAndServerStartAndStopTest()
		{
			var server = CommunicationFactory.GetCommunicator(false, true);
			var serverIPAndPort = server.Start();
			var IPAndPort = serverIPAndPort.Split(":");

			var client1 = CommunicationFactory.GetCommunicator(true, true);
			var client1Return = client1.Start(IPAndPort[0], IPAndPort[1]);

			var client2 = CommunicationFactory.GetCommunicator(true, true);
			var client2Return = client2.Start(IPAndPort[0], IPAndPort[1]);

			Assert.AreEqual("1", client1Return);
			Assert.AreEqual("1", client2Return);

			server.Stop();
			client1.Stop();
			client2.Stop();
		}
	}
}
