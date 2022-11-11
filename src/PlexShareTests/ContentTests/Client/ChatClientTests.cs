using Microsoft.VisualBasic;
using PlexShareContent;
using PlexShareContent.Client;
using PlexShareContent.Enums;

namespace PlexShareTests.ContentTests.Client
{
    public class ChatClientTests
    {
        [Fact]
        public void ConvertSendContentData_ValidInput_ReturnsValidContentData()
        {
            var utility = new Utility();
            var sendContentData = utility.GenerateSendContentData(MessageType.Chat, "This is a message string", new[] {100});
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

        //[Fact]
        //public void SerializeAndSendToServer_ValidInput_ReturnsValidContentData()
        //{
        //    var utility = new Utility();
            
        //    var chatClient = new ChatClient(utility.GetFakeCommunicator());
        //}

    }
}
