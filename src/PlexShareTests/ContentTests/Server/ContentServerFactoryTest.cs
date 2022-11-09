/******************************************************************************
 * Filename    = ContentServerFactoryTest.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Contains Tests for ContentServerFactory
 *****************************************************************************/

using PlexShareContent.Server;
using System.Threading;

namespace PlexShareTests.ContentTests.Server
{
    public class ContentServerFactoryTest
    {
        [Fact]
        public void GetInstance_SingleThread_ReturnsSingleInstance()
        {
            IContentServer ref1 = ContentServerFactory.GetInstance();
            IContentServer ref2 = ContentServerFactory.GetInstance();

           Assert.Same(ref1, ref2);
        }
    }
}
