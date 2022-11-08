/******************************************************************************
 * Filename    = ContentServer.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = This file is used to obtain the file and chat messages on the server and pass them along after processing to respective class.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using PlexShareContent.Client;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using PlexShareNetwork;
using PlexShareNetwork.Communication;

namespace PlexShareContent.Server
{
    public class ContentServer : IContentServer
    {
        private static readonly object _l = new();
        private INotificationHandler _notifHandler;
        private ChatServer _chatServer;
        private ICommunicator _comm;
        private ContentDB _contentDB;
        private FileServer _fileServer;
        private IContentSerializer _serializer;
        private List<IContentListener> _subscribers;

        /// <summary>
        ///     ContentServer Constructor to initilize member variales.
        /// </summary>
        public ContentServer()
        {
            _subscribers = new List<IContentListener>();
            _comm = CommunicationFactory.GetCommunicator(false);
            _contentDB = new ContentDB();
            _notifHandler = new ContentServerNotificationHandler(this);
            _fileServer = new FileServer(_contentDB);
            _chatServer = new ChatServer(_contentDB);
            _serializer = new ContentSerializer();
            _comm.Subscribe("Content", _notifHandler);
        }

        /// <summary>
        ///     Function to reset the member variables.
        /// </summary>
        public void Reset()
        {
            _subscribers = new List<IContentListener>();
            _contentDB = new ContentDB();
            _fileServer = new FileServer(_contentDB);
            _chatServer = new ChatServer(_contentDB);
            _serializer = new ContentSerializer();
        }

        /// <summary>
        ///     get and set values of communicator.
        /// </summary>
        public ICommunicator Communicator
        {
            get => _comm;
            set
            {
                _comm = value;
                _comm.Subscribe("Content", _notifHandler);
            }
        }

        /// <inheritdoc />
        public void ServerSubscribe(IContentListener subscriber)
        {
            _subscribers.Add(subscriber);
        }

        /// <summary>
        ///     Notifies all the subscribed modules.
        /// </summary>
        /// <param name="receiveMessageData"></param>
        private void Notify(ReceiveContentData receivedMsg)
        {
            foreach (var subscriber in _subscribers) _ = Task.Run(() => { subscriber.OnMessageReceived(receivedMsg); });
        }

        /// <summary>
        ///     This function is used to receive data from ContentServerNotificationHandler and processes it.
        /// </summary>
        /// <param name="data"></param>
        public void Receive(string data)
        {
            ContentData messageData;
            try
            {
                messageData = _serializer.Deserialize<ContentData>(data);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentServer] Exception occured while deserialsing data. Exception: {e}");
                return;
            }
            ContentData receivedMsg = null;
            Trace.WriteLine("[ContentServer] Received messageData from ContentServerNotificationHandler");

            lock (_l)
            {
                if (messageData.Type == MessageType.Chat)
                {
                    Trace.WriteLine("[ContentServer] MessageType is Chat, Calling ChatServer.Receive()");
                    receivedMsg = _chatServer.Receive(messageData);
                }
                else if (messageData.Type == MessageType.File)
                {
                    Trace.WriteLine("[ContentServer] MessageType is File, Calling FileServer.Receive()");
                    receivedMsg = _fileServer.Receive(messageData);
                }
                else if (messageData.Type == MessageType.HistoryRequest)
                {
                    Trace.WriteLine("[ContentServer] MessageType is HistoryRequest, Calling ContentServer.Receive()");
                    SSendAllMessagesToClient(messageData.SenderID);
                }
                else
                {
                    Trace.WriteLine("[ContentServer] MessageType is Incorrect");
                    return;
                }
            }

            if (receivedMsg == null)
            {
                Trace.WriteLine("[ContentServer] Something went wrong while handling the message.");
                return;
            }

            try
            {
                if (messageData.Event == MessageEvent.Download)
                {
                    Trace.WriteLine("[ContentServer] Download initiated, trying to fetch file and send to client");
                    SendFile(receivedMsg);
                }
                // Else send the message to all the receivers and notify the subscribers
                else
                {
                    Trace.WriteLine("[ContentServer] Notifying subscribers");
                    Notify(receivedMsg);
                    Trace.WriteLine("[ContentServer] Sending message to clients");
                    Send(receivedMsg);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentServer] Exception {e}");
                return;
            }
            Trace.WriteLine("[ContentServer] Message sent");
        }

        /// <inheritdoc />
        public List<ChatThread> ServerGetMessages()
        {
            lock (_l)
            {
                return _chatServer.GetMessages();
            }
        }

        /// <inheritdoc />
        public void SSendAllMessagesToClient(int id)
        {
            var serializedMsg = _serializer.Serialize(ServerGetMessages());
            _comm.Send(serializedMsg, "Content", id.ToString());
        }

        /// <summary>
        ///     This function is used to send the message to the client.
        /// </summary>
        /// <param name="messageData"></param>
        private void Send(ContentData msg)
        {
            var message = _serializer.Serialize(msg);

            // If length of ReceiverIds is 0 that means its a broadcast.
            if (msg.ReceiverIDs.Length == 0)
            {
                _comm.Send(message, "Content", null);
            }
            // Else send the message to the receivers in ReceiversIds.
            else
            {
                foreach (var userId in msg.ReceiverIDs)
                    _comm.Send(message, "Content", userId.ToString());
                // Sending the message back to the sender.
                _comm.Send(message, "Content", msg.SenderID.ToString());
            }
        }

        /// <summary>
        ///     This function is used to send the file to the client who requests for it.
        /// </summary>
        /// <param name="messageData"></param>
        private void SendFile(ContentData msg)
        {
            var message = _serializer.Serialize(msg);
            _comm.Send(message, "Content", msg.SenderID.ToString());
        }

    }
}
