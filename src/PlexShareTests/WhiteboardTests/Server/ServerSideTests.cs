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
        public ServerSideTests()
        {
            server = ServerSide.Instance;
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
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            server.OnShapeReceived(Utility.CreateShape(start, end, "EllipseGeometry", "u0_f0"), Operation.Creation);
            server.OnShapeReceived(Utility.CreateShape(start, end, "RectangleGeometry", "u0_f1"), Operation.Creation);

            Assert.Equal(server.GetServerListSize(), 2);
        }

        [Fact]
        public void Clear_ServerListSizeZero()
        {
            server.OnShapeReceived(null, Operation.Clear);
            Assert.Equal(server.GetServerListSize(), 0);
        }
    }
}
