using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Server;

namespace PlexShareTests.WhiteboardTests.Server
{
    [Collection("Sequential")]
    public class ServerSideTests
    {
        private ServerSide server;
        Utility utility;
        public ServerSideTests()
        {
            server = ServerSide.Instance;
            server.ClearServerList();
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

            //server.OnShapeReceived(Utility.CreateShape(start, end, "EllipseGeometry", "u0_f0"), Operation.Creation);
            //server.OnShapeReceived(Utility.CreateShape(start, end, "RectangleGeometry", "u0_f1"), Operation.Creation);
            server.OnShapeReceived(utility.CreateShape(start, end, "RectangleGeometry", "u0_f1"), Operation.Creation);
            server.OnShapeReceived(utility.CreateShape(start1, end1, "RectangleGeometry", "u0_f2"), Operation.Creation);
            Assert.Equal(server.GetServerListSize(), 2);
        }


    }
}
