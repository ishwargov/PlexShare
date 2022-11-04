using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using WpfApp1;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Documents;

namespace WpfApp1
{

    internal class WBViewModel
    {


        public ObservableCollection<ShapeItem> ShapeItems { get; set; }
        Brush Fill_Brush= Brushes.Azure;
        String curId = "u0_f0";
        int curId_val = 0;
        int user_id = 0;
        Brush Stroke_Brush= Brushes.Black;
        string mode = "create_rectangle";
        int curZindex = 0;
        ShapeItem curShape = null;
        ShapeItem lastShape;
        public WBViewModel()
        {
            ShapeItems = new ObservableCollection<ShapeItem>();
        }

        public void IncrementId()
        {
            curId_val++;
            curId = "u" + user_id + "_f" + curId_val;
        }
        public void ShapeStart(Point a)
        {

            Debug.WriteLine("Shape Start");

            if (mode== "view_mode")
            {
                ;
            }

            else if (mode == "delete_mode")
            {
                ;
            }
            else if(mode=="create_rectangle")
            {

                curShape = new ShapeItem
                {
                    Geometry = new RectangleGeometry(new Rect(a.X, a.Y, 1 , 1)),
                    Fill = Fill_Brush,
                    Stroke = Stroke_Brush,
                    ZIndex = curZindex,
                    anchorPoint = new Point(a.X, a.Y),
                    Id = curId
                };
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                curZindex++;
            }
            else if (mode == "create_ellipse")
            {

                curShape = new ShapeItem
                {
                    Geometry = new EllipseGeometry(new Point(a.X, a.Y), 1, 1),
                    Fill = Fill_Brush,
                    Stroke = Stroke_Brush,
                    ZIndex = curZindex,
                    anchorPoint = new Point(a.X, a.Y),
                    Id = curId

                };
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                curZindex++;
            }
            else if(mode == "create_freehand")
            {

                var geometry1 = new PathGeometry(new[] { new PathFigure(a, 
                    new[] {new LineSegment(a, true)}, true) });

                curShape = new ShapeItem
                {
                    Geometry = geometry1,
                    Fill = Fill_Brush,
                    Stroke = Stroke_Brush,
                    ZIndex = curZindex,
                    anchorPoint = new Point(a.X, a.Y),
                    Id = curId

                };
                IncrementId();
                lastShape = curShape;
                ShapeItems.Add(curShape);
                curZindex++;
            }
            

        }

        // this is movemove
        public void ShapeBuilding(Point a)
        {
            Debug.WriteLine("Shape Building ");

            if (mode == "view_mode")
            {
                ;
            }

            else if (mode == "delete_mode")
            {
                ;
            }
            else if (mode == "create_rectangle")
            {

                if (lastShape != null)
                {
                    Point _anchorPoint = lastShape.anchorPoint;
                    double nw = Math.Max(0, -_anchorPoint.X + a.X);
                    double nh = Math.Max(0, -_anchorPoint.Y + a.Y);
                    ShapeItem newShape = new ShapeItem
                    {
                        Geometry = new RectangleGeometry(new Rect(_anchorPoint, a)),
                        Fill = Fill_Brush,
                        Stroke = Stroke_Brush,
                        ZIndex = curZindex,
                        anchorPoint = _anchorPoint,
                        Id = lastShape.Id
                    };

                    for (int i = 0; i < ShapeItems.Count; i++)
                    {
                        if (ShapeItems[i].Id == lastShape.Id)
                            ShapeItems[i] = newShape;
                    }
                }
            }
            else if (mode == "create_ellipse")
            {
                if (lastShape != null)
                {
                    Point _anchorPoint = lastShape.anchorPoint;
                    double nw = Math.Max(0, -_anchorPoint.X + a.X);
                    double nh = Math.Max(0, -_anchorPoint.Y + a.Y);
                    ShapeItem newShape = new ShapeItem
                    {
                        Geometry = new EllipseGeometry(new Rect(_anchorPoint, a)),
                        Fill = Fill_Brush,
                        Stroke = Stroke_Brush,
                        ZIndex = curZindex,
                        anchorPoint = _anchorPoint,
                        Id = lastShape.Id
                    };


                    for (int i = 0; i < ShapeItems.Count; i++)
                    {
                        if (ShapeItems[i].Id == lastShape.Id)
                            ShapeItems[i] = newShape;
                    }
                }
            }
            else if (mode == "create_freehand")
            {
                if (lastShape != null)
                {
                    Point _anchorPoint = lastShape.anchorPoint;
                    PathGeometry g1 = (PathGeometry)lastShape.Geometry;
                    PathFigureCollection fig = g1.Figures;
                    fig.Add(new PathFigure(a,new[] { new LineSegment(new Point(0.01,0.01), true) }, true));
                    g1.Figures= fig;

                    ShapeItem newShape = new ShapeItem
                    {
                        Geometry = g1,
                        Fill = Fill_Brush,
                        Stroke = Stroke_Brush,
                        ZIndex = curZindex,
                        anchorPoint = _anchorPoint,
                        Id = lastShape.Id
                    };


                    for (int i = 0; i < ShapeItems.Count; i++)
                    {
                        if (ShapeItems[i].Id == lastShape.Id)
                            ShapeItems[i] = newShape;
                    }
                }

            }
        }

        // this is moouse up
        public void ShapeFinished(Point a)
        {
            Debug.WriteLine("Shape Finished");
            lastShape = null;
        }

        public void changeMode(string new_mode)
        {
            mode = new_mode;
        }


        internal void changeFillBrush(SolidColorBrush br)
        {
            Fill_Brush = br;
        }
        internal void increaseZIndex()
        {
            curZindex++;
        }
        internal void decreaseZIndex()
        {
            curZindex--;
        }
    }
}

