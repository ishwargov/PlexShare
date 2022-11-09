/******************************************************************************
 * Filename    = ContentServerFactory.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class to implement Factory design Pattern for ContentServer.
 *****************************************************************************/

using System;

namespace PlexShareContent.Server
{
    public class ContentServerFactory
    {
        // initializing in a thread safe way using Lazy<>
        private static readonly Lazy<ContentServer> _contentServer = new(() => new ContentServer());

        /// <summary>
        /// Creates a Server side content manager that will live until the end of the program
        /// </summary>
        /// <returns>ContentServer object which implements IContentServer interface</returns>
        public static IContentServer GetInstance()
        {
            return _contentServer.Value;
        }
        
    }
}
