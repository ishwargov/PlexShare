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
        }

        [Fact]
        public void OnDataReceived_ChatIsReceived_CallReceiveMethodOfContentDB()
        {
            Initialiser();
            sleeptime = 50;
            contentServer = ContentServerFactory.GetInstance() as ContentServer;
            contentServer.Reset();
            _notifHandler = new ContentServerNotificationHandler(contentServer);
            _serializer = new ContentSerializer();
            _listener = new FakeContentListener();
            _communicator = new FakeCommunicator();
            contentServer.Communicator = _communicator;
            contentServer.ServerSubscribe(_listener);
            var messageData = utils.GenerateContentData(data: "Message");

        }
    }
}
