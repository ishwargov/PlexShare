using PlexShareContent.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareContent;
using PlexShareContent.DataModels;

namespace PlexShareTests.ContentTests
{
    public class FakeContentListener : IContentListener
    {
        private List<ChatThread> _chatContextList;
        private ReceiveContentData _rcvMsgData;

        public FakeContentListener()
        {
            _rcvMsgData = new ReceiveContentData();
            _chatContextList = new List<ChatThread>();
        }

        /// <summary>
        ///     Handler for messages received by the Content module.
        /// </summary>
        /// <param name="messageData">Received message</param>
        public void OnMessageReceived(ReceiveContentData messageData)
        {
            _rcvMsgData = messageData;
        }

        /// <summary>
        ///     Handler for the event of all messages sent to/from client being received at once
        ///     The Dashboard module may simply throw an excpetion in the body of
        ///     this function because it doesn't expect to receive list of all messages
        ///     as it is running on the server, not on the clients.
        /// </summary>
        /// <param name="allMessages">list of Thread objects containing all messages</param>
        public void OnAllMessages(List<ChatThread> allMessages)
        {
            _chatContextList = allMessages;
        }

        public ReceiveContentData GetOnMessageData()
        {
            return _rcvMsgData;
        }

        public List<ChatThread> GetOnAllMessagesData()
        {
            return _chatContextList;
        }
    }
}
