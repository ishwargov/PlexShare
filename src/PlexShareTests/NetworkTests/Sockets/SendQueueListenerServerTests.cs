/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener.
/// </summary>

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networking;
using NUnit.Framework;

namespace Networking.Sockets.Test
{
	[TestFixture]
	public class SendQueueListenerServerTest
	{
		private IQueue _sendQueue;
		private IQueue _receiveQueue1, _receiveQueue2;
		private Machine _server;
		private SendQueueListenerServer _sendQueueListenerServer;
		private SocketListener _socketListener1, _socketListener2;
		private TcpClient _serverSocket1, _serverSocket2, _clientSocket1, _clientSocket2;
		private TcpListener _serverListener;
		private Dictionary<string, TcpClient> _clientIdToSocket;
		private Dictionary<string, INotificationHandler> _subscribedModules;
		private int _port;
		private IPAddress _IP;

		public void OneTimeSetup()
		{
			Environment.SetEnvironmetVariable("TEST_MODE", "UNIT");
		}

		[SetUp]
		public void StartSendQueueListenerServer()
		{
			_server = new FakeServer();
			var IPAndPort = _server.Communicator.Start().Split(":");
			_IP = IPAddress.Parse(IPAndPort[0]);
			_port = int.Parse(IPAndPort[1]);
			_server.Communicator.Stop();
			_serverListener = new TcpListener(_IP, _port);
			_serverListener.Start();
			_clientIdToSocket = new Dictionary<string, TcpClient>();
			var t1 = Task.Run(() =>
			{
				_clientSocket1 = new TcpClient();
				_clientSocket1.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
				_clientSocket1.Connect(_IP, _port);
			});
			var t2 = Task.Run(() =>
			{
				_serverSocket = serverSocket.AcceptTcpClient();
				_clientIdToSocket["1"] = _serverSocket1;
			});
			Task.WaitAll(t1, t2);
			_subscribedModules = new Dictionary<string, INotificationHandler>();
			_sendQueue = new Queue();
			_sendQueue.RegisterModule(NetworkingGlobals.whiteboardName, NetworkingGlobals.whiteboardPriority);
			var handler = new FakeNotificationHandler();
			_subscribedModules[NetworkingGlobals.whiteboardName] = handler;
			_sendQueueListenerServer = new SendQueueListenerServer(_sendQueue, _clientIdToSocket, _subscribedModules);
			_sendQueueListenerServer.Start();

			_receiveQueue1 = new Queue();
			_receiveQueue1.RegisterModule(NetworkingGlobals.whiteboardName, NetworkingGlobals.whiteboardPriority);

			_socketListener1 = new SocketListener(_receiveQueue1, _clientSocket1);
			_socketListener1.Start();
		}

		[TearDown]
		public void TearDown()
		{
			_sendQueue.Close();
			_receiveQueue1.Close();
			_clientSocket1.Close();
			_serverSocket1.Close();
			_socketListener1.Close();
			_sendQueueListenerServer.Close();
		}

		[Test]
		public void BroadcastSendTest()
		{
			_receiveQueue2 = new Queue();
			_receiveQueue2.RegisterModule(NetworkingGlobals.whiteboardName, NetworkingGlobals.whiteboardPriority);

			var t1 = Task.Run(() => 
			{
				_clientSocket2 = new TcpClient();
				_clientSocket2.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
				_clientSocket2.Connect(_IP, _port);
			});
			var t1 = Task.Run(() => 
			{
				_serverSocket2 = _serverListener.AcceptTcpClient();
				_clientIdToSocket["2"] = _serverSocket2;
			});
			Task.WaitAll(t1, t2);
			_socketListener2 = new SocketListener(_receiveQueue1, _clientSocket2);
			_socketListener2.Start();
			var data = "Test string";
			var sendPacket = new Packet {ModuleIdentifier = NetworkingGlobals.whiteboardName, SerializedData = data};
			_sendQueue.Enqueue(sendPacket);
			while (_receiveQueue1.IsEmpty())
			{
			}
			var receivedPacket = _receiveQueue1.Dequeue();
			Assert.Multiple(() =>
			{
				Assert.AreEqual(sendPacket.SerializedData, receivedPacket.SerializedData);
				Assert.AreEqual(sendPacket.ModuleIdentifier, receivedPacket.ModuleIdentifier);
			});
			while (_receiveQueue2.IsEmpty())
			{
			}
			var receivedPacket2 = _receiveQueue1.Dequeue();
			Assert.Multiple(() =>
			{
				Assert.AreEqual(sendPacket.SerializedData, receivedPacket2.SerializedData);
				Assert.AreEqual(sendPacket.ModuleIdentifier, receivedPacket2.ModuleIdentifier);
			});
			_serverSocket2.Close();
			_receiveQueue2.Close();
			_socketListener2.Close();
			_clientSocket2.Close();
		}

		[Test]
		public void SinglePacketUnicastTest()
		{
			var data = "Test string";
			var sendPacket = new Packet {ModuleIdentifier = NetworkingGlobals.whiteboardName, SerializedData = data, Destination = "1"};
			_sendQueue.Enqueue(sendPacket);
			while (_receiveQueue1.IsEmpty())
			{
			}
			var receivedPacket = _receiveQueue1.Dequeue();
			Assert.Multiple(() =>
			{
				Assert.AreEqual(sendPacket.SerializedData, receivedPacket.SerializedData);
				Assert.AreEqual(sendPacket.ModuleIdentifier, receivedPacket.ModuleIdentifier);
			});
		}

		[Test]
		public void LargeSizePacketSendTest()
		{
			var data = NetworkingGlobals.GetRandomString(1500);
			var sendPacket = new Packet {ModuleIdentifier = NetworkingGlobals.whiteboardName, SerializedData = data};
			_sendQueue.Enqueue(sendPacket);

			while (_receiveQueue1.IsEmpty())
			{
			}
			var receivedPacket = _receiveQueue1.Dequeue();
			Assert.Multiple(() =>
			{
				Assert.AreEqual(sendPacket.SerializedData, receivedPacket.SerializedData);
				Assert.AreEqual(sendPacket.ModuleIdentifier, receivedPacket.ModuleIdentifier);
			});
		}

		[Test]
		public void ClientGotDisconnectedTest()
		{
			_clientSocket1.Close();
			_clientSocket1.Dispose();
			var data = "Test string";
			var sendPacket = new Packet {ModuleIdentifier = NetworkingGlobals.whiteboardName, SerializedData = data};
			_queueS.Enqueue(sendPacket);
			var whiteBoardHandler = (FakeNotificationHandler) _notificationHandlers[NetworkingGlobals.whiteboardName];
			whiteBoardHandler.Wait();
			Assert.AreEqual(NotificationEvents.OnClientLeft, whiteBoardHandler.Event);
		}
	}
}
