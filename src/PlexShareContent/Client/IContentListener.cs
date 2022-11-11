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
using System.Collections.Generic;

namespace PlexShareContent.Client
{
    public interface IContentListener
    {
        /// <summary>
        /// Handles the reception of a message.
        /// </summary>
        /// <param name="contentData">Instance of ReceiveContentData class</param>
        void OnMessageReceived(ReceiveContentData contentData);

        /// <summary>
        /// Handles event of all messages sent to / received from client at once
        /// </summary>
        /// <param name="allMessages">List of thread objects containing all messages</param>
        void OnAllMessagesReceived(List<ChatThread> allMessages);
    }
}