/******************************************************************************
 * Filename    = IContentClient.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Interface to let clients send data and subscribe to
 *				 to notification from this class. 
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
        public void ClientSendData(SendContentData contentData);

        /// <summary>
        /// Lets client subscribe to notifications from this class
        /// </summary>
        /// <param name="subscriber">Subscriber object which is an implementation of the IContentListener interface</param>
        public void ClientSubscribe(IContentListener subscriber);
    }
}
