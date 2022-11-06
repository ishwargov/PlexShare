using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;


namespace PlexShareWhiteboard
{
    public class ShapeItem
    {

        public ShapeItem()
        {
            ;
        }
        public Geometry Geometry { get; set; }
        public String GeometryString { get; set; }
        public String TextString { get; set; }
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


        public SerialisableShapeItem ConvertToSerialisableShapeItem(ShapeItem x)
        {
            SerialisableShapeItem y = new SerialisableShapeItem
            {
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
