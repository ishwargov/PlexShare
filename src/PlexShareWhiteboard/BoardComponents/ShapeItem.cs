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

        public ShapeItem()
        {
            ;
        }
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
                GeometryString = this.GeometryString,
                Start = this.Start,
                End = this.End,
                Fill = this.Fill,
                Stroke = this.Stroke,
                ZIndex = this.ZIndex,
                AnchorPoint = this.AnchorPoint,
                Id = this.Id,
                StrokeThickness = this.StrokeThickness,
            };
            return newShape;
        }
        public SerializableShapeItem ConvertToSerialisableShapeItem(ShapeItem x)
        {
            SerializableShapeItem y = new SerializableShapeItem
            {
                FontSize = x.FontSize,
                //GeometryString = x.GeometryString,
                GeometryString = x.Geometry.GetType().Name,
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
