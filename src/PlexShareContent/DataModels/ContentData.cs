/******************************************************************************
 * Filename    = ContentData.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class containing metadata related to a message. Used for 
 *               internal communication between client and server components. 
 *****************************************************************************/

namespace PlexShareContent.DataModels
{
    public class ContentData : ReceiveContentData
    {
        /// <summary>
        /// Details related to the file. 
        /// </summary>
        public SendFileData FileData;

        /// <summary>
        /// Constructor to initialize fields
        /// </summary>
        /// <param name="receiveContentData">Object implementing the ReceiveContentData class</param>
        public ContentData(ReceiveContentData receiveContentData)
        {
            Type = receiveContentData.Type;
            Data = receiveContentData.Data;
            MessageID = receiveContentData.MessageID;
            ReceiverIDs = receiveContentData.ReceiverIDs;
            ReplyMessageID = receiveContentData.ReplyMessageID;
            ReplyThreadID = receiveContentData.ReplyThreadID;
            SenderID = receiveContentData.SenderID;
            SentTime = receiveContentData.SentTime;
            Starred = receiveContentData.Starred;
            Event = receiveContentData.Event;
        }

        /// <summary>
        /// Make a copy of ContentData object.
        /// </summary>
        /// <returns>Shallow copy of the object.</returns>
        public ContentData Copy()
        {
            return MemberwiseClone() as ContentData;
        }
    }
}
