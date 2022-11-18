/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the class definition of CommunicatorServer
/// which is the communicator for the server side.
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PlexShareNetwork.Communication
{
    public class CommunicatorServer : ICommunicator
    {
        // initialize the sending queue and receiving queue
        private readonly SendingQueue _sendingQueue = new();
        private readonly ReceivingQueue _receivingQueue = new();

        // declare all the threads
        private readonly SendQueueListenerServer
            _sendQueueListenerServer;
        private readonly ReceiveQueueListener _receiveQueueListener;

        // tcp listener to listen for client connect requests
        private readonly TcpListener _tcpClientConnectRequestListener;

        // map to store the sockets of the clients to send data
        private readonly Dictionary<string, TcpClient>
            _clientIdToClientSocket = new();

        // this map will store the socket listeners, one socket
        // listener listening to one client
        private readonly Dictionary<string, SocketListener> 
            _clientIdToSocketListener = new();

        // map to store the notification handlers of subscribed modules
        private readonly Dictionary<string, INotificationHandler> 
            _moduleToNotificationHanderMap = new();

        // this thread will be used to accept client requests
        private readonly Thread _clientConnectReuqestAcceptorThread;

        // boolean to tell whether thread is running or stopped
        private bool _runClientConnectReuqestAcceptorThread;

        // variables to store ip address and port of the server
        private readonly IPAddress ip;
        private readonly int port;

        /// <summary>
        /// Constructor finds the ip address and port of the current
        /// machine, and initialize the tcp listener on that IP and
        /// port, and initializes all threads.
        /// </summary>
        public CommunicatorServer()
        {
            // find ip address and port of the current machine and
            // initialize tcp client connect request listener on
            // this ip address and port
            ip = IPAddress.Parse(FindIpAddress());
            port = FindFreePort(ip);
            _tcpClientConnectRequestListener = new TcpListener(
                IPAddress.Any, port);

            // SendQueueListenerServer listens to the sending queue and
            // sends the packets whenever they comes into the queue
            // it also notifies all modules when a client disconnects
            _sendQueueListenerServer = new SendQueueListenerServer(
                _sendingQueue, _clientIdToClientSocket, 
                _moduleToNotificationHanderMap);

            // receive queue listener listens to the receiving queue
            // and notifies the respective module whenever data for
            // that module comes into the receiving queue
            _receiveQueueListener = new ReceiveQueueListener(
                _moduleToNotificationHanderMap, _receivingQueue);

            // this thread listens to connect requests from clients
            _clientConnectReuqestAcceptorThread = new Thread(
                AcceptClientConnectRequests);
        }

        /// <summary>
        /// Starts the tcp client connect request listener, and starts
        /// all threads. The function arguments are not requred on the
        /// server side, give null on server side.
        /// </summary>
        /// <param name="serverIP">
        /// Required only on client side. On server side give null.
        /// </param>
        /// <param name="serverPort">
        /// Required only on client side. On server side give null.
        /// </param>
        /// <returns>
        ///  If success then returns the address of the server as a 
        ///  string of "IP:Port", else returns string "failure"
        /// </returns>
        public string Start(string? serverIP = null,
            string? serverPort = null)
        {
            Trace.WriteLine("[Networking] " +
                "CommunicatorServer.Start() function called.");
            try
            {
                // start the tcp client connect request listener
                _tcpClientConnectRequestListener.Start();

                // start all threads
                _sendQueueListenerServer.Start();
                _receiveQueueListener.Start();
                _runClientConnectReuqestAcceptorThread = true;
                _clientConnectReuqestAcceptorThread.Start();

                Trace.WriteLine("[Networking] CommunicatorServer " +
                    "started on IP: " + ip + " and Port: "  + port);
                return ip + ":" + port;
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "CommunicatorServer.Start(): " + e.Message);
                return "failure";
            }
        }

        /// <summary>
        /// Stops listening to client connect requests and
        /// stops all threads. And clears the queues.
        /// </summary>
        /// <returns> void </returns>
        public void Stop()
        {
            Trace.WriteLine("[Networking] CommunicatorServer.Stop()" +
                " function called.");
            try
            {
                // stop the client connect requests acceptor thread
                _runClientConnectReuqestAcceptorThread = false;

                // stop listening to the clients
                foreach (var socketListener in
                    _clientIdToSocketListener.Values)
                {
                    socketListener.Stop();
                }

                // stop all running threads
                _tcpClientConnectRequestListener.Stop();
                _sendQueueListenerServer.Stop();
                _receiveQueueListener.Stop();

                // clear the queues
                _sendingQueue.Clear();
                _receivingQueue.Clear();

                Trace.WriteLine("[Networking] CommunicatorServer " +
                    "stopped.");
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "CommunicatorServer.Stop(): " + e.Message);
            }
        }

        /// <summary>
        /// Finds IP4 address of the current machine which does not 
        /// ends with 1
        /// </summary>
        /// <returns>
        /// IP address of the current machine as a string
        /// </returns>
        private static string FindIpAddress()
        {
            Trace.WriteLine("[Networking] " +
                "CommunicatorServer.FindIpAddress() function called.");
            try
            {
                // get the IP address of the machine
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                // iterate through the ip addresses and return the
                // address if it is IPv4 and does not end with 1
                foreach (IPAddress ipAddress in host.AddressList)
                {
                    // check if the address is IPv4 address
                    if (ipAddress.AddressFamily == 
                        AddressFamily.InterNetwork)
                    {
                        string address = ipAddress.ToString();
                        // return the IP address if it does not end
                        // with 1, as the loopback address ends with 1
                        if (address.Split(".")[3] != "1")
                        {
                            return ipAddress.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "CommunicatorServer.FindIpAddress(): " +
                    e.Message);
                return "null";
            }
            throw new Exception("[Networking] Error in " +
                "CommunicatorServer.FindIpAddress(): IPv4 address " +
                "not found on this machine!");
        }

        /// <summary>
        /// Finds a free TCP port on the current machine for the given
        /// IP address.
        /// </summary>
        /// <param name="ipAddress">
        /// IP address for which to find the free port.
        /// </param>
        /// <returns> The port number </returns>
        private static int FindFreePort(IPAddress ipAddress)
        {
            Trace.WriteLine("[Networking] " +
                "CommunicatorServer.FindFreePort() function called.");
            try
            {
                // start a tcp listener on port = 0, the tcp listener
                // will be assigned a port number
                TcpListener tcpListener = new(ipAddress, 0);
                tcpListener.Start();

                // return the port number of the tcp listener
                int port = 
                    ((IPEndPoint)tcpListener.LocalEndpoint).Port;
                tcpListener.Stop();
                return port;
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "CommunicatorServer.FindFreePort(): " +
                    e.Message);
                return -1;
            }
        }

        /// <summary>
        /// Accepts the connect requests from clients.
        /// </summary>
        /// <returns> void </returns>
        private void AcceptClientConnectRequests()
        {
            Trace.WriteLine("[Networking] CommunicatorServer." +
                "AcceptClientConnectRequests() function called.");
            while (_runClientConnectReuqestAcceptorThread)
            {
                try
                {
                    // accept client connect request, it will return
                    // the socket which can be used to communicate
                    // with the client
                    TcpClient clientSocket = 
                        _tcpClientConnectRequestListener.
                        AcceptTcpClient();

                    // notify all "subscribed" modules that a new
                    // client has joined
                    foreach (var moduleToNotificationHandler in 
                        _moduleToNotificationHanderMap)
                    {
                        string module = 
                            moduleToNotificationHandler.Key;
                        var notificationHandler = 
                            moduleToNotificationHandler.Value;
                        notificationHandler.OnClientJoined(
                            clientSocket);
                        Trace.WriteLine("[Networking] Notifed " +
                            "module: " + module + " that new client" +
                            " has joined.");
                    }
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.Interrupted)
                    {
                        Trace.WriteLine("[Networking] Error in " +
                            "CommunicatorServer." +
                            "AcceptClientConnectRequests(): client " +
                            "connect request tcp listener socket " +
                            "has been closed.");
                    }
                    else
                    {
                        Trace.WriteLine("[Networking] SocketException "
                            + "in CommunicatorServer." +
                            "AcceptClientConnectRequests(): " + 
                            e.Message);
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("[Networking] Error in " +
                        "CommunicatorServer." +
                        "AcceptClientConnectRequests(): " + 
                        e.Message);
                }
            }
        }

        /// <summary>
        /// This function is to be called by the Dashboard module on
        /// the server side when a new client joins. It adds the client
        /// socket to the map and starts listening to the client.
        /// </summary>
        /// <param name="clientId"> The client Id. </param>
        /// <param name="socket">
        /// The socket which is connected to the client.
        /// </param>
        /// <returns> void </returns>
        public void AddClient(string clientId, TcpClient socket)
        {
            Trace.WriteLine("[Networking] " +
                "CommunicatorServer.AddClient() function called.");
            try
            {
                // store the socket of the client in the respective map
                // and create a socket listener for that client
                // and add the socket listener to the respective map
                // and start the socket listener
                _clientIdToClientSocket[clientId] = socket;
                SocketListener socketListener = new(
                    _receivingQueue, socket);
                _clientIdToSocketListener[clientId] = socketListener;
                socketListener.Start();
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "CommunicatorServer.AddClient(): " + e.Message);
            }
            Trace.WriteLine("[Networking] Client added with " +
                "clientID: " + clientId);
        }

        /// <summary>
        /// This function is to be called by the Dashboard module on
        /// the server side when a client leaves. It will remove the 
        /// client from the networking modules map on the server.
        /// </summary>
        /// <param name="clientId"> The client Id. </param>
        /// <returns> void </returns>
        public void RemoveClient(string clientId)
        {
            Trace.WriteLine("[Networking] " +
                "CommunicatorServer.RemoveClient() function called.");

            // stop listening to this client and remove the client
            // socket listener from the respective map
            SocketListener socketListener = 
                _clientIdToSocketListener[clientId];
            socketListener.Stop();
            _clientIdToSocketListener.Remove(clientId);

            // close the connection to the client and remove the
            // client socket from the respective map
            TcpClient socket = _clientIdToClientSocket[clientId];
            socket.GetStream().Close();
            socket.Close();
            _clientIdToClientSocket.Remove(clientId);

            Trace.WriteLine("[Networking] Client removed with " +
                "clientID: " + clientId);
        }

        /// <summary>
        /// Sends data to a particular client if client id given in
        /// the destination argument, otherwise broadcasts data to
        /// all clients if destination null.
        /// </summary>
        /// <param name="serializedData">
        /// The serialzed data to be sent to the client(s).
        /// </param>
        /// <param name="moduleName"> 
        /// Name of module sending the data.
        /// </param>
        /// <param name="destination">
        /// Client Id of the client to which you want to send the data.
        /// To broadcast to all clients give null in destination.
        /// </param>
        /// <returns> void </returns>
        public void Send(string serializedData, string moduleName,
            string? destination)
        {
            Trace.WriteLine("[Networking] CommunicatorServer.Send()" +
                " function called.");

            // check if destination is not null then it must be id of
            // a client, then check if the client id is present in our
            // map or not, if not then print trace message and return
            if (destination != null)
            {
                if (!_clientIdToClientSocket.ContainsKey(destination))
                {
                    Trace.WriteLine("[Networking] Sending Falied. " +
                        "Client with ID: " + destination + " does " +
                        "not exist in the room!");
                    return;
                }
            }
            Packet packet = new(
                serializedData, destination, moduleName);
            bool isEnqueued = _sendingQueue.Enqueue(packet);
            if (isEnqueued)
            {
                Trace.WriteLine("[Networking] Enqueued packet in " +
                    "sending queue of the module: " + moduleName +
                    " for destination: " + destination);
            }
            else
            {
                Trace.WriteLine("[Networking] Packet not enqueued " +
                    "in sending queue of the module: " + moduleName +
                    " for destination: " + destination);
            }
        }

        /// <summary>
        /// Other modules can subscribe using this function to be able
        /// to send data. And be notified when data is received, and
        /// when a client joins, and when a client leaves.
        /// </summary>
        /// <param name="moduleName"> Name of the module. </param>
        /// <param name="notificationHandler">
        /// Module implementation of the INotificationHandler.
        /// </param>
        /// <param name="isHighPriority">
        /// Boolean telling whether module's data is high priority
        /// or low priority.
        /// </param>
        /// <returns> void </returns>
        public void Subscribe(string moduleName, INotificationHandler 
            notificationHandler, bool isHighPriority)
        {
            Trace.WriteLine("[Networking] " +
                "CommunicatorServer.Subscribe() function called.");
            try
            {
                // store the notification handler of the module in our
                // map
                _moduleToNotificationHanderMap.Add(
                    moduleName, notificationHandler);

                // sending queue implements priority queues so we need
                // to register the priority of the module
                _sendingQueue.RegisterModule(
                    moduleName, isHighPriority);

                Trace.WriteLine("[Networking] Module: " + moduleName +
                    " subscribed with priority [True for high/False" +
                    " for low]: " + isHighPriority.ToString());
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "CommunicatorServer.Subscribe(): " + e.Message);
            }
        }
    }
}
