using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlexShareWhiteboard;
using System.Windows;
using PlexShareScreenshare.Client;
using System.Xml.Linq;
using System.Diagnostics;
using PlexShareWhiteboard.BoardComponents;
using System.Windows.Shapes;
using PlexShareWhiteboard.Server;

namespace PlexShareTests.WhiteboardTests
{
    [Collection("Sequential")]
    public class LineOperationsTests
    {
        Point start = new(1, 1);
        Point end = new(5, 5);
        private ServerSide server;
        public LineOperationsTests()
        {
            server = ServerSide.Instance;
            server.ClearServerList();
        }
       
        [Fact]
        public void Test1()
        {

            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.ShapeItems.Clear();
            viewModel.SetUserId(20);
            
            //Create a line (15,15) and (15,45) 
            viewModel.ChangeMode("create_line");
            start = new(15, 15);
            end = new(15,45);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());

            //Select the line
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(15, 22));
            viewModel.ShapeFinished(new Point());

            Assert.Equal("LineGeometry", viewModel.select.selectedObject.Geometry.GetType().Name);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
            server.ClearServerList();

        }

        [Fact]
        public void Test2()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(2);
            //Create line 
            viewModel.ChangeMode("create_line");
            start = new(15, 15);
            end = new(45, 45);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());
            
            //Select the line
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeFinished(new Point());
            //Transform the line keeping start point as pivot 
            viewModel.ShapeStart(new Point(45, 45));
            viewModel.ShapeBuilding(new Point(65, 65));
            viewModel.ShapeFinished(new Point());

            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(45, 45));
            
            //Assert
            Assert.Equal(viewModel.select.selectedObject.End, new Point(65, 65));
            
                      
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
            server.ClearServerList();

        }

        [Fact]
        public void Test3()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(2);

            viewModel.ChangeMode("create_line");
            start = new(15, 15);
            end = new(45, 45);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());

            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeFinished(new Point());
            //Translate 
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeBuilding(new Point(50, 30));

            Assert.Equal(viewModel.lastShape.Start, new Point(35, 15));
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
            server.ClearServerList();

        }

        [Fact]
        public void Test4()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(50);

            viewModel.ChangeMode("create_line");
            start = new(15, 15);
            end = new(45, 45);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());

            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeFinished(new Point());
            //Transofrm the line keeping end point as pivot
            viewModel.ShapeStart(new Point(15, 15));
            viewModel.ShapeBuilding(new Point(10, 10));
            viewModel.ShapeFinished(new Point());

            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(45, 45));

            Assert.Equal(viewModel.select.selectedObject.Start, new Point(10, 10));


            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
            server.ClearServerList();

        }

        [Fact]
        public void Test5()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(32);
            //Checking translation of lines of the kind '/'
            viewModel.ChangeMode("create_line");
            start = new(15, 45);
            end = new(45, 15);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());

            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeBuilding(new Point(50, 50));
            viewModel.ShapeFinished(new Point());

            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(50, 50));

            Assert.Equal(viewModel.select.selectedObject.Start, new Point(35, 65));

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
            server.ClearServerList();

        }


    }
}
