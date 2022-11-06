using PlexShareContent;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.ContentTests
{
    public class Utils
    {
        private readonly FakeCommunicator _fakeCommunicator;

        public Utils()
        {
            _fakeCommunicator = new FakeCommunicator();
        }

        public FakeCommunicator GetFakeCommunicator()
        {
            return _fakeCommunicator;
        }

        public SendContentData GenerateChatSendMsgData(string msg = "Hello", int[] rcvIds = null, int replyId = -1,
            MessageType type = MessageType.Chat)
        {
            if (rcvIds == null) rcvIds = new int[0];
            var toConvert = new SendContentData();
            toConvert.Data = msg;
            toConvert.Type = type;
            toConvert.ReplyThreadID = replyId;
            toConvert.ReceiverIDs = rcvIds;
            return toConvert;
        }

        public ContentData GenerateChatContentData(MessageEvent chatEvent = MessageEvent.New,
            string msg = "Hello", int[] rcvIds = null, int replyId = -1, MessageType type = MessageType.Chat)
        {
            if (rcvIds == null) rcvIds = new int[0];
            var sampleData = GenerateChatSendMsgData(msg, rcvIds, replyId, type);
            var contentChatClient = new ChatClient(_fakeCommunicator);
            var msgData = contentChatClient.SendToMessage(sampleData, chatEvent);
            return msgData;
        }

        public ContentData GenerateNewData(string Message, int MessageID = 1, int[] rcvIds = null,
            int ReplyThreadID = -1, int SenderID = -1, bool Starred = false, MessageType Type = MessageType.Chat)
        {
            if (rcvIds == null) rcvIds = new int[0];
            var msg = new ContentData();
            msg.Event = MessageEvent.New;
            msg.Data = Message;
            msg.MessageID = MessageID;
            msg.ReceiverIDs = rcvIds;
            msg.SenderID = SenderID;
            msg.ReplyThreadID = ReplyThreadID;
            msg.Starred = Starred;
            msg.Type = Type;
            return msg;
        }

        public ReceiveContentData ContentDataToReceiveContentData(ContentData msgData)
        {
            var msg = new ReceiveContentData();
            msg.Event = msgData.Event;
            msg.Data = msgData.Data;
            msg.MessageID = msgData.MessageID;
            msg.ReceiverIDs = msgData.ReceiverIDs;
            msg.SenderID = msgData.SenderID;
            msg.ReplyThreadID = msgData.ReplyThreadID;
            msg.Starred = msgData.Starred;
            msg.Type = msgData.Type;
            return msg;
        }

        public ReceiveContentData GenerateNewReceiveContentData(string Message, int MessageID = 1, int[] rcvIds = null,
            int ReplyThreadID = -1, int SenderID = -1, bool Starred = false, MessageType Type = MessageType.Chat)
        {
            if (rcvIds == null) rcvIds = new int[0];
            var msg = new ReceiveContentData();
            msg.Event = MessageEvent.New;
            msg.Data = Message;
            msg.MessageID = MessageID;
            msg.ReceiverIDs = rcvIds;
            msg.SenderID = SenderID;
            msg.ReplyThreadID = ReplyThreadID;
            msg.Starred = Starred;
            msg.Type = Type;
            return msg;
        }

        public List<ChatThread> getlistContext(ContentData message)
        {
            ReceiveContentData receivedMessage = message;

            // add the message to the correct ChatThread in allMessages
            var sampleData = new List<ChatThread>();
            var newContext = new ChatThread();
            newContext.AddMessage(receivedMessage);

            sampleData.Add(newContext);
            return sampleData;
        }

        public SendContentData GetSendContentData1()
        {
            var toconvert1 = new SendContentData();
            toconvert1.Data = "Hello";
            toconvert1.Type = MessageType.Chat;
            toconvert1.ReplyThreadID = -1;
            toconvert1.ReceiverIDs = new int[0];
            return toconvert1;
        }

        public ContentData GetContentData1()
        {
            var sampleData = GetSendContentData1();
            var conch = new ChatClient(_fakeCommunicator);
            var msgData = conch.SendToMessage(sampleData, MessageEvent.New);
            return msgData;
        }

        public SendContentData GetSendContentData2()
        {
            var toconvert2 = new SendContentData();
            toconvert2.Data = null;
            toconvert2.Type = MessageType.Chat;
            toconvert2.ReplyThreadID = -1;
            toconvert2.ReceiverIDs = new int[0];
            return toconvert2;
        }

        ///<summary>
        /// We need output string from server to trigger INotificationHandler function so that we can deserialized it and update ChatThread map
        /// </summary>
    }
}
