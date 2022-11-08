using PlexShareContent;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using PlexShareContent.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.ContentTests.Server
{
    public class ChatServerTests
    {
        private Utility _utils;
        private ContentDB database;
        private ChatServer ChatServer;

        public void Setup()
        {
            database = new ContentDB();
            ChatServer = new ChatServer(database);
            _utils = new Utility();
        }

        [Fact]
        public void Receive_HandlingNewMessage_StoreTheNewMessageAndReturnTheStoredMessage()
        {
            Setup();
            var message1 = _utils.GenerateContentData(data: "Hello", senderID: 1);

            ReceiveContentData recv = ChatServer.Receive(message1);

            Assert.Equal(message1.Data, recv.Data);
        }

        [Fact]
        public void Receive_StarringAStoredMessage_MessageIsStarredAndReturnsTheStarredMessage()
        {
            Setup();
            var message1 = _utils.GenerateContentData(data: "Hello", senderID: 1);

            ReceiveContentData recv = ChatServer.Receive(message1);

            Assert.Equal(message1.Data, recv.Data);

            var starMessage = new ContentData
            {
                MessageID = recv.MessageID,
                ReplyThreadID = recv.ReplyThreadID,
                Type = recv.Type,
                Event = MessageEvent.Star
            };

            recv = ChatServer.Receive(starMessage);

            Assert.NotNull(recv);
            Assert.Equal("Hello", recv.Data);
        }

        [Fact]
        public void Receive_StarringAMessageThatDoesNotExist_NullIsReturned()
        {
            Setup();
            var message1 = _utils.GenerateContentData(data: "Hello", senderID: 1);

            ReceiveContentData recv = ChatServer.Receive(message1);

            Assert.Equal(message1.Data, recv.Data);

            var starMessage = new ContentData
            {
                MessageID = 1,
                ReplyThreadID = recv.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            recv = ChatServer.Receive(starMessage);

            Assert.Null(recv);

            starMessage.MessageID = 0;
            starMessage.ReplyThreadID = 1;

            recv = ChatServer.Receive(starMessage);

            Assert.Null(recv);
        }

        [Fact]
        public void Receive_UpdatingAStoredMessage_MessageIsUpdatedAndReturnsTheUpdatedMessage()
        {
            Setup();
            var message1 = _utils.GenerateContentData(data: "Hello", senderID: 1);

            ReceiveContentData recv = ChatServer.Receive(message1);

            Assert.Equal(message1.Data, recv.Data);

            var updateMessage = new ContentData
            {
                Data = "Hello World!",
                MessageID = recv.MessageID,
                ReplyThreadID = recv.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            recv = ChatServer.Receive(updateMessage);

            Assert.NotNull(recv);
            Assert.Equal("Hello World!", recv.Data);
        }

        [Fact]
        public void Receive_UpdatingAMessageThatDoesNotExist_NullIsReturned()
        {
            Setup();
            var message1 = _utils.GenerateContentData(data: "Hello", senderID: 1);

            ReceiveContentData recv = ChatServer.Receive(message1);

            Assert.Equal(message1.Data, recv.Data);
            Assert.Equal(message1.Type, recv.Type);

            var updateMessage = new ContentData
            {
                Data = "Hello World!",
                MessageID = 1,
                ReplyThreadID = recv.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            recv = ChatServer.Receive(updateMessage);

            Assert.Null(recv);

            updateMessage.MessageID = 0;
            updateMessage.ReplyThreadID = 1;

            recv = ChatServer.Receive(updateMessage);

            Assert.Null(recv);
        }

        [Fact]
        public void Receive_ProvidingInvalidEventForChatType_NullIsReturned()
        {
            Setup();
            var message1 = _utils.GenerateContentData(data: "Hello", senderID: 1);

            message1.Event = MessageEvent.Download;

            ReceiveContentData recv = ChatServer.Receive(message1);

            Assert.Null(recv);
        }

        [Fact]
        public void Receive_StoringMultipleMessages_AllMessagesAreStoredAndReturned()
        {
            Setup();
            var message1 = _utils.GenerateContentData(data: "Hello", senderID: 1);

            ReceiveContentData recv = ChatServer.Receive(message1);

            Assert.Equal(message1.Data, recv.Data);

            var message2 = _utils.GenerateContentData(data: "Hello2", senderID: 1, replyThreadID: message1.ReplyThreadID);

            recv = ChatServer.Receive(message2);

            Assert.Equal(message2.Data, recv.Data);
            Assert.NotEqual(message2.MessageID, message1.MessageID);

            var message3 = _utils.GenerateContentData(data: "Hello3", senderID: 1);

            recv = ChatServer.Receive(message3);

            Assert.Equal(message3.Data, recv.Data);
            Assert.NotEqual(message3.MessageID, message2.MessageID);
            Assert.NotEqual(message3.MessageID, message1.MessageID);
        }

        [Fact]
        public void Receive_StarringMultipleMessages_OnlyTheGivenMessagesAreStarred()
        {
            Setup();
            database = new ContentDB();
            ChatServer = new ChatServer(database);

            Receive_StoringMultipleMessages_AllMessagesAreStoredAndReturned();

            var message1 = new ContentData
            {
                MessageID = 0,
                ReplyThreadID = 0,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            ReceiveContentData recv = ChatServer.Receive(message1);

            Assert.Equal("Hello", recv.Data);

            message1 = new ContentData
            {
                MessageID = 2,
                ReplyThreadID = 1,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            recv = ChatServer.Receive(message1);

            Assert.Equal("Hello3", recv.Data);
        }

        [Fact]
        public void Receive_UpdatingMultipleMessages_OnlyTheGivenMessagesAreUpdated()
        {
            Setup();
            database = new ContentDB();
            ChatServer = new ChatServer(database);

            Receive_StoringMultipleMessages_AllMessagesAreStoredAndReturned();

            var message1 = new ContentData
            {
                MessageID = 0,
                Data = "Hello World!",
                ReplyThreadID = 0,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            ReceiveContentData recv = ChatServer.Receive(message1);

            Assert.Equal("Hello World!", recv.Data);

            message1 = new ContentData
            {
                MessageID = 2,
                Data = "Hello There",
                ReplyThreadID = 1,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            recv = ChatServer.Receive(message1);

            Assert.Equal("Hello There", recv.Data);
        }

        [Fact]
        public void GetAllMessages_GetAllTheMessagesStoredOnTheServer_ListOfChatContextsIsReturnedWithAllTheMessages()
        {
            Setup();
            Receive_StarringMultipleMessages_OnlyTheGivenMessagesAreStarred();

            var msgList = ChatServer.GetMessages();

            var message1 = msgList[0].MessageList[0];
            Assert.Equal("Hello", message1.Data);

            message1 = msgList[0].MessageList[1];
            Assert.Equal("Hello2", message1.Data);

            message1 = msgList[1].MessageList[0];
            Assert.Equal("Hello3", message1.Data);
        }
    }
}
