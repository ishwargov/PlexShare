/******************************************************************************
 * Filename    = ContentClientFactoryTests.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareTests
 *
 * Description = Tests for content client factory. 
 *****************************************************************************/

using PlexShareContent.Client;

namespace PlexShareTests.ContentTests.Client
{
    public class ContentClientFactoryTests
    {
        [Fact]
        public void GetInstance_SingleThread_ReturnsSingleInstance()
        {
            var ref1 = ContentClientFactory.GetInstance();
            var ref2 = ContentClientFactory.GetInstance();

            Assert.Same(ref1, ref2);
        }
        [Fact]
        public void GetInstance_MultiThread_ReturnsSingleInstance()
        {
            IContentClient? ref1 = null;
            IContentClient? ref2 = null;
            var t1 = Task.Run(() => { ref1 = ContentClientFactory.GetInstance(); });
            var t2 = Task.Run(() => { ref2 = ContentClientFactory.GetInstance(); });

            Task.WaitAll(t1, t2);

            Assert.Same(ref1, ref2);
        }
    }
}
