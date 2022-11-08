using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        public void ShapeBuilding(Point a)
        {
            //Debug.WriteLine("Shape Building ");

            if (mode == "view_mode")
            {
                ;
            }

            else if (mode == "delete_mode")
            {
                ;
            }
            else if (mode == "scale_mode")
            {
                ;
            }
            else if (mode == "dimensionChange_mode")
            {
                ;
            }
            else if (mode == "translate_mode")
            {

                UnHighLightIt();
                ShapeItem shape = select.selectedObject;
                Rect boundingBox = shape.Geometry.Bounds;
                double bx = select.selectedObject.AnchorPoint.X + (a.X - select.InitialSelectionPoint.X);
                double by = select.selectedObject.AnchorPoint.Y + (a.Y - select.InitialSelectionPoint.Y);
                double width = boundingBox.Width;
                double height = boundingBox.Height;
                Point p1 = new Point(bx, by);
                Point p2 = new Point(bx + width, by + height);

                if (select.selectedObject.Geometry.GetType() == (new RectangleGeometry()).GetType())
                    CreateShape(p1, p2, "rectangle", select.selectedObject.Id);
                else if (select.selectedObject.Geometry.GetType() == (new EllipseGeometry()).GetType())
                    CreateShape(p1, p2, "ellipse", select.selectedObject.Id);
                else if (select.selectedObject.Geometry.GetType() == (new PathGeometry()).GetType())
                {
                    Debug.WriteLine("translating geomteries " + ShapeItems.Count);
                    Geometry g = select.selectedObject.Geometry;
                    g.Transform = new TranslateTransform(100, 100);
                    ShapeItem newShape22 = new ShapeItem
                    {
                        Geometry = g,
                        Fill = fillBrush,
                        Stroke = strokeBrush,
                        ZIndex = curZIndex,
                        Id = select.selectedObject.Id
                    };

                    for (int i = 0; i < ShapeItems.Count; i++)
                    {
                        if (ShapeItems[i].Id == select.selectedObject.Id)
                            ShapeItems[i] = newShape22;
                    }
                    Debug.WriteLine(" Post      translating geomteries " + ShapeItems.Count);


                }
                HighLightIt(p1, p2);
            }
            else if (mode == "create_rectangle")
            {

                if (lastShape != null)
                {
                    Point _anchorPoint = lastShape.AnchorPoint;
                    CreateShape(_anchorPoint, a, "rectangle", lastShape.Id);
                }
            }
            else if (mode == "create_ellipse")
            {
                if (lastShape != null)
                {
                    Point _anchorPoint = lastShape.AnchorPoint;
                    CreateShape(_anchorPoint, a, "ellipse", lastShape.Id);
                }
            }
            else if (mode == "create_freehand")
            {
                if (lastShape != null)
                {

                    Point _anchorPoint = lastShape.AnchorPoint;
                    PathGeometry g1 = (PathGeometry)lastShape.Geometry;

                    var line = new LineGeometry(a, _anchorPoint);
                    g1.AddGeometry(line);

                    ShapeItem newShape = new ShapeItem
                    {
                        Geometry = g1,
                        Fill = fillBrush,
                        Stroke = strokeBrush,
                        ZIndex = curZIndex,
                        AnchorPoint = a,
                        Id = lastShape.Id,
                        StrokeThickness = 5,
                    };
                    lastShape = newShape;


                    for (int i = 0; i < ShapeItems.Count; i++)
                    {
                        if (ShapeItems[i].Id == lastShape.Id)
                            ShapeItems[i] = newShape;
                    }
                }
            }
        }

    }
}
