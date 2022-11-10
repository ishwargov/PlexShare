/******************************************************************************
 * Filename    = ContentServerTests.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Contains Tests for ContentServer
 *****************************************************************************/

using PlexShareContent;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using PlexShareContent.Server;

namespace PlexShareTests.ContentTests.Server
{
    public class ContentServerTests
    {
        private FakeCommunicator _communicator;
        private ContentServer _contentServer;
        private FakeContentListener _listener;
        private IContentSerializer _serializer;
        private int sleeptime;
        private Utility _utility;

        public void Initialiser()
        {
            _communicator = new FakeCommunicator();
            sleeptime = 50;
            _serializer = new ContentSerializer();
            _utility = new Utility();
            _listener = new FakeContentListener();
            _contentServer = ContentServerFactory.GetInstance() as ContentServer;
            _contentServer.Reset();
            _contentServer.ServerSubscribe(_listener);
            _contentServer.Communicator = _communicator;
           

            var messageData = _utility.GenerateContentData(data: "First Message");
            var serializedMessage =_serializer.Serialize(messageData);
            _contentServer.Receive(serializedMessage);

            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            var file = new ContentData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New,
                ReceiverIDs = new int[0]
            };

            serializedMessage =_serializer.Serialize(file);
            _contentServer.Receive(serializedMessage);
        }

        [Fact]
        public void ServerSubscribe_SubscribingToNotification_GetNewMessageNotification()
        {
            Initialiser();
            var messageData = _utility.GenerateContentData(data: "Test Message");
           _serializer = new ContentSerializer();
            var serializesMessage =_serializer.Serialize(messageData);

            _contentServer.Receive(serializesMessage);

            Thread.Sleep(50);
           
            var notifiedMessage =_listener.GetReceivedMessage();
            Assert.Equal( "Test Message", notifiedMessage.Data);
        }

        [Fact]
        public void Receive_NewMessage_NotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Initialiser();
            var messageData = _utility.GenerateContentData(data: "Test Message");
            var serializesMessage =_serializer.Serialize(messageData);
            _contentServer.Receive(serializesMessage);
            Thread.Sleep(sleeptime);

            var notifiedMessage =_listener.GetReceivedMessage();
            Assert.Equal( "Test Message", notifiedMessage.Data);

            var sentMessage = _communicator.GetSentData();
            var deserializesSentMessage =_serializer.Deserialize<ContentData>(sentMessage);

            Assert.Equal( "Test Message", deserializesSentMessage.Data);
            Assert.True(_communicator.IsBroadcast());
        }

        [Fact]
        public void Receive_NewFile_SaveFileAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Initialiser();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            var file = new ContentData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New,
                ReceiverIDs = new int[0]
            };

            var serializesMessage =_serializer.Serialize(file);
            _contentServer.Receive(serializesMessage);
            Thread.Sleep(sleeptime);

            var notifiedMessage =_listener.GetReceivedMessage();
            Assert.Equal("Test_File.pdf", notifiedMessage.Data);
            Assert.Equal(file.Type, notifiedMessage.Type);

            var sentMessage = _communicator.GetSentData();
            var deserializesSentMessage =_serializer.Deserialize<ContentData>(sentMessage);
            Assert.Equal("Test_File.pdf", deserializesSentMessage.Data);
            Assert.Equal(file.Type, deserializesSentMessage.Type);
        }

        [Fact]
        public void Receive_DownloadFile_FetchFileAndForwadToCommunicator()
        {
            Initialiser();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            var file = new SendFileData(pathA);
            var fileDownloadMessage = new ContentData
            {
                Data = "x.pdf",
                MessageID = 1,
                ReplyThreadID = 1,
                Type = MessageType.File,
                Event = MessageEvent.Download,
                SenderID = 10
            };

            var serializedFileDownloadMessage =_serializer.Serialize(fileDownloadMessage);
            _contentServer.Receive(serializedFileDownloadMessage);

            var sentData = _communicator.GetSentData();
            var deserializedSentData =_serializer.Deserialize<ContentData>(sentData);

            Assert.Equal("x.pdf", deserializedSentData.Data);
            Assert.Equal(1, deserializedSentData.MessageID);
            Assert.Equal(1, deserializedSentData.ReplyThreadID);
            Assert.Equal(MessageType.File, deserializedSentData.Type);
            Assert.Equal(MessageEvent.Download, deserializedSentData.Event);

            var receivers = _communicator.GetReceiverIDs();
            Assert.Equal(1, receivers.Count);
            Assert.Equal("10", receivers[0]);
            Assert.False(_communicator.IsBroadcast());
        }

        [Fact]
        public void
            Receive_HandlingPrivateMessages_ShouldSaveTheNewMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Initialiser();
            var messageData = _utility.GenerateContentData(data: "Test Message");
            messageData.ReceiverIDs = new[] { 2, 3 };
            messageData.SenderID = 1;

            var serializesMessage =_serializer.Serialize(messageData);

            _contentServer.Receive(serializesMessage);

            Thread.Sleep(sleeptime);

            var notifiedMessage =_listener.GetReceivedMessage();

            Assert.Equal( "Test Message", notifiedMessage.Data);

            var sentMessage = _communicator.GetSentData();

            var deserializesSentMessage =_serializer.Deserialize<ContentData>(sentMessage);

            Assert.Equal( "Test Message", deserializesSentMessage.Data);

            var receivers = _communicator.GetReceiverIDs();
            Assert.Equal(3, receivers.Count);
            Assert.Equal("1", receivers[2]);
            Assert.Equal("2", receivers[0]);
            Assert.Equal("3", receivers[1]);
            Assert.False(_communicator.IsBroadcast());
        }


        [Fact]
        public void SGetAllMessages_GettingAllTheMessagesOnServer_ShouldReturnListOfChatContextsWithAllTheMessages()
        {
            Initialiser();
            var chatContexts = _contentServer.GetAllMessages();

            var firstMessage = chatContexts[0].MessageList[0];

            Assert.Equal("First Message", firstMessage.Data);
            Assert.Equal(MessageType.Chat, firstMessage.Type);
            Assert.Equal(MessageEvent.New, firstMessage.Event);
            Assert.Equal(0, firstMessage.MessageID);
            Assert.Equal(0, firstMessage.ReplyThreadID);

            var secondMessage = chatContexts[1].MessageList[0];

            Assert.Equal("Test_File.pdf", secondMessage.Data);
            Assert.Equal(MessageType.File, secondMessage.Type);
            Assert.Equal(MessageEvent.New, secondMessage.Event);
            Assert.Equal(1, secondMessage.MessageID);
            Assert.Equal(1, secondMessage.ReplyThreadID);
        }
    }
}
