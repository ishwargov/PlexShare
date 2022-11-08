/******************************************************************************
 * Filename    = FakeCommunicator.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class that mocks the network communicator
 *****************************************************************************/

using PlexShareNetwork;
using PlexShareNetwork.Communication;
using System.Net.Sockets;

namespace PlexShareTests.ContentTests
{
    public class FakeCommunicator : ICommunicator
    {
        // communicator parameters
        private bool _isBroadcast;
        private List<string> _receiverIds;
        private string _sendSerializedStr;
        private readonly List<INotificationHandler> _subscribers;

        /// <summary>
        /// Constructor to create communicator
        /// </summary>
        public FakeCommunicator()
        {
            _sendSerializedStr = "";
            _isBroadcast = false;
            _receiverIds = new List<string>();
            _subscribers = new List<INotificationHandler>();
        }

        ///<inheritdoc/>
        public string Start(string serverIp = null, string serverPort = null)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public void Stop()
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public void AddClient(string clientId, TcpClient socketObject)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public void RemoveClient(string clientId)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public void Send(string data, string identifier, string destination)
        {
            // if destination is null, message is a broadcast message
            if(destination == null)
            {
                _sendSerializedStr = data;
                _receiverIds = new List<string>();
                _isBroadcast = true;
            }
            else
            {
                _sendSerializedStr = data;
                _receiverIds.Add(destination);
                _isBroadcast = false;
            }
        }

        ///<inheritdoc/>
        public void Subscribe(string identifier, INotificationHandler handler, bool isHighPriority = false)
        {
            _subscribers.Add(handler);
        }

        /// <summary>
        /// Resets communicator parameters
        /// </summary>
        public void Reset()
        {
            _isBroadcast = false;
            _receiverIds = new List<string>();
        }

        // other function for testing

        /// <summary>
        /// Gets sent string data
        /// </summary>
        /// <returns>Serializes string data</returns>
        public string GetSentData()
        {
            return _sendSerializedStr;
        }
        
        /// <summary>
        /// Gets receiver ID list
        /// </summary>
        /// <returns>List of receiver IDs</returns>
        public List<string> GetReceiverIDs()
        {
            return _receiverIds;
        }

        /// <summary>
        /// Checks if a message is broadcast or not
        /// </summary>
        /// <returns>True if broadcast, false otherwise</returns>
        public bool IsBroadcast()
        {
            var flag = _isBroadcast;
            Reset();
            return flag;
        }

        /// <summary>
        /// Notifies the subscribers on data received
        /// </summary>
        /// <param name="data">Message string</param>
        public void Notify(string data)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.OnDataReceived(data);
            }
        }
    }
}
