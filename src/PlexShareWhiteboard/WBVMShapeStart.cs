/***************************************************************************
 * Filename    = WBVMShapeStart.cs
 *
 * Author      = Deon Saji
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is part of View Model.
 *               This contains the Shape Start method which is called from 
 *               view by mouse down to reflect on view model. This is 
 *               typically called when a user clicks on the canvas.
 **************************************************************************/

using System.Diagnostics;
using System.Windows;
using System.Windows.Shapes;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        /// <summary>
        /// Shape start is called for mouse down event
        /// This is the first process for creating a shape
        /// mouse up event is basically mouse clicking on the canvas
        /// </summary>
        /// <param name="a"></param>
        public void ShapeStart(Point a)
        {
            Trace.WriteLine("[Whiteboard] : Entering Shape Start");
            UnHighLightIt();

            // deleting happens entirely in shape start and then goes to shape finished
            if (mode == "delete_mode")
            {
                Trace.WriteLine("[Whiteboard] : In Delete mode");
                modeForUndo = "delete";
                DeleteShape(a);
            }
            // selection happens entirely in shape start and then goes to shape finished
            else if (mode == "select_mode")
            {
                Trace.WriteLine("[Whiteboard] : In Select mode");
                select.ifSelected = false;
                ObjectSelection(a);
            }
            // clicking on it creates a rectangle and dragging around happens in shape building and then goes to shape finished
            else if (mode == "create_rectangle")
            {
                Trace.WriteLine("[Whiteboard] : In create rectangle mode");
                ShapeItem curShape = CreateShape(a, a, "RectangleGeometry", currentId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                IncreaseZIndex();
            }
            // clicking on it creates a ellipse and dragging around happens in shape building and then goes to shape finished
            else if (mode == "create_ellipse")
            {
                Trace.WriteLine("[Whiteboard] : In create ellipse mode");
                ShapeItem curShape = CreateShape(a, a, "EllipseGeometry", currentId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                IncreaseZIndex();
            }
            // clicking on it creates a freed hand curve and dragging around happens in shape building and then goes to shape finished
            else if (mode == "create_freehand")
            {
                Trace.WriteLine("[Whiteboard] : In create freehand mode");
                currentShape = CreateCurve(a);
                IncrementId();
                lastShape = currentShape;
                ShapeItems.Add(currentShape);
                IncreaseZIndex();
            }
            // clicking on it creates a line and dragging around happens in shape building and then goes to shape finished
            else if (mode == "create_line")
            {
                Trace.WriteLine("[Whiteboard] : In create line mode");
                ShapeItem curShape = CreateShape(a, a, "LineGeometry", currentId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                IncreaseZIndex();
            }
            // clicking on it creates a text box with an empty string and then goes to shape finished, after which key is pressed
            // pressing key triggers key down function which adds text to the text box
            else if (mode == "create_textbox")
            {
                TextBoxAddding(modeForUndo);

                Trace.WriteLine("[Whiteboard] : In create text box mode");
                modeForUndo = "create_textbox";
                textBoxPoint = a;
                ShapeItem curShape = CreateShape(textBoxPoint, textBoxPoint, "GeometryGroup", currentId, "");
                IncrementId();
                textBoxLastShape = curShape.DeepClone();
                ShapeItems.Add(curShape);
                IncreaseZIndex();
            }
            else
            {
                Trace.WriteLine("[Whiteboard] : In Unknown Mode");
            }

            Trace.WriteLine("[Whiteboard] : Exiting Shape Start");
        }
    }
}
