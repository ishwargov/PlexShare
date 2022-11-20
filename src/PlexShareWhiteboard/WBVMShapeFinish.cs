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

            if ((mode == "transform_mode" || mode == "dimensionChange_mode") &&
                select.selectedObject != null && select.finalPointList.Count > 0 &&
                select.selectedObject.Geometry.GetType().Name == "PathGeometry")
            {
                FinishingCurve();
            }


            if (modeForUndo == "create" )
            {
                Trace.WriteLine("[Whiteboard]  " + "passing into undo stack " + lastShape.Geometry.GetType().Name);
                stackElement = new UndoStackElement(lastShape, lastShape, Operation.Creation);
                InsertIntoStack(stackElement);

                if (lastShape != null)
                    machine.OnShapeReceived(lastShape, Operation.Creation);

            }
            else if (modeForUndo == "delete" && lastShape != null)
            {

                Trace.WriteLine("[Whiteboard]  " + "passing into undo stack " + lastShape.Geometry.GetType().Name);
                stackElement = new UndoStackElement(lastShape, lastShape, Operation.Deletion);
                InsertIntoStack(stackElement);
                if (lastShape != null)
                    machine.OnShapeReceived(lastShape, Operation.Deletion);

            }
            /*else if (modeForUndo == "textbox_creation")
            {
                stackElement = new UndoStackElement(textBoxLastShape, textBoxLastShape, Operation.Creation);
                InsertIntoStack(stackElement);
                if (textBoxLastShape != null)
                    machine.OnShapeReceived(textBoxLastShape, Operation.Creation);
            }*/
            else if (modeForUndo == "textbox_translate" && textBoxLastShape != null)
            {
                Debug.WriteLine("entering translate undo.........");
                stackElement = new UndoStackElement(select.initialSelectionObject, textBoxLastShape, Operation.ModifyShape);
                InsertIntoStack(stackElement);
                if (textBoxLastShape != null)
                    machine.OnShapeReceived(textBoxLastShape, Operation.ModifyShape);
            }
            else if (modeForUndo == "modify" && lastShape != null)
            {
               // Trace.WriteLine("[Whiteboard]  " + "passing into undo stack " + lastShape.Geometry.GetType().Name);
              //  Trace.WriteLine("[Whiteboard]  " + " inital bounding box" + select.initialSelectionObject.Geometry.Bounds.ToString() + "  final bounding box " + lastShape.Geometry.Bounds.ToString());
                stackElement = new UndoStackElement(select.initialSelectionObject, lastShape, Operation.ModifyShape);
                InsertIntoStack(stackElement);

                if (lastShape != null)
                    machine.OnShapeReceived(lastShape, Operation.ModifyShape);

            }

            if (mode == "transform_mode" || mode == "dimensionChange_mode" || mode == "translate_mode")
                mode = "select_mode";

            lastShape = null;

            if (modeForUndo != "create_textbox")
                modeForUndo = "select";

            //Trace.WriteLine("[Whiteboard]  " + "Exiting Shape Finished........");
        }
    }
}
