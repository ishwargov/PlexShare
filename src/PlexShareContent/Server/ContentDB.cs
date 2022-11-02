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
        private readonly List<ChatContext> _chatContext;
        private readonly Dictionary<int, int> _chatsMap;
        private readonly Dictionary<int, MessageData> _files;

        public ContentDB()
        {
            _files = new Dictionary<int, MessageData>();
            _chatContext = new List<ChatContext>();
            _chatsMap = new Dictionary<int, int>();
            IdGenerator.ResetChatId();
            IdGenerator.ResetMsgId();
        }

        public MessageData FileStore(MessageData msg)
        {
            var message = StoreMessage(msg);
            _files[message.MessageId] = msg;
            return message;
        }

        public MessageData FilesFetch(int msgId)
        {
            // If requested messageId is not in the map return null
            if (!_files.ContainsKey(msgId))
                return null;
            return _files[msgId];
        }

        public MessageData MessageStore(MessageData msg)
        {
            msg.MessageId = IdGenerator.GetMsgId();
            // If message is a part of already existing chatContext
            if (_chatsMap.ContainsKey(msg.ReplyThreadId))
            {
                var threadIndex = _chatsMap[msg.ReplyThreadId];
                var chatContext = _chatContext[threadIndex];
                ReceiveMessageData message = msg.Clone();
                chatContext.AddMessage(message);
            }
            // else create a new chatContext and add the message to it
            else
            {
                var chatContext = new ChatContext();
                var newThreadId = IdGenerator.GetChatContextId();
                msg.ReplyThreadId = newThreadId;
                ReceiveMessageData message = msg.Clone();
                chatContext.AddMessage(message);

                _chatContext.Add(chatContext);
                _chatsMap[chatContext.ThreadId] = _chatContext.Count - 1;
            }

            return msg;
        }

        public List<ChatContext> GetChatContexts()
        {
            return _chatContext;
        }

        public ReceiveMessageData GetMessage(int threadId, int msgId)
        {
            // If given ChatContext or Message doesn't exists return null
            if (!_chatsMap.ContainsKey(threadId)) 
                return null;

            var threadIndex = _chatsMap[threadId];
            var chat = _chatContext[threadIndex];

            // If given ChatContext doesn't contain the message return null
            if (!chat.ContainsMessageId(msgId)) 
                return null;

            var messageIndex = chat.RetrieveMessageIndex(msgId);
            return chat.MsgList[messageIndex];
        }
    }
}
