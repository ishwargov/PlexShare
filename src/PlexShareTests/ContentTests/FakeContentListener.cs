/******************************************************************************
 * Filename    = FakeContentListener.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class that mocks the content listener
 *****************************************************************************/

using PlexShareContent.Client;
using PlexShareContent.DataModels;

namespace PlexShareTests.ContentTests
{
    public class FakeContentListener : IContentListener
    {
        // content listener parameters
        private ReceiveContentData _receivedMessage;
        private List<ChatThread> _allMessages;

        /// <summary>
        /// Constructor to create content listener
        /// </summary>
        public FakeContentListener()
        {
            _receivedMessage = new ReceiveContentData();
            _allMessages = new List<ChatThread>();
        }

        ///<inheritdoc/>
        public void OnMessageReceived(ReceiveContentData messageData)
        {
            _receivedMessage = messageData;
        }

        ///<inheritdoc/>
        public void OnAllMessagesReceived(List<ChatThread> allMessages)
        {
            _allMessages = allMessages;
        }

        /// <summary>
        /// Gets the received message
        /// </summary>
        /// <returns>Received message</returns>
        public ReceiveContentData GetReceivedMessage()
        {
            return _receivedMessage;
        }

        /// <summary>
        /// Gets the list of threads containing all messages
        /// </summary>
        /// <returns>List of threads containing all messages</returns>
        public List<ChatThread> GetAllMessages()
        {
            return _allMessages;
        }
    }
}
