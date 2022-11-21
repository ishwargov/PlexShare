/********************************************************************************
 * Filename    = WBVMShapeBuilding.cs
 *
 * Author      = Asha Jose
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is part of View Model.
 *               This contains the Shape Build method which is called from 
 *               view by mouse move to reflect on view model. This is typically 
 *               called when a user drags teh mouse by pressing on it.
 ********************************************************************************/

using System.Diagnostics;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        /// <summary>
        /// This is called by mouse move
        /// Mouse mokve is called when mouse is pressed and dragged
        /// </summary>
        /// <param name="a"></param>
        public void ShapeBuilding(Point a)
        {
            if (mode == "transform_mode")
            {
                modeForUndo = "modify";
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
                    TransformCurve(a, shape);
                }
                else if (shape.Geometry.GetType().Name == "LineGeometry")
                {
                    TransformingLine(a, shape);
                }
                else
                {
                    TransformShape(shape, newXLen, newYLen, signX, signY);
                }
            }
            else if (mode == "dimensionChange_mode")
            {
                modeForUndo = "modify";
                UnHighLightIt();

                ShapeItem shape = select.selectedObject;

                if (shape.Geometry.GetType().Name == "PathGeometry")
                {

                    DimensionChangeCurve(a, shape);
                }
                else
                {

                    DimensionChangingShape(a, shape);
                }
            }
            else if (mode == "translate_mode")
            {
                if (select.selectedObject.Geometry.GetType().Name == "GeometryGroup")
                    modeForUndo = "textbox_translate";
                else
                    modeForUndo = "modify";
                UnHighLightIt();

                ShapeItem shape = select.selectedObject;
                Rect boundingBox = shape.Geometry.Bounds;

                Trace.WriteLine("[Whiteboard] : "+ " tranlsating line " + shape.AnchorPoint.ToString() + "   start : " + shape.Start.ToString());
                double bx = shape.AnchorPoint.X + (a.X - select.initialSelectionPoint.X);
                double by = shape.AnchorPoint.Y + (a.Y - select.initialSelectionPoint.Y);
                double width = boundingBox.Width;
                double height = boundingBox.Height;
                Point p1 = new (bx, by);
                Point p2 = new (bx + width, by + height);

                if (shape.Geometry.GetType().Name == "PathGeometry")
                {

                    TranslatingCurve(shape, bx, by, p1);
                }
                else if (shape.Geometry.GetType().Name == "LineGeometry")
                {

                    TranslatingLine(boundingBox, shape, p1, p2, width, height);

                }
                else
                {

                    TranslatingShape(shape, p1, p2);
                }
            }
            else if (mode == "create_rectangle")
            {

                if (lastShape != null)
                {
                    modeForUndo = "create";
                    Point _AnchorPoint = lastShape.AnchorPoint;
                    lastShape = UpdateShape(_AnchorPoint, a, "RectangleGeometry", lastShape);
                }
            }
            else if (mode == "create_ellipse")
            {

                if (lastShape != null)
                {
                    modeForUndo = "create";
                    Point _AnchorPoint = lastShape.AnchorPoint;
                    lastShape = UpdateShape(_AnchorPoint, a, "EllipseGeometry", lastShape);
                }
            }
            else if (mode == "create_freehand")
            {

                if (lastShape != null)
                {
                    modeForUndo = "create";
                    Point _anchorPoint = lastShape.AnchorPoint;
                    lastShape = UpdateCurve(a, _anchorPoint);
                }
            }
            else if (mode == "create_line")
            {

                if (lastShape != null)
                {
                    modeForUndo = "create";
                    Point _anchorPoint = lastShape.AnchorPoint;
                    //UpdateShape(_anchorPoint, a, "LineGeometry", lastShape);
                    //UpdateShape(lastShape.Start, a, "LineGeometry", lastShape);
                    lastShape =UpdateShape(lastShape.Start, a, "LineGeometry", lastShape);
                    // when we do lastshape == 
                }
            }
        }
    }
}
