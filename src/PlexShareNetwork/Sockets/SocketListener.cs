/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of SocketListener.
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PlexShareNetwork.Sockets
{
	public class SocketListener
	{
		// max size of the send buffer
		private const int bufferSize = 1000000;
		// create the buffer
		private readonly byte[] buffer = new byte[bufferSize];

		// object to store the the received message, StringBuilder type is mutable while string type is not
		private readonly StringBuilder _receivedString = new();

        // serializer object to deserialize the received string
        readonly Serializer _serializer = new();

        // the thread which will be running
        public Thread _thread;
		// boolean to tell whether the thread is running or stopped
		public volatile bool _threadRun;

		// variable to store the receive queue
		private readonly ReceivingQueue _queue;

		// variable to store the socket
		private readonly Socket _socket;

		/// <summary>
		/// It is the Constructor which initializes the queue and socket variables.
		/// </summary>
		/// <param name="queue"> The receive queue. </param>
		/// <param name="socket"> The socket on which to listen. </param>
		public SocketListener(ReceivingQueue queue, TcpClient socket)
		{
			_queue = queue;
			_socket = socket.Client;
		}

		/// <summary>
		/// This function starts the thread.
		/// </summary>
		/// <returns> void </returns>
		public void Start()
		{
			_threadRun = true;
			_thread = new Thread(() => _socket.BeginReceive(buffer, 0, bufferSize, 0, ReceiveCallback, null));
			_thread.Start();
			Trace.WriteLine("[Networking] SocketListener thread started.");
		}

		/// <summary>
		/// This menthod stops the thread.
		/// </summary>
		/// <returns> void </returns>
		public void Stop()
		{
			_threadRun = false;
			Trace.WriteLine("[Networking] SocketListener thread stopped.");
		}

		/// <summary>
		/// This menthod is the AsyncCallback function passed to socket.BeginReceive() as an argument.
		/// </summary>
		/// <returns> void </returns>
		private void ReceiveCallback(IAsyncResult ar)
		{
			if (!_threadRun)
			{
				return;
			}
			try
			{
				int bytesCount = _socket.EndReceive(ar);
				if (bytesCount > 0)
				{
					_receivedString.Append(Encoding.ASCII.GetString(buffer, 0, bytesCount));
					string remainingString = ProcessReceivedString(_receivedString.ToString());
					_receivedString.Clear();
					_receivedString.Append(remainingString);
				}
				_socket.BeginReceive(buffer, 0, bufferSize, 0, ReceiveCallback, null);
			}
			catch (Exception e)
			{
				Trace.WriteLine($"[Networking] Error in SocketListener thread: {e.Message}");
			}
		}

        /// <summary>
        /// This menthod processes the packets from the given packets string, and
        /// removes the escape characters from each packet and calls the EnqueuePacket() function to enqueue the packet.
        /// </summary>
        /// <param name="receivedString"> The string containing packets. </param>
        /// <returns> The remaining string string after processing the packets from the string. </returns>
        private string ProcessReceivedString(string receivedString)
		{
            int packetBeginIndex = receivedString.IndexOf("BEGIN", StringComparison.Ordinal) + 5;
            int packetEndIndex = receivedString.IndexOf("END", StringComparison.Ordinal);
            while (packetBeginIndex != -1 && packetEndIndex != -1)
            {
                Packet packet = _serializer.Deserialize<Packet>(receivedString[packetBeginIndex..packetEndIndex]);
                receivedString = receivedString[(packetEndIndex + 3)..]; // remove the first packet from the packets string
                Trace.WriteLine($"[Networking] Received data from module {packet.moduleOfPacket}.");
                _queue.Enqueue(packet);
                packetBeginIndex = receivedString.IndexOf("BEGIN", StringComparison.Ordinal) + 5;
                packetEndIndex = receivedString.IndexOf("END", StringComparison.Ordinal);
            }
			return receivedString; // return the remaining packets string
		}
	}
}
