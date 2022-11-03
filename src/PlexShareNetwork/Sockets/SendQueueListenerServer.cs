/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of SendQueueListenerServer.
/// </summary>

using PlexShareNetworking.Queues;
using PlexShareNetworking.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlexShareNetworking.Sockets
{
	public class SendQueueListenerServer
	{
		// the thread which will be running
		private Thread _thread;
		// boolean to tell whether thread is running or stopped
		private volatile bool _threadRun;

		// variable to store the send queue
		private readonly SendingQueues _queue;

		// variable to store the dictionary which maps clientIds to their respective sockets
		private readonly Dictionary<string, TcpClient> _clientIdToSocket;

		// variable to store the dictionary which maps module identifiers to their respective notification handlers
		private readonly Dictionary<string, INotificationHandler> _subscribedModules;

		/// <summary>
		/// It is the Constructor which initializes the queue, clientIdSocket and subscribedModules
		/// </summary>
		/// <param name="queue"> The the send queue. </param>
		/// <param name="clientIdToSocket"> The dictionary which maps clientIds to their respective sockets. </param>
		/// <param name="subscribedModules">
		/// The dictionary which maps module identifiers to their respective notification handlers.
		/// </param>
		public SendQueueListenerServer(SendingQueues queue, Dictionary<string, TcpClient> clientIdToSocket,
			Dictionary<string, INotificationHandler> subscribedModules)
		{
			_queue = queue;
			_clientIdToSocket = clientIdToSocket;
			_subscribedModules = subscribedModules;
		}

		/// <summary>
		/// This function starts the thread.
		/// </summary>
		/// <returns> void </returns>
		public void Start()
		{
			_thread = new Thread(Listen);
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
			while (_threadRun)
			{
				_queue.WaitForPacket();
				while (!_queue.IsEmpty())
				{
					var packet = _queue.Dequeue();

                    /// we put flag string at the start and end of the packet, and we need to put
                    /// escape string before the flag and escape strings which are in the packet
                    Serializer serializer = new();
                    var pkt = serializer.Serialize(packet); 
                    pkt = pkt.Replace("[ESC]", "[ESC][ESC]");
					pkt = pkt.Replace("[FLAG]", "[ESC][FLAG]");
					pkt = "[FLAG]" + pkt + "[FLAG]";

					// get the socket corresponding to the destination in the packet
					var sockets = DestinationToSocket(packet);
					foreach (var socket in sockets)
					{
						var bytes = Encoding.ASCII.GetBytes(pkt);
						try
						{
							var client = socket.Client;

							// if client is connected then we can send the data
							if (!(client.Poll(1, SelectMode.SelectRead) && client.Available == 0))
							{
								client.Send(bytes);
								Trace.WriteLine($"[Networking] Data sent from server to client by {packet.getModuleOfPacket()}.");
							}
							else // else the client is disconnected so we try to reconnect 3 times
							{
								Trace.WriteLine("[Networking] Client lost connection. Retrying ...");

								// the work performed by a Task object typically executes asynchronously on a
								// thread pool thread rather than synchronously on the main application thread
								_ = Task.Run(() =>
								{
									var socketTry = socket;
									var bytesTry = bytes;
									var clientTry = socketTry.Client;
									var isSent = false;
									try
									{
										// try to reconnect 3 times
										for (var i = 0; i < 3; i++)
										{
											Thread.Sleep(100); // the time is in milliseconds

											// check if the client is now connected and send the data
											if (!(clientTry.Poll(1, SelectMode.SelectRead) && clientTry.Available == 0))
											{
												Trace.WriteLine("[Networking] Client connected.");
												clientTry.Send(bytesTry);
												Trace.WriteLine($"[Networking] Data sent from server to client by {packet.getModuleOfPacket()}.");
												isSent = true; // after sending the data we can set isSent to true
												break;
											}
										}

										// if data was not send even after trying 3 more times then client is disconnected
										if (isSent == false)
										{
											Trace.WriteLine("[Networking] Client disconnected. Removing client ...");
											var clientId = SocketToClientId(socketTry);

											// call the OnClientLeft() handler of each subscribed module
											foreach (var module in _subscribedModules)
											{
												if (clientId != null)
												{
													module.Value.OnClientLeft(clientId);
												}
												else
												{
													Trace.WriteLine("[Networking] Client ID not present.");
												}
											}
										}
									}
									catch (Exception e)
									{
										Trace.WriteLine($"[Networking] Error in SendQueueListenerServer thread: {e.Message}");

									}
								});
							}
						}
						catch (Exception e)
						{
							Trace.WriteLine($"[Networking] Error in SendQueueListenerServer thread: {e.Message}");
						}
					}
				}
			}
		}

		/// <summary>
		/// This function gets the destination from the packet and returns the respective socket for that client.
		/// If the destination is null then its a broadcast packet, then it returns sockets of all clients.
		/// </summary>
		/// <param name="packet"> The packet which contains the destination. </param>
		/// <returns> Set of sockets. </returns>
		private HashSet<TcpClient> DestinationToSocket(Packet packet)
		{
			var sockets = new HashSet<TcpClient>();
			if (packet.getDestination() == null)
			{
				foreach (var keyValue in _clientIdToSocket) sockets.Add(keyValue.Value);
			}
			else
			{
				sockets.Add(_clientIdToSocket[packet.getDestination()]);
			}
			return sockets;
		}

		/// <summary>
		/// This function returns the client ID corresponding to the given socket object.
		/// </summary>
		/// <param name="socket"> The socket object. </param>
		/// <returns> ClientId string. </returns>
		private string SocketToClientId(TcpClient socket)
		{
			foreach (var clientId in _clientIdToSocket.Keys)
			{
				if (_clientIdToSocket[clientId] == socket)
				{
					return clientId;
				}
			}
			return null;
		}	
	}
}
