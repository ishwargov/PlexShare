/******************************************************************************
 * Filename    = FakeContentNotificationHandler.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareTests
 *
 * Description = Fake Notification handler that handles received data from server   
 *****************************************************************************/

using PlexShareContent.Client;
using PlexShareContent.DataModels;

namespace PlexShareTests.ContentTests
{
    public class FakeContentNotificationHandler : ContentClientNotificationHandler
    {
        // member varialbes to store received message and all messages
        private ContentData _contentData;
        private List<ChatThread> _chatThreads;

        /// <summary>
        /// constructor that calls base class ContentClient constructor
        /// </summary>
        /// <param name="contentClient"></param>
        public FakeContentNotificationHandler(IContentClient contentClient) : base(contentClient)
        {
            _contentData = new ContentData();
            _chatThreads = new List<ChatThread>();
        }

        /// <summary>
        /// Reset member variables
        /// </summary>
        private void Reset()
        {
            _contentData = new ContentData();
            _chatThreads = new List<ChatThread>();
        }

        /// <summary>
        /// Gets the received message 
        /// </summary>
        /// <returns>Object of ContentData class</returns>
        public ContentData GetReceivedMessage()
        {
            Reset();
            _contentData = _receivedMessage;
            return _contentData;
        }

        /// <summary>
        /// Gets list of chat threads containing all messages
        /// </summary>
        /// <returns>List of chat threads</returns>
        public List<ChatThread> GetAllMessages()
        {
            Reset();
            _chatThreads = _allMessages;
            return _chatThreads;
        }
    }
}
