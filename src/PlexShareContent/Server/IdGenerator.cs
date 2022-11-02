using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareContent.Server
{
    public class IdGenerator
    {
        private static int msgId;
        private static int chatContentId;

        public static int GetMsgId()
        {
            var id = msgId;
            msgId++;
            return id;
        }

        public static void ResetMsgId()
        {
            msgId = 0;
        }

        public static int GetChatId()
        {
            var id = chatContentId;
            chatContentId++;
            return id;
        }

        public static void ResetChatId()
        {
            chatContentId = 0;
        }
    }
}
