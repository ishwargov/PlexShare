/******************************************************************************
 * Filename    = MessageEvent.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Enum for type of message event called. 
 *****************************************************************************/

namespace PlexShareContent.Enums
{
    /// <summary>
    /// Type of message event - New, Edit, Star, Download.
    /// </summary>
    public enum MessageEvent
    {
        New,
        Edit,
        Delete,
        Star,
        Download
    }
}
