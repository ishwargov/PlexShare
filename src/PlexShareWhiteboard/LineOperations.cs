/***************************
 * Filename    = LineOperations.cs
 *
 * Author      = Deon Saji
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This handles line operations like transform and translate.
 *               This also implements generating a line given start and end points and generating a bounding line
 *               after selecting a line.   
 ***************************/

using System;
using System.Windows.Media;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;
using System.Diagnostics;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        /// <summary>
        ///  Create a new shapeitem with geometry as line geometry.
        /// </summary>
        public ShapeItem GenerateLine(double x1, double y1, double x2, double y2, Brush f1, Brush s1, String id, int zi)
        {
            ShapeItem s = new ShapeItem
            {
                Geometry = new LineGeometry(new Point(x1, y1), new Point(x2, y2)),
                Fill = f1,
                Stroke = s1,
                ZIndex = zi,
                Id = id
            };
            return s;
        }

        /// <summary>
        /// Generates a boundingline for the selected line. 
        /// </summary>
        /// <param name="shape">shape object</param>
        /// <returns>Created line geometry</returns>
        public Geometry GenerateBoundingLine(ShapeItem shape)
        {
            LineGeometry boundingLine = new();
            boundingLine.StartPoint = new Point(shape.Start.X, shape.Start.Y);
            boundingLine.EndPoint = new Point(shape.End.X, shape.End.Y);
            return boundingLine;
        }
        public void HelperSelectLine(Rect boundingBox, int tempZIndex, int i, Point a)
        {
            Trace.WriteLine("[Whiteboard]  " + " HelperSelectLine ");
            //Selecting vertical and horizontal lines
            if ((boundingBox.Width == 0 || boundingBox.Height == 0))
            {
                select.ifSelected = true;
                select.selectedObject = ShapeItems[i];
                select.initialSelectionPoint = a;
                tempZIndex = ShapeItems[i].ZIndex;
                select.selectBox = 0;
            }
        }

        /// <summary>
        /// Implements transformation of a line. On clicking either of the two blobs in the boundingline
        /// the clicked end can be moved to any other point keeping the other point as pivot. 
        /// </summary>
        /// <param name="a">Point at which mouse is clicked</param>
        /// <param name="shape">shape object</param>
        public void TransformingLine(Point a, ShapeItem shape)
        {
            Trace.WriteLine("[Whiteboard]  " + " Transforming Line " + "point clicked is " + a);
            UnHighLightIt();
            //Point which moves keeping other point as pivot
            Point move_point = a;
            //Check which end to keep as pivot 
            if (ClickInsideHighlightBox(shape.Start, select.initialSelectionPoint, blobSize / 2) == 1)
            {
                move_point = shape.Start;
                lastShape= UpdateShape(a, shape.End, "LineGeometry", shape);
            }
            else
            {
                move_point = shape.End;
                lastShape= UpdateShape(shape.Start, a, "LineGeometry", shape);
            }
            LineGeometry boundingLine = (LineGeometry)GenerateBoundingLine(lastShape);
            Debug.WriteLine("selected boundingline " + boundingLine.StartPoint + " " + boundingLine.EndPoint);
            HighLightIt(boundingLine);
        }

        /// <summary>
        /// Implements translation of line. Both start and end points of the line change after translation.
        /// </summary>
        /// <param name="boundingBox">Bounding rectangle enclosing the line before translation</param>
        /// <param name="shape">Shape object</param>
        /// <param name="p1">Start point of the rectangle bounding the translated line</param>
        /// <param name="p2">End point of the rectangle bounding translated line</param>
        /// <param name="width">Width of the bounding box</param>
        /// <param name="height">Height of the bounding box</param>
        public void TranslatingLine(Rect boundingBox, ShapeItem shape, Point p1, Point p2, double width, double height)
        {
            Trace.WriteLine("[Whiteboard]  " + " Translating Line " + p1.ToString() + " " + p2.ToString() + "   " + shape.AnchorPoint.ToString());
            // Lines with negative slope( of the kind '\')
            if (
                (Math.Abs(boundingBox.X - shape.Start.X) < 5 && Math.Abs(boundingBox.Y - shape.Start.Y) < 5)
                ||
                (Math.Abs(boundingBox.X - shape.End.X) < 5 && Math.Abs(boundingBox.Y - shape.End.Y) < 5)

                )
            {
                    ;
            }
            //Lines of the kind '/'   
            else
            {
                double x1 = p1.X + width;
                double y1 = p1.Y;
                double x2 = p1.X;
                double y2 = p1.Y + height;
                p1 = new Point(x2, y2);
                p2 = new Point(x1, y1);
            }
            lastShape=UpdateShape(p1, p2, shape.Geometry.GetType().Name, shape, shape.TextString);
            LineGeometry boundingLine = (LineGeometry)GenerateBoundingLine(lastShape);
            HighLightIt(boundingLine);
        }
    }
}
