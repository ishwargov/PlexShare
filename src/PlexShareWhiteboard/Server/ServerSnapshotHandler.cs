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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Server.Interfaces;

namespace PlexShareWhiteboard.Server
{
    public class ServerSnapshotHandler : IServerSnapshotHandler
    {
        /// <summary>
        ///     Gets and sets current snapshotNumber
        /// </summary>
        public int SnapshotNumber { get; set; }
        private Serializer _serializer;
        private List<Tuple<int, string, List<ShapeItem>>> _snapshotSummary = new();

        /// <summary>
        ///     Constructor for SnapshotHandler
        /// </summary>
        public ServerSnapshotHandler()
        {
            _serializer = new Serializer();
            SnapshotNumber = 0;
        }

        /// <summary>
        ///     Fetches and loades the snapshot corresponding to provided snapshotNumber.
        /// </summary>
        /// <param name="snapshotNumber">The number of the snapshots which needs to fetched.</param>
        /// <returns></returns>
        public List<ShapeItem> LoadBoard(int snapshotNumber)
        {
            try
            {
                if (snapshotNumber > SnapshotNumber) 
                    throw new ArgumentException("Invalid SnapshotNumber");
                
                var boardShapesPath = snapshotNumber + ".json";
                var jsonString = File.ReadAllText(boardShapesPath);
                Trace.WriteLine("[Whiteboard] ServerSnapshotHandler.LoadBoard: Deserialized file" + boardShapesPath);
                var shapeItems = _serializer.DeserializeShapeItems(jsonString);
                return shapeItems;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: SnapshotHandler:Load");
                Trace.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        ///     Saves the snapshot at the server.
        /// </summary>
        /// <param name="boardShapes">List containing all the shapes to save the snapshot.</param>
        /// <param name="userID">User who requested the saving of snapshot.</param>
        /// <returns></returns>
        public int SaveBoard(List<ShapeItem> boardShapes, string userID)
        {
            try
            {
                SnapshotNumber = SnapshotNumber + 1;
                string boardShapesPath = SnapshotNumber + ".json";
                var jsonString = _serializer.SerializeShapeItems(boardShapes);
                Trace.WriteLine("[Whiteboard] SnapshotHandler.Save: Saving in file in "+boardShapesPath);
                File.WriteAllText(boardShapesPath, jsonString);

                _snapshotSummary.Add(
                    new Tuple<int, string, List<ShapeItem>>(SnapshotNumber, userID, boardShapes));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: SnapshotHandler:Save");
                Trace.WriteLine(ex.Message);
            }
            return SnapshotNumber;
        }
    }
}
