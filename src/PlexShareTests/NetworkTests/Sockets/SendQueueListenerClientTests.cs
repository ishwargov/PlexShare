/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener.
/// </summary>

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Networking;
using NUnit.Framework;

namespace Networking.Sockets.Test
{
	[TestFixture]
	public class SendQueueListenerClientTest
	{
		private IQueue _sendQueue;
		private IQueue _receiveQueue;
		private Machine _server;
		private SendQueueListener _sendQueueListenerClient;
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
			_sendQueue = new Queue();
			_sendQueue.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
			_sendQueueListenerClient = new SendQueueListenerClient(_sendQueue, _clientSocket);
			_sendQueueListenerClient.Start();

			_receiveQueue = new Queue();
			_receiveQueue.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);

			_socketListener = new SocketListener(_receiveQueue, _serverSocket);
			_socketListener.Start();
		}

		[TearDown]
		public void TearDown()
		{
			_sendQueue.Close();
			_receiveQueue.Close();
			_clientSocket.Close();
			_serverSocket.Close();
			_socketListener.Close();
			_sendQueueListenerClient.Close();
		}

		[Test]
		public void SinglePacketSendTest()
		{
			const string data = "Test string";
			var sendPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = data};
			_sendQueue.Enqueue(sendPacket);

			while (_receiveQueue.IsEmpty())
			{
			}
			var receivedPacket = _queue.Dequeue();
			Assert.Multiple(() =>
			{
				Assert.AreEqual(sendPacket.SerializedData, receivedPacket.SerializedData);
				Assert.AreEqual(sendPacket.ModuleIdentifier, receivedPacket.ModuleIdentifier);
			});
		}

		[Test]
		public void LargeSizePacketSendTest()
		{
			var data = NetworkingGlobals.GetRandomString(4000);
			var sendPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = data};
			_sendQueue.Enqueue(sendPacket);

			while (_receiveQueue.IsEmpty())
			{
			}
			var receivedPacket = _queue.Dequeue();
			Assert.Multiple(() =>
			{
				Assert.AreEqual(sendPacket.SerializedData, receivedPacket.SerializedData);
				Assert.AreEqual(sendPacket.ModuleIdentifier, receivedPacket.ModuleIdentifier);
			});
		}

		[Test]
		public void MultiplePacketSendTest()
		{
			for (var i = 1; i <= 10; i++)
			{
				var data = "Test string" + i;
				var sendPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = data};
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
				Assert.AreEqual(data, packet.SerializedData);
			}
		}
	}
}
