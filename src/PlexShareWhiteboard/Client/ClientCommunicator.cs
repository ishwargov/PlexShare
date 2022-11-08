using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Client
{
    internal class ClientCommunicator : IClientCommunicator
    {
        private static ClientCommunicator instance;
        public static ClientCommunicator Instance => instance ?? (instance = new ClientCommunicator());

        public void SendMessageToServer(string message, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public void SendToServer(WBServerShape wBServerShape)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(object listener)
        {
            throw new NotImplementedException();
        }

    }
}
