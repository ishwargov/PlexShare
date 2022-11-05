/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the definition of the 'INotificationHandler' interface, which gives the blueprint of functions to be called when
/// packets need to be transmitted from the receiving queue and when a specific module subscribes and unsubscribes
/// </summary>

namespace PlexShareNetwork
{
    public interface INotificationHandler
    {
        /// <summary>
        /// Called by the Communicator when a packet is to be transmitted from the receiving queue
        /// </summary>
        public void OnDataReceived(string serializedData);

        /// <summary>
        /// Called by the Communicator when a module declares that it wants the networking module for communication
        /// It maps the socket object to the module which calls this function
        /// </summary>
        public void OnClientJoined<T>(T socket)
        {

        }

        /// <summary>
        /// Called by the Communicator when a module declares that it no more needs the networking module
        /// The mapping of a socket object to the module is erased
        /// </summary>
        public void OnClientLeft(string clientId)
        {

        }
    }
}
