/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener.
/// </summary>

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Networking;
using NUnit.Framework;

namespace Networking.Sockets.Test
{
	[TestFixture]
	public class SocketListenerTest
	{
		private IQueue _queue;
		private Machine _server;
		private TcpClient _serverSocket;
		private TcpClient _clientSocket;
		private SocketListener _socketListener;

		[SetUp]
		public void StartSocketListener()
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
			_queue = new Queue();
			_queue.RegisterModule(NetworkingGlobals.whiteboardName, NetworkingGlobals.whiteboardPriority);
			_socketListener = new SocketListener(_queue, _serverSocket);
			_socketListener.Start();
		}

		[TearDown]
		public void TearDown()
		{
			_queue.Close();
			_clientSocket.Close();
			_serverSocket.Close();
			_socketListener.Close();
		}

		[Test]
		public void SinglePacketReceiveTest()
		{
			const string data = "Test string";
			var sendPacket = new Packet { ModuleIdentifier = NetworkingGlobals.whiteboardName, SerializedData = data };
			var pkt = sendPacket.ModuleIdentifier + ":" + sendPacket.SerializedData;
			pkt = pkt.Replace("[ESC]", "[ESC][ESC]");
			pkt = pkt.Replace("[FLAG]", "[ESC][FLAG]");
			pkt = "[FLAG]" + pkt + "[FLAG]";

			var stream = _clientSocket.GetStream();
			stream.Write(Encoding.ASCII.GetBytes(pkt), 0, pkt.Length);
			stream.Flush();
			while (_queue.IsEnpty())
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
		public void LargeSizePacketReceiveTest()
		{
			var data = NetworkingGlobals.GetRandomString(4000);
			var sendPacket = new Packet { ModuleIdentifier = NetworkingGlobals.whiteboardName, SerializedData = data };
			var pkt = sendPacket.ModuleIdentifier + ":" + sendPacket.SerializedData;
			pkt = pkt.Replace("[ESC]", "[ESC][ESC]");
			pkt = pkt.Replace("[FLAG]", "[ESC][FLAG]");
			pkt = "[FLAG]" + pkt + "[FLAG]";

			var stream = _clientSocket.GetStream();
			stream.Write(Encoding.ASCII.GetBytes(pkt), 0, pkt.Length);
			stream.Flush();
			while (_queue.IsEnpty())
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
		public void MultiplePacketReceiveTest()
		{
			for (var i = 1; i <= 10; i++)
			{
				var data = "Test string" + i;
				var sendPacket = new Packet { ModuleIdentifier = NetworkingGlobals.whiteboardName, SerializedData = data };
				var pkt = sendPacket.ModuleIdentifier + ":" + sendPacket.SerializedData;
				pkt = pkt.Replace("[ESC]", "[ESC][ESC]");
				pkt = pkt.Replace("[FLAG]", "[ESC][FLAG]");
				pkt = "[FLAG]" + pkt + "[FLAG]";
				_clientSocket.Client.Send(Encoding.ASCII.GetBytes(pkt));
			}
			while (_queue.Size() != 10)
			{
				Thread.Sleep(10);
			}
			for (var i = 1; i <= 10; i++)
			{
				var packet = _queue.Dequeue();
				var data = "Test string" + i;
				Assert.AreEqual(data, packet.SerializedData);
			}
		}
	}
}
