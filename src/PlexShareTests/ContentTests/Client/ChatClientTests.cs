/******************************************************************************
 * Filename    = ChatClientTests.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareTests
 *
 * Description = Tests for chat client class.    
 *****************************************************************************/

using PlexShareContent;
using PlexShareContent.Client;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;

namespace PlexShareTests.ContentTests.Client
{
    public class ChatClientTests
    {
        [Fact]
        public void ConvertSendContentData_ValidInput_ReturnsValidContentData()
        {
            var utility = new Utility();
            var sendContentData = utility.GenerateSendContentData(MessageType.Chat, "This is a message string");
            var chatClient = new ChatClient(utility.GetFakeCommunicator());

            var contentData = chatClient.ConvertSendContentData(sendContentData, MessageEvent.New);

            Assert.Equal(sendContentData.Type, contentData.Type);
            Assert.Equal(sendContentData.Data, contentData.Data);
            Assert.Equal(sendContentData.ReceiverIDs, contentData.ReceiverIDs);
            Assert.Equal(sendContentData.ReplyThreadID, contentData.ReplyThreadID);
            Assert.Equal(MessageEvent.New, contentData.Event);
            Assert.False(contentData.Starred);
            Assert.Null(contentData.FileData);
        }

        [Fact]
        public void NewChat_ValidInput_ReturnsValidContentData()
        {
            var utility = new Utility();
            var sendContentData = utility.GenerateSendContentData(MessageType.Chat, "This is a message string");
            var fakeCommunicator = utility.GetFakeCommunicator();
            var serializer = new ContentSerializer();
            var userID = 5;
            var chatClient = new ChatClient(fakeCommunicator);
            chatClient.UserID = userID;
            chatClient.Communicator = fakeCommunicator;
            var contentData = chatClient.ConvertSendContentData(sendContentData, MessageEvent.New);

            chatClient.NewChat(sendContentData);
            var serializedData = fakeCommunicator.GetSentData();
            var deserializedData = serializer.Deserialize<ContentData>(serializedData);

            Assert.IsType<ContentData>(deserializedData);
            Assert.Equal(contentData.Type, deserializedData.Type);
            Assert.Equal(contentData.Data, deserializedData.Data);
            Assert.Equal(contentData.ReceiverIDs, deserializedData.ReceiverIDs);
            Assert.Equal(contentData.ReplyThreadID, deserializedData.ReplyThreadID);
            Assert.Equal(contentData.FileData, deserializedData.FileData);
            Assert.Equal(contentData.SenderID, deserializedData.SenderID);
            Assert.Equal(contentData.Starred, deserializedData.Starred);
            Assert.Equal(contentData.Event, deserializedData.Event);
        }

        [Fact]
        public void NewChat_EmptyMessageString_ReturnsArgumentException()
        {
            var utility = new Utility();
            var sendContentData = utility.GenerateSendContentData(MessageType.Chat, "");
            var fakeCommunicator = utility.GetFakeCommunicator();
            var serializer = new ContentSerializer();
            var userID = 5;
            var chatClient = new ChatClient(fakeCommunicator);
            chatClient.UserID = userID;
            chatClient.Communicator = fakeCommunicator;

            Assert.Throws<ArgumentException>(() => chatClient.NewChat(sendContentData));
        }

        [Fact]
        public void NewChat_NullMessageString_ReturnsArgumentException()
        {
            var utility = new Utility();
            var sendContentData = utility.GenerateSendContentData(MessageType.Chat, null);
            var fakeCommunicator = utility.GetFakeCommunicator();
            var serializer = new ContentSerializer();
            var userID = 5;
            var chatClient = new ChatClient(fakeCommunicator);
            chatClient.UserID = userID;
            chatClient.Communicator = fakeCommunicator;

            Assert.Throws<ArgumentException>(() => chatClient.NewChat(sendContentData));
        }

