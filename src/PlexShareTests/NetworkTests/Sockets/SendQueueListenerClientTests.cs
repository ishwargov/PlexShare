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
		private readonly SendingQueues _sendQueue = new();
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
			_sendQueue.RegisterModule("Test Module", true);
			_sendQueueListenerClient = new SendQueueListenerClient(_sendQueue, _clientSocket);
			_sendQueueListenerClient.Start();
			_socketListener = new SocketListener(_receivingQueue, _serverSocket);
			_socketListener.Start();
		}

		[Fact]
		public void SinglePacketSendTest()
		{
            Packet sendPacket = new("Test string", "To Server", "Test Module");
			_sendQueue.Enqueue(sendPacket);

			while (_receivingQueue.IsEmpty())
			{
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue.Size() == 1);

            Packet receivedPacket = _receivingQueue.Dequeue();
			Assert.Equal(sendPacket.getSerializedData(), receivedPacket.getSerializedData());
			Assert.Equal(sendPacket.getDestination(), receivedPacket.getDestination());
			Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket.getModuleOfPacket());
		}

		[Fact]
		public void LargeSizePacketSendTest()
		{
            Packet sendPacket = new(NetworkingGlobals.RandomString(5000), "To Server", "Test Module");
			_sendQueue.Enqueue(sendPacket);

			while (_receivingQueue.IsEmpty())
			{
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue.Size() == 1);

            Packet receivedPacket = _receivingQueue.Dequeue();
			Assert.Equal(sendPacket.getSerializedData(), receivedPacket.getSerializedData());
            Assert.Equal(sendPacket.getDestination(), receivedPacket.getDestination());
            Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket.getModuleOfPacket());
		}

		[Fact]
		public void MultiplePacketSendTest()
		{
			for (var i = 1; i <= 10; i++)
			{
                Packet sendPacket = new("Test string" + i, "To Server", "Test Module");
				_sendQueue.Enqueue(sendPacket);
			}

			while (_receivingQueue.Size() != 10)
			{
				Thread.Sleep(100);
			}
            Assert.True(_receivingQueue.Size() == 10);

            for (var i = 1; i <= 10; i++)
			{
                Packet receivedPacket = _receivingQueue.Dequeue();
				Assert.Equal("Test string" + i, receivedPacket.getSerializedData());
                Assert.Equal("To Server", receivedPacket.getDestination());
                Assert.Equal("Test Module", receivedPacket.getModuleOfPacket());
            }
		}
	}
}
