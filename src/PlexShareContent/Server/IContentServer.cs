using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareContent.Server
{
    public interface IContentServer
    {
        public void ServerSubscribe(IContentListener subscriber);

        public List<ChatContent> ServerGetMessages();

        public void ServerSendMessages(int userID);
    }
}
