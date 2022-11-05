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
using PlexShareNetwork.Queues;
using Xunit;

namespace PlexShareNetwork.Sockets.Tests
{
	public class SendQueueListenerServerTest
	{
		private readonly SendingQueue _sendingQueue = new();
        private readonly ReceivingQueue _receivingQueue1 = new();
        private readonly ReceivingQueue _receivingQueue2 = new();
		private readonly Machine _server = new FakeServer();
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

		public SendQueueListenerServerTest()
		{
			var IPAndPort = _server.Communicator.Start().Split(":");
            _server.Communicator.Stop();
            _IP = IPAddress.Parse(IPAndPort[0]);
			_port = int.Parse(IPAndPort[1]);
			_serverListener = new TcpListener(_IP, _port);
			_serverListener.Start();

            _sendingQueue.RegisterModule("Test Module", true);
            _subscribedModules["Test Module"] = new FakeNotificationHandler();
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
            Packet sendPacket = new("Test string", "Client1 ID", "Test Module");
            _sendingQueue.Enqueue(sendPacket);

			while (_receivingQueue1.Size() < 1)
			{
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue1.Size() == 1);

            Packet receivedPacket = _receivingQueue1.Dequeue();
            NetworkingGlobals.AssertPacketEquality(sendPacket, receivedPacket);
		}

		[Fact]
		public void LargeSizePacketUnicastTest()
		{
            Packet sendPacket = new(NetworkingGlobals.RandomString(1000), "Client1 ID", "Test Module");
            _sendingQueue.Enqueue(sendPacket);

			while (_receivingQueue1.Size() < 1)
			{
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue1.Size() == 1);

            Packet receivedPacket = _receivingQueue1.Dequeue();
            NetworkingGlobals.AssertPacketEquality(sendPacket, receivedPacket);
		}

        [Fact]
        public void MultiplePacketsUnicastTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new("Test string" + i, "Client1 ID", "Test Module");
                _sendingQueue.Enqueue(sendPackets[i]);
            }

            while (_receivingQueue1.Size() < 10)
            {
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue1.Size() == 10);

            for (var i = 0; i < 10; i++)
            {
                Packet receivedPacket = _receivingQueue1.Dequeue();
                NetworkingGlobals.AssertPacketEquality(sendPackets[i], receivedPacket);
            }
        }

        [Fact]
        public void SinglePacketBroadcastTest()
        {
            Packet sendPacket = new("Test string", null, "Test Module");
            _sendingQueue.Enqueue(sendPacket);

            while (_receivingQueue1.Size() < 1 || _receivingQueue2.Size() < 1)
            {
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue1.Size() == 1 && _receivingQueue2.Size() == 1);

            Packet receivedPacket1 = _receivingQueue1.Dequeue();
            NetworkingGlobals.AssertPacketEquality(sendPacket, receivedPacket1);

            Packet receivedPacket2 = _receivingQueue2.Dequeue();
            NetworkingGlobals.AssertPacketEquality(sendPacket, receivedPacket2);
        }

        [Fact]
        public void LargeSizePacketBroadcastTest()
        {
            Packet sendPacket = new(NetworkingGlobals.RandomString(1000), null, "Test Module");
            _sendingQueue.Enqueue(sendPacket);

            while (_receivingQueue1.Size() < 1 || _receivingQueue2.Size() < 1)
            {
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue1.Size() == 1 && _receivingQueue2.Size() == 1);

            Packet receivedPacket1 = _receivingQueue1.Dequeue();
            NetworkingGlobals.AssertPacketEquality(sendPacket, receivedPacket1);

            Packet receivedPacket2 = _receivingQueue2.Dequeue();
            NetworkingGlobals.AssertPacketEquality(sendPacket, receivedPacket2);
        }

        [Fact]
        public void MultiplePacketsBroadcastTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new("Test string" + i, null, "Test Module");
                _sendingQueue.Enqueue(sendPackets[i]);
            }

            while (_receivingQueue1.Size() < 10 || _receivingQueue2.Size() < 10)
            {
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue1.Size() == 10 && _receivingQueue2.Size() == 10);

            for (var i = 0; i < 10; i++)
            {
                Packet receivedPacket1 = _receivingQueue1.Dequeue();
                NetworkingGlobals.AssertPacketEquality(sendPackets[i], receivedPacket1);

                Packet receivedPacket2 = _receivingQueue2.Dequeue();
                NetworkingGlobals.AssertPacketEquality(sendPackets[i], receivedPacket2);
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
            _sendingQueue.Enqueue(sendPacket);
			var whiteBoardHandler = (FakeNotificationHandler) _subscribedModules["Test Module"];
			whiteBoardHandler.Wait();
			Assert.Equal(NotificationEvents.OnClientLeft, whiteBoardHandler.Event);
		}
	}
}
