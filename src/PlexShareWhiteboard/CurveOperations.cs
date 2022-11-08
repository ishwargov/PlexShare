using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;

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
                Id = currentId,
                PointList = new List<Point>()

            };
            currentShape.PointList.Add(a);
            return currentShape;
        }

        public ShapeItem UpdateCurve(Point a, Point _anchorPoint)
        {
            PathGeometry g1 = (PathGeometry)lastShape.Geometry;

            var line = new LineGeometry(a, _anchorPoint);
            g1.AddGeometry(line);

            ShapeItem newShape = new()
            {
                Geometry = g1,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
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

            select.FinalPointList.Clear();
            // Y fixing
            double extraY = a.Y - select.InitialSelectionPoint.Y;
            double height = select.SelectedObject.Geometry.Bounds.Height;
            double newHeight = height - extraY;
            if (select.SelectBox == 3 || select.SelectBox == 4)
                newHeight = height + extraY;
            double scaleY = newHeight / height;
            double rectTopY = select.SelectedObject.Geometry.Bounds.Y;
            double rectBottomY = select.SelectedObject.Geometry.Bounds.Y + select.SelectedObject.Geometry.Bounds.Height;

            if (select.SelectBox == 3 || select.SelectBox == 4)
            {
                for (int i = 0; i < x.PointList.Count; i++)
                {
                    Point curPoint = x.PointList[i];
                    curPoint.Y = rectTopY + (curPoint.Y - rectTopY) * scaleY;
                    select.FinalPointList.Add(curPoint);
                }

            }
            else
            {
                for (int i = 0; i < x.PointList.Count; i++)
                {
                    Point curPoint = x.PointList[i];
                    curPoint.Y = rectBottomY + (curPoint.Y - rectBottomY) * scaleY;
                    select.FinalPointList.Add(curPoint);
                }
            }

            double b1 = select.SelectedObject.Geometry.Bounds.Height;
            double a1 = select.SelectedObject.Geometry.Bounds.Width;
            double newWidth = newHeight * (a1 / b1);

            double extraX = a.X - select.InitialSelectionPoint.X;
            double width = select.SelectedObject.Geometry.Bounds.Width;
            
            double scaleX = newWidth / width;
            double rectTopX = select.SelectedObject.Geometry.Bounds.X;
            double rectBottomX = select.SelectedObject.Geometry.Bounds.X + select.SelectedObject.Geometry.Bounds.Width;

            if (select.SelectBox == 2 || select.SelectBox == 4)
            {
                for (int i = 0; i < select.FinalPointList.Count; i++)
                {
                    Point curPoint = select.FinalPointList[i];
                    curPoint.X = rectTopX + (curPoint.X - rectTopX) * scaleX;
                    select.FinalPointList[i] = curPoint;
                }

            }
            else
            {
                for (int i = 0; i < select.FinalPointList.Count; i++)
                {
                    Point curPoint = select.FinalPointList[i];
                    curPoint.X = rectBottomX + (curPoint.X - rectBottomX) * scaleX;
                    select.FinalPointList[i] = curPoint;
                }
            }


            // adding to geometry
            for (int i = 1; i < select.FinalPointList.Count; i++)
            {
                Point curPoint = select.FinalPointList[i];
                Point prevPoint = select.FinalPointList[i - 1];
                var line = new LineGeometry(curPoint, prevPoint);
                g1.AddGeometry(line);
            }

            ShapeItem newShape = new()
            {
                Geometry = g1,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
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
            //HighLightIt(shape.Geometry.Bounds);
        }

        public void DimensionChangeCurve(Point a, ShapeItem shape)
        {
            Debug.WriteLine(select.SelectBox + " bounce bounce bounce yippe yipee ");
            if (select.SelectBox == 6 || select.SelectBox == 7)
            {
                // horitzontal
                ShapeItem x = shape;
                PathGeometry g1 = new();

                select.FinalPointList.Clear();

                // X fixing
                double extraX = a.X - select.InitialSelectionPoint.X;
                double width = select.SelectedObject.Geometry.Bounds.Width;
                double newWidth = width + extraX;
                if (select.SelectBox == 6)
                    newWidth = width - extraX;
                double scaleX = newWidth / width;
                double rectTopX = select.SelectedObject.Geometry.Bounds.X;
                double rectBottomX = select.SelectedObject.Geometry.Bounds.X + select.SelectedObject.Geometry.Bounds.Width;

                if (select.SelectBox == 7)
                {
                    for (int i = 0; i < select.SelectedObject.PointList.Count; i++)
                    {
                        Point curPoint = select.SelectedObject.PointList[i];
                        curPoint.X = rectTopX + (curPoint.X - rectTopX) * scaleX;
                        select.FinalPointList.Add(curPoint);
                    }

                }
                else
                {
                    for (int i = 0; i < select.SelectedObject.PointList.Count; i++)
                    {
                        Point curPoint = select.SelectedObject.PointList[i];
                        curPoint.X = rectBottomX + (curPoint.X - rectBottomX) * scaleX;
                        select.FinalPointList.Add(curPoint);
                    }
                }


                // adding to geometry
                for (int i = 1; i < select.FinalPointList.Count; i++)
                {
                    Point curPoint = select.FinalPointList[i];
                    Point prevPoint = select.FinalPointList[i - 1];
                    var line = new LineGeometry(curPoint, prevPoint);
                    g1.AddGeometry(line);
                }

                ShapeItem newShape = new()
                {
                    Geometry = g1,
                    Fill = fillBrush,
                    Stroke = strokeBrush,
                    ZIndex = currentZIndex,
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
            else if (select.SelectBox == 5 || select.SelectBox == 8)
            {

                ShapeItem x = shape;
                PathGeometry g1 = new PathGeometry();

                select.FinalPointList.Clear();

                // Y fixing
                double extraY = a.Y - select.InitialSelectionPoint.Y;
                double height = select.SelectedObject.Geometry.Bounds.Height;
                double newHeight = height - extraY;
                if (select.SelectBox == 8)
                    newHeight = height + extraY;
                double scaleY = newHeight / height;
                double rectTopY = select.SelectedObject.Geometry.Bounds.Y;
                double rectBottomY = select.SelectedObject.Geometry.Bounds.Y + select.SelectedObject.Geometry.Bounds.Height;

                if (select.SelectBox == 8)
                {
                    for (int i = 0; i < x.PointList.Count; i++)
                    {
                        Point curPoint = x.PointList[i];
                        curPoint.Y = rectTopY + (curPoint.Y - rectTopY) * scaleY;
                        select.FinalPointList.Add(curPoint);
                    }

                }
                else
                {
                    for (int i = 0; i < x.PointList.Count; i++)
                    {
                        Point curPoint = x.PointList[i];
                        curPoint.Y = rectBottomY + (curPoint.Y - rectBottomY) * scaleY;
                        select.FinalPointList.Add(curPoint);
                    }
                }


                // adding to geometry
                for (int i = 1; i < select.FinalPointList.Count; i++)
                {
                    Point curPoint = select.FinalPointList[i];
                    Point prevPoint = select.FinalPointList[i - 1];
                    var line = new LineGeometry(curPoint, prevPoint);
                    g1.AddGeometry(line);
                }

                ShapeItem newShape = new ShapeItem
                {
                    Geometry = g1,
                    Fill = fillBrush,
                    Stroke = strokeBrush,
                    ZIndex = currentZIndex,
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
                    var line = new LineGeometry(newPoint, prevPoint);
                    g1.AddGeometry(line);
                }
                prevPoint = newPoint;
            }
            ShapeItem newShape = new ShapeItem
            {
                Geometry = g1,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
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

            HighLightIt(newShape.Geometry.Bounds);
        }

        public void FinishingCurve()
        {
            PathGeometry g1 = new();
            for (int i = 1; i < select.FinalPointList.Count; i++)
            {
                Point curPoint = select.FinalPointList[i];
                Point prevPoint = select.FinalPointList[i - 1];
                var line = new LineGeometry(curPoint, prevPoint);
                g1.AddGeometry(line);
            }

            List<Point> newPointList = new();

            foreach (Point p in select.FinalPointList)
                newPointList.Add(p);

            ShapeItem updatingShape = new()
            {
                Geometry = g1,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
                Id = select.SelectedObject.Id,
                PointList = newPointList
            };


            for (int i = 0; i < ShapeItems.Count; i++)
            {
                if (ShapeItems[i].Id == select.SelectedObject.Id)
                {
                    ShapeItems[i] = updatingShape;
                }
            }

        }
    }
}
