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
using PlexShareNetwork.Serialization;

namespace PlexShareContent.Server
{
    public class ContentServer : IContentServer
    {
        private static readonly object _lock = new();
        private readonly INotificationHandler _notificationHandler;
        private ChatServer _chatContextServer;
        private ICommunicator _communicator;
        private ContentDB _contentDatabase;
        private FileServer _fileServer;
        private IContentSerializer _serializer;
        private List<IContentListener> _subscribers;

        public ContentServer()
        {
            _subscribers = new List<IContentListener>();
            _communicator = CommunicationFactory.GetCommunicator(false);
            _contentDatabase = new ContentDB();
            _notificationHandler = new ContentServerNotificationHandler(this);
            _fileServer = new FileServer(_contentDatabase);
            _chatContextServer = new ChatServer(_contentDatabase);
            _serializer = new ContentSerializer();
            _communicator.Subscribe("Content", _notificationHandler);
        }

        /// <summary>
        ///     Get and Set Communicator, Meant to be only used for testing
        /// </summary>
        public ICommunicator Communicator
        {
            get => _communicator;
            set
            {
                _communicator = value;
                _communicator.Subscribe("Content", _notificationHandler);
            }
        }

        /// <inheritdoc />
        public void ServerSubscribe(IContentListener subscriber)
        {
            _subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public List<ChatThread> GetAllMessages()
        {
            lock (_lock)
            {
                return _chatContextServer.GetMessages();
            }
        }

        /// <inheritdoc />
        public void SSendAllMessagesToClient(int userId)
        {
            var allMessagesSerialized = _serializer.Serialize(GetAllMessages());
            _communicator.Send(allMessagesSerialized, "Content", userId.ToString());
        }

        /// <summary>
        ///     Receives data from ContentServerNotificationHandler and processes it accordingly
        /// </summary>
        /// <param name="data"></param>
        public void Receive(string data)
        {
            ContentData messageData;
            // Try deserializing the data if error then do nothing and return.
            try
            {
                messageData = _serializer.Deserialize<ContentData>(data);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentServer] Exception occured while deserialsing data. Exception: {e}");
                return;
            }

            ContentData receiveMessageData;

            Trace.WriteLine("[ContentServer] Received messageData from ContentServerNotificationHandler");

            // lock to prevent multiple threads from modifying the messages at once.
            lock (_lock)
            {
                switch (messageData.Type)
                {
                    case MessageType.Chat:
                        Trace.WriteLine("[ContentServer] MessageType is Chat, Calling ChatServer.Receive()");
                        receiveMessageData = _chatContextServer.Receive(messageData);
                        break;

                    case MessageType.File:
                        Trace.WriteLine("[ContentServer] MessageType is File, Calling FileServer.Receive()");
                        receiveMessageData = _fileServer.Receive(messageData);
                        break;

                    case MessageType.HistoryRequest:
                        Trace.WriteLine(
                            "[ContentServer] MessageType is HistoryRequest, Calling ContentServer.SSendAllMessagesToClient");
                        SSendAllMessagesToClient(messageData.SenderID);
                        return;

                    default:
                        Trace.WriteLine("[ContentServer] Unknown Message Type");
                        return;
                }
            }

            // If this is null then something went wrong, probably message was not found.
            if (receiveMessageData == null)
            {
                Trace.WriteLine("[ContentServer] Something went wrong while handling the message.");
                return;
            }

            try
            {
                // If Event is Download then send the file to client
                if (messageData.Event == MessageEvent.Download)
                {
                    Trace.WriteLine("[ContentServer] Sending File to client");
                    SendFile(receiveMessageData);
                }
                // Else send the message to all the receivers and notify the subscribers
                else
                {
                    Trace.WriteLine("[ContentServer] Notifying subscribers");
                    Notify(receiveMessageData);
                    Trace.WriteLine("[ContentServer] Sending message to clients");
                    Send(receiveMessageData);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentServer] Something went wrong while sending message. Exception {e}");
                return;
            }

            Trace.WriteLine("[ContentServer] Message sent");
        }

        /// <summary>
        ///     Sends the message to clients.
        /// </summary>
        /// <param name="messageData"></param>
        public void Send(ContentData messageData)
        {
            var message = _serializer.Serialize(messageData);

            // If length of ReceiverIds is 0 that means its a broadcast.
            if (messageData.ReceiverIDs.Length == 0)
            {
                _communicator.Send(message, "Content", null);
            }
            // Else send the message to the receivers in ReceiversIds.
            else
            {
                foreach (var userId in messageData.ReceiverIDs)
                    _communicator.Send(message, "Content", userId.ToString());
                // Sending the message back to the sender.
                _communicator.Send(message, "Content", messageData.SenderID.ToString());
            }
        }

        /// <summary>
        ///     Sends the file back to the requester.
        /// </summary>
        /// <param name="messageData"></param>
        public void SendFile(ContentData messageData)
        {
            var message = _serializer.Serialize(messageData);
            _communicator.Send(message, "Content", messageData.SenderID.ToString());
        }

        /// <summary>
        ///     Notifies all the subscribed modules.
        /// </summary>
        /// <param name="receiveMessageData"></param>
        public void Notify(ReceiveContentData receiveMessageData)
        {
            foreach (var subscriber in _subscribers) _ = Task.Run(() => { subscriber.OnMessageReceived(receiveMessageData); });
        }

        /// <summary>
        ///     Resets the ContentServer, Meant to be used only for Testing
        /// </summary>
        public void Reset()
        {
            _subscribers = new List<IContentListener>();
            _contentDatabase = new ContentDB();
            _fileServer = new FileServer(_contentDatabase);
            _chatContextServer = new ChatServer(_contentDatabase);
            _serializer = new ContentSerializer();
        }
    }
}
