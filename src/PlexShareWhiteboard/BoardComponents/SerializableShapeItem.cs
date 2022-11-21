/***************************
 * Filename    = SerialisableShapeItem.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This declares an adapter class for adapting shape class to a
 *               serialisable shape class inorder to facilitate serialising.
 ***************************/

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

    }
}
