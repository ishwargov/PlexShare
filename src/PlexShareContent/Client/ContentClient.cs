/******************************************************************************
 * Filename    = ContentClient.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Client-side content manager class which provides interfaces 
 *               to other modules and deals with various user functions.   
 *****************************************************************************/

using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using PlexShareNetwork;
using PlexShareNetwork.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlexShareContent.Client
{
    public class ContentClient : IContentClient
    {
        /// <summary>
        /// Network related parameters
        /// </summary>
        private readonly INotificationHandler _notificationHandler;
        private ICommunicator _communicator;
        private IContentSerializer _serializer;

        /// <summary>
        /// List of subscribers
        /// </summary>
        private List<IContentListener> _subscribers;

        /// <summary>
        /// Chat and File handlers
        /// </summary>
        private readonly ChatClient _chatHandler;
        private readonly FileClient _fileHandler;

        /// <summary>
        /// ID of the user
        /// </summary>
        private int _userID;

        /// <summary>
        /// Lock object for locking
        /// </summary>
        private readonly object _lock;

        /// <summary>
        /// List of threads containing all messages
        /// </summary>
        public List<ChatThread> AllMessages { get; private set; }

        /// <summary>
        /// Dictionary containing mapping from message ID to thread ID
        /// </summary>
        private Dictionary<int, int> _messageIDMap;

        /// <summary>
        /// Dictionary containing mapping from thread ID to index of thread ID 
        /// </summary>
        private Dictionary<int, int> _threadIDMap;

        /// <summary>
        /// Dictionary containing mapping from type of message event
        /// to the action associtated with the message event
        /// </summary>
        private readonly Dictionary<MessageEvent, Action<ContentData>> _messageEventHandler;

        /// <summary>
        /// Constructor that instantiates all requried parameters
        /// </summary>
        public ContentClient()
        {
            // instantiate requried network parameters
            _notificationHandler = new ContentClientNotificationHandler(this);
            _communicator = CommunicationFactory.GetCommunicator();
            _serializer = new ContentSerializer();
            // subscribe to network module
            try
            {
                _communicator.Subscribe("Content", _notificationHandler);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentClient] Error subscribing to networking module.\n{e.GetType().Name} : {e.Message}");
            }
            // instantiate subscribers list
            _subscribers = new List<IContentListener>();
            // initialize chat and file handler
            _chatHandler = new ChatClient(_communicator);
            _fileHandler = new FileClient(_communicator);
            // instantiate other parameters
            _userID = -1;
            _lock = new object();
            AllMessages = new List<ChatThread>();
            _messageIDMap = new Dictionary<int, int>();
            _threadIDMap = new Dictionary<int, int>();
            // instantiate message event handler and add functions for each event
            _messageEventHandler = new Dictionary<MessageEvent, Action<ContentData>>();
            _messageEventHandler[MessageEvent.New] = NewMessageHandler;
            _messageEventHandler[MessageEvent.Edit] = EditMessageHandler;
            _messageEventHandler[MessageEvent.Delete] = DeleteMessageHandler;
            _messageEventHandler[MessageEvent.Star] = StarMessageHandler;
            _messageEventHandler[MessageEvent.Download] = DownloadMessageHandler;
        }

        /// <summary>
        /// Communicator get and set functions
        /// </summary>
        public ICommunicator Communicator
        {
            get { return _communicator; }
            set 
            { 
                _communicator = value;
                try
                {
                    _communicator.Subscribe("Content", _notificationHandler);
                }
                catch(Exception e)
                {
                    Trace.WriteLine($"[ContentClient] Exception occured while subscribing to networking module.\n{e.GetType().Name} : {e.Message}");
                }
                _chatHandler.Communicator = value;
                _fileHandler.Communicator = value;
            }
        }

        /// <summary>
        /// User ID getter and setter functions
        /// </summary>
        public int UserID
        {
            get => _userID;
            set
            {
                _userID = value;
                _chatHandler.UserID = value;
                _fileHandler.UserID = value;
            }
        }

        /// <summary>
        /// Check for valid reply message ID 
        /// </summary>
        /// <param name="replyMessageID">ID of message being replied to</param>
        /// <param name="threadID">ID of thread the message belongs to</param>
        /// <exception cref="ArgumentException"></exception>
        private void ValidateReplyMessageID(int replyMessageID, int threadID)
        {
            // ensure message being replied to exists
            if(!_messageIDMap.ContainsKey(replyMessageID))
            {
                throw new ArgumentException("Message being replied to does not exist");
            }
            // if received message is part of existing thread, ensure message
            // being replied to is in same thread as the replied message
            if(threadID != -1)
            {
                if (_messageIDMap[replyMessageID] != threadID)
                {
                    throw new ArgumentException("Message being replied to is a part of different thread");
                }
            }
            // if message is part of a new thread, no check is requried
        }

        /// <summary>
        /// Gets message from data structures using message ID
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <returns>Message object implementing the ReceiveContentData class</returns>
        /// <exception cref="ArgumentException"></exception>
        private ReceiveContentData GetMessage(int messageID)
        {
            // return null if message ID is not present in map
            if(!_messageIDMap.ContainsKey(messageID))
            {
                return null;
            }
            var threadID = _messageIDMap[messageID];
            var threadIndex = _threadIDMap[threadID];
            var thread = AllMessages[threadIndex];
            var messageIndex = thread.GetMessageIndex(messageID);
            return thread.MessageList[messageIndex];
        }

        /// <summary>
        /// Returns the array of intersecting receiver IDs of an existing message and a new reply message 
        /// </summary>
        /// <param name="previousReceiverIDs">List of receiver IDs of existing message</param>
        /// <param name="currentReceiverIDs">List of receiver IDs of the new message</param>
        /// <param name="previousSenderID">ID of the sender of existing message</param>
        /// <returns>List of Receiver IDs</returns>
        /// <exception cref="ArgumentException"></exception>
        private int[] AllReceivers(int[] previousReceiverIDs, int[] currentReceiverIDs, int previousSenderID)
        {
            // exisiting message receiver ID array is empty implies message is a broadcast message
            // final receiver array is simply the receiver array of the new message
            if (!(previousReceiverIDs?.Length > 0))
            {
                return currentReceiverIDs;
            }
            // add the sender of existing message to the receiver array
            _ = previousReceiverIDs.Concat(new[] { previousSenderID }).ToArray();
            // final receiver array is existing message receiver array if current array is empty 
            if(!(currentReceiverIDs?.Length > 0))
            {
                return previousReceiverIDs;
            }
            // get intersecting array of receiver IDs
            var allReceivers = previousReceiverIDs.Intersect(currentReceiverIDs).ToArray();
            if (!(allReceivers?.Length > 0))
            {
                throw new ArgumentException("Invalid array of receivers.");
            }
            return allReceivers;
        }

        private void CheckFilePath(string path)
        {
            try
            {
                using (var fs = File.Create(path, 1, FileOptions.DeleteOnClose))
                { }
            }
            catch
            {
                throw new ArgumentException("Invalid file path");
            }
        }

        // interface functions

        ///<inheritdoc/>
        public void ClientSendData(SendContentData contentData)
        {
            // check if receiver ID list is not null
            if (contentData.ReceiverIDs is null)
            {
                throw new ArgumentException("List of receiver IDs given is null");
            }
            // check if message is part of thread
            if(contentData.ReplyThreadID != -1)
            {
                // make sure that thread exists
                if(!_threadIDMap.ContainsKey(contentData.ReplyThreadID))
                {
                    throw new ArgumentException($"Thread with given reply thread ID ({contentData.ReplyThreadID}) does not exist");
                }
            }
            // if message is a reply to an existing message
            if(contentData.ReplyMessageID != -1)
            {
                ValidateReplyMessageID(contentData.ReplyMessageID, contentData.ReplyThreadID);
                var existingMessage = GetMessage(contentData.ReplyMessageID);
                if(existingMessage is null)
                {
                    throw new ArgumentException("Message being replied to does not exist");
                }
                contentData.ReceiverIDs = AllReceivers(existingMessage.ReceiverIDs, contentData.ReceiverIDs, existingMessage.SenderID);
            }
            // otherwise, use the respective message type handlers
            switch (contentData.Type)
            {
                case MessageType.Chat:
                    Trace.WriteLine("[ContentClient] Using chat handler to send event to server");
                    _chatHandler.NewChat(contentData);
                    break;
                case MessageType.File:
                    Trace.WriteLine("[ContentClient] Using file handler to send event to server");
                    _fileHandler.SendFile(contentData);
                    break;
                default:
                    throw new ArgumentException($"Invalid Message Field Type : {contentData.Type}");
            }
        }

        

        ///<inheritdoc/>
        public void ClientSubscribe(IContentListener subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("Null subscriber input argument");
            }
            else
            {
                // add subscriber to the list of subscribers
                Trace.WriteLine("[ContentClient] Added new subscriber");
                _subscribers.Add(subscriber);
            }
        }

        ///<inheritdoc/>
        public void ClientEdit(int messageID, string newMessage)
        {
            // get message and check if it is empty
            var message = GetMessage(messageID);
            if (message is null)
            {
                throw new ArgumentException("Message with given message ID does not exist");
            }
            // check message type
            if (message.Type != MessageType.Chat)
            {
                throw new ArgumentException($"Invalid message type for editing : {message.Type}");
            }
            // check if user can edit the message
            if (message.SenderID != UserID)
            {
                throw new ArgumentException("Edit not allowed for messages from another sender");

            }
            Trace.WriteLine("[ContentClient] Using chat handler to send edit event to server");
            _chatHandler.EditChat(messageID, newMessage, message.ReplyThreadID);
        }

        ///<inheritdoc/>
        public void ClientDelete(int messageID)
        {
            // get message and check if it is empty
            var message = GetMessage(messageID);
            if (message is null)
            {
                throw new ArgumentException("Message being replied to does not exist");
            }
            // check message type
            if (message.Type != MessageType.Chat)
            {
                throw new ArgumentException($"Invalid message type for deleting : {message.Type}");
            }
            // check if user can delete the message
            if (message.SenderID != UserID)
            {
                throw new ArgumentException("Delete not allowed for messages from another sender");
            }
            Trace.WriteLine("[ContentClient] Using chat handler to send delete event to server");
            _chatHandler.DeleteChat(messageID, message.ReplyThreadID);
        }

        

        /// <inheritdoc/>
        public void ClientDownload(int messageID, string savePath)
        {
            // check save path and message 
            CheckFilePath(savePath);
            var message = GetMessage(messageID);
            if (message is null)
            {
                Trace.WriteLine("[ContentClient] File requested for download not found");
                throw new ArgumentException("File requested for download does not exist");
            }
            // check message type
            if(message.Type != MessageType.File)
            {
                throw new ArgumentException($"Invalid message type : {message.Type}");
            }
            Trace.WriteLine("[ContentClient] Using file handler to send download file event to server");
            _fileHandler.DownloadFile(messageID, savePath);
        }

        ///<inheritdoc/>
        public void ClientStar(int messageID)
        {
            // check if message is empty
            var message = GetMessage(messageID);
            if (message is null)
            {
                throw new ArgumentException("Message with given message ID to does not exist");
            }
            // check message type
            if (message.Type != MessageType.Chat)
            {
                throw new ArgumentException($"Invalid message type : {message.Type}");
            }
            Trace.WriteLine("[ContentClient] Using chat handler to send star chat to server");
            _chatHandler.StarChat(messageID, message.ReplyThreadID);
        }

        /// <inheritdoc/>
        public ChatThread ClientGetThread(int threadID)
        {
            // check if thread exists
            if (!_threadIDMap.ContainsKey(threadID))
            {
                throw new ArgumentException("Thread with given thread ID does not exist");

            }
            var index = _threadIDMap[threadID];
            Trace.WriteLine($"[ContentClient] Returning thread with ID = {threadID}");
            return AllMessages[index];
        }

        /// <inheritdoc/>
        public int GetUserID()
        {
            return _userID;
        }

        // event handler helper functions

        /// <summary>
        /// Notifies the subscribers of the received message
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void Notify(ReceiveContentData message)
        {
            if (message is null)
            {
                throw new ArgumentException("Null message in argument");
            }
            Trace.WriteLine("[ContentClient] Notifying subscribers of new received message");
            foreach (var subscriber in _subscribers)
            {
                _ = Task.Run(() => { subscriber.OnMessageReceived(message); });
            }
        }

        // event handler functions

        /// <summary>
        /// Handles the new message event - checks the message data and 
        /// makes updates accordingly in the internal data structures. 
        /// Also notifies all the subscribers about the received message.
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void NewMessageHandler(ContentData message)
        {
            Trace.WriteLine("[ContentClient] Received message from server");
            // ensure file data is null
            message.FileData = null;
            ReceiveContentData receivedMessage = message;
            // check if message ID is unique 
            if(_messageIDMap.ContainsKey(receivedMessage.MessageID))
            {
                throw new ArgumentException("Message ID is not unique");
            }
            // check if thread exists
            var key = receivedMessage.ReplyThreadID;
            if (key == -1)
            {
                throw new ArgumentException("Message has invalid reply thread ID");
            }
            // if message is a reply, check if message being replied to exists
            if (receivedMessage.ReplyMessageID != -1)
            {
                ValidateReplyMessageID(receivedMessage.ReplyMessageID, receivedMessage.ReplyThreadID);
            }
            // using locks as the data structures may be shared across multiple system threads
            lock(_lock)
            {
                // add message ID to message ID map
                _messageIDMap.Add(receivedMessage.MessageID, key);
                // add message to all messages list if message belongs to a chat thread
                if(_threadIDMap.ContainsKey(key))
                {
                    var index = _threadIDMap[key];
                    AllMessages[index].AddMessage(receivedMessage);
                }
                else // create new thread if the message does not belong to any thread
                {
                    var newThread = new ChatThread();
                    newThread.AddMessage(receivedMessage);
                    AllMessages.Add(newThread);
                    var index = AllMessages.Count - 1;
                    // add entry into the thread ID map
                    _threadIDMap.Add(key, index);
                }
            }
            // notfiy subscribers of the new message
            Notify(receivedMessage);
        }

        /// <summary>
        /// Handles the edit message event - checks the message data and 
        /// makes updates accordingly in the internal data structures. 
        /// Also notifies all the subscribers about the received message.
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void EditMessageHandler(ContentData message)
        {
            Trace.WriteLine("[ContentClient] Received edited message from server");
            // ensure file data is null
            message.FileData = null;
            ReceiveContentData receivedMessage = message;
            var messageID = receivedMessage.MessageID;
            // check if message ID is unique 
            //if (_messageIDMap.ContainsKey(messageID))
            //{
            //    throw new ArgumentException("Message ID is not unique");
            //}
            // get thread index and check if thread exists
            var threadID = _messageIDMap[messageID];
            if (!_threadIDMap.ContainsKey(threadID))
            {
                throw new ArgumentException("No message thread with given ID exists");
            }
            // if thread exists, update message in all messages list
            var index = _threadIDMap[threadID];
            lock(_lock)
            {
                var newMessage = receivedMessage.Data;
                AllMessages[index].EditMessage(messageID, newMessage);
            }
            // notfiy subscribers of the edited message
            Notify(receivedMessage);
        }

        /// <summary>
        /// Handles the delete message event - checks the message data and 
        /// makes updates accordingly in the internal data structures. 
        /// Also notifies all the subscribers about the received message.
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void DeleteMessageHandler(ContentData message)
        {
            Trace.WriteLine("[ContentClient] Received deleted message from server");
            // ensure file data is null
            message.FileData = null;
            ReceiveContentData receivedMessage = message;
            var messageID = receivedMessage.MessageID;
            // check if message ID is unique 
            //if (_messageIDMap.ContainsKey(messageID))
            //{
            //    throw new ArgumentException("Message ID is not unique");
            //}
            // get thread index and check if thread exists
            var threadID = _messageIDMap[messageID];
            if (!_threadIDMap.ContainsKey(threadID))
            {
                throw new ArgumentException("No message thread with given ID exists");
            }
            // if thread exists, update message in all messages list
            var index = _threadIDMap[threadID];
            lock (_lock)
            {
                AllMessages[index].DeleteMessage(messageID);
            }
            // notfiy subscribers of the edited message
            Notify(receivedMessage);
        }

        /// <summary>
        /// Handles the star message event - checks the message data and 
        /// makes updates accordingly in the internal data structures. 
        /// Also notifies all the subscribers about the received message.
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void StarMessageHandler(ContentData message)
        {
            Trace.WriteLine("[ContentClient] Received starred message from server");
            // ensure file data is null
            message.FileData = null;
            ReceiveContentData receivedMessage = message;
            var messageID = receivedMessage.MessageID;
            // check if message ID is unique 
            //if (_messageIDMap.ContainsKey(messageID))
            //{
            //    throw new ArgumentException("Message ID is not unique");
            //}
            // get thread index and check if thread exists
            var threadID = _messageIDMap[messageID];
            if (!_threadIDMap.ContainsKey(threadID))
            {
                throw new ArgumentException("No message thread with given ID exists");
            }
            // if thread exists, update message in all messages list
            var index = _threadIDMap[threadID];
            lock (_lock)
            {
                AllMessages[index].StarMessage(messageID);
            }
            Notify(receivedMessage);
        }

        /// <summary>
        /// Handles the download file event - gets the save path from the message
        /// data and writes the data onto the file on the path
        /// </summary>
        /// <param name="message">Received message object from server</param>
        private void DownloadMessageHandler(ContentData message)
        {
            Trace.WriteLine("[ContentClient] Received requested file from server");
            var savePath = message.Data;
            Trace.WriteLine($"[ContentClient] Saving file to path : {savePath}");
            File.WriteAllBytes(savePath, message.FileData.Data);
        }

        /// <summary>
        /// Reset class parameters and data structures
        /// </summary>
        public void Reset()
        {
            _userID = -1;
            _subscribers = new List<IContentListener>();
            lock (_lock)
            {
                AllMessages = new List<ChatThread>();
                _threadIDMap = new Dictionary<int, int>();
                _messageIDMap = new Dictionary<int, int>();
            }
        }

        /// <summary>
        /// Set all messages of in the internal data strcutures
        /// </summary>
        /// <param name="allMessages">List of threads containing all messages</param>
        private void SetAllMessages(List<ChatThread> allMessages)
        {
            // lock before updating data strcutures
            lock (_lock)
            {
                _threadIDMap = new Dictionary<int, int>();
                _messageIDMap = new Dictionary<int, int>();
                AllMessages = allMessages;
                // update the internal data structures
                for (int i = 0; i < AllMessages.Count; i++)
                {
                    var thread = AllMessages[i];
                    var threadID = thread.ThreadID;
                    _threadIDMap.Add(threadID, i);
                    foreach (var message in thread.MessageList)
                    {
                        _messageIDMap.Add(message.MessageID, threadID);
                    }
                }
            }
        }

        /// <summary>
        /// Handles received messages from network
        /// </summary>
        /// <param name="receivedMessage">Received message object from network</param>
        /// <exception cref="ArgumentException"></exception>
        public void OnReceive(ContentData receivedMessage)
        {
            if (receivedMessage is null)
            {
                throw new ArgumentException("Received null message!");
            }
            _messageEventHandler[receivedMessage.Event](receivedMessage);
        }

        /// <summary>
        /// Handles all messages received at once 
        /// </summary>
        /// <param name="allMessages">List of threads containing all messages</param>
        /// <exception cref="ArgumentException"></exception>
        public void OnReceive(List<ChatThread> allMessages)
        {
            if (allMessages is null)
            {
                throw new ArgumentException("Received null argument!");
            }
            Trace.WriteLine("[ContentClient] Received message history from server");
            // update the internal data strcutures using the received history
            SetAllMessages(allMessages);
            Notify(allMessages);
        }        

        /// <summary>
        /// Notify all subscribers of received entire message history
        /// </summary>
        /// <param name="allMessages"></param>
        /// <exception cref="ArgumentException"></exception>
        private void Notify(List<ChatThread> allMessages)
        {
            if (allMessages is null)
            {
                throw new ArgumentException("Null message in argument");
            }
            Trace.WriteLine("[ContentClient] Notifying subscribers of all messages shared");
            foreach (var subscriber in _subscribers)
            {
                _ = Task.Run(() => { subscriber.OnAllMessagesReceived(allMessages); });
            }
        }

        /// <summary>
        /// Sends a request to server asking for all messages received on server
        /// </summary>
        public void RequestMessageHistory()
        {
            var message = new ContentData();
            message.SenderID = UserID;
            message.Type = MessageType.HistoryRequest;
            try
            {
                // serialize message and send to server via network
                var serializedMessage = _serializer.Serialize(message);
                Trace.WriteLine($"[ContentClient] Sending request for message history to server for user ID = {UserID}");
                _communicator.Send(serializedMessage, "Content", null);
            }
            catch(Exception e)
            {
                Trace.WriteLine($"[ContentClient] Exception occurred during sending message history request.\n{e.GetType()} : {e.Message}");
            }
        } 
    }
}
