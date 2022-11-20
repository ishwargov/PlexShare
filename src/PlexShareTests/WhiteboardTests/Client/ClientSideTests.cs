using Client.Models;
using PlexShareWhiteboard.Client;
using PlexShareWhiteboard.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Threading.Tasks;
using PlexShareWhiteboard.Client.Interfaces;
using PlexShareWhiteboard.BoardComponents;
using System.Windows;
using PlexShareNetwork.Communication;

namespace PlexShareTests.WhiteboardTests.Client
{
    [Collection("Sequential")]
    public class ClientSideTests
    {
        ClientSide client;
        private Mock<IClientCommunicator> _mockCommunicator;
        ClientSnapshotHandler _snapshotHandler;
        private Serializer _serializer;
        Utility utility;
        public ClientSideTests()
        {
            client = ClientSide.Instance;
            _mockCommunicator = new Mock<IClientCommunicator>();
            //_mockSnapshotHandler = new ClientSnapshotHandler();
            utility = new Utility();
            _serializer = new Serializer();
            client.SetCommunicator(_mockCommunicator.Object);
        }
        
        [Fact]
        public void Instance_Always_ReturnsSameInstance()
        {
            ClientSide client1 = ClientSide.Instance;
            ClientSide client2 = ClientSide.Instance;
            Assert.Equal(client1, client2);
        }

        

        [Fact]
        public void ClientSendToCommunicator()
        {
            _mockCommunicator.Setup(m => m.SendToServer(It.IsAny<WBServerShape>()));
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            ShapeItem boardShape = utility.CreateShape(start, end, "EllipseGeometry", "u0_f0");
            List<ShapeItem> newShapes = new List<ShapeItem>();
            newShapes.Add(boardShape);

            var newSerializedShapes = _serializer.ConvertToSerializableShapeItem(newShapes);
            WBServerShape expected = new WBServerShape(newSerializedShapes, Operation.Creation);
            client.OnShapeReceived(boardShape, Operation.Creation);
            _mockCommunicator.Verify(m => m.SendToServer(
                It.Is<WBServerShape>(obj => utility.CompareBoardServerShapes(obj, expected))
            ), Times.Once());

        }

        [Fact]
        public void NewUserInfo_PassToCommunicator()
        {
            _mockCommunicator.Setup(m => m.SendToServer(It.IsAny<WBServerShape>()));
            client.SetUserId("2");
            client.NewUserHandler();
            WBServerShape expected = new WBServerShape(null, Operation.NewUser, "2");
            _mockCommunicator.Verify(m => m.SendToServer(
                It.Is<WBServerShape>(obj => utility.CompareBoardServerShapes(obj, expected))
            ), Times.Once());
        }
        [Fact]
        public void SnapshotNoIncrease_OnSave()
        {
            // To check increase in Snaphot no on clicking Save multiple times
            Assert.Equal(client.OnSaveMessage("1"), 1);
            Assert.Equal(client.OnSaveMessage("1"), 2);
        }

        [Fact]
        public void ClientSnapshotHandling()
        {
            _snapshotHandler = client.GetSnapshotHandler();

            // Create a Snapshot
            int currSnapshotNumber = client.OnSaveMessage("1");

            // Load that specific Snapshot
            List<ShapeItem> ls = client.OnLoadMessage(currSnapshotNumber, "1");
            Assert.Equal(currSnapshotNumber, _snapshotHandler.SnapshotNumber);

            // Setting a Snapshot Number for client
            client.SetSnapshotNumber(4);
            Assert.Equal(4, _snapshotHandler.SnapshotNumber);
        }

    }
}
