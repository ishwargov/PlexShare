using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Server;

namespace PlexShareTests.WhiteboardTests.Server
{

    public class ServerSideTests
    {
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
            ServerSide server = ServerSide.Instance;
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            server.OnShapeReceived(Utility.CreateShape(start, end, "EllipseGeometry", "u0_f0"), Operation.Creation);
            server.OnShapeReceived(Utility.CreateShape(start, end, "RectangleGeometry", "u0_f1"), Operation.Creation);

            Assert.Equal(server.GetServerListSize(), 2);

        }

    }
}
