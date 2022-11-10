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

namespace PlexShareWhiteboard.Server
{
    internal class ServerSnapshotHandler
    {
        public ObservableCollection<ShapeItem> LoadBoard(string boardShapesPath)
        {
            try
            {
                var jsonString = File.ReadAllText(boardShapesPath);
                var boardShapes = JsonConvert.DeserializeObject<List<SerialisableShapeItem>>(
                    jsonString
                //new JsonSerializerSettings
                //{
                //    TypeNameHandling = TypeNameHandling.All
                //}
                );
                ObservableCollection<ShapeItem> result = new ObservableCollection<ShapeItem>();
                foreach (var boardShape in boardShapes)
                {
                    Trace.WriteLine(boardShape);

                    result.Add(ConvertToShapeItem(boardShape));
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error Occured: SnapshotHandler:Load");
                Trace.WriteLine(ex.Message);
            }

            return new ObservableCollection<ShapeItem>();
        }

        public int SaveBoard(ObservableCollection<ShapeItem> boardShapes, string boardShapesPath)
        {
            try
            {
                List<SerialisableShapeItem> shapeItems = new List<SerialisableShapeItem>();
                foreach (ShapeItem shapeItem in boardShapes)
                {
                    shapeItems.Add(ConvertToSerializableShapeItem(shapeItem));
                }
                var jsonString = JsonConvert.SerializeObject(
                    shapeItems,
                    Formatting.Indented
                );
                File.WriteAllText(boardShapesPath, jsonString);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error Occured: SnapshotHandler:Save");
                Trace.WriteLine(ex.Message);
            }

            return 0;
        }

        public ShapeItem ConvertToShapeItem(SerialisableShapeItem x)
        {
            Geometry g = null;
            if (x.GeometryString == "EllipseGeometry")
            {
                Rect boundingBox = new Rect(x.Start, x.End);
                g = new EllipseGeometry(boundingBox);
            }
            else if (x.GeometryString == "RectangleGeometry")
            {
                Rect boundingBox = new Rect(x.Start, x.End);
                g = new RectangleGeometry(boundingBox);
            }
            else if (x.GeometryString== "PathGeometry")
            {
                PathGeometry g1 = new PathGeometry();

                for (int i = 1; i < x.PointList.Count; i++)
                {
                    Point curPoint = x.PointList[i];
                    Point prevPoint = x.PointList[i - 1];
                    var line = new LineGeometry(curPoint, prevPoint);
                    g1.AddGeometry(line);
                }
                g = g1;
            }
            else if (x.GeometryString == "LineGeometry")
            {
                g = new LineGeometry(x.Start, x.End);

            }
            else if (x.GeometryString== "TextGeometry")
            {
                // this geometry string does not have a corresponding 
                FormattedText formattedText = new FormattedText(
                    x.TextString,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    x.FontSize,
                    x.Stroke,
                    3);

            }
            Trace.WriteLine(g);
            ShapeItem y = new ShapeItem
            {
                Geometry = g,
                FontSize = x.FontSize,
                GeometryString = x.GeometryString,
                TextString = x.TextString,
                PointList = x.PointList,
                Start = x.Start,
                End = x.End,
                Fill = x.Fill,
                Stroke = x.Stroke,
                ZIndex = x.ZIndex,
                StrokeThickness= x.StrokeThickness,
                Id = x.Id,
                User = x.User,
                TimeStamp = x.TimeStamp,
                AnchorPoint = x.AnchorPoint,
            };

            return y;
        }

        public SerialisableShapeItem ConvertToSerializableShapeItem(ShapeItem x)
        {
            SerialisableShapeItem y = new SerialisableShapeItem
            {
                FontSize = x.FontSize,
                GeometryString = x.GeometryString,
                TextString = x.TextString,
                PointList = x.PointList,
                Start = x.Start,
                End = x.End,
                Fill = x.Fill,
                Stroke = x.Stroke,
                ZIndex = x.ZIndex,
                StrokeThickness= x.StrokeThickness,
                Id = x.Id,
                User = x.User,
                TimeStamp = x.TimeStamp,
                AnchorPoint = x.AnchorPoint,
            };

            return y;
        }
    }
}
