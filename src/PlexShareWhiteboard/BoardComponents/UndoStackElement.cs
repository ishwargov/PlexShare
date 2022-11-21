/***************************
 * Filename    = UndoStackElement.cs
 *
 * Author      = Aiswarya H
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This contains the UndoStackElement.
 *               This constitutes an element of the Undo and Redo Stacks.
 ***************************/

using PlexShareWhiteboard;

namespace PlexShareWhiteboard.BoardComponents
{
    /// <summary>
    ///         Class used for Stack Element to be stored in UndoStack or RestoreStack. 
    /// </summary>
    public class UndoStackElement
    {
        /// <summary>
        ///      Constructor for creating UndoStackElement.
        /// </summary>
        /// <param name="prvShape">The previous state of the ShapeItem that is passed </param>
        /// <param name="newShape">The new state of the ShapeItem that is passed </param>
        /// <param name="op">Operation that caused the state change </param>
        public UndoStackElement(ShapeItem prvShape, ShapeItem newShape, Operation op)
        {
            PrvShape = prvShape;
            NewShape = newShape;
            Op = op;
        }

        public ShapeItem PrvShape { get; set; }
        public ShapeItem NewShape { get; set; }
        public Operation Op { get; set; }
    }
}
