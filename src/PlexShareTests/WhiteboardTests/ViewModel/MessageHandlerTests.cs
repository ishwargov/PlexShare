using Microsoft.Identity.Client;
using PlexShareWhiteboard;
using PlexShareWhiteboard.Client.Interfaces;
using PlexShareWhiteboard.Client;
using PlexShareWhiteboard.Server.Interfaces;
using PlexShareWhiteboard.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using PlexShareWhiteboard.BoardComponents;
using System.Windows;

namespace PlexShareTests.WhiteboardTests.ViewModel
{ 
    [Collection("Sequential")]
    public class MessageHandlerTests
    {
        private WhiteBoardViewModel viewModel;
        private Mock<IClientCommunicator> _mockClientCommunicator;
        private Mock<IServerCommunicator> _mockServerCommunicator;
        private Serializer _serializer;

        public MessageHandlerTests()
        {
            //viewModel.Setup(m => m.LoadBoard(It.IsAny<List<ShapeItem>>(), It.IsAny<bool>()));
            //viewModel.Setup(m => m.UpdateCheckList(It.IsAny<int>()));
            //viewModel.Setup(m => m.DeleteIncomingShape(It.IsAny<ShapeItem>()));
            //viewModel.Setup(m => m.CreateIncomingShape(It.IsAny<ShapeItem>()));
            //viewModel.Setup(m => m.ModifyIncomingShape(It.IsAny<ShapeItem>()));
            _mockClientCommunicator = new Mock<IClientCommunicator>();
            _mockServerCommunicator = new Mock<IServerCommunicator>();
            viewModel = WhiteBoardViewModel.Instance;
            _mockClientCommunicator.Setup(m => m.SendToServer(It.IsAny<WBServerShape>()));
            _mockServerCommunicator.Setup(m => m.Broadcast(It.IsAny<WBServerShape>(), It.IsAny<string>()));
            _serializer = new Serializer();
        }

        [Fact]
        public void OnDataReceived_Server_CreationTest()
        {
            viewModel.isServer = true;
            viewModel.ShapeItems.Clear();

            Point start = new Point(0, 0);
            Point end = new Point(5, 5);
            ShapeItem sh = Utility.CreateShape(start, end, "EllipseGeometry", "randomID");
            List<ShapeItem> newShapes = new List<ShapeItem>() { sh };
            var newSerializedShapes = _serializer.ConvertToSerializableShapeItem(newShapes);
            WBServerShape wbShape = new WBServerShape(newSerializedShapes, Operation.Creation);
            string jsonString = _serializer.SerializeWBServerShape(wbShape);
            viewModel.OnDataReceived(jsonString);

            Assert.Equal(viewModel.ShapeItems[0], sh);
        }
    }
}
