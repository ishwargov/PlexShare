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
            var instance1 = ContentClientFactory.GetInstance();
            var instance2 = ContentClientFactory.GetInstance();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void GetInstance_MultiThread_ReturnsSingleInstance()
        {
            IContentClient? instance1 = null;
            IContentClient? instance2 = null;
            // run the factory on multiple threads
            var t1 = Task.Run(() => { instance1 = ContentClientFactory.GetInstance(); });
            var t2 = Task.Run(() => { instance2 = ContentClientFactory.GetInstance(); });

            Task.WaitAll(t1, t2);

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void SetUser_ValidInput_SetsValidUserID()
        {
            var instance = ContentClientFactory.GetInstance();
            ContentClientFactory.SetUser(42);
            Assert.Equal(42, instance.GetUserID());
        }

        [Fact]
        public void SetUser_MultipleInstances_ReturnsSingleUserID()
        {
            var instance1 = ContentClientFactory.GetInstance();
            var instance2 = ContentClientFactory.GetInstance();

            Assert.Same(instance1, instance2);

            var userID1 = instance1.GetUserID();
            var userID2 = instance2.GetUserID();

            Assert.Equal(userID1, userID2);
        }
    }
}
