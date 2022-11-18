/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the class definition of SendQueueListenerServer.
/// </summary>

using PlexShareNetwork.Queues;
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
        private Thread _sendQueueListenerThread;
        // boolean to tell whether thread is running or stopped
        private bool _runSendQueueListenerThread;

        // declare the sending queue
        private readonly SendingQueue _sendingQueue;

        // map of clientId to socket connected to the client
        private readonly Dictionary<string, TcpClient> 
            _clientIdToClientSocketMap;

        // map of module name to the module's notification handlers
        private readonly Dictionary<string, INotificationHandler> 
            _moduleToNotificationHandlerMap;

        /// <summary>
        /// Constructor initializes the sending queue and the clientId
        /// to client socket map, and the module to notification 
        /// handler map, and the send queue listener thread
        /// </summary>
        /// <param name="sendingQueue"> The sending queue. </param>
        /// <param name="clientIdToClientSocketMap">
        /// The map of clientId to socket connected to the client.
        /// </param>
        /// <param name="moduleToNotificationHandlerMap">
        /// Map of module name to the module's notification handlers.
        /// </param>
        public SendQueueListenerServer(SendingQueue sendingQueue, 
            Dictionary<string, TcpClient> clientIdToClientSocketMap,
            Dictionary<string, INotificationHandler> 
            moduleToNotificationHandlerMap)
        {
            _sendingQueue = sendingQueue;
            _clientIdToClientSocketMap = clientIdToClientSocketMap;
            _moduleToNotificationHandlerMap = 
                moduleToNotificationHandlerMap;
        }

        /// <summary>
        /// Starts the send queue listener server thread.
        /// </summary>
        /// <returns> void </returns>
        public void Start()
        {
            Trace.WriteLine("[Networking] " +
                "SendQueueListenerServer.Start() function called.");
            _runSendQueueListenerThread = true;
            _sendQueueListenerThread = new Thread(Listen);
            _sendQueueListenerThread.Start();
            Trace.WriteLine("[Networking] SendQueueListenerServer " +
                "thread started.");
        }

        /// <summary>
        /// Stops the send queue listener server thread.
        /// </summary>
        /// <returns> void </returns>
        public void Stop()
        {
            Trace.WriteLine("[Networking] " +
                "SendQueueListenerServer.Stop() function called.");
            _runSendQueueListenerThread = false;
            _sendQueueListenerThread.Join();
            Trace.WriteLine("[Networking] SendQueueListenerServer " +
                "thread stopped.");
        }

        /// <summary>
        /// Listens to send queue and when some packet comes in the
        /// send queue then it calls the SendDataToClient() function
        /// to send the packet to the client(s) corresponding to the 
        /// destination field of the packet.
        /// </summary>
        /// <returns> void </returns>
        private void Listen()
        {
            Trace.WriteLine("[Networking] " +
                "SendQueueListenerServer.Listen() function called.");
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
                byte[] bytes = Encoding.ASCII.GetBytes(packetString);

                // if destination is not null then destination contains
                // a client Id so send data to that particular client
                if (packet.destination != null)
                {
                    SendDataToClient(packet.destination, bytes,
                        packet.moduleOfPacket);
                }
                else
                {
                    // destination is null, so send the data to all
                    // clients
                    foreach (string clientId in
                        _clientIdToClientSocketMap.Keys)
                    {
                        SendDataToClient(clientId, bytes,
                            packet.moduleOfPacket);
                    }
                }
            }
        }

        /// <summary>
        /// Sends the data given by bytes to client given by the
        /// clientId. If the client is not reachable then it calls
        /// the TryReconnectingToClient() function.
        /// </summary>
        /// <param name="clientId"> The Client Id. </param>
        /// <param name="bytes"> The data that is to be sent. </param>
        /// <param name="module"> The module sending the data. </param>
        /// <returns> void </returns>
        private void SendDataToClient(string clientId, 
            byte[] bytes, string module)
        {
            Trace.WriteLine("[Networking] SendQueueListenerServer." +
                "SendDataToClient() function called.");
            try
            {
                TcpClient clientSocket = 
                    _clientIdToClientSocketMap[clientId];
    
                // check if the client is connected then send data
                if (!(clientSocket.Client.Poll(
                    1, SelectMode.SelectRead)
                    && clientSocket.Client.Available == 0))
                {
                    clientSocket.Client.Send(bytes);
                    Trace.WriteLine("[Networking] Data sent from " +
                        "server to client: " + clientId +
                        " by module: " + module);
                }
                else 
                {
                    // the client is disconnected so try to reconnect
                    Trace.WriteLine("[Networking] Client: " + clientId
                        + " got disconnected. Trying to reconnect...");
                    Task.Run(() => TryReconnectingToClient(
                        clientId, bytes, module));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "SendQueueListenerServer.SendDataToClient(): " +
                    e.Message);
            }
        }

        /// <summary>
        /// Tries to connect to the client 3 times and if connected 
        /// then sends the data to the client, otherwise it notifies
        /// all subscribed modules that the client has left.
        /// </summary>
        /// <param name="clientId"> The client Id. </param>
        /// <param name="bytes"> The data that is to be sent. </param>
        /// <param name="module"> The module sending the data. </param>
        /// <returns> void </returns>
        private void TryReconnectingToClient(string clientId,
            byte[] bytes, string module)
        {
            Trace.WriteLine("[Networking] SendQueueListenerServer." +
                "TryReconnectingToClient() function called.");
            try
            {
                TcpClient clientSocket =
                    _clientIdToClientSocketMap[clientId];
                var isSent = false;
                // try to reconnect 3 times
                for (var i = 0; i < 3 && !isSent; i++)
                {
                    // wait for some time for client to reconnect
                    Thread.Sleep(100);

                    // if client is now connected then send the data
                    if (!(clientSocket.Client.Poll(
                        1, SelectMode.SelectRead)
                        && clientSocket.Client.Available == 0))
                    {
                        Trace.WriteLine("[Networking] Client: " +
                            clientId + " reconnected.");
                        clientSocket.Client.Send(bytes);
                        Trace.WriteLine("[Networking] Data sent " +
                            "from server to client: " + clientId + 
                            " by module: " + module);
                        isSent = true;
                    }
                }

                // if data was not send even after trying 3 times then
                // client has left, notify all subscribed modules
                if (!isSent)
                {
                    Trace.WriteLine("[Networking] Client: " + 
                        clientId + " has left. Removing client...");
                    foreach (var moduleToNotificationHandler in 
                        _moduleToNotificationHandlerMap)
                    {
                        string moduleName =
                            moduleToNotificationHandler.Key;
                        var notificationHandler =
                            moduleToNotificationHandler.Value;
                        notificationHandler.OnClientLeft(clientId);
                        Trace.WriteLine("[Networking] Notifed " +
                            "module: " + moduleName + " that the " +
                            "client: " + clientId + " has left.");
                    }
                    // here we dont need to remove the client socket
                    // from the _clientIdToClientSocketMap, it will be
                    // done when the CommunicatorServer.RemoveClient()
                    // function will be called by the Dashboard
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "SendQueueListenerServer." +
                    "TryReconnectingToClient(): " + e.Message);
            }
        }
    }
}
