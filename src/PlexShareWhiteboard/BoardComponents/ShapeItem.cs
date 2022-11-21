using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace PlexShareWhiteboard.BoardComponents
{
    public class ShapeItem
    {
        public Geometry Geometry { get; set; }
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

        public ShapeItem DeepClone()
        {
            ShapeItem newShape = new()
            {
                Geometry = this.Geometry.Clone(),
                FontSize = this.FontSize,
                GeometryString = this.Geometry.GetType().Name,
                TextString = this.TextString,
                PointList = this.PointList,
                Start = this.Start,
                End = this.End,
                Fill = this.Fill,
                Stroke = this.Stroke,
                ZIndex = this.ZIndex,
                StrokeThickness = this.StrokeThickness,
                Id = this.Id,
                User = this.User,
                TimeStamp = this.TimeStamp,
                AnchorPoint = this.AnchorPoint,
            };
            return newShape;
        }
    }
}
