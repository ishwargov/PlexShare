using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
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

    }
}
