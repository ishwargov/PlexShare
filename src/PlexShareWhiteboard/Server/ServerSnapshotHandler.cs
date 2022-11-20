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
        public int SnapshotNumber { get; set; }
        private List<Tuple<int, string, List<ShapeItem>>> _snapshotSummary = new();
        public ServerSnapshotHandler()
        {
            _serializer = new Serializer();
            SnapshotNumber = 0;
        }

        public List<ShapeItem> LoadBoard(int snapshotNumber)
        {
            try
            {
                if (snapshotNumber > SnapshotNumber) 
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
                SnapshotNumber = SnapshotNumber + 1;
                string boardShapesPath = SnapshotNumber + ".json";
                var jsonString = _serializer.SerializeShapeItems(boardShapes);
                Trace.WriteLine("[Whiteboard] SnapshotHandler.Save: Saving in file in "+boardShapesPath);
                //Trace.WriteLine("[Whiteboard] SnapshotHandler.Save: Saving as "+jsonString);
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
        //public int GetSnapshotNumber()
        //{
        //    return SnapshotNumber;
        //}
        //public void SetSnapshotNumber(int snapshotNumber)
        //{
        //    SnapshotNumber = snapshotNumber;
        //}
        public List<Tuple<int, string, List<ShapeItem>>> Summary()
        {
            return _snapshotSummary;
        }
    }
}
