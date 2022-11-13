/***************************
 * Filename    = WhiteBoardViewModel.cs
 *
 * Author      = Aiswarya H
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is the Server Side Implementation.
 *               This implements the server side handling when it receives 
 *               a message or shape from the network and broadcasts it to clients.
 *               It also contains the Server Side WhiteBoardState Management.
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;
using PlexShareWhiteboard.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Server
{
    /// <summary>
    ///         Class to perform Server Side WhiteBoardState Management and Broadcast (Server to Clients).
    /// </summary>
    public class ServerSide : IShapeListener
    {

        // An instance of the ServerCommunicator
        private static ServerCommunicator _communicator;

        private static ServerSide instance;

        // To create only a single instance of ServerSide
        public static ServerSide Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerSide();
                    _communicator = ServerCommunicator.Instance;
                }

                return instance;
            }
        }

        string userID;

        public void SetUserId(string userId)
        {
            userID = userId;
        }
        

        public int GetServerListSize()
        {
            return objIdToObjectMap.Count();
        }
        // An objectId to object map which contains all the ShapeItems in the WhiteBoard presently
        private Dictionary<string, ShapeItem> objIdToObjectMap = new Dictionary<string, ShapeItem>();

        // A copy to restore old state 
        private Dictionary<string, ShapeItem> oldObjectMapCopy;

        // To keep track of the ZIndex of the objects
        private static int _maxZIndex = 0;



        Serializer serializer = new Serializer();

        ServerSnapshotHandler serverSnapshotHandler = new ServerSnapshotHandler();

        /// <summary>
        ///         When a ShapeItem is received from the Client/ViewModel, it updates the server side 
        ///         List of ShapeItems (objIdToObjectMap) depending on the operation performed on the ShapeItem.
        /// </summary>
        /// <param name="newShape">ShapeItem received from the client</param>
        /// <param name="op">Operation that needs to be performed</param>
        public void OnShapeReceived(ShapeItem newShape, Operation op)
        {
            switch (op)
            {

                case Operation.Creation:
                    AddObjectToServerList(newShape.Id, newShape, op);
                    break;
                case Operation.Deletion:
                    RemoveObjectFromServerList(newShape.Id, newShape, op);
                    break;
                case Operation.ModifyShape:
                    UpdateObjectInServerList(newShape.Id, newShape, op);
                    break;
                case Operation.Clear:
                    ClearObjectsInServerList(newShape, op);
                    break;
            }
        }

        /// <summary>
        ///         Function to Broadcast to Clients
        /// </summary>
        /// <param name="newShape"></param>
        /// <param name="op"></param>
        public void BroadcastToClients(ShapeItem newShape, Operation op)
        {
            _communicator.Broadcast(newShape, op);
        }
        public void BroadcastToClients(List<ShapeItem> newShapes, Operation op)
        {
            _communicator.Broadcast(newShapes, op);
        }

        /// <summary>
        ///         Add the ShapeItem in the objIdToObjectMap with key as objectId
        ///         when a ShapeItem is created.
        ///         The ShapeItem is then broadcasted with operation as creation.
        /// </summary>
        /// <param name="objectId">Id of the object to be added</param>
        /// <param name="newShape">ShapeItem which was created</param>
        /// <param name="op">Operation performed (Creation)</param>
        private void AddObjectToServerList(string objectId, ShapeItem newShape, Operation op)
        {

            newShape.ZIndex = Math.Max(_maxZIndex, newShape.ZIndex);
            objIdToObjectMap.Add(objectId, newShape);
            BroadcastToClients(newShape, op);
            _maxZIndex++;
        }

        /// <summary>
        ///         Remove the ShapeItem corresponding to the objectId from objIdToObjectMap 
        ///         when a ShapeItem is deleted.
        ///         The ShapeItem is then broadcasted with operation as deletion.
        /// </summary>
        /// <param name="objectId">Id of the object to be removed</param>
        /// <param name="newShape">ShapeItem which was deleted</param>
        /// <param name="op">Operation performed (Deletion)</param>
        private void RemoveObjectFromServerList(string objectId, ShapeItem newShape, Operation op)
        {
            if (objIdToObjectMap.ContainsKey(objectId))
            {
                objIdToObjectMap.Remove(objectId);
                BroadcastToClients(newShape, op);
            }
        }

        /// <summary>
        ///         Update the ShapeItem corresponding to the objectId in objIdToObjectMap
        ///         whenever a ShapeItem is modified.
        ///         The ShapeItem is then broadcasted with corresponding operation.
        /// </summary>
        /// <param name="objectId">Id of the object to be updated</param>
        /// <param name="newShape">ShapeItem that was modified</param>
        /// <param name="op">Operation which led to the modification</param>
        private void UpdateObjectInServerList(string objectId, ShapeItem newShape, Operation op)
        {
            if (objIdToObjectMap.ContainsKey(objectId))
            {
                objIdToObjectMap[objectId] = newShape;
                BroadcastToClients(newShape, op);
            }
        }

        // To clear (empty) the Server Object List
        // Save the current list in a copy (oldObjectMapCopy)
        private void ClearObjectsInServerList(ShapeItem newShape, Operation op)
        {
            oldObjectMapCopy = objIdToObjectMap;
            objIdToObjectMap = new Dictionary<string, ShapeItem>();
            BroadcastToClients(newShape, op);
        }



        public void RestoreSnapshotHandler(WBServerShape deserializedObject)
        {
            List<ShapeItem> loadedShapes = serverSnapshotHandler.LoadBoard(deserializedObject.SnapshotNumber);
            List<SerializableShapeItem> serializableShapeItems = serializer.ConvertToSerializableShapeItem(loadedShapes);
            WBServerShape wBServerShape = new WBServerShape(
                serializableShapeItems,
                Operation.RestoreSnapshot,
                deserializedObject.UserID
            );
            BroadcastToClients(loadedShapes, Operation.RestoreSnapshot);
        }

        public void CreateSnapshotHandler(WBServerShape deserializedObject)
        {
            serverSnapshotHandler.SaveBoard(objIdToObjectMap.Values.ToList());
            _communicator.Broadcast(deserializedObject);
        }

        public void NewUserHandler(WBServerShape deserializedObject)
        {
            List<ShapeItem> shapeItems = objIdToObjectMap.Values.ToList();
            List<SerializableShapeItem> serializableShapeItems = serializer.ConvertToSerializableShapeItem(shapeItems);
            WBServerShape wBServerShape = new WBServerShape(
                serializableShapeItems,
                Operation.NewUser,
                deserializedObject.UserID
            );
            
            _communicator.Broadcast(wBServerShape, deserializedObject.UserID);
        }
    }
}
