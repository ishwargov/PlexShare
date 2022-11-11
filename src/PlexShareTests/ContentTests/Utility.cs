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
            string data = "Hello",
            int[] receiverIDs = null,
            int replyThreadID = -1
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
            return sendContentData;
        }

        /// <summary>
        /// Generates an object of the ReceiveContentData class 
        /// </summary>
        /// <param name="data">Message string</param>
        /// <param name="messageID">ID of the message</param>
        /// <param name="receiverIDs">List of receiver IDs</param>
        /// <param name="replyThreadID">ID of thread the message belongs to</param>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="starred">Boolean for starred message</param>
        /// <param name="type">Type of message - Chat or File</param>
        /// <returns></returns>
        public ReceiveContentData GenerateReceiveContentData(
            MessageType type = MessageType.Chat,
            string data = "Hello",
            int messageID = -1,
            int[] receiverIDs = null,
            int replyThreadID = -1,
            int senderID = -1,
            bool starred = false
        )
        {
            if (receiverIDs == null)
            {
                receiverIDs = new int[0];
            }
            var receiveContentData = new ReceiveContentData();
            receiveContentData.Event = MessageEvent.New;
            receiveContentData.Data = data;
            receiveContentData.MessageID = messageID;
            receiveContentData.ReceiverIDs = receiverIDs;
            receiveContentData.SenderID = senderID;
            receiveContentData.ReplyThreadID = replyThreadID;
            receiveContentData.Starred = starred;
            receiveContentData.Type = type;
            return receiveContentData;
        }

        /// <summary>
        /// Generates an object of the ContentData class 
        /// </summary>
        /// <param name="data">Message string</param>
        /// <param name="messageID">ID of the message</param>
        /// <param name="receiverIDs">List of receiver IDs</param>
        /// <param name="replyThreadID">ID of thread the message belongs to</param>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="starred">Boolean for starred message</param>
        /// <param name="type">Type of message - Chat or File</param>
        /// <returns></returns>
        public ContentData GenerateContentData(
            MessageType type = MessageType.Chat,
            string data = "Hello", 
            int messageID = 1, 
            int[] receiverIDs = null,
            int replyThreadID = -1, 
            int senderID = -1, 
            bool starred = false
        )
        {
            if (receiverIDs == null)
            {
                receiverIDs = new int[0];
            }
            var newContentData = new ContentData
            {
                Event = MessageEvent.New,
                Data = data,
                MessageID = messageID,
                ReceiverIDs = receiverIDs,
                SenderID = senderID,
                ReplyThreadID = replyThreadID,
                Starred = starred,
                Type = type
            };
            return newContentData;
        }
    }
}
