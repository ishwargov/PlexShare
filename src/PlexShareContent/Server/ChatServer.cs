/******************************************************************************
 * Filename    = ChatServer.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = This file handles the chat messges and various functionlaities associted with chat.
 *****************************************************************************/

using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlexShareContent.Server
{
    public class ChatServer
    {
        private ContentDB _contentDB;

        /// <summary>
        ///     Constructor to initializes the content Database.
        /// </summary>
        public ChatServer(ContentDB db)
        {
            _contentDB = db;
        }

        /// <summary>
        ///     This function returns all the messages stored.
        /// </summary>
        public List<ChatThread> GetMessages()
        {
            return _contentDB.GetChatThreads();
        }

        /// <summary>
        ///     This event is used to preocess the chat based on the type of event occured.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Returns the new message</returns>
        public ContentData Receive(ContentData msg)
        {
            ReceiveContentData receivedMsg;
            Trace.WriteLine("[ChatServer] Received message from ContentServer");
            if(msg.Event == MessageEvent.New)
            {
                Trace.WriteLine("[ChatServer] Event is NewMessage, Adding message to existing Thread");
                return _contentDB.MessageStore(msg);
            }
            else if(msg.Event == MessageEvent.Star)
            {
                Trace.WriteLine("[ChatServer] Event is Star, Starring message in existing Thread");
                receivedMsg = StarMessage(msg.ReplyThreadID, msg.MessageID);
            }
            else if(msg.Event == MessageEvent.Edit)
            {
                Trace.WriteLine("[ChatServer] Event is Update, Updating message in existing Thread");
                receivedMsg = UpdateMessage(msg.ReplyThreadID, msg.MessageID,
                    msg.Data);
            }
            else if (msg.Event == MessageEvent.Delete)
            {
                Trace.WriteLine("[ChatServer] Event is Update, Updating message in existing Thread");
                receivedMsg = DeleteMessage(msg.ReplyThreadID, msg.MessageID);
            }
            else
            {
                Trace.WriteLine($"[ChatServer] invalid event");
                return null;
            }
            if (receivedMsg == null)
            {
                return null;
            }
            //Create a MessageData object and return this notify object.
            var notifyMsgData = new ContentData(receivedMsg)
            {
                Event = msg.Event
            };
            return notifyMsgData;
        }

        /// <summary>
        ///     This function is used to update a message with a new updated message.
        /// </summary>
        public ReceiveContentData UpdateMessage(int replyId, int _msgId, string updatedMsg)
        {
            var message = _contentDB.GetMessage(replyId, _msgId);

            //message doesn't exists in database, return null
            if (message == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {_msgId}.");
                return null;
            }

            // Update the message and return the updated message
            message.Data = updatedMsg;
            return message;
        }

        /// <summary>
        ///     This function is used to star a message.
        /// </summary>
        public ReceiveContentData StarMessage(int replyId, int _msgId)
        {
            var msg = _contentDB.GetMessage(replyId, _msgId);

            //message doesn't exists in database, return null
            if (msg == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {_msgId}.");
                return null;
            }

            // Star the message and return the starred message
            msg.Starred = true;
            return msg;
        }

        /// <summary>
        ///     This function is used to Delete a message.
        /// </summary>
        public ReceiveContentData DeleteMessage(int replyId, int _msgId)
        {
            var message = _contentDB.GetMessage(replyId, _msgId);

            // Message doesn't exists in database, return null
            if (message == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {_msgId}.");
                return null;
            }

            // The data of message now becomes "Message Deleted.".
            message.Data = "Message Deleted.";
            return message;
        }

    }
}
