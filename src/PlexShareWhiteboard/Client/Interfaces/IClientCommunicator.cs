using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Client.Interfaces
{
    internal interface IClientCommunicator
    {
        void SendToServer(WBServerShape wBServerShape);
        void SendMessageToServer(string message, string ipAddress);
        void Subscribe(object listener);
    }
}
