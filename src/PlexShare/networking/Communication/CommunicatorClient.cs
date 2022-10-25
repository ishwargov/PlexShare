/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of CommunicatorClient.
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
	internal class CommunicatorClient : ICommunicator
	{
		// initialize the send queue and receive queue
		private readonly Queue _sendQueue = new();
		private readonly Queue _receiveQueue = new();

		// declate the variable of SendQueueListenerClient class
		private SendQueueListenerClient _sendQueueListener;

		// declate the variable of ReceiveQueueListener class
		private ReceiveQueueListener = _receiveQueueListener;

		// declare the socket variable for the client
		private TcpClient = _socket;

		// declate the variable of SocketListener class
		private SocketListener _socketListener;

		// map to store the handlers of subscribed modules
		private readonly Dictionary<string, INotificationHandler> _subscribedModulesToHandler = new();

		/// <summary>
		/// This function connects the client to the server.
		/// And initializes queues and sockets.
		/// </summary>
		/// <param name="serverIP"> IP Address of the server. Required only on client side. </param>
		/// <param name="serverPort"> Port no. of the server. Required only on client side. </param>
		/// <returns>
		///  string "1" if success, "0" if failure
		/// </returns>
		string ICommunicator.Start(string serverIP, string serverPort)
		{

		}

		/// <summary>
		/// Disconnects from the server and stops all running threads.
		/// </summary>
		/// <returns> void </returns>
		void ICommunicator.Stop()
		{

		}

		/// <summary>
		/// This function is to be called only on the server.
		/// </summary>
		[ExcludeFromCodeCoverage]
		void ICommunicator.AddClient<T>(string clientId, T socket)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// This function is to be called only on the server.
		/// </summary>
		[ExcludeFromCodeCoverage]
		void ICommunicator.RemoveClient(string clientId)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// This function sends data to the server.
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
		[ExcludeFromCodeCoverage]
		void Send(string serializedData, string moduleIdentifier, string destination)
		{
			throw new NotSupportedException();
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