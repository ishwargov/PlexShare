/******************************************************************************
 * Filename    = IContentClient.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Interface to let clients send, edit, update messages, etc.
 *               and subscribe to notification from this class. 
 *****************************************************************************/

using PlexShareContent.DataModels;

namespace PlexShareContent.Client
{
    public interface IContentClient
    {
        /// <summary>
        /// Sends chat or file data to clients
        /// </summary>
        /// <param name="contentData">Data to be sent</param>
        void ClientSendData(SendContentData contentData);

        /// <summary>
        /// Lets client subscribe to notifications from this class
        /// </summary>
        /// <param name="subscriber">Subscriber object which is an implementation of the IContentListener interface</param>
        void ClientSubscribe(IContentListener subscriber);

        /// <summary>
        /// Edit a previous chat message
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="newMessage">Edited message</param>
        void ClientEdit(int messageID, string newMessage);

        /// <summary>
        /// Download file to specific path on client machine
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="savePath">Path to which the file will be downloaded</param>
        void ClientDownload(int messageID, string savePath);

        /// <summary>
        /// Star message for it to be included in the dashboard summary
        /// </summary>
        /// <param name="messageID"></param>
        void ClientStar(int messageID);

        /// <summary>
        /// Get message thread corresponding to thread ID
        /// </summary>
        /// <param name="threadID">ID of the thread</param>
        /// <returns>Object implementing ChatThread class</returns>
        ChatThread ClientGetThread(int threadID);

        /// <summary>
        /// Get user ID associated with instance
        /// </summary>
        /// <returns>User ID associated with instance</returns>
        int GetUserID();
    }
}
