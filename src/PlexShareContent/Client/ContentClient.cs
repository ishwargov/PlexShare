/******************************************************************************
 * Filename    = ContentClient.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Client-side content manager class which provides interfaces 
 *               to other modules and deals with various user functions.   
 *****************************************************************************/

using PlexShareContent.DataModels;
using System;

namespace PlexShareContent.Client
{
    public class ContentClient : IContentClient
    {
        public ContentClient()
        {

        }

        public void ClientSendData(SendContentData contentData)
        {
            switch (contentData.Type)
            {
                case MessageType.Chat:
                    // send message to chat client
                    break;
                case MessageType.File:
                    // send file to file client
                    break;
                default:
                    // throw exception
                    throw new ArgumentException("Invalid Message Field Type");
            }
        }

        public void ClientSubscribe(IContentListener subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("Null subscriber");
            }
            else
            {
                // add subscriber to subsribers' list
            }
        }
    }
}
