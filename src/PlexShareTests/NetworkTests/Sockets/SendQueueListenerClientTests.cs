/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener.
/// </summary>

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Networking.Queues;
using NUnit.Framework;

namespace Networking.Sockets.Test
{
	[TestFixture]
	public class SendQueueListenerClientTest
	{
		private SendingQueues _sendQueue;
		private ReceivingQueue _receiveQueue;
		private Machine _server;
		private SendQueueListenerClient _sendQueueListenerClient;
		private SocketListener _socketListener;
		private TcpClient _serverSocket;
		private TcpClient _clientSocket;

		[SetUp]
		public void StartSendQueueListenerClient()
		{
			_server = new FakeServer();
			var IPAndPort = _server.Communicator.Start().Split(":");
			var IP = IPAddress.Parse(IPAndPort[0]);
			var port = int.Parse(IPAndPort[1]);
			_server.Communicator.Stop();
			var serverSocket = new TcpListener(IP, port);
			serverSocket.Start();
			_clientSocket = new TcpClient();
			_clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
			var t1 = Task.Run(() => { _clientSocket.Connect(IP, port); });
			var t2 = Task.Run(() => { _serverSocket = serverSocket.AcceptTcpClient(); });
			Task.WaitAll(t1, t2);
			_sendQueue = new SendingQueues();
			_sendQueue.RegisterModule(NetworkingGlobals.whiteboardName, NetworkingGlobals.whiteboardPriority);
			_sendQueueListenerClient = new SendQueueListenerClient(_sendQueue, _clientSocket);
			_sendQueueListenerClient.Start();

			_receiveQueue = new ReceivingQueue();
			//_receiveQueue.RegisterModule(NetworkingGlobals.whiteboardName, NetworkingGlobals.whiteboardPriority);

			_socketListener = new SocketListener(_receiveQueue, _serverSocket);
			_socketListener.Start();
		}

		[TearDown]
		public void TearDown()
		{
			_clientSocket.Close();
			_serverSocket.Close();
			_socketListener.Stop();
			_sendQueueListenerClient.Stop();
		}

		[Test]
		public void SinglePacketSendTest()
		{
			const string data = "Test string";
			var sendPacket = new Packet (data, null, NetworkingGlobals.whiteboardName);
			_sendQueue.Enqueue(sendPacket);

			while (_receiveQueue.IsEmpty())
			{
			}
			var receivedPacket = _receiveQueue.Dequeue();
			Assert.Multiple(() =>
			{
				Assert.AreEqual(sendPacket.getSerializedData(), receivedPacket.getSerializedData());
				Assert.AreEqual(sendPacket.getModuleOfPacket(), receivedPacket.getModuleOfPacket());
			});
		}

		[Test]
		public void LargeSizePacketSendTest()
		{
			var data = NetworkingGlobals.RandomString(4000);
			var sendPacket = new Packet(data, null, NetworkingGlobals.whiteboardName);
			_sendQueue.Enqueue(sendPacket);

			while (_receiveQueue.IsEmpty())
			{
			}
			var receivedPacket = _receiveQueue.Dequeue();
			Assert.Multiple(() =>
			{
				Assert.AreEqual(sendPacket.getSerializedData(), receivedPacket.getSerializedData());
				Assert.AreEqual(sendPacket.getModuleOfPacket(), receivedPacket.getModuleOfPacket());
			});
		}

		[Test]
		public void MultiplePacketSendTest()
		{
			for (var i = 1; i <= 10; i++)
			{
				var data = "Test string" + i;
				var sendPacket = new Packet(data, null, NetworkingGlobals.whiteboardName);
				_sendQueue.Enqueue(sendPacket);
			}
			while (_receiveQueue.Size() != 10)
			{
				Thread.Sleep(10);
			}
			for (var i = 1; i <= 10; i++)
			{
				var packet = _receiveQueue.Dequeue();
				var data = "Test string" + i;
				Assert.AreEqual(data, packet.getSerializedData());
			}
		}
	}
}
