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
using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using Xunit;

namespace PlexShareNetwork.Sockets.Tests
{
	public class SocketListenerTests
	{
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 100;
        private readonly int _largePacketSize = 10000;
        private readonly int _veryLargePacketSize = 10000000; // adding one more 0 to it will hang you laptop
        private readonly string _destination = "Test Destination";
        private readonly string _module = "Test Module";

        private void PacketsReceiveTest(int size, int count)
        {
            ReceivingQueue _receivingQueue = new();
            TcpClient _clientSocket = new();
            TcpClient _serverSocket = new();
            SocketListener _socketListener;
            CommunicatorServer _communicatorServer = new();

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
            _socketListener = new(_receivingQueue, _serverSocket);
            _socketListener.Start();

            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(size, _destination, _module, count);
            NetworkTestGlobals.SendPackets(sendPackets, _clientSocket, count);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue, count);
        }

		[Fact]
		public void SmallPacketReceiveTest()
		{
            PacketsReceiveTest(_smallPacketSize, 1);
		}

        [Fact]
        public void LargePacketReceiveTest()
        {
            PacketsReceiveTest(_largePacketSize, 1);
        }

        [Fact]
        public void VeryLargePacketReceiveTest()
        {
            PacketsReceiveTest(_veryLargePacketSize, 1);
        }

        [Fact]
        public void MultipleSmallPacketsReceiveTest()
        {
            PacketsReceiveTest(_smallPacketSize, _multiplePacketsCount);
        }

        [Fact]
        public void MultipleLargePacketsReceiveTest()
        {
            PacketsReceiveTest(_largePacketSize, _multiplePacketsCount);
        }
    }
}
