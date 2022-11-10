using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using PlexShareWhiteboard.BoardComponents;
using System.Globalization;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
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

        public void HelperSelectLine(Rect boundingBox, int tempZIndex, int i, Point a)
        {
            if ((boundingBox.Width == 0 || boundingBox.Height == 0))
            {
                select.ifSelected = true;
                select.selectedObject = ShapeItems[i];
                select.initialSelectionPoint = a;
                tempZIndex = ShapeItems[i].ZIndex;
                select.selectBox = 0;
            }
        }

        public void TransformingLine(Point a, ShapeItem shape)
        {
            UnHighLightIt();
            Point move_point = a;
            if (ClickInsideHighlightBox(shape.Start, select.initialSelectionPoint, blobSize / 2) == 1)
            {
                move_point = shape.Start;
                CreateShape(a, shape.End, "LineGeometry", shape.Id);
                //Debug.WriteLine("Start point selected for transform in line");
            }
            else
            {
                move_point = shape.End;
                lastShape = CreateShape(shape.Start, a, "LineGeometry", shape.Id);
            }
        }

        public void TranslatingLine(Rect boundingBox, ShapeItem shape, Point p1, Point p2, double width, double height)
        {
            // p1 : 
            // p2 :  

            Debug.WriteLine(" asha " + p1.ToString() + " " + p2.ToString() + "   " + shape.AnchorPoint.ToString());
            if (
                (Math.Abs(boundingBox.X - shape.Start.X) < 5 && Math.Abs(boundingBox.Y - shape.Start.Y) < 5)
                ||
                (Math.Abs(boundingBox.X - shape.End.X) < 5 && Math.Abs(boundingBox.Y - shape.End.Y) < 5)

                )
            {
                    ;
            }
            else
            {
                ;
                double x1 = p1.X + width;
                double y1 = p1.Y;
                double x2 = p1.X;
                double y2 = p1.Y + height;

                //deon
                //p1 = new Point(x1, y1);
                //p2 = new Point(x2, y2);

                p1 = new Point(x2, y2);
                p2 = new Point(x1, y1);


            }

            lastShape = UpdateShape(p1, p2, shape.Geometry.GetType().Name, shape, shape.TextString);
            HighLightIt(p1, p2);
        }
    }
}
