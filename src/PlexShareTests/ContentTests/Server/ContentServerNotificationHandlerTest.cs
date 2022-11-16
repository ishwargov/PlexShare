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
using Microsoft.Azure.WebJobs.Host.Listeners;
using PlexShareNetwork.Communication;

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

        [Fact]
        public void OnDataReceived_ChatIsReceived_CallReceiveMethodOfContentDB()
        {
            utils = new Utility();
            contentServer = ContentServerFactory.GetInstance() as ContentServer;
            contentServer.Reset();
            sleeptime = 50;
            _notifHandler = new ContentServerNotificationHandler(contentServer);
            _serializer = new ContentSerializer();
            _listener = new FakeContentListener();
            _communicator = new FakeCommunicator();
            contentServer.Communicator = _communicator;
            contentServer.ServerSubscribe(_listener);

            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\Testing" }, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

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

            Assert.Equal("Test_File.pdf", notifiedMessage.Data);
        }
    }
}
