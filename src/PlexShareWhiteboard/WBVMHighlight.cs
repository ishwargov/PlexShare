using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;
using System.Diagnostics;
using System.Windows.Shapes;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        public static ShapeItem GenerateRectangleXYWidthHeight(double x, double y, double w, double h, Brush f1, Brush s1, String id, int zi)
        {
            ShapeItem s = new ()
            {
                Geometry = new RectangleGeometry(new Rect(x, y, w, h)),
                Fill = f1,
                Stroke = s1,
                ZIndex = zi,
                Id = id,
                StrokeThickness = 1
            };
            return s;
        }

        public void HighLightTextBox(Rect rect)
        {
            double x = rect.X;
            double y = rect.Y;
            double height = rect.Height;
            double width = rect.Width;

            ShapeItem hsBody = GenerateRectangleXYWidthHeight(x, y, width, height, null, Brushes.DodgerBlue, "hsBody", 100000);
            highlightShapes.Add(hsBody);
            ShapeItems.Add(hsBody);
        }

        public void HighLightIt(Rect rect)
        {
            double x = rect.X;
            double y = rect.Y;
            double height = rect.Height;
            double width = rect.Width;

            int highlightZIndex = 100000;

            ShapeItem hsBody = GenerateRectangleXYWidthHeight(x, y, width, height, null, Brushes.DodgerBlue, "hsBody", highlightZIndex);
            highlightShapes.Add(hsBody);

            ShapeItem hsTopCenter = GenerateRectangleXYWidthHeight(x + width / 2 - blobSize / 2, y - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsTopCenter", highlightZIndex);
            highlightShapes.Add(hsTopCenter);

            ShapeItem hsBottomCenter = GenerateRectangleXYWidthHeight(x + width / 2 - blobSize / 2, y + height - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsBottomCenter", highlightZIndex);
            highlightShapes.Add(hsBottomCenter);

            ShapeItem hsRightCenter = GenerateRectangleXYWidthHeight(x + width - blobSize / 2, y + height / 2 - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsRightCenter", highlightZIndex);
            highlightShapes.Add(hsRightCenter);

            ShapeItem hsLeftCenter = GenerateRectangleXYWidthHeight(x - blobSize / 2, y + height / 2 - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsLeftCenter", highlightZIndex);
            highlightShapes.Add(hsLeftCenter);

            ShapeItem hsTopLeft = GenerateRectangleXYWidthHeight(x - blobSize / 2, y - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsTopLeft", highlightZIndex);
            highlightShapes.Add(hsTopLeft);

            ShapeItem hsBottomLeft = GenerateRectangleXYWidthHeight(x - blobSize / 2, y + height - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsBottomLeft", highlightZIndex);
            highlightShapes.Add(hsBottomLeft);

            ShapeItem hsTopRight = GenerateRectangleXYWidthHeight(x + width - blobSize / 2, y - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsTopRight", highlightZIndex);
            highlightShapes.Add(hsTopRight);

            ShapeItem hsBottomRight = GenerateRectangleXYWidthHeight(x + width - blobSize / 2, y + height - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsBottomRight", highlightZIndex);
            highlightShapes.Add(hsBottomRight);

            // just adds to shapeitems
            foreach (ShapeItem si in highlightShapes)
                ShapeItems.Add(si);
        }
        //deon
        public void HighLightIt(Line line)
        {
            double x1 = line.X1;
            double y1 = line.Y1;
            double x2 = line.X2;
            double y2 = line.Y2;

            Debug.WriteLine("Entering line hightlighting with " + x1 + "  " + y1 + " " + x2 + " " + y2);

            ShapeItem hsBody = GenerateLine(x1, y1, x2, y2, null, Brushes.DodgerBlue, "hsBody", currentZIndex);
            highlightShapes.Add(hsBody);

            ShapeItem hsTop = GenerateRectangleXYWidthHeight(x1 - blobSize / 2, y1 - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsTop", currentZIndex);
            highlightShapes.Add(hsTop);

            ShapeItem hsBottom = GenerateRectangleXYWidthHeight(x2 - blobSize / 2, y2 - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsBottom", currentZIndex);
            highlightShapes.Add(hsBottom);

            // just adds to shapeitems
            foreach (ShapeItem si in highlightShapes)
                ShapeItems.Add(si);
        }

        
        public void HighLightIt(Point a, Point b)
        {
            Rect rect = new (a, b);
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
    }
}
