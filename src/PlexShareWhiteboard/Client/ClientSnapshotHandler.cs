/**
 * Owned By: Joel Sam Mathew
 * Created By: Joel Sam Mathew
 * Date Created: 22/10/2022
 * Date Modified: 08/11/2022
**/

using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Client
{
    public class ClientSnapshotHandler : IClientSnapshotHandler
    {
        private IClientCommunicator _clientCommunicator = ClientCommunicator.Instance;
        public int SnapshotNumber { get; set; }

        public void RestoreSnapshot(int snapshotNumber, string UserId)
        {
            try
            {
                if (snapshotNumber <= SnapshotNumber)
                {
                    //sending boardServerShape object to _clientBoardCommunicator
                    WBServerShape wBServerShape = new WBServerShape(null, Operation.RestoreSnapshot, UserId, snapshotNumber);
                    _clientCommunicator.SendToServer(wBServerShape);
                }
                else
                {
                    throw new ArgumentException("Invalid snapshotNumber");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientSnapshotHandler.RestoreSnapshot: An exception occured.");
                Trace.WriteLine(e.Message);
                throw new Exception();
            }
        }

        public int SaveSnapshot(string UserId)
        {
            try
            {
                // increasing the checkpoint number by one
                SnapshotNumber++;

                //sending boardServerShape object to _clientBoardCommunicator
                WBServerShape wBServerShape = new WBServerShape(null, Operation.CreateSnapshot, UserId, SnapshotNumber);
                _clientCommunicator.SendToServer(wBServerShape);
            }

            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientSnapshotHandler.SaveSnapshot: An exception occured.");
                Trace.WriteLine(e.Message);
            }
            return SnapshotNumber;
        }

        public void SetCommunicator(IClientCommunicator communicator)
        {
            _clientCommunicator = communicator;
        }
    }
}
