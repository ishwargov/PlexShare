/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using Xunit;

namespace PlexShareNetwork.Sockets.Tests
{
	public class SendQueueListenerServerTests
	{
        private readonly int _multiplePacketsCount = 10;
        private readonly int _smallPacketSize = 100;
        private readonly int _largePacketSize = 10000;
        private readonly int _veryLargePacketSize = 10000000; // adding one more 0 to it will hang you laptop
        private readonly string[] _destinations = { "Client1 ID", "Client2 ID" };
        private readonly string _module = "Test Module";
        bool _clientGotDisconnectedTest = false;

        private void PacketsSendTest(int size, string? destination, int count)
        {
            SendingQueue _sendingQueue = new();
            ReceivingQueue _receivingQueue1 = new();
            ReceivingQueue _receivingQueue2 = new();
            CommunicatorServer _communicatorServer = new();
            SendQueueListenerServer _sendQueueListenerServer;
            SocketListener _socketListener1;
            SocketListener _socketListener2;
            TcpClient _clientSocket1 = new();
            TcpClient _clientSocket2 = new();
            TcpClient _serverSocket1 = new();
            TcpClient _serverSocket2 = new();
            TcpListener _serverListener;
            Dictionary<string, TcpClient> _clientIdToSocket = new();
            Dictionary<string, INotificationHandler> _subscribedModules = new();
            int _port;
            IPAddress _IP;

            string[] IPAndPort = _communicatorServer.Start().Split(":");
            _communicatorServer.Stop();
            _IP = IPAddress.Parse(IPAndPort[0]);
            _port = int.Parse(IPAndPort[1]);
            _serverListener = new TcpListener(_IP, _port);
            _serverListener.Start();

            _sendingQueue.RegisterModule("Test Module", true);
            _subscribedModules["Test Module"] = new TestNotificationHandler();

            _sendQueueListenerServer = new SendQueueListenerServer(_sendingQueue, _clientIdToSocket, _subscribedModules);
            _sendQueueListenerServer.Start();

            _clientSocket1.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            _clientSocket2.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            var t1 = Task.Run(() => { _clientSocket1.Connect(_IP, _port); });
            var t2 = Task.Run(() => { _serverSocket1 = _serverListener.AcceptTcpClient(); });
            Task.WaitAll(t1, t2);

            Task t3 = Task.Run(() => { _clientSocket2.Connect(_IP, _port); });
            Task t4 = Task.Run(() => { _serverSocket2 = _serverListener.AcceptTcpClient(); });
            Task.WaitAll(t3, t4);

            _clientIdToSocket["Client1 ID"] = _serverSocket1;
            _clientIdToSocket["Client2 ID"] = _serverSocket2;

            _socketListener1 = new SocketListener(_receivingQueue1, _clientSocket1);
            _socketListener1.Start();
            _socketListener2 = new SocketListener(_receivingQueue2, _clientSocket2);
            _socketListener2.Start();

            if (_clientGotDisconnectedTest)
            {
                _clientSocket1.Close();
                _clientSocket1.Dispose();
            }

            Packet[] sendPackets = NetworkTestGlobals.GeneratePackets(size, destination, _module, count);
            NetworkTestGlobals.SendPackets(sendPackets, _sendingQueue, count);
            if (_clientGotDisconnectedTest)
            {
                TestNotificationHandler testNotificationHandler = (TestNotificationHandler)_subscribedModules["Test Module"];
                testNotificationHandler.WaitForEvent();
                // assert module is notified, client1 did not receive, and client2 received
                Assert.Equal("OnClientLeft", testNotificationHandler.GetLastEvent());
                Assert.Equal("Client1 ID", testNotificationHandler.GetLastEventClientId());
                Assert.True(_receivingQueue1.IsEmpty());
            }
            else
            {
                NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue1, count);
            }
            if (destination == null)
            {
                NetworkTestGlobals.PacketsReceiveAssert(sendPackets, _receivingQueue2, count);
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
