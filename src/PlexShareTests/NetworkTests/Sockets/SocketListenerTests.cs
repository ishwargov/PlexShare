/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using Xunit;

namespace PlexShareNetwork.Sockets.Tests
{
	public class SocketListenerTest
	{
		private readonly ReceivingQueue _receivingQueue = new();
		private readonly Machine _server = new FakeServer();
        private readonly TcpClient _clientSocket = new();
        private TcpClient _serverSocket;
		private readonly SocketListener _socketListener;
        private readonly Serializer _serializer = new();

        public SocketListenerTest()
		{
			var IPAndPort = _server.Communicator.Start().Split(":");
			IPAddress IP = IPAddress.Parse(IPAndPort[0]);
			int port = int.Parse(IPAndPort[1]);
			_server.Communicator.Stop();
            _clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            TcpListener serverSocket = new TcpListener(IP, port);
			serverSocket.Start();
			Task t1 = Task.Run(() => { _clientSocket.Connect(IP, port); });
            Task t2 = Task.Run(() => { _serverSocket = serverSocket.AcceptTcpClient(); });
			Task.WaitAll(t1, t2);
			_socketListener = new SocketListener(_receivingQueue, _serverSocket);
			_socketListener.Start();
        }

		[Fact]
		public void SinglePacketReceiveTest()
		{
			Packet sendPacket = new Packet("Test string", "Test Destination", "Test Module");
            string sendString = "BEGIN" + _serializer.Serialize(sendPacket) + "END";
            _clientSocket.Client.Send(Encoding.ASCII.GetBytes(sendString));

            while (_receivingQueue.IsEmpty())
			{
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue.Size() == 1);

            Packet receivedPacket = _receivingQueue.Dequeue();
			Assert.Equal(sendPacket.serializedData, receivedPacket.serializedData);
            Assert.Equal(sendPacket.destination, receivedPacket.destination);
            Assert.Equal(sendPacket.moduleOfPacket, receivedPacket.moduleOfPacket);
		}

		[Fact]
		public void LargeSizePacketReceiveTest()
		{
			Packet sendPacket = new Packet(NetworkingGlobals.RandomString(5000), "Test Destination", "Test Module");
            string sendString = "BEGIN" + _serializer.Serialize(sendPacket) + "END";
            _clientSocket.Client.Send(Encoding.ASCII.GetBytes(sendString));

			while (_receivingQueue.IsEmpty())
			{
                Thread.Sleep(100);
            }
            Assert.True(_receivingQueue.Size() == 1);

            Packet receivedPacket = _receivingQueue.Dequeue();
			Assert.Equal(sendPacket.serializedData, receivedPacket.serializedData);
            Assert.Equal(sendPacket.destination, receivedPacket.destination);
            Assert.Equal(sendPacket.moduleOfPacket, receivedPacket.moduleOfPacket);
		}

		[Fact]
		public void MultiplePacketReceiveTest()
		{
			for (var i = 1; i <= 10; i++)
			{
				Packet sendPacket = new Packet("Test string" + i, "Test Destination" + i, "Test Module" + i);
                string sendString = "BEGIN" + _serializer.Serialize(sendPacket) + "END";
                _clientSocket.Client.Send(Encoding.ASCII.GetBytes(sendString));
			}

			while (_receivingQueue.Size() < 10)
			{
				Thread.Sleep(1000);
			}
            Assert.True(_receivingQueue.Size() == 10);

            for (var i = 1; i <= 10; i++)
			{
				Packet receivedPacket = _receivingQueue.Dequeue();
				Assert.Equal("Test string" + i, receivedPacket.serializedData);
                Assert.Equal("Test Destination" + i, receivedPacket.destination);
                Assert.Equal("Test Module" + i, receivedPacket.moduleOfPacket);
            }
		}
	}
}
