using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Moq;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client;
using PlexShareWhiteboard.Client.Interfaces;
using PlexShareWhiteboard.Server;
using PlexShareWhiteboard.Server.Interfaces;

namespace PlexShareTests.WhiteboardTests.Server
{
    [Collection("Sequential")]
    public class ServerSideTests
    {
        private ServerSide server;
        private ServerSnapshotHandler _snapshotHandler;
        private Serializer _serializer;
        Utility utility;
        public ServerSideTests()
        {
            server = ServerSide.Instance;
            _serializer = new Serializer();
            utility = new Utility();
        }

        [Fact]
        public void Clear_ServerListSizeZero()
        {
            server.OnShapeReceived(null, Operation.Clear);
            Assert.Equal(0, server.GetServerListSize());
            server.ClearServerList();
        }
        [Fact]
        public void Instance_Always_ReturnsSameInstance()
        {
            ServerSide server1 = ServerSide.Instance;
            ServerSide server2 = ServerSide.Instance;
            Assert.Equal(server1, server2);
        }

        [Fact]
        public void CreateTwoShapes_ServerListSizeIncrease()
        {
            server.ClearServerList();
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);

            Point start1 = new Point(10, 10);
            Point end1 = new Point(20, 20);

            server.OnShapeReceived(utility.CreateShape(start, end, "RectangleGeometry", "u0_f1"), Operation.Creation);
            server.OnShapeReceived(utility.CreateShape(start1, end1, "RectangleGeometry", "u0_f2"), Operation.Creation);
            Assert.Equal(2, server.GetServerListSize());
        }

        [Fact]
        public void RemoveShape_Working()
        {
            server.ClearServerList();
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);

            server.OnShapeReceived(utility.CreateShape(start, end, "RectangleGeometry", "u0_f1"), Operation.Creation);
            Assert.Equal(1, server.GetServerListSize());

            // Trying to remove non existent object, size remains 1
            server.OnShapeReceived(utility.CreateShape(start, end, "RectangleGeometry", "u1_f1"), Operation.Deletion);
            Assert.Equal(1, server.GetServerListSize());

            // Trying to remove the initially created object , size becomes 0
            server.OnShapeReceived(utility.CreateShape(start, end, "RectangleGeometry", "u0_f1"), Operation.Deletion);
            Assert.Equal(0, server.GetServerListSize());

        }

        [Fact]
        public void SaveAndRestoreSnapShot()
        {
            server.ClearServerList();
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);

            // Adding two shapes to Server List
            ShapeItem shape1 = utility.CreateShape(start, end, "RectangleGeometry", "u0_f1");
            ShapeItem shape2 = utility.CreateShape(start, end, "lineGeometry", "u1_f1");
            server.OnShapeReceived(shape1, Operation.Creation);
            server.OnShapeReceived(shape2, Operation.Creation);
            Assert.Equal(2, server.GetServerListSize());
            
            // Saving Snapshot
            Assert.Equal(1,server.OnSaveMessage("1"));

            // Expected Saved shapes in Snapshot no 1
            List<ShapeItem> expected = new List<ShapeItem>();
            expected.Add(shape1);
            expected.Add(shape2);

            // Restoring snapshot no 1 and comparing with expected value
            List<ShapeItem> savedShapes = server.OnLoadMessage(1, "1");
            Assert.True(utility.CompareShapeItems(savedShapes, expected));
           
        }

        [Fact]
        public void SetServerSnapshotNumber()
        {
            server.ClearServerList();
            _snapshotHandler = server.GetSnapshotHandler();
            server.SetSnapshotNumber(3);
            Assert.Equal(3,_snapshotHandler.SnapshotNumber);
        }

    }
}
