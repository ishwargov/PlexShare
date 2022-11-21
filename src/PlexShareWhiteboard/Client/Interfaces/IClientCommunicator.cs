/***************************
 * Filename    = IClientCommunicator.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Interface to be used by ClientSide and ClientSnapshotHandler
 *               to send objects across the network.
 ***************************/

using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard.Client.Interfaces
{
    public interface IClientCommunicator
    {
        /// <summary>
        ///     Send the WBServerShape across the network
        /// </summary>
        /// <param name="wBServerShape">Update from client to server side</param>
        void SendToServer(WBServerShape wBServerShape);
    }
}
