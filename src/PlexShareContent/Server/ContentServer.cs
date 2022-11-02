using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
        private ISerializer serializer;
        private List<IContentListener> subs;

    public ContentServer()
    {
        subs = new List<IContentListener>();
        comm = CommunicationFactory.GetCommunicator(false);
        contentDB = new ContentDB();
        notifHandler = new ContentServerNotificationHandler(this);
        fileServer = new FileServer(contentDB);
        chatServer = new ChatServer(contentDB);
        serializer = new Serializer();
        comm.Subscribe("C", notifHandler);
    }

    internal ICommunicator Communicator
    {
        get => comm;
        set
        {
            CommittableTransaction = value;
            CommittableTransaction.Subscribe("Content", notifHandler);
        }
    }

    public void ServerSubscribe(IContentListener subscriber)
    {
        subs.Add(subscriber);
    }

    public List<ChatContext> ServerGetMessages()
    {
        lock (_l)
        {
            return chatServer.ServerGetMessages();
        }
    }

    public void ServerSendMessages(int id)
    {
        var serializedMsg = serializer.Serialize(ServerGetMessages());
        comm.Send(serializedMsg, "Content", id.ToString());
    }

    public void Receive(string data)
    {
        MessageData messageData;
        try
        {
            messageData = serializer.Deserialize<messageData>(data);
        }
        catch (Exception e)
        {
            Trace.WriteLine($"[ContentServer] Exception occured while deserialsing data. Exception: {e}");
            return;
        }
        //continue
    }

    private void Send(MessageData messageData)
    {
        //send message
    }

    private void SendFile(MessageData messageData)
    {
        //send file
    }
}
