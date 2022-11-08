using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json;
using PlexShareContent;
using PlexShareContent.Client;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using SharpDX.Direct3D11;
using System;
using System.Threading;
using System.Windows.Markup;

namespace PlexShareTests.ContentTests.Client
{
    public class FileClientTests
    {
        [Fact]
        public void SendFile_ValidInput_ReturnsValidContentData()
        {
            var utility = new Utility();
            var currentDirectory = Directory.GetCurrentDirectory();
            var pathArray = currentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            string path = pathArray[0] + "\\PlexShareTests\\ContentTests\\Utility.cs";
            var userID = 5;
            var sendContentData = utility.GenerateSendContentData(MessageType.File, path);
            var fakeCommunicator = utility.GetFakeCommunicator();
            IContentSerializer serializer = new ContentSerializer();
            var fileClient = new FileClient(fakeCommunicator);
            fileClient.UserID = userID;
            fileClient.Communicator = fakeCommunicator;
            var fileData = new SendFileData(path);

            fileClient.SendFile(sendContentData);
            var serializedData = fakeCommunicator.GetSentData();
            var deserializedData = serializer.Deserialize<ContentData>(serializedData);

            Assert.IsType<ContentData>(deserializedData);
            Assert.Equal(sendContentData.Type, deserializedData.Type);
            Assert.Equal(sendContentData.Data, deserializedData.Data);
            Assert.Equal(sendContentData.ReceiverIDs, deserializedData.ReceiverIDs);
            Assert.Equal(sendContentData.ReplyThreadID, deserializedData.ReplyThreadID);
            Assert.Equal(fileData.DataInFile, deserializedData.FileData.DataInFile);
            Assert.Equal(fileData.Name, deserializedData.FileData.Name);
            Assert.Equal(fileData.Size, deserializedData.FileData.Size);
            Assert.Equal(userID, deserializedData.SenderID);
            Assert.Equal(MessageEvent.New, deserializedData.Event);
        }

        [Fact]
        public void SendFile_InvalidMessageType_ReturnsArgumentException()
        {
            var utility = new Utility();
            var userID = 5;
            var sendContentData = utility.GenerateSendContentData(MessageType.Chat, "");
            var fakeCommunicator = utility.GetFakeCommunicator();
            var fileClient = new FileClient(fakeCommunicator);
            fileClient.UserID = userID;
            fileClient.Communicator = fakeCommunicator;

            Assert.Throws<ArgumentException>(() => fileClient.SendFile(sendContentData));
        }

        [Fact]
        public void SendFile_EmptyFilePath_ReturnsFileNotFoundException()
        {
            var utility = new Utility();
            var userID = 5;
            var sendContentData = utility.GenerateSendContentData(MessageType.File, "");
            var fakeCommunicator = utility.GetFakeCommunicator();
            var fileClient = new FileClient(fakeCommunicator);
            fileClient.UserID = userID;
            fileClient.Communicator = fakeCommunicator;

            Assert.Throws<FileNotFoundException>(() => fileClient.SendFile(sendContentData));
        }

        [Fact]
        public void SendFile_NullFilePath_ReturnsFileNotFoundException()
        {
            var utility = new Utility();
            var userID = 5;
            var sendContentData = utility.GenerateSendContentData(MessageType.File, null);
            var fakeCommunicator = utility.GetFakeCommunicator();
            var fileClient = new FileClient(fakeCommunicator);
            fileClient.UserID = userID;
            fileClient.Communicator = fakeCommunicator;

            Assert.Throws<FileNotFoundException>(() => fileClient.SendFile(sendContentData));
        }

        [Fact]
        public void DownloadFile_ValidInput_ReturnsValidContentData()
        {
            var utility = new Utility();
            var currentDirectory = Directory.GetCurrentDirectory();
            var pathArray = currentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            string path = pathArray[0] + "\\PlexShareTests\\ContentTests\\Utility.cs";
            var userID = 5;
            var messageID = 6;
            var fakeCommunicator = utility.GetFakeCommunicator();
            IContentSerializer serializer = new ContentSerializer();
            var fileClient = new FileClient(fakeCommunicator);
            fileClient.UserID = userID;
            fileClient.Communicator = fakeCommunicator;

            fileClient.DownloadFile(messageID, path);
            var serializedData = fakeCommunicator.GetSentData();
            var deserializedData = serializer.Deserialize<ContentData>(serializedData);

            Assert.IsType<ContentData>(deserializedData);
            Assert.Equal(MessageType.File, deserializedData.Type);
            Assert.Equal(path, deserializedData.Data);
            Assert.Equal(userID, deserializedData.SenderID);
            Assert.Equal(messageID, deserializedData.MessageID);
            Assert.Equal(MessageEvent.Download, deserializedData.Event);
        }

        [Fact]
        public void DownloadFile_EmptySavePath_ReturnsFileNotFoundException()
        {
            var utility = new Utility();
            var userID = 5;
            var messageID = 6;
            var fakeCommunicator = utility.GetFakeCommunicator();
            var fileClient = new FileClient(fakeCommunicator);
            fileClient.UserID = userID;
            fileClient.Communicator = fakeCommunicator;

            Assert.Throws<ArgumentException>(() => fileClient.DownloadFile(messageID, ""));
        }

        [Fact]
        public void DownloadFile_NullSavePath_ReturnsFileNotFoundException()
        {
            var utility = new Utility();
            var userID = 5;
            var messageID = 6;
            var fakeCommunicator = utility.GetFakeCommunicator();
            var fileClient = new FileClient(fakeCommunicator);
            fileClient.UserID = userID;
            fileClient.Communicator = fakeCommunicator;

            Assert.Throws<ArgumentException>(() => fileClient.DownloadFile(messageID, null));
        }

    }
}
