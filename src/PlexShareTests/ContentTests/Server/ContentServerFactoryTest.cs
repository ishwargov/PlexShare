using PlexShareContent.Client;
using PlexShareContent.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.ContentTests.Server
{
    public class ContentServerFactoryTest
    {
        [Fact]
        public void GetInstance_SingleThread_ReturnsSingleInstance()
        {
            var ref1 = ContentServerFactory.GetInstance();
            var ref2 = ContentServerFactory.GetInstance();

            Assert.Same(ref1, ref2);
        }
        [Fact]
        public void GetInstance_MultiThread_ReturnsSingleInstance()
        {
            IContentServer? ref1 = null;
            IContentServer? ref2 = null;
            var t1 = Task.Run(() => { ref1 = ContentServerFactory.GetInstance(); });
            var t2 = Task.Run(() => { ref2 = ContentServerFactory.GetInstance(); });

            Task.WaitAll(t1, t2);

            Assert.Same(ref1, ref2);
        }
    }
}
