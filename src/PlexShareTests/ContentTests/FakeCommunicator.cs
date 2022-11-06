using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.ContentTests
{
    public class FakeCommunicator : ICommunicator
    {
        private bool _isBroadcast;
        private List<string> _receiverIds;
        private string _sendSerializedStr;
        private readonly List<INotificationHandler> _subscribers;

        public FakeCommunicator()
        {
            _sendSerializedStr = "";
            _isBroadcast = false;
            _receiverIds = new List<string>();
            _subscribers = new List<INotificationHandler>();
        }

        public string Start(string serverIp = null, string serverPort = null)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Indicates the joining of a new client to concerned modules.
        /// </summary>
        /// <typeparam name="T">socketObject.</typeparam>
        /// <param name="clientId">Unique ID of thr Client.</param>
        /// <param name="socketObject">socket object associated with the client.</param>
        public void AddClient<T>(string clientId, T socketObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Notifies all concerned modules regarding the removal of the client.
        /// </summary>
        /// <param name="clientId">Unique ID of the client.</param>
        public void RemoveClient(string clientId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Sends data to the server[Client-Side].
        ///     Broadcasts data to all connected clients[Server-Side].
        /// </summary>
        /// <param name="data">Data to be sent over the network.</param>
        /// <param name="identifier">Module Identifier.</param>
        public void Send(string data, string identifier)
        {
            _sendSerializedStr = "";
            _sendSerializedStr = data;
            _isBroadcast = true;
            _receiverIds = new List<string>();
        }

        /// <summary>
        ///     Sends the data to one client[Server-Side].
        /// </summary>
        /// <param name="data">Data to be sent over the network.</param>
        /// <param name="identifier">Module Identifier.</param>
        /// <param name="destination">Client ID of the receiver.</param>
        public void Send(string data, string identifier, string destination)
        {
            _sendSerializedStr = "";
            _receiverIds.Add(destination);
            _isBroadcast = false;
            _sendSerializedStr = data;
        }

        /// <summary>
        ///     Provides a subscription to the modules for listening for the data over the network.
        /// </summary>
        /// <param name="identifier">Module Identifier.</param>
        /// <param name="handler">Module implementation of handler; called to notify about an incoming message.</param>
        /// <param name="priority">Priority Number indicating the weight in queue to be given to the module.</param>
        public void Subscribe(string identifier, INotificationHandler handler, bool isHighPriority = false)
        {
            _subscribers.Add(handler);
        }

        public void Reset()
        {
            _isBroadcast = false;
            _receiverIds = new List<string>();
        }

        public string GetSentData()
        {
            return _sendSerializedStr;
        }

        public List<string> GetRcvIds()
        {
            return _receiverIds;
        }

        public bool GetIsBroadcast()
        {
            var flag = _isBroadcast;
            Reset();
            return flag;
        }

        public void Notify(string data)
        {
            foreach (var subscriber in _subscribers) subscriber.OnDataReceived(data);
        }
    }
}
