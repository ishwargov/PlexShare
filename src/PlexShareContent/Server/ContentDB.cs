using PlexShareContent.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PlexShareContent.Server
{
    public class ContentDB
    {
        private readonly List<ChatThread> _chatContext;
        private readonly Dictionary<int, int> _chatsMap;
        private readonly Dictionary<int, ContentData> _files;

        public ContentDB()
        {
            _files = new Dictionary<int, ContentData>();
            _chatContext = new List<ChatThread>();
            _chatsMap = new Dictionary<int, int>();
            IdGenerator.ResetChatId();
            IdGenerator.ResetMsgId();
        }

        public ContentData FileStore(ContentData msg)
        {
            var message = MessageStore(msg);
            _files[message.MessageID] = msg;
            return message;
        }

        public ContentData FilesFetch(int msgId)
        {
            // If requested messageId is not in the map return null
            if (!_files.ContainsKey(msgId))
                return null;
            return _files[msgId];
        }

        public ContentData MessageStore(ContentData msg)
        {
            msg.MessageID = IdGenerator.GetMsgId();
            // If message is a part of already existing chatContext
            if (_chatsMap.ContainsKey(msg.ReplyThreadID))
            {
                var threadIndex = _chatsMap[msg.ReplyThreadID];
                var chatContext = _chatContext[threadIndex];
                ReceiveContentData message = msg.Copy();
                chatContext.AddMessage(message);
            }
            // else create a new chatContext and add the message to it
            else
            {
                var chatContext = new ChatThread();
                var newThreadId = IdGenerator.GetChatId();
                msg.ReplyThreadID = newThreadId;
                ReceiveContentData message = msg.Copy();
                chatContext.AddMessage(message);

                _chatContext.Add(chatContext);
                _chatsMap[chatContext.ThreadID] = _chatContext.Count - 1;
            }

            return msg;
        }

        public List<ChatThread> GetChatContexts()
        {
            return _chatContext;
        }

        public ReceiveContentData GetMessage(int threadId, int msgId)
        {
            // If given ChatContext or Message doesn't exists return null
            if (!_chatsMap.ContainsKey(threadId))
            {
                return null;
            } 

            var threadIndex = _chatsMap[threadId];
            var chat = _chatContext[threadIndex];

            // If given ChatContext doesn't contain the message return null
            if (!chat.ContainsMessageID(msgId))
            {
                return null;
            }
                
            var messageIndex = chat.GetMessageIndex(msgId);
            return chat.MessageList[messageIndex];
        }
    }
}
