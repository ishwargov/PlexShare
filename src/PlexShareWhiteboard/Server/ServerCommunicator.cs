/***************************
 * Filename    = ServerCommunicator.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Communication between Server side White Board Modules and Networking module.
 ***************************/

using PlexShareNetwork;
using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using PlexShareWhiteboard.Server.Interfaces;
using PlexShareNetwork.Communication;
using System.Diagnostics;
using Serializer = PlexShareWhiteboard.BoardComponents.Serializer;

namespace PlexShareWhiteboard.Server
{
    public class ServerCommunicator: IServerCommunicator
    {
        private static ServerCommunicator instance;
        private static Serializer serializer;
        private static ICommunicator communicator;
        private static readonly string moduleIdentifier = "Whiteboard";

        /// <summary>
        ///     Getter of ServerCommunicator singleton instance.
        /// </summary>
        public static ServerCommunicator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerCommunicator();
                    serializer = new Serializer();
                    communicator = CommunicationFactory.GetCommunicator(false);
                    communicator.Subscribe(moduleIdentifier, WhiteBoardViewModel.Instance);
                }

                return instance;
            }
        }

        /// <summary>
        ///     Send the ShapeItem across the network.
        /// </summary>
        /// <param name="newShape">List of updates</param>
        /// <param name="op">Operation to perform</param>
        public void Broadcast(ShapeItem newShape, Operation op)
        {
            List<ShapeItem> newShapeList = new List<ShapeItem>();
            newShapeList.Add(newShape);
            Broadcast(newShapeList, op);
        }

        /// <summary>
        ///     Send the List of ShapeItems across the network.
        /// </summary>
        /// <param name="newShapes">List of updates</param>
        /// <param name="op">Operation to perform</param>
        public void Broadcast(List<ShapeItem> newShapes, Operation op)
        {
            List<SerializableShapeItem> newSerializableShapes = serializer.ConvertToSerializableShapeItem(newShapes);
            WBServerShape clientUpdate = new WBServerShape(newSerializableShapes, op);
            Broadcast(clientUpdate);
        }

        /// <summary>
        ///     Send the WBServerShape across the network.
        /// </summary>
        /// <param name="clientUpdate"></param>
        /// <param name="userID">Client id to whom to send these objects to</param>
        public void Broadcast(WBServerShape clientUpdate, string? userID=null)
        {
            try
            {
                Trace.WriteLine("[Whiteboard] ServerCommunicator.Broadcast: Sending objects to client");
                var serializedObj = serializer.SerializeWBServerShape(clientUpdate);
                communicator.Send(serializedObj, moduleIdentifier, userID);
                Trace.WriteLine("[Whiteboard] ServerCommunicator.Broadcast: Sent objects to client");
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ServerCommunicator.Broadcast: Exception Occured");
                Trace.WriteLine(e.Message);
            }
        }
    }
}
