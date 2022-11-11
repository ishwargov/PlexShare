/******************************************************************************
 * Filename    = ChatContext.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class containing data related to messages of a specific thread.
 *****************************************************************************/

using System;
using System.Collections.Generic;

namespace PlexShareContent.DataModels
{
    public class ChatThread
    {
        /// <summary>
        /// ID of the thread
        /// </summary>
        public int ThreadID;

        /// <summary>
        /// Time of creation of thread
        /// </summary>
        public DateTime CreationTime;

        /// <summary>
        /// List of all messages in the thread
        /// </summary>
        public List<ReceiveContentData> MessageList;

        /// <summary>
        /// Dictionary containing mapping from message ID to index in message list
        /// </summary>
        public Dictionary<int, int> MessageIDToIndex;

        /// <summary>
        /// Constructor to create type without parameters.
        /// </summary>
        public ChatThread()
        {
            ThreadID = -1;
            CreationTime = new DateTime();
            MessageIDToIndex = new Dictionary<int, int>();
            MessageList = new List<ReceiveContentData>();
        }
        
        /// <summary>
        /// Checks if message ID is present in the dictionary
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <returns>True if message ID is present, False otherwise</returns>
        public bool ContainsMessageID(int messageID)
        {
            return MessageIDToIndex.ContainsKey(messageID);
        }

        /// <summary>
        /// Get message index corresonding to the message ID
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <returns>Index of the message in </returns>
        /// <exception cref="ArgumentException"></exception>
        public int GetMessageIndex(int messageID)
        {
            if(!MessageIDToIndex.ContainsKey(messageID))
            {
                throw new ArgumentException("Message ID does not exist in thread.");
            }
            return MessageIDToIndex[messageID];
        }

        /// <summary>
        /// Check if message is valid or not
        /// </summary>
        /// <param name="message">Message string</param>
        /// <returns></returns>
        private bool IsValidMessage(string message)
        {
            if(message == null || message == "")
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Count of the number of messages in list.
        /// </summary>
        public int MessageCount => MessageList.Count;

        /// <summary>
        /// Add message to the present chat context.
        /// </summary>
        /// <param name="message">Instance of the ReceiveContentData class</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddMessage(ReceiveContentData message)
        {
            // check for valid message
            if (!IsValidMessage(message.Data))
            {
                throw new ArgumentException("Invalid message string.");

            }
            // if the message belongs to a new thread without any messages
            if (ThreadID == -1)
            {
                if(message.ReplyThreadID == -1)
                {
                    throw new ArgumentException("Thread ID cannot be -1 as it does not exist.");
                }
                ThreadID = message.ReplyThreadID;
                CreationTime = message.SentTime;
            }
            else
            {
                // check if message belongs to this chat context.
                if(message.ReplyThreadID != ThreadID)
                {
                    throw new ArgumentException("Invalid Thread ID. Message is not a part of this thread.");
                }
                // check if message already exists in thread.
                if(ContainsMessageID(message.MessageID))
                {
                    throw new ArgumentException("Message with given message ID already exists in thread.");
                }
                // check if messages being replied to also belong to the same thread.
                if(message.ReplyMessageID != -1 && !ContainsMessageID(message.ReplyMessageID))
                {
                    throw new ArgumentException("Replied message does not belong to same thread.");
                }
            }
            // add message and other data related to the data structures
            MessageList.Add(message);
            MessageIDToIndex.Add(message.MessageID, MessageCount - 1);
        }

        /// <summary>
        /// Edit the message present in the internal data structures
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="newMessage">Edited message string</param>
        /// <exception cref="ArgumentException"></exception>
        public void EditMessage(int messageID, string newMessage)
        {
            if(!IsValidMessage(newMessage))
            {
                throw new ArgumentException("Invalid message string.");
            }
            int index = GetMessageIndex(messageID);
            // check if the message type is chat for editing
            if (MessageList[index].Type != MessageType.Chat)
            {
                throw new ArgumentException("Message requested for update is not chat.");
            }
            MessageList[index].Data = newMessage;
        }

        /// <summary>
        /// Delete message present in the internal data structures
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <exception cref="ArgumentException"></exception>
        public void DeleteMessage(int messageID)
        {
            int index = GetMessageIndex(messageID);
            // check if the message type is chat for editing
            if (MessageList[index].Type != MessageType.Chat)
            {
                throw new ArgumentException("Message requested for delete is not chat.");
            }
            MessageList[index].Data = "Message Deleted.";
        }

        /// <summary>
        /// Star message present in the internal data structures
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <exception cref="ArgumentException"></exception>
        public void StarMessage(int messageID)
        {
            int index = GetMessageIndex(messageID);
            // check if the message type is chat for editing
            if (MessageList[index].Type != MessageType.Chat)
            {
                throw new ArgumentException("Message requested for update is not chat.");
            }
            MessageList[index].Starred = true;
        }
    }
}
