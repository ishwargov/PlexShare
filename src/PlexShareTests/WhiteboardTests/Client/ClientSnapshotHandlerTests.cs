/***************************
 * Filename    = ClientSnapshotHandlerTests.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share Tests
 *
 * Project     = White Board Tests
 *
 * Description = Tests for ClientSnapshotHandler.cs.
 ***************************/

using Moq;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client;
using PlexShareWhiteboard.Client.Interfaces;

namespace PlexShareTests.WhiteboardTests.Client
{
    [Collection("Sequential")]
    public class ClientSnapshotHandlerTests
    {
        private ClientSnapshotHandler _clientSnapshotHandler;
        private Mock<IClientCommunicator> _mockCommunicator;
        Utility utility;
        /// <summary>
        ///     Setup of tests.
        /// </summary>
        public ClientSnapshotHandlerTests()
        {
            _clientSnapshotHandler = new ClientSnapshotHandler();
            _mockCommunicator = new Mock<IClientCommunicator>();
            _clientSnapshotHandler.SetCommunicator(_mockCommunicator.Object);
            utility = new Utility();
        }

        /// <summary>
        ///     Verifies save snapshot functionality.
        /// </summary>
        [Fact]
        public void SaveSnapshot_RequestSentToCommunicator()
        {
            _mockCommunicator.Setup(m => m.SendToServer(It.IsAny<WBServerShape>()));
            WBServerShape expected = new(null, Operation.CreateSnapshot, "randomID", 1);
            _clientSnapshotHandler.SaveSnapshot("randomID");
            _mockCommunicator.Verify(m => m.SendToServer(
                It.Is<WBServerShape>(obj => utility.CompareBoardServerShapes(obj, expected))
            ), Times.Once());
        }

        /// <summary>
        ///     Verifies restore snapshot functionality.
        /// </summary>
        [Fact]
        public void RestoreSnapshot_RequestSentToCommunicator()
        {
            _mockCommunicator.Setup(m => m.SendToServer(It.IsAny<WBServerShape>()));
            WBServerShape expected = new(null, Operation.RestoreSnapshot, "randomID", 1);
            
            _clientSnapshotHandler.SaveSnapshot("randomID");
            _clientSnapshotHandler.RestoreSnapshot(1, "randomID");
            _mockCommunicator.Verify(m => m.SendToServer(
                It.Is<WBServerShape>(obj => utility.CompareBoardServerShapes(obj, expected))
            ), Times.Once());
        }

        /// <summary>
        ///     Tests restore snapshot exception handling.
        /// </summary>
        [Fact]
        public void RestoreSnapshot_GenerateException()
        { 
            Assert.Throws<Exception>(() => _clientSnapshotHandler.RestoreSnapshot(1000, "randomID"));
        }
    }
}