        [Fact]
        public void EditChat_ValidInput_ReturnsValidContentData()
        {
            var utility = new Utility();
            var sendContentData = utility.GenerateSendContentData(MessageType.Chat, "This is an edited message");
            var fakeCommunicator = utility.GetFakeCommunicator();
            var serializer = new ContentSerializer();
            var userID = 5;
            var messageID = 6;
            var threadID = 7;
            var chatClient = new ChatClient(fakeCommunicator);
            chatClient.UserID = userID;
            chatClient.Communicator = fakeCommunicator;
            var contentData = chatClient.ConvertSendContentData(sendContentData, MessageEvent.Edit);

            chatClient.EditChat(messageID, "This is an edited message", threadID);
            var serializedData = fakeCommunicator.GetSentData();
            var deserializedData = serializer.Deserialize<ContentData>(serializedData);

            Assert.IsType<ContentData>(deserializedData);
            Assert.Equal(contentData.Type, deserializedData.Type);
            Assert.Equal(contentData.Data, deserializedData.Data);
            Assert.Equal(messageID, deserializedData.MessageID);
            Assert.Equal(threadID, deserializedData.ReplyThreadID);
            Assert.Equal(contentData.FileData, deserializedData.FileData);
            Assert.Equal(contentData.SenderID, deserializedData.SenderID);
            Assert.Equal(contentData.Starred, deserializedData.Starred);
            Assert.Equal(contentData.Event, deserializedData.Event);
        }

        [Fact]
        public void DeleteChat_ValidInput_ReturnsValidContentData()
        {
            var utility = new Utility();
            var sendContentData = utility.GenerateSendContentData(MessageType.Chat, "Message Deleted.");
            var fakeCommunicator = utility.GetFakeCommunicator();
            var serializer = new ContentSerializer();
            var userID = 5;
            var messageID = 6;
            var threadID = 7;
            var chatClient = new ChatClient(fakeCommunicator);
            chatClient.UserID = userID;
            chatClient.Communicator = fakeCommunicator;
            var contentData = chatClient.ConvertSendContentData(sendContentData, MessageEvent.Delete);

            chatClient.DeleteChat(messageID, threadID);
            var serializedData = fakeCommunicator.GetSentData();
            var deserializedData = serializer.Deserialize<ContentData>(serializedData);

            Assert.IsType<ContentData>(deserializedData);
            Assert.Equal(contentData.Type, deserializedData.Type);
            Assert.Equal(contentData.Data, deserializedData.Data);
            Assert.Equal(messageID, deserializedData.MessageID);
            Assert.Equal(threadID, deserializedData.ReplyThreadID);
            Assert.Equal(contentData.FileData, deserializedData.FileData);
            Assert.Equal(contentData.SenderID, deserializedData.SenderID);
            Assert.Equal(contentData.Starred, deserializedData.Starred);
            Assert.Equal(contentData.Event, deserializedData.Event);
        }

        [Fact]
        public void StarChat_ValidInput_ReturnsValidContentData()
        {
            var utility = new Utility();
            var sendContentData = utility.GenerateSendContentData(MessageType.Chat, null);
            var fakeCommunicator = utility.GetFakeCommunicator();
            var serializer = new ContentSerializer();
            var userID = 5;
            var messageID = 6;
            var threadID = 7;
            var chatClient = new ChatClient(fakeCommunicator);
            chatClient.UserID = userID;
            chatClient.Communicator = fakeCommunicator;
            var contentData = chatClient.ConvertSendContentData(sendContentData, MessageEvent.Star);

            chatClient.StarChat(messageID, threadID);
            var serializedData = fakeCommunicator.GetSentData();
            var deserializedData = serializer.Deserialize<ContentData>(serializedData);

            Assert.IsType<ContentData>(deserializedData);
            Assert.Equal(contentData.Type, deserializedData.Type);
            Assert.Equal(contentData.Data, deserializedData.Data);
            Assert.Equal(messageID, deserializedData.MessageID);
            Assert.Equal(threadID, deserializedData.ReplyThreadID);
            Assert.Equal(contentData.FileData, deserializedData.FileData);
            Assert.Equal(contentData.SenderID, deserializedData.SenderID);
            Assert.Equal(contentData.Event, deserializedData.Event);
        }
    }
}
