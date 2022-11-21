/***************************
 * Filename    = Utility.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share Tests
 *
 * Project     = White Board Tests
 *
 * Description = Utility methods for Whiteboard Tests
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using System.Globalization;
using System.Windows.Media;
using System.Windows;

namespace PlexShareTests.WhiteboardTests
{
    [Collection("Sequential")]
    public class Utility
    {
        /// <summary>
        ///     Compares two ShapeItems and return true if equal.
        /// </summary>
        /// <param name="shape1">ShapeItem 1</param>
        /// <param name="shape2">ShapeItem 2</param>
        /// <returns>True if objects are same, false otherwise</returns>
        public bool CompareShapeItems(ShapeItem shape1, ShapeItem shape2)
        {
            if (shape1 == null && shape2 == null)
                return true;
            if (shape1 == null || shape2 == null)
                return false;

            return shape1.GeometryString == shape2.GeometryString
                && shape1.TextString == shape2.TextString
                && shape1.Start == shape2.Start
                && shape1.End == shape2.End
                && shape1.FontSize == shape2.FontSize
                && shape1.StrokeThickness == shape2.StrokeThickness
                && shape1.Id == shape2.Id
                && shape1.User == shape2.User
                && shape1.TimeStamp == shape2.TimeStamp
                && shape1.AnchorPoint == shape2.AnchorPoint
                && shape1.PointList == shape2.PointList;
        }

        /// <summary>
        ///     Compares two List of ShapeItems and return true if equal.
        /// </summary>
        /// <param name="shapeItems1">ShapeItem List 1</param>
        /// <param name="shapeItems2">ShapeItem List 2</param>
        /// <returns>True if objects are same, false otherwise</returns>
        public bool CompareShapeItems(
            List<ShapeItem> shapeItems1,
            List<ShapeItem> shapeItems2
        )
        {
            if (shapeItems1 == null && shapeItems2 == null)
                return true;
            if ((shapeItems2 == null) || (shapeItems1 == null) || (shapeItems1.Count != shapeItems2.Count))
                return false;
            for (var i = 0; i < shapeItems1.Count; i++)
            {
                if (!CompareShapeItems(shapeItems1[i], shapeItems2[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        ///     Compares two WBServerShapes and return true if equal.
        /// </summary>
        /// <param name="shape1">WBServerShapes 1</param>
        /// <param name="shape2">WBServerShapes 2</param>
        /// <returns>True if objects are same, false otherwise</returns>
        public bool CompareBoardServerShapes(WBServerShape shape1, WBServerShape shape2)
        {
            Serializer serializer = new Serializer();
            if (shape1 == null && shape2 == null)
                return true;

            if (shape1.UserID != shape2.UserID || shape1.Op != shape2.Op || shape1.SnapshotNumber != shape2.SnapshotNumber)
                return false;
            List<ShapeItem> shapeItems1 = serializer.ConvertToShapeItem(shape1.ShapeItems);
            List<ShapeItem> shapeItems2 = serializer.ConvertToShapeItem(shape2.ShapeItems);
            
            return CompareShapeItems(shapeItems1, shapeItems2);
        }

        /// <summary>
        ///     Creates a shape with the given parameters
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        /// <param name="name">String of geometry type</param>
        /// <param name="id">UID of the ShapeItem</param>
        /// <param name="textDataOpt">Text inside the textbox</param>
        /// <returns></returns>
        public ShapeItem CreateShape(Point start, Point end, string name, string id, string textDataOpt = "")
        {
            Rect boundingBox = new(start, end);
            Geometry geometry;
            geometry = new RectangleGeometry(boundingBox);

            if (name == "EllipseGeometry")
                geometry = new EllipseGeometry(boundingBox);
            else if (name == "LineGeometry")
            {
                geometry = new LineGeometry(start, end);
            }
            else if (name == "GeometryGroup")
            {
                // Create the formatted text based on the properties set.
                FormattedText formattedText = new FormattedText(
                    textDataOpt,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Tahoma"),
                    32,
                    Brushes.Black
                );

                // Build the geometry object that represents the text.
                geometry = formattedText.BuildGeometry(start);
            }
            ShapeItem newShape = new ShapeItem
            {
                Geometry = geometry,
                GeometryString = geometry.GetType().Name,
                Start = start,
                End = end,
                Fill = Brushes.Azure,
                Stroke = Brushes.Black,
                ZIndex = 1,
                AnchorPoint =
                    name == "LineGeometry"
                        ? new Point(geometry.Bounds.X, geometry.Bounds.Y)
                        : start,
                Id = id,
                TextString = textDataOpt,
                FontSize =32
            };

            return newShape;
        }

        /// <summary>
        ///     Creates a random shape for test purposes.
        /// </summary>
        /// <returns>Random ShapeItem</returns>
        public ShapeItem CreateRandomShape()
        {
            Random random = new();
            Dictionary<int, string> shapeTypes = new Dictionary<int, string>()
            {
                {0,"RectangleGeometry"},
                {1,"EllipseGeometry" },
                {2,"LineGeometry" },
                {3, "GeometryGroup"}
            };
            Point start = new Point(random.Next(0, 100), random.Next(0, 100));
            Point end = new Point(random.Next(0, 100), random.Next(0, 100));
            return CreateShape(start, end, shapeTypes[random.Next(0, 4)], RandomString(5));
        }

        /// <summary>
        ///     Generates a list of random ShapeItems representing a random WhiteBoard.
        /// </summary>
        /// <param name="n">Number of random shapeItems to generate</param>
        /// <returns>List of ShapeItems</returns>
        public List<ShapeItem> GenerateRandomBoardShapes(int n)
        {
            List<ShapeItem> boardShapes = new();
            Random random = new();
            
            for (var i = 0; i < n; i++)
            {
                boardShapes.Add(CreateRandomShape());
            }
            return boardShapes;
        }

        /// <summary>
        ///     Random string generator. Used for random IDs.
        /// </summary>
        /// <param name="length">Length of string</param>
        /// <returns>Random string of length</returns>
        public string RandomString(int length)
        {
            Random random = new();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        ///     Simulates the serialzation of an object when sent through the server
        /// </summary>
        /// <param name="newShapes">Object to be sent across server</param>
        /// <param name="op">Operation to be performed at other end of network</param>
        /// <returns>Serialized string</returns>
        public string SendThroughServer(List<ShapeItem> newShapes, Operation op)
        {
            Serializer _serializer = new();
            var newSerializedShapes = _serializer.ConvertToSerializableShapeItem(newShapes);
            WBServerShape wbShape = new WBServerShape(newSerializedShapes, op);
            return _serializer.SerializeWBServerShape(wbShape);
        }
    }
}
