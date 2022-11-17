/**
 * Owned By: Joel Sam Mathew
 * Created By: Joel Sam Mathew
 * Date Created: 22/10/2022
 * Date Modified: 08/11/2022
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace PlexShareWhiteboard.BoardComponents
{
    public class Serializer
    {
        public List<ShapeItem> ConvertToShapeItem(List<SerializableShapeItem> serializableShapeItems)
        {
            if (serializableShapeItems == null)
                return null;
            List<ShapeItem> shapeItems = new List<ShapeItem>(); 
            foreach (SerializableShapeItem serializableShapeItem in serializableShapeItems)
            {
                shapeItems.Add(ConvertToShapeItem(serializableShapeItem));
            }
            return shapeItems;
        }

        public List<SerializableShapeItem> ConvertToSerializableShapeItem(List<ShapeItem> shapeItems)
        {
            if (shapeItems == null)
                return null;
            List<SerializableShapeItem> serializableShapeItems = new List<SerializableShapeItem>();
            foreach (ShapeItem shapeItem in shapeItems)
            {
                serializableShapeItems.Add(ConvertToSerializableShapeItem(shapeItem));
            }
            return serializableShapeItems;
        }
        public ShapeItem ConvertToShapeItem(SerializableShapeItem x)
        {

            Trace.WriteLine("[WhiteBoard] converting to shape item " + x.GeometryString);
            if (x == null)
                return null;
            Trace.WriteLine("[WhiteBoard] converting to shape item chk -1" + x.GeometryString);

            Geometry g = null;
            Trace.WriteLine("[WhiteBoard] converting to shape item chk -2" + x.GeometryString + " x.FontSize : "+ x.FontSize + "x.Stroke:"+ x.Stroke + "x.TextString"+ x.TextString);

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
            else if (x.GeometryString == "PathGeometry")
            {
                PathGeometry g1 = new PathGeometry();
                if(x.PointList != null)
                {
                    for (int i = 1; i < x.PointList.Count; i++)
                    {
                        Point curPoint = x.PointList[i];
                        Point prevPoint = x.PointList[i - 1];
                        var line = new LineGeometry(curPoint, prevPoint);
                        var circle = new EllipseGeometry(curPoint, 0.1, 0.1);
                        g1.AddGeometry(circle);
                        g1.AddGeometry(line);
                    }
                }
                g = g1;
            }
            else if (x.GeometryString == "LineGeometry")
            {
                g = new LineGeometry(x.Start, x.End);

            }
            else if (x.GeometryString == "GeometryGroup")
            {
                // this geometry string does not have a corresponding 
                Trace.WriteLine("geometry groupil inside start");
                FormattedText formattedText = new FormattedText(
                    x.TextString,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    32,
                    x.Stroke,
                    3);
                Trace.WriteLine("geometry groupil inside middle");
                g=formattedText.BuildGeometry(x.Start);
                Trace.WriteLine("geometry groupil inside end");

            }
            Trace.WriteLine(g);
            Trace.WriteLine("[WhiteBoard] converting to shape item chk -3" + x.GeometryString);

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
                StrokeThickness = x.StrokeThickness,
                Id = x.Id,
                User = x.User,
                TimeStamp = x.TimeStamp,
                AnchorPoint = x.AnchorPoint,
            };

            return y;
        }

        public SerializableShapeItem ConvertToSerializableShapeItem(ShapeItem x)
        {
            if (x == null)
                return null;
            SerializableShapeItem y = new SerializableShapeItem
            {
                FontSize = x.FontSize,
                GeometryString = x.Geometry.GetType().Name,
                //GeometryString = x.GeometryString,
                TextString = x.TextString,
                PointList = x.PointList,
                Start = x.Start,
                End = x.End,
                Fill = x.Fill,
                Stroke = x.Stroke,
                ZIndex = x.ZIndex,
                StrokeThickness = x.StrokeThickness,
                Id = x.Id,
                User = x.User,
                TimeStamp = x.TimeStamp,
                AnchorPoint = x.AnchorPoint,
            };

            return y;
        }

        public string SerializeShapeItems(List<ShapeItem> boardShapes)
        {
            try
            {
                List<SerializableShapeItem> shapeItems = new List<SerializableShapeItem>();
                foreach (ShapeItem shapeItem in boardShapes)
                {
                    shapeItems.Add(ConvertToSerializableShapeItem(shapeItem));
                }
                var jsonString = JsonConvert.SerializeObject(
                    shapeItems,
                    Formatting.Indented
                );
                return jsonString;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: Serializer:SerializeShapeItems");
                Trace.WriteLine(ex.Message);
            }
            return null;
        }

        public List<ShapeItem> DeserializeShapeItems(string jsonString)
        {
            try
            {
                var boardShapes = JsonConvert.DeserializeObject<List<SerializableShapeItem>>(
                    jsonString
                );
                List<ShapeItem> result = new List<ShapeItem>();
                foreach (var boardShape in boardShapes)
                {
                    Trace.WriteLine(boardShape);
                    result.Add(ConvertToShapeItem(boardShape));
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: Serializer:DeserializeShapeItems");
                Trace.WriteLine(ex.Message);
            }
            return null;
        }

        public string SerializeWBServerShape(WBServerShape wBServerShape)
        {
            try
            {
                return JsonConvert.SerializeObject(
                    wBServerShape,
                    Formatting.Indented
                );
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: Serializer:SerializeWBServerShape");
                Trace.WriteLine(ex.Message);
            }
            return null;
        }
        public WBServerShape DeserializeWBServerShape(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<WBServerShape>(
                    jsonString
                );
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: Serializer:DeserializeWBServerShape");
                Trace.WriteLine(ex.Message);
            }
            return null;
        }
    }

}
