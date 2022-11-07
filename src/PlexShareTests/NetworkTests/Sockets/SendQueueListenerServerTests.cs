/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using Xunit;

namespace PlexShareNetwork.Sockets.Tests
{
	public class SendQueueListenerServerTests
	{
		private readonly SendingQueue _sendingQueue = new();
        private readonly ReceivingQueue _receivingQueue1 = new();
        private readonly ReceivingQueue _receivingQueue2 = new();
        private readonly ICommunicator _communicatorServer = CommunicationFactory.GetCommunicator(false);
		private readonly SendQueueListenerServer _sendQueueListenerServer;
        private readonly SocketListener _socketListener1;
        private readonly SocketListener _socketListener2;
        private readonly TcpClient _clientSocket1 = new();
        private readonly TcpClient _clientSocket2 = new();
        private TcpClient _serverSocket1, _serverSocket2;
		private readonly TcpListener _serverListener;
		private readonly Dictionary<string, TcpClient> _clientIdToSocket = new();
		private readonly Dictionary<string, INotificationHandler> _subscribedModules = new();
		private readonly int _port;
		private readonly IPAddress _IP;
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 100;
        private readonly int _largePacketSize = 10000;
        private readonly int _veryLargePacketSize = 10000000; // adding one more 0 to it will hang you laptop
        private readonly string[] _destinations = { "Client1 ID", "Client2 ID" };
        private readonly string _module = "Test Module";

        public SendQueueListenerServerTests()
		{
			string[] IPAndPort = _communicatorServer.Start().Split(":");
            _communicatorServer.Stop();
            _IP = IPAddress.Parse(IPAndPort[0]);
			_port = int.Parse(IPAndPort[1]);
			_serverListener = new TcpListener(_IP, _port);
			_serverListener.Start();

            _sendingQueue.RegisterModule("Test Module", true);
            _subscribedModules["Test Module"] = new TestNotificationHandler();

            _sendQueueListenerServer = new SendQueueListenerServer(_sendingQueue, _clientIdToSocket, _subscribedModules);
            _sendQueueListenerServer.Start();

            _clientSocket1.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            _clientSocket2.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            var t1 = Task.Run(() => { _clientSocket1.Connect(_IP, _port); });
			var t2 = Task.Run(() => { _serverSocket1 = _serverListener.AcceptTcpClient(); });
			Task.WaitAll(t1, t2);

            Task t3 = Task.Run(() => { _clientSocket2.Connect(_IP, _port); });
            Task t4 = Task.Run(() => { _serverSocket2 = _serverListener.AcceptTcpClient(); });
            Task.WaitAll(t3, t4);

			_clientIdToSocket["Client1 ID"] = _serverSocket1;
            _clientIdToSocket["Client2 ID"] = _serverSocket2;

            _socketListener1 = new SocketListener(_receivingQueue1, _clientSocket1);
            _socketListener1.Start();
            _socketListener2 = new SocketListener(_receivingQueue2, _clientSocket2);
            _socketListener2.Start();
        }

        private void PacketsUnicastTest(int size, string destination, int count)
        {
            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(size, destination, _module, count);
            NetworkTestGlobals.SendPackets(sendPackets, _sendingQueue, count);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue1, count);
        }

        private void PacketsBroadcastTest(int size, int count)
        {
            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(size, null, _module, count);
            NetworkTestGlobals.SendPackets(sendPackets, _sendingQueue, count);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue1, count);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue2, count);
        }

        private void PacketsMulticastTest(int size, int count)
        {
            Packet[] sendPackets1 = NetworkTestGlobals.GeneratePackets(size, _destinations[0], _module, count);
            Packet[] sendPackets2 = NetworkTestGlobals.GeneratePackets(size, _destinations[1], _module, count);
            for (var i = 0; i < count; i++)
            {
                Packet[] sendPackets = { sendPackets1[i], sendPackets2[i] };
                NetworkTestGlobals.SendPackets(sendPackets, _sendingQueue, 2);
            }
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets1, _receivingQueue1, count);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets2, _receivingQueue2, count);
        }

        [Fact]
		public void SmallPacketUnicastTest()
		{
            PacketsUnicastTest(_smallPacketSize, _destinations[0], 1);
        }

		[Fact]
		public void LargePacketUnicastTest()
		{
            PacketsUnicastTest(_largePacketSize, _destinations[0], 1);
        }

        [Fact]
        public void VeryLargePacketUnicastTest()
        {
            PacketsUnicastTest(_veryLargePacketSize, _destinations[0], 1);
        }

        [Fact]
        public void MultipleSmallPacketsUnicastTest()
        {
            PacketsUnicastTest(_smallPacketSize, _destinations[0], _multiplePacketsCount);
        }

        [Fact]
        public void MultipleLargePacketsUnicastTest()
        {
            PacketsUnicastTest(_largePacketSize, _destinations[0], _multiplePacketsCount);
        }

        [Fact]
        public void SmallPacketBroadcastTest()
        {
            PacketsBroadcastTest(_smallPacketSize, 1);
        }

        [Fact]
        public void LargePacketBroadcastTest()
        {
            PacketsBroadcastTest(_largePacketSize, 1);
        }

        [Fact]
        public void VeryLargePacketBroadcastTest()
        {
            PacketsBroadcastTest(_veryLargePacketSize, 1);
        }

        [Fact]
        public void MultipleSmallPacketsBroadcastTest()
        {
            PacketsBroadcastTest(_smallPacketSize, _multiplePacketsCount);
        }

        [Fact]
        public void MultipleLargePacketsBroadcastTest()
        {
            PacketsBroadcastTest(_largePacketSize, _multiplePacketsCount);
        }

        [Fact]
        public void SmallPacketMulticastTest()
        {
            PacketsMulticastTest(_smallPacketSize, 1);
        }

        [Fact]
        public void LargePacketMulticastTest()
        {
            PacketsMulticastTest(_largePacketSize, 1);
        }

        [Fact]
        public void VeryLargePacketMulticastTest()
        {
            PacketsMulticastTest(_veryLargePacketSize, 1);
        }

        [Fact]
        public void MultipleSmallPacketsMulticastTest()
        {
            PacketsMulticastTest(_smallPacketSize, _multiplePacketsCount);
        }

        [Fact]
        public void MultipleLargePacketsMulticastTest()
        {
            PacketsMulticastTest(_largePacketSize, _multiplePacketsCount);
        }

        [Fact]
		public void ClientGotDisconnectedTest()
		{
            // client1 got disconnected, modules should be notified about client1 and
            // client2 should still receive the sent data as it is being broadcasted
            TestNotificationHandler testNotificationHandler = (TestNotificationHandler) _subscribedModules["Test Module"];
			_clientSocket1.Close();
			_clientSocket1.Dispose();
            Packet[] sendPackets = { new("Test string", null, _module) };
            _sendingQueue.Enqueue(sendPackets[0]);

            testNotificationHandler.WaitForEvent();
            // assert module is notified, client1 did not receive, and client2 received
			Assert.Equal("OnClientLeft", testNotificationHandler.GetLastEvent());
            Assert.Equal("Client1 ID", testNotificationHandler.GetLastEventClientId());
            Assert.True(_receivingQueue1.IsEmpty());
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue2, 1);
        }
    }
}
