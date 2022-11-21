/***************************
 * Filename    = IClientSnapshotHandler.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Interface to specify the functions handled by ClientSnapshotHandler
 ***************************/

namespace PlexShareWhiteboard.Client.Interfaces
{
    public interface IClientSnapshotHandler
    {
        /// <summary>
        ///     Gets and sets snapshot number.
        /// </summary>
        int SnapshotNumber { get; set; }

        /// <summary>
        ///     Creates and saves the snapshot.
        /// </summary>
        /// <param name="UserId">User ID saving snapshot</param>
        /// <returns></returns>
        int SaveSnapshot(string UserId);

        /// <summary>
        ///     Fetches and restores the snapshot from server.
        /// </summary>
        /// <param name="snapshotNumber">Snapshot number to be restored</param>
        /// <param name="UserId">User ID requesting restore</param>
        void RestoreSnapshot(int snapshotNumber, string UserId);
    }
}
