using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Diagnostics;
using Client.Models;
using System.Reflection;
using System.Windows.Ink;
using System.Windows.Media.Media3D;

namespace PlexShareTests.WhiteboardTests
{
    public class Utility
    {
        public static bool CompareShapeItems(ShapeItem shape1, ShapeItem shape2)
        {
            if (shape1 == null && shape2 == null)
                return true;

            Debug.WriteLine(shape1);
            Debug.WriteLine(shape2);
            return shape1.Geometry.Equals(shape2.Geometry)
                && shape1.GeometryString.Equals(shape2.GeometryString)
                && shape1.TextString.Equals(shape2.TextString)
                && shape1.Start.Equals(shape2.Start)
                && shape1.End.Equals(shape2.End)
                && shape1.Fill.Equals(shape2.Fill)
                && shape1.Stroke.Equals(shape2.Stroke)
                && shape1.ZIndex.Equals(shape2.ZIndex)
                && shape1.FontSize.Equals(shape2.FontSize)
                && shape1.StrokeThickness.Equals(shape2.StrokeThickness)
                && shape1.Id.Equals(shape2.Id)
                && shape1.User.Equals(shape2.User)
                && shape1.TimeStamp.Equals(shape2.TimeStamp)
                && shape1.AnchorPoint.Equals(shape2.AnchorPoint);
        }

        public static bool CompareShapeItems(
            List<ShapeItem> shapeItems1,
            List<ShapeItem> shapeItems2
        )
        {
            if (shapeItems1 == null && shapeItems2 == null)
                return true;
            if (shapeItems1.Count != shapeItems2.Count)
                return false;
            for (var i = 0; i < shapeItems1.Count; i++)
                if (!CompareShapeItems(shapeItems1[i], shapeItems2[i]))
                    return false;
            return true;
        }

        public static bool CompareBoardServerShapes(WBServerShape shape1, WBServerShape shape2)
        {
            Serializer serializer = new Serializer();
            return serializer.SerializeWBServerShape(shape1)
                == serializer.SerializeWBServerShape(shape2);
        }

        public static ShapeItem CreateShape(Point start, Point end, string name, String id, string textDataOpt = "")
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
                TextString = textDataOpt
            };

            return newShape;
        }
    }
}
