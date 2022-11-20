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

using System.Diagnostics.CodeAnalysis;

namespace PlexShareContent.DataModels
{
    [ExcludeFromCodeCoverage]
    public class ContentData : ReceiveContentData
    {
        /// <summary>
        /// Details related to the file. 
        /// </summary>
        public SendFileData FileData;

        /// <summary>
        /// Empty constructor to create type without parameters.
        /// </summary>
        public ContentData()
        {
        }

        /// <summary>
        /// Constructor to create type with parameters.
        /// </summary>
        /// <param name="receiveContentData">Instance of the ReceiveContentData class</param>
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
