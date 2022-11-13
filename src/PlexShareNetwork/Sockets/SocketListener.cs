/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the class definition of SocketListener.
/// </summary>

using PlexShareNetwork.Queues;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PlexShareNetwork.Sockets
{
	public class SocketListener
	{
		// set the max size of buffer and initialize the buffer
		private const int bufferSize = 1000000;
		private readonly byte[] buffer = new byte[bufferSize];

        // StringBuilder string to store the the received message
        // StringBuilder type is mutable while string type is not
        private readonly StringBuilder _receivedString = new();

        // the thread which will be running
        private readonly Thread _socketListenerThread;
		// boolean to tell whether the thread is running or stopped
		private bool _runSocketListenerThread;

		// declare the receiving queue and socket
		private readonly ReceivingQueue _receivingQueue;
		private readonly Socket _socket;

        /// <summary>
        /// Constructor initializes the receiving queue and socket,
        /// and the socket listener thread.
        /// </summary>
        /// <param name="receivingQueue"> The receiving queue. </param>
        /// <param name="socket">
        /// The socket on which to listen.
        /// </param>
        public SocketListener(ReceivingQueue receivingQueue, 
            TcpClient socket)
		{
            _receivingQueue = receivingQueue;
			_socket = socket.Client;

            // initialize the thread to listen to the socket
            // the thread is only initialized here and not started
            _socketListenerThread = new Thread(() => 
            _socket.BeginReceive(buffer, 0, bufferSize, 0,
            ReceiveCallback, null));
		}

        /// <summary>
        /// Starts the socket listener thread.
        /// </summary>
        /// <returns> void </returns>
        public void Start()
		{
            Trace.WriteLine("[Networking] SocketListener.Start() " +
                "function called.");
            try
            {
                _runSocketListenerThread = true;
                _socketListenerThread.Start();
                Trace.WriteLine("[Networking] SocketListener thread " +
                    "started.");
            }
            catch(Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "SocketListener.Start(): " + e.Message);
            }
		}

        /// <summary>
        /// This function stops the thread.
        /// </summary>
        /// <returns> void </returns>
        public void Stop()
		{
            Trace.WriteLine("[Networking] SocketListener.Stop()" +
                " function called.");
            _runSocketListenerThread = false;
			Trace.WriteLine("[Networking] SocketListener thread " +
                "stopped.");
		}

        /// <summary>
        /// This is the AsyncCallback function which is passed to
        /// socket.BeginReceive() as an argument. It reads the 
        /// received bytes, converts them back to string and calls
        /// ProcessReceivedString() to process the received string.
        /// </summary>
        /// <returns> void </returns>
        private void ReceiveCallback(IAsyncResult ar)
		{
            Trace.WriteLine("[Networking] " +
                "SocketListener.ReceiveCallback() function called.");

            // if _runSocketListenerThread is set to false then this if
            // block will not run and we will just return
            if (_runSocketListenerThread)
            {
                try
                {
                    // get the count of received bytes and if its
                    // greater than 0 then process the received bytes
                    int bytesCount = _socket.EndReceive(ar);
                    if (bytesCount > 0)
                    {
                        // covert the received bytes to string and
                        // append that string to _receivedString
                        _receivedString.Append(
                            Encoding.ASCII.GetString(
                                buffer, 0, bytesCount));

                        // call ProcessReceivedString() to process the
                        // received string and get the remaning string
                        string remainingString = ProcessReceivedString(
                            _receivedString.ToString());

                        // clear the received strign and append the
                        // remaining string to it
                        _receivedString.Clear();
                        _receivedString.Append(remainingString);
                    }
                    _socket.BeginReceive(buffer, 0, bufferSize, 0,
                        ReceiveCallback, null);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("[Networking] Error in " +
                        "SocketListener.ReceiveCallback(): " +
                        e.Message);
                }
            }
		}

        /// <summary>
        /// Processes the packets from the given string,
        /// and enqueues the packets to receiving queue.
        /// </summary>
        /// <param name="receivedString">
        /// The string received from the network.
        /// </param>
        /// <returns>
        /// The remaining string after processing the given string.
        /// </returns>
        private string ProcessReceivedString(string receivedString)
		{
            Trace.WriteLine("[Networking] SocketListener." +
                "ProcessReceivedString() function called.");
            while (true)
            {
                // find index of "BEGIN" string in the received string
                // "BEGIN" marks the beginning of the packet
                int packetBegin = receivedString.IndexOf(
                    "BEGIN", StringComparison.Ordinal);

                // find index of "END" string in the received string
                // "END" marks the end of the packet
                int packetEnd = receivedString.IndexOf(
                    "END", StringComparison.Ordinal);

                // while index of "END" is not -1 and the "END" was
                // actually "NOTEND" then find the next "END"
                while (packetEnd != -1 && receivedString[
                    (packetEnd - 3)..(packetEnd + 3)] == "NOTEND")
                {
                    // find the index of next "END"
                    packetEnd = receivedString.IndexOf("END", 
                        packetEnd + 3, StringComparison.Ordinal);
                }

                // if packet end was not found that means we have not
                // yet received the full packet, so break
                if (packetEnd == -1)
                {
                    break;
                }
                // the below code will run only if we dont break in 
                // the above if statement, that is when we have
                // found the packetEnd

                // get the packet string from the received string and
                // convert it back to packet and enqueue the packet
                // to receiving queue
                string packetString = receivedString[
                    packetBegin..(packetEnd + 3)];
                Packet packet = PacketString.PacketStringToPacket(
                    packetString);
                _receivingQueue.Enqueue(packet);

                // remove the first packet from the received string
                receivedString = receivedString[(packetEnd + 3)..];

                Trace.WriteLine("[Networking] SocketListener:" +
                    " Received data from module: "
                    + packet.moduleOfPacket);
            }
            Trace.WriteLine("[Networking] SocketListener." +
                "ProcessReceivedString() function exited.");
            return receivedString; // return the remaining string
		}
	}
}
