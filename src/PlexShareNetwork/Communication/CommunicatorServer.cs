/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of CommunicatorServer.
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PlexShareNetwork.Communication
{
	public class CommunicatorServer : ICommunicator
	{
		// initialize the send queue and receive queue
		private readonly SendingQueue _sendingQueue = new();
		private readonly ReceivingQueue _receivingQueue = new();

		// declate the variable of SendQueueListenerClient class
		private SendQueueListenerServer _sendQueueListener;

		// declate the variable of ReceiveQueueListener class
		private ReceiveQueueListener _receiveQueueListener;

		// declare the server socket variable
		private TcpListener _socketListener;

		// map to store the sockets of the clients to be able to send data to a speicific client
		private readonly Dictionary<string, TcpClient> _clientIdToClientSocket = new();

		// this map will store the socket listeners, one socket listener listening to one client
		private readonly Dictionary<string, SocketListener> _clientIdToClientListener = new();

		// map to store the handlers of subscribed modules
		private readonly Dictionary<string, INotificationHandler> _moduleToNotificationHanderMap = new();

		// this thread will be used to accept client requests
		private Thread _thread;

		// boolean to tell whether thread is running or stopped
		private volatile bool _threadRun;

        /// <summary>
        /// This function finds IP and port of the machine and start listening on it.
        /// And initializes queues and sockets.
        /// The function arguments are not requred on the server side.
        /// </summary>
        /// <param name="serverIP"> Required only on client side. On server side give null. </param>
        /// <param name="serverPort"> Required only on client side. On server side give null. </param>
        /// <returns>
        ///  Address of the server as a string of "IP:Port"
        /// </returns>
        public string Start(string serverIP = null, string serverPort = null)
		{
            Trace.WriteLine("[Networking] CommunicatorServer.Start() function called.");

            IPAddress ip = IPAddress.Parse(FindIpAddress());
			int port = FindFreePort(ip);
			_socketListener = new TcpListener(IPAddress.Any, port);
			_socketListener.Start(); // start listening on this IP address and port

            // start send queue listener to send data from sending queue
            _sendQueueListener = new SendQueueListenerServer(_sendingQueue, _clientIdToClientSocket, _moduleToNotificationHanderMap);
			_sendQueueListener.Start();

            // start receive queue listener to notify modules on data receive
            _receiveQueueListener = new ReceiveQueueListener(_moduleToNotificationHanderMap, _receivingQueue);
			_receiveQueueListener.Start();

            // start the thread to accept client connect requests
			_threadRun = true;
			_thread = new Thread(AcceptClientConnectRequests);
			_thread.Start();

			Trace.WriteLine("[Networking] CommunicatorServer started on IP: " + ip + " Port: " + port);
			return ip + ":" + port;
		}

		/// <summary>
		/// Stops listening and stops all running threads.
		/// </summary>
		/// <returns> void </returns>
		public void Stop()
		{
            Trace.WriteLine("[Networking] CommunicatorServer.Stop() function called.");

            // stop the accept client connect requests thread
            _threadRun = false;

            // stop listening to the clients
            foreach (var clientListener in _clientIdToClientListener.Values)
			{
				clientListener.Stop();
			}

            // stop all running threads
            _socketListener.Stop();
            _sendQueueListener.Stop();
            _receiveQueueListener.Stop();

            // clear the queues
            _sendingQueue.Clear();
            _receivingQueue.Clear();

            Trace.WriteLine("[Networking] CommunicatorServer stopped.");
        }

		/// <summary>
		/// This function finds IP4 address of machine which does not ends with 1
		/// </summary>
		/// <returns> IP address as string </returns>
		private static string FindIpAddress()
		{
            Trace.WriteLine("[Networking] CommunicatorServer.FindIpAddress() function called.");
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress IP in host.AddressList)
			{
				if (IP.AddressFamily == AddressFamily.InterNetwork)
				{
					string address = IP.ToString();
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
            Trace.WriteLine("[Networking] CommunicatorServer.FindFreePort() function called.");
            TcpListener tcpListener = new(IP, 0);
			tcpListener.Start();
			var port = ((IPEndPoint) tcpListener.LocalEndpoint).Port;
			tcpListener.Stop();
			return port;
		}

		/// <summary>
		/// This functions accepts the connect requests from clients.
		/// </summary>
		/// <returns> void </returns>
		private void AcceptClientConnectRequests()
		{
            Trace.WriteLine("[Networking] CommunicatorServer.AcceptClientConnectRequests() function called.");
            while (_threadRun)
			{
				try
				{
					var clientSocket = _socketListener.AcceptTcpClient();

                    foreach (var moduleToNotificationHandler in _moduleToNotificationHanderMap)
                    {
                        moduleToNotificationHandler.Value.OnClientJoined(clientSocket);
                        Trace.WriteLine($"[Networking] Notifed module:{moduleToNotificationHandler.Key} that new client has joined.");
                    }
				}
				catch (SocketException e)
				{
                    if (e.SocketErrorCode == SocketError.Interrupted)
                    {
                        Trace.WriteLine("[Networking] Error in CommunicatorServer: Socket listener has been closed.");
                    }
                    else
                    {
                        Trace.WriteLine($"[Networking] Error in CommunicatorServer: {e.Message}");
                    }
				}
				catch (Exception e)
				{
					Trace.WriteLine($"[Networking] Error in CommunicatorServer: {e.Message}");
				}
			}
		}

        /// <summary>
        /// This function is to be called by the Dashboard module only on the server side when a new 
        /// client joins. It adds the client socket to the map and starts listening to the client.
        /// </summary>
        /// <typeparam name="T"> socket </typeparam>
        /// <param name="clientId"> The client Id. </param>
        /// <param name="socket"> The socket object of the client. </param>
        /// <returns> void </returns>
        public void AddClient(string clientId, TcpClient socket)
		{
            Trace.WriteLine("[Networking] CommunicatorServer.AddClient() function called.");
            try
            {
                _clientIdToClientSocket[clientId] = socket;
                SocketListener clientListener = new(_receivingQueue, socket);
                _clientIdToClientListener[clientId] = clientListener;
                clientListener.Start();
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[Networking] Error in AddClient(): {e.Message}");
            }
            Trace.WriteLine($"[Networking] Client added with clientID: {clientId}");
        }

		/// <summary>
		/// This function is to be called by the Dashboard module only on the server when 
        /// a client is leaves. It removes the client from the map on the server.
		/// </summary>
		/// <param name="clientId"> The client Id. </param>
		/// <returns> void </returns>
		public void RemoveClient(string clientId)
		{
            Trace.WriteLine("[Networking] CommunicatorServer.RemoveClient() function called.");

            // stop listening to this client and remove the client listener from the map
            SocketListener socketListener = _clientIdToClientListener[clientId];
			socketListener.Stop();
			_clientIdToClientListener.Remove(clientId);

            // close the connection to the client and remove the client socket from the map
			TcpClient socket = _clientIdToClientSocket[clientId];
			socket.GetStream().Close();
			socket.Close();
			_clientIdToClientSocket.Remove(clientId);

            Trace.WriteLine($"[Networking] Client removed with clientID: {clientId}");
        }

        /// <summary>
        /// Function to send data to a particular client or all clients form the server.
        /// </summary>
        /// <param name="serializedData"> The serialzed data to be sent to the client(s). </param>
        /// <param name="moduleName"> Module sending the data. </param>
        /// <param name="destination"> The client Id to which to send the data. To broadcast give null in detination. </param>
        /// <returns> void </returns>
        public void Send(string serializedData, string moduleName, string? destination)
		{
            Trace.WriteLine("[Networking] CommunicatorServer.Send() function called.");
            if (destination != null)
            {
                if (!_clientIdToClientSocket.ContainsKey(destination))
                {
                    Trace.WriteLine($"[Networking] Sending Falied. Client with ID: {destination} does not exist in the room!");
                }
            }
            Packet packet = new(serializedData, destination, moduleName);
			_sendingQueue.Enqueue(packet);
            Trace.WriteLine($"[Networking] Enqueued packet in send queue of the module : {moduleName} for destination : {destination}");
        }

        /// <summary>
        /// Other modules can subscribe using this function to be notified when data is received.
        /// </summary>
        /// <param name="moduleName"> Name of the module. </param>
        /// <param name="notificationHandler"> Module implementation of the INotificationHandler. </param>
        /// <param name="isHighPriority"> Boolean which tells whether data is high priority or low priority. </param>
        /// <returns> void </returns>
        public void Subscribe(string moduleName, INotificationHandler notificationHandler, bool isHighPriority)
		{
            Trace.WriteLine("[Networking] CommunicatorServer.Subscribe() function called.");
            _moduleToNotificationHanderMap.Add(moduleName, notificationHandler);
            _sendingQueue.RegisterModule(moduleName, isHighPriority);
			Trace.WriteLine($"[Networking] Module: {moduleName} subscribed with priority is high: {isHighPriority}");
		}
	}
}
