using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        // this is mouse up -> typically mouse release
        public void ShapeFinished(Point _)
        {
            Debug.WriteLine("Entering Shape Finished..............\n");
            if (mode == "create_rectangle" || mode == "create_ellipse" || mode == "create_freehand")
            {
                stackElement = new UndoStackElement(lastShape, lastShape, Operation.Creation);
                InsertIntoStack(stackElement);
            }
            else if (mode == "delete_mode")
            {
                stackElement = new UndoStackElement(lastShape, lastShape, Operation.Deletion);
                InsertIntoStack(stackElement);
            }
            else if (mode == "translate_mode" || mode == "transform_mode" || mode == "dimensionChange_mode")
            {
                stackElement = new UndoStackElement(select.initialSelectionObject, select.selectedObject, Operation.ModifyShape);
                InsertIntoStack(stackElement);
            }
            else;


            if ((mode == "transform_mode" || mode == "dimensionChange_mode") && 
                select.selectedObject != null && select.finalPointList.Count > 0 && 
                select.selectedObject.Geometry.GetType().Name == "PathGeometry")
            {
                FinishingCurve();
            }
            

            if (mode == "transform_mode" || mode == "dimensionChange_mode" || mode == "translate_mode")
                mode = "select_mode";
            /*
            if (mode == "create_rectangle" || mode == "create_ellipse" || mode == "create_freehand")
                undoredo.OnShapeReceiveFromVM(lastShape, lastShape, Operation.Creation);
            if (mode == "delete_mode")
                undoredo.OnShapeReceiveFromVM(lastShape,lastShape, Operation.Deletion);*/
            /*if (mode == "transform_mode" || mode == "translate_mode")*/

            lastShape = null;

            Debug.WriteLine("Exiting Shape Finished........");
        }
    }
}
