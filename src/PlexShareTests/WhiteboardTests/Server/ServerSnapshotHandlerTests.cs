/***************************
 * Filename    = ServerSnapshotHandlerTests.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Tests for ServerSnapshotHandler.cs.
 ***************************/

using PlexShareWhiteboard.Server;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareTests.WhiteboardTests.Server
{
    [Collection("Sequential")]
    public class ServerSnapshotHandlerTests
    {
        private ServerSnapshotHandler _serverSnapshotHandler;
        private List<Tuple<int, string, List<ShapeItem>>> _snapshotSummary;
        Utility utility;

        /// <summary>
        ///     Setup for tests.
        /// </summary>
        public ServerSnapshotHandlerTests()
        {
            _serverSnapshotHandler = new ServerSnapshotHandler();
            utility = new Utility();
            _snapshotSummary = new List<Tuple<int, string, List<ShapeItem>>>();
            for (var i = 1; i <= 4; i++)
            {
                List<ShapeItem> shapeItems = utility.GenerateRandomBoardShapes(i);
                string userID = utility.RandomString(5);
                _snapshotSummary.Add(
                    new Tuple<int, string, List<ShapeItem>>(i, userID, shapeItems));

                _serverSnapshotHandler.SaveBoard(shapeItems, userID);
            }
        }
        /// <summary>
        ///     Test for snapshot saving. Checks if file gets saved locally.
        /// </summary>
        [Fact]
        public void SaveSnapshotTest()
        {
            Boolean flag = true;
            for (var i = 1; i <= 4; i++)
            {
                var filePath = i + ".json";
                if (!File.Exists(filePath))
                {
                    flag = false;
                    break;
                }
            }
            Assert.True(flag);
        }

        /// <summary>
        ///     Checks for correct snapshot number.
        /// </summary>
        [Fact]
        public void GetSnapshotNumberTest()
        {
            var checkpointNumbers = _serverSnapshotHandler.SnapshotNumber;
            Assert.Equal(4, checkpointNumbers);
        }

        /// <summary>
        ///     Verifies functionality of LoadBoard.
        /// </summary>
        [Fact]
        public void LoadBoardTest()
        {
            List<List<ShapeItem>> loadedShapeItems = new();
            for (var i = 1; i <= 4; i++)
                loadedShapeItems.Add(_serverSnapshotHandler.LoadBoard(i));

            Boolean flag = true;

            for (var i = 0; i < 4; i++)
                if (!utility.CompareShapeItems(loadedShapeItems[i], _snapshotSummary[i].Item3))
                    flag = false;

            Assert.True(flag);
        }

        /// <summary>
        ///     Verifies null handling of LoadBoard.
        /// </summary>
        [Fact]
        public void LoadBoard_FailTest()
        {
            var boardShape = _serverSnapshotHandler.LoadBoard(5);
            Assert.Null(boardShape);
        }
    }
}
