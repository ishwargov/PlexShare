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
using PlexShareNetworking.Queues;
using Xunit;

namespace PlexShareNetworking.Sockets.Tests
{
	public class SendQueueListenerServerTest
	{
		private readonly SendingQueues _sendQueue = new();
        private ReceivingQueue _receiveQueue1 = new();
        private ReceivingQueue _receiveQueue2 = new();
		private readonly Machine _server = new FakeServer();
		private readonly SendQueueListenerServer _sendQueueListenerServer;
		private SocketListener _socketListener1, _socketListener2;
        private TcpClient _clientSocket1 = new();
        private TcpClient _clientSocket2 = new();
        private TcpClient _serverSocket1, _serverSocket2;
		private readonly TcpListener _serverListener;
		private Dictionary<string, TcpClient> _clientIdToSocket = new();
		private Dictionary<string, INotificationHandler> _subscribedModules = new();
		private readonly int _port;
		private readonly IPAddress _IP;

		public SendQueueListenerServerTest()
		{
			var IPAndPort = _server.Communicator.Start().Split(":");
            _server.Communicator.Stop();
            _IP = IPAddress.Parse(IPAndPort[0]);
			_port = int.Parse(IPAndPort[1]);
			_serverListener = new TcpListener(_IP, _port);
			_serverListener.Start();

            _sendQueue.RegisterModule("Test Module", true);
            _subscribedModules["Test Module"] = new FakeNotificationHandler();
            _sendQueueListenerServer = new SendQueueListenerServer(_sendQueue, _clientIdToSocket, _subscribedModules);
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

            _socketListener1 = new SocketListener(_receiveQueue1, _clientSocket1);
            _socketListener1.Start();
            _socketListener2 = new SocketListener(_receiveQueue2, _clientSocket2);
            _socketListener2.Start();
        }

		[Fact]
		public void SinglePacketUnicastTest()
		{
            Packet sendPacket = new("Test string", "Client1 ID", "Test Module");
			_sendQueue.Enqueue(sendPacket);

			while (_receiveQueue1.IsEmpty())
			{
                Thread.Sleep(100);
            }
            Assert.True(_receiveQueue1.Size() == 1);

            Packet receivedPacket = _receiveQueue1.Dequeue();
			Assert.Equal(sendPacket.getSerializedData(), receivedPacket.getSerializedData());
            Assert.Equal(sendPacket.getDestination(), receivedPacket.getDestination());
            Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket.getModuleOfPacket());
		}

