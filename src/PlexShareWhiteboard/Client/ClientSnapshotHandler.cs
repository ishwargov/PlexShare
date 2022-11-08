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
                    //creating boardServerShape object with FetchCheckpoint object
                    List<ShapeItem> boardShape = null;

                    //sending boardServerShape object to _clientBoardCommunicator
                    WBServerShape wBServerShape = new WBServerShape(boardShape, Operation.RestoreSnapshot, UserId, snapshotNumber);
                    _clientCommunicator.SendToServer(wBServerShape);
                }
                else
                {
                    throw new ArgumentException("invalid checkpointNumber");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("ClientCheckPointHandler.FetchCheckPoint: An exception occured.");
                Trace.WriteLine(e.Message);
            }
        }

        public void SaveSnapshot(string UserId)
        {
            try
            {
                // increasing the checkpoint number by one
                SnapshotNumber++;

                //creating boardServerShape object with CreateCheckpoint object
                List<ShapeItem> boardShape = null;

                //sending boardServerShape object to _clientBoardCommunicator
                WBServerShape wBServerShape = new WBServerShape(boardShape, Operation.CreateSnapshot, UserId, SnapshotNumber);
                _clientCommunicator.SendToServer(wBServerShape);
            }

            catch (Exception e)
            {
                Trace.WriteLine("ClientCheckPointHandler.SaveCheckPoint: An exception occured.");
                Trace.WriteLine(e.Message);
            }
        }
    }
}
