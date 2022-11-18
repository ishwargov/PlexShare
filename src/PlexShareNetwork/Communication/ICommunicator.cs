/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the ICommunicator interface.
/// </summary>

using System.Net.Sockets;

namespace PlexShareNetwork.Communication
{
    public interface ICommunicator
    {
        /// <summary>
        /// Client side: Connects to the server and starts all threads.
        /// Server side: Starts the tcp client connect request listener
        /// and starts all threads.
        /// </summary>
        /// <param name="serverIP">
        /// IP Address of the server. Required only on client side.
        /// </param>
        /// <param name="serverPort">
        /// Port no. of the server. Required only on client side.
        /// </param>
        /// <returns>
        ///  Client side: string "success" if success, "failure" 
        ///  if failure
        /// Server side: If success then address of the server as a 
        ///  string of "IP:Port", else string "failure"
        /// </returns>
        public string Start(string serverIP = null, 
            string serverPort = null);

        /// <summary>
        /// Client side: Stops all threads, clears queues and 
        /// closes the socket.
        /// Server side: Stops listening to client connect requests 
        /// and stops all threads. And clears the queues.
        /// </summary>
        /// <returns> void </returns>
        public void Stop();

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
        public void AddClient(string clientId, TcpClient socket);

        /// <summary>
        /// This function is to be called by the Dashboard module on
        /// the server side when a client leaves. It will remove the 
        /// client from the networking modules map on the server.
        /// </summary>
        /// <param name="clientId"> The client Id. </param>
        /// <returns> void </returns>
        public void RemoveClient(string clientId);

        /// <summary>
        /// Sends data from client to server or server to client(s).
        /// Client Side: Sends data to the server.
        /// Server Side: Sends data to a particular client if client
        /// id given in the destination argument, otherwise broadcasts
        /// data to all clients if destination null.
        /// </summary>
        /// <param name="serializedData">
        /// The serialzed data to be sent.
        /// </param>
        /// <param name="moduleName"> 
        /// Name of module sending the data.
        /// </param>
        /// <param name="destination">
        /// Client side: Not required on client side, give null.
        /// Server Side: Client Id of the client to which you want
        /// to send the data. To broadcast to all clients give null.
        /// </param>
        /// <returns> void </returns>
        public void Send(string serializedData, string moduleOfPacket,
            string? destination);

        /// <summary>
        /// Other modules can subscribe using this function to be able
        /// to send data. And be notified when data is received, and
        /// when a client joins, and when a client leaves.
        /// </summary>
        /// <param name="moduleName">  Name of the module. </param>
        /// <param name="notificationHandler">
        /// Module implementation of the INotificationHandler.
        /// </param>
        /// <param name="isHighPriority">
        /// Boolean telling whether module's data is high priority
        /// or low priority.
        /// </param>
        /// <returns> void </returns>
        public void Subscribe(string moduleName, INotificationHandler
            notificationHandler, bool isHighPriority = false);
    }
}
