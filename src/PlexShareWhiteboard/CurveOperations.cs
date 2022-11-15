using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using PlexShareWhiteboard.BoardComponents;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace PlexShareWhiteboard
{
    partial class WhiteBoardViewModel
    {
        public ShapeItem CreateCurve(Point a)
        {
            var geometry1 = new PathGeometry();
            currentShape = new ShapeItem
            {
                Geometry = geometry1,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
                AnchorPoint = a,
                StrokeThickness = strokeThickness,
                Id = currentId,
                PointList = new List<Point>()

            };
            currentShape.PointList.Add(a);
            return currentShape;
        }

        public static void AddToPathGeometry(PathGeometry g1, Point currentPoint, Point previousPoint)
        {
            var line = new LineGeometry(currentPoint, previousPoint);
            Geometry geometry = new EllipseGeometry(currentPoint, 0.1, 0.1);
            g1.AddGeometry(geometry);
            g1.AddGeometry(line);

        }

        public ShapeItem UpdateCurve(Point a, Point _anchorPoint)
        {
            PathGeometry g1 = (PathGeometry)lastShape.Geometry;

            //var line = new LineGeometry(a, _anchorPoint);
            //Geometry geometry = new EllipseGeometry(a, 0.1, 0.1);

            //g1.AddGeometry(geometry);
            //g1.AddGeometry(line);

            AddToPathGeometry(g1, a, _anchorPoint);



            ShapeItem newShape = new()
            {
                Geometry = g1,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
                StrokeThickness= strokeThickness,
                AnchorPoint = a,
                Id = lastShape.Id,
                PointList = lastShape.PointList
            };

            int i;
            for (i = 0; i < ShapeItems.Count; i++)
            {
                if (ShapeItems[i].Id == lastShape.Id)
                {
                    ShapeItems[i] = newShape;
                    newShape.PointList.Add(a);
                    ShapeItems[i].PointList.Add(a);
                }
            }
            return newShape;
        }
        public void TransformCurve(Point a, ShapeItem shape)
        {

            ShapeItem x = shape;
            PathGeometry g1 = new();

            select.finalPointList.Clear();
            // Y fixing
            double extraY = a.Y - select.initialSelectionPoint.Y;
            double height = select.selectedObject.Geometry.Bounds.Height;
            double newHeight = height - extraY;
            if (select.selectBox == 3 || select.selectBox == 4)
                newHeight = height + extraY;
            double scaleY = newHeight / height;
            double rectTopY = select.selectedObject.Geometry.Bounds.Y;
            double rectBottomY = select.selectedObject.Geometry.Bounds.Y + select.selectedObject.Geometry.Bounds.Height;

            if (select.selectBox == 3 || select.selectBox == 4)
            {
                for (int i = 0; i < x.PointList.Count; i++)
                {
                    Point curPoint = x.PointList[i];
                    curPoint.Y = rectTopY + (curPoint.Y - rectTopY) * scaleY;
                    select.finalPointList.Add(curPoint);
                }

            }
            else
            {
                for (int i = 0; i < x.PointList.Count; i++)
                {
                    Point curPoint = x.PointList[i];
                    curPoint.Y = rectBottomY + (curPoint.Y - rectBottomY) * scaleY;
                    select.finalPointList.Add(curPoint);
                }
            }

            double b1 = select.selectedObject.Geometry.Bounds.Height;
            double a1 = select.selectedObject.Geometry.Bounds.Width;
            double newWidth = newHeight * (a1 / b1);

            double extraX = a.X - select.initialSelectionPoint.X;
            double width = select.selectedObject.Geometry.Bounds.Width;
            
            double scaleX = newWidth / width;
            double rectTopX = select.selectedObject.Geometry.Bounds.X;
            double rectBottomX = select.selectedObject.Geometry.Bounds.X + select.selectedObject.Geometry.Bounds.Width;

            if (select.selectBox == 2 || select.selectBox == 4)
            {
                for (int i = 0; i < select.finalPointList.Count; i++)
                {
                    Point curPoint = select.finalPointList[i];
                    curPoint.X = rectTopX + (curPoint.X - rectTopX) * scaleX;
                    select.finalPointList[i] = curPoint;
                }

            }
            else
            {
                for (int i = 0; i < select.finalPointList.Count; i++)
                {
                    Point curPoint = select.finalPointList[i];
                    curPoint.X = rectBottomX + (curPoint.X - rectBottomX) * scaleX;
                    select.finalPointList[i] = curPoint;
                }
            }


            // adding to geometry
            for (int i = 1; i < select.finalPointList.Count; i++)
            {
                Point curPoint = select.finalPointList[i];
                Point prevPoint = select.finalPointList[i - 1];
                //var line = new LineGeometry(curPoint, prevPoint);
                //g1.AddGeometry(line);

                AddToPathGeometry(g1, curPoint, prevPoint);

            }


            ShapeItem newShape = new()
            {
                Geometry = g1,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
                StrokeThickness = strokeThickness,
                AnchorPoint = a,
                Id = shape.Id,
                PointList = x.PointList
            };

            for (int i = 0; i < ShapeItems.Count; i++)
            {
                if (ShapeItems[i].Id == shape.Id)
                {
                    ShapeItems[i] = newShape;
                }
            }
            lastShape = newShape;
            //HighLightIt(shape.Geometry.Bounds);
        }

        public void DimensionChangeCurve(Point a, ShapeItem shape)
        {
            Debug.WriteLine(select.selectBox + " bounce bounce bounce yippe yipee ");
            if (select.selectBox == 6 || select.selectBox == 7)
            {
                // horitzontal
                ShapeItem x = shape;
                PathGeometry g1 = new();

                select.finalPointList.Clear();

                // X fixing
                double extraX = a.X - select.initialSelectionPoint.X;
                double width = select.selectedObject.Geometry.Bounds.Width;
                double newWidth = width + extraX;
                if (select.selectBox == 6)
                    newWidth = width - extraX;
                double scaleX = newWidth / width;
                double rectTopX = select.selectedObject.Geometry.Bounds.X;
                double rectBottomX = select.selectedObject.Geometry.Bounds.X + select.selectedObject.Geometry.Bounds.Width;

                if (select.selectBox == 7)
                {
                    for (int i = 0; i < select.selectedObject.PointList.Count; i++)
                    {
                        Point curPoint = select.selectedObject.PointList[i];
                        curPoint.X = rectTopX + (curPoint.X - rectTopX) * scaleX;
                        select.finalPointList.Add(curPoint);
                    }

                }
                else
                {
                    for (int i = 0; i < select.selectedObject.PointList.Count; i++)
                    {
                        Point curPoint = select.selectedObject.PointList[i];
                        curPoint.X = rectBottomX + (curPoint.X - rectBottomX) * scaleX;
                        select.finalPointList.Add(curPoint);
                    }
                }


                // adding to geometry
                for (int i = 1; i < select.finalPointList.Count; i++)
                {
                    Point curPoint = select.finalPointList[i];
                    Point prevPoint = select.finalPointList[i - 1];
                    //var line = new LineGeometry(curPoint, prevPoint);
                    //g1.AddGeometry(line);
                    AddToPathGeometry(g1, curPoint, prevPoint);

                }

                ShapeItem newShape = new()
                {
                    Geometry = g1,
                    Fill = fillBrush,
                    Stroke = strokeBrush,
                    ZIndex = currentZIndex,
                    StrokeThickness = strokeThickness,
                    AnchorPoint = a,
                    Id = shape.Id,
                    PointList = x.PointList
                };

                for (int i = 0; i < ShapeItems.Count; i++)
                {
                    if (ShapeItems[i].Id == shape.Id)
                    {
                        ShapeItems[i] = newShape;
                    }
                }

            }
            else if (select.selectBox == 5 || select.selectBox == 8)
            {

                ShapeItem x = shape;
                PathGeometry g1 = new PathGeometry();

                select.finalPointList.Clear();

                // Y fixing
                double extraY = a.Y - select.initialSelectionPoint.Y;
                double height = select.selectedObject.Geometry.Bounds.Height;
                double newHeight = height - extraY;
                if (select.selectBox == 8)
                    newHeight = height + extraY;
                double scaleY = newHeight / height;
                double rectTopY = select.selectedObject.Geometry.Bounds.Y;
                double rectBottomY = select.selectedObject.Geometry.Bounds.Y + select.selectedObject.Geometry.Bounds.Height;

                if (select.selectBox == 8)
                {
                    for (int i = 0; i < x.PointList.Count; i++)
                    {
                        Point curPoint = x.PointList[i];
                        curPoint.Y = rectTopY + (curPoint.Y - rectTopY) * scaleY;
                        select.finalPointList.Add(curPoint);
                    }

                }
                else
                {
                    for (int i = 0; i < x.PointList.Count; i++)
                    {
                        Point curPoint = x.PointList[i];
                        curPoint.Y = rectBottomY + (curPoint.Y - rectBottomY) * scaleY;
                        select.finalPointList.Add(curPoint);
                    }
                }


                // adding to geometry
                for (int i = 1; i < select.finalPointList.Count; i++)
                {
                    Point curPoint = select.finalPointList[i];
                    Point prevPoint = select.finalPointList[i - 1];
                    //var line = new LineGeometry(curPoint, prevPoint);
                    //g1.AddGeometry(line);

                    AddToPathGeometry(g1, curPoint, prevPoint);

                }

                ShapeItem newShape = new()
                {
                    Geometry = g1,
                    Fill = fillBrush,
                    Stroke = strokeBrush,
                    ZIndex = currentZIndex,
                    StrokeThickness = strokeThickness,
                    AnchorPoint = a,
                    Id = shape.Id,
                    PointList = x.PointList
                };

                for (int i = 0; i < ShapeItems.Count; i++)
                {
                    if (ShapeItems[i].Id == shape.Id)
                    {
                        ShapeItems[i] = newShape;
                    }
                }
                lastShape = newShape;
                //HighLightIt(shape.Geometry.Bounds);


            }
        }

        public void TranslatingCurve(ShapeItem shape, double bx, double by, Point p1)
        {
            Point prevPoint = shape.PointList[0];
            var g1 = new PathGeometry();
            List<Point> lis = new();
            for (int i = 0; i < shape.PointList.Count; ++i)
            {
                Point p = shape.PointList[i];
               
                double newx = bx - shape.AnchorPoint.X + p.X;
                double newy = by - shape.AnchorPoint.Y + p.Y;
                Point newPoint = new (newx, newy);

                lis.Add(newPoint);
                if (i != 0)
                {
                    //var line = new LineGeometry(newPoint, prevPoint);
                    //g1.AddGeometry(line);

                    AddToPathGeometry(g1, newPoint, prevPoint);
                }
                prevPoint = newPoint;
            }

            ShapeItem newShape = new()
            {
                Geometry = g1,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
                StrokeThickness = strokeThickness,
                AnchorPoint = p1,
                Id = shape.Id,
                PointList = lis
            };

            for (int i = 0; i < ShapeItems.Count; i++)
            {
                if (ShapeItems[i].Id == shape.Id)
                {
                    ShapeItems[i] = newShape;
                }
            }
            lastShape = newShape;
            HighLightIt(newShape.Geometry.Bounds);
        }

        public void FinishingCurve()
        {
            PathGeometry g1 = new();
            for (int i = 1; i < select.finalPointList.Count; i++)
            {
                Point curPoint = select.finalPointList[i];
                Point prevPoint = select.finalPointList[i - 1];
                //var line = new LineGeometry(curPoint, prevPoint);
                //g1.AddGeometry(line);

                //Geometry geometry = new EllipseGeometry(curPoint, 0.1, 0.1);

                //g1.AddGeometry(geometry);

                AddToPathGeometry(g1, curPoint, prevPoint);
            }

            List<Point> newPointList = new();

            foreach (Point p in select.finalPointList)
                newPointList.Add(p);


            ShapeItem updatingShape = new()
            {
                Geometry = g1,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
                StrokeThickness = strokeThickness,
                Id = select.selectedObject.Id,
                PointList = newPointList
            };



            for (int i = 0; i < ShapeItems.Count; i++)
            {
                if (ShapeItems[i].Id == select.selectedObject.Id)
                {
                    ShapeItems[i] = updatingShape;
                }
            }

        }
    }
}
