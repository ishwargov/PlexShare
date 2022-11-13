/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the class definition of CommunicatorClient
/// which is the communicator for the client side.
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
		// initialize the sending queue and receiving queue
		private readonly SendingQueue _sendingQueue = new();
		private readonly ReceivingQueue _receivingQueue = new();

        // declare all the threads
        private readonly SocketListener _socketListener;
        private readonly SendQueueListenerClient
            _sendQueueListenerClient;
		private readonly ReceiveQueueListener _receiveQueueListener;

		// declare the socket variable for the client
        // this will be used for communication with the server
		private readonly TcpClient _clientSocket = new();

		// map to store the notification handlers of subscribed modules
		private readonly Dictionary<string, INotificationHandler>
            _moduleToNotificationHanderMap = new();

        /// <summary>
        /// Constructor initializes all threads.
        /// </summary>
        public CommunicatorClient()
        {
            // SocketListener listens to the socket and enqueues 
            // received packets in the receiving queue
            _socketListener = new SocketListener(
                _receivingQueue, _clientSocket);

            // SendQueueListenerClient listens to the sending queue and
            // sends the packets whenever they comes into the queue
            _sendQueueListenerClient = new SendQueueListenerClient(
                _sendingQueue, _clientSocket);

            // ReceiveQueueListener listens to the receiving queue
            // and notifies the respective module whenever data for
            // that module comes into the receiving queue
            _receiveQueueListener = new ReceiveQueueListener(
                _moduleToNotificationHanderMap, _receivingQueue);
        }

        /// <summary>
        /// Connects the client to the server. And starts all threads.
        /// The function arguments are needed only on the client side.
        /// </summary>
        /// <param name="serverIP">
        /// IP Address of the server. Required only on client side.
        /// </param>
        /// <param name="serverPort">
        /// Port no. of the server. Required only on client side.
        /// </param>
        /// <returns>
        /// string "success" if success, "failure" if failure.
        /// </returns>
        public string Start(string serverIP, string serverPort)
		{
            Trace.WriteLine("[Networking] CommunicatorClient.Start()" +
                " function called.");
            try
			{
                // connect to the server
                _clientSocket.Client.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.DontLinger, true);
                _clientSocket.Connect(
                    ParseIPstring(serverIP), int.Parse(serverPort));

                // start all threads
				_socketListener.Start();
                _sendQueueListenerClient.Start();
				_receiveQueueListener.Start();

                Trace.WriteLine("[Networking] CommunicatorClient" +
                    " started.");
                return "success";
			}
			catch(Exception e)
			{
				Trace.WriteLine("[Networking] Error in " +
                    "CommunicatorClient.Start(): " + e.Message);
				return "failure";
			}
		}

        /// <summary>
        /// Stops all threads, clears queues and closes the socket.
        /// </summary>
        /// <returns> void </returns>
        public void Stop()
		{
            Trace.WriteLine("[Networking] CommunicatorClient.Stop() " +
                "function called.");
            try
            {
                // stop all threads
                _socketListener.Stop();
                _sendQueueListenerClient.Stop();
                _receiveQueueListener.Stop();

                // clear the queues
                _sendingQueue.Clear();
                _receivingQueue.Clear();

                // if socket is connected then close it
                if (_clientSocket.Connected)
                {
                    _clientSocket.GetStream().Close();
                    _clientSocket.Close();
                }
                Trace.WriteLine("[Networking] CommunicatorClient " +
                    "stopped.");
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "CommunicatorClient.Stop(): " + e.Message);
            }
        }

        /// <summary>
        /// Parses IP address given as a string and 
        /// returns IP address of type "IPAddress".
        /// </summary>
        /// <param name="IPstring">
        /// IP address as a string.
        /// </param>
        /// <returns>
        /// IP address of type "IPAddress"
        /// </returns>
        private static IPAddress ParseIPstring(string IPstring)
		{
            Trace.WriteLine("[Networking] " +
                "CommunicatorClient.ParseIP() function called.");
            try
            {
				Trace.WriteLine("[Networking] Parsing IP address: " +
                    IPstring);
				return IPAddress.Parse(IPstring);
			}
			catch
			{
				Trace.WriteLine("[Networking] Parsing DNS name: " +
                    IPstring);
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
        /// Sends data to the server.
        /// </summary>
        /// <param name="serializedData">
        /// The serialized data to be sent to the server.
        /// </param>
        /// <param name="moduleName">
        /// Name of module sending the data.
        /// </param>
        /// <param name="destination">
        /// Not required on client side, give null.
        /// </param>
        /// <returns> void </returns>
        public void Send(string serializedData, string moduleName,
            string? destination = null)
		{
            Trace.WriteLine("[Networking] CommunicatorClient.Send() " +
                "function called.");
            Packet packet = new(serializedData, null, moduleName);
			bool isEnqueued = _sendingQueue.Enqueue(packet);
            if (isEnqueued)
            {
                Trace.WriteLine("[Networking] Enqueued packet in " +
                    "sending queue of the module: " + moduleName);
            }
            else
            {
                Trace.WriteLine("[Networking] Packet not enqueued " +
                    "in sending queue of the module: " + moduleName);
            }
		}

        /// <summary>
        /// Other modules can subscribe using this function to be able
        /// to send data. And be notified when data is received, and
        /// when a client joins, and when a client leaves.
        /// </summary>
        /// <param name="moduleName"> Name of the module. </param>
        /// <param name="notificationHandler"> 
        /// Module implementation of the INotificationHandler.
        /// </param>
        /// <param name="isHighPriority">
        /// Boolean telling whether module's data is high priority
        /// or low priority.
        /// </param>
        /// <returns> void </returns>
        public void Subscribe(string moduleName, INotificationHandler
            notificationHandler, bool isHighPriority)
		{
            Trace.WriteLine("[Networking] " +
                "CommunicatorClient.Subscribe() function called.");
            try
            {
                // store the notification handler of the module in our
                // map
                _moduleToNotificationHanderMap.Add(
                    moduleName, notificationHandler);
                
                // sending queue implements priority queues so we need
                // to register the priority of the module
                _sendingQueue.RegisterModule(
                    moduleName, isHighPriority);

                Trace.WriteLine("[Networking] Module: " + moduleName +
                    " subscribed with priority [True for high/False" +
                    "for low]: " + isHighPriority.ToString());
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "CommunicatorClient.Subscribe(): " + e.Message);
            }
        }
	}
}
