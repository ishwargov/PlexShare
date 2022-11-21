/***************************
 * Filename    = ClientSide.cs
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
using PlexShareWhiteboard.Server.Interfaces;


namespace PlexShareWhiteboard.Client
{
    public class ClientSide : IShapeListener
    {
        // Instance of ClientCommunicator for sending to Server
        IClientCommunicator _communicator;
        Serializer _serializer;
        ClientSnapshotHandler _snapshotHandler;
        private static ClientSide instance;

        /// <summary>
        ///         To make ClientSide a singleton class. There should be a single 
        ///         instance of the client on a particular machine.
        /// </summary>
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

        // ClientSide Constructor 
        private ClientSide()
        {
            _communicator = ClientCommunicator.Instance;
            _serializer = new Serializer();
            _snapshotHandler = new ClientSnapshotHandler();
            NewUserHandler();
        }

        string userID;

        // Sets the user ID
        public void SetUserId(string userId)
        {
            userID = userId;
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
            WBServerShape wbShape = new WBServerShape(newSerializedShapes, op);
            _communicator.SendToServer(wbShape);

        }

        /// <summary>
        ///         When a new user joins the session. The information about this 
        ///         including the userId is communicated to the server through the clientcommunicator
        ///         as a WBServerShape. 
        /// </summary>
        public void NewUserHandler()
        {
            WBServerShape wbShape = new WBServerShape(null, Operation.NewUser, userID);
            _communicator.SendToServer(wbShape);
        }

        /// <summary>
        ///         When a user wants to save a snapshot. The clientSnapshotHandler uses 
        ///         the SaveSnapshot to achieve this.
        /// </summary>
        /// <param name="userId">UserId of user who clicked on Save</param>
        /// <returns>SnapShot Number of the snapshot created</returns>
        public int OnSaveMessage(string userId)
        {
            return _snapshotHandler.SaveSnapshot(userId);
        }

        /// <summary>
        ///         When a user wants to restore a snapshot. The clientSnapshotHandler uses 
        ///         the RestoreSnapshot to achieve this.
        /// </summary>
        /// <param name="snapshotNumber">SnapShot Number of the snapshot requested</param>
        /// <param name="userId">UserId of user who clicked on Restore</param>
        public List<ShapeItem> OnLoadMessage(int snapshotNumber, string userId)
        {
            _snapshotHandler.RestoreSnapshot(snapshotNumber, userId);
            return null;
        }

        // Returns the ZIndex
        public int GetMaxZindex(ShapeItem lastShape)
        {
            return lastShape.ZIndex;
        }

        // Utility function to set the snapshot number
        public void SetSnapshotNumber(int snapshotNumber)
        {
            _snapshotHandler.SnapshotNumber = snapshotNumber;
        }

        // Utility function to set a ClientCommunicator (for testing purposes)
        public void SetCommunicator(IClientCommunicator communicator)
        {
            _communicator = communicator;
        }

        // Utility function to get the snapshotHandler (for testing purposes)
        public ClientSnapshotHandler GetSnapshotHandler()
        {
            return _snapshotHandler;
        }

    }
}