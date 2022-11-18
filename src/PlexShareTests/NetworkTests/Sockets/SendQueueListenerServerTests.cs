/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains unit tests for SendQueueListenerServer
/// </summary>

using PlexShareNetwork;
using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using PlexShareNetwork.Sockets;
using System.Net;
using System.Net.Sockets;

namespace PlexShareTests.NetworkTests.Sockets
{
	public class SendQueueListenerServerTests
	{
        // variables to be used in tests
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 10;
        private readonly int _largePacketSize = 1000;
        private readonly int _veryLargePacketSize = 1000000;
        private readonly int _multipleClientsCount = 10;
        private readonly string clientId = "Client Id";
        private readonly string _module = "Test Module";

        /// <summary>
        /// Enqueues packets into server's sending queue and tests that
        /// they are being received on the client's receiving queue.
        /// </summary>
        /// <param name="size"> Size of the packet to test. </param>
        /// <param name="count"> Count of packets to test. </param>
        /// <param name="doBroadcast">
        /// Boolean to tell whether to do unicast or broadcast.
        /// </param>
        /// <returns> void </returns>
        private void PacketsSendTest(int size, int count,
            int numClients, bool doBroadcast, bool disconnectClient)
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

            // map that will store sockets connected to clients
            Dictionary<string, TcpClient> clientIdToSocket = new();
            // to store socket listeners of clients
            List<SocketListener> socketListeners = new();
            // to store sockets of clients
            List<TcpClient> clientSockets = new();
            // to store receiving queues of clients
            List<ReceivingQueue> clientReceivingQueues = new();

            // first generate client Ids
            string[] _clientIds = new string[numClients];
            for (var i = 0; i < numClients; i++)
            {
                _clientIds[i] = clientId + i;
            }

            // iterate through client Ids and connect each client
            // to server
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
                
                // start the socket listener on client
                ReceivingQueue receivingQueue = new();
                SocketListener socketListener = 
                    new(receivingQueue, clientSocket);
                socketListener.Start();

                // add everything into its respective collections
                clientIdToSocket.Add(clientId, serverSocket);
                socketListeners.Add(socketListener);
                clientSockets.Add(clientSocket);
                clientReceivingQueues.Add(receivingQueue);
            }

            // start SendQueueListenerServer to send data
            // from the sernver's sending queue to client(s)
            SendingQueue sendingQueue = new();
            sendingQueue.RegisterModule(_module, true);
            TestNotificationHandler testNotificationHandler = new();
            Dictionary<string, INotificationHandler> subscribedModules
                = new() { [_module] = testNotificationHandler };
            SendQueueListenerServer sendQueueListenerServer = new(
                sendingQueue, clientIdToSocket, subscribedModules);
            sendQueueListenerServer.Start();

            // if disconnectClient is true then we disconnect client 0
            if (disconnectClient)
            {
                TcpClient clientSocket = clientSockets[0];
                clientSocket.GetStream().Close();
                clientSocket.Close();
            }

            // send packets, for broadcast destination has to be null
            Packet[] sendPackets = new Packet[count];
            if (doBroadcast)
            {
                sendPackets = NetworkTestGlobals.GeneratePackets(
                    size, null, _module, count);
            }
            else
            {
                sendPackets = NetworkTestGlobals.GeneratePackets(
                    size, _clientIds[0], _module, count);
            }
            NetworkTestGlobals.EnqueuePackets(
                sendPackets, sendingQueue);

            // if client 0 was not disconnected then check it
            // received the packets
            if (!disconnectClient)
            {
                NetworkTestGlobals.PacketsReceiveAssert(
                   sendPackets, clientReceivingQueues[0], count);
            }
            else
            {
                // client 0 was disconnected so check whether the
                // subscribed module is notified on the server that
                // the client has left
                testNotificationHandler.WaitForEvent();
                Assert.Equal("OnClientLeft",
                    testNotificationHandler.GetLastEvent());
                Assert.Equal(_clientIds[0],
                    testNotificationHandler.GetLastEventClientId());
                Assert.True(clientReceivingQueues[0].IsEmpty());
            }

            // if it was packets were broadcasted then check all
            // other clients received the packets
            if (doBroadcast)
            {
                for (var i = 1; i < numClients && doBroadcast; i++)
                {
                    NetworkTestGlobals.PacketsReceiveAssert(
                        sendPackets, clientReceivingQueues[i], count);
                }
            }
        }

        /// <summary>
        /// Tests a small packet is unicasted by
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
		public void SmallPacketUnicastTest()
		{
            PacketsSendTest(_smallPacketSize, 1,
                _multipleClientsCount, false, false);
        }

        /// <summary>
        /// Tests a large packet is unicasted by 
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
		public void LargePacketUnicastTest()
		{
            PacketsSendTest(_largePacketSize, 1, 
                _multipleClientsCount, false, false);
        }

        /// <summary>
        /// Tests a very large packet is unicasted by
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void VeryLargePacketUnicastTest()
        {
            PacketsSendTest(_veryLargePacketSize, 1, 
                _multipleClientsCount, false, false);
        }

        /// <summary>
        /// Tests multiple small packets are unicasted by 
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void MultipleSmallPacketsUnicastTest()
        {
            PacketsSendTest(_smallPacketSize, _multiplePacketsCount,
                _multipleClientsCount, false, false);
        }

        /// <summary>
        /// Tests multiple large packets are unicasted by 
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void MultipleLargePacketsUnicastTest()
        {
            PacketsSendTest(_largePacketSize, _multiplePacketsCount,
                _multipleClientsCount, false, false);
        }

        /// <summary>
        /// Tests a small packet is broadcasted by
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SmallPacketBroadcastTest()
        {
            PacketsSendTest(_smallPacketSize, 1,
                _multipleClientsCount, true, false);
        }

        /// <summary>
        /// Tests a large packet is broadcasted by 
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void LargePacketBroadcastTest()
        {
            PacketsSendTest(_largePacketSize, 1,
                _multipleClientsCount, true, false);
        }

        /// <summary>
        /// Tests a very large packet is broadcasted by 
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void VeryLargePacketBroadcastTest()
        {
            PacketsSendTest(_veryLargePacketSize, 1,
                _multipleClientsCount, true, false);
        }

        /// <summary>
        /// Tests multiple small packets are broadcasted by 
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void MultipleSmallPacketsBroadcastTest()
        {
            PacketsSendTest(_smallPacketSize, _multiplePacketsCount,
                _multipleClientsCount, true, false);
        }

        /// <summary>
        /// Tests multiple large packets are broadcasted by 
        /// SendQueueListenerServer
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void MultipleLargePacketsBroadcastTest()
        {
            PacketsSendTest(_largePacketSize, _multiplePacketsCount,
                _multipleClientsCount, true, false);
        }

        /// <summary>
        /// Tests when a client gets disconnected then the subscribed
        /// modules are notified on the server and all other clients
        /// still receive the data
        /// </summary>
        /// <returns> void </returns>
        [Fact]
		public void ClientGotDisconnectedTest()
		{
            PacketsSendTest(_smallPacketSize, _multiplePacketsCount,
                _multipleClientsCount, false, false);
        }
    }
}
