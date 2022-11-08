/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using PlexShareNetwork.Communication;
using PlexShareNetwork;

namespace PlexShareNetwork.Communication.Test
{
    public class CommunicationFactoryTests
    {
        [Fact]
        public void GetClientCommunicatorTest()
        {
            ICommunicator clientCommunicator = CommunicationFactory.GetCommunicator(true);
            clientCommunicator.Start("0", "0");
            clientCommunicator.Stop();
        }

        [Fact]
        public void GetServerCommunicatorTest()
        {
            ICommunicator serverCommunicator = CommunicationFactory.GetCommunicator(false);
            serverCommunicator.Start();
            serverCommunicator.Stop();
        }
    }
}
