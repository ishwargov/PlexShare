/****************************
 * Filename    = WBVMHighlight.cs
 *
 * Author      = Jerry John Thomas
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is part of View Model.
 *               This is used for showing whether an object is selected with a 
 *               highlight box that has corners for transmission and transformation.
 ****************************/

using System;
using System.Windows.Media;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;
using System.Diagnostics;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        /// <summary>
        /// Generates a rectangle taking in x, y, width and height.  
        /// </summary>
        /// <param name="x">bounding box rectangle anchor point x</param>
        /// <param name="y">bounding box rectangle anchor point y</param>
        /// <param name="w">bounding box rectangle width</param>
        /// <param name="h">bounding box rectangle height</param>
        /// <param name="f1">fill brush </param>
        /// <param name="s1">stroke brush</param>
        /// <param name="id">id of the shape</param>
        /// <param name="zi">Z index</param>
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
        /// <summary>
        /// Generates a highlight Textbox for a given Rectangle, this is used usually by the textbox.
        /// </summary>
        /// <param name="rect">the Rectangle for which highlight box is created for</param>
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
        /// <summary>
        /// This takes in rectangle and computes the 9 highlight shapes that a selected rectanlge has. 
        /// (1 : bounding rectanlge) + (4 : middle of the sides) + (4: corners of the rectangle)
        /// </summary>
        /// <param name="rect">the Rectangle for which highlight box is created for</param>
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

        /// <summary>
        /// Generates a highlight box for a line
        /// </summary>
        /// <param name="line">takes in the line for highlight generation</param>
        public void HighLightIt(LineGeometry line)
        {
            double x1 = line.StartPoint.X;
            double y1 = line.StartPoint.Y;
            double x2 = line.EndPoint.X;
            double y2 = line.EndPoint.Y;

            Trace.WriteLine("[Whiteboard]  " + "Entering line hightlighting with " + x1 + "  " + y1 + " " + x2 + " " + y2);

            ShapeItem hsBody = GenerateLine(x1, y1, x2, y2, null, Brushes.DodgerBlue, "hsBody", currentZIndex);
            highlightShapes.Add(hsBody);

            ShapeItem hsTop = GenerateRectangleXYWidthHeight(x1 - blobSize / 2, y1 - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsTop", 100000);
            highlightShapes.Add(hsTop);

            ShapeItem hsBottom = GenerateRectangleXYWidthHeight(x2 - blobSize / 2, y2 - blobSize / 2, blobSize, blobSize, Brushes.DodgerBlue, Brushes.DodgerBlue, "hsBottom", 100000);
            highlightShapes.Add(hsBottom);

            // just adds to shapeitems
            foreach (ShapeItem si in highlightShapes)
                ShapeItems.Add(si);
        }

        /// <summary>
        /// Generates a rectangle taking in 2 points of the Rectangle.  
        /// </summary>
        /// <param name="a">One point of the rectangle</param>
        /// <param name="b">Other point of the rectangle</param>
        public void HighLightIt(Point a, Point b)
        {
            Rect rect = new (a, b);
            HighLightIt(rect);
        }

        /// <summary>
        /// UnHighLights all the highlight shapes generated by HighLightIt  
        /// </summary>
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
