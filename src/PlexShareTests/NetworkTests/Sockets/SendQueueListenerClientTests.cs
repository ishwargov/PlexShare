/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PlexShareNetwork.Queues;
using Xunit;

namespace PlexShareNetwork.Sockets.Tests
{
	public class SendQueueListenerClientTest
	{
		private readonly SendingQueue _sendingQueue = new();
		private readonly ReceivingQueue _receivingQueue = new();
		private readonly Machine _server = new FakeServer();
		private readonly SendQueueListenerClient _sendQueueListenerClient;
		private readonly SocketListener _socketListener;
		private TcpClient _serverSocket;
		private readonly TcpClient _clientSocket = new();

		public SendQueueListenerClientTest()
		{
			var IPAndPort = _server.Communicator.Start().Split(":");
            _server.Communicator.Stop();
            IPAddress IP = IPAddress.Parse(IPAndPort[0]);
			int port = int.Parse(IPAndPort[1]);
            TcpListener serverSocket = new(IP, port);
			serverSocket.Start();
			_clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
			Task t1 = Task.Run(() => { _clientSocket.Connect(IP, port); });
            Task t2 = Task.Run(() => { _serverSocket = serverSocket.AcceptTcpClient(); });
			Task.WaitAll(t1, t2);
            _sendingQueue.RegisterModule("Test Module1", true);
            _sendingQueue.RegisterModule("Test Module2", true);
            _sendQueueListenerClient = new(_sendingQueue, _clientSocket);
			_sendQueueListenerClient.Start();
			_socketListener = new(_receivingQueue, _serverSocket);
			_socketListener.Start();
		}

		[Fact]
		public void SinglePacketSendTest()
		{
            Packet sendPacket = new("Test string", "To Server", "Test Module1");
            _sendingQueue.Enqueue(sendPacket);

			while (_receivingQueue.Size() < 1)
			{
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue.Size() == 1);

            Packet receivedPacket = _receivingQueue.Dequeue();
            NetworkingGlobals.AssertPacketEquality(sendPacket, receivedPacket);
		}

		[Fact]
		public void LargeSizePacketSendTest()
		{
            Packet sendPacket = new(NetworkingGlobals.RandomString(1000), "To Server", "Test Module1");
            _sendingQueue.Enqueue(sendPacket);

			while (_receivingQueue.Size() < 1)
			{
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue.Size() == 1);

            Packet receivedPacket = _receivingQueue.Dequeue();
            NetworkingGlobals.AssertPacketEquality(sendPacket, receivedPacket);
		}

		[Fact]
		public void MultiplePacketSendFromSameModuleTest()
		{
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
			{
                sendPackets[i] = new Packet("Test string" + i, "To Server", "Test Module1");
                _sendingQueue.Enqueue(sendPackets[i]);
			}

			while (_receivingQueue.Size() < 10)
			{
				Thread.Sleep(100);
			}
            Assert.True(_receivingQueue.Size() == 10);

            for (var i = 0; i < 10; i++)
			{
                Packet receivedPacket = _receivingQueue.Dequeue();
                NetworkingGlobals.AssertPacketEquality(sendPackets[i], receivedPacket);
            }
		}

        [Fact]
        public void MultiplePacketSendFromDifferentModulesTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new Packet("Test string" + i, "To Server", "Test Module" + (i%2+1));
                _sendingQueue.Enqueue(sendPackets[i]);
            }

            while (_receivingQueue.Size() < 10)
            {
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue.Size() == 10);

            for (var i = 0; i < 10; i++)
            {
                Packet receivedPacket = _receivingQueue.Dequeue();
                NetworkingGlobals.AssertPacketEquality(sendPackets[i], receivedPacket);
            }
        }

        [Fact]
        public void MultipleLargeSizePacketsSendTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new Packet(NetworkingGlobals.RandomString(1000), "To Server", "Test Module1");
                _sendingQueue.Enqueue(sendPackets[i]);
            }

            while (_receivingQueue.Size() < 10)
            {
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue.Size() == 10);

            for (var i = 0; i < 10; i++)
            {
                Packet receivedPacket = _receivingQueue.Dequeue();
                NetworkingGlobals.AssertPacketEquality(sendPackets[i], receivedPacket);
            }
        }

        [Fact]
        public void PacketSendFromUnregisteredModuleTest()
        {
            // packet from an unregistered module should not be sent because
            // SendingQueue only enqueues packets that are registered
            Packet sendPacket = new("Test string", "To Server", "Unregistered Module");
            _sendingQueue.Enqueue(sendPacket);
            Thread.Sleep(5000);
            Assert.True(_receivingQueue.Size() == 0);
        }
    }
}
