using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareWhiteboard;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareTests.WhiteboardTests.BoardComponents
{
    public class SerializerTests
    {
        Serializer _serializer;
        public SerializerTests()
        {
            _serializer = new Serializer();
        }

        [Fact]
        public void Serializer_ShapeItemSerialization()
        {
            List<ShapeItem> shapeItems = new List<ShapeItem>();
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            shapeItems.Add(Utility.CreateShape(start, end, "EllipseGeometry", "randomID"));
            shapeItems.Add(Utility.CreateShape(start, end, "RectangleGeometry", "randomID"));
            shapeItems.Add(Utility.CreateShape(start, end, "LineGeometry", "randomID"));
            shapeItems.Add(Utility.CreateShape(start, end, "GeometryGroup", "randomID"));

            var jsonString = _serializer.SerializeShapeItems(shapeItems);
            List<ShapeItem> deserializedObject = _serializer.DeserializeShapeItems(jsonString);

            Assert.True(Utility.CompareShapeItems(shapeItems, deserializedObject));

        }

        [Fact]
        public void Serializer_WBServerShapeItemSerialization()
        {
            WBServerShape wBServerShape = new(null, Operation.CreateSnapshot, "randomID", 1);
            var jsonString = _serializer.SerializeWBServerShape(wBServerShape);
            WBServerShape deserializedObject = _serializer.DeserializeWBServerShape(jsonString);

            Assert.True(Utility.CompareBoardServerShapes(wBServerShape,deserializedObject));
        }
    }
}
