/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of SocketListener.
/// </summary>

using PlexShareNetworking.Queues;
using PlexShareNetworking.Serialization;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PlexShareNetworking.Sockets
{
	public class SocketListener
	{
		// max size of the send buffer
		private const int bufferSize = 1000000;
		// create the buffer
		private byte[] buffer = new byte[bufferSize];

		// object to store the the received message, StringBuilder type is mutable while string type is not
		private readonly StringBuilder _message = new();

        // serializer object to deserialize the received string
        Serializer serializer = new();

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
			_thread = new Thread(() => _socket.BeginReceive(buffer, 0, bufferSize, 0, ReceiveCallback, null));
			_threadRun = true;
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
					_message.Append(Encoding.ASCII.GetString(buffer, 0, bytesCount));
					string packets = _message.ToString();
					packets = ProcessPackets(packets); // process the packets received so far
					_message.Clear();
					_message.Append(packets); // append the remaining packets tring to message
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
		/// <param name="packets"> The string containing packets. </param>
		/// <returns> The remaining packets string after processing the packets from the string. </returns>
		private string ProcessPackets(string packets)
		{
			if (packets.Length < 12)
			{
				return packets;
			}
			var isPacket = false;
			do
			{
				var firstFlagIndex = packets.IndexOf("[FLAG]", StringComparison.Ordinal);
				var nextFlagIndex = packets.IndexOf("[FLAG]", firstFlagIndex + 5, StringComparison.Ordinal);
				while (!isPacket && nextFlagIndex != -1)
				{
					// if last flag has an escape before it then it is not the last flag, so find the flag after it
					if (packets[(nextFlagIndex - 5)..nextFlagIndex] == "[ESC]")
					{
						nextFlagIndex = packets.IndexOf("[FLAG]", nextFlagIndex + 6, StringComparison.Ordinal);
						continue;
					}
					isPacket = true;
				}
				if (isPacket)
				{
					string packetString = packets[(firstFlagIndex + 6)..nextFlagIndex];
					packets = packets[(nextFlagIndex + 6)..]; // remove the first packet from the packets string
					packetString = packetString.Replace("[ESC][ESC]", "[ESC]");
					packetString = packetString.Replace("[ESC][FLAG]", "[FLAG]");
                    Packet packet = serializer.Deserialize<Packet>(packetString);
                    Trace.WriteLine($"[Networking] Received data from module {packet._moduleOfPacket}.");
                    _queue.Enqueue(packet);
                }
			} while(isPacket && packets.Length > 12);
			return packets; // return the remaining packets string
		}
	}
}
