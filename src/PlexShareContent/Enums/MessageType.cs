/******************************************************************************
 * Filename    = MessageType.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Enum for type of message sent or received.
 *****************************************************************************/

namespace PlexShareContent
{
    /// <summary>
    ///     Type of message - Chat or File.
    /// </summary>
    public enum MessageType
    {
        File,
        Chat,
        HistoryRequest
    }
}