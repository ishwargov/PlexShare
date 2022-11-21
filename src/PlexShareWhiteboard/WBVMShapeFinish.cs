/*******************************************************************************
 * Filename    = WBVMShapeFinish.cs
 *
 * Author      = Asha Jose
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is part of View Model.
 *               This contains the Shape Finish method which is called from 
 *               view by mouse up to reflect on view model. This is typically 
 *               called when a user release the mouse.
 ******************************************************************************/

using System;
using System.Diagnostics;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        /// <summary>
        /// This is called for mouse up
        /// Last process for shape creation
        /// // Mouss up is basically releasing the mouse
        /// </summary>
        /// <param name="_"></param>
        public void ShapeFinished(Point _)
        {
            Trace.WriteLine("[Whiteboard] : entering shape finished");
            // for free hand curves, in translation, transformation and dimension change mode, the point list needs to be updated
            // this is not done in shape building since we need the original points for reference
            if ((mode == "transform_mode" || mode == "dimensionChange_mode") &&
                select.selectedObject != null && select.finalPointList.Count > 0 &&
                select.selectedObject.Geometry.GetType().Name == "PathGeometry")
            {
                FinishingCurve();
            }

            // mode for undo regulates pushing into undo redo stack and seding to server
            if (modeForUndo == "create" )
            {
                // this is done to update z index value of the shapes drawn by the server
                // this is relevant because unlike the clients whose z index is updated during broadcast, server's z index isn't updated
                if(isServer)
                {
                    lastShape.ZIndex = Math.Max(lastShape.ZIndex, machine.GetMaxZindex(lastShape));
                    ModifyIncomingShape(lastShape);
                }

                Trace.WriteLine("[Whiteboard] : passing created object into undo stack and passing into server");
                stackElement = new UndoStackElement(lastShape, lastShape, Operation.Creation);
                InsertIntoStack(stackElement);

                if (lastShape != null)
                    machine.OnShapeReceived(lastShape, Operation.Creation);

            }
            else if (modeForUndo == "delete" && lastShape != null)
            {
                Trace.WriteLine("[Whiteboard] : passing object that is deleted into undo stack and passing into server");
                stackElement = new UndoStackElement(lastShape, lastShape, Operation.Deletion);
                InsertIntoStack(stackElement);

                if (lastShape != null)
                    machine.OnShapeReceived(lastShape, Operation.Deletion);

            }
            else if (modeForUndo == "textbox_translate" && textBoxLastShape != null)
            {
                
                stackElement = new UndoStackElement(select.initialSelectionObject, textBoxLastShape, Operation.ModifyShape);
                InsertIntoStack(stackElement);

                if (textBoxLastShape != null)
                    machine.OnShapeReceived(textBoxLastShape, Operation.ModifyShape);
            }
            else if (modeForUndo == "modify" && lastShape != null)
            {
                Trace.WriteLine("[Whiteboard] : passing object before and after modification to undo stack and passing into server");
                stackElement = new UndoStackElement(select.initialSelectionObject, lastShape, Operation.ModifyShape);
                InsertIntoStack(stackElement);

                if (lastShape != null)
                    machine.OnShapeReceived(lastShape, Operation.ModifyShape);

            }

            // translation, transformation and dimension change happens as a part of selection
            // so the mode is then set to select
            if (mode == "transform_mode" || mode == "dimensionChange_mode" || mode == "translate_mode")
                mode = "select_mode";

            lastShape = null;

            // modeForUndo is reset if it is not text box creation
            // if it is text box creation, then it is not reset to facilitate adding text box to undo stack and sending to server
            if (modeForUndo != "create_textbox")
                modeForUndo = "select";

            Trace.WriteLine("[Whiteboard] : Exiting Shape Finished");
        }
    }
}
