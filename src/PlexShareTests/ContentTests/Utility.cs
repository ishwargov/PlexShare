/******************************************************************************
 * Filename    = Utility.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class containing helper functions used in unit testing
 *****************************************************************************/

using PlexShareContent;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;

namespace PlexShareTests.ContentTests
{
    public class Utility
    {
        private readonly FakeCommunicator _fakeCommunicator;

        /// <summary>
        /// Constructor to instantiate fake communicator
        /// </summary>
        public Utility()
        {
            _fakeCommunicator = new FakeCommunicator();
        }

        /// <summary>
        /// Gets the fake communicator instance
        /// </summary>
        /// <returns></returns>
        public FakeCommunicator GetFakeCommunicator()
        {
            return _fakeCommunicator;
        }

        /// <summary>
        /// Generates an object of the SendContentData class 
        /// </summary>
        /// <param name="data">Message string</param>
        /// <param name="receiverIDs">List of receiver IDs</param>
        /// <param name="replyThreadID">ID of thread the message belongs to</param>
        /// <param name="type">Type of message - Chat or File</param>
        /// <returns></returns>
        public SendContentData GenerateSendContentData(
            MessageType type = MessageType.Chat,
            string data = "This is a sample message",
            int[] receiverIDs = null,
            int replyThreadID = -1,
            int replyMessageID = -1
        )
        {
            if(receiverIDs == null)
            {
                receiverIDs = new int[0];
            }
            var sendContentData = new SendContentData();
            sendContentData.Data = data;
            sendContentData.ReceiverIDs = receiverIDs;
            sendContentData.ReplyThreadID = replyThreadID;
            sendContentData.Type = type;
            sendContentData.ReplyMessageID = replyMessageID;
            return sendContentData;
        }

        /// <summary>
        /// Generates an object of the ReceiveContentData class 
        /// </summary>
        /// <param name="type">Type of message - Chat or File</param>
        /// <param name="event">Message event - New, Edit, Delete, etc.</param>
        /// <param name="data">Message string</param>
        /// <param name="messageID">ID of the message</param>
        /// <param name="receiverIDs">List of receiver IDs</param>
        /// <param name="replyThreadID">ID of thread the message belongs to</param>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="starred">Boolean for starred message</param>
        /// <returns>Object of the ReceiveContentData class</returns>
        public ReceiveContentData GenerateReceiveContentData(
            MessageType type = MessageType.Chat,
            MessageEvent @event = MessageEvent.New,
            string data = "This is a sample message",
            int messageID = -1,
            int[] receiverIDs = null,
            int replyThreadID = -1,
            int senderID = -1,
            bool starred = false,
            int replyMessageID = -1
        )
        {
            if (receiverIDs == null)
            {
                receiverIDs = new int[0];
            }
            var receiveContentData = new ReceiveContentData();
            receiveContentData.Event = @event;
            receiveContentData.Data = data;
            receiveContentData.MessageID = messageID;
            receiveContentData.ReceiverIDs = receiverIDs;
            receiveContentData.SenderID = senderID;
            receiveContentData.ReplyThreadID = replyThreadID;
            receiveContentData.Starred = starred;
            receiveContentData.Type = type;
            receiveContentData.ReplyMessageID = replyMessageID;
            return receiveContentData;
        }

        /// <summary>
        /// Generates an object of the ContentData class 
        /// </summary>
        /// <param name="type">Type of message - Chat or File</param>
        /// <param name="event">Message event - New, Edit, Delete, etc.</param>
        /// <param name="data">Message string</param>
        /// <param name="messageID">ID of the message</param>
        /// <param name="receiverIDs">List of receiver IDs</param>
        /// <param name="replyThreadID">ID of thread the message belongs to</param>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="starred">Boolean for starred message</param>
        /// <returns>Object of the ContentData class</returns>
        public ContentData GenerateContentData(
            MessageType type = MessageType.Chat,
            MessageEvent @event = MessageEvent.New,
            string data = "This is a sample message", 
            int messageID = -1, 
            int[] receiverIDs = null,
            int replyThreadID = -1, 
            int senderID = -1, 
            bool starred = false,
            int replyMessageID = -1
        )
        {
            if (receiverIDs == null)
            {
                receiverIDs = new int[0];
            }
            var newContentData = new ContentData
            {
                Event = @event,
                Data = data,
                MessageID = messageID,
                ReceiverIDs = receiverIDs,
                SenderID = senderID,
                ReplyThreadID = replyThreadID,
                Starred = starred,
                Type = type,
                ReplyMessageID = replyMessageID
            };
            return newContentData;
        }

        /// <summary>
        /// Checks if two ReceiveContentData objects are equal
        /// </summary>
        /// <param name="rcdata1">ReceiveContentData object</param>
        /// <param name="rcdata2">ReceiveContentData objeect</param>
        public void CheckReceiveContentData(ReceiveContentData rcdata1, ReceiveContentData rcdata2)
        {
            Assert.Equal(rcdata1.Type, rcdata2.Type);
            Assert.Equal(rcdata1.Data, rcdata2.Data);
            Assert.Equal(rcdata1.MessageID, rcdata2.MessageID);
            Assert.Equal(rcdata1.ReceiverIDs, rcdata2.ReceiverIDs);
            Assert.Equal(rcdata1.SenderID, rcdata2.SenderID);
            Assert.Equal(rcdata1.ReplyMessageID, rcdata2.ReplyMessageID);
            Assert.Equal(rcdata1.ReplyThreadID, rcdata2.ReplyThreadID);
            Assert.Equal(rcdata1.SenderID, rcdata2.SenderID);
            Assert.Equal(rcdata1.Starred, rcdata2.Starred);
            Assert.Equal(rcdata1.Event, rcdata2.Event);
        }

        /// <summary>
        /// Checks if two chat threads are similar
        /// </summary>
        /// <param name="thread1">ChatThread object</param>
        /// <param name="thread2">ChatThread object</param>
        public void CheckChatThreads(ChatThread thread1, ChatThread thread2)
        {
            Assert.Equal(thread1.ThreadID, thread2.ThreadID);
            Assert.Equal(thread1.MessageList.Count, thread2.MessageList.Count);
            for(int i = 0; i < thread1.MessageList.Count; i++)
            {
                CheckReceiveContentData(thread1.MessageList[i], thread2.MessageList[i]);
            }
        }

        /// <summary>
        /// Checks if two lists of chat threads are similar
        /// </summary>
        /// <param name="list1">List of chat threads</param>
        /// <param name="list2">List of chat threads</param>
        public void CheckChatThreadLists(List<ChatThread> list1, List<ChatThread> list2)
        {
            Assert.Equal(list1.Count, list2.Count);
            for(int i = 0; i < list1.Count; i++)
            {
                CheckChatThreads(list1[i], list2[i]);
            }
        }
    }

    
}
