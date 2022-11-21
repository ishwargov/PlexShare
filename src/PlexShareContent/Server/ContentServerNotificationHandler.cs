/******************************************************************************
 * Filename    = ContentServerNotificationHandler.cs
 * Author      = Anurag Jha
 * Product     = PlexShare
 * Project     = PlexShareContent
 * Description =  This file handles the notifications from Network Module.
 *****************************************************************************/

using PlexShareNetwork;
namespace PlexShareContent.Server
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        public readonly ContentServer ContentServer;
        public ContentServerNotificationHandler(ContentServer contentServer)
        {
            ContentServer = contentServer;
        }
        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            ContentServer.Receive(data);
        }
    }
}
