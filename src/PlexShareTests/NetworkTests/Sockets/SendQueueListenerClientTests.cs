/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using System.Net;
using System.Net.Sockets;

namespace PlexShareNetwork.Sockets.Tests
{
	public class SendQueueListenerClientTests
	{
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 10;
        private readonly int _largePacketSize = 1000;
        private readonly int _veryLargePacketSize = 1000000;
        private readonly string _destination = "Test Destination";
        private readonly string _module = "Test Module";

        private void PacketsSendTest(int size, int count)
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

            // start send queue listener on client
            SendingQueue sendingQueue = new();
            sendingQueue.RegisterModule(_module, true);
            SendQueueListenerClient sendQueueListenerClient = new(sendingQueue, clientSocket);
			sendQueueListenerClient.Start();

            // start socket listener on server
            ReceivingQueue receivingQueue = new();
            SocketListener socketListener = new(receivingQueue, serverSocket);
			socketListener.Start();

            // send packets and check they are received
            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(size, _destination, _module, count);
            NetworkTestGlobals.EnqueuePackets(sendPackets, sendingQueue);
            NetworkTestGlobals.PacketsReceiveAssert(sendPackets, receivingQueue, count);
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
