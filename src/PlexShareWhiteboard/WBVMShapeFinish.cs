using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;
using System.Windows.Automation.Peers;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        // this is mouse up -> typically mouse release
        public void ShapeFinished(Point _)
        {
            //Debug.WriteLine("Entering Shape Finished..............\n");
            if (modeForUndo == "create" )
            {
                Debug.WriteLine("passing into undo stack " + lastShape.Geometry.GetType().Name);
                stackElement = new UndoStackElement(lastShape, lastShape, Operation.Creation);
                InsertIntoStack(stackElement);

                if (lastShape != null)
                    machine.OnShapeReceived(lastShape, Operation.Creation);

            }
            else if (modeForUndo == "delete" && lastShape != null)
            {

                Debug.WriteLine("passing into undo stack " + lastShape.Geometry.GetType().Name);
                stackElement = new UndoStackElement(lastShape, lastShape, Operation.Deletion);
                InsertIntoStack(stackElement);
                if (lastShape != null)
                    machine.OnShapeReceived(lastShape, Operation.Deletion);

            }
            else if (modeForUndo == "modify")
            {
                Debug.WriteLine("passing into undo stack " + lastShape.Geometry.GetType().Name);
                Debug.WriteLine(" inital bounding box" + select.initialSelectionObject.Geometry.Bounds.ToString() + "  final bounding box " + lastShape.Geometry.Bounds.ToString());
                stackElement = new UndoStackElement(select.initialSelectionObject, lastShape, Operation.ModifyShape);
                InsertIntoStack(stackElement);

                if (lastShape != null)
                    machine.OnShapeReceived(lastShape, Operation.ModifyShape);

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

            
            if (mode != "create_textbox")
                lastShape = null;

            modeForUndo = "select";

            //Debug.WriteLine("Exiting Shape Finished........");
        }
    }
}
