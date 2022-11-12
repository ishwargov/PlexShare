/******************************************************************************
 * Filename    = CotentClientTests.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareTests
 *
 * Description = Tests for content client class.    
 *****************************************************************************/

using PlexShareContent;
using PlexShareContent.Client;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;

namespace PlexShareTests.ContentTests.Client
{
    public class ContentClientTests
    {
        [Fact]
        public void ClientSendData_ValidChatNotReply_ReturnsValidDataAtServer()
        {
            var utility = new Utility();
            var sendContentData = utility.GenerateSendContentData();
            var fakeCommunicator = utility.GetFakeCommunicator();
            var contentClient = ContentClientFactory.GetInstance() as ContentClient;
            contentClient.UserID = 5;
            contentClient.Communicator = fakeCommunicator;
            IContentSerializer serializer = new ContentSerializer();

            contentClient.ClientSendData(sendContentData);
            var sentData = fakeCommunicator.GetSentData();
            var deserializedData = serializer.Deserialize<ContentData>(sentData);

            Assert.Equal(deserializedData.Type, sendContentData.Type);
            Assert.Equal(MessageEvent.New, deserializedData.Event);
            Assert.Equal(deserializedData.Data, sendContentData.Data);
            Assert.Equal(deserializedData.ReceiverIDs, sendContentData.ReceiverIDs);
            Assert.Equal(deserializedData.ReplyMessageID, sendContentData.ReplyMessageID);
            Assert.False(deserializedData.Starred);
        }
    }
}
