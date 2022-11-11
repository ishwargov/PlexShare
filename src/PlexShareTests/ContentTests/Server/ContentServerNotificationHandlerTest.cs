/******************************************************************************
 * Filename    = ContentServerNotificationHandlerTests.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Contains Tests for ContentServerNotificationHandler
 *****************************************************************************/

using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using PlexShareContent.Server;
using PlexShareContent;
using PlexShareNetwork;

namespace PlexShareTests.ContentTests.Server
{
    public class ContentServerNotificationHandlerTest
    {
        private FakeCommunicator _communicator;
        private ContentServer contentServer;
        private FakeContentListener _listener;
        private INotificationHandler _notifHandler;
        private IContentSerializer _serializer;

        private int sleeptime;
        private Utility utils;

        public void Initialiser()
        {
            utils = new Utility();
            contentServer = ContentServerFactory.GetInstance() as ContentServer;
            contentServer.Reset();
            _notifHandler = new ContentServerNotificationHandler(contentServer);
            _serializer = new ContentSerializer();
           _listener = new FakeContentListener();
           _communicator = new FakeCommunicator();
            contentServer.Communicator =_communicator;
            contentServer.ServerSubscribe(_listener);
            sleeptime = 50;
        }

        [Fact]
        public void OnDataReceived_FileIsReceived_CallReceiveMethodOfContentDB()
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

            var serializedMessage = _serializer.Serialize(file);
            _notifHandler.OnDataReceived(serializedMessage);

            Thread.Sleep(sleeptime);
            var notifiedMessage = _listener.GetReceivedMessage();

            var sentMessage = _communicator.GetSentData();
            var deserializesSentMessage = _serializer.Deserialize<ContentData>(sentMessage);
            Assert.Equal("Test_File.pdf", deserializesSentMessage.Data);
            Assert.True(_communicator.IsBroadcast());
        }

        [Fact]
        public void OnDataReceived_ChatIsReceived_CallReceiveMethodOfContentDB()
        {
            Initialiser();
            var messageData = utils.GenerateContentData(data: "Test Message");
            var serializedMessage = _serializer.Serialize(messageData);
            _notifHandler.OnDataReceived(serializedMessage);

            Thread.Sleep(sleeptime);
            var notifiedMessage =_listener.GetReceivedMessage();
            Assert.Equal("Test Message", notifiedMessage.Data);
            Assert.Equal(messageData.Type, notifiedMessage.Type);
            Assert.Equal(messageData.Event, notifiedMessage.Event);
            Assert.Equal(messageData.SenderID, notifiedMessage.SenderID);
            Assert.Equal(messageData.Starred, notifiedMessage.Starred);
            Assert.Equal(messageData.ReceiverIDs, notifiedMessage.ReceiverIDs);

            var sentMessage =_communicator.GetSentData();
            var deserializesSentMessage = _serializer.Deserialize<ContentData>(sentMessage);

            Assert.Equal("Test Message", deserializesSentMessage.Data);
            Assert.True(_communicator.IsBroadcast());
        }
    }
}
