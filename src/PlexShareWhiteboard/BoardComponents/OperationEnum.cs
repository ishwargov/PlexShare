/***************************
 * Filename    = WhiteBoardViewModel.cs
 *
 * Author      = Aiswarya H
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This defines all the operations that can be performed 
                 on the WhiteBoard.
 ***************************/

namespace PlexShareWhiteboard.BoardComponents
{
    /// <summary>
    ///         Various operations that can be performed on the ShapeItems.
    /// </summary>
    public enum Operation
    {
        Creation,
        Deletion,
        ModifyShape,
        Clear,
        UndoClear,
        RestoreSnapshot,
        CreateSnapshot
    }
}