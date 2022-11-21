/***************************
 * Filename    = IServerCommunicator.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Interface to be used by ServerSide and ServerSnapshotHandler
 *               to send objects across the network.
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using System.Collections.Generic;

namespace PlexShareWhiteboard.Server.Interfaces
{
    public interface IServerCommunicator
    {
        /// <summary>
        ///     Send the ShapeItem across the network.
        /// </summary>
        /// <param name="newShape">List of updates</param>
        /// <param name="op">Operation to perform</param>
        public void Broadcast(ShapeItem newShape, Operation op);

        /// <summary>
        ///     Send the List of ShapeItems across the network.
        /// </summary>
        /// <param name="newShapes">List of updates</param>
        /// <param name="op">Operation to perform</param>
        public void Broadcast(List<ShapeItem> newShapes, Operation op);

        /// <summary>
        ///     Send the WBServerShape across the network.
        /// </summary>
        /// <param name="clientUpdate"></param>
        /// <param name="userID">Client id to whom to send these objects to</param>
        public void Broadcast(WBServerShape clientUpdate, string? userID);
    }
}
