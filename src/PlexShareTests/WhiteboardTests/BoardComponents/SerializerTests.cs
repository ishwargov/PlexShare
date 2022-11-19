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
    [Collection("Sequential")]
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
            shapeItems.Add(Utility.CreateShape(start, end, "PathGeometry", "randomID"));

            var jsonString = _serializer.SerializeShapeItems(shapeItems);
            List<ShapeItem> deserializedObject = _serializer.DeserializeShapeItems(jsonString);

            Assert.True(Utility.CompareShapeItems(shapeItems, deserializedObject));

        }

        [Fact]
        public void Serializer_WBServerShapeItemSerialization()
        {
            WBServerShape wBServerShape1 = new(null, Operation.CreateSnapshot, "randomID", 1);
            var jsonString1 = _serializer.SerializeWBServerShape(wBServerShape1);
            WBServerShape deserializedObject1 = _serializer.DeserializeWBServerShape(jsonString1);

            List<SerializableShapeItem> serializableShapeList = new List<SerializableShapeItem>() { new SerializableShapeItem()};
            WBServerShape wBServerShape2 = new(serializableShapeList, Operation.Creation, "randomID", 1);
            var jsonString2 = _serializer.SerializeWBServerShape(wBServerShape2);
            WBServerShape deserializedObject2 = _serializer.DeserializeWBServerShape(jsonString2);

            Assert.True(Utility.CompareBoardServerShapes(wBServerShape1,deserializedObject1) 
                && Utility.CompareBoardServerShapes(wBServerShape2, deserializedObject2));
        }
        [Fact]
        public void ConvertToSerializableShapeItem_NullReturnsNull()
        {
            Assert.Null(_serializer.ConvertToSerializableShapeItem(null as List<ShapeItem>));
        }
        [Fact]
        public void ConvertToShapeItem_NullReturnsNull()
        {
            Assert.Null(_serializer.ConvertToShapeItem(null as List<SerializableShapeItem>));
        }
    }
}
