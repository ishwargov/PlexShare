/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of CommunicatorServer.
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Networking
{
	internal class CommunicatorServer : ICommunicator
	{
		// initialize the send queue and receive queue
		private readonly Queue _sendQueue = new();
		private readonly Queue _receiveQueue = new();

		// declate the variable of SendQueueListenerClient class
		private SendQueueListenerServer _sendQueueListener;

		// declate the variable of ReceiveQueueListener class
		private ReceiveQueueListener = _receiveQueueListener;

		// declare the server socket variable
		private TcpListener _socket;

		// map to store the sockets of the clients to be able to send data to a speicific client
		private readonly Dictionary<string, TcpClient> _clientIdToSocket = new();

		// this map will store the socket listeners, one socket listener listening to one client
		private readonly Dictionary<string, SocketListener> _clientIdToSocketListeners = new();

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
		string ICommunicator.Start(string serverIP = null, string serverPort = null)
		{

		}

		/// <summary>
		/// Stops listening and stops all running threads.
		/// </summary>
		/// <returns> void </returns>
		void ICommunicator.Stop()
		{

		}

		/// <summary>
		/// This function is to be called only on the server when a new cilent joins.
		/// It adds the client socket to the map and starts listening to the client.
		/// </summary>
		/// <typeparam name="T"> socket </typeparam>
		/// <param name="clientId"> The client Id. </param>
		/// <param name="socketObject"> The socket object of the client. </param>
		/// <returns> void </returns>
		void ICommunicator.AddClient<T>(string clientId, T socket)
		{

		}

		/// <summary>
		/// This function is to be called only on the server when a client is leaves.
		/// It removes the client information from the server.
		/// </summary>
		/// <param name="clientId"> The client Id. </param>
		/// <returns> void </returns>
		void ICommunicator.RemoveClient(string clientId)
		{

		}

		/// <summary>
		/// This function broadcasts data to all the clients.
		/// </summary>
		/// <param name="serializedData"> The serialzed data to be sent over the network. </param>
		/// <param name="moduleIdentifier"> Module Identifier of the module. </param>
		/// <returns> void </returns>
		void ICommunicator.Send(string serializedData, string moduleIdentifier)
		{

		}

		/// <summary>
		/// Function to send data to a specific client given by the destination argument.
		/// This function is to be called only on the server side.
		/// </summary>
		/// <param name="serializedData"> The serialzed data to be sent over the network. </param>
		/// <param name="moduleIdentifier"> Module Identifier of the module. </param>
		/// <param name="destination"> The destination or client Id to which to send the data. </param>
		/// <returns> void </returns>
		void Send(string serializedData, string moduleIdentifier, string destination)
		{

		}

		/// <summary>
		/// Other modules can subscribe using this function to be notified on receiving data over the network.
		/// </summary>
		/// <param name="moduleIdentifier"> Module Identifier of the module. </param>
		/// <param name="handler"> Module implementation of the INotificationHandler. </param>
		/// <param name="isHighPriority"> Boolean which tells whether data is high priority or low priority. </param>
		/// <returns> void </returns>
		void ICommunicator.Subscribe(string moduleIdentifier, INotificationHandler notificationHandler, bool isHighPriority)
		{

		}
	}
}
