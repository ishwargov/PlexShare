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

            Debug.WriteLine("Entering Shape Start......\n");
            UnHighLightIt();

            if (mode == "delete_mode")
            {

                Debug.WriteLine("In Delete mode\n");
                modeForUndo = "delete";
                DeleteShape(a);
            }
            else if (mode == "select_mode")
            {

                //Debug.WriteLine("In select mode\n");

                select.ifSelected = false;
                ObjectSelection(a);
            }
            else if (mode == "create_rectangle")
            {

                Debug.WriteLine("In create rectangle mode\n");

                ShapeItem curShape = CreateShape(a, a, "RectangleGeometry", currentId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                IncreaseZIndex();
            }
            else if (mode == "create_ellipse")
            {

                Debug.WriteLine("In create ellipse mode\n");

                ShapeItem curShape = CreateShape(a, a, "EllipseGeometry", currentId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                IncreaseZIndex();
            }
            else if (mode == "create_freehand")
            {

                Debug.WriteLine("In create freehand mode\n");

                currentShape = CreateCurve(a);
                IncrementId();
                lastShape = currentShape;
                ShapeItems.Add(currentShape);
                IncreaseZIndex();
            }
            else if (mode == "create_line")
            {

                //Debug.WriteLine("In create line mode\n");

                ShapeItem curShape = CreateShape(a, a, "LineGeometry", currentId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                currentZIndex++;
            }
            else if (mode == "create_textbox")
            {

                Debug.WriteLine("In create text box mode\n");

                text = "";
                textBoxPoint = a;
                ShapeItem curShape = CreateShape(textBoxPoint, textBoxPoint, "GeometryGroup", currentId, "");
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                currentZIndex++;
            }
            else
            {
                Debug.WriteLine("In Unknown Mode\n");
            }

            //Debug.WriteLine("Exiting Shape Start......\n");
        }
    }
}
