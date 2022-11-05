/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of SendQueueListenerServer.
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlexShareNetwork.Sockets
{
	public class SendQueueListenerServer
	{
		// the thread which will be running
		private readonly Thread _thread;
		// boolean to tell whether thread is running or stopped
		private volatile bool _threadRun;

		// variable to store the send queue
		private readonly SendingQueue _sendingQueue;

		// variable to store the dictionary which maps clientIds to their respective sockets
		private readonly Dictionary<string, TcpClient> _clientIdToClientSocketMap;

		// variable to store the dictionary which maps module identifiers to their respective notification handlers
		private readonly Dictionary<string, INotificationHandler> _moduleToNotificationHandlerMap;

        // serializer object to serialize the packet to send
        readonly Serializer _serializer = new();

        /// <summary>
        /// It is the Constructor which initializes the queue, clientIdSocket and subscribedModules
        /// </summary>
        /// <param name="sendingQueue"> The sending queue. </param>
        /// <param name="clientIdToClientSocketMap"> The map from clientIds to their respective sockets. </param>
        /// <param name="moduleToNotificationHandlerMap">
        /// The dictionary which maps module identifiers to their respective notification handlers.
        /// </param>
        public SendQueueListenerServer(SendingQueue sendingQueue, Dictionary<string, TcpClient> clientIdToClientSocketMap,
			Dictionary<string, INotificationHandler> moduleToNotificationHandlerMap)
		{
            _sendingQueue = sendingQueue;
			_clientIdToClientSocketMap = clientIdToClientSocketMap;
			_moduleToNotificationHandlerMap = moduleToNotificationHandlerMap;
			_thread = new Thread(Listen); // the thread is only created and not started here
        }

		/// <summary>
		/// This function starts the thread.
		/// </summary>
		/// <returns> void </returns>
		public void Start()
		{
            Trace.WriteLine("[Networking] SendQueueListenerServer.Start() function called.");
            _threadRun = true;
			_thread.Start();
			Trace.WriteLine("[Networking] SendQueueListenerServer thread started.");
		}

		/// <summary>
		/// This function stops the thread.
		/// </summary>
		/// <returns> void </returns>
		public void Stop()
		{
            Trace.WriteLine("[Networking] SendQueueListenerServer.Stop() function called.");
            _threadRun = false;
            Trace.WriteLine("[Networking] SendQueueListenerServer thread stopped.");
		}

		/// <summary>
		/// This function listens to send queue and when some packet comes in the send queue then
		/// it sends the packet to the client corresponding to the destination field of the packet.
		///  The thread will be running this function.
		/// </summary>
		/// <returns> void </returns>
		private void Listen()
		{
            Trace.WriteLine("[Networking] SendQueueListenerServer.Listen() function called.");
            while (_threadRun)
			{
                _sendingQueue.WaitForPacket();
				Packet packet = _sendingQueue.Dequeue();
                string sendString = "BEGIN" + _serializer.Serialize(packet) + "END";

                // get the socket corresponding to the destination in the packet
                var clientSockets = GetClientIdToSocket(packet.destination);
				foreach (var clientSocket in clientSockets)
				{
					var bytes = Encoding.ASCII.GetBytes(sendString);
					try
					{
						// if client is connected then we send the data
						if (!(clientSocket.Client.Poll(1, SelectMode.SelectRead) && clientSocket.Client.Available == 0))
						{
                            clientSocket.Client.Send(bytes);
							Trace.WriteLine($"[Networking] Data sent from server to client by module: {packet.moduleOfPacket}.");
						}
						else // else the client is disconnected so we try to reconnect
						{
							Trace.WriteLine("[Networking] Client got disconnected. Trying to reconnect...");
                            Task.Run(() => TryReconnectingToClient(clientSocket, bytes, packet));
						}
					}
					catch (Exception e)
					{
						Trace.WriteLine($"[Networking] Error in SendQueueListenerServer thread: {e.Message}");
					}
				}
			}
		}

        /// <summary>
		/// This function tries to connect to the client 3 times and if connected then send the data
		/// to the client, otherwise it notifies all other modules that the client got disconnected.
		/// </summary>
        ///  /// <param name="clientSocket"> The socket object to send to client. </param>
        /// <param name="bytes"> The data that is to be sent. </param>
        /// <param name="packet"> The packet to check which module sent the data. </param>
		/// <returns> void </returns>
        private void TryReconnectingToClient(TcpClient clientSocket, byte[] bytes, Packet packet)
        {
            Trace.WriteLine("[Networking] SendQueueListenerServer.TryReconnectingToClient() function called.");
            try
            {
                var isSent = false;
                // try to reconnect 3 times
                for (var i = 0; i < 3 && !isSent; i++)
                {
                    Thread.Sleep(100);

                    // check if the client is now connected and send the data
                    if (!(clientSocket.Client.Poll(1, SelectMode.SelectRead) && clientSocket.Client.Available == 0))
                    {
                        Trace.WriteLine("[Networking] Client reconnected.");
                        clientSocket.Client.Send(bytes);
                        Trace.WriteLine($"[Networking] Data sent from server to client by {packet.moduleOfPacket}.");
                        isSent = true;
                    }
                }

                // if data was not send even after trying 3 times then client is disconnected
                if (!isSent)
                {
                    Trace.WriteLine("[Networking] Client has left. Removing client...");
                    var clientId = SocketToClientId(clientSocket);
                    foreach (var moduleToNotificationHandler in _moduleToNotificationHandlerMap)
                    {
                        moduleToNotificationHandler.Value.OnClientLeft(clientId);
                        Trace.WriteLine($"[Networking] Notifed module:{moduleToNotificationHandler.Key} that the client has left.");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[Networking] Error in SendQueueListenerServer thread: {e.Message}");

            }
        }

        /// <summary>
        /// This function returns the socket for the given destination/ClientId. If destination 
        /// is null then destination is broadcast, then it returns sockets of all clients.
        /// </summary>
        /// <param name="destination"> It is the client ID for unicast and null for boradcast. </param>
        /// <returns> Set of client sockets. </returns>
        private HashSet<TcpClient> GetClientIdToSocket(string destination)
		{
            Trace.WriteLine("[Networking] SendQueueListenerServer.DestinationToSocket() function called.");
            var clientSockets = new HashSet<TcpClient>();
			if (destination == null)
			{
                foreach (var clientIdToClientSocket in _clientIdToClientSocketMap)
                {
                    clientSockets.Add(clientIdToClientSocket.Value);
                }
			}
			else
			{
                clientSockets.Add(_clientIdToClientSocketMap[destination]);
			}
			return clientSockets;
		}

		/// <summary>
		/// This function returns the client ID corresponding to the given socket object.
		/// </summary>
		/// <param name="socket"> The socket object. </param>
		/// <returns> ClientId string. </returns>
		private string SocketToClientId(TcpClient socket)
		{
            Trace.WriteLine("[Networking] SendQueueListenerServer.SocketToClientId() function called.");
            foreach (var clientIdToClientSocket in _clientIdToClientSocketMap)
            {
                if (clientIdToClientSocket.Value == socket)
                {
                    return clientIdToClientSocket.Key;
                }
            }
			return null;
		}
	}
}
