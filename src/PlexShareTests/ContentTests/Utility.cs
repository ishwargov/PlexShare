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
        /// Generates a content data object with message event set as 'New'
        /// </summary>
        /// <param name="message">Message string</param>
        /// <param name="messageID">ID of the message</param>
        /// <param name="receiverIDs">List of receiver IDs</param>
        /// <param name="replyThreadID">ID of thread the message belongs to</param>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="starred">Boolean for starred message</param>
        /// <param name="type">Type of message - Chat or File</param>
        /// <returns></returns>
        public ContentData GenerateNewContentData(
            string message, 
            int messageID = 1, 
            int[] receiverIDs = null,
            int replyThreadID = -1, 
            int senderID = -1, 
            bool starred = false, 
            MessageType type = MessageType.Chat
        )
        {
            if (receiverIDs == null) receiverIDs = new int[0];
            var msg = new ContentData
            {
                Event = MessageEvent.New,
                Data = message,
                MessageID = messageID,
                ReceiverIDs = receiverIDs,
                SenderID = senderID,
                ReplyThreadID = replyThreadID,
                Starred = starred,
                Type = type
            };
            return msg;
        }
    }
}
