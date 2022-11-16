using System.Diagnostics;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;
using System.Xml.Linq;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        public void ShapeStart(Point a)
        {

            Trace.WriteLine("[Whiteboard] : Entering Shape Start");

            UnHighLightIt();

            if (mode == "delete_mode")
            {
                Trace.WriteLine("[Whiteboard] : In Delete mode");
                modeForUndo = "delete";
                DeleteShape(a);
            }
            else if (mode == "select_mode")
            {

                Trace.WriteLine("[Whiteboard] : In Select mode");
                select.ifSelected = false;
                ObjectSelection(a);
            }
            else if (mode == "create_rectangle")
            {

                Trace.WriteLine("[Whiteboard] : In create rectangle mode");
                ShapeItem curShape = CreateShape(a, a, "RectangleGeometry", currentId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                IncreaseZIndex();
            }
            else if (mode == "create_ellipse")
            {

                Trace.WriteLine("[Whiteboard] : In create ellipse mode");
                ShapeItem curShape = CreateShape(a, a, "EllipseGeometry", currentId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                IncreaseZIndex();
            }
            else if (mode == "create_freehand")
            {

                Trace.WriteLine("[Whiteboard] : In create freehand mode");
                currentShape = CreateCurve(a);
                IncrementId();
                lastShape = currentShape;
                ShapeItems.Add(currentShape);
                IncreaseZIndex();
            }
            else if (mode == "create_line")
            {

                //Trace.WriteLine("[Whiteboard]  " + "In create line mode\n");
                Trace.WriteLine("[Whiteboard] : In create line mode");
                ShapeItem curShape = CreateShape(a, a, "LineGeometry", currentId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                currentZIndex++;
            }
            else if (mode == "create_textbox")
            {
                
                if (modeForUndo == "create_textbox")
                {
                    Debug.WriteLine("mode for undo : ", textBoxLastShape.Id);
                    if (textBoxLastShape != null && textBoxLastShape.TextString.Length != 0)
                    {
                        
                        TextFinishPush();
                        Debug.WriteLine("entering undo modeeeee");

                    }
                    else if (textBoxLastShape != null)
                    {
                        for (int i = 0; i < ShapeItems.Count; ++i)
                        {
                            if (textBoxLastShape.Id == ShapeItems[i].Id)
                            {
                                ShapeItems.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

                modeForUndo = "create_textbox";
                Trace.WriteLine("[Whiteboard] : In create text box mode");
                textBoxPoint = a;
                ShapeItem curShape = CreateShape(textBoxPoint, textBoxPoint, "GeometryGroup", currentId, "");
                Debug.WriteLine("initial text box ", curShape.Id);
                IncrementId();
                textBoxLastShape = curShape.DeepClone();
                Trace.WriteLine("[Whiteboard]  " + "inside create text box " + textBoxLastShape);
                ShapeItems.Add(curShape);
                currentZIndex++;
            }
            else
            {
                Trace.WriteLine("[Whiteboard] : In Unknown Mode");
            }
        }
    }
}
