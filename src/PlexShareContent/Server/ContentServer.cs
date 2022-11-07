using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Networking;
using Networking.Serialization;
using PlexShareContent.Client;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;

namespace PlexShareContent.Server
{
    public class ContentServer : IContentServer
    {
        private static readonly object _l = new();
        private INotificationHandler notifHandler;
        private ChatServer chatServer;
        private ICommunicator comm;
        private ContentDB contentDB;
        private FileServer fileServer;
        private IContentSerializer serializer;
        private List<IContentListener> subs;

        public ContentServer()
        {
            subs = new List<IContentListener>();
            comm = CommunicationFactory.GetCommunicator(false);
            contentDB = new ContentDB();
            notifHandler = new ContentServerNotificationHandler(this);
            fileServer = new FileServer(contentDB);
            chatServer = new ChatServer(contentDB);
            serializer = new ContentSerializer();
            comm.Subscribe("Content", notifHandler);
        }

        public ICommunicator Communicator
        {
            get => comm;
            set
            {
                comm = value;
                comm.Subscribe("Content", notifHandler);
            }
        }

        public void ServerSubscribe(IContentListener subscriber)
        {
            subs.Add(subscriber);
        }

        public List<ChatThread> ServerGetMessages()
        {
            lock (_l)
            {
                return chatServer.GetMessages();
            }
        }

        public void SSendAllMessagesToClient(int id)
        {
            var serializedMsg = serializer.Serialize(ServerGetMessages());
            comm.Send(serializedMsg, "Content", id.ToString());
        }

        public void Receive(string data)
        {
            ContentData messageData;
            try
            {
                messageData = serializer.Deserialize<ContentData>(data);
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
                if(messageData.Type == MessageType.Chat)
                {
                    Trace.WriteLine("[ContentServer] MessageType is Chat, Calling ChatServer.Receive()");
                    receivedMsg = chatServer.Receive(messageData);
                }
                else if (messageData.Type == MessageType.File)
                {
                    Trace.WriteLine("[ContentServer] MessageType is File, Calling FileServer.Receive()");
                    receivedMsg = fileServer.Receive(messageData);
                }
                else if (messageData.Type == MessageType.HistoryRequest)
                {
                    Trace.WriteLine("[ContentServer] MessageType is HistoryRequest, Calling ContentServer.Receive()");
                    SSendAllMessagesToClient(messageData.SenderID);
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
                    Trace.WriteLine("[ContentServer] Sending File to client");
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
            catch(Exception e)
            {
                Trace.WriteLine($"[ContentServer] Exception {e}");
                return;
            }
            Trace.WriteLine("[ContentServer] Message sent");
        }

        private void Send(ContentData msg)
        {
            var message = serializer.Serialize(msg);

            // If length of ReceiverIds is 0 that means its a broadcast.
            if (msg.ReceiverIDs.Length == 0)
            {
                comm.Send(message, "Content");
            }
            // Else send the message to the receivers in ReceiversIds.
            else
            {
                foreach (var userId in msg.ReceiverIDs)
                    comm.Send(message, "Content", userId.ToString());
                // Sending the message back to the sender.
                comm.Send(message, "Content", msg.SenderID.ToString());
            }
        }

        private void SendFile(ContentData msg)
        {
            var message = serializer.Serialize(msg);
            comm.Send(message, "Content", msg.SenderID.ToString());
        }

        private void Notify(ReceiveContentData receivedMsg)
        {
            foreach (var subscriber in subs) _ = Task.Run(() => { subscriber.OnMessageReceived(receivedMsg); });
        }

        public void Reset()
        {
            subs = new List<IContentListener>();
            contentDB = new ContentDB();
            fileServer = new FileServer(contentDB);
            chatServer = new ChatServer(contentDB);
            serializer = new ContentSerializer();
        }
    }
}
