/******************************************************************************
 * Filename    = IContentListener.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Interface to notify clients when a message has been received.
 *****************************************************************************/

using PlexShareContent.DataModels;

namespace PlexShareContent.Client
{
    public interface IContentListener
    {
        /// <summary>
        /// Handles the reception of a message.
        /// </summary>
        /// <param name="contentData"></param>
        void OnMessageReceived(ReceiveContentData contentData);
    }
}