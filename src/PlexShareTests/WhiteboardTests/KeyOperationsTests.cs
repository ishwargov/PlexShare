using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlexShareWhiteboard;
//using System.Drawing;
using System.Windows;
using PlexShareScreenshare.Client;
using System.Xml.Linq;
using System.Diagnostics;
using PlexShareWhiteboard.BoardComponents;
using System.Windows.Shapes;
using Moq;
using System.Windows.Input;
using PlexShareWhiteboard.Server;

namespace PlexShareTests.WhiteboardTests
{
    [Collection("Sequential")]
    public class KeyOperationsTests
    {
        Point start = new(1, 1);
        private ServerSide server;
        public KeyOperationsTests()
        {
            server = ServerSide.Instance;
            server.ClearServerList();
        }

        //[Fact]
        public void Test1()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(20);
            viewModel.ChangeMode("create_textbox");
            start = new(15, 15);
            viewModel.ShapeStart(start);
            viewModel.ShapeFinished(new Point());
            KeyConverter k = new KeyConverter();
            Key mykey = (Key)k.ConvertFromString("H");
            viewModel.TextBoxStart(mykey);
            viewModel.ShapeStart(new Point(60, 60));
            viewModel.ShapeFinished(new Point());
            Assert.Equal("H", viewModel.textBoxLastShape.TextString);
        }
    }
}