/******************************************************************************
 * Filename    = ChatServerTests.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Contains Tests for ChatServer
 *****************************************************************************/

using PlexShareContent;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using PlexShareContent.Server;

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
        public void Receive_UpdatingMessage_ReturnsUpdatedMessage()
        {
            Setup();
            var msg1 = _utils.GenerateContentData(data: "Test Message", senderID: 1);
            ReceiveContentData receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal(msg1.Data, receivedMsg.Data);

            var updateMessage = new ContentData
            {
                Data = "Testing Update",
                MessageID = receivedMsg.MessageID,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            receivedMsg = ChatServer.Receive(updateMessage);

            Assert.NotNull(receivedMsg);
            Assert.Equal("Testing Update", receivedMsg.Data);
        }

        [Fact]
        public void Receive_UpdatingMessageDoesntExist_ReturnsNull()
        {
            Setup();
            var msg1 = _utils.GenerateContentData(data: "Test Message", senderID: 1);

            ReceiveContentData receivedMsg = ChatServer.Receive(msg1);

            Assert.Equal(msg1.Data, receivedMsg.Data);
            Assert.Equal(msg1.Type, receivedMsg.Type);

            var updateMessage = new ContentData
            {
                Data = "Testing Update",
                MessageID = 1,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            receivedMsg = ChatServer.Receive(updateMessage);
            updateMessage.MessageID = 0;
            updateMessage.ReplyThreadID = 1;
            receivedMsg = ChatServer.Receive(updateMessage);
            Assert.Null(receivedMsg);
        }

        [Fact]
        public void Receive_StoringMultipleMessages_AllMessagesReturned()
        {
            Setup();
            var msg1 = _utils.GenerateContentData(data: "Test Message", senderID: 1);
            ReceiveContentData receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal(msg1.Data, receivedMsg.Data);

            var msg2 = _utils.GenerateContentData(data: "Test Message2", senderID: 1, replyThreadID: msg1.ReplyThreadID);
            receivedMsg = ChatServer.Receive(msg2);
            Assert.Equal(msg2.Data, receivedMsg.Data);
            Assert.NotEqual(msg2.MessageID, msg1.MessageID);

            var msg3 = _utils.GenerateContentData(data: "Test Message3", senderID: 1);
            receivedMsg = ChatServer.Receive(msg3);
            Assert.Equal(msg3.Data, receivedMsg.Data);
            Assert.NotEqual(msg3.MessageID, msg2.MessageID);
            Assert.NotEqual(msg3.MessageID, msg1.MessageID);
        }

        [Fact]
        public void Receive_NewMessage_StoreMessageAn_ReturnStoredMessage()
        {
            Setup();
            var msg1 = _utils.GenerateContentData(data: "Test Message", senderID: 1);
            ReceiveContentData receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal(msg1.Data, receivedMsg.Data);
        }

        [Fact]
        public void Receive_StarringMessage_ReturnsTheStarredMessage()
        {
            Setup();
            var msg = _utils.GenerateContentData(data: "Test Message", senderID: 1);
            ReceiveContentData receivedMsg = ChatServer.Receive(msg);
            Assert.Equal(msg.Data, receivedMsg.Data);

            var starMessage = new ContentData
            {
                MessageID = receivedMsg.MessageID,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = receivedMsg.Type,
                Event = MessageEvent.Star
            };

            receivedMsg = ChatServer.Receive(starMessage);
            Assert.NotNull(receivedMsg);
            Assert.Equal("Test Message", receivedMsg.Data);
        }

        [Fact]
        public void Receive_StarringMessageDoesNotExist_ReturnsNull()
        {
            Setup();
            var msg = _utils.GenerateContentData(data: "Test Message", senderID: 1);
            ReceiveContentData receivedMsg = ChatServer.Receive(msg);
            Assert.Equal(msg.Data, receivedMsg.Data);

            var starMessage = new ContentData
            {
                MessageID = 1,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            receivedMsg = ChatServer.Receive(starMessage);
            starMessage.MessageID = 0;
            starMessage.ReplyThreadID = 1;
            receivedMsg = ChatServer.Receive(starMessage);
            Assert.Null(receivedMsg);
        }

        [Fact]
        public void Receive_UpdatingMultipleMessages_OnlyTheGivenMessagesAreUpdated()
        {
            Setup();
            database = new ContentDB();                                                                                                                         
            ChatServer = new ChatServer(database);

            Receive_StoringMultipleMessages_AllMessagesReturned();

            var msg1 = new ContentData
            {
                MessageID = 0,
                Data = "Test Message",
                ReplyThreadID = 0,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            ReceiveContentData receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal("Test Message", receivedMsg.Data);

            msg1 = new ContentData
            {
                MessageID = 2,
                Data = "Updated",
                ReplyThreadID = 1,
                Type = MessageType.Chat,
                Event = MessageEvent.Edit
            };

            receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal("Updated", receivedMsg.Data);
        }

        [Fact]
        public void GetMessages_GetAllTheMessagesStoredOnTheServer_ListOfChatContextsIsReturnedWithAllTheMessages()
        {
            Setup();
            Receive_StoringMultipleMessages_AllMessagesReturned();

            var msg1 = new ContentData
            {
                MessageID = 0,
                ReplyThreadID = 0,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            ReceiveContentData recv = ChatServer.Receive(msg1);

            Assert.Equal("Test Message", recv.Data);
            Assert.Equal(MessageType.Chat, recv.Type);
            Assert.Equal(1, recv.SenderID);
            Assert.Equal(MessageEvent.Star, recv.Event);
            Assert.True(recv.Starred);

            msg1 = new ContentData
            {
                MessageID = 2,
                ReplyThreadID = 1,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            recv = ChatServer.Receive(msg1);

            Assert.Equal("Test Message3", recv.Data);
            Assert.Equal(MessageType.Chat, recv.Type);
            Assert.Equal(1, recv.SenderID);
            Assert.Equal(MessageEvent.Star, recv.Event);
            Assert.True(recv.Starred);

            var msgList = ChatServer.GetMessages();

            var msg = msgList[0].MessageList[0];
            Assert.Equal("Test Message", msg.Data);

            msg = msgList[0].MessageList[1];
            Assert.Equal("Test Message2", msg.Data);

            msg = msgList[1].MessageList[0];
            Assert.Equal("Test Message3", msg.Data);
        }

        [Fact]
        public void Receive_DeletingtingMessage_ReturnsMessageDeleted()
        {
            Setup();
            var msg1 = _utils.GenerateContentData(data: "Test Message", senderID: 1);
            ReceiveContentData receivedMsg = ChatServer.Receive(msg1);
            Assert.Equal(msg1.Data, receivedMsg.Data);

            var deleteMessage = new ContentData
            {
                MessageID = receivedMsg.MessageID,
                ReplyThreadID = receivedMsg.ReplyThreadID,
                Type = MessageType.Chat,
                Event = MessageEvent.Delete
            };

            receivedMsg = ChatServer.Receive(deleteMessage);
            Assert.NotNull(receivedMsg);
            Assert.Equal("Message Deleted.", receivedMsg.Data);
        }
    }
}
