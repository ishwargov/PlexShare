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
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 100;
        private readonly int _largePacketSize = 10000;
        private readonly int _veryLargePacketSize = 10000000; // adding one more 0 to it will hang you laptop
        private readonly string _destination = "Test Destination";
        private readonly string _module = "Test Module";

        private void PacketsSendTest(int size, int count)
        {
            SendingQueue _sendingQueue = new();
            ReceivingQueue _receivingQueue = new();
            CommunicatorServer _communicatorServer = new();
            SendQueueListenerClient _sendQueueListenerClient;
            SocketListener _socketListener;
            TcpClient _serverSocket = new();
            TcpClient _clientSocket = new();

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
            _sendQueueListenerClient = new (_sendingQueue, _clientSocket);
			_sendQueueListenerClient.Start();
			_socketListener = new (_receivingQueue, _serverSocket);
			_socketListener.Start();

            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(size, _destination, _module, count);
            NetworkTestGlobals.SendPackets(sendPackets, _sendingQueue, count);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue, count);
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
        public void VeryLargePacketSendTest()
        {
            PacketsSendTest(_veryLargePacketSize, 1);
        }

        [Fact]
		public void MultipleSmallPacketsSendTest()
		{
            PacketsSendTest(_smallPacketSize, _multiplePacketsCount);
        }

        [Fact]
        public void MultipleLargePacketsSendTest()
        {
            PacketsSendTest(_largePacketSize, _multiplePacketsCount);
        }
    }
}
