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
		private readonly SendingQueue _sendingQueue = new();
		private readonly ReceivingQueue _receivingQueue = new();

        // declare all the threads
        private readonly SocketListener _socketListener;
        private readonly SendQueueListenerClient _sendQueueListener;
		private readonly ReceiveQueueListener _receiveQueueListener;

		// declare the socket variable for the client
		private readonly TcpClient _socket = new();

		// map to store the notification handlers of subscribed modules
		private readonly Dictionary<string, INotificationHandler> _moduleToNotificationHanderMap = new();

        /// <summary>
        /// Constructor initializes all threads.
        /// </summary>
        public CommunicatorClient()
        {
            // initialize all threads
            _socketListener = new SocketListener(_receivingQueue, _socket);
            _sendQueueListener = new SendQueueListenerClient(_sendingQueue, _socket);
            _receiveQueueListener = new ReceiveQueueListener(_moduleToNotificationHanderMap, _receivingQueue);
        }

        /// <summary>
        /// This function connects the client to the server. And starts all threads. The
        /// function arguments are needed only on the client side to connect to the server.
        /// </summary>
        /// <param name="serverIP"> IP Address of the server. Required only on client side. </param>
        /// <param name="serverPort"> Port no. of the server. Required only on client side. </param>
        /// <returns>
        ///  string "success" if success, "failure" if failure
        /// </returns>
        public string Start(string serverIP, string serverPort)
		{
            Trace.WriteLine("[Networking] CommunicatorClient.Start() function called.");
            try
			{
                // connect to the server
				_socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
				_socket.Connect(ParseIP(serverIP), int.Parse(serverPort));

                // start all threads
				_socketListener.Start();				
				_sendQueueListener.Start();
				_receiveQueueListener.Start();

                Trace.WriteLine("[Networking] CommunicatorClient started.");
                return "success";
			}
			catch(Exception e)
			{
				Trace.WriteLine($"[Networking] Error in CommunicatorClient.Start(): {e.Message}");
				return "failure";
			}
		}

        /// <summary>
        /// Stops all running threads, clears queues and closes the socket.
        /// </summary>
        /// <returns> void </returns>
        public void Stop()
		{
            Trace.WriteLine("[Networking] CommunicatorClient.Stop() function called.");
            try
            {
                // stop all threads
                _socketListener.Stop();
                _sendQueueListener.Stop();
                _receiveQueueListener.Stop();

                // clear the queues
                _sendingQueue.Clear();
                _receivingQueue.Clear();

                // if socket is connected then close the socket stream and socket
                if (_socket.Connected)
                {
                    _socket.GetStream().Close();
                    _socket.Close();
                }
                Trace.WriteLine("[Networking] CommunicatorClient stopped.");
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[Networking] Error in CommunicatorClient.Stop(): {e.Message}");
            }
        }

		/// <summary>
		/// This function parses the IP address given as string and returns the IPAddress.
		/// </summary>
		/// <param name="IPstring"> IP address string. </param>
		/// <returns>
		///  The "IPAddress" type IP address
		/// </returns>
		private static IPAddress ParseIP(string IPstring)
		{
            Trace.WriteLine("[Networking] CommunicatorClient.ParseIP() function called.");
            try
            {
				Trace.WriteLine($"[Networking] Parsing IP address: {IPstring}.");
				return IPAddress.Parse(IPstring);
			}
			catch
			{
				Trace.WriteLine($"[Networking] Parsing DNS name: {IPstring}.");
				return Dns.GetHostAddresses(IPstring).Last();
			}
		}

		/// <summary>
		/// This function is to be called only on the server.
		/// </summary>
		[ExcludeFromCodeCoverage]
		public void AddClient(string clientId, TcpClient socket)
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
        /// <param name="serializedData"> The serialized data to be sent. </param>
        /// <param name="moduleName"> Module sending the data. </param>
        /// <param name="destination"> Destination not required to send from client to server. </param>
        /// <returns> void </returns>
        public void Send(string serializedData, string moduleName, string? destination = null)
		{
            Trace.WriteLine("[Networking] CommunicatorClient.Send() function called.");
            Packet packet = new(serializedData, null, moduleName);
			_sendingQueue.Enqueue(packet);
			Trace.WriteLine($"[Networking] Enqueued packet in send queue of the module: {moduleName}");
		}

        /// <summary>
        /// Other modules can subscribe using this function to be notified when 
        /// data is received, and when a client joins, and when a client leaves.
        /// </summary>
        /// <param name="moduleName"> Name of the module. </param>
        /// <param name="notificationHandler"> Module implementation of the INotificationHandler. </param>
        /// <param name="isHighPriority"> Boolean telling whether module's data is high priority or low priority. </param>
        /// <returns> void </returns>
        public void Subscribe(string moduleName, INotificationHandler notificationHandler, bool isHighPriority)
		{
            Trace.WriteLine("[Networking] CommunicatorClient.Subscribe() function called.");
            _moduleToNotificationHanderMap.Add(moduleName, notificationHandler);
			_sendingQueue.RegisterModule(moduleName, isHighPriority); // sending queue implements priority queues
            Trace.WriteLine($"[Networking] Module: {moduleName} subscribed with priority is high: {isHighPriority}");
		}
	}
}
