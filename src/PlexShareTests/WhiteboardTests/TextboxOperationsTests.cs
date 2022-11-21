/***************************
 * Filename    = TextboxOperationsTests.cs
 *
 * Author      = Deon Saji
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This implements textbox operations tests
 ***************************/
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

    public class TextBoxOperationsTests
    {
        Point start = new(1, 1);
        private ServerSide server;
        public TextBoxOperationsTests()
        {
            server = ServerSide.Instance;
            server.ClearServerList();
        }
        [Fact]
        public void Test10()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(20);
            //Start writing from (15,15) 
            viewModel.ChangeMode("create_textbox");
            start = new(15, 15);
            viewModel.ShapeStart(start);
            viewModel.ShapeFinished(new Point());
            viewModel.TextBoxStartChar('(');
            viewModel.TextBoxStartChar('H');
            viewModel.TextBoxStartChar('e');
            viewModel.TextBoxStartChar('l');
            viewModel.TextBoxStartChar('l');
            viewModel.TextBoxStartChar('o');
            viewModel.TextBoxStartChar('!');
            viewModel.TextBoxStartChar('!');
            viewModel.TextBoxStartChar(' ');
            viewModel.TextBoxStartChar('1');
            viewModel.TextBoxStartChar('2');
            viewModel.TextBoxStartChar('3');
            viewModel.TextBoxStartChar('*');
            viewModel.TextBoxStartChar('@');
            viewModel.TextBoxStartChar('#');
            viewModel.TextBoxStartChar(')');

            Assert.Equal("(Hello!! 123*@#)", viewModel.textBoxLastShape.TextString);
            viewModel.ShapeStart(new Point(60, 60));
            viewModel.ShapeFinished(new Point());
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
            server.ClearServerList();

        }

        [Fact]
        public void Test11()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(50);

            viewModel.ChangeMode("create_textbox");
            start = new(45, 45);
            viewModel.ShapeStart(start);
            viewModel.ShapeFinished(new Point());
            viewModel.TextBoxStartChar('T');
            viewModel.TextBoxStartChar('E');
            viewModel.TextBoxStartChar('X');
            viewModel.TextBoxStartChar('T');
            viewModel.TextBoxStartChar('+');
            viewModel.TextBoxStartChar(']');
            //Check backspace
            Assert.Equal("TEXT+]", viewModel.textBoxLastShape.TextString);
            viewModel.TextBoxStartChar('\b');
            Assert.Equal("TEXT+", viewModel.textBoxLastShape.TextString);
            viewModel.TextBoxStartChar('\b');
            Assert.Equal("TEXT", viewModel.textBoxLastShape.TextString);
            viewModel.TextBoxStartChar(':');
            Assert.Equal("TEXT:", viewModel.textBoxLastShape.TextString);
            viewModel.ShapeStart(new Point(10, 10));
            viewModel.ShapeFinished(new Point());
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
            server.ClearServerList();

        }

        [Fact]
        public void Test12()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(50);

            viewModel.ChangeMode("create_textbox");
            start = new(70, 70);
            viewModel.ShapeStart(start);
            viewModel.ShapeFinished(new Point());
            viewModel.TextBoxStartChar('a');
            viewModel.TextBoxStartChar('b');
            viewModel.TextBoxStartChar('c');
            //On changing mode from textbox to rectangle, the text pushed to the undo stack   
            viewModel.ChangeMode("create_rectangle");
            Assert.Equal("abc", viewModel.undoStack.Peek().NewShape.TextString);
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(70, 10));
            viewModel.ShapeFinished(new Point());
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
            server.ClearServerList();

        }

        [Fact]
        public void Test13()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(2);

            viewModel.ChangeMode("create_textbox");
            start = new(15, 15);
            viewModel.ShapeStart(start);
            viewModel.ShapeFinished(new Point());
            viewModel.TextBoxStartChar('a');
            viewModel.TextBoxStartChar('b');
            viewModel.TextBoxStartChar('c');
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeFinished(new Point());

            viewModel.ChangeMode("create_rectangle");
            Assert.Equal("abc", viewModel.undoStack.Peek().NewShape.TextString);

            viewModel.ChangeMode("create_textbox");
            start = new(45, 45);
            viewModel.ShapeStart(start);
            viewModel.ShapeFinished(new Point());
            viewModel.TextBoxStartChar('U');
            viewModel.TextBoxStartChar('V');
            viewModel.TextBoxStartChar('W');
            viewModel.TextBoxStartChar('X');
            viewModel.TextBoxStartChar('Y');
            viewModel.TextBoxStartChar('Z');
            //Translate textbox and select . 
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(48, 48));
            Assert.Equal(viewModel.select.selectedObject.Geometry.GetType().Name, "GeometryGroup");
            viewModel.ShapeBuilding(new Point(60, 60));
            viewModel.ShapeFinished(new Point());
            viewModel.ShapeStart(new Point(48, 48));
            viewModel.ShapeFinished(new Point());

            Assert.Equal(viewModel.textBoxLastShape.Start, new Point(57, 57));
            UndoStackElement popFromUndo = viewModel.Undo();
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(48, 48));
            Assert.Equal(viewModel.select.selectedObject.Start, new Point(45, 45));
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
            server.ClearServerList();


        }

    }
}












