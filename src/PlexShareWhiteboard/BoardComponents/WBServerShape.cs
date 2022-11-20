/***************************
 * Filename    = WBServerShape.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = A class capturing information regarding the Whiteboard
 *               to be sent across the network to be processed by Server or Client.
 ***************************/

using System.Collections.Generic;

namespace PlexShareWhiteboard.BoardComponents
{
    public class WBServerShape
    {
        /// <summary>
        ///     Constructor for WBServerShape.
        /// </summary>
        /// <param name="shapeItems">List of ShapeItems</param>
        /// <param name="op">Operation performed on state.</param>
        /// <param name="userID">User id.</param>
        /// <param name="snapshotNumber">Snapshot Number.</param>
        public WBServerShape(
            List<SerializableShapeItem> shapeItems,
            Operation op,
            string userID = "1",
            int snapshotNumber = -1
        )
        {
            ShapeItems = shapeItems;
            Op = op;
            SnapshotNumber = snapshotNumber;
            UserID = userID;
        }

        /// <summary>
        ///     List of ShapeItems(serialized) signifying updates sent to and received from the server.
        /// </summary>
        public List<SerializableShapeItem> ShapeItems { get; set; }

        /// <summary>
        ///     The operation performed on state.
        /// </summary>
        public Operation Op { get; set; }

        /// <summary>
        ///     Snapshot Number.
        /// </summary>
        public int SnapshotNumber { get; set; }

        /// <summary>
        ///     The user id which requested the update.
        /// </summary>
        public string UserID { get; set; }
    }
}
