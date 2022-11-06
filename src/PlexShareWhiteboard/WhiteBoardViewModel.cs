using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PlexShareWhiteboard
{
    public class WhiteBoardViewModel
    {
        public ObservableCollection<ShapeItem> ShapeItems { get; set; }
        Brush fillBrush = Brushes.Azure;
        String curId = "u0_f0";
        int curIdVal = 0;
        int userId = 0;
        Brush strokeBrush = Brushes.Black;
        string mode = "create_rectangle";
        int curZIndex = 0;
        ShapeItem currentShape = null;
        ShapeItem lastShape;
        SelectObject select = new SelectObject();
        List<ShapeItem> highlightShapes;

        public WhiteBoardViewModel()
        {
            ShapeItems = new ObservableCollection<ShapeItem>();
            highlightShapes = new List<ShapeItem>();
        }

        public void IncrementId()
        {
            curIdVal++;
            curId = "u" + userId + "_f" + curIdVal;
        }

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


        public ShapeItem GenerateRectangleXYWidthHeight(double x, double y, double w, double h, Brush f1, Brush s1, String id, int zi)
        {
            ShapeItem shape = new ShapeItem
            {
                Geometry = new RectangleGeometry(new Rect(x, y, w, h)),
                Fill = f1,
                Stroke = s1,
                ZIndex = zi,
                Id = id
            };
            return shape;
        }

        public void HighLightIt(Rect rect)
        {
            double x = rect.X;
            double y = rect.Y;
            double height = rect.Height;
            double width = rect.Width;

            int blobSize = 10;

            ShapeItem hsBody = GenerateRectangleXYWidthHeight(x, y, width, height, null, Brushes.DodgerBlue, "hsBody", curZIndex);
            highlightShapes.Add(hsBody);

            ShapeItem hsTopCenter = GenerateRectangleXYWidthHeight(x + width / 2 - blobSize / 2, y - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsTopCenter", curZIndex);
            highlightShapes.Add(hsTopCenter);

            ShapeItem hsBottomCenter = GenerateRectangleXYWidthHeight(x + width / 2 - blobSize / 2, y + height - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsBottomCenter", curZIndex);
            highlightShapes.Add(hsBottomCenter);

            ShapeItem hsRightCenter = GenerateRectangleXYWidthHeight(x + width - blobSize / 2, y + height / 2 - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsRightCenter", curZIndex);
            highlightShapes.Add(hsRightCenter);

            ShapeItem hsLeftCenter = GenerateRectangleXYWidthHeight(x - blobSize / 2, y + height / 2 - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsLeftCenter", curZIndex);
            highlightShapes.Add(hsLeftCenter);

            ShapeItem hsTopLeft = GenerateRectangleXYWidthHeight(x - blobSize / 2, y - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsTopLeft", curZIndex);
            highlightShapes.Add(hsTopLeft);

            ShapeItem hsBottomLeft = GenerateRectangleXYWidthHeight(x - blobSize / 2, y + height - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsBottomLeft", curZIndex);
            highlightShapes.Add(hsBottomLeft);

            ShapeItem hsTopRight = GenerateRectangleXYWidthHeight(x + width - blobSize / 2, y - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsTopRight", curZIndex);
            highlightShapes.Add(hsTopRight);

            ShapeItem hsBottomRight = GenerateRectangleXYWidthHeight(x + width - blobSize / 2, y + height - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsBottomRight", curZIndex);
            highlightShapes.Add(hsBottomRight);


            // just adds to shapeitems
            foreach (ShapeItem si in highlightShapes)
                ShapeItems.Add(si);
        }
        public void HighLightIt(Point a, Point b)
        {
            Rect rect = new Rect(a, b);
            HighLightIt(rect);
        }

        public void UnHighLightIt()
        {
            // removes from shapeitems and clears it
            if (highlightShapes.Count == 0)
                return;

            foreach (ShapeItem x in highlightShapes)
                ShapeItems.Remove(x);

            highlightShapes.Clear();
        }

        public bool HighLightSideMouse(Point a)
        {
            if (highlightShapes.Count < 4)
                return false;
            for (int i = 1; i <= 4; i++)
            {
                Rect boundingBox = highlightShapes.ElementAt(i).Geometry.Bounds;
                if (a.X > boundingBox.X && a.X < boundingBox.X + boundingBox.Width &&
                        a.Y > boundingBox.Y && a.Y < boundingBox.Y + boundingBox.Height)
                    return true;
            }
            return false;
        }

        public bool HighLightCornerMouse(Point a)
        {
            if (highlightShapes.Count < 8)
                return false;

            for (int i = 5; i <= 8; i++)
            {
                Rect boundingBox = highlightShapes.ElementAt(i).Geometry.Bounds;
                if (a.X > boundingBox.X && a.X < boundingBox.X + boundingBox.Width &&
                        a.Y > boundingBox.Y && a.Y < boundingBox.Y + boundingBox.Height)
                    return true;
            }
            return false;
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


        public void ShapeBuilding(Point a)
        {
            //Debug.WriteLine("Shape Building ");
            
            if (mode == "view_mode")
            {
                ;
            }

            else if (mode == "delete_mode")
            {
                ;
            }
            else if (mode == "scale_mode")
            {
                ;
            }
            else if (mode == "dimensionChange_mode")
            {
                ;
            }
            else if (mode == "translate_mode")
            {

                UnHighLightIt();
                ShapeItem shape = select.selectedObject;
                Rect boundingBox = shape.Geometry.Bounds;
                double bx = select.selectedObject.AnchorPoint.X + (a.X - select.InitialSelectionPoint.X);
                double by = select.selectedObject.AnchorPoint.Y + (a.Y - select.InitialSelectionPoint.Y);
                double width = boundingBox.Width;
                double height = boundingBox.Height;
                Point p1 = new Point(bx, by);
                Point p2 = new Point(bx + width, by + height);

                if (select.selectedObject.Geometry.GetType() == (new RectangleGeometry()).GetType())
                    CreateShape(p1, p2, "rectangle", select.selectedObject.Id);
                else if (select.selectedObject.Geometry.GetType() == (new EllipseGeometry()).GetType())
                    CreateShape(p1, p2, "ellipse", select.selectedObject.Id);
                else if (select.selectedObject.Geometry.GetType() == (new PathGeometry()).GetType())
                {
                    Debug.WriteLine("translating geomteries " + ShapeItems.Count);
                    Geometry g = select.selectedObject.Geometry;
                    g.Transform = new TranslateTransform(100, 100);
                    ShapeItem newShape22 = new ShapeItem
                    {
                        Geometry = g,
                        Fill = fillBrush,
                        Stroke = strokeBrush,
                        ZIndex = curZIndex,
                        Id = select.selectedObject.Id
                    };

                    for (int i = 0; i < ShapeItems.Count; i++)
                    {
                        if (ShapeItems[i].Id == select.selectedObject.Id)
                            ShapeItems[i] = newShape22;
                    }
                    Debug.WriteLine(" Post      translating geomteries " + ShapeItems.Count);


                }
                HighLightIt(p1, p2);
            }
            else if (mode == "create_rectangle")
            {

                if (lastShape != null)
                {
                    Point _anchorPoint = lastShape.AnchorPoint;
                    CreateShape(_anchorPoint, a, "rectangle", lastShape.Id);
                }
            }
            else if (mode == "create_ellipse")
            {
                if (lastShape != null)
                {
                    Point _anchorPoint = lastShape.AnchorPoint;
                    CreateShape(_anchorPoint, a, "ellipse", lastShape.Id);
                }
            }
            else if (mode == "create_freehand")
            {
                if (lastShape != null)
                {

                    Point _anchorPoint = lastShape.AnchorPoint;
                    PathGeometry g1 = (PathGeometry)lastShape.Geometry;
                    
                    var line = new LineGeometry(a, _anchorPoint);
                    g1.AddGeometry(line);

                    ShapeItem newShape = new ShapeItem
                    {
                        Geometry = g1,
                        Fill = fillBrush,
                        Stroke = strokeBrush,
                        ZIndex = curZIndex,
                        AnchorPoint = a,
                        Id = lastShape.Id,
                        StrokeThickness = 5,
                    };
                    lastShape = newShape;


                    for (int i = 0; i < ShapeItems.Count; i++)
                    {
                        if (ShapeItems[i].Id == lastShape.Id)
                            ShapeItems[i] = newShape;
                    }
                }
            }
        }

        public void ShapeFinished(Point a)
        {
            Debug.Write("Shape Finished with Before mode: " + mode);
            lastShape = null;
            if (mode == "scale_mode" || mode == "dimensionChange_mode" || mode == "translate_mode")
                mode = "select_mode";
            Debug.WriteLine(" and After mode : " + mode);
        }

        public void ChangeMode(string new_mode)
        {
            mode = new_mode;
        }
        public void ChangeFillBrush(SolidColorBrush br)
        {
            fillBrush = br;
        }
        public void IncreaseZIndex()
        {
            curZIndex++;
        }
        public void DecreaseZIndex()
        {
            curZIndex--;
        }
    }
}
