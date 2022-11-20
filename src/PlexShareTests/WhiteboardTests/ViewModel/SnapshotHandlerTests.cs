using Moq;
using PlexShareWhiteboard;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.WhiteboardTests.ViewModel
{
    [Collection("Sequential")]
    public class SnapshotHandlerTests
    {
        WhiteBoardViewModel viewModel;
        Utility utility;
        Mock<IShapeListener> _mockMachine;

        public SnapshotHandlerTests()
        {
            viewModel = WhiteBoardViewModel.Instance;
            utility = new Utility();
            _mockMachine = new Mock<IShapeListener>();
            viewModel.SetMachine(_mockMachine.Object);
        }

        [Fact]
        public void SaveSnapshot_Test()
        {
            _mockMachine.Setup(m => m.OnSaveMessage(It.IsAny<string>())).Returns(1);
            viewModel.SaveSnapshot();
            ObservableCollection<int> expectedCheckList = new ObservableCollection<int>() { 1 };
            Assert.Equal(expectedCheckList, viewModel.CheckList);
        }
        [Fact]
        public void LoadSnapshot_Test()
        {
            viewModel.isServer = true;
            List<ShapeItem> shapeItems = utility.GenerateRandomBoardShapes(5);
            _mockMachine.Setup(m => m.OnLoadMessage(1, It.IsAny<string>())).Returns(shapeItems);
            viewModel.LoadSnapshot(1);
            Assert.True(utility.CompareShapeItems(shapeItems, viewModel.ShapeItems.ToList()));
        }
    }
}
