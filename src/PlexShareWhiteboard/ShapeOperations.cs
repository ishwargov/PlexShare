/***************************
 * Filename    = ShapeOperations.cs
 *
 * Author      = Asha Jose
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is part of View Model.
 *               This contains all the operations for shape objects ellipse and 
 *               rectangle.
 ***************************/

using System;
using System.Windows.Media;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;
using System.Globalization;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        // this is not for creation but for updating a created shape as we are using lastid.
        public ShapeItem UpdateShape(Point start, Point end, string name, ShapeItem oldShape, string textDataOpt = "")
        {
            Rect boundingBox = new (start, end);
            Geometry geometry;
            geometry = new RectangleGeometry(boundingBox);

            if (name == "EllipseGeometry")
                geometry = new EllipseGeometry(boundingBox);
            else if (name == "LineGeometry")
                geometry = new LineGeometry(start, end);
            else if (name == "GeometryGroup")
            {
                // Create the formatted text based on the properties set.
                FormattedText formattedText = new(
                  textDataOpt,
                  CultureInfo.GetCultureInfo("en-us"),
                  FlowDirection.LeftToRight,
                  new Typeface("Tahoma"),
                  32,
                  Brushes.Black);


                // Build the geometry object that represents the text.
                geometry = formattedText.BuildGeometry(start);

            }
          
            ShapeItem newShape = new ShapeItem
            {
                Geometry = geometry,
                Fill = oldShape.Fill,
                Stroke = oldShape.Stroke,
                ZIndex = oldShape.ZIndex,
                AnchorPoint = name == "LineGeometry" ? new Point(geometry.Bounds.X, geometry.Bounds.Y) : start,
                Id = oldShape.Id,
                TextString = textDataOpt,
                StrokeThickness = name == "GeometryGroup" ? 1 : oldShape.StrokeThickness,
                Start = start,
                End = end
            };

            for (int i = 0; i < ShapeItems.Count; i++)
            {

                if (ShapeItems[i].Id == oldShape.Id)
                {
                    ShapeItems[i] = newShape;
                }
            }

            return newShape;
        }
        public ShapeItem CreateShape(Point start, Point end, string name, String id, string textDataOpt = "")
        {
            Rect boundingBox = new (start, end);
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
                  Brushes.Black);

                
                // Build the geometry object that represents the text.
                geometry = formattedText.BuildGeometry(start);

            }
            ShapeItem newShape = new ShapeItem
            {
                Geometry = geometry,
                GeometryString = name,
                Start = start,
                End = end,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
                StrokeThickness = name == "GeometryGroup" ? 1 : strokeThickness,
                AnchorPoint = name == "LineGeometry" ? new Point(geometry.Bounds.X, geometry.Bounds.Y) : start,
                Id = id,
                TextString = textDataOpt
            };
           
            for (int i = 0; i < ShapeItems.Count; i++)
            {

                if (ShapeItems[i].Id == id)
                {
                    ShapeItems[i] = newShape;
                }
            }
            return newShape;
        }

        public void DeleteShape(Point a)
        {
            int tempZIndex = -1;
            ShapeItem toDelete = null;

            for (int i = ShapeItems.Count - 1; i >= 0; i--)
            {
                Geometry Child = ShapeItems[i].Geometry;

                if (ShapeItems[i].ZIndex > tempZIndex && Child.FillContains(a))
                {
                    tempZIndex = ShapeItems[i].ZIndex;
                    toDelete = ShapeItems[i];
                }
                else if (ShapeItems[i].ZIndex > tempZIndex &&
                    (Child.GetType().Name == "PathGeometry" ||  Child.GetType().Name == "LineGeometry" ||
                    Child.GetType().Name == "GeometryGroup") &&
                    PointInsideRect(Child.Bounds, a))
                {
                    //Trace.WriteLine("[Whiteboard]  " + " child bounds "+ Child.Bounds.ToString());
                    tempZIndex = ShapeItems[i].ZIndex;
                    toDelete = ShapeItems[i];
                }
            }

            if (toDelete != null)
            {
                lastShape = toDelete;
                ShapeItems.Remove(toDelete);
            }
        }

        public void TransformShape(ShapeItem shape, double newXLen, double newYLen, int signX, int signY)
        {
            Rect boundingBox = shape.Geometry.Bounds;
            double ratio = Math.Abs(boundingBox.Width / boundingBox.Height);
            if (ratio < Math.Abs(newXLen / newYLen))
                newXLen = Math.Abs(ratio * newYLen) * signX;
            else
                newYLen = Math.Abs(newXLen / ratio) * signY;

            double newX = select.initialSelectionPoint.X + newXLen;
            double newY = select.initialSelectionPoint.Y + newYLen;
            Point p1 = new (boundingBox.X, boundingBox.Y);
            Point p2 = new (boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);

            int boxNumber = select.selectBox;
                
            if (boxNumber == 1 && signX * signY > 0)
            {
                p1 = new Point(newX, newY);
                p2 = new Point(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 2 && signX * signY < 0)
            {
                p1 = new Point(boundingBox.X, newY);
                p2 = new Point(newX, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 3 && signY * signX < 0)
            {
                p1 = new Point(newX, boundingBox.Y);
                p2 = new Point(boundingBox.X + boundingBox.Width, newY);
            }
            else if (boxNumber == 4 && signX * signY > 0)
            {
                p1 = new Point(boundingBox.X, boundingBox.Y);
                p2 = new Point(newX, newY);
            }

            if (boxNumber > 0)
            {
                shape = UpdateShape(p1, p2, shape.Geometry.GetType().Name, shape);
                lastShape = shape;
                HighLightIt(shape.Geometry.Bounds);
            }
        }

        public void DimensionChangingShape(Point a, ShapeItem shape)
        {
            Rect boundingBox = shape.Geometry.Bounds;
            Point p1 = new(boundingBox.X, boundingBox.Y);
            Point p2 = new(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);
            int boxNumber = select.selectBox;

            if (boxNumber == 5)
            {
                p1 = new Point(boundingBox.X, a.Y);
                p2 = new Point(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 6)
            {
                p1 = new Point(a.X, boundingBox.Y);
                p2 = new Point(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 7)
            {
                p1 = new Point(boundingBox.X, boundingBox.Y);
                p2 = new Point(a.X, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 8)
            {
                p1 = new Point(boundingBox.X, boundingBox.Y);
                p2 = new Point(boundingBox.X + boundingBox.Width, a.Y);
            }

            if (boxNumber > 0)
            {
                shape = UpdateShape(p1, p2, shape.Geometry.GetType().Name, shape,shape.TextString);
                lastShape = shape;
                //HighLightIt(shape.Geometry.Bounds);
            }
        }
        public void TranslatingShape(ShapeItem shape, Point p1, Point p2)
        {
            if (shape.Geometry.GetType().Name == "GeometryGroup")
            {
                textBoxLastShape = UpdateShape(p1, p2, shape.Geometry.GetType().Name, shape, shape.TextString);
                Rect rect = textBoxLastShape.Geometry.Bounds;
                HighLightTextBox(rect);
                //HighLightIt(textBoxLastShape.Geometry.Bounds);
            }
            else
            {
                lastShape = UpdateShape(p1, p2, shape.Geometry.GetType().Name, shape, shape.TextString);
                HighLightIt(p1, p2);
            }
        }
    }
}
