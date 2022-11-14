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
	public class SendQueueListenerServerTests
	{
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 10;
        private readonly int _largePacketSize = 1000;
        private readonly int _veryLargePacketSize = 1000000;
        private readonly string[] _destinations = { "Client Id1", "Client Id2" };
        private readonly string _module = "Test Module";
        bool _clientGotDisconnectedTest = false;

        private void PacketsSendTest(int size, string? destination, int count)
        {
            // start the server and start listening to client connect requests
            CommunicatorServer communicatorServer = new();
            string[] ipAddressAndPort = communicatorServer.Start().Split(":");
            IPAddress ipAddress = IPAddress.Parse(ipAddressAndPort[0]);
            int port = int.Parse(ipAddressAndPort[1]);
            TcpListener clientConnectRequestListener = new(ipAddress, port);
            clientConnectRequestListener.Start();

            // connect the 2 clients to the server
            TcpClient serverSocket1 = new();
            TcpClient serverSocket2 = new();
            TcpClient clientSocket2 = new();
            TcpClient clientSocket1 = new();
            clientSocket1.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            clientSocket2.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            var t1 = Task.Run(() => { clientSocket1.Connect(ipAddress, port); });
            var t2 = Task.Run(() => { serverSocket1 = clientConnectRequestListener.AcceptTcpClient(); });
            Task.WaitAll(t1, t2);
            Task t3 = Task.Run(() => { clientSocket2.Connect(ipAddress, port); });
            Task t4 = Task.Run(() => { serverSocket2 = clientConnectRequestListener.AcceptTcpClient(); });
            Task.WaitAll(t3, t4);

            // start send queue listener on server
            SendingQueue sendingQueue = new();
            sendingQueue.RegisterModule("Test Module", true);
            Dictionary<string, TcpClient> clientIdToSocket = new()
            {
                ["Client Id1"] = serverSocket1,
                ["Client Id2"] = serverSocket2
            };
            Dictionary<string, INotificationHandler> subscribedModules = new()
            {
                ["Test Module"] = new TestNotificationHandler()
            };
            SendQueueListenerServer sendQueueListenerServer = new(sendingQueue, clientIdToSocket, subscribedModules);
            sendQueueListenerServer.Start();

            // start socket listener on both clients
            ReceivingQueue receivingQueue1 = new();
            ReceivingQueue receivingQueue2 = new();
            SocketListener socketListener1 = new(receivingQueue1, clientSocket1);
            SocketListener socketListener2 = new(receivingQueue2, clientSocket2);
            socketListener1.Start();
            socketListener2.Start();

            // if its client got disconnected test then disconnect client 1
            if (_clientGotDisconnectedTest)
            {
                clientSocket1.GetStream().Close();
                clientSocket1.Close();
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
                Assert.True(receivingQueue1.IsEmpty());
            }
            else // check client 1 received the packets
            {
                NetworkTestGlobals.PacketsReceiveAssert(sendPackets, receivingQueue1, count);
            }
            // if it was a broadcast test then check client2 also received the packets
            if (destination == null)
            {
                NetworkTestGlobals.PacketsReceiveAssert(sendPackets, receivingQueue2, count);
            }
        }

        [Fact]
		public void SmallPacketUnicastTest()
		{
            PacketsSendTest(_smallPacketSize, _destinations[0], 1);
        }

		[Fact]
		public void LargePacketUnicastTest()
		{
            PacketsSendTest(_largePacketSize, _destinations[0], 1);
        }

        [Fact]
        public void VeryLargePacketUnicastTest()
        {
            PacketsSendTest(_veryLargePacketSize, _destinations[0], 1);
        }

        [Fact]
        public void MultipleSmallPacketsUnicastTest()
        {
            PacketsSendTest(_smallPacketSize, _destinations[0], _multiplePacketsCount);
        }

        [Fact]
        public void MultipleLargePacketsUnicastTest()
        {
            PacketsSendTest(_largePacketSize, _destinations[0], _multiplePacketsCount);
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
