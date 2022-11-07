using PlexShareContent.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareContent.Server
{
    public class ContentServerFactory
    {
        // initializing in a thread safe way using Lazy<>
        private static readonly Lazy<IContentServer> _contentServer = new(() => new ContentServer());

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
