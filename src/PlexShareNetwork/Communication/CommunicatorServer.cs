/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of CommunicatorServer.
/// </summary>

using PlexShareNetworking.Queues;
using PlexShareNetworking.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PlexShareNetworking.Communication
{
	public class CommunicatorServer : ICommunicator
	{
		// initialize the send queue and receive queue
		private readonly SendingQueues _sendQueue = new();
		private readonly ReceivingQueue _receiveQueue = new();

		// declate the variable of SendQueueListenerClient class
		private SendQueueListenerServer _sendQueueListener;

		// declate the variable of ReceiveQueueListener class
		private ReceiveQueueListener _receiveQueueListener;

		// declare the server socket variable
		private TcpListener _socket;

		// map to store the sockets of the clients to be able to send data to a speicific client
		private readonly Dictionary<string, TcpClient> _clientIdToSocket = new();

		// this map will store the socket listeners, one socket listener listening to one client
		private readonly Dictionary<string, SocketListener> _clientListeners = new();

		// map to store the handlers of subscribed modules
		private readonly Dictionary<string, INotificationHandler> _subscribedModulesToHandler = new();

		// this thread will be used to accept client requests
		private Thread _thread;

		// boolean to tell whether thread is running or stopped
		private volatile bool _threadRun;

		/// <summary>
		/// This function Find and IP and port and start listening on it.
		/// And initializes queues and sockets.
		/// </summary>
		/// <param name="serverIP"> IP Address of the server. Required only on client side. </param>
		/// <param name="serverPort"> Port no. of the server. Required only on client side. </param>
		/// <returns>
		///  Address of the server as a string of "IP:Port"
		/// </returns>
		public string Start(string serverIP = null, string serverPort = null)
		{
			if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E")
			{
				return "127.0.0.1:8080";
			}
			var ip = IPAddress.Parse(FindIpAddress());
			string stringPort = Environment.GetEnvironmentVariable("MEETME_PORT");
			int port = stringPort is null ? -1 : Int32.Parse(stringPort); 
			if (port == -1)
			{
				port = FindFreePort(ip);
			}
			_socket = new TcpListener(IPAddress.Any, port);
			_socket.Start();

			_sendQueueListener = new SendQueueListenerServer(_sendQueue, _clientIdToSocket, _subscribedModulesToHandler);
			_sendQueueListener.Start();

			_receiveQueueListener = new ReceiveQueueListener(_subscribedModulesToHandler, _receiveQueue);
			_receiveQueueListener.Start();

			_thread = new Thread(AcceptRequest);
			_threadRun = true;
			_thread.Start();

			Trace.WriteLine("Server started on IP: " + ip + " Port: " + port);
			return ip + ":" + port;
		}

		/// <summary>
		/// Stops listening and stops all running threads.
		/// </summary>
		/// <returns> void </returns>
		public void Stop()
		{
			if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E")
			{
				return;
			}
			_threadRun = false;

			_socket.Stop();

			foreach (var clientIdToSocketListener in _clientListeners)
			{
				var socketListener = clientIdToSocketListener.Value;
				socketListener.Stop();
			}
			_sendQueueListener.Stop();
			_receiveQueueListener.Stop();
		}

		/// <summary>
		/// This function finds IP4 address of machine which does not end with 1
		/// </summary>
		/// <returns> String IP address </returns>
		private static string FindIpAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var IP in host.AddressList)
			{
				if (IP.AddressFamily == AddressFamily.InterNetwork)
				{
					var address = IP.ToString();
					// return the IP address if it does not end with 1
					if (address.Split(".")[3] != "1")
					{
						return IP.ToString();
					}
				}
			}
			throw new Exception("[Networking] Error in CommunicatorServer: IPv4 address not found on this machine!");
		}

		/// <summary>
		/// This function finds a free TCP port.
		/// </summary>
		/// <param name="IP"> IP address </param>
		/// <returns> The port number </returns>
		private static int FindFreePort(IPAddress IP)
		{
			var tcpListener = new TcpListener(IP, 0);
			tcpListener.Start();
			var port = ((IPEndPoint) tcpListener.LocalEndpoint).Port;
			tcpListener.Stop();
			return port;
		}

		/// <summary>
		/// This functions accepts the requests from clients.
		/// </summary>
		/// <returns> void </returns>
		private void AcceptRequest()
		{
			while (_threadRun)
			{
				try
				{
					var clientSocket = _socket.AcceptTcpClient();

					foreach (var module in _subscribedModulesToHandler)
					{
						module.Value.OnClientJoined(clientSocket);
					}
					Trace.WriteLine("[Networking] New client joined! Notified all modules.");
				}
				catch (SocketException e)
				{
					Trace.WriteLine($"[Networking] Error in CommunicatorServer: {e.Message}");
				}
				catch (Exception e)
				{
					Trace.WriteLine($"[Networking] Error in CommunicatorServer: {e.Message}");
				}
			}
		}

		/// <summary>
		/// This function is to be called only on the server when a new cilent joins.
		/// It adds the client socket to the map and starts listening to the client.
		/// </summary>
		/// <typeparam name="T"> socket </typeparam>
		/// <param name="clientId"> The client Id. </param>
		/// <param name="socketObject"> The socket object of the client. </param>
		/// <returns> void </returns>
		public void AddClient<T>(string clientId, T socket)
		{
			if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E")
			{
				return;
			}

			_clientIdToSocket[clientId] = (TcpClient) (object) socket;

			var socketListener = new SocketListener(_receiveQueue, (TcpClient) (object) socket);
			_clientListeners[clientId] = socketListener;
			socketListener.Start();
		}

		/// <summary>
		/// This function is to be called only on the server when a client is leaves.
		/// It removes the client information from the server.
		/// </summary>
		/// <param name="clientId"> The client Id. </param>
		/// <returns> void </returns>
		public void RemoveClient(string clientId)
		{
			if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E")
			{
				return;
			}
			var socketListener = _clientListeners[clientId];
			socketListener.Stop();

			var socket = _clientIdToSocket[clientId];
			socket.GetStream().Close();
			socket.Close();

			_clientListeners.Remove(clientId);
			_clientIdToSocket.Remove(clientId);
		}

		/// <summary>
		/// This function broadcasts data to all the clients.
		/// </summary>
		/// <param name="serializedData"> The serialzed data to be sent over the network. </param>
		/// <param name="moduleIdentifier"> Module Identifier of the module. </param>
		/// <returns> void </returns>
		public void Send(string serializedData, string moduleIdentifier)
		{
			if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E")
			{
				File.WriteAllText("networking_test.json", serializedData);
				return;
			}

			var packet = new Packet (serializedData, null, moduleIdentifier);
			try
			{
				_sendQueue.Enqueue(packet);
			}
			catch (Exception e)
			{
				Trace.WriteLine($"[Networking] Error in CommunicatorServer: {e.Message}");
				throw;
			}
		}

		/// <summary>
		/// Function to send data to a specific client given by the destination argument.
		/// This function is to be called only on the server side.
		/// </summary>
		/// <param name="serializedData"> The serialzed data to be sent over the network. </param>
		/// <param name="moduleIdentifier"> Module Identifier of the module. </param>
		/// <param name="destination"> The destination or client Id to which to send the data. </param>
		/// <returns> void </returns>
		public void Send(string serializedData, string moduleIdentifier, string destination)
		{
			if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E")
			{
				File.WriteAllText("networking_test.json", serializedData);
				return;
			}

			if (!_clientIdToSocket.ContainsKey(destination))
			{
				throw new Exception("Client given in the destination of packet does not exist in the room!");
			}

			var packet = new Packet(serializedData, destination, moduleIdentifier);
			try
			{
				_sendQueue.Enqueue(packet);
			}
			catch (Exception e)
			{
				Trace.WriteLine($"[Networking] Error in CommunicatorServer: {e.Message}");
				throw;
			}
		}

		/// <summary>
		/// Other modules can subscribe using this function to be notified on receiving data over the network.
		/// </summary>
		/// <param name="moduleIdentifier"> Module Identifier of the module. </param>
		/// <param name="handler"> Module implementation of the INotificationHandler. </param>
		/// <param name="isHighPriority"> Boolean which tells whether data is high priority or low priority. </param>
		/// <returns> void </returns>
		public void Subscribe(string moduleIdentifier, INotificationHandler notificationHandler, bool isHighPriority)
		{
			if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E")
			{
				return;
			}
			_subscribedModulesToHandler.Add(moduleIdentifier, notificationHandler);
			_sendQueue.RegisterModule(moduleIdentifier, isHighPriority);
			//_receiveQueue.RegisterModule(moduleIdentifier, isHighPriority);
			Trace.WriteLine($"[Networking] Module: {moduleIdentifier} registered with priority is high?: {isHighPriority}");
		}
	}
}
