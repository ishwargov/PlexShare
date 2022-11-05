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
			_server.Communicator.Stop();
			IPAddress IP = IPAddress.Parse(IPAndPort[0]);
			int port = int.Parse(IPAndPort[1]);
            TcpListener serverSocket = new(IP, port);
			serverSocket.Start();
            _clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
			Task t1 = Task.Run(() => { _clientSocket.Connect(IP, port); });
            Task t2 = Task.Run(() => { _serverSocket = serverSocket.AcceptTcpClient(); });
			Task.WaitAll(t1, t2);
			_socketListener = new(_receivingQueue, _serverSocket);
			_socketListener.Start();
        }

		[Fact]
		public void SinglePacketReceiveTest()
		{
			Packet sendPacket = new("Test string", "Test Destination", "Test Module");
            NetworkingGlobals.SendPacket(sendPacket, _clientSocket);
            NetworkingGlobals.AssertSinglePacketReceive(sendPacket, _receivingQueue);
		}

		[Fact]
		public void LargePacketReceiveTest()
		{
			Packet sendPacket = new(NetworkingGlobals.RandomString(1000), "Test Destination", "Test Module");
            NetworkingGlobals.SendPacket(sendPacket, _clientSocket);
            NetworkingGlobals.AssertSinglePacketReceive(sendPacket, _receivingQueue);
        }

		[Fact]
		public void MultiplePacketsFromSameModuleReceiveTest()
		{
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
			{
                sendPackets[i] = new("Test string" + i, "Test Destination", "Test Module");
                NetworkingGlobals.SendPacket(sendPackets[i], _clientSocket);
            }
            NetworkingGlobals.AssertTenPacketsReceive(sendPackets, _receivingQueue);
        }

        [Fact]
        public void MultiplePacketsFromDifferentModulesReceiveTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new("Test string" + i, "Test Destination", "Test Module" + i);
                NetworkingGlobals.SendPacket(sendPackets[i], _clientSocket);
            }
            NetworkingGlobals.AssertTenPacketsReceive(sendPackets, _receivingQueue);
        }

        [Fact]
        public void MultipleLargePacketsReceiveTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new(NetworkingGlobals.RandomString(1000), "Test Destination", "Test Module");
                NetworkingGlobals.SendPacket(sendPackets[i], _clientSocket);
            }
            NetworkingGlobals.AssertTenPacketsReceive(sendPackets, _receivingQueue);
        }
    }
}
