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
    internal class ServerSide : IClientServer
    {
        // An objectId to object map which contains all the ShapeItems in the WhiteBoard presently
        private Dictionary<string, ShapeItem> objIdToObjectMap = new Dictionary<string, ShapeItem>();

        // A copy to restore old state 
        private Dictionary<string, ShapeItem> oldObjectMapCopy;

        // To keep track of the ZIndex of the objects
        private static int _maxZIndex = 0;

        // An instance of the ServerCommunicator
        IServerCommunicator _communicator;

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
                /*case Operation.UndoClear:
                        DisplayObjectsInServerList(newShape, op);
                        break; */
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
            _communicator.Broadcast(newShapes,op);
        }

        // To add an object to the Server Object List
        private void AddObjectToServerList(string objectId, ShapeItem newShape, Operation op)
        {

            newShape.ZIndex = Math.Max(_maxZIndex, newShape.ZIndex);
            objIdToObjectMap.Add(objectId, newShape);
            BroadcastToClients(newShape, op);
            _maxZIndex++;
        }

        // To remove an object from the Server Object List
        private void RemoveObjectFromServerList(string objectId, ShapeItem newShape, Operation op)
        {
            if (objIdToObjectMap.ContainsKey(objectId))
            {
                objIdToObjectMap.Remove(objectId);
                BroadcastToClients(newShape, op);
            }
        }

        // To update an existing object in the Server Object List
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

        // If a user performs undo after clear, then display all objects in the oldObjectMapCopy
        // Set the objIdToObjectMap as the old copy i.e. oldObjectMapCopy
        private void DisplayObjectsInServerList(ShapeItem newShape, Operation op)
        {
            objIdToObjectMap = oldObjectMapCopy;
            BroadcastToClients(oldObjectMapCopy.Values.ToList(), op);
        }

    }
}
