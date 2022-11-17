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

namespace PlexShareTests.WhiteboardTests
{
    public class LineOperationsTests
    {
        Point start = new(1, 1);
        Point end = new(5, 5);

        //[Fact]
        public void Test1()
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
            //viewModel.ObjectSelection(new Point(30, 30));
            Assert.Equal("LineGeometry", viewModel.select.selectedObject.Geometry.GetType().Name);
        }

        //[Fact]
        public void Test2()
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
            viewModel.ShapeStart(new Point(45, 45));
            viewModel.ShapeBuilding(new Point(65, 65));
            viewModel.ShapeFinished(new Point());
            
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(45, 45));
            
            Assert.Equal(viewModel.select.selectedObject.End, new Point(65, 65));

        }

        //[Fact]
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
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeBuilding(new Point(50, 30));

            Assert.Equal(viewModel.lastShape.Start, new Point(35, 15));

        }
    }
}
