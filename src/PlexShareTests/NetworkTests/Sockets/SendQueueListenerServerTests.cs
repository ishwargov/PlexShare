/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains unit tests for SendQueueListenerServer
/// </summary>

using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using System.Net;
using System.Net.Sockets;

namespace PlexShareNetwork.Sockets.Tests
{
	public class SendQueueListenerServerTests
	{
        // variables to be used in tests
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 10;
        private readonly int _largePacketSize = 1000;
        private readonly int _veryLargePacketSize = 1000000;
        private readonly string[] _clientIds = { "Client Id1", "Client Id2" };
        private readonly string _module = "Test Module";
        bool _clientGotDisconnectedTest = false;

        /// <summary>
        /// Enqueues packets into server's sending queue and tests that
        /// they are being received on the client's receiving queue.
        /// </summary>
        /// <param name="size"> Size of the packet to test. </param>
        /// <param name="destination">
        /// Client Id to send to a particular client. Or null to broadcast.
        /// </param>
        /// <param name="count"> Count of packets to test. </param>
        /// <returns> void </returns>
        private void PacketsSendTest(int size, string? destination, int count)
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

            Dictionary<string, TcpClient> clientIdToSocket = new();
            List<SocketListener> socketListeners = new();
            List<TcpClient> clientSockets = new();
            List<ReceivingQueue> clientReceivingQueues = new();

            foreach (string clientId in _clientIds)
            {
                // connect the client and server sockets
                TcpClient serverSocket = new();
                TcpClient clientSocket = new();
                clientSocket.Client.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.DontLinger, true);
                Task t1 = Task.Run(() => {
                    clientSocket.Connect(ip, port);
                });
                Task t2 = Task.Run(() => {
                    serverSocket =
                    clientConnectRequestListener.AcceptTcpClient();
                });
                Task.WaitAll(t1, t2);
                
                clientIdToSocket.Add(clientId, serverSocket);
                clientSockets.Add(clientSocket);
                ReceivingQueue receivingQueue = new();
                clientReceivingQueues.Add(receivingQueue);
                SocketListener socketListener = new(receivingQueue, clientSocket);
                socketListener.Start();
                socketListeners.Add(socketListener);
            }

            // start send queue listener on server
            SendingQueue sendingQueue = new();
            sendingQueue.RegisterModule("Test Module", true);
            Dictionary<string, INotificationHandler> subscribedModules = new()
            {
                ["Test Module"] = new TestNotificationHandler()
            };
            SendQueueListenerServer sendQueueListenerServer = new(sendingQueue, clientIdToSocket, subscribedModules);
            sendQueueListenerServer.Start();

            // if its client got disconnected test then disconnect client 1
            if (_clientGotDisconnectedTest)
            {
                var clientSocket = clientSockets[0];
                clientSocket.GetStream().Close();
                clientSocket.Close();
            }

            // send packets
            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(size, destination, _module, count);
            NetworkTestGlobals.EnqueuePackets(sendPackets, sendingQueue);

            // if its client got disconnected test then check whether modules are notified
            if (_clientGotDisconnectedTest)
            {
                TestNotificationHandler testNotificationHandler = (TestNotificationHandler)subscribedModules["Test Module"];
                testNotificationHandler.WaitForEvent();
                // assert module is notified, client1 did not receive, and client2 received
                Assert.Equal("OnClientLeft", testNotificationHandler.GetLastEvent());
                Assert.Equal("Client Id1", testNotificationHandler.GetLastEventClientId());
                Assert.True(clientReceivingQueues[0].IsEmpty());
            }
            else // check client 1 received the packets
            {
                NetworkTestGlobals.PacketsReceiveAssert(sendPackets, clientReceivingQueues[0], count);
            }
            // if it was a broadcast test then check client2 also received the packets
            if (destination == null)
            {
                NetworkTestGlobals.PacketsReceiveAssert(sendPackets, clientReceivingQueues[1], count);
            }
        }

        [Fact]
		public void SmallPacketUnicastTest()
		{
            PacketsSendTest(_smallPacketSize, _clientIds[0], 1);
        }

		[Fact]
		public void LargePacketUnicastTest()
		{
            PacketsSendTest(_largePacketSize, _clientIds[0], 1);
        }

        [Fact]
        public void VeryLargePacketUnicastTest()
        {
            PacketsSendTest(_veryLargePacketSize, _clientIds[0], 1);
        }

        [Fact]
        public void MultipleSmallPacketsUnicastTest()
        {
            PacketsSendTest(_smallPacketSize, _clientIds[0], _multiplePacketsCount);
        }

        [Fact]
        public void MultipleLargePacketsUnicastTest()
        {
            PacketsSendTest(_largePacketSize, _clientIds[0], _multiplePacketsCount);
        }

        [Fact]
        public void SmallPacketBroadcastTest()
        {
            PacketsSendTest(_smallPacketSize, null, 1);
        }

        [Fact]
        public void LargePacketBroadcastTest()
        {
            PacketsSendTest(_largePacketSize, null, 1);
        }

        [Fact]
        public void VeryLargePacketBroadcastTest()
        {
            PacketsSendTest(_veryLargePacketSize, null, 1);
        }

        [Fact]
        public void MultipleSmallPacketsBroadcastTest()
        {
            PacketsSendTest(_smallPacketSize, null, _multiplePacketsCount);
        }

        [Fact]
        public void MultipleLargePacketsBroadcastTest()
        {
            PacketsSendTest(_largePacketSize, null, _multiplePacketsCount);
        }

        [Fact]
		public void ClientGotDisconnectedTest()
		{
            _clientGotDisconnectedTest = true;
            SmallPacketBroadcastTest();
        }
    }
}
