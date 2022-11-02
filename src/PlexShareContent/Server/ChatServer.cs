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

        public MessageData Receive(MessageData msg)
        {
            ReceiveMessageData receivedMsg;
            Trace.WriteLine("[ChatServer] Received message from ContentServer");
            if(msg.Event == MessageEvent.NewMessage)
            {
                Trace.WriteLine("[ChatServer] Event is NewMessage, Adding message to existing Thread");
                return _contentDB.MessageStore(msg);
            }
            else if(msg.Event == MessageEvent.Star)
            {
                Trace.WriteLine("[ChatServer] Event is Star, Starring message in existing Thread");
                receivedMsg = StarMessage(msg.ReplyThreadId, msg.MessageId);
            }
            else if(msg.Event == MessageEvent.Update)
            {
                Trace.WriteLine("[ChatServer] Event is Update, Updating message in existing Thread");
                receivedMsg = UpdateMessage(msg.ReplyThreadId, msg.MessageId,
                    msg.Message);
            }
            else
            {
                Trace.WriteLine($"[ChatServer] invalid event");
                return null;
            }
            if (!receivedMsg) return receivedMsg;

            var notifyMsgData = new MessageData(receivedMsg)
            {
                Event = messageData.Event
            };
            return notifyMsgData;
        }
        public List<ChatContext> GetMessages()
        {
            return _contentDB.GetChatContexts();
        }

        public ReceiveMessageData StarMessage(int replyId, int msgId)
        {
            var msg = _contentDB.GetMessage(replyId, msgId);

            // If ContentDatabase returns null that means the message doesn't exists, return null
            if (!msg)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {msgId}.");
                return null;
            }

            // Star the message and return the starred message
            msg.Starred = true;
            return msg;
        }

        private ReceiveMessageData UpdateMessage(int replyId, int msgId, string updatedMsg)
        {
            var message = _contentDB.GetMessage(replyId, msgId);

            // If ContentDatabase returns null that means the message doesn't exists, return null
            if (!message)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {msgId}.");
                return null;
            }

            // Update the message and return the updated message
            message.Message = updatedMsg;
            return message;
        }

    }
}
