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
            var path = CurrentDirectory.Split(new[] { "\\PlesShareTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\PlesShareTests\\ContentTests\\Test_File.pdf";

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
            Assert.Same(file1.Type, recv.Type);
            Assert.Same(file1.SenderID, recv.SenderID);
            Assert.Same(file1.Event, recv.Event);
            Assert.Same(file1.FileData.Size, recv.FileData.Size);
            Assert.Same(file1.FileData.Name, recv.FileData.Name);
            Assert.Same(file1.FileData.Data, recv.FileData.Data);
        }

        [Fact]
        public void FilesFetch_StoringAndFetchingAFileFromMessageDatabase_ShouldBeAbleToFetchStoredFile()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\Testing" }, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

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
            Assert.Same(file1.Type, recv.Type);
            Assert.Same(file1.SenderID, recv.SenderID);
            Assert.Same(file1.Event, recv.Event);
            Assert.Same(file1.FileData.Size, recv.FileData.Size);
            Assert.Same(file1.FileData.Name, recv.FileData.Name);
            Assert.Same(file1.FileData.Data, recv.FileData.Data);

            recv = MessageDatabase.FilesFetch(file1.MessageID);

            Assert.Same(file1.Data, recv.Data);
            Assert.Same(file1.MessageID, recv.MessageID);
            Assert.Same(file1.Type, recv.Type);
            Assert.Same(file1.SenderID, recv.SenderID);
            Assert.Same(file1.Event, recv.Event);
            Assert.Same(file1.FileData.Size, recv.FileData.Size);
            Assert.Same(file1.FileData.Name, recv.FileData.Name);
            Assert.Same(file1.FileData.Data, recv.FileData.Data);
            Assert.Same(file1.ReplyThreadID, recv.ReplyThreadID);
        }

        [Fact]
        public void FilesFetch_TryingToFetchAFileThatDoesNotExist_NullShouldBeReturned()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\Testing" }, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

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
            Assert.Same(file1.Type, recv.Type);
            Assert.Same(file1.SenderID, recv.SenderID);
            Assert.Same(file1.Event, recv.Event);
            Assert.Same(file1.FileData.Size, recv.FileData.Size);
            Assert.Same(file1.FileData.Name, recv.FileData.Name);
            Assert.Same(file1.FileData.Data, recv.FileData.Data);

            recv = MessageDatabase.FilesFetch(10);

            Assert.Null(recv);
        }

        [Fact]
        public void FileStore_StoringMultipleFiles_ShouldBeAbleToStoreAndFetchMultipleFiles()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\Testing" }, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            var pathB = path[0] + "\\Testing\\Content\\Utils.cs";

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
            Assert.Same(file1.Type, recv.Type);
            Assert.Same(file1.SenderID, recv.SenderID);
            Assert.Same(file1.Event, recv.Event);
            Assert.Same(file1.FileData.Size, recv.FileData.Size);
            Assert.Same(file1.FileData.Name, recv.FileData.Name);
            Assert.Same(file1.FileData.Data, recv.FileData.Data);

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
            Assert.NotSame(recv.MessageID, file1.MessageID);
            Assert.NotSame(recv.ReplyThreadID, file1.ReplyThreadID);
            Assert.Same(file2.Type, recv.Type);
            Assert.Same(file2.SenderID, recv.SenderID);
            Assert.Same(file2.Event, recv.Event);
            Assert.Same(file2.FileData.Size, recv.FileData.Size);
            Assert.Same(file2.FileData.Name, recv.FileData.Name);
            Assert.Same(file2.FileData.Data, recv.FileData.Data);

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
            Assert.NotSame(recv.MessageID, file1.MessageID);
            Assert.NotSame(recv.MessageID, file2.MessageID);
            Assert.Same(recv.ReplyThreadID, file1.ReplyThreadID);
            Assert.Same(file3.Type, recv.Type);
            Assert.Same(file3.SenderID, recv.SenderID);
            Assert.Same(file3.Event, recv.Event);
            Assert.Same(file3.FileData.Size, recv.FileData.Size);
            Assert.Same(file3.FileData.Name, recv.FileData.Name);
            Assert.Same(file3.FileData.Data, recv.FileData.Data);

            recv = MessageDatabase.FilesFetch(file1.MessageID);

            Assert.Same(file1.Data, recv.Data);
            Assert.Same(file1.MessageID, recv.MessageID);
            Assert.Same(file1.Type, recv.Type);
            Assert.Same(file1.SenderID, recv.SenderID);
            Assert.Same(file1.Event, recv.Event);
            Assert.Same(file1.FileData.Size, recv.FileData.Size);
            Assert.Same(file1.FileData.Name, recv.FileData.Name);
            Assert.Same(file1.FileData.Data, recv.FileData.Data);
            Assert.Same(file1.ReplyThreadID, recv.ReplyThreadID);

            recv = MessageDatabase.FilesFetch(file2.MessageID);

            Assert.Same(file2.Data, recv.Data);
            Assert.Same(file2.MessageID, recv.MessageID);
            Assert.Same(file2.Type, recv.Type);
            Assert.Same(file2.SenderID, recv.SenderID);
            Assert.Same(file2.Event, recv.Event);
            Assert.Same(file2.FileData.Size, recv.FileData.Size);
            Assert.Same(file2.FileData.Name, recv.FileData.Name);
            Assert.Same(file2.FileData.Data, recv.FileData.Data);
            Assert.Same(file2.ReplyThreadID, recv.ReplyThreadID);

            recv = MessageDatabase.FilesFetch(file3.MessageID);

            Assert.Same(file3.Data, recv.Data);
            Assert.Same(file3.MessageID, recv.MessageID);
            Assert.Same(file3.Type, recv.Type);
            Assert.Same(file3.SenderID, recv.SenderID);
            Assert.Same(file3.Event, recv.Event);
            Assert.Same(file3.FileData.Size, recv.FileData.Size);
            Assert.Same(file3.FileData.Name, recv.FileData.Name);
            Assert.Same(file3.FileData.Data, recv.FileData.Data);
            Assert.Same(file3.ReplyThreadID, recv.ReplyThreadID);
        }

        [Fact]
        public void MessageStore_StoringASingleMessage_MessageShouldBeStored()
        {
            var message1 = _utils.GenerateNewData("Hello", SenderID: 1);

            var recv = MessageDatabase.MessageStore(message1);

            Assert.Same(message1.Data, recv.Data);
            Assert.Same(message1.Type, recv.Type);
            Assert.Same(message1.SenderID, recv.SenderID);
            Assert.Same(message1.Event, recv.Event);
            Assert.Null(recv.FileData);
        }

        [Fact]
        public void GetMessage_StoringAndFetchingAMessage_ShouldBeAbleToFetchStoredMessage()
        {
            var message1 = _utils.GenerateNewData("Hello", SenderID: 1);

            var recv = MessageDatabase.MessageStore(message1);

            Assert.Same(message1.Data, recv.Data);
            Assert.Same(message1.Type, recv.Type);
            Assert.Same(message1.SenderID, recv.SenderID);
            Assert.Same(message1.Event, recv.Event);
            Assert.Null(recv.FileData);

            var msg = MessageDatabase.GetMessage(message1.ReplyThreadID, message1.MessageID);

            Assert.Same(message1.Data, msg.Data);
            Assert.Same(msg.MessageID, message1.MessageID);
            Assert.Same(message1.Type, msg.Type);
            Assert.Same(message1.SenderID, msg.SenderID);
            Assert.Same(message1.Event, msg.Event);
            Assert.Same(message1.ReplyThreadID, msg.ReplyThreadID);
        }

        [Fact]
        public void GetMessage_FetchingAnInvalidMessage_NullShouldBeReturned()
        {
            var message1 = _utils.GenerateNewData("Hello", SenderID: 1);

            var message2 = _utils.GenerateNewData("Hello2", SenderID: 1);

            var recv = MessageDatabase.MessageStore(message1);

            Assert.Same(message1.Data, recv.Data);
            Assert.Same(message1.Type, recv.Type);
            Assert.Same(message1.SenderID, recv.SenderID);
            Assert.Same(message1.Event, recv.Event);
            Assert.Null(recv.FileData);

            recv = MessageDatabase.MessageStore(message2);

            Assert.Same(message2.Data, recv.Data);
            Assert.Same(message2.Type, recv.Type);
            Assert.Same(message2.SenderID, recv.SenderID);
            Assert.Same(message2.Event, recv.Event);
            Assert.Null(recv.FileData);

            var message3 = _utils.GenerateNewData("Hello3", SenderID: 1, ReplyThreadID: message1.ReplyThreadID);

            recv = MessageDatabase.MessageStore(message3);

            Assert.Same(message3.Data, recv.Data);
            Assert.Same(message3.Type, recv.Type);
            Assert.Same(message3.SenderID, recv.SenderID);
            Assert.Same(message3.Event, recv.Event);
            Assert.Null(recv.FileData);

            var msg = MessageDatabase.GetMessage(10, message1.MessageID);
            Assert.Null(msg);

            msg = MessageDatabase.GetMessage(message1.ReplyThreadID, 10);
            Assert.Null(msg);

            msg = MessageDatabase.GetMessage(message2.ReplyThreadID, 2);
            Assert.Null(msg);
        }

        [Fact]
        public void MessageStore_StoringMultipleMessages_ShouldBeAbleToStoreAndFetchMultipleMessages()
        {
            var message1 = _utils.GenerateNewData("Hello", SenderID: 1);

            var recv = MessageDatabase.MessageStore(message1);

            Assert.Same(message1.Data, recv.Data);
            Assert.Same(message1.Type, recv.Type);
            Assert.Same(message1.SenderID, recv.SenderID);
            Assert.Same(message1.Event, recv.Event);
            Assert.Null(recv.FileData);

            var message2 = _utils.GenerateNewData("Hello2", SenderID: 1, ReplyThreadID: message1.ReplyThreadID);

            recv = MessageDatabase.MessageStore(message2);

            Assert.Same(message2.Data, recv.Data);
            Assert.NotSame(message2.MessageID, message1.MessageID);
            Assert.Same(message2.Type, recv.Type);
            Assert.Same(message2.SenderID, recv.SenderID);
            Assert.Same(message2.Event, recv.Event);
            Assert.Null(recv.FileData);
            Assert.Same(message2.ReplyThreadID, recv.ReplyThreadID);

            var message3 = _utils.GenerateNewData("Hello3", SenderID: 1);

            recv = MessageDatabase.MessageStore(message3);

            Assert.Same(message3.Data, recv.Data);
            Assert.NotSame(message3.MessageID, message2.MessageID);
            Assert.NotSame(message3.MessageID, message1.MessageID);
            Assert.Same(message3.Type, recv.Type);
            Assert.Same(message3.SenderID, recv.SenderID);
            Assert.Same(message3.Event, recv.Event);
            Assert.Null(recv.FileData);
            Assert.NotSame(message2.ReplyThreadID, recv.ReplyThreadID);
            Assert.NotSame(message1.ReplyThreadID, recv.ReplyThreadID);

            var msg = MessageDatabase.GetMessage(message1.ReplyThreadID, message1.MessageID);

            Assert.Same(message1.Data, msg.Data);
            Assert.Same(msg.MessageID, message1.MessageID);
            Assert.Same(message1.Type, msg.Type);
            Assert.Same(message1.SenderID, msg.SenderID);
            Assert.Same(message1.Event, msg.Event);
            Assert.Same(message1.ReplyThreadID, msg.ReplyThreadID);

            msg = MessageDatabase.GetMessage(message2.ReplyThreadID, message2.MessageID);

            Assert.Same(message2.Data, msg.Data);
            Assert.Same(message2.MessageID, msg.MessageID);
            Assert.Same(message2.Type, msg.Type);
            Assert.Same(message2.SenderID, msg.SenderID);
            Assert.Same(message2.Event, msg.Event);
            Assert.Same(message2.ReplyThreadID, msg.ReplyThreadID);

            msg = MessageDatabase.GetMessage(message3.ReplyThreadID, message3.MessageID);

            Assert.Same(message3.Data, msg.Data);
            Assert.Same(message3.MessageID, msg.MessageID);
            Assert.Same(message3.Type, msg.Type);
            Assert.Same(message3.SenderID, msg.SenderID);
            Assert.Same(message3.Event, msg.Event);
            Assert.Same(message3.ReplyThreadID, msg.ReplyThreadID);
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
