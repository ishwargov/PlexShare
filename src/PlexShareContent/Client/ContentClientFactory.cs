/******************************************************************************
 * Filename    = ContentClientFactory.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Factory class for creating class implementing 
 *               IContentClient interface.  
 *****************************************************************************/

using System;

namespace PlexShareContent.Client
{
    public class ContentClientFactory
    {
        // initializing in a thread safe way using Lazy<>
        private static readonly Lazy<IContentClient> _contentClient = new(() => new ContentClient());

        /// <summary>
        /// Creates a client side content manager that will live until the end of the program
        /// </summary>
        /// <returns>ContentClient object which implements IContentClient interface</returns>
        public static IContentClient GetInstance()
        {
            return _contentClient.Value;
        }
    }
}
