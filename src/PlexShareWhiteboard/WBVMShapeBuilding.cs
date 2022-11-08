using System;
using System.Diagnostics;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        public void ShapeBuilding(Point a)
        {
            //Debug.WriteLine("Entering Shape Building......\n");

            if (mode == "transform_mode")
            {

                Debug.WriteLine("In transform mode\n");
                UnHighLightIt();

                double newXLen = a.X - select.initialSelectionPoint.X;
                double newYLen = a.Y - select.initialSelectionPoint.Y;
                ShapeItem shape = select.selectedObject;
                int signX = 1, signY = 1;

                if (newXLen < 0)
                    signX = -1;
                if (newYLen < 0)
                    signY = -1;
                
                if (shape.Geometry.GetType().Name == "PathGeometry")
                {

                    Debug.WriteLine("Transforming curve\n");
                    TransformCurve(a, shape);
                }
                else if (shape.Geometry.GetType().Name == "LineGeometry")
                {
                    Debug.WriteLine("Transforming line\n");
                    TransformingLine(a, shape);
                }
                else
                {

                    Debug.WriteLine("Transforming shape\n");
                    TransformShape(shape, newXLen, newYLen, signX, signY);
                }
            }
            else if (mode == "dimensionChange_mode")
            {
                Debug.WriteLine("In dimension changing mode\n");
                UnHighLightIt();

                ShapeItem shape = select.selectedObject;

                if (shape.Geometry.GetType().Name == "PathGeometry")
                {

                    Debug.WriteLine("Changing dimenstion of curve\n");
                    DimensionChangeCurve(a, shape);
                }
                else
                {

                    Debug.WriteLine("Changing dimenstion of shape\n");
                    DimensionChangingShape(a, shape);
                }
            }
            else if (mode == "translate_mode")
            {

                Debug.WriteLine("In translate mode\n");
                UnHighLightIt();

                ShapeItem shape = select.selectedObject;
                Rect boundingBox = shape.Geometry.Bounds;
                Debug.WriteLine(" tranlsating line " + shape.AnchorPoint.ToString() + "   start : " + shape.Start.ToString());
                double bx = shape.AnchorPoint.X + (a.X - select.initialSelectionPoint.X);
                double by = shape.AnchorPoint.Y + (a.Y - select.initialSelectionPoint.Y);
                double width = boundingBox.Width;
                double height = boundingBox.Height;
                Point p1 = new (bx, by);
                Point p2 = new (bx + width, by + height);

                if (shape.Geometry.GetType().Name == "PathGeometry")
                {

                    Debug.WriteLine("Translating curve\n");
                    TranslatingCurve(shape, bx, by, p1);
                }
                else if (shape.Geometry.GetType().Name == "LineGeometry")
                {

                    Debug.WriteLine("Translating line\n");
                    TranslatingLine(boundingBox, shape, p1, p2, width, height);

                }
                else
                {

                    Debug.WriteLine("Translating curve\n");
                    TranslatingShape(shape, p1, p2);
                }
            }
            else if (mode == "create_rectangle")
            {

                Debug.WriteLine("In create rectangle mode\n");

                if (lastShape != null)
                {
                    Point _AnchorPoint = lastShape.AnchorPoint;
                    lastShape = UpdateShape(_AnchorPoint, a, "RectangleGeometry", lastShape);
                }
            }
            else if (mode == "create_ellipse")
            {

                Debug.WriteLine("In create ellipse mode\n");

                if (lastShape != null)
                {

                    Point _AnchorPoint = lastShape.AnchorPoint;
                    lastShape = UpdateShape(_AnchorPoint, a, "EllipseGeometry", lastShape);
                }
            }
            else if (mode == "create_freehand")
            {
                Debug.WriteLine("In create curve mode\n");

                if (lastShape != null)
                {

                    Point _anchorPoint = lastShape.AnchorPoint;
                    lastShape = UpdateCurve(a, _anchorPoint);
                }
            }
            else if (mode == "create_line")
            {
                if (lastShape != null)
                {
                    Point _anchorPoint = lastShape.AnchorPoint;
                    //UpdateShape(_anchorPoint, a, "LineGeometry", lastShape);
                    UpdateShape(lastShape.Start, a, "LineGeometry", lastShape);
                }

            }
            else
            {
                //Debug.WriteLine("In unknown mode\n");
            }

            //Debug.WriteLine("Exiting Shape Building......\n");
        }
    }
}
