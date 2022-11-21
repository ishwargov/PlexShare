/***************************
 * Filename    = OperationEnum.cs
 *
 * Author      = Aiswarya H
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This defines all the operations that can be performed 
                 on the WhiteBoard. It also includes some messages 
                 that have to be communicated. 
 ***************************/

namespace PlexShareWhiteboard.BoardComponents
{
    /// <summary>
    ///         Type of operations that can be performed on the ShapeItems
    ///         and various kinds of messages that can be sent
    ///         (NewUser, RestoreSnapshot,CreateSnapshot, Clear)
    /// </summary>
    public enum Operation
    {
        Creation,
        Deletion,
        ModifyShape,
        Clear,
        RestoreSnapshot,
        CreateSnapshot,
        NewUser
    }
}