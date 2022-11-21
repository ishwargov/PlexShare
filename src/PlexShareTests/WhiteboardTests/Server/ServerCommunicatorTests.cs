/***************************
 * Filename    = ServerCommunicatorTests.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Tests for ServerCommunicator.cs.
 ***************************/

using PlexShareWhiteboard.Server;

namespace PlexShareTests.WhiteboardTests.Server
{
    [Collection("Sequential")]
    public class ServerCommunicatorTests
    {
        /// <summary>
        ///     Verifies same instance generation.
        /// </summary>
        [Fact]
        public void Instance_Always_ReturnsSameInstance()
        {
            ServerCommunicator comm1 = ServerCommunicator.Instance;
            ServerCommunicator comm2 = ServerCommunicator.Instance;
            Assert.Equal(comm1, comm2);
        }
    }
}
