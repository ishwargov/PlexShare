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
        private FakeCommunicator _communicator;
        private FakeContentListener _listener;
        private ContentClient _contentClient;

        private Utility _utility;
        private IContentSerializer _serializer;
        private string _path;

        private readonly int userID = 42;

        private int _maxValidMessageID;
        private int _maxValidThreadID;
        private readonly int _sleepTime = 50;

        private ContentData chatMessage, fileMessage, userMessage;

        public void Setup()
        {
            _utility = new Utility();
            _communicator = new FakeCommunicator();
            _listener = new FakeContentListener();
            _contentClient = (ContentClient)ContentClientFactory.GetInstance();
            _serializer = new ContentSerializer();

            _contentClient.UserID = userID;
            _contentClient.Communicator = _communicator;
            _contentClient.ClientSubscribe(_listener);

            var currentDirectory = Directory.GetCurrentDirectory();
            var pathArray = currentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            _path = pathArray[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            chatMessage = _utility.GenerateContentData(
                type: MessageType.Chat,
                @event: MessageEvent.New,
                data: "This is a sample message string",
                messageID: ++_maxValidMessageID,
                replyThreadID: ++_maxValidThreadID,
                senderID: 1
            );

            fileMessage = _utility.GenerateContentData(
                type: MessageType.File,
                @event: MessageEvent.New,
                data: _path,
                messageID: ++_maxValidMessageID,
                replyThreadID: ++_maxValidThreadID,
                senderID: 2
            );
            fileMessage.FileData = new SendFileData(_path);

            userMessage = chatMessage.Copy();
            userMessage.SenderID = userID;
            userMessage.MessageID = ++_maxValidMessageID;
            userMessage.ReplyThreadID = ++_maxValidThreadID;

            _contentClient.OnReceive(chatMessage);
            Thread.Sleep(_sleepTime);

            _contentClient.OnReceive(fileMessage);
            Thread.Sleep(_sleepTime);

            _contentClient.OnReceive(userMessage);
            Thread.Sleep(_sleepTime);
        }

        public void Dispose()
        {
            _contentClient.Reset();
        }

        [Fact]
        public void ClientSendData_ChatNotReply_ReturnsValidDataAtServer()
        {
            Setup();
            var sendContentData = _utility.GenerateSendContentData();

            _contentClient.ClientSendData(sendContentData);
            var sentData = _communicator.GetSentData();
            var deserializedData = _serializer.Deserialize<ContentData>(sentData);

            Assert.Equal(sendContentData.Type, deserializedData.Type);
            Assert.Equal(MessageEvent.New, deserializedData.Event);
            Assert.Equal(sendContentData.Data, deserializedData.Data);
            Assert.Equal(sendContentData.ReceiverIDs, deserializedData.ReceiverIDs);
            Assert.Equal(sendContentData.ReplyMessageID, deserializedData.ReplyMessageID);
            Assert.Equal(_contentClient.UserID, deserializedData.SenderID);
            Assert.False(deserializedData.Starred);

            Dispose();
        }

        [Fact]
        public void ClientSendData_ChatReplyExistingThread_ReturnsValidDataAtServer()
        {
            Setup();
            var sendContentData = _utility.GenerateSendContentData(
                replyMessageID: chatMessage.ReplyMessageID,
                replyThreadID: chatMessage.ReplyThreadID
            );

            _contentClient.ClientSendData(sendContentData);
            var sentData = _communicator.GetSentData();
            var deserializedData = _serializer.Deserialize<ContentData>(sentData);

            Assert.Equal(sendContentData.Type, deserializedData.Type);
            Assert.Equal(MessageEvent.New, deserializedData.Event);
            Assert.Equal(sendContentData.Data, deserializedData.Data);
            Assert.Equal(sendContentData.ReceiverIDs, deserializedData.ReceiverIDs);
            Assert.Equal(sendContentData.ReplyMessageID, deserializedData.ReplyMessageID);
            Assert.Equal(_contentClient.UserID, deserializedData.SenderID);
            Assert.False(deserializedData.Starred);

            Dispose();
        }

        [Fact]
        public void ClientSendData_ChatReplyNewThread_ReturnsValidDataAtServer()
        {
            Setup();
            var sendContentData = _utility.GenerateSendContentData(
                replyMessageID: chatMessage.ReplyMessageID
            );

            _contentClient.ClientSendData(sendContentData);
            var sentData = _communicator.GetSentData();
            var deserializedData = _serializer.Deserialize<ContentData>(sentData);

            Assert.Equal(sendContentData.Type, deserializedData.Type);
            Assert.Equal(MessageEvent.New, deserializedData.Event);
            Assert.Equal(sendContentData.Data, deserializedData.Data);
            Assert.Equal(sendContentData.ReceiverIDs, deserializedData.ReceiverIDs);
            Assert.Equal(sendContentData.ReplyMessageID, deserializedData.ReplyMessageID);
            Assert.Equal(_contentClient.UserID, deserializedData.SenderID);
            Assert.False(deserializedData.Starred);

            Dispose();
        }

        [Fact]
        public void ClientSendData_ValidFile_ReturnsValidDataAtServer()
        {
            Setup();
            var sendContentData = _utility.GenerateSendContentData(MessageType.File, _path);

            _contentClient.ClientSendData(sendContentData);
            var sentData = _communicator.GetSentData();
            var deserializedData = _serializer.Deserialize<ContentData>(sentData);
            var fileData = new SendFileData(_path);

            Assert.Equal(sendContentData.Type, deserializedData.Type);
            Assert.Equal(MessageEvent.New, deserializedData.Event);
            Assert.Equal(sendContentData.Data, deserializedData.Data);
            Assert.Equal(fileData.Data, deserializedData.FileData.Data);
            Assert.Equal(fileData.Name, deserializedData.FileData.Name);
            Assert.Equal(fileData.Size, deserializedData.FileData.Size);
            Assert.Equal(sendContentData.ReceiverIDs, deserializedData.ReceiverIDs);
            Assert.Equal(sendContentData.ReplyMessageID, deserializedData.ReplyMessageID);
            Assert.Equal(_contentClient.UserID, deserializedData.SenderID);
            Assert.False(deserializedData.Starred);

            Dispose();
        }

        [Fact]
        public void ClientSendData_ChatNoReceiverIDs_ReturnsArgumentException()
        {
            Setup();
            var sendContentData = _utility.GenerateSendContentData();
            sendContentData.ReceiverIDs = null;

            Assert.Throws<ArgumentException>(() => _contentClient.ClientSendData(sendContentData));
            
            Dispose();
        }

        [Fact]
        public void ClientSendData_ChatInvalidReplyThreadID_ReturnsArgumentException()
        {
            Setup();
            var sendContentData = _utility.GenerateSendContentData(replyThreadID: _maxValidThreadID + 1);

            Assert.Throws<ArgumentException>(() => _contentClient.ClientSendData(sendContentData));

            Dispose();
        }

        [Fact]
        public void ClientSendData_ChatInvalidReplyMessageID_ReturnsArgumentException()
        {
            Setup();
            var sendContentData = _utility.GenerateSendContentData(replyMessageID: _maxValidMessageID + 1);

            Assert.Throws<ArgumentException>(() => _contentClient.ClientSendData(sendContentData));

            Dispose();
        }

        [Fact]
        public void ClientSendData_InvalidType_ReturnsArgumentException()
        {
            Setup();
            var sendContentData = _utility.GenerateSendContentData();
            sendContentData.Type += 1;

            Assert.Throws<ArgumentException>(() => _contentClient.ClientSendData(sendContentData));

            Dispose();
        }

        [Fact]
        public void ClientSendData_InvalidFilePath_ReturnsFileNotFoundException()
        {
            Setup();
            var sendContentData = _utility.GenerateSendContentData(
                type: MessageType.File, 
                data: "This is supposed to be a file path"
            );

            Assert.Throws<FileNotFoundException>(() => _contentClient.ClientSendData(sendContentData));
            
            Dispose();
        }

        [Fact]
        public void ClientSubscribe_NullSubscriber_ReturnsArgumentException()
        {
            Setup();
            IContentListener subscriber = null;

            Assert.Throws<ArgumentNullException>(() => _contentClient.ClientSubscribe(subscriber));

            Dispose();
        }

        [Fact]
        public void ClientEdit_ValidEdit_ReturnsValidDataAtServer()
        {
            Setup();
            string newMessage = "This is the edited message string";
            _contentClient.ClientEdit(userMessage.MessageID, newMessage);
            var sentData = _communicator.GetSentData();
            var deserializedData = _serializer.Deserialize<ContentData>(sentData);

            Assert.Equal(MessageType.Chat, deserializedData.Type);
            Assert.Equal(MessageEvent.Edit, deserializedData.Event);
            Assert.Equal(newMessage, deserializedData.Data);
            Assert.Equal(userMessage.MessageID, deserializedData.MessageID);
            Assert.Equal(userMessage.ReplyThreadID, deserializedData.ReplyThreadID);

            Dispose();
        }

        [Fact]
        public void ClientEdit_InvalidMessageID_ReturnsArgumentException()
        {
            Setup();
            string newMessage = "This is the edited message string";

            Assert.Throws<ArgumentException>(() => _contentClient.ClientEdit(_maxValidMessageID + 1, newMessage));

            Dispose();
        }

        [Fact]
        public void ClientEdit_OtherUserEdit_ReturnsArgumentException()
        {
            Setup();
            string newMessage = "This is the edited message string";

            Assert.Throws<ArgumentException>(() => _contentClient.ClientEdit(chatMessage.MessageID, newMessage));

            Dispose();
        }

        [Fact]
        public void ClientEdit_EditFile_ReturnsArgumentException()
        {
            Setup();
            string newMessage = "This is the edited message string";

            Assert.Throws<ArgumentException>(() => _contentClient.ClientEdit(fileMessage.MessageID, newMessage));

            Dispose();
        }

        [Fact]
        public void ClientEdit_NullEditedMessage_ReturnsArgumentException()
        {
            Setup();
            string newMessage = null;

            Assert.Throws<ArgumentException>(() => _contentClient.ClientEdit(userMessage.MessageID, newMessage));

            Dispose();
        }

        [Fact]
        public void ClientEdit_EmptyEditedMessage_ReturnsArgumentException()
        {
            Setup();
            string newMessage = "";

            Assert.Throws<ArgumentException>(() => _contentClient.ClientEdit(userMessage.MessageID, newMessage));

            Dispose();
        }

        [Fact]
        public void ClientDelete_ValidDelete_ReturnsValidDataAtServer()
        {
            Setup();
            _contentClient.ClientDelete(userMessage.MessageID);
            var sentData = _communicator.GetSentData();
            var deserializedData = _serializer.Deserialize<ContentData>(sentData);

            Assert.Equal(MessageType.Chat, deserializedData.Type);
            Assert.Equal(MessageEvent.Delete, deserializedData.Event);
            Assert.Equal("Message Deleted.", deserializedData.Data);
            Assert.Equal(userMessage.MessageID, deserializedData.MessageID);
            Assert.Equal(userMessage.ReplyThreadID, deserializedData.ReplyThreadID);

            Dispose();
        }

        [Fact]
        public void ClientDelete_InvalidMessageID_ReturnsArgumentException()
        {
            Setup();

            Assert.Throws<ArgumentException>(() => _contentClient.ClientDelete(_maxValidMessageID + 1));

            Dispose();
        }

        [Fact]
        public void ClientDelete_OtherUserDelete_ReturnsArgumentException()
        {
            Setup();

            Assert.Throws<ArgumentException>(() => _contentClient.ClientDelete(chatMessage.MessageID));

            Dispose();
        }

        [Fact]
        public void ClientDelete_DeleteFile_ReturnsArgumentException()
        {
            Setup();

            Assert.Throws<ArgumentException>(() => _contentClient.ClientDelete(fileMessage.MessageID));

            Dispose();
        }

        //[Fact]
        //public void ClientDownload_ValidDownload_ReturnsValidDataAtServer()
        //{
        //    Setup();
        //    var currentDirectory = Directory.GetCurrentDirectory();
        //    var pathArray = currentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
        //    string savePath = pathArray[0] + "\\PlexShareTests\\ContentTests\\Save_File.pdf";
        //    _contentClient.ClientDownload(fileMessage.MessageID, savePath);
        //    var sentData = _communicator.GetSentData();
        //    var deserializedData = _serializer.Deserialize<ContentData>(sentData);

        //    Assert.Equal(MessageType.File, deserializedData.Type);
        //    Assert.Equal(MessageEvent.Download, deserializedData.Event);
        //    Assert.Equal(fileMessage.MessageID, deserializedData.MessageID);
        //    Assert.Equal(savePath, deserializedData.Data);

        //    Dispose();
        //}

        [Fact]
        public void ClientStar_ValidStar_ReturnsValidDataAtServer()
        {
            Setup();
            _contentClient.ClientStar(userMessage.MessageID);
            var sentData = _communicator.GetSentData();
            var deserializedData = _serializer.Deserialize<ContentData>(sentData);

            Assert.Equal(MessageType.Chat, deserializedData.Type);
            Assert.Equal(MessageEvent.Star, deserializedData.Event);
            Assert.Equal(userMessage.MessageID, deserializedData.MessageID);
            Assert.Equal(userMessage.ReplyThreadID, deserializedData.ReplyThreadID);

            Dispose();
        }

        [Fact]
        public void ClientStar_InvalidMessageID_ReturnsArgumentException()
        {
            Setup();

            Assert.Throws<ArgumentException>(() => _contentClient.ClientStar(_maxValidMessageID + 1));

            Dispose();
        }

        [Fact]
        public void ClientStar_StarFile_ReturnsArgumentException()
        {
            Setup();

            Assert.Throws<ArgumentException>(() => _contentClient.ClientStar(fileMessage.MessageID));

            Dispose();
        }


    }
}
