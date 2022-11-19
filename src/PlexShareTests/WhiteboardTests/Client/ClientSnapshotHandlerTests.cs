using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ClientSnapshotHandlerTests()
        {
            _clientSnapshotHandler = new ClientSnapshotHandler();
            _mockCommunicator = new Mock<IClientCommunicator>();
            _clientSnapshotHandler.SetCommunicator(_mockCommunicator.Object);
            utility = new Utility();
        }

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
        [Fact]
        public void RestoreSnapshot_GenerateException()
        { 
            Assert.Throws<Exception>(() => _clientSnapshotHandler.RestoreSnapshot(1000, "randomID"));
        }
    }
}
