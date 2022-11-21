/***************************
 * Filename    = IServerSnapshotHandler.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Interface to specify the functions handled by ServerSnapshotHandler.
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using System.Collections.Generic;

namespace PlexShareWhiteboard.Server.Interfaces
{
    public interface IServerSnapshotHandler
    {
        /// <summary>
        ///     Fetches and loades the snapshot corresponding to provided snapshotNumber.
        /// </summary>
        /// <param name="snapshotNumber">The number of the snapshots which needs to fetched.</param>
        /// <returns></returns>
        public List<ShapeItem> LoadBoard(int snapshotNumber);

        /// <summary>
        ///     Saves the snapshot at the server.
        /// </summary>
        /// <param name="boardShapes">List containing all the shapes to save the snapshot.</param>
        /// <param name="userID">User who requested the saving of snapshot.</param>
        /// <returns></returns>
        public int SaveBoard(List<ShapeItem> boardShapes, string userID);
    }
}
