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
        private readonly ContentServer _contentServer;

        internal ContentServerNotificationHandler(ContentServer contentServer)
        {
            _contentServer = contentServer;
        }

        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            _contentServer.Receive(data);
        }
    }
}
