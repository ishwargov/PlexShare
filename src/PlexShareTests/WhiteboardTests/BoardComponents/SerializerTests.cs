/***************************
 * Filename    = SerializerTests.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share Tests
 *
 * Project     = White Board Tests
 *
 * Description = Tests for Serializer.cs.
 ***************************/

using System.Windows;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareTests.WhiteboardTests.BoardComponents
{
    [Collection("Sequential")]
    public class SerializerTests
    {
        Serializer _serializer;
        Utility utility;

        /// <summary>
        ///     Setup for tests
        /// </summary>
        public SerializerTests()
        {
            _serializer = new Serializer();
            utility = new Utility();
        }

        /// <summary>
        ///     Testing serialization and deserialization of ShapeItems.
        /// </summary>
        [Fact]
        public void Serializer_ShapeItemSerialization()
        {
            List<ShapeItem> shapeItems = new List<ShapeItem>();
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            shapeItems.Add(utility.CreateShape(start, end, "EllipseGeometry", "randomID"));
            shapeItems.Add(utility.CreateShape(start, end, "RectangleGeometry", "randomID"));
            shapeItems.Add(utility.CreateShape(start, end, "LineGeometry", "randomID"));
            shapeItems.Add(utility.CreateShape(start, end, "GeometryGroup", "randomID"));
            shapeItems.Add(utility.CreateShape(start, end, "PathGeometry", "randomID"));

            var jsonString = _serializer.SerializeShapeItems(shapeItems);
            List<ShapeItem> deserializedObject = _serializer.DeserializeShapeItems(jsonString);

            Assert.True(utility.CompareShapeItems(shapeItems, deserializedObject));

        }

        /// <summary>
        ///     Testing serialization and deserialization of WBServerShape.
        /// </summary>
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

            Assert.True(utility.CompareBoardServerShapes(wBServerShape1,deserializedObject1) 
                && utility.CompareBoardServerShapes(wBServerShape2, deserializedObject2));
        }

        /// <summary>
        ///     Testing null of Shape Item serialization.
        /// </summary>
        [Fact]
        public void ConvertToSerializableShapeItem_NullReturnsNull()
        {
            Assert.Null(_serializer.ConvertToSerializableShapeItem(null as List<ShapeItem>));
        }

        /// <summary>
        ///     Testing null of WBServerShape serialization.
        /// </summary>
        [Fact]
        public void ConvertToShapeItem_NullReturnsNull()
        {
            Assert.Null(_serializer.ConvertToShapeItem(null as List<SerializableShapeItem>));
        }
    }
}
