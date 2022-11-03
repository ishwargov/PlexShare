using PlexShareContent.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareContent.Server
{
    public interface IContentServer
    {
        public void ServerSubscribe(Client.IContentListener subscriber);

        public List<ChatThread> ServerGetMessages();

        public void SSendAllMessagesToClient(int userID);
    }
}
