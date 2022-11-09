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
            var utility = new Utility();
            var contentData = utility.GenerateContentData(MessageType.Chat, MessageEvent.New, "This is a message string", messageID: 6, receiverIDs: new[] { 100 }, replyMessageID: 1);
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

        }
    }
}
