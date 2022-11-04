using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace WpfApp1
{
    internal class ShapeItem
    {
        public Geometry Geometry { get; set; }
        public Brush Fill { get; set; }
        public Brush Stroke { get; set; }
        public int ZIndex { get; set; }
        public string Id { get; set; }
        public string user { get; set; }
        public string timeStamp { get; set; }
        public Point anchorPoint { get; set; }

    }
}
