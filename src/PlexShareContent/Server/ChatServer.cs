using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlexShareContent.Server
{
    public class ChatServer
    {
        private readonly ContentDB _contentDB;

        public ChatServer(ContentDB db)
        {
            _contentDB = db;
        }

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
            else
            {
                Trace.WriteLine($"[ChatServer] invalid event");
                return null;
            }
            if (receivedMsg == null)
            {
                return null;
            }

            var notifyMsgData = new ContentData(receivedMsg)
            {
                Event = msg.Event
            };
            return notifyMsgData;
        }
        public List<ChatThread> GetMessages()
        {
            return _contentDB.GetChatContexts();
        }

        public ReceiveContentData StarMessage(int replyId, int msgId)
        {
            var msg = _contentDB.GetMessage(replyId, msgId);

            // If ContentDatabase returns null that means the message doesn't exists, return null
            if (msg == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {msgId}.");
                return null;
            }

            // Star the message and return the starred message
            msg.Starred = true;
            return msg;
        }

        private ReceiveContentData UpdateMessage(int replyId, int msgId, string updatedMsg)
        {
            var message = _contentDB.GetMessage(replyId, msgId);

            // If ContentDatabase returns null that means the message doesn't exists, return null
            if (message == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {msgId}.");
                return null;
            }

            // Update the message and return the updated message
            message.Data = updatedMsg;
            return message;
        }

    }
}
