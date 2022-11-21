/***************************
 * Filename    = ClientCommunicator.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Used to communicate between Client side White Board Modules and 
 *               the Networking module.
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;
using System;
using PlexShareNetwork;
using PlexShareNetwork.Communication;
using System.Diagnostics;
using Serializer = PlexShareWhiteboard.BoardComponents.Serializer;

namespace PlexShareWhiteboard.Client
{
    public class ClientCommunicator : IClientCommunicator
    {
        private static ClientCommunicator instance;
        private static Serializer serializer;
        private static ICommunicator communicator;
        private static readonly string moduleIdentifier = "Whiteboard";

        /// <summary>
        ///     Getter for singleton class instance.
        /// </summary>
        public static ClientCommunicator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientCommunicator();
                    serializer = new Serializer();
                    communicator = CommunicationFactory.GetCommunicator();
                    communicator.Subscribe(moduleIdentifier, WhiteBoardViewModel.Instance);
                }

                return instance;
            }
        }

        /// <summary>
        ///     Serializes the WBServeShape object and passes it to communicator.Send().
        /// </summary>
        /// <param name="clientUpdate">The object to be passed to server.</param>
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
