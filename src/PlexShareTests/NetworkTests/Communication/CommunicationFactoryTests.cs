using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareNetwork.Communication.Test
{
    public class CommunicationFactoryTests
    {
        [Fact]
        public void GetCommunicatorClientTest()
        {
            ICommunicator communicatorClient = CommunicationFactory.GetCommunicator(true);
            string returnString = communicatorClient.Start("0", "0");
            Assert.Equal("failure", returnString);
            communicatorClient.Stop();
        }

        [Fact]
        public void GetCommunicatorServerTest()
        {
            ICommunicator communicatorServer = CommunicationFactory.GetCommunicator(false);
            string returnString = communicatorServer.Start();
            Assert.NotEqual("success", returnString);
            Assert.NotEqual("failure", returnString);
            communicatorServer.Stop();
        }
    }
}
