using Networking.Serialization;
using Networking;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using PlexShareContent.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareContent;

namespace PlexShareTests.ContentTests.Server
{
    public class ContentServerNotificationHandlerTest
    {
        private FakeCommunicator communicator;
        private ContentServer contentServer;
        private FakeContentListener listener;
        private INotificationHandler notificationHandler;
        private ISerializer serializer;

        private int sleeptime;
        private Utils utils;

        public void Setup()
        {
            utils = new Utils();
            contentServer = ContentServerFactory.GetInstance() as ContentServer;
            contentServer.Reset();
            notificationHandler = new ContentServerNotificationHandler(contentServer);
            serializer = new Serializer();
            listener = new FakeContentListener();
            communicator = new FakeCommunicator();
            contentServer.Communicator = communicator;
            contentServer.ServerSubscribe(listener);
            sleeptime = 50;
        }

        [Fact]
        public void OnDataReceived_ChatDataIsReceived_CallReceiveMethodOfContentDatabase()
        {
            var messageData = utils.GenerateNewMessageData("Hello");

            var serializedMessage = serializer.Serialize(messageData);

            notificationHandler.OnDataReceived(serializedMessage);

            Thread.Sleep(sleeptime);

            var notifiedMessage = listener.GetOnMessageData();

            Assert.Same("Hello", notifiedMessage.Message);
            Assert.Same(messageData.Type, notifiedMessage.Type);
            Assert.Same(messageData.Event, notifiedMessage.Event);
            Assert.Same(messageData.SenderId, notifiedMessage.SenderId);
            Assert.Same(messageData.Starred, notifiedMessage.Starred);
            Assert.Same(messageData.ReceiverIds, notifiedMessage.ReceiverIds);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<ContentData>(sentMessage);

            Assert.Same("Hello", deserializesSentMessage.Data);
            Assert.Same(messageData.Type, deserializesSentMessage.Type);
            Assert.Same(messageData.Event, deserializesSentMessage.Event);
            Assert.Same(messageData.SenderId, deserializesSentMessage.SenderID);
            Assert.Same(messageData.Starred, deserializesSentMessage.Starred);
            Assert.Same(messageData.ReceiverIds, deserializesSentMessage.ReceiverIDs);
            Assert.True(communicator.GetIsBroadcast());
        }

        [Fact]
        public void OnDataReceived_FileDataIsReceived_CallReceiveMethodOfContentDatabase()
        {
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

            var serializedMessage = serializer.Serialize(file);

            notificationHandler.OnDataReceived(serializedMessage);

            Thread.Sleep(sleeptime);

            var notifiedMessage = listener.GetOnMessageData();

            Assert.Same("Test_File.pdf", notifiedMessage.Message);
            Assert.Same(file.Type, notifiedMessage.Type);
            Assert.Same(file.Event, notifiedMessage.Event);
            Assert.Same(file.SenderID, notifiedMessage.SenderId);
            Assert.Same(file.Starred, notifiedMessage.Starred);
            Assert.Same(file.ReceiverIDs, notifiedMessage.ReceiverIds);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<ContentData>(sentMessage);

            Assert.Same("Test_File.pdf", deserializesSentMessage.Data);
            Assert.Same(file.Type, deserializesSentMessage.Type);
            Assert.Same(file.Event, deserializesSentMessage.Event);
            Assert.Same(file.SenderID, deserializesSentMessage.SenderID);
            Assert.Same(file.Starred, deserializesSentMessage.Starred);
            Assert.Same(file.ReceiverIDs, deserializesSentMessage.ReceiverIDs);
            Assert.True(communicator.GetIsBroadcast());
        }

        [Fact]
        public void OnDataReceived_InvalidDataIsReceived_CallReceiveMethodOfContentDatabase()
        {
            var previousMessageToSubsribers = listener.GetOnMessageData();
            var previousMessageToCommunicator = communicator.GetSentData();

            var garbageData = " adfasfasfsadf";
            notificationHandler.OnDataReceived(garbageData);

            Thread.Sleep(sleeptime);

            var currentMessageToSubscribers = listener.GetOnMessageData();

            Assert.Same(currentMessageToSubscribers, previousMessageToSubsribers);
            Assert.Same(communicator.GetSentData(), previousMessageToCommunicator);
        }
    }
}
