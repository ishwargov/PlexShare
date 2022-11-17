using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlexShareWhiteboard.Server;
using PlexShareWhiteboard.BoardComponents;
using Microsoft.AspNetCore.Routing;

namespace PlexShareTests.WhiteboardTests.Server
{
    [Collection("Sequential")]
    public class ServerSnapshotHandlerTests
    {
        private ServerSnapshotHandler _serverSnapshotHandler;
        private List<Tuple<int, string, List<ShapeItem>>> _snapshotSummary;
        public ServerSnapshotHandlerTests()
        {
            _serverSnapshotHandler = new ServerSnapshotHandler();
            _snapshotSummary = new List<Tuple<int, string, List<ShapeItem>>>();
            for (var i = 1; i <= 4; i++)
            {
                List<ShapeItem> shapeItems = Utility.GenerateRandomBoardShapes(i);
                string userID = Utility.RandomString(5);
                _snapshotSummary.Add(
                    new Tuple<int, string, List<ShapeItem>>(i, userID, shapeItems));

                _serverSnapshotHandler.SaveBoard(shapeItems, userID);
            }
        }
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
        [Fact]
        public void GetSnapshotNumberTest()
        {
            var checkpointNumbers = _serverSnapshotHandler.SnapshotNumber;
            Assert.Equal(4, checkpointNumbers);
        }

        [Fact]
        public void LoadBoardTest()
        {
            List<List<ShapeItem>> loadedShapeItems = new();
            for (var i = 1; i <= 4; i++)
                loadedShapeItems.Add(_serverSnapshotHandler.LoadBoard(i));

            Boolean flag = true;

            for (var i = 0; i < 4; i++)
                if (!Utility.CompareShapeItems(loadedShapeItems[i], _snapshotSummary[i].Item3))
                    flag = false;

            Assert.True(flag);
        }

        [Fact]
        public void LoadBoard_FailTest()
        {
            var boardShape = _serverSnapshotHandler.LoadBoard(5);
            Assert.Null(boardShape);
        }
    }
}
