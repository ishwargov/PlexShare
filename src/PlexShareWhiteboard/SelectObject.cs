using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace PlexShareWhiteboard
{
    public class SelectObject
    {
        public bool IfSelected;
        public ShapeItem SelectedObject;
        public Point InitialSelectionPoint;
        public int SelectBox;
        public List<Point> FinalPointList;

        public SelectObject()
        {
            IfSelected = false;
            SelectedObject = null;
            SelectBox = -1;
            FinalPointList = new List<Point>();
        }
    }

    public partial class WhiteBoardViewModel
    {
        public static bool PointInsideRect(Rect shape, Point click)
        {
            if (click.X > shape.X && click.X < shape.X + shape.Width &&
                click.Y > shape.Y && click.Y < shape.Y + shape.Height)
                return true;
            
            return false;
        }

        public static int PointInsideHighlightBox(Rect shape, Point click, double halfSize)
        {
            double left = shape.X;
            double top = shape.Y;
            double width = shape.Width;
            double height = shape.Height;

            Rect topLeft = new Rect(left - halfSize, top - halfSize, 2 * halfSize, 2 * halfSize);
            Rect topRight = new Rect(left - halfSize + width, top - halfSize, 2 * halfSize, 2 * halfSize);
            Rect bottomLeft = new Rect(left - halfSize, top - halfSize + height, 2 * halfSize, 2 * halfSize);
            Rect bottomRight = new Rect(left - halfSize + width, top - halfSize + height, 2 * halfSize, 2 * halfSize);

            Rect topCentre = new Rect(left - halfSize + width / 2, top - halfSize, 2 * halfSize, 2 * halfSize);
            Rect leftCentre = new Rect(left - halfSize, top - halfSize + height / 2, 2 * halfSize, 2 * halfSize);
            Rect rightCentre = new Rect(left - halfSize + width, top - halfSize + height / 2, 2 * halfSize, 2 * halfSize);
            Rect bottomCentre = new Rect(left - halfSize + width / 2, top - halfSize + height, 2 * halfSize, 2 * halfSize);

            if (PointInsideRect(topLeft, click))
                return 1;
            else if(PointInsideRect(topRight, click))
                return 2;
            else if(PointInsideRect(bottomLeft, click)) 
                return 3;
            else if(PointInsideRect(bottomRight, click))
                return 4;
            else if(PointInsideRect(topCentre, click))
                return 5;
            else if(PointInsideRect(leftCentre, click))
                return 6;
            else if(PointInsideRect(rightCentre, click))
                return 7;
            else if(PointInsideRect(bottomCentre, click))
                return 8;
         
            return -1;
        }

        public static bool HelperSelect(Rect boundingBox, Point click, double halfSize)
        {
            if (PointInsideRect(boundingBox, click) || PointInsideHighlightBox(boundingBox, click, halfSize) > 0)
                return true;

            return false;
        }

        public void ObjectSelection(Point a)
        {
            int tempZIndex = -10000;
            Rect boundingBox = new(1, 1, 1, 1);

            for (int i = ShapeItems.Count - 1; i >= 0; i--)
            {
                if (ShapeItems[i].ZIndex < tempZIndex)
                    continue;

                Geometry Child = ShapeItems[i].Geometry;
                boundingBox = Child.Bounds;

                if (HelperSelect(boundingBox, a, blobSize / 2))
                {
                    select.IfSelected = true;
                    select.SelectedObject = ShapeItems[i];
                    select.InitialSelectionPoint = a;
                    tempZIndex = ShapeItems[i].ZIndex;
                    select.SelectBox = 0;
                }
            }

            if (select.IfSelected == true)
            {
                Debug.WriteLine("object selected\n");
                HighLightIt(select.SelectedObject.Geometry.Bounds);
                int boxNumber = PointInsideHighlightBox(boundingBox, a, blobSize / 2);

                if (boxNumber > 4)
                {
                    Debug.WriteLine("Going to enter dimensionChange_mode \n");
                    mode = "dimensionChange_mode";
                    select.SelectBox = boxNumber;
                }
                else if (boxNumber > 0)
                {
                    Debug.WriteLine("Going to enter transform mode \n");
                    mode = "transform_mode";
                    select.SelectBox = boxNumber;
                }
                else if (select.SelectBox == 0)
                {
                    Debug.Write("Going to enter translate_mode \n");
                    mode = "translate_mode";
                }

            }
        }

    }


}