		[Fact]
		public void LargeSizePacketUnicastTest()
		{
            Packet sendPacket = new Packet (NetworkingGlobals.RandomString(4000), "Client1 ID", "Test Module");
			_sendQueue.Enqueue(sendPacket);

			while (_receiveQueue1.IsEmpty())
			{
                Thread.Sleep(100);
            }
            Assert.True(_receiveQueue1.Size() == 1);

            Packet receivedPacket = _receiveQueue1.Dequeue();
			Assert.Equal(sendPacket.getSerializedData(), receivedPacket.getSerializedData());
            Assert.Equal(sendPacket.getDestination(), receivedPacket.getDestination());
            Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket.getModuleOfPacket());
		}

        [Fact]
        public void MultiplePacketsUnicastTest()
        {
            for (var i = 1; i <= 10; i++)
            {
                Packet sendPacket = new("Test string" + i, "Client1 ID", "Test Module");
                _sendQueue.Enqueue(sendPacket);
            }

            while (_receiveQueue1.Size() != 10)
            {
                Thread.Sleep(100);
            }
            Assert.True(_receiveQueue1.Size() == 10);

            for (var i = 1; i <= 10; i++)
            {
                Packet receivedPacket = _receiveQueue1.Dequeue();
                Assert.Equal("Test string" + i, receivedPacket.getSerializedData());
                Assert.Equal("Client1 ID", receivedPacket.getDestination());
                Assert.Equal("Test Module", receivedPacket.getModuleOfPacket());
            }
        }

        [Fact]
        public void SinglePacketBroadcastTest()
        {
            Packet sendPacket = new("Test string", null, "Test Module");
            _sendQueue.Enqueue(sendPacket);

            while (_receiveQueue1.IsEmpty() || _receiveQueue2.IsEmpty())
            {
                Thread.Sleep(100);
            }
            Assert.True(_receiveQueue1.Size() == 1 && _receiveQueue2.Size() == 1);

            Packet receivedPacket1 = _receiveQueue1.Dequeue();
            Assert.Equal(sendPacket.getSerializedData(), receivedPacket1.getSerializedData());
            Assert.Equal(sendPacket.getDestination(), receivedPacket1.getDestination());
            Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket1.getModuleOfPacket());

            Packet receivedPacket2 = _receiveQueue2.Dequeue();
            Assert.Equal(sendPacket.getSerializedData(), receivedPacket2.getSerializedData());
            Assert.Equal(sendPacket.getDestination(), receivedPacket2.getDestination());
            Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket2.getModuleOfPacket());
        }

        [Fact]
        public void LargeSizePacketBroadcastTest()
        {
            Packet sendPacket = new(NetworkingGlobals.RandomString(4000), null, "Test Module");
            _sendQueue.Enqueue(sendPacket);

            while (_receiveQueue1.IsEmpty() || _receiveQueue2.IsEmpty())
            {
                Thread.Sleep(100);
            }
            Assert.True(_receiveQueue1.Size() == 1 && _receiveQueue2.Size() == 1);

            Packet receivedPacket1 = _receiveQueue1.Dequeue();
            Assert.Equal(sendPacket.getSerializedData(), receivedPacket1.getSerializedData());
            Assert.Equal(sendPacket.getDestination(), receivedPacket1.getDestination());
            Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket1.getModuleOfPacket());

            Packet receivedPacket2 = _receiveQueue2.Dequeue();
            Assert.Equal(sendPacket.getSerializedData(), receivedPacket2.getSerializedData());
            Assert.Equal(sendPacket.getDestination(), receivedPacket2.getDestination());
            Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket2.getModuleOfPacket());
        }

        [Fact]
        public void MultiplePacketsBroadcastTest()
        {
            for (var i = 1; i <= 10; i++)
            {
                Packet sendPacket = new("Test string" + i, null, "Test Module");
                _sendQueue.Enqueue(sendPacket);
            }

            while (_receiveQueue1.Size() != 10 || _receiveQueue2.Size() != 10)
            {
                Thread.Sleep(100);
            }
            Assert.True(_receiveQueue1.Size() == 10 && _receiveQueue2.Size() == 10);

            for (var i = 1; i <= 10; i++)
            {
                Packet receivedPacket1 = _receiveQueue1.Dequeue();
                Assert.Equal("Test string" + i, receivedPacket1.getSerializedData());
                Assert.Null(receivedPacket1.getDestination());
                Assert.Equal("Test Module", receivedPacket1.getModuleOfPacket());

                Packet receivedPacket2 = _receiveQueue2.Dequeue();
                Assert.Equal("Test string" + i, receivedPacket2.getSerializedData());
                Assert.Null(receivedPacket2.getDestination());
                Assert.Equal("Test Module", receivedPacket2.getModuleOfPacket());
            }
        }

        [Fact]
		public void ClientGotDisconnectedTest()
		{
			_socketListener1.Stop();
			while (_socketListener1._thread.IsAlive)
            {
				Thread.Sleep(100);
            }
			_clientSocket1.Close();
			_clientSocket1.Dispose();
            Packet sendPacket = new("Test string", null, "Test Module");
			_sendQueue.Enqueue(sendPacket);
			var whiteBoardHandler = (FakeNotificationHandler) _subscribedModules["Test Module"];
			whiteBoardHandler.Wait();
			Assert.Equal(NotificationEvents.OnClientLeft, whiteBoardHandler.Event);
		}
	}
}
