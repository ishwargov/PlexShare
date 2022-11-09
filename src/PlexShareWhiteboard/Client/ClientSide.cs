using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareWhiteboard.Client.Interfaces;

namespace PlexShareWhiteboard.Client
{
    internal class ClientSide : IClientServer
    {
        // Instance of ClientCommunicator for sending to Server
        ClientCommunicator _communicator;
        public ClientSide()
        {
            _communicator = new ClientCommunicator();
        }

        /// <summary>
        /// Whenever an action is performed, the operation and shape
        /// associated is passed from the View Model to the ClientSide.
        /// This function will send the ShapeItem and Operation to the server.
        /// </summary>
        /// <param name="newShape">ShapeItem to be sent to Server</param>
        /// <param name="op">Operation to be sent to Server</param>
        public void OnShapeReceived(ShapeItem boardShape, Operation op)
        {
            List<ShapeItem> newShapes = new List<ShapeItem>();
            newShapes.Add(boardShape);
            WBServerShape wbShape = new WBServerShape(newShapes, op, boardShape.User);
            _communicator.SendToServer(wbShape);
        }

        public void OnNewUserJoinMessage(string message, string ipAddress)
        {
            _communicator.SendMessageToServer(message, ipAddress);
        }

        public void OnSaveMessage(string message, string userId)
        {
            _communicator.SendMessageToServer(message, userId);
        }

        public void OnLoadMessage(string message, string userId)
        {
            _communicator.SendMessageToServer(message, userId);
        }

    }
}