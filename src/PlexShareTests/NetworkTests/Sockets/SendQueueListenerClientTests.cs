/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains unit tests for SendQueueListenerClient
/// </summary>

using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using PlexShareNetwork.Sockets;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PlexShareTests.NetworkTests.Sockets
{
	public class SendQueueListenerClientTests
	{
        // variables to be used in tests
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 10;
        private readonly int _largePacketSize = 1000;
        private readonly int _veryLargePacketSize = 1000000;
        private readonly string _destination = "Test Destination";
        private readonly string _module = "Test Module";

        /// <summary>
        /// Enqueues packets into client's sending queue and tests that
        /// they are being received on the server's receiving queue.
        /// </summary>
        /// <param name="size"> Size of the packet to test. </param>
        /// <param name="count"> Count of packets to test. </param>
        /// <returns> void </returns>
        private void PacketsSendTest(int size, int count)
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

            // start the SendQueueListenerClient to send data
            // from the client's sending queue to the server
            SendingQueue sendingQueue = new();
            sendingQueue.RegisterModule(_module, true);
            SendQueueListenerClient sendQueueListenerClient = 
                new(sendingQueue, clientSocket);
			sendQueueListenerClient.Start();

            // start SocketListener to receive data on server
            ReceivingQueue receivingQueue = new();
            SocketListener socketListener = 
                new(receivingQueue, serverSocket);
			socketListener.Start();

            // send packets and check they are received
            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(
                size, _destination, _module, count);
            NetworkTestGlobals.EnqueuePackets(
                sendPackets, sendingQueue);
            NetworkTestGlobals.PacketsReceiveAssert(
                sendPackets, receivingQueue, count);
            sendQueueListenerClient.Stop();
        }

        /// <summary>
        /// Tests a small packet is sent by SendQueueListenerClient
        /// </summary>
        /// <returns> void </returns>
        [Fact]
		public void SmallPacketSendTest()
		{
            PacketsSendTest(_smallPacketSize, 1);
        }

        /// <summary>
        /// Tests a large packet is sent by SendQueueListenerClient
        /// </summary>
        /// <returns> void </returns>
        [Fact]
		public void LargePacketSendTest()
		{
            PacketsSendTest(_largePacketSize, 1);
        }

        /// <summary>
        /// Tests a very large packet is sent by
        /// SendQueueListenerClient
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void VeryLargePacketSendTest()
        {
            PacketsSendTest(_veryLargePacketSize, 1);
        }

        /// <summary>
        /// Tests a multiple small packets are sent by
        /// SendQueueListenerClient
        /// </summary>
        /// <returns> void </returns>
        [Fact]
		public void MultipleSmallPacketsSendTest()
		{
            PacketsSendTest(_smallPacketSize, _multiplePacketsCount);
        }

        /// <summary>
        /// Tests a multiple large packets are sent by
        /// SendQueueListenerClient
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void MultipleLargePacketsSendTest()
        {
            PacketsSendTest(_largePacketSize, _multiplePacketsCount);
        }

        /// <summary>
        /// Tests error catch in SendQueueListenerClient
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SendQueueListenerClientErrorCatchTest()
        {
            // start SendQueueListenerClient with null
            // socket so error must be thrown and catch
            TcpClient clientSocket = null;
            SendingQueue sendingQueue = new();
            sendingQueue.RegisterModule(_module, true);
            SendQueueListenerClient sendQueueListenerClient =
                new(sendingQueue, clientSocket);
            sendQueueListenerClient.Start();
            Packet packet = new("Data", _destination, _module);
            sendingQueue.Enqueue(packet);
            while (sendingQueue.Size() != 0)
            {
                Thread.Sleep(100);
            }
            sendQueueListenerClient.Stop();
        }
    }
}
