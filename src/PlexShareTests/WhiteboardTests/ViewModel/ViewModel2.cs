using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlexShareWhiteboard;
//using System.Drawing;
using System.Windows;
using System.Windows.Media;
using PlexShareScreenshare.Client;
using System.Xml.Linq;
using System.Diagnostics;
using PlexShareWhiteboard.BoardComponents;
using System.Windows.Shapes;
using NuGet.Frameworks;

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

        [Fact]
        public void DimensionChangeShape()
        {
            // shape creation
            viewModel.ChangeMode("create_rectangle");
            viewModel.ShapeStart(new Point(10, 10));
            viewModel.ShapeBuilding(new Point(20, 20));
            viewModel.ShapeFinished(new Point());

            // setting dragging edge
            viewModel.select.selectBox = 5;
            // dimension change
            viewModel.DimensionChangingShape(new Point(50, 0), viewModel.ShapeItems[0]);

            Assert.Equal(20, viewModel.ShapeItems[0].Geometry.Bounds.Height);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();

        }

        [Fact]
        public void HighlightShape()
        {
            // shape creation
            viewModel.ChangeMode("create_ellipse");
            viewModel.ShapeStart(new Point(10, 10));
            viewModel.ShapeBuilding(new Point(20, 20));
            viewModel.ShapeFinished(new Point());

            // highlighting shape
            viewModel.HighLightIt(viewModel.ShapeItems[0].Geometry.Bounds);
           
            Assert.Equal(10, viewModel.ShapeItems.Count);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();

            
        }

        [Fact]
        public void HighlightLine()
        {
            // line creation
            viewModel.ChangeMode("create_line");
            viewModel.ShapeStart(new Point(10, 10));
            viewModel.ShapeBuilding(new Point(20, 20));
            viewModel.ShapeFinished(new Point());

            // highlighting line
            LineGeometry line = (LineGeometry)viewModel.GenerateBoundingLine(viewModel.ShapeItems[0]);
            viewModel.HighLightIt(line);

            Assert.Equal(4, viewModel.ShapeItems.Count);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();


        }

        [Fact]
        public void ShapeOp()
        {
            // rectangle creation
            viewModel.ChangeMode("create_rectangle");
            viewModel.ShapeStart(new(20, 20));
            viewModel.ShapeBuilding(new(40, 40));
            viewModel.ShapeFinished(new Point());
            Rect boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check1 = (boundingBox.Width == 20);

            // selection
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(30, 30));
            bool check2 = (viewModel.select.ifSelected);
            //translation
            viewModel.ShapeBuilding(new Point(40, 40));
            viewModel.ShapeFinished(new Point());
            boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check3 = (boundingBox.X == 30 && boundingBox.Y == 30);
            bool check4 = (boundingBox.Width == 20);

            // dimension change
            // left
            viewModel.ShapeStart(new Point(30, 40));
            viewModel.ShapeBuilding(new Point(20, 40));
            viewModel.ShapeFinished(new Point());
            boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check5 = (boundingBox.Width == 30);
            //right
            viewModel.ShapeStart(new Point(50, 40));
            viewModel.ShapeBuilding(new Point(60, 40));
            viewModel.ShapeFinished(new Point());
            boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check6 = (boundingBox.Width == 40);
            //top
            viewModel.ShapeStart(new Point(40, 30));
            viewModel.ShapeBuilding(new Point(40, 20));
            viewModel.ShapeFinished(new Point());
            boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check7 = (boundingBox.Height == 30);
            //bottom
            viewModel.ShapeStart(new Point(40, 50));
            viewModel.ShapeBuilding(new Point(40, 60));
            viewModel.ShapeFinished(new Point());
            boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check8 = (boundingBox.Height == 40);

            viewModel.UnHighLightIt();
            bool check9 = (boundingBox.Height / boundingBox.Width == 1);
            bool check10 = (viewModel.ShapeItems.Count == 1);

            bool check = check1 && check2 && check3 && check4 & check5 && check6 & check7 && check8 && check9 && check10;
            Assert.True(check);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void ShapeTransform()
        {
            // rectangle creation
            viewModel.ChangeMode("create_rectangle");
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeBuilding(new Point(60, 60));
            viewModel.ShapeFinished(new Point());

            // selecting object
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(45, 45));
            viewModel.ShapeFinished(new Point());
            // transformation
            //left_top
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeBuilding(new Point(20, 20));
            viewModel.ShapeFinished(new Point());
            //right_bottom
            viewModel.ShapeStart(new Point(60, 60));
            viewModel.ShapeBuilding(new Point(70, 70));
            viewModel.ShapeFinished(new Point());
            //right_top
            viewModel.ShapeStart(new Point(70, 20));
            viewModel.ShapeBuilding(new Point(80, 10));
            viewModel.ShapeFinished(new Point());
            //left_bottom
            viewModel.ShapeStart(new Point(20, 70));
            viewModel.ShapeBuilding(new Point(10, 80));
            viewModel.ShapeFinished(new Point());
            Rect boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;

            double ratioFinal = boundingBox.Height / boundingBox.Width;
            Assert.Equal(1, ratioFinal);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        

        [Fact]
        public void TextBoxEmpty()
        {
            // create first text box
            viewModel.ChangeMode("create_textbox");
            viewModel.ShapeStart(new Point(20, 20));
            viewModel.ShapeFinished(new Point());

            // create second text box
            // since 1st box is empty, it is already deleted in the next step
            viewModel.ShapeStart(new Point(30, 30));
            viewModel.ShapeFinished(new Point());
            // second text box is deleted in the next step
            viewModel.ChangeMode("select_mode");

            Assert.Empty(viewModel.ShapeItems);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        //[Fact]
        //public void UnknownMode()
        //{
        //    viewModel.ChangeMode("unknown_mode");
        //    viewModel.ShapeStart(new Point(0, 0));
        //    viewModel.ShapeFinished(new Point());


        //}
    }
}
