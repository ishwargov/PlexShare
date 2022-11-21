/***************************
 * Filename    = MessageHandlerTests.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Tests for WBMessageHandler.cs.
 ***************************/

using PlexShareWhiteboard;
using PlexShareWhiteboard.Client.Interfaces;
using PlexShareWhiteboard.Server.Interfaces;
using Moq;
using PlexShareWhiteboard.BoardComponents;
using System.Windows;

namespace PlexShareTests.WhiteboardTests.ViewModel
{ 
    [Collection("Sequential")]
    public class MessageHandlerTests
    {
        WhiteBoardViewModel viewModel;
        Utility utility;
        
        /// <summary>
        ///     Setup for test.
        /// </summary>
        public MessageHandlerTests()
        {
            viewModel = WhiteBoardViewModel.Instance;
            utility = new Utility();

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }
        
        /// <summary>
        ///     Server Creation test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ServerCreationTest()
        {
            viewModel.isServer = true;
            viewModel.ShapeItems.Clear();

            ShapeItem sh = utility.CreateRandomShape();
            List<ShapeItem> newShapes = new List<ShapeItem>() { sh };
            string jsonString = utility.SendThroughServer(newShapes, Operation.Creation);
            viewModel.DataHandler(jsonString);
            Assert.True(utility.CompareShapeItems(viewModel.ShapeItems[0], sh));

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        /// <summary>
        ///     Server Deletion test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ServerDeletionTest()
        {
            viewModel.isServer = true;
            viewModel.ShapeItems.Clear();
            ShapeItem sh = utility.CreateRandomShape();
            viewModel.ShapeItems.Add(sh);
            List<ShapeItem> newShapes = new List<ShapeItem>() { sh };
            string jsonString = utility.SendThroughServer(newShapes, Operation.Deletion);
            viewModel.DataHandler(jsonString);

            Assert.Equal(viewModel.ShapeItems.Count, 0);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        /// <summary>
        ///     Server Modification test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ServerModifyTest()
        {
            viewModel.isServer = true;
            viewModel.ShapeItems.Clear();
            ShapeItem sh = utility.CreateRandomShape();
            viewModel.ShapeItems.Add(sh);
            sh.Start = new Point(0, 0);
            List<ShapeItem> newShapes = new List<ShapeItem>() { sh };
            string jsonString = utility.SendThroughServer(newShapes, Operation.ModifyShape);
            viewModel.DataHandler(jsonString);

            Assert.True(utility.CompareShapeItems(viewModel.ShapeItems[0], sh));
            
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        /// <summary>
        ///     Server Clear test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ServerClearTest()
        {
            viewModel.isServer = true;
            viewModel.ShapeItems.Clear();
            List<ShapeItem> shapes = utility.GenerateRandomBoardShapes(10);
            foreach (ShapeItem sh in shapes)
                viewModel.ShapeItems.Add(sh);
            List<ShapeItem> newShapes = null;
            string jsonString = utility.SendThroughServer(newShapes, Operation.Clear);
            viewModel.DataHandler(jsonString);

            Assert.Equal(viewModel.ShapeItems.Count, 0);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        /// <summary>
        ///     Server New User test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ServerNewUserTest()
        {
            viewModel.isServer = true;
            viewModel.ShapeItems.Clear();
            List<ShapeItem> newShapes = utility.GenerateRandomBoardShapes(5);
            string jsonString = utility.SendThroughServer(newShapes, Operation.NewUser);
            viewModel.DataHandler(jsonString);

            Assert.True(utility.CompareShapeItems(newShapes, viewModel.ShapeItems.ToList()));

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        /// <summary>
        ///     Client Creation test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ClientCreationTest()
        {
            viewModel.ShapeItems.Clear();
            viewModel.isServer = false;

            ShapeItem sh = utility.CreateRandomShape();
            List<ShapeItem> newShapes = new List<ShapeItem>() { sh };
            string jsonString = utility.SendThroughServer(newShapes, Operation.Creation);
            viewModel.DataHandler(jsonString);
            Assert.True(utility.CompareShapeItems(viewModel.ShapeItems[0], sh));

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        /// <summary>
        ///     Client Deletion test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ClientDeletionTest()
        { 
            viewModel.isServer = false;
            viewModel.ShapeItems.Clear();
            ShapeItem sh = utility.CreateRandomShape();
            viewModel.ShapeItems.Add(sh);
            List<ShapeItem> newShapes = new List<ShapeItem>() { sh };
            string jsonString = utility.SendThroughServer(newShapes, Operation.Deletion);
            viewModel.DataHandler(jsonString);

            Assert.Equal(viewModel.ShapeItems.Count, 0);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        /// <summary>
        ///     Client Modification test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ClientModifyTest()
        {
            viewModel.isServer = false;
            viewModel.ShapeItems.Clear();
            ShapeItem sh = utility.CreateRandomShape();
            viewModel.ShapeItems.Add(sh);
            sh.Start = new Point(0, 0);
            List<ShapeItem> newShapes = new List<ShapeItem>() { sh };
            string jsonString = utility.SendThroughServer(newShapes, Operation.ModifyShape);
            viewModel.DataHandler(jsonString);

            Assert.True(utility.CompareShapeItems(viewModel.ShapeItems[0], sh));

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        /// <summary>
        ///     Client Clear test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ClientClearTest()
        {
            viewModel.isServer = false;
            viewModel.ShapeItems.Clear();
            List<ShapeItem> shapes = utility.GenerateRandomBoardShapes(10);
            foreach (ShapeItem sh in shapes)
                viewModel.ShapeItems.Add(sh);
            List<ShapeItem> newShapes = null;
            string jsonString = utility.SendThroughServer(newShapes, Operation.Clear);
            viewModel.DataHandler(jsonString);

            Assert.Equal(viewModel.ShapeItems.Count, 0);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        /// <summary>
        ///     Client New User test.
        /// </summary>
        [Fact]
        public void OnDataReceived_ClientNewUserTest()
        {
            viewModel.ShapeItems.Clear();
            viewModel.isServer = false;
            List<ShapeItem> newShapes = utility.GenerateRandomBoardShapes(5);
            string jsonString = utility.SendThroughServer(newShapes, Operation.NewUser);
            viewModel.DataHandler(jsonString);

            Assert.True(utility.CompareShapeItems(newShapes, viewModel.ShapeItems.ToList()));

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }
    }
}
