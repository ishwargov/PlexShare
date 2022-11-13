using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace PlexShareWhiteboard.BoardComponents
{
    public class SerializableShapeItem
    {
        public string GeometryString { get; set; }
        public string TextString { get; set; }
        public List<Point> PointList { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public Brush Fill { get; set; }
        public Brush Stroke { get; set; }
        public int ZIndex { get; set; }
        public int FontSize { get; set; }
        public int StrokeThickness { get; set; }
        public string Id { get; set; }
        public string User { get; set; }
        public string TimeStamp { get; set; }
        public Point AnchorPoint { get; set; }

        public SerializableShapeItem()
        {
            ;
        }

        public ShapeItem ConvertToShapeItem(SerializableShapeItem x)
        {
            if (x == null)
                return null;
            Geometry g = null;

            if (x.GeometryString == "EllipseGeometry")
            {
                Rect boundingBox = new Rect(x.Start, x.End);
                g = new EllipseGeometry(boundingBox);
            }
            else if (x.GeometryString == "RectangleGeometry")
            {
                Rect boundingBox = new Rect(Start, End);
                g = new RectangleGeometry(boundingBox);
            }
            else if (x.GeometryString == "PathGeometry")
            {
                PathGeometry g1 = new PathGeometry();

                for (int i = 1; i < x.PointList.Count; i++)
                {
                    Point curPoint = x.PointList[i];
                    Point prevPoint = x.PointList[i - 1];
                    var line = new LineGeometry(curPoint, prevPoint);
                    g1.AddGeometry(line);
                }
                g = g1;
            }
            else if (x.GeometryString == "LineGeometry")
            {
                g = new LineGeometry(Start, End);

            }
            else if (x.GeometryString == "TextGeometry")
            {
                FormattedText formattedText = new FormattedText(
                    x.TextString,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    x.FontSize,
                    x.Stroke,
                    3);

            }

            ShapeItem y = new ShapeItem
            {
                Geometry = g,
                FontSize = x.FontSize,
                GeometryString = x.GeometryString,
                TextString = x.TextString,
                PointList = x.PointList,
                Start = x.Start,
                End = x.End,
                Fill = x.Fill,
                Stroke = x.Stroke,
                ZIndex = x.ZIndex,
                StrokeThickness = x.StrokeThickness,
                Id = x.Id,
                User = x.User,
                TimeStamp = x.TimeStamp,
                AnchorPoint = x.AnchorPoint,
            };

            return y;
        }


    }
}
