/***************************
 * Filename    = ClientCommunicatorTests.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share Tests
 *
 * Project     = White Board Tests
 *
 * Description = Tests for ClientCommunicator.cs.
 ***************************/

using PlexShareWhiteboard.Client;

namespace PlexShareTests.WhiteboardTests.Client
{
    [Collection("Sequential")]
    public class ClientCommunicatorTests
    {

        /// <summary>
        ///     Checks for same instance.
        /// </summary>
        [Fact]
        public void Instance_Always_ReturnsSameInstance()
        {
            ClientCommunicator comm1 = ClientCommunicator.Instance;
            ClientCommunicator comm2 = ClientCommunicator.Instance;
            Assert.Equal(comm1, comm2);
        }
        
    }
}