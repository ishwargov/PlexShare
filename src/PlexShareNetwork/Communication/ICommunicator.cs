/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the ICommunicator interface.
/// </summary>

namespace PlexShareNetwork.Communication
{
	public interface ICommunicator
	{

		/// <summary>
		/// Client side: Connects to the server, and initializes queues and sockets.
		/// Server side: Find and IP and port and start listening on it.
		/// </summary>
		/// <param name="serverIP"> IP Address of the server. Required only on client side. </param>
		/// <param name="serverPort"> Port no. of the server. Required only on client side. </param>
		/// <returns>
		///  Client side: string "1" if success, "0" if failure
		/// Server side: Address of the server as a string of "IP:Port"
		/// </returns>
		public string Start(string serverIP = null, string serverPort = null);


		/// <summary>
		/// Client side: Disconnects from the server and stops all running threads.
		/// Server side: Stops listening and stops all running threads.
		/// </summary>
		/// <returns> void </returns>
		public void Stop();


		/// <summary>
		/// This function is to be called only on the server when a new cilent joins.
		/// It lets the server know that a new client has joined.
		/// </summary>
		/// <typeparam name="T"> socket </typeparam>
		/// <param name="clientId"> The client Id. </param>
		/// <param name="socketObject"> The socket object of the client. </param>
		/// <returns> void </returns>
		public void AddClient<T>(string clientId, T socket);

		/// <summary>
		/// This function is to be called only on the server when a client is leaves.
		/// It removes the client information from the server.
		/// </summary>
		/// <param name="clientId"> The client Id. </param>
		/// <returns> void </returns>
		public void RemoveClient(string clientId);

		/// <summary>
		/// Client side: Sends data to the server.
		/// Server side: Broadcasts data to all clients.
		/// </summary>
		/// <param name="serializedData"> The serialzed data to be sent over the network. </param>
		/// <param name="moduleIdentifier"> Module Identifier of the module. </param>
		/// <returns> void </returns>
		public void Send(string serializedData, string moduleIdentifier);

		/// <summary>
		/// Function to send data to a specific client given by the destination argument.
		/// This function is to be called only on the server side.
		/// </summary>
		/// <param name="serializedData"> The serialzed data to be sent over the network. </param>
		/// <param name="moduleIdentifier"> Module Identifier of the module. </param>
		/// <param name="destination"> The destination or client Id to which to send the data. </param>
		/// <returns> void </returns>
		public void Send(string serializedData, string moduleIdentifier, string destination);

		/// <summary>
		/// Other modules can subscribe using this function to be notified on receiving data over the network.
		/// </summary>
		/// <param name="moduleIdentifier"> Module Identifier of the module. </param>
		/// <param name="handler"> Module implementation of the INotificationHandler. </param>
		/// <param name="isHighPriority"> Boolean which tells whether data is high priority or low priority. </param>
		/// <returns> void </returns>
		public void Subscribe(string moduleIdentifier, INotificationHandler notificationHandler, bool isHighPriority = false);
	}
}
