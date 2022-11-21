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
 *               a message or shape from the ViewModel (of the server) and broadcasts it to clients.
 *               It also contains the Server Side WhiteBoardState Management.
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;
using PlexShareWhiteboard.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static IServerCommunicator _communicator;
        private static ServerSide instance;
        private Serializer _serializer;
        private ServerSnapshotHandler _serverSnapshotHandler;
        string userID;

        /// <summary>
        ///         To make ServerSide a singleton class. 
        /// </summary>
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

        // ServerSide Constructor
        private ServerSide()
        {
            _serializer = new Serializer();
            _serverSnapshotHandler = new ServerSnapshotHandler();
        }


        /// <summary>
        ///         This is the Server list (Server Side State). An objectId to object map which contains all 
        ///         the ShapeItems in the WhiteBoard presently. 
        /// </summary>
        private Dictionary<string, ShapeItem> objIdToObjectMap = new Dictionary<string, ShapeItem>();

        // To keep track of the ZIndex of the objects
        private static int _maxZIndex = 0;

        /// <summary>
        ///         Increments the maxZindex value (when a shape is created on Server).
        ///         Returns the current maxZindex value.
        /// </summary>
        public int GetMaxZindex(ShapeItem lastShape)
        {
            _maxZIndex++;
            return _maxZIndex - 1;
        }

        // Sets the user ID
        public void SetUserId(string userId)
        {
            userID = userId;
        }

        // Returns the current size of server list (for testing purposes)
        public int GetServerListSize()
        {
            return objIdToObjectMap.Count();
        }

        // Utility function to set the snapshot number
        public void SetSnapshotNumber(int snapshotNumber)
        {
            _serverSnapshotHandler.SnapshotNumber = snapshotNumber;
        }

        // Utility function to get the snapshotHandler (for testing purposes)
        public ServerSnapshotHandler GetSnapshotHandler()
        {
            return _serverSnapshotHandler;
        }


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

        
        //Function Overloading to Broadcast to clients ( Single shape and List of Shapes)
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
            try
            {
                newShape.ZIndex = Math.Max(_maxZIndex, newShape.ZIndex);
                objIdToObjectMap.Add(objectId, newShape);
                Trace.WriteLine("[Whiteboard]  " + "inside AddObjectToServerList" + newShape.Id);
                Trace.WriteLine("[Whiteboard]  " + "inside AddObjectToServerList" + newShape.Geometry.GetType().Name);
                BroadcastToClients(newShape, op);
                _maxZIndex++;
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: ServerSide: AddObjectToServerList");
                Trace.WriteLine(e.Message);
            }
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

        /// <summary>
        ///         Clear all objects in the server list. 
        ///         This will be performed when a user clicks Clear.
        /// </summary>
        /// <param name="newShape">ShapeItem (which will bw null)</param>
        /// <param name="op">Operation - Clear</param>
        private void ClearObjectsInServerList(ShapeItem newShape, Operation op)
        {
            objIdToObjectMap = new Dictionary<string, ShapeItem>();
            BroadcastToClients(newShape, op);
        }

        /// <summary>
        ///         To clear the server list. This will be used by some utility functions.
        /// </summary>
        public void ClearServerList()
        {
            objIdToObjectMap.Clear();
        }


        /// <summary>
        ///         To handle creation of snapshot. It calls the serverSnapshotHandler to save the
        ///         current server list shapeItems as a snapshot (along with the user id).
        ///         It then broadcasts the WBServerShape received to all the clients 
        ///         (This will contain the Snapshot number).
        /// </summary>
        /// <param name="deserializedObject">Deserialized object received from network</param>
        /// <returns>Current Snapshot Number</returns>
        public int CreateSnapshotHandler(WBServerShape deserializedObject)
        {
            int n =_serverSnapshotHandler.SaveBoard(objIdToObjectMap.Values.ToList(), deserializedObject.UserID);
            _communicator.Broadcast(deserializedObject,null);
            return n;
        }


        /// <summary>
        ///         When a new user joins the session. All the shapeItems in the server list currently
        ///         (in the session) have to be passed to the new user. These shapes are serialized 
        ///         and then converted to a WBServerShape with operation as NewUser. 
        ///         The WBServerShape is then broadcasted (only) to the user corresponding to the userID.
        /// </summary>
        /// <param name="deserializedObject">Deserialized object received from network</param>
        public void NewUserHandler(WBServerShape deserializedObject)
        {
            List<ShapeItem> shapeItems = objIdToObjectMap.Values.ToList();
            List<SerializableShapeItem> serializableShapeItems = _serializer.ConvertToSerializableShapeItem(shapeItems);
            WBServerShape wBServerShape = new WBServerShape(
                serializableShapeItems,
                Operation.NewUser,
                deserializedObject.UserID
            );

            _communicator.Broadcast(wBServerShape, deserializedObject.UserID);
        }


        /// <summary>
        ///         When a request to save a Snapshot ( "SaveMessage" ) is received. A new wBserverShape is 
        ///         created with operation as CreateSnapshot and the snapshotNumber set accordingly 
        ///         after receiving from serverSnapshotHandler. 
        ///         It then passes this wBserverShape to CreateSnapshotHandler.
        /// </summary>
        /// <param name="userId">UserId of user who wants to take a Snapshot</param>
        /// <returns>SnapShot Number of that snapshot</returns>
        public int OnSaveMessage(string userId)
        {
            int snapshotNumber = _serverSnapshotHandler.SnapshotNumber+1;
            WBServerShape wBServerShape = new WBServerShape(null, Operation.CreateSnapshot, userId, snapshotNumber);
            return CreateSnapshotHandler(wBServerShape);
        }


        /// <summary>
        ///         When a request to restore a Snapshot ( "LoadMessage" ) is received. A new wBserverShape is 
        ///         created with operation as RestoreSnapshot and Snapshot Number of the snapshot which the user
        ///         wants to restore. The serverSnapshotHandler uses the loadboard function to obtain the snapshot
        ///         as a list of ShapeItems. These shapes are serialized and then converted to a WBServerShape 
        ///         with all the shapeItems. It is then broadcasted to all the clients. 
        /// </summary>
        /// <param name="snapshotNumber">Snapshot Number of the desired snapshot</param>
        /// <param name="userId">UserId of user who wants to restore the Snapshot</param>
        /// <returns></returns>
        public List<ShapeItem> OnLoadMessage(int snapshotNumber, string userId)
        {
            WBServerShape deserializedObject = new WBServerShape(null, Operation.RestoreSnapshot, userId, snapshotNumber);

            Trace.WriteLine("[Whiteboard] ServerSide.RestoreSnapshotHandler: Restoring Snapshot " + deserializedObject.SnapshotNumber);
            Trace.WriteLine("[Whiteboard] " + GetServerListSize());

            List<ShapeItem> loadedShapes = _serverSnapshotHandler.LoadBoard(deserializedObject.SnapshotNumber);
            List<SerializableShapeItem> serializableShapeItems = _serializer.ConvertToSerializableShapeItem(loadedShapes);
            WBServerShape wBServerShape = new WBServerShape(
                serializableShapeItems,
                Operation.RestoreSnapshot,
                deserializedObject.UserID
            );
            BroadcastToClients(loadedShapes, Operation.RestoreSnapshot);
            return loadedShapes;
        }

        
    }
}
