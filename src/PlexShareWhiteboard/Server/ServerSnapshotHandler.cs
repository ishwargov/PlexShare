/**
 * Owned By: Joel Sam Mathew
 * Created By: Joel Sam Mathew
 * Date Created: 22/10/2022
 * Date Modified: 08/11/2022
**/

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Server.Interfaces;

namespace PlexShareWhiteboard.Server
{
    public class ServerSnapshotHandler : IServerSnapshotHandler
    {
        private Serializer _serializer;
        private int _snapshotNumber;
        private List<Tuple<int, string, List<ShapeItem>>> _snapshotSummary = new();
        public ServerSnapshotHandler()
        {
            _serializer = new Serializer();
            _snapshotNumber = 0;
        }

        public List<ShapeItem> LoadBoard(int snapshotNumber)
        {
            try
            {
                if (snapshotNumber > _snapshotNumber) 
                    throw new ArgumentException("Invalid SnapshotNumber");
                
                var boardShapesPath = snapshotNumber + ".json";
                var jsonString = File.ReadAllText(boardShapesPath);
                Trace.WriteLine("[Whiteboard] ServerSnapshotHandler.LoadBoard: Deserialized file" + boardShapesPath);
                //Trace.WriteLine("[Whiteboard] ServerSnapshotHandler.LoadBoard: Deserialized file"+jsonString);
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

        public int SaveBoard(List<ShapeItem> boardShapes, string userID)
        {
            try
            {
                _snapshotNumber = _snapshotNumber + 1;
                string boardShapesPath = _snapshotNumber + ".json";
                var jsonString = _serializer.SerializeShapeItems(boardShapes);
                Trace.WriteLine("[Whiteboard] SnapshotHandler.Save: Saving in file"+boardShapes);
                //Trace.WriteLine("[Whiteboard] SnapshotHandler.Save: Saving as "+jsonString);
                File.WriteAllText(boardShapesPath, jsonString);

                _snapshotSummary.Add(
                    new Tuple<int, string, List<ShapeItem>>(_snapshotNumber, userID, boardShapes));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: SnapshotHandler:Save");
                Trace.WriteLine(ex.Message);
            }
            return _snapshotNumber;
        }
        public int GetSnapshotNumber()
        {
            return _snapshotNumber;
        }
        public List<Tuple<int, string, List<ShapeItem>>> Summary()
        {
            return _snapshotSummary;
        }
    }
}
