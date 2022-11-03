/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of SendQueueListenerClient.
/// </summary>

using Networking.Queues;
using Networking.Serialization;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Networking
{
	public class SendQueueListenerClient
	{
		// the thread which will be running
		private Thread _thread;
		// boolean to tell whether thread is running or stopped
		private volatile bool _threadRun;

		// variable to store the send queue
		private readonly SendingQueues _sendQueue;

		// variable to store the socket
		private readonly TcpClient _socket;

        // serializer object to serialize the packet to send
        readonly Serializer _serializer = new();

        /// <summary>
        /// It is the Constructor which initializes the queue and socket
        /// </summary>
        /// <param name="queue"> The the send queue. </param>
        /// <param name="socket"> The socket to send the data. </param>
        public SendQueueListenerClient(SendingQueues sendQueue, TcpClient socket)
		{
            _sendQueue = sendQueue;
			_socket = socket;
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
			Trace.WriteLine("[Networking] SendQueueListenerClient thread started.");
		}

		/// <summary>
		/// This function stops the thread.
		/// </summary>
		/// <returns> void </returns>
		public void Stop()
		{
			_threadRun = false;
			Trace.WriteLine("[Networking] SendQueueListenerClient thread stopped.");
		}

		/// <summary>
		/// This function listens to send queue and when some packet comes in the send queue then
		/// it sends the packet to the server. The thread will be running this function.
		/// </summary>
		/// <returns> void </returns>
		private void Listen()
		{
			while (_threadRun)
			{
                _sendQueue.WaitForPacket();
				while (!_sendQueue.IsEmpty())
				{
					Packet packet = _sendQueue.Dequeue();
                    string sendString = "BEGIN" + _serializer.Serialize(packet) + "END";
					try
					{
						_socket.Client.Send(Encoding.ASCII.GetBytes(sendString));
						Trace.WriteLine($"[Networking] Data sent from client to server by module {packet.getModuleOfPacket()}.");
					}
					catch (Exception e)
					{
						Trace.WriteLine($"[Networking] Error in SendQueueListenerClient thread: {e.Message}");
					}
				}
			}
		}
	}
}
