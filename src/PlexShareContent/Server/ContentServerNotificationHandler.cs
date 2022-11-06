using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Networking;

namespace PlexShareContent.Server
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        public readonly ContentServer ContentServer;

        public ContentServerNotificationHandler(ContentServer contentServer)
        {
            ContentServer = contentServer;
        }

        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            ContentServer.Receive(data);
        }
    }
}
