/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Networking.Queues;
using Xunit;

namespace Networking.Sockets.Tests
{
	public class SendQueueListenerServerTest
	{
		private SendingQueues _sendQueue;
		private ReceivingQueue _receiveQueue1, _receiveQueue2;
		private Machine _server;
		private SendQueueListenerServer _sendQueueListenerServer;
		private SocketListener _socketListener1, _socketListener2;
		private TcpClient _serverSocket1, _serverSocket2, _clientSocket1, _clientSocket2;
		private TcpListener _serverListener;
		private Dictionary<string, TcpClient> _clientIdToSocket;
		private Dictionary<string, INotificationHandler> _subscribedModules;
		private int _port;
		private IPAddress _IP;

		public SendQueueListenerServerTest()
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
				_serverSocket1 = _serverListener.AcceptTcpClient();
				_clientIdToSocket["1"] = _serverSocket1;
			});
			Task.WaitAll(t1, t2);
			_subscribedModules = new Dictionary<string, INotificationHandler>();
			_sendQueue = new SendingQueues();
			_sendQueue.RegisterModule(NetworkingGlobals.whiteboardName, NetworkingGlobals.whiteboardPriority);
			var handler = new FakeNotificationHandler();
			_subscribedModules[NetworkingGlobals.whiteboardName] = handler;
			_sendQueueListenerServer = new SendQueueListenerServer(_sendQueue, _clientIdToSocket, _subscribedModules);
			_sendQueueListenerServer.Start();

			_receiveQueue1 = new ReceivingQueue();

			_socketListener1 = new SocketListener(_receiveQueue1, _clientSocket1);
			_socketListener1.Start();
		}

		[Fact]
		public void BroadcastSendTest()
		{
			_receiveQueue2 = new ReceivingQueue();

			var t1 = Task.Run(() => 
			{
				_clientSocket2 = new TcpClient();
				_clientSocket2.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
				_clientSocket2.Connect(_IP, _port);
			});
			var t2 = Task.Run(() => 
			{
				_serverSocket2 = _serverListener.AcceptTcpClient();
				_clientIdToSocket["2"] = _serverSocket2;
			});
			Task.WaitAll(t1, t2);
			_socketListener2 = new SocketListener(_receiveQueue2, _clientSocket2);
			_socketListener2.Start();
			var data = "Test string";
			var sendPacket = new Packet(data, null, NetworkingGlobals.whiteboardName);
			_sendQueue.Enqueue(sendPacket);
			while (_receiveQueue1.IsEmpty())
			{
			}
			var receivedPacket = _receiveQueue1.Dequeue();
			Assert.Equal(sendPacket.getSerializedData(), receivedPacket.getSerializedData());
			Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket.getModuleOfPacket());
			while (_receiveQueue2.IsEmpty())
			{
			}
			var receivedPacket2 = _receiveQueue2.Dequeue();
			Assert.Equal(sendPacket.getSerializedData(), receivedPacket2.getSerializedData());
			Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket2.getModuleOfPacket());
			_serverSocket2.Close();
			_socketListener2.Stop();
			_clientSocket2.Close();
		}

		[Fact]
		public void SinglePacketUnicastTest()
		{
			var data = "Test string";
			var sendPacket = new Packet (data, "1", NetworkingGlobals.whiteboardName);
			_sendQueue.Enqueue(sendPacket);
			while (_receiveQueue1.IsEmpty())
			{
			}
			var receivedPacket = _receiveQueue1.Dequeue();
			Assert.Equal(sendPacket.getSerializedData(), receivedPacket.getSerializedData());
			Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket.getModuleOfPacket());
		}

		[Fact]
		public void LargeSizePacketSendTest()
		{
			var data = NetworkingGlobals.RandomString(1500);
			var sendPacket = new Packet (data, null, NetworkingGlobals.whiteboardName);
			_sendQueue.Enqueue(sendPacket);

			while (_receiveQueue1.IsEmpty())
			{
			}
			var receivedPacket = _receiveQueue1.Dequeue();
			Assert.Equal(sendPacket.getSerializedData(), receivedPacket.getSerializedData());
			Assert.Equal(sendPacket.getModuleOfPacket(), receivedPacket.getModuleOfPacket());
		}

		[Fact]
		public void ClientGotDisconnectedTest()
		{
			_socketListener1.Stop();
			while (_socketListener1._thread.IsAlive)
            {
				Thread.Sleep(100);
            }
			_clientSocket1.Close();
			_clientSocket1.Dispose();
			var data = "Test string";
			var sendPacket = new Packet(data, null, NetworkingGlobals.whiteboardName);
			_sendQueue.Enqueue(sendPacket);
			var whiteBoardHandler = (FakeNotificationHandler) _subscribedModules[NetworkingGlobals.whiteboardName];
			whiteBoardHandler.Wait();
			Assert.Equal(NotificationEvents.OnClientLeft, whiteBoardHandler.Event);
		}
	}
}
