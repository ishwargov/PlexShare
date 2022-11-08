using PlexShareContent;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using PlexShareContent.Server;

namespace PlexShareTests.ContentTests.Server
{
    public class ContentServerTests
    {
        private FakeCommunicator communicator;
        private ContentServer contentServer;
        private FakeContentListener listener;
        private IContentSerializer serializer;
        private int sleeptime;
        private Utility utils;

        public void Setup()
        {
            contentServer = ContentServerFactory.GetInstance() as ContentServer;
            contentServer.Reset();

            utils = new Utility();
            listener = new FakeContentListener();
            contentServer.ServerSubscribe(listener);
            communicator = new FakeCommunicator();
            contentServer.Communicator = communicator;
            serializer = new ContentSerializer();
            sleeptime = 50;

            var messageData = utils.GenerateContentData(data: "First Message");
            var serializedMessage = serializer.Serialize(messageData);
            contentServer.Receive(serializedMessage);

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

            serializedMessage = serializer.Serialize(file);

            contentServer.Receive(serializedMessage);
        }

        [Fact]
        public void SSubscribe_SubsribingToNotification_ShouldBeAbleToGetNotificationOfNewMessages()
        {
            utils = new Utility();
            contentServer = ContentServerFactory.GetInstance() as ContentServer;
            var messageData = utils.GenerateContentData(data: "Hello");
            serializer = new ContentSerializer();
            var serializesMessage = serializer.Serialize(messageData);
            
            contentServer.Receive(serializesMessage);

            Thread.Sleep(50);
            listener = new FakeContentListener();
            var notifiedMessage = listener.GetReceivedMessage();

            Assert.Equal( "Hello", notifiedMessage.Data);
        }

        [Fact]
        public void
            Receive_HandlingNewMessage_ShouldSaveTheNewMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Setup();
            var messageData = utils.GenerateContentData(data: "Hello");

            var serializesMessage = serializer.Serialize(messageData);

            contentServer.Receive(serializesMessage);

            Thread.Sleep(sleeptime);

            var notifiedMessage = listener.GetReceivedMessage();

            Assert.Equal( "Hello", notifiedMessage.Data);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<ContentData>(sentMessage);

            Assert.Equal( "Hello", deserializesSentMessage.Data);
            Assert.True(communicator.IsBroadcast());
        }

        [Fact]
        public void
            Receive_HandlingNewFile_ShouldSaveTheNewFileAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Setup();
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

            var serializesMessage = serializer.Serialize(file);

            contentServer.Receive(serializesMessage);

            Thread.Sleep(sleeptime);

            var notifiedMessage = listener.GetReceivedMessage();

            Assert.Equal("Test_File.pdf", notifiedMessage.Data);
            Assert.Equal(file.Type, notifiedMessage.Type);
            Assert.Equal(file.Event, notifiedMessage.Event);
            Assert.Equal(file.SenderID, notifiedMessage.SenderID);
            Assert.Equal(file.Starred, notifiedMessage.Starred);
            Assert.Equal(file.ReceiverIDs, notifiedMessage.ReceiverIDs);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<ContentData>(sentMessage);

