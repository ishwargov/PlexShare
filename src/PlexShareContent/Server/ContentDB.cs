/******************************************************************************
 * Filename    = ContentDB.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = This file implements the database for the content module and is used to store and fetch chats and files from database.
 *****************************************************************************/

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
        private readonly List<ChatThread> _chatThread;
        private readonly Dictionary<int, int> _chatIdToDataMap;
        private readonly Dictionary<int, ContentData> _filesMap;

        /// <summary>
        ///     Database Constructor to initilize member variales.
        /// </summary>
        public ContentDB()
        {
            _filesMap = new Dictionary<int, ContentData>();
            _chatThread = new List<ChatThread>();
            _chatIdToDataMap = new Dictionary<int, int>();
            IdGenerator.ResetChatId();
            IdGenerator.ResetMsgId();
        }

        /// <summary>
        ///     Fetch all the chat threads
        /// </summary>
        public List<ChatThread> GetChatContexts()
        {
            return _chatThread;
        }

        /// <summary>
        ///     Function to store Messages on Database.
        /// </summary>
        public ContentData MessageStore(ContentData msg)
        {
            msg.MessageID = IdGenerator.GetMsgId();
            // If message is a part of already existing chatThread, add to the thread
            if (_chatIdToDataMap.ContainsKey(msg.ReplyThreadID))
            {
                var threadIndex = _chatIdToDataMap[msg.ReplyThreadID];
                var chatContext = _chatThread[threadIndex];
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

                _chatThread.Add(chatContext);
                //Decrease the count by 1, becuase we had already incremented count.
                _chatIdToDataMap[chatContext.ThreadID] = _chatThread.Count - 1;
            }

            return msg;
        }

        /// <summary>
        ///     Retreive message from the Database based on the thread ID and message ID 
        /// </summary>
        public ReceiveContentData GetMessage(int threadId, int _msgId)
        {
            // If given ChatThread or Message doesn't exists return null
            if (!_chatIdToDataMap.ContainsKey(threadId))
            {
                return null;
            }

            var threadIndex = _chatIdToDataMap[threadId];
            var chat = _chatThread[threadIndex];

            // If given ChatContext doesn't contain the message return null
            if (!chat.ContainsMessageID(_msgId))
            {
                return null;
            }

            var messageIndex = chat.GetMessageIndex(_msgId);
            return chat.MessageList[messageIndex];
        }

        /// <summary>
        ///     Function to store Files on Database.
        /// </summary>
        public ContentData FileStore(ContentData msg)
        {
            var message = MessageStore(msg);
            _filesMap[message.MessageID] = msg;
            return message;
        }

        /// <summary>
        ///     Function to Fetch the stored file with given id from the database.
        /// </summary>
        public ContentData FilesFetch(int _msgId)
        {
            // If requested messageId is not in the map return null
            if (!_filesMap.ContainsKey(_msgId))
                return null;
            return _filesMap[_msgId];
        }
        
    }
}
