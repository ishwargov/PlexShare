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
    public class ViewModel1    
    {
        WhiteBoardViewModel viewModel;
        public ViewModel1()
        {
            viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(3);

        }

        [Fact]
        public void Test1()
        {
            int s = 10;
            int l = 2;
            int c = 3;
            int d = 2;
            Point start;
            Point end;

            //Point translatePoint, transformPoint, dimensionChangePoint;

            viewModel.ChangeMode("create_rectangle");

            for (int i = 0; i < s; ++i)
            {
                start = new(i + 10, i + 10);
                end = new(i + 20, i + 20);
                viewModel.ShapeStart(start);
                viewModel.ShapeBuilding(end);
                viewModel.ShapeFinished(new Point());
            }

            viewModel.ChangeMode("create_line");
            for (int i = 0; i < l; ++i)
            {
                start = new(i + 15, i + 15);
                end = new(i + 30, i + 30);
                viewModel.ShapeStart(start);
                viewModel.ShapeBuilding(end);
                viewModel.ShapeFinished(new Point());
            }

            viewModel.ChangeMode("create_freehand");

            for (int i = 0; i < c; ++i)
            {
                start = new(i + 40, i + 40);
                end = new(i + 50, i + 50);
                viewModel.ShapeStart(start);
                viewModel.ShapeBuilding(end);
                viewModel.ShapeFinished(new Point());
            }

            viewModel.ChangeMode("delete_mode");
            ShapeItem toDelete = viewModel.ShapeItems[^1];

            for (int i = 0; i < d; ++i)
                viewModel.ShapeStart(new Point(45, 45));

            viewModel.DeleteIncomingShape(toDelete);
            viewModel.CreateIncomingShape(viewModel.ShapeItems[0]);

            Assert.Equal(s + l + c - d, viewModel.ShapeItems.Count);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();

        }

        [Fact]
        public void Test2()
        {
            Point start = new(40, 40);
            Point end = new(50, 50);
            viewModel.ChangeMode("create_freehand");
            viewModel.lastShape = viewModel.CreateCurve(start);
            viewModel.ShapeItems.Add(viewModel.lastShape);
            viewModel.UpdateCurve(end, start);

            viewModel.ObjectSelection(new Point(45, 45));

            Assert.True(viewModel.select.ifSelected);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }



        [Fact]
        public void Test3()
        {

            viewModel.ChangeMode("create_ellipse");
            Point start = new(10, 10);
            Point end = new(20, 20);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());

            string name = viewModel.ShapeItems[0].Geometry.GetType().Name;

            Assert.Equal("EllipseGeometry", name);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void Test4()
        {
            int s = 10;
            viewModel.ChangeMode("create_rectangle");

            for (int i = 0; i < s; ++i)
            {
                Point start = new(i + 10, i + 10);
                Point end = new(i + 20, i + 20);
                viewModel.ShapeStart(start);
                viewModel.ShapeBuilding(end);
                viewModel.ShapeFinished(new Point());
            }
            viewModel.ClearAllShapes();

            Assert.Empty(viewModel.ShapeItems);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();

        }

        [Fact]
        public void Test5()
        {

            viewModel.ChangeMode("create_ellipse");
            Point start = new(10, 10);
            Point end = new(20, 20);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());

            //viewModel.ChangeMode("select_mode
            //viewModel.select.ifSelected = true;
            //viewModel.select.selectedObject = ;
            //viewModel.TranslatingCurve(viewModel.ShapeItems[0], 40, 50, new Point(40, 50));
            viewModel.TranslatingShape(viewModel.ShapeItems[0], new Point(40, 50), new Point(50, 60));

            Assert.Equal(40, viewModel.ShapeItems[0].Geometry.Bounds.X);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void Test6()
        {
            viewModel.ChangeMode("create_rectangle");
            viewModel.ShapeStart(new(10, 10));
            viewModel.ShapeBuilding(new(40, 40));
            viewModel.ShapeFinished(new Point());

            int thickness = viewModel.ShapeItems[0].StrokeThickness;
            int c = 2;
            viewModel.ChangeStrokeThickness(thickness + c);

            viewModel.ChangeMode("create_rectangle");
            viewModel.ShapeStart(new(10, 10));
            viewModel.ShapeBuilding(new(40, 40));
            viewModel.ShapeFinished(new Point());

            int diff = viewModel.ShapeItems[1].StrokeThickness - thickness;

            Assert.Equal(c, diff);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void Test7()
        {
            viewModel.ChangeMode("create_line");
            Point start = new(25, 30);
            viewModel.ShapeStart(start);

            Assert.NotNull(viewModel.lastShape);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void Test8()
        {
            viewModel.ChangeMode("create_rectangle");
            Point start = new(10, 10);
            viewModel.ShapeStart(start);
            viewModel.ShapeStart(start);

            Assert.True(viewModel.ShapeItems[1].ZIndex > viewModel.ShapeItems[0].ZIndex);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void Test9()
        {

            viewModel.ChangeMode("create_freehand");
            Point start = new(0, 0);
            Point end = new(15, 20);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());

            viewModel.select.ifSelected = true;
            viewModel.select.selectedObject = viewModel.ShapeItems[0];
            Rect boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            double ratio = boundingBox.Width / boundingBox.Height;
            viewModel.TransformCurve(new Point(90, 50), viewModel.ShapeItems[0]);
            boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            double ratioFinal = boundingBox.Width / boundingBox.Height;
            int ratioTemp = (int)(ratio * 100);
            int ratioFinalTemp = (int)(ratioFinal * 100);

            Assert.Equal(ratioTemp, ratioFinalTemp);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }



    }
}
