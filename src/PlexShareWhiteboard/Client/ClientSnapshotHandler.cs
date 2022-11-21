/***************************
 * Filename    = ClientSnapshotHandler.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Snapshot handling at client side.
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;
using System;
using System.Diagnostics;

namespace PlexShareWhiteboard.Client
{
    public class ClientSnapshotHandler : IClientSnapshotHandler
    {
        private IClientCommunicator _clientCommunicator = ClientCommunicator.Instance;

        /// <summary>
        ///      Gets and sets snapshot number.
        /// </summary>
        public int SnapshotNumber { get; set; }

        /// <summary>
        ///     Fetches and restores the snapshot from server.
        /// </summary>
        /// <param name="snapshotNumber">Snapshot number to be restored</param>
        /// <param name="UserId">User ID requesting restore</param>
        /// <exception cref="Exception"></exception>
        public void RestoreSnapshot(int snapshotNumber, string UserId)
        {
            try
            {
                if (snapshotNumber <= SnapshotNumber)
                {
                    //sending boardServerShape object to _clientBoardCommunicator
                    WBServerShape wBServerShape = new WBServerShape(
                        null,
                        Operation.RestoreSnapshot,
                        UserId,
                        snapshotNumber
                    );
                    _clientCommunicator.SendToServer(wBServerShape);
                }
                else
                {
                    throw new ArgumentException("Invalid snapshotNumber");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    "[Whiteboard] ClientSnapshotHandler.RestoreSnapshot: An exception occured."
                );
                Trace.WriteLine(e.Message);
                throw new Exception();
            }
        }

        /// <summary>
        ///     Creates and saves the snapshot.
        /// </summary>
        /// <param name="UserId">User ID saving snapshot</param>
        /// <returns></returns>
        public int SaveSnapshot(string UserId)
        {
            try
            {
                // increasing the checkpoint number by one
                SnapshotNumber++;

                //sending boardServerShape object to _clientBoardCommunicator
                WBServerShape wBServerShape = new WBServerShape(
                    null,
                    Operation.CreateSnapshot,
                    UserId,
                    SnapshotNumber
                );
                _clientCommunicator.SendToServer(wBServerShape);
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    "[Whiteboard] ClientSnapshotHandler.SaveSnapshot: An exception occured."
                );
                Trace.WriteLine(e.Message);
            }
            return SnapshotNumber;
        }

        /// <summary>
        ///     Function used for testing. Sets a mock communicator.
        /// </summary>
        /// <param name="communicator">ClientCommunicator instance</param>
        public void SetCommunicator(IClientCommunicator communicator)
        {
            _clientCommunicator = communicator;
        }
    }
}
