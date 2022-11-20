using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareTests.WhiteboardTests.BoardComponents
{
    [Collection("Sequential")]
    public class ShapeItemTests
    {
        [Fact]  
        public void ShapeItem_DeepCloneObject()
        {
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            ShapeItem shape1 = Utility.CreateShape(start, end, "EllipseGeometry", "randomID");
            ShapeItem shape2 = shape1.DeepClone();
            Assert.True(Utility.CompareShapeItems(shape1, shape2));
        }
    }
}
