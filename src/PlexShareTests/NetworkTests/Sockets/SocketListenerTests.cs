/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using PlexShareNetwork.Sockets;
using System.Net;
using System.Net.Sockets;

namespace PlexShareTests.NetworkTests.Sockets
{
	public class SocketListenerTests
	{
        // variables to be used in tests
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 10;
        private readonly int _largePacketSize = 1000;
        private readonly int _veryLargePacketSize = 1000000;
        private readonly string _destination = "Test Destination";
        private readonly string _module = "Test Module";

        /// <summary>
        /// Sends the packets through socket and tests that they are
        /// being received by the socket listener.
        /// </summary>
        /// <param name="size"> Size of the packet to test. </param>
        /// <param name="count"> Count of packets to test. </param>
        /// <returns> void </returns>
        private void PacketsReceiveTest(int size, int count)
        {
            // get IP address by starting the communicator server
            CommunicatorServer communicatorServer = new();
            string[] ipAndPort = communicatorServer.Start().Split(":");
            communicatorServer.Stop();

            // start tcp listener on this ip address
            IPAddress ip = IPAddress.Parse(ipAndPort[0]);
            int port = int.Parse(ipAndPort[1]);
            TcpListener clientConnectRequestListener = new(ip, port);
            clientConnectRequestListener.Start();

            // connect the client and server sockets
            TcpClient serverSocket = new();
            TcpClient clientSocket = new();
            clientSocket.Client.SetSocketOption(
                SocketOptionLevel.Socket, 
                SocketOptionName.DontLinger, true);
            Task t1 = Task.Run(() => { 
                clientSocket.Connect(ip, port); });
            Task t2 = Task.Run(() => { serverSocket = 
                clientConnectRequestListener.AcceptTcpClient(); });
            Task.WaitAll(t1, t2);

            // start the SocketListener
            ReceivingQueue receivingQueue = new();
            SocketListener socketListener = 
                new(receivingQueue, serverSocket);
            socketListener.Start();

            // send packets and check they are received
            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(
                size, _destination, _module, count);
            NetworkTestGlobals.SendPackets(sendPackets, clientSocket);
            NetworkTestGlobals.PacketsReceiveAssert(
                sendPackets, receivingQueue, count);
            socketListener.Stop();
        }

        /// <summary>
        /// Tests a small packet is received by SocketListener
        /// </summary>
        /// <returns> void </returns>
		[Fact]
		public void SmallPacketReceiveTest()
		{
            PacketsReceiveTest(_smallPacketSize, 1);
		}

        /// <summary>
        /// Tests a large packet is received by SocketListener
        /// </summary>
        /// <returns> void </returns>

        [Fact]
        public void LargePacketReceiveTest()
        {
            PacketsReceiveTest(_largePacketSize, 1);
        }

        /// <summary>
        /// Tests a very large packet is received by SocketListener
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void VeryLargePacketReceiveTest()
        {
            PacketsReceiveTest(_veryLargePacketSize, 1);
        }

        /// <summary>
        /// Tests multiple small packets are received by SocketListener
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void MultipleSmallPacketsReceiveTest()
        {
            PacketsReceiveTest(
                _smallPacketSize, _multiplePacketsCount);
        }

        /// <summary>
        /// Tests multiple large packets are received by SocketListener
        /// listener.
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void MultipleLargePacketsReceiveTest()
        {
            PacketsReceiveTest(
                _largePacketSize, _multiplePacketsCount);
        }

        /// <summary>
        /// Tests error catch when starting socket listener
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SocketListenerStartErrorCatchTest()
        {
            // start socket listener without connecting socket
            // so error must be thrown and catch
            TcpClient serverSocket = null;
            ReceivingQueue receivingQueue = new();
            SocketListener socketListener =
                new(receivingQueue, serverSocket);
            socketListener.Start();
        }
    }
}
