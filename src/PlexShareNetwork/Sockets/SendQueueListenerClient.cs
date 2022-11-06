/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of SendQueueListenerClient.
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlexShareNetwork.Sockets
{
	public class SendQueueListenerClient
	{
		// the thread which will be running
		private Thread _thread;
		// boolean to tell whether thread is running or stopped
		private volatile bool _threadRun;

		// variable to store the send queue
		private readonly SendingQueue _sendingQueue;

		// variable to store the socket
		private readonly TcpClient _clientSocket;

        // serializer object to serialize the packet to send
        readonly Serializer _serializer = new();

        /// <summary>
        /// It is the Constructor which initializes the queue and socket
        /// </summary>
        /// <param name="sendingQueue"> The the send queue. </param>
        /// <param name="clientSocket"> The socket object to send the data. </param>
        public SendQueueListenerClient(SendingQueue sendingQueue, TcpClient clientSocket)
		{
            _sendingQueue = sendingQueue;
            _clientSocket = clientSocket;
        }

		/// <summary>
		/// This function starts the thread.
		/// </summary>
		/// <returns> void </returns>
		public void Start()
		{
            Trace.WriteLine("[Networking] SendQueueListenerClient.Start() function called.");
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
            Trace.WriteLine("[Networking] SendQueueListenerClient.Stop() function called.");
            _threadRun = false;
            Trace.WriteLine("[Networking] SendQueueListenerClient thread stopped.");
		}

		/// <summary>
		/// This function listens to the send queue and when some packet comes in the queue then
		/// it sends the packet to the server. The thread will be running this function.
		/// </summary>
		/// <returns> void </returns>
		private void Listen()
		{
            Trace.WriteLine("[Networking] SendQueueListenerClient.Listen() function called.");
            while (_threadRun)
			{
                _sendingQueue.WaitForPacket();
				Packet packet = _sendingQueue.Dequeue();
                string sendString = "BEGIN" + _serializer.Serialize(packet).Replace("END", "NOTEND") + "END";
                try
				{
                    _clientSocket.Client.Send(Encoding.ASCII.GetBytes(sendString));
					Trace.WriteLine($"[Networking] SendQueueListenerClient. Data sent from client to server by module: {packet.moduleOfPacket}.");
				}
				catch (Exception e)
				{
					Trace.WriteLine($"[Networking] SendQueueListenerClient. Error in sending data: {e.Message}");
				}
			}
		}
	}
}
