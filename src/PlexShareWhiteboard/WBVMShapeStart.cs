using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        public ShapeItem CreateShape(Point start, Point end, string name, String id)
        {
            Rect boundingBox = new Rect(start, end);
            Geometry geometry;
            geometry = new RectangleGeometry(boundingBox);

            if (name == "ellipse")
                geometry = new EllipseGeometry(boundingBox);

            ShapeItem newShape = new ShapeItem
            {
                Geometry = geometry,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = curZIndex,
                AnchorPoint = start,
                Id = id,
                StrokeThickness = 10,
            };

            for (int i = 0; i < ShapeItems.Count; i++)
            {
                if (ShapeItems[i].Id == id)
                {
                    ShapeItems[i] = newShape;
                }
            }

            return newShape;
        }

        public void ShapeStart(Point a)
        {
            Debug.WriteLine("Shape Start");
            UnHighLightIt();

            if (mode == "view_mode")
            {
                Debug.WriteLine("In View mode");

            }
            else if (mode == "delete_mode")
            {
                Debug.WriteLine("In Delete mode");

                int tempZIndex = -1;
                ShapeItem toDelete = null;
                for (int i = ShapeItems.Count - 1; i >= 0; i--)
                {
                    Geometry Child = ShapeItems[i].Geometry;
                    //var outline = Child.GetOutlinedPathGeometry();  
                    if (ShapeItems[i].ZIndex > tempZIndex && Child.FillContains(a))
                    {
                        tempZIndex = ShapeItems[i].ZIndex;
                        toDelete = ShapeItems[i];
                    }
                }

                if (toDelete != null)
                    ShapeItems.Remove(toDelete);
            }

            else if (mode == "select_mode")
            {
                Debug.WriteLine("In Select mode !!!!!!!!!!!!!!!!!!!!!");
                select.ifSelected = false;
                int tempZIndex = -1;
                Rect boundingBox = new Rect(1, 1, 1, 1);
                for (int i = ShapeItems.Count - 1; i >= 0; i--)
                {
                    if (ShapeItems[i].ZIndex < tempZIndex)
                        continue;
                    Geometry Child = ShapeItems[i].Geometry;
                    boundingBox = Child.Bounds;

                    //Child.FillContains(a)
                    if (a.X > boundingBox.X && a.X < boundingBox.X + boundingBox.Width &&
                        a.Y > boundingBox.Y && a.Y < boundingBox.Y + boundingBox.Height)
                    {
                        select.ifSelected = true;
                        select.selectedObject = ShapeItems[i];
                        select.InitialSelectionPoint = a;
                        tempZIndex = ShapeItems[i].ZIndex;
                    }
                }

                if (select.ifSelected == true)
                {
                    HighLightIt(boundingBox);
                    if (HighLightCornerMouse(a) == true)
                    {
                        Debug.Write("In scale_mode");
                        mode = "scale_mode";
                    }
                    else if (HighLightSideMouse(a) == true)
                    {
                        Debug.Write("In dimensionChange_mode ");
                        mode = "dimensionChange_mode";

                    }
                    else
                    {
                        Debug.Write("In translate_mode ");
                        mode = "translate_mode";
                    }

                    Debug.WriteLine("modee " + mode);
                }


            }
            else if (mode == "create_rectangle")
            {
                Debug.WriteLine("In create rectangle mode");

                ShapeItem curShape = CreateShape(a, a, "rectangle", curId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                curZIndex++;
            }
            else if (mode == "create_ellipse")
            {
                Debug.WriteLine("In create ellipse mode");

                ShapeItem curShape = CreateShape(a, a, "ellipse", curId);
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                curZIndex++;
            }
            else if (mode == "create_freehand")
            {
                Debug.WriteLine("In create freehand mode");

                var geometry1 = new PathGeometry();
                currentShape = new ShapeItem
                {
                    Geometry = geometry1,
                    Fill = fillBrush,
                    Stroke = strokeBrush,
                    ZIndex = curZIndex,
                    AnchorPoint = new Point(a.X, a.Y),
                    Id = curId,
                    StrokeThickness = 2
                };
                IncrementId();
                lastShape = currentShape;
                ShapeItems.Add(currentShape);
                Debug.WriteLine("Started the free hand");
                curZIndex++;
            }
            else
            {
                Debug.WriteLine("In Unknown Mode");
            }
        }

    }
}
