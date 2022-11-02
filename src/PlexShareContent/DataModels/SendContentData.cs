/******************************************************************************
 * Filename    = SendContentData.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class containing metadata related to a message. Used for 
 *               sending messages. 
 *****************************************************************************/

namespace PlexShareContent.DataModels
{
    public class SendContentData
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
        /// If the thread does not exist, -1.
        /// </summary>
        public int ReplyThreadID;

        /// <summary>
        /// Constrcutor to initialize the fields.
        /// </summary>
        public SendContentData()
        {
            Data = "";
            ReceiverIDs = null;
            ReplyMessageID = -1;
            ReplyThreadID = -1;
        }
    }
}
