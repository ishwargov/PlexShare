using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareContent.Server
{
    public class FactoryContentServer
    {
        public static IContentServer contentServerInstance;

        public static IContentServer GetInstance()
        {
            if (contentServerInstance != null)
                return contentServerInstance;
            else
            {
                contentServerInstance = new ContentServer();
                return contentServerInstance;
            }
        }
    }
}
