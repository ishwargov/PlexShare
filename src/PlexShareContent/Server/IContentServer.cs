/******************************************************************************
 * Filename    = IContentServer.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description =   This file provides Interface for ContentServer
 *****************************************************************************/

using PlexShareContent.Client;
using PlexShareContent.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareContent.Server
{
    public interface IContentServer
    {
        /// <summary>
        ///     Add a new subscriber to the list of subscribers
        /// </summary>
        /// <param name="subscriber">IContentListener implementation provided by the subscriber</param>
        void ServerSubscribe(IContentListener subscriber);

        /// <summary>
        ///     Get all the messages sent
        /// </summary>
        /// <returns>List of Thread objects</returns>
        List<ChatThread> GetAllMessages();

        /// <summary>
        ///     Sends all the messages to the client of the user with user id = userId
        /// </summary>
        /// <param name="userId">user id of the user to which messages needs to be sent</param>
        void SSendAllMessagesToClient(int userId);
    }
}
