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
 *               received data. 
 *****************************************************************************/

namespace PlexShareContent.DataModels
{
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
        /// List containing the receiver IDs.
        /// </summary>
        public int[]? ReceiverIDs;

        /// <summary>
        /// Constrcutor to initialize the fields
        /// </summary>
        public ReceiveContentData()
        {
            Data = "";
            ReceiverIDs = null;
        }
    }
}
