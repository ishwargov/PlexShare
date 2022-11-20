/******************************************************************************
 * Filename    = ReceiveContentData.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class containing metadata related to a message. Used for 
 *               received messages. 
 *****************************************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using PlexShareContent.Enums;

namespace PlexShareContent.DataModels
{
    [ExcludeFromCodeCoverage]
    public class ReceiveContentData
    {
        /// <summary>
        /// Type of message - chat or file
        /// </summary>
        public MessageType Type;

        /// <summary>
        /// Content of message if chat type message.
        /// File path if file type message.
        /// </summary>
        public string Data;

        /// <summary>
        /// ID of the message.
        /// </summary>
        public int MessageID;

        /// <summary>
        /// List containing the receiver IDs.
        /// Empty in case of broadcast message.
        /// </summary>
        public int[]? ReceiverIDs;

        /// <summary>
        /// ID of message being replied to.
        /// </summary>
        public int ReplyMessageID;

        /// <summary>
        /// ID of thread to which the message belongs to.
        /// If the thread does not exist, -1
        /// </summary>
        public int ReplyThreadID;

        /// <summary>
        /// User ID of the sender
        /// </summary>
        public int SenderID;

        /// <summary>
        /// Time at which the message was sent
        /// </summary>
        public DateTime SentTime;

        /// <summary>
        /// Boolean to check if message is starred
        /// </summary>
        public bool Starred;

        /// <summary>
        /// Type of message event - star, download, etc. 
        /// </summary>
        public MessageEvent Event;

        /// <summary>
        /// Constructor to create type without parameters.
        /// </summary>
        public ReceiveContentData()
        {
            MessageID = -1;
            ReceiverIDs = null;
            ReplyMessageID = -1;
            ReplyThreadID = -1;
            SenderID = -1;
            SentTime = new DateTime();
            Starred = false;
        }
    }
}
