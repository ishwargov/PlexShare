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
        private readonly int _smallPacketSize = 10;
        private readonly int _largePacketSize = 1000;
        private readonly int _veryLargePacketSize = 1000000;
        private readonly string _destination = "Test Destination";
        private readonly string _module = "Test Module";

        private void PacketsReceiveTest(int size, int count)
        {
            // start the server and start listening to client connect requests
            CommunicatorServer communicatorServer = new();
            string[] ipAddressAndPort = communicatorServer.Start().Split(":");
            IPAddress ipAddress = IPAddress.Parse(ipAddressAndPort[0]);
            int port = int.Parse(ipAddressAndPort[1]);
            TcpListener clientConnectRequestListener = new(ipAddress, port);
            clientConnectRequestListener.Start();

            // connect the client to the server
            TcpClient serverSocket = new();
            TcpClient clientSocket = new();
            clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            Task t1 = Task.Run(() => { clientSocket.Connect(ipAddress, port); });
            Task t2 = Task.Run(() => { serverSocket = clientConnectRequestListener.AcceptTcpClient(); });
            Task.WaitAll(t1, t2);

            // start the socket listener
            ReceivingQueue receivingQueue = new();
            SocketListener socketListener = new(receivingQueue, serverSocket);
            socketListener.Start();

            // send packets and check they are received
            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(size, _destination, _module, count);
            NetworkTestGlobals.SendPackets(sendPackets, clientSocket, count);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, receivingQueue, count);
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
