/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of CommunicatorClient.
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace PlexShareNetwork.Communication
{
	public class CommunicatorClient : ICommunicator
	{
		// initialize the send queue and receive queue
		private readonly SendingQueues _sendQueue = new();
		private readonly ReceivingQueue _receiveQueue = new();

		// declate the variable of SendQueueListenerClient class
		private SendQueueListenerClient _sendQueueListener;

		// declate the variable of ReceiveQueueListener class
		private ReceiveQueueListener _receiveQueueListener;

		// declare the socket variable for the client
		private TcpClient _socket = new();

		// declate the variable of SocketListener class
		private SocketListener _socketListener;

		// map to store the handlers of subscribed modules
		private readonly Dictionary<string, INotificationHandler> _subscribedModulesHandlers = new();

		/// <summary>
		/// This function connects the client to the server.
		/// And initializes queues and sockets.
        /// The function arguments are to be only used on the client to connect to the server.
		/// </summary>
		/// <param name="serverIP"> IP Address of the server. Required only on client side. </param>
		/// <param name="serverPort"> Port no. of the server. Required only on client side. </param>
		/// <returns>
		///  string "success" if success, "failure" if failure
		/// </returns>
		public string Start(string serverIP, string serverPort)
		{
			try
			{
				_socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
				_socket.Connect(ParseIP(serverIP), int.Parse(serverPort));

				_socketListener = new SocketListener(_receiveQueue, _socket);
				_socketListener.Start();

				_sendQueueListener = new SendQueueListenerClient(_sendQueue, _socket);
				_sendQueueListener.Start();

				_receiveQueueListener = new ReceiveQueueListener(_subscribedModulesHandlers, _receiveQueue);
				_receiveQueueListener.Start();

                Trace.WriteLine("[Networking] CommunicatorClient started.");
                return "success";
			}
			catch(Exception e)
			{
				Trace.WriteLine($"[Networking] Error in CommunicatorClient: {e.Message}");
				return "failure";
			}
		}

		/// <summary>
		/// Disconnects from the server and stops all running threads.
		/// </summary>
		/// <returns> void </returns>
		public void Stop()
		{
			if (!_socket.Connected)
			{
				return;
			}
			// stop all the running threads
			_socketListener.Stop();
			_sendQueueListener.Stop();
			_receiveQueueListener.Stop();

            // clear the queues
            _sendQueue.Clear();
            _receiveQueue.Clear();

			// close the socket stream and socket connection
			_socket.GetStream().Close();
			_socket.Close();

            Trace.WriteLine("[Networking] CommunicatorClient stopped.");
        }

		/// <summary>
		/// This function parses the IP address given as string and returns the IPAddress.
		/// </summary>
		/// <param name="IPstring"> IP address string. </param>
		/// <returns>
		///  The IP Address
		/// </returns>
		public IPAddress ParseIP(string IPstring)
		{
			try
			{
				Trace.WriteLine("[Networking] Parsing IPv4 address.");
				return IPAddress.Parse(IPstring);
			}
			catch(Exception)
			{
				Trace.WriteLine("[Networking] Parsing DNS name.");
				return Dns.GetHostAddresses(IPstring).Last();
			}
		}

		/// <summary>
		/// This function is to be called only on the server.
		/// </summary>
		[ExcludeFromCodeCoverage]
		public void AddClient<T>(string clientId, T socket)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// This function is to be called only on the server.
		/// </summary>
		[ExcludeFromCodeCoverage]
		public void RemoveClient(string clientId)
		{
			throw new NotSupportedException();
		}

        /// <summary>
        /// This function sends data to the server.
        /// </summary>
        /// <param name="serializedData"> The serialzed data to be sent over the network. </param>
        /// <param name="moduleOfPacket"> Module sending the data. </param>
        /// <returns> void </returns>
        public void Send(string serializedData, string moduleOfPacket)
		{
			Packet packet = new(serializedData, null, moduleOfPacket);
			_sendQueue.Enqueue(packet);
			Trace.WriteLine($"[Networking] Enqueued packet in send queue of the module : {moduleOfPacket}");
		}

		/// <summary>
		/// Function to send data to a specific client given by the destination argument.
		/// This function is to be called only on the server side.
		/// </summary>
		[ExcludeFromCodeCoverage]
		public void Send(string serializedData, string moduleOfPacket, string destination)
		{
			throw new NotSupportedException();
		}

        /// <summary>
        /// Other modules can subscribe using this function to be notified on receiving data over the network.
        /// </summary>
        /// <param name="moduleName"> Name of the module. </param>
        /// <param name="notificationHandler"> Module implementation of the INotificationHandler. </param>
        /// <param name="isHighPriority"> Boolean which tells whether data is high priority or low priority. </param>
        /// <returns> void </returns>
        public void Subscribe(string moduleName, INotificationHandler notificationHandler, bool isHighPriority)
		{
			_subscribedModulesHandlers.Add(moduleName, notificationHandler);
			_sendQueue.RegisterModule(moduleName, isHighPriority);
            _receiveQueueListener.RegisterModule(moduleName, notificationHandler);
            Trace.WriteLine($"[Networking] Module: {moduleName} subscribed with priority is high: {isHighPriority}");
		}
	}
}