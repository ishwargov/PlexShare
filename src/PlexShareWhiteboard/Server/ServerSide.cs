using PlexShareWhiteboard.BoardComponents;
using System.Collections.Generic;
using System.Linq;

namespace PlexShareWhiteboard.Server
{
    internal class ServerSide
    {
        // An objectId to object map which contains all the ShapeItems in the WhiteBoard presently
        public Dictionary<string, ShapeItem> objIdToObjectMap = new Dictionary<string, ShapeItem>();

        // A copy to restore old state on undo-clear
        public Dictionary<string, ShapeItem> oldObjectMapCopy;

        // To add an object to the Server Object List
        private void AddObjectToServerList(string objectId, ShapeItem newShape)
        {
            objIdToObjectMap.Add(objectId, newShape);
        }

        // To remove an object from the Server Object List
        private void RemoveObjectFromServerList(string objectId, ShapeItem newShape)
        {
            objIdToObjectMap.Remove(objectId);
        }

        // To update an existing object in the Server Object List
        private void UpdateObjectInServerList(string objectId, ShapeItem newShape)
        {
            if (objIdToObjectMap.ContainsKey(objectId))
            {
                objIdToObjectMap[objectId] = newShape;
            }
        }

        // To clear (empty) the Server Object List
        // Save the current list in a copy (oldObjectMapCopy)
        private void ClearObjectsInServerList()
        {
            oldObjectMapCopy = objIdToObjectMap;
            objIdToObjectMap = new Dictionary<string, ShapeItem>();
        }

        // If a user performs undo after clear, then display all objects in the oldObjectMapCopy
        // Set the objIdToObjectMap as the old copy i.e. oldObjectMapCopy
        private List<ShapeItem> DisplayObjectsInServerList()
        {
            objIdToObjectMap = oldObjectMapCopy;
            return oldObjectMapCopy.Values.ToList();
        }

        // Function to Broadcast to Clients
        private void BroadcastToClients(ShapeItem newShape, Operation op)
        {
            if (op == Operation.UndoClear)
            {
                return;
            }
            // sam function
        }

        // On receiving a shape from the ClientSide / Undo Redo
        public void OnShapeReceived(ShapeItem newShape, Operation op)
        {
            if (op == Operation.Creation)
                AddObjectToServerList(newShape.Id, newShape);
            else if (op == Operation.Deletion)
                RemoveObjectFromServerList(newShape.Id, newShape);
            else if (op == Operation.Clear)
                ClearObjectsInServerList();
            else if (op == Operation.UndoClear)
                DisplayObjectsInServerList();
            else
                UpdateObjectInServerList(newShape.Id, newShape);

            BroadcastToClients(newShape, op);
        }
    }
}
