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
		private readonly SendingQueue _sendingQueue = new();
		private readonly ReceivingQueue _receivingQueue = new();
        private readonly ICommunicator _serverCommunicator = CommunicationFactory.GetCommunicator(false);
		private readonly SendQueueListenerClient _sendQueueListenerClient;
		private readonly SocketListener _socketListener;
		private TcpClient _serverSocket;
		private readonly TcpClient _clientSocket = new();

		public SendQueueListenerClientTests()
		{
			string[] IPAndPort = _serverCommunicator.Start().Split(":");
            _serverCommunicator.Stop();
            IPAddress IP = IPAddress.Parse(IPAndPort[0]);
			int port = int.Parse(IPAndPort[1]);
            TcpListener serverSocket = new(IP, port);
			serverSocket.Start();
			_clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
			Task t1 = Task.Run(() => { _clientSocket.Connect(IP, port); });
            Task t2 = Task.Run(() => { _serverSocket = serverSocket.AcceptTcpClient(); });
			Task.WaitAll(t1, t2);
            _sendingQueue.RegisterModule("Test Module1", true);
            _sendingQueue.RegisterModule("Test Module2", true);
            _sendQueueListenerClient = new(_sendingQueue, _clientSocket);
			_sendQueueListenerClient.Start();
			_socketListener = new(_receivingQueue, _serverSocket);
			_socketListener.Start();
		}

		[Fact]
		public void SinglePacketSendTest()
		{
            Packet sendPacket = new("Test string", "To Server", "Test Module1");
            _sendingQueue.Enqueue(sendPacket);
            NetworkTestGlobals.AssertSinglePacketReceive(sendPacket, _receivingQueue);
        }

		[Fact]
		public void LargePacketSendTest()
		{
            Packet sendPacket = new(NetworkTestGlobals.RandomString(1000), "To Server", "Test Module1");
            _sendingQueue.Enqueue(sendPacket);
            NetworkTestGlobals.AssertSinglePacketReceive(sendPacket, _receivingQueue);
        }

		[Fact]
		public void MultiplePacketsFromSameModuleSendTest()
		{
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
			{
                sendPackets[i] = new Packet("Test string" + i, "To Server", "Test Module1");
                _sendingQueue.Enqueue(sendPackets[i]);
			}
            NetworkTestGlobals.AssertTenPacketsReceive(sendPackets, _receivingQueue);
        }

        [Fact]
        public void MultiplePacketsFromDifferentModulesSendTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new Packet("Test string" + i, "To Server", "Test Module" + (i%2+1));
                _sendingQueue.Enqueue(sendPackets[i]);
            }
            NetworkTestGlobals.AssertTenPacketsReceive(sendPackets, _receivingQueue);
        }

        [Fact]
        public void MultipleLargePacketsSendTest()
        {
            Packet[] sendPackets = new Packet[10];
            for (var i = 0; i < 10; i++)
            {
                sendPackets[i] = new Packet(NetworkTestGlobals.RandomString(1000), "To Server", "Test Module1");
                _sendingQueue.Enqueue(sendPackets[i]);
            }
            NetworkTestGlobals.AssertTenPacketsReceive(sendPackets, _receivingQueue);
        }
    }
}
