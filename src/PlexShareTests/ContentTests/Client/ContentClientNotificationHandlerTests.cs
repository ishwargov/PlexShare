/******************************************************************************
 * Filename    = ContentClientNotificationHandlerTests.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareTests
 *
 * Description = Tests for content client notification handler class.    
 *****************************************************************************/

using PlexShareContent;
using PlexShareContent.Client;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;

namespace PlexShareTests.ContentTests.Client
{
    public class ContentClientNotificationHandlerTests
    {
        [Fact]
        public void OnDataReceived_TypeAsContentData_ReturnsValidContentData()
        {
            // send message and deserialize it from fake notification handler
            var utility = new Utility();
            var contentData = utility.GenerateContentData(MessageType.Chat, MessageEvent.New, "This is a message string", messageID: 6, receiverIDs: new[] { 100 }, replyThreadID: 2);
            IContentSerializer serializer = new ContentSerializer();
            var contentHandler = ContentClientFactory.GetInstance();
            var fakeNotificationHandler = new FakeContentNotificationHandler(contentHandler);
            var serializedData = serializer.Serialize(contentData);

            fakeNotificationHandler.OnDataReceived(serializedData);
            var dataRecevied = fakeNotificationHandler.GetReceivedMessage();

            Assert.IsType<ContentData>(dataRecevied);
            Assert.Equal(contentData.Type, dataRecevied.Type);
            Assert.Equal(contentData.Data, dataRecevied.Data);
            Assert.Equal(contentData.ReceiverIDs, dataRecevied.ReceiverIDs);
            Assert.Equal(contentData.ReplyThreadID, dataRecevied.ReplyThreadID);
            Assert.Equal(contentData.FileData, dataRecevied.FileData);
            Assert.Equal(contentData.Starred, dataRecevied.Starred);
            Assert.Equal(contentData.Event, dataRecevied.Event);
        }

        [Fact]
        public void OnDataReceived_TypeAsChatThreadList_ReturnsValidChatThreadList()
        {
            // send List<ChatThread> and deserialize message at fake notification handler
            var utility = new Utility();
            var contentData1 = utility.GenerateContentData(MessageType.Chat, MessageEvent.New, "This is a message string!", messageID: 4, receiverIDs: new[] { 100 }, replyThreadID: 2);
            var contentData2 = utility.GenerateContentData(MessageType.Chat, MessageEvent.New, "This is a message string!!", messageID: 5, receiverIDs: new[] { 100 }, replyThreadID: 2);
            var contentData3 = utility.GenerateContentData(MessageType.Chat, MessageEvent.New, "This is a message string!!!", messageID: 6, receiverIDs: new[] { 100 }, replyThreadID: 3);
            var chatThreads = new List<ChatThread>();
            var thread1 = new ChatThread();
            thread1.ThreadID = 1;
            thread1.MessageList.Add(contentData1);
            thread1.MessageList.Add(contentData2);
            var thread2 = new ChatThread();
            thread2.ThreadID = 2;
            thread2.MessageList.Add(contentData3);
            chatThreads.Add(thread1);
            chatThreads.Add(thread2);

            IContentSerializer serializer = new ContentSerializer();
            var contentHandler = ContentClientFactory.GetInstance();
            var fakeNotificationHandler = new FakeContentNotificationHandler(contentHandler);
            var serializedData = serializer.Serialize(chatThreads);

            fakeNotificationHandler.OnDataReceived(serializedData);
            var dataRecevied = fakeNotificationHandler.GetAllMessages();

            Assert.IsType<List<ChatThread>>(dataRecevied);
            utility.CheckChatThreadLists(chatThreads, dataRecevied);
        }

        [Fact]
        public void OnDataReceived_InvalidObjectType_ReturnsArgumentException()
        {
            // send an int object (which is not supported)
            var utility = new Utility();
            int data = 0;
            IContentSerializer serializer = new ContentSerializer();
            var contentHandler = ContentClientFactory.GetInstance();
            var fakeNotificationHandler = new FakeContentNotificationHandler(contentHandler);
            var serializedData = serializer.Serialize(data);

            Assert.Throws<ArgumentException>(() => fakeNotificationHandler.OnDataReceived(serializedData));
        }
    }
}
