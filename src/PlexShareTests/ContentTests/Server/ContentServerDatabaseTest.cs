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
    public class ContentServerDatabaseTest
    {
        private Utils _utils = new Utils();
        private ContentDB MessageDatabase = new ContentDB();

        [Fact]
        public void FileStore_StoringAFile_ShouldBeAbleToFileStore()
        {
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

            var recv = MessageDatabase.FileStore(file1);

            Assert.Same(file1.Data, recv.Data);
        }

        [Fact]
        public void FilesFetch_StoringAndFetchingAFileFromMessageDatabase_ShouldBeAbleToFetchStoredFile()
        {
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

            var recv = MessageDatabase.FileStore(file1);

            Assert.Same(file1.Data, recv.Data);

            recv = MessageDatabase.FilesFetch(file1.MessageID);

            Assert.Same(file1.Data, recv.Data);
            
            recv = MessageDatabase.FilesFetch(file1.MessageID);
            
            Assert.Same(file1.Data, recv.Data);
            
        }

        [Fact]
        public void FilesFetch_TryingToFetchAFileThatDoesNotExist_NullShouldBeReturned()
        {
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

            var recv = MessageDatabase.FileStore(file1);

            Assert.Same(file1.Data, recv.Data);

            recv = MessageDatabase.FilesFetch(10);

            Assert.Null(recv);
        }

        [Fact]
        public void FileStore_StoringMultipleFiles_ShouldBeAbleToStoreAndFetchMultipleFiles()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();

            var path = CurrentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlexShareTests\\ContentTests\\Test_File.pdf";

            var pathB = path[0] + "\\PlexShareTests\\ContentTests\\Utils.cs";

            var file1 = new ContentData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            var recv = MessageDatabase.FileStore(file1);

            Assert.Same(file1.Data, recv.Data);

            var file2 = new ContentData
            {
                Data = "Utils.cs",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            recv = MessageDatabase.FileStore(file2);

            Assert.Same(file2.Data, recv.Data);

            var file3 = new ContentData
            {
                Data = "c.txt",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = file1.ReplyThreadID,
                Event = MessageEvent.New
            };

            recv = MessageDatabase.FileStore(file3);

            Assert.Same(file3.Data, recv.Data);

            recv = MessageDatabase.FilesFetch(file1.MessageID);

            Assert.Same(file1.Data, recv.Data);

            recv = MessageDatabase.FilesFetch(file2.MessageID);

            Assert.Same(file2.Data, recv.Data);

            recv = MessageDatabase.FilesFetch(file3.MessageID);

            Assert.Same(file3.Data, recv.Data);
        }

        [Fact]
        public void MessageStore_StoringASingleMessage_MessageShouldBeStored()
        {
            var message1 = _utils.GenerateNewMessageData("Hello", SenderID: 1);

            var recv = MessageDatabase.MessageStore(message1);

            Assert.Same(message1.Data, recv.Data);
            Assert.Null(recv.FileData);
        }

        [Fact]
        public void GetMessage_StoringAndFetchingAMessage_ShouldBeAbleToFetchStoredMessage()
        {
            var message1 = _utils.GenerateNewMessageData("Hello", SenderID: 1);

            var recv = MessageDatabase.MessageStore(message1);

            Assert.Same(message1.Data, recv.Data);
            Assert.Null(recv.FileData);

            var msg = MessageDatabase.GetMessage(message1.ReplyThreadID, message1.MessageID);

            Assert.Equal(message1.Data, msg.Data);
            Assert.Equal(msg.MessageID, message1.MessageID);
            Assert.Equal(message1.Type, msg.Type);
            Assert.Equal(message1.SenderID, msg.SenderID);
            Assert.Equal(message1.Event, msg.Event);
            Assert.Equal(message1.ReplyThreadID, msg.ReplyThreadID);
           
        }

        [Fact]
        public void GetMessage_FetchingAnInvalidMessage_NullShouldBeReturned()
        {
            var message1 = _utils.GenerateNewMessageData("Hello", SenderID: 1);

            var message2 = _utils.GenerateNewMessageData("Hello2", SenderID: 1);

            var recv = MessageDatabase.MessageStore(message1);

            Assert.Same(message1.Data, recv.Data);

            recv = MessageDatabase.MessageStore(message2);

            Assert.Same(message2.Data, recv.Data);

            var message3 = _utils.GenerateNewMessageData("Hello3", SenderID: 1, ReplyThreadID: message1.ReplyThreadID);

            recv = MessageDatabase.MessageStore(message3);

            Assert.Same(message3.Data, recv.Data);
        }

        [Fact]
        public void MessageStore_StoringMultipleMessages_ShouldBeAbleToStoreAndFetchMultipleMessages()
        {
            var message1 = _utils.GenerateNewMessageData("Hello", SenderID: 1);

            var recv = MessageDatabase.MessageStore(message1);

            Assert.Same(message1.Data, recv.Data);

            var message2 = _utils.GenerateNewMessageData("Hello2", SenderID: 1, ReplyThreadID: message1.ReplyThreadID);

            recv = MessageDatabase.MessageStore(message2);

            Assert.Same(message2.Data, recv.Data);

            var message3 = _utils.GenerateNewMessageData("Hello3", SenderID: 1);

            recv = MessageDatabase.MessageStore(message3);

            Assert.Same(message3.Data, recv.Data);

            var msg = MessageDatabase.GetMessage(message1.ReplyThreadID, message1.MessageID);

            Assert.Same(message1.Data, msg.Data);

            msg = MessageDatabase.GetMessage(message2.ReplyThreadID, message2.MessageID);

            Assert.Same(message2.Data, msg.Data);
            msg = MessageDatabase.GetMessage(message3.ReplyThreadID, message3.MessageID);

            Assert.Same(message3.Data, msg.Data);
        }

        [Fact]
        public void
            GetChatContexts_StoringMultipleFilesAndMessages_ShouldBeAbleToFetchAllChatContextsAndMessagesAreInCorrectChatContexts()
        {
            MessageDatabase = new ContentDB();

            FileStore_StoringMultipleFiles_ShouldBeAbleToStoreAndFetchMultipleFiles();
            MessageStore_StoringMultipleMessages_ShouldBeAbleToStoreAndFetchMultipleMessages();

            var msgList = MessageDatabase.GetChatContexts();

            foreach (var m in msgList)
                foreach (var n in m.MessageList)
                    Assert.Same(n, MessageDatabase.GetMessage(m.ThreadID, n.MessageID));
        }
    }
}
