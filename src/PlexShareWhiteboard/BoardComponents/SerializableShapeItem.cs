/***************************
 * Filename    = SerializableShapeItem.cs
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
using System.Windows.Media;
using System.Windows;

namespace PlexShareWhiteboard.BoardComponents
{
    public class SerializableShapeItem
    {
        /// <summary>
        ///     String representing the type of Geometry of the ShapeItem.
        /// </summary>
        public string GeometryString { get; set; }

        /// <summary>
        ///     String consisting of the message inside a text box ShapeItem.
        /// </summary>
        public string TextString { get; set; }

        /// <summary>
        ///     Contains curves of Points
        /// </summary>
        public List<Point> PointList { get; set; }

        /// <summary>
        ///     Start of the ShapeItem.
        /// </summary>
        public Point Start { get; set; }

        /// <summary>
        ///     End of the ShapeItem.
        /// </summary>
        public Point End { get; set; }

        /// <summary>
        ///     The fill brush of the ShapeItem.
        /// </summary>
        public Brush Fill { get; set; }

        /// <summary>
        ///     The stroke brush of the ShapeItem.
        /// </summary>
        public Brush Stroke { get; set; }

        /// <summary>
        ///     Specifies the stack order of an element.
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        ///     Font Size of Text.
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        ///     Thickness of stroke
        /// </summary>
        public int StrokeThickness { get; set; }

        /// <summary>
        ///     UID representing a unique ShapeItem
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     User ID of creator of ShapeItem
        /// </summary>
        public string User { get; set; }

        /// <summary>
        ///     Time at which ShapeItem was created or updated
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        ///     Point on an object that remains stationary while an object is resized.
        /// </summary>
        public Point AnchorPoint { get; set; }
    }
}
