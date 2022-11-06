/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using Xunit;

namespace PlexShareNetwork.Sockets.Tests
{
	public class SendQueueListenerClientTests
	{
		private readonly SendingQueue _sendingQueue = new();
		private readonly ReceivingQueue _receivingQueue = new();
        private readonly ICommunicator _communicatorServer = CommunicationFactory.GetCommunicator(false);
		private readonly SendQueueListenerClient _sendQueueListenerClient;
		private readonly SocketListener _socketListener;
		private TcpClient _serverSocket;
		private readonly TcpClient _clientSocket = new();
        private readonly int _multiplePacktesCount = 10;
        private readonly int _smallPacketSize = 10;
        private readonly int _largePacketSize = 1000;
        private readonly string _destination = "Test Destination";
        private readonly string _module = "Test Module";

        public SendQueueListenerClientTests()
		{
			string[] IPAndPort = _communicatorServer.Start().Split(":");
            _communicatorServer.Stop();
            IPAddress IP = IPAddress.Parse(IPAndPort[0]);
			int port = int.Parse(IPAndPort[1]);
            TcpListener serverSocket = new(IP, port);
			serverSocket.Start();
			_clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
			Task t1 = Task.Run(() => { _clientSocket.Connect(IP, port); });
            Task t2 = Task.Run(() => { _serverSocket = serverSocket.AcceptTcpClient(); });
			Task.WaitAll(t1, t2);
            _sendingQueue.RegisterModule(_module, true);
            _sendQueueListenerClient = new(_sendingQueue, _clientSocket);
			_sendQueueListenerClient.Start();
			_socketListener = new(_receivingQueue, _serverSocket);
			_socketListener.Start();
		}

        private void PacketsSendTest(int size, int count)
        {
            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(size, _destination, _module, count);
            NetworkTestGlobals.SendAndReceiveAssert(sendPackets, _sendingQueue, _receivingQueue, count);
        }

        [Fact]
		public void SmallPacketSendTest()
		{
            PacketsSendTest(_smallPacketSize, 1);
        }

		[Fact]
		public void LargePacketSendTest()
		{
            PacketsSendTest(_largePacketSize, 1);
        }

        [Fact]
		public void MultipleSmallPacketsSendTest()
		{
            PacketsSendTest(_smallPacketSize, _multiplePacktesCount);
        }

        [Fact]
        public void MultipleLargePacketsSendTest()
        {
            PacketsSendTest(_largePacketSize, _multiplePacktesCount);
        }
    }
}
