/***************************
 * Filename    = ShapeItemTests.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Tests for ShapeItem.cs.
 ***************************/

using System.Windows;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareTests.WhiteboardTests.BoardComponents
{
    [Collection("Sequential")]
    public class ShapeItemTests
    {
        /// <summary>
        ///     Testing Deep Clone of ShapeItem.
        /// </summary>
        [Fact]  
        public void ShapeItem_DeepCloneObject()
        {
            Utility utility = new();
            ShapeItem shape1 = utility.CreateRandomShape();
            ShapeItem shape2 = shape1.DeepClone();
            Assert.True(utility.CompareShapeItems(shape1, shape2));
        }
    }
}
