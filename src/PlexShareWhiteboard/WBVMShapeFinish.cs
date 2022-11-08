using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        // this is mouse up -> typically mouse release
        public void ShapeFinished(Point _)
        {
            Debug.WriteLine("Entering Shape Finished..............\n");

            if ((mode == "transform_mode" || mode == "dimensionChange_mode") && 
                select.SelectedObject != null && select.FinalPointList.Count > 0 && 
                select.SelectedObject.Geometry.GetType().Name == "PathGeometry")
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
