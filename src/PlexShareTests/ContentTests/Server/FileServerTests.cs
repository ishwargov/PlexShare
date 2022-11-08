using PlexShareContent;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using PlexShareContent.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.ContentTests.Server
{
    public class FileServerTests
    {
        private ContentDB database;
        private FileServer fileServer;

        public void Setup()
        {
            database = new ContentDB();
            fileServer = new FileServer(database);
        }

        [Fact]
        public void Receive_StoringFile_ShouldBeAbleToStoreFile()
        {
            Setup();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            var file1 = new ContentData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            var recv = fileServer.Receive(file1);

            Assert.Equal(file1.Data, recv.Data);
            Assert.Equal(file1.Type, recv.Type);
            Assert.Equal(file1.SenderID, recv.SenderID);
            Assert.Equal(file1.Event, recv.Event);
            Assert.Null(recv.FileData);
        }

        [Fact]
        public void Receive_FetchingFile_ShouldBeAbleToFetchAStoredFile()
        {
            Setup();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            var file1 = new ContentData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            var recv = fileServer.Receive(file1);

            Assert.Equal(file1.Data, recv.Data);
            Assert.Equal(file1.Type, recv.Type);
            Assert.Equal(file1.SenderID, recv.SenderID);
            Assert.Equal(file1.Event, recv.Event);
            Assert.Null(recv.FileData);

            var file = new ContentData
            {
                Data = "Test_File.pdf",
                SenderID = 1,
                MessageID = recv.MessageID,
                Type = MessageType.File,
                Event = MessageEvent.Download
            };

            recv = fileServer.Receive(file);

            Assert.Equal(file1.Data, recv.Data);
            Assert.Equal(file1.MessageID, recv.MessageID);
            Assert.Equal(file1.Type, recv.Type);
            Assert.Equal(file1.SenderID, recv.SenderID);
            Assert.Equal(MessageEvent.Download, recv.Event);
            Assert.Equal(file1.FileData.Size, recv.FileData.Size);
            Assert.Equal(file1.FileData.Name, recv.FileData.Name);
            Assert.Equal(file1.FileData.Data, recv.FileData.Data);
            Assert.Equal(file1.ReplyThreadID, recv.ReplyThreadID);
        }

        [Fact]
        public void Receive_GivingInvalidEventForFileType_NullShouldBeReturned()
        {
            Setup();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            var file1 = new ContentData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            var recv = fileServer.Receive(file1);

            Assert.Equal(file1.Data, recv.Data);
            Assert.Equal(file1.Type, recv.Type);
            Assert.Equal(file1.SenderID, recv.SenderID);
            Assert.Equal(file1.Event, recv.Event);
            Assert.Null(recv.FileData);

            var file = new ContentData
            {
                MessageID = 0,
                Type = MessageType.File,
                Event = MessageEvent.Star
            };

            recv = fileServer.Receive(file);
            Assert.Null(recv);

            file = new ContentData
            {
                MessageID = 0,
                Type = MessageType.File,
                Event = MessageEvent.Edit
            };

            recv = fileServer.Receive(file);
            Assert.Null(recv);
        }

        [Fact]
        public void Receive_FetchingAFilesThatDoesNotExist_NullShouldBeReturned()
        {
            Setup();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            var file1 = new ContentData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            var recv = fileServer.Receive(file1);

            Assert.Equal(file1.Data, recv.Data);
            Assert.Equal(file1.Type, recv.Type);
            Assert.Equal(file1.SenderID, recv.SenderID);
            Assert.Equal(file1.Event, recv.Event);
            Assert.Null(recv.FileData);

            var file = new ContentData
            {
                MessageID = 10,
                Type = MessageType.File,
                Event = MessageEvent.Download
            };

            recv = fileServer.Receive(file);
            Assert.Null(recv);
        }

        [Fact]
        public void Receive_StoringAndFetchingMultipleFiles_ShouldBeAbleToStoreFilesAndFetchThem()
        {
            Setup();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";
            var pathB = path[0] + "\\PlexShareTests\\ContentTests\\Utility.cs";

            var file1 = new ContentData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            var recv = fileServer.Receive(file1);

            Assert.Equal(file1.Data, recv.Data);
            Assert.Equal(file1.Type, recv.Type);
            Assert.Equal(file1.SenderID, recv.SenderID);
            Assert.Equal(file1.Event, recv.Event);
            Assert.Null(recv.FileData);

            var file2 = new ContentData
            {
                Data = "Utility.cs",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            recv = fileServer.Receive(file2);

            Assert.Equal(file2.Data, recv.Data);
            Assert.NotEqual(recv.MessageID, file1.MessageID);
            Assert.NotEqual(recv.ReplyThreadID, file1.ReplyThreadID);
            Assert.Equal(file2.Type, recv.Type);
            Assert.Equal(file2.SenderID, recv.SenderID);
            Assert.Equal(file2.Event, recv.Event);
            Assert.Null(recv.FileData);

            var file3 = new ContentData
            {
                Data = "c.txt",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = file1.ReplyThreadID,
                Event = MessageEvent.New
            };

            recv = fileServer.Receive(file3);

            Assert.Equal(file3.Data, recv.Data);
            Assert.NotEqual(recv.MessageID, file1.MessageID);
            Assert.NotEqual(recv.MessageID, file2.MessageID);
            Assert.Equal(recv.ReplyThreadID, file1.ReplyThreadID);
            Assert.Equal(file3.Type, recv.Type);
            Assert.Equal(file3.SenderID, recv.SenderID);
            Assert.Equal(file3.Event, recv.Event);
            Assert.Null(recv.FileData);

            file1.Event = MessageEvent.Download;
            recv = fileServer.Receive(file1);

            Assert.Equal(file1.Data, recv.Data);
            Assert.Equal(file1.MessageID, recv.MessageID);
            Assert.Equal(file1.Type, recv.Type);
            Assert.Equal(file1.SenderID, recv.SenderID);
            Assert.Equal(file1.Event, recv.Event);
            Assert.Equal(file1.FileData.Size, recv.FileData.Size);
            Assert.Equal(file1.FileData.Name, recv.FileData.Name);
            Assert.Equal(file1.FileData.Data, recv.FileData.Data);
            Assert.Equal(file1.ReplyThreadID, recv.ReplyThreadID);

            file2.Event = MessageEvent.Download;
            recv = fileServer.Receive(file2);

            Assert.Equal(file2.Data, recv.Data);
            Assert.Equal(file2.MessageID, recv.MessageID);
            Assert.Equal(file2.Type, recv.Type);
            Assert.Equal(file2.SenderID, recv.SenderID);
            Assert.Equal(file2.Event, recv.Event);
            Assert.Equal(file2.FileData.Size, recv.FileData.Size);
            Assert.Equal(file2.FileData.Name, recv.FileData.Name);
            Assert.Equal(file2.FileData.Data, recv.FileData.Data);
            Assert.Equal(file2.ReplyThreadID, recv.ReplyThreadID);

            file3.Event = MessageEvent.Download;
            recv = fileServer.Receive(file3);

            Assert.Equal(file3.Data, recv.Data);
            Assert.Equal(file3.MessageID, recv.MessageID);
            Assert.Equal(file3.Type, recv.Type);
            Assert.Equal(file3.SenderID, recv.SenderID);
            Assert.Equal(file3.Event, recv.Event);
            Assert.Equal(file3.FileData.Size, recv.FileData.Size);
            Assert.Equal(file3.FileData.Name, recv.FileData.Name);
            Assert.Equal(file3.FileData.Data, recv.FileData.Data);
            Assert.Equal(file3.ReplyThreadID, recv.ReplyThreadID);
        }
    }
}
