/***************************
 * Filename    = Serializer.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Consists of methods to serialize ShapeItems and WBServerShapes
 *               and conversion methods.
 ***************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media;
using System.Windows;
using Newtonsoft.Json;

namespace PlexShareWhiteboard.BoardComponents
{
    public class Serializer
    {
        /// <summary>
        /// Converts a list of serializable ShapeItems to a list of ShapeItems
        /// </summary>
        /// <param name="serializableShapeItems">List of serializable ShapeItems</param>
        /// <returns></returns>
        public List<ShapeItem> ConvertToShapeItem(
            List<SerializableShapeItem> serializableShapeItems
        )
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

        /// <summary>
        /// Converts a list of ShapeItems to a list of serializable ShapeItems
        /// </summary>
        /// <param name="shapeItems">List of ShapeItems</param>
        /// <returns></returns>
        public List<SerializableShapeItem> ConvertToSerializableShapeItem(
            List<ShapeItem> shapeItems
        )
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

        /// <summary>
        /// Converts a SerializableShapeItem to a ShapeItem
        /// </summary>
        /// <param name="shapeItem">Serializable ShapeItem to be converted</param>
        /// <returns></returns>
        public ShapeItem ConvertToShapeItem(SerializableShapeItem shapeItem)
        {
            if (shapeItem == null)
                return null;

            Geometry g = null;
            if (shapeItem.GeometryString == "EllipseGeometry")
            {
                Rect boundingBox = new Rect(shapeItem.Start, shapeItem.End);
                g = new EllipseGeometry(boundingBox);
            }
            else if (shapeItem.GeometryString == "RectangleGeometry")
            {
                Rect boundingBox = new Rect(shapeItem.Start, shapeItem.End);
                g = new RectangleGeometry(boundingBox);
            }
            else if (shapeItem.GeometryString == "PathGeometry")
            {
                PathGeometry g1 = new PathGeometry();
                if (shapeItem.PointList != null)
                {
                    for (int i = 1; i < shapeItem.PointList.Count; i++)
                    {
                        Point curPoint = shapeItem.PointList[i];
                        Point prevPoint = shapeItem.PointList[i - 1];
                        var line = new LineGeometry(curPoint, prevPoint);
                        var circle = new EllipseGeometry(curPoint, 0.1, 0.1);
                        g1.AddGeometry(circle);
                        g1.AddGeometry(line);
                    }
                }
                g = g1;
            }
            else if (shapeItem.GeometryString == "LineGeometry")
            {
                g = new LineGeometry(shapeItem.Start, shapeItem.End);
            }
            else if (shapeItem.GeometryString == "GeometryGroup")
            {
                FormattedText formattedText = new FormattedText(
                    shapeItem.TextString,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    32,
                    shapeItem.Stroke,
                    3
                );
                g = formattedText.BuildGeometry(shapeItem.Start);
            }

            return new ShapeItem
            {
                Geometry = g,
                FontSize = shapeItem.FontSize,
                GeometryString = shapeItem.GeometryString,
                TextString = shapeItem.TextString,
                PointList = shapeItem.PointList,
                Start = shapeItem.Start,
                End = shapeItem.End,
                Fill = shapeItem.Fill,
                Stroke = shapeItem.Stroke,
                ZIndex = shapeItem.ZIndex,
                StrokeThickness = shapeItem.StrokeThickness,
                Id = shapeItem.Id,
                User = shapeItem.User,
                TimeStamp = shapeItem.TimeStamp,
                AnchorPoint = shapeItem.AnchorPoint,
            };
        }

        /// <summary>
        /// Converts a ShapeItem to a SerializableShapeItem
        /// </summary>
        /// <param name="shapeItem">ShapeItem to be converted</param>
        /// <returns></returns>
        public SerializableShapeItem ConvertToSerializableShapeItem(ShapeItem shapeItem)
        {
            if (shapeItem == null)
                return null;
            return new SerializableShapeItem
            {
                FontSize = shapeItem.FontSize,
                GeometryString = shapeItem.Geometry.GetType().Name,
                TextString = shapeItem.TextString,
                PointList = shapeItem.PointList,
                Start = shapeItem.Start,
                End = shapeItem.End,
                Fill = shapeItem.Fill,
                Stroke = shapeItem.Stroke,
                ZIndex = shapeItem.ZIndex,
                StrokeThickness = shapeItem.StrokeThickness,
                Id = shapeItem.Id,
                User = shapeItem.User,
                TimeStamp = shapeItem.TimeStamp,
                AnchorPoint = shapeItem.AnchorPoint,
            };
        }

        /// <summary>
        /// Converts a list of ShapeItems to a serialized json string using the Newtonsoft library.
        /// </summary>
        /// <param name="boardShapes">List of ShapeItems to be converted</param>
        /// <returns></returns>
        public string SerializeShapeItems(List<ShapeItem> boardShapes)
        {
            try
            {
                List<SerializableShapeItem> shapeItems = new List<SerializableShapeItem>();
                foreach (ShapeItem shapeItem in boardShapes)
                {
                    shapeItems.Add(ConvertToSerializableShapeItem(shapeItem));
                }
                var jsonString = JsonConvert.SerializeObject(shapeItems, Formatting.Indented);
                return jsonString;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: Serializer:SerializeShapeItems");
                Trace.WriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Converts a serialized json string to a List of ShapeItems by deserializing using the Newtonsoft library.
        /// </summary>
        /// <param name="jsonString">Json string to be deserialized</param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts a WBServerShape to a serialized json string using the Newtonsoft library.
        /// </summary>
        /// <param name="wBServerShape">WBServerShape to be serialized</param>
        /// <returns></returns>
        public string SerializeWBServerShape(WBServerShape wBServerShape)
        {
            try
            {
                return JsonConvert.SerializeObject(wBServerShape, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: Serializer:SerializeWBServerShape");
                Trace.WriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Converts a serialized json string to a WBServerShape by deserializing using the Newtonsoft library.
        /// </summary>
        /// <param name="jsonString">Json string to be deserialized</param>
        /// <returns></returns>
        public WBServerShape DeserializeWBServerShape(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<WBServerShape>(jsonString);
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
