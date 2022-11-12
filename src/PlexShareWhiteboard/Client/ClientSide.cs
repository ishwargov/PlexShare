/***************************
 * Filename    = WhiteBoardViewModel.cs
 *
 * Author      = Aiswarya H
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is the Client Side Implementation.
 *               This implements the client side handling when it receives 
 *               a message or shape from the View Model and sends it to Server
 *               with the help of methods provided by IClientCommunicator interface.
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareWhiteboard.Client.Interfaces;
using PlexShareWhiteboard.Server;

namespace PlexShareWhiteboard.Client
{
    public class ClientSide : IShapeListener
    {
        // Instance of ClientCommunicator for sending to Server
        ClientCommunicator _communicator;
        Serializer _serializer;
        ClientSnapshotHandler _snapshotHandler;

        
        private static ClientSide instance;

        // To create only a single instance of ClientSide
        public static ClientSide Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientSide(); 
                }

                return instance;
            }
        }

        string userID;

        public void SetUserId(string userId)
        {
            userID = userId;
        }
        private ClientSide()
        {
            _communicator = ClientCommunicator.Instance;
            _serializer = new Serializer();
            NewUserHandler();
        }

        /// <summary>
        ///         Whenever an action is performed, the operation and shape
        ///         associated is passed from the View Model to the ClientSide.
        ///         This function will send the ShapeItem and Operation to the server.
        /// </summary>
        /// <param name="newShape">ShapeItem to be sent to Server</param>
        /// <param name="op">Operation to be sent to Server</param>
        public void OnShapeReceived(ShapeItem boardShape, Operation op)
        {
            
            List<ShapeItem> newShapes = new List<ShapeItem>();
            newShapes.Add(boardShape);

            var newSerializedShapes = _serializer.ConvertToSerializableShapeItem(newShapes);
            WBServerShape wbShape = new WBServerShape(newSerializedShapes, op, boardShape.User);
            _communicator.SendToServer(wbShape);

        }

        public void NewUserHandler()
        {
            WBServerShape wbShape = new WBServerShape(null, Operation.NewUser, userID);
            _communicator.SendToServer(wbShape);
        }

        public void OnSaveMessage(string userId)
        {
            _snapshotHandler.SaveSnapshot(userId);
        }

        public void OnLoadMessage(int snapshotNumber, string userId)
        {
            _snapshotHandler.RestoreSnapshot(snapshotNumber, userId);
        }

    }
}