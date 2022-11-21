/***************************
 * Filename    = WBSnapshotHandler.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Methods to handle saving and loading of the whiteboard.
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        /// <summary>
        ///     Is called when any device clicks on SaveSnapshot. 
        /// </summary>
        public void SaveSnapshot()
        {
            int currSnapshotNumber = machine.OnSaveMessage(userId);
            CheckList.Add(currSnapshotNumber);

            UpdateCheckList(currSnapshotNumber);
        }

        /// <summary>
        ///     Updates the dropdown for LoadSnapshot.
        /// </summary>
        /// <param name="n">Number of the latest snapshot</param>
        public void UpdateCheckList(int latestSnapshotNumber)
        {
            Trace.WriteLine("[Whiteboard] WBSnapshotHandler: Updating checklist with " + latestSnapshotNumber);
            CheckList.Clear();
            for(int i = latestSnapshotNumber; i> latestSnapshotNumber - 5 && i>0; i--)
            {
                CheckList.Add(i);
            }
            machine.SetSnapshotNumber(latestSnapshotNumber);
        }

        /// <summary>
        ///     Loads the snapshot loaded ShapeItems to the whiteboard.
        /// </summary>
        /// <param name="snapshotNumber">Number of the snapshot to be loaded</param>
        public void LoadSnapshot(int snapshotNumber)
        {
            List<ShapeItem> shapeList = machine.OnLoadMessage(snapshotNumber, userId);
            ShapeItems.Clear();
            undoStack.Clear();
            redoStack.Clear();
            if (isServer)
            {
                ShapeItems.Clear();
                foreach(ShapeItem s in shapeList)
                    ShapeItems.Add(s);
            }
        }

        /// <summary>
        ///     Used for testing. Sets the machine instance to a mock machine instance
        /// </summary>
        /// <param name="mockMachine">Mock IShapeListener instance</param>
        public void SetMachine(IShapeListener mockMachine)
        {
            machine = mockMachine;
        }
    }
}