            Assert.Equal("Test_File.pdf", deserializesSentMessage.Data);
            Assert.Equal(file.Type, deserializesSentMessage.Type);
            Assert.Equal(file.Event, deserializesSentMessage.Event);
            Assert.Equal(file.SenderID, deserializesSentMessage.SenderID);
            Assert.Equal(file.Starred, deserializesSentMessage.Starred);
            Assert.Equal(file.ReceiverIDs, deserializesSentMessage.ReceiverIDs);
            Assert.True(communicator.IsBroadcast());
        }

        [Fact]
        public void
            Receive_StarringAMessage_ShouldStarTheMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Setup();
            var starMessage = new ContentData
            {
                MessageID = 0,
                ReplyThreadID = 0,
                Event = MessageEvent.Star,
                Type = MessageType.Chat
            };

            var serializedStarMessage = serializer.Serialize(starMessage);

            contentServer.Receive(serializedStarMessage);

            Thread.Sleep(sleeptime);

            var starredMessage = listener.GetReceivedMessage();

            Assert.Equal("First Message", starredMessage.Data);
            Assert.Equal(MessageType.Chat, starredMessage.Type);
            Assert.Equal(MessageEvent.Star, starredMessage.Event);
            Assert.Equal(0, starredMessage.MessageID);
            Assert.Equal(0, starredMessage.ReplyThreadID);
            Assert.True(starredMessage.Starred);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<ContentData>(sentMessage);

            Assert.Equal("First Message", deserializesSentMessage.Data);
            Assert.Equal(MessageType.Chat, deserializesSentMessage.Type);
            Assert.Equal(MessageEvent.Star, deserializesSentMessage.Event);
            Assert.Equal(0, deserializesSentMessage.MessageID);
            Assert.Equal(0, deserializesSentMessage.ReplyThreadID);
            Assert.True(deserializesSentMessage.Starred);
            Assert.True(communicator.IsBroadcast());
        }

        [Fact]
        public void
            Receive_UpdatingAMessage_ShouldUpdateTheMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Setup();
            var updateMessage = new ContentData
            {
                MessageID = 0,
                ReplyThreadID = 0,
                Event = MessageEvent.Edit,
                Type = MessageType.Chat,
                Data = "Hello World!"
            };

            var serializedUpdateMessage = serializer.Serialize(updateMessage);

            contentServer.Receive(serializedUpdateMessage);

            Thread.Sleep(sleeptime);

            var updatedMessage = listener.GetReceivedMessage();

            Assert.Equal("Hello World!", updatedMessage.Data);
            Assert.Equal(MessageType.Chat, updatedMessage.Type);
            Assert.Equal(MessageEvent.Edit, updatedMessage.Event);
            Assert.Equal(0, updatedMessage.MessageID);
            Assert.Equal(0, updatedMessage.ReplyThreadID);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<ContentData>(sentMessage);

            Assert.Equal("Hello World!", deserializesSentMessage.Data);
            Assert.Equal(MessageType.Chat, deserializesSentMessage.Type);
            Assert.Equal(MessageEvent.Edit, deserializesSentMessage.Event);
            Assert.Equal(0, deserializesSentMessage.MessageID);
            Assert.Equal(0, deserializesSentMessage.ReplyThreadID);
            Assert.True(communicator.IsBroadcast());
        }

        [Fact]
        public void Receive_DownloadingAFile_FileShouldBeFetchedAndForwadedToTheCommunicator()
        {
            Setup();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            var file = new SendFileData(pathA);

            var fileDownloadMessage = new ContentData
            {
                Data = "a.pdf",
                MessageID = 1,
                ReplyThreadID = 1,
                Type = MessageType.File,
                Event = MessageEvent.Download,
                SenderID = 10
            };

            var serializedFileDownloadMessage = serializer.Serialize(fileDownloadMessage);

            contentServer.Receive(serializedFileDownloadMessage);

            var sentData = communicator.GetSentData();

            var deserializedSentData = serializer.Deserialize<ContentData>(sentData);

            Assert.Equal("a.pdf", deserializedSentData.Data);
            Assert.Equal(1, deserializedSentData.MessageID);
            Assert.Equal(1, deserializedSentData.ReplyThreadID);
            Assert.Equal(MessageType.File, deserializedSentData.Type);
            Assert.Equal(MessageEvent.Download, deserializedSentData.Event);
            Assert.Equal(file.Name, deserializedSentData.FileData.Name);
            Assert.Equal(file.Size, deserializedSentData.FileData.Size);
            Assert.Equal(file.Data, deserializedSentData.FileData.Data);

            var receivers = communicator.GetReceiverIDs();
            Assert.Equal(1, receivers.Count);
            Assert.Equal("10", receivers[0]);
            Assert.False(communicator.IsBroadcast());
        }

        [Fact]
        public void
            Receive_HandlingPrivateMessages_ShouldSaveTheNewMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            Setup();
            var messageData = utils.GenerateContentData(data: "Hello");
            messageData.ReceiverIDs = new[] { 2, 3 };
            messageData.SenderID = 1;

            var serializesMessage = serializer.Serialize(messageData);

            contentServer.Receive(serializesMessage);

            Thread.Sleep(sleeptime);

            var notifiedMessage = listener.GetReceivedMessage();

            Assert.Equal( "Hello", notifiedMessage.Data);
            Assert.Equal(messageData.Type, notifiedMessage.Type);
            Assert.Equal(messageData.Event, notifiedMessage.Event);
            Assert.Equal(messageData.SenderID, notifiedMessage.SenderID);
            Assert.Equal(messageData.Starred, notifiedMessage.Starred);
            Assert.Equal(messageData.ReceiverIDs, notifiedMessage.ReceiverIDs);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<ContentData>(sentMessage);

            Assert.Equal( "Hello", deserializesSentMessage.Data);
            Assert.Equal(messageData.Type, deserializesSentMessage.Type);
            Assert.Equal(messageData.Event, deserializesSentMessage.Event);
            Assert.Equal(messageData.SenderID, deserializesSentMessage.SenderID);
            Assert.Equal(messageData.Starred, deserializesSentMessage.Starred);
            Assert.Equal(messageData.ReceiverIDs, deserializesSentMessage.ReceiverIDs);

            var receivers = communicator.GetReceiverIDs();
            Assert.Equal(3, receivers.Count);
            Assert.Equal("1", receivers[2]);
            Assert.Equal("2", receivers[0]);
            Assert.Equal("3", receivers[1]);
            Assert.False(communicator.IsBroadcast());
        }

        [Fact]
        public void Receive_InvalidEventForChatType_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            Setup();
            var previousMessageToCommunicator = communicator.GetSentData();

            var eventMessage = new ContentData
            {
                MessageID = 0,
                ReplyThreadID = 0,
                Event = MessageEvent.Download,
                Type = MessageType.Chat
            };

            var serializedStarMessage = serializer.Serialize(eventMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.Equal(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Fact]
        public void Receive_InvalidEventForFileType_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            Setup();
            var previousMessageToCommunicator = communicator.GetSentData();

            var eventMessage = new ContentData
            {
                MessageID = 1,
                ReplyThreadID = 1,
                Event = MessageEvent.Star,
                Type = MessageType.File
            };

            var serializedStarMessage = serializer.Serialize(eventMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.Equal(communicator.GetSentData(), previousMessageToCommunicator);

            eventMessage = new ContentData
            {
                MessageID = 1,
                ReplyThreadID = 1,
                Event = MessageEvent.Edit,
                Type = MessageType.File
            };

            serializedStarMessage = serializer.Serialize(eventMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.Equal(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Fact]
        public void
            Receive_StarringAMessageThatDoesNotExist_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            Setup();
            var previousMessageToCommunicator = communicator.GetSentData();

            var starMessage = new ContentData
            {
                MessageID = 10,
                ReplyThreadID = 10,
                Event = MessageEvent.Star,
                Type = MessageType.Chat
            };

            var serializedStarMessage = serializer.Serialize(starMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.Equal(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Fact]
        public void
            Receive_UpdatingAMessageThatDoesNotExist_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            Setup();
            var previousMessageToCommunicator = communicator.GetSentData();

            var updateMessage = new ContentData
            {
                MessageID = 10,
                ReplyThreadID = 10,
                Event = MessageEvent.Edit,
                Type = MessageType.Chat,
                Data = "Hello There!"
            };

            var serializedStarMessage = serializer.Serialize(updateMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.Equal(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Fact]
        public void Receive_GettingInvalidDataFromNotificationListener_ShouldReturnGracefully()
        {
            Setup();
            var previousMessageToCommunicator = communicator.GetSentData();

            var garbageData = " adfasfasfsadf";
            contentServer.Receive(garbageData);

            Assert.Equal(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Fact]
        public void SGetAllMessages_GettingAllTheMessagesOnServer_ShouldReturnListOfChatContextsWithAllTheMessages()
        {
            Setup();
            var chatContexts = contentServer.ServerGetMessages();

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

        [Fact]
        public void
            SSendAllMessagesToClient_SendingAllMessagesToANewlyJoinedClient_ListOfChatContextsShouldBeForwadedToCommunicator()
        {
            Setup();
            communicator.Reset();

            var messageData = new ContentData
            {
                Type = MessageType.HistoryRequest,
                SenderID = 10
            };

            var serializedMessageData = serializer.Serialize(messageData);

            contentServer.Receive(serializedMessageData);

            var serializedAllMessages = communicator.GetSentData();

            var chatContexts = serializer.Deserialize<List<ChatThread>>(serializedAllMessages);

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

            var ReceiverIDs = communicator.GetReceiverIDs();
            Assert.Equal(1, ReceiverIDs.Count);
            Assert.Equal("10", ReceiverIDs[0]);
            Assert.False(communicator.IsBroadcast());
        }
    }
}
