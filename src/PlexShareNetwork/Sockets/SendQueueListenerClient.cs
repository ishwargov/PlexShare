/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the class definition of SendQueueListenerClient.
/// </summary>

using PlexShareNetwork.Queues;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PlexShareNetwork.Sockets
{
    public class SendQueueListenerClient
    {
        // the thread which will be running
        private Thread _sendQueueListenerThread;
        // boolean to tell whether thread is running or stopped
        private bool _runSendQueueListenerThread;

        // declare the sending queue and client socket
        private readonly SendingQueue _sendingQueue;
        private readonly TcpClient _clientSocket;

        /// <summary>
        /// Constructor initializes the queue and socket.
        /// </summary>
        /// <param name="sendingQueue">
        /// The the sending queue.
        /// </param>
        /// <param name="clientSocket">
        /// The socket object which is connected to the server.
        /// </param>
        public SendQueueListenerClient(SendingQueue sendingQueue,
            TcpClient clientSocket)
        {
            _sendingQueue = sendingQueue;
            _clientSocket = clientSocket;
        }

        /// <summary>
        /// Starts the send queue listener client thread.
        /// </summary>
        /// <returns> void </returns>
        public void Start()
        {
            Trace.WriteLine("[Networking] " +
                "SendQueueListenerClient.Start() function called.");
            _runSendQueueListenerThread = true;
            _sendQueueListenerThread = new Thread(Listen);
            _sendQueueListenerThread.Start();
            Trace.WriteLine("[Networking] SendQueueListenerClient " +
                "thread started.");
        }

        /// <summary>
        /// Stops the send queue listener client thread.
        /// </summary>
        /// <returns> void </returns>
        public void Stop()
        {
            Trace.WriteLine("[Networking] " +
                "SendQueueListenerClient.Stop() function called.");
            _runSendQueueListenerThread = false;
            _sendQueueListenerThread.Join();
            Trace.WriteLine("[Networking] SendQueueListenerClient " +
                "thread stopped.");
        }

        /// <summary>
        /// Listens to the send queue and when some packet comes
        /// in the queue then it sends the packet to the server.
        /// </summary>
        /// <returns> void </returns>
        private void Listen()
        {
            Trace.WriteLine("[Networking] " +
                "SendQueueListenerClient.Listen() function called.");
            while (_runSendQueueListenerThread)
            {
                // keep waiting while boolean to run the thread is
                // set to true and sending queue is empty
                while (_runSendQueueListenerThread && 
                    _sendingQueue.IsEmpty())
                {
                    Thread.Sleep(100);
                }
                // if boolean to run the thread is set to false
                // then break
                if (_runSendQueueListenerThread == false)
                {
                    break;
                }

                Packet packet = _sendingQueue.Dequeue();

                // convert packet to string as string can be sent
                string packetString =
                    PacketString.PacketToPacketString(packet);

                // convert the string to bytes and send the bytes
                byte[] bytes = Encoding.UTF32.GetBytes(packetString);
                try
                {
                    _clientSocket.Client.Send(bytes);
                    Trace.WriteLine("[Networking] SendQueueListener" +
                        "Client: Data sent from client to server by" +
                        " module: " + packet.moduleOfPacket);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("[Networking] Error in " +
                        "SendQueueListenerClient.Listen(): " +
                        e.Message);
                }
            }
        }
    }
}
