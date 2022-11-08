/**
 * Owned By: Joel Sam Mathew
 * Created By: Joel Sam Mathew
 * Date Created: 22/10/2022
 * Date Modified: 08/11/2022
**/

using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareNetwork;
using PlexShareNetwork.Communication;
using PlexShareNetwork.Serialization;
using System.Diagnostics;
using System.Windows.Markup;
using Serializer = PlexShareWhiteboard.BoardComponents.Serializer;

namespace PlexShareWhiteboard.Client
{
    internal class ClientCommunicator : IClientCommunicator
    {
        private static ClientCommunicator instance;
        private static Serializer serializer;
        private static ICommunicator communicator;
        private static readonly string moduleIdentifier = "Whiteboard";
        public static ClientCommunicator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientCommunicator();
                    serializer = new Serializer();
                    communicator = CommunicationFactory.GetCommunicator();
                }

                return instance;
            }
        }

        public void SendMessageToServer(string message, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public void SendToServer(WBServerShape clientUpdate)
        {
            try
            {
                Trace.WriteLine("[Whiteboard] ClientCommunicator.Send: Sending objects to server");
                var serializedObj = serializer.SerializeWBServerShape(clientUpdate);
                communicator.Send(serializedObj, moduleIdentifier, null);
                Trace.WriteLine("[Whiteboard] ClientCommunicator.Send: Sent objects to server");
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientCommunicator.Send: Exception Occured");
                Trace.WriteLine(e.Message);
            }
        }

    }
}
