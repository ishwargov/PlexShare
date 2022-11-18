using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlexShareWhiteboard;
//using System.Drawing;
using System.Windows;
using PlexShareScreenshare.Client;
using System.Xml.Linq;
using System.Diagnostics;
using PlexShareWhiteboard.BoardComponents;
using System.Windows.Shapes;

namespace PlexShareTests.WhiteboardTests.ViewModel
{
    [Collection("Sequential")]
    public class ViewModel2
    {

        WhiteBoardViewModel viewModel;
        public ViewModel2()
        {
            viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(6);
        }
        //[Fact]
        //public void Test6()
        //{
            

        //    viewModel.ChangeMode("create_rectangle");
        //    Point start = new(10, 10);
        //    Point end = new(20, 20);
        //    viewModel.ShapeStart(start);
        //    viewModel.ShapeBuilding(end);
        //    viewModel.ShapeFinished(new Point());
        //    viewModel.select.selectBox = 5;
        //    viewModel.DimensionChangingShape(new Point(50, 0), viewModel.ShapeItems[0]);
        //    Assert.Equal(viewModel.ShapeItems[0].Geometry.Bounds.Height, 20);
        //    viewModel.ShapeItems.Clear();
        //}

        [Fact]
        public void Test10()
        {

            viewModel.ChangeMode("create_rectangle");
            Point start = new(10, 10);
            Point end = new(20, 20);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());
            viewModel.select.selectBox = 5;
            viewModel.DimensionChangingShape(new Point(50, 0), viewModel.ShapeItems[0]);
            Assert.Equal(viewModel.ShapeItems[0].Geometry.Bounds.Height, 20);
            viewModel.ShapeItems.Clear();
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();

        }

        [Fact]
        public void Highlight()
        {

            Point start = new(10, 10);
            Point end = new(20, 20);

            viewModel.ChangeMode("create_ellipse");
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());
            viewModel.HighLightIt(viewModel.ShapeItems[0].Geometry.Bounds);

            Assert.Equal(10, viewModel.ShapeItems.Count);
            viewModel.ShapeItems.Clear();
            //viewModel = null;
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }
    }
}
