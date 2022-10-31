using PlexShareContent.Client;

namespace ContentTests.Client
{
    [TestClass]
    public class ContentClientFactoryTests
    {
        [TestMethod]
        public void GetInstance_SingleThread_ReturnsSingleInstance()
        {
            var ref1 = ContentClientFactory.GetInstance();
            var ref2 = ContentClientFactory.GetInstance();

            Assert.ReferenceEquals(ref1, ref2);
        }
        [TestMethod]
        public void GetInstance_MultiThread_ReturnsSingleInstance()
        {
            IContentClient? ref1 = null;
            IContentClient? ref2 = null;
            var t1 = Task.Run(() => { ref1 = ContentClientFactory.GetInstance(); });
            var t2 = Task.Run(() => { ref2 = ContentClientFactory.GetInstance(); });

            Task.WaitAll(t1, t2);

            Assert.ReferenceEquals(ref1, ref2);
        }
    }
}