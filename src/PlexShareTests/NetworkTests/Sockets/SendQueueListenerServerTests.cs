/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

		public SendQueueListenerServerTests()
		{
			string[] IPAndPort = _communicatorServer.Start().Split(":");
            _communicatorServer.Stop();
            _IP = IPAddress.Parse(IPAndPort[0]);
			_port = int.Parse(IPAndPort[1]);
			_serverListener = new TcpListener(_IP, _port);
			_serverListener.Start();

            _sendingQueue.RegisterModule("Test Module1", true);
            _subscribedModules["Test Module1"] = new TestNotificationHandler();
            _sendingQueue.RegisterModule("Test Module2", true);
            _subscribedModules["Test Module2"] = new TestNotificationHandler();

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

		[Fact]
		public void SinglePacketUnicastTest()
		{
            Packet[] sendPackets = { new("Test string", "Client1 ID", "Test Module1") };
            NetworkTestGlobals.SendAndReceiveAssert(sendPackets, _sendingQueue, _receivingQueue1, 1);
        }

		[Fact]
		public void LargePacketUnicastTest()
		{
            Packet[] sendPackets = { new(NetworkTestGlobals.RandomString(1000), "Client1 ID", "Test Module1") };
            NetworkTestGlobals.SendAndReceiveAssert(sendPackets, _sendingQueue, _receivingQueue1, 1);
        }

        [Fact]
        public void MultipleClientsUnicastTest()
        {
            Packet[] sendPackets1 = { new("Test string1", "Client1 ID", "Test Module1") };
            Packet[] sendPackets2 = { new("Test string2", "Client2 ID", "Test Module2") };
            NetworkTestGlobals.SendAndReceiveAssert(sendPackets1, sendPackets2, _sendingQueue, _receivingQueue1, _receivingQueue2, 1, 1);
        }

        [Fact]
        public void MultiplePacketsUnicastTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new("Test string" + i, "Client1 ID", "Test Module1");
            }
            NetworkTestGlobals.SendAndReceiveAssert(sendPackets, _sendingQueue, _receivingQueue1, 10);
        }

        [Fact]
        public void MultipleLargePacketsUnicastTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new(NetworkTestGlobals.RandomString(1000), "Client1 ID", "Test Module1");
            }
            NetworkTestGlobals.SendAndReceiveAssert(sendPackets, _sendingQueue, _receivingQueue1, 10);
        }

        [Fact]
        public void SinglePacketBroadcastTest()
        {
            Packet[] sendPackets = { new("Test string", null, "Test Module1") };
            _sendingQueue.Enqueue(sendPackets[0]);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue1, 1);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue2, 1);
        }

        [Fact]
        public void LargePacketBroadcastTest()
        {
            Packet[] sendPackets = { new(NetworkTestGlobals.RandomString(1000), null, "Test Module1") };
            _sendingQueue.Enqueue(sendPackets[0]);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue1, 1);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue2, 1);
        }

        [Fact]
        public void MultiplePacketsBroadcastTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new("Test string" + i, null, "Test Module1");
                _sendingQueue.Enqueue(sendPackets[i]);
            }
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue1, 10);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue2, 10);
        }

        [Fact]
        public void MultipleLargePacketsBroadcastTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new(NetworkTestGlobals.RandomString(1000), null, "Test Module1");
                _sendingQueue.Enqueue(sendPackets[i]);
            }
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue1, 10);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue2, 10);
        }

        [Fact]
		public void ClientGotDisconnectedTest()
		{
            // client1 got disconnected, other modules should be notified about client1 and
            // client2 should still receive the sent data as it is being broadcasted
            TestNotificationHandler testNotificationHandler = (TestNotificationHandler) _subscribedModules["Test Module1"];
            testNotificationHandler.Reset();
			_clientSocket1.Close();
			_clientSocket1.Dispose();
            Packet[] sendPackets = { new("Test string", null, "Test Module1") };
            _sendingQueue.Enqueue(sendPackets[0]);
            testNotificationHandler.WaitForEvent();

            // assert module is notified, client1 did not receive, and client2 received
			Assert.Equal("OnClientLeft", testNotificationHandler.Event);
            Assert.Equal("Client1 ID", testNotificationHandler.ClientID);
            Assert.True(_receivingQueue1.IsEmpty());
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue2, 1);
        }
    }
}
