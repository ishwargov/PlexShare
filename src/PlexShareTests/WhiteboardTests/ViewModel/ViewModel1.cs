/********************************************************************************
 * Filename    = ViewModel1.cs
 *
 * Author      = Asha Jose
 *
 * Product     = Plex Share Tests
 * 
 * Project     = White Board Tests
 *
 * Description = This is testing the view model in the white board tests.
 ********************************************************************************/

using PlexShareWhiteboard;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;

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

        // checking is shape items list contains all the necessary shapes
        [Fact]
        public void Test1()
        {
            int s = 10;
            int l = 2;
            int c = 3;
            int d = 2;
            Point start;
            Point end;

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

        // ensuring that the object is same as created
        [Fact]
        public void Test2()
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

        // clear all shapes makes the shape item list empty
        [Fact]
        public void Test3()
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

        // translating shape
        [Fact]
        public void Test4()
        {
            viewModel.ChangeMode("create_ellipse");
            Point start = new(10, 10);
            Point end = new(20, 20);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());

            viewModel.TranslatingShape(viewModel.ShapeItems[0], new Point(40, 50), new Point(50, 60));

            Assert.Equal(40, viewModel.ShapeItems[0].Geometry.Bounds.X);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }


        // testing thicness of shape changes after stroke thickness is updated
        [Fact]
        public void Test5()
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

        // testing last shape is set correctly for a line
        [Fact]
        public void Test6()
        {
            viewModel.ChangeMode("create_line");
            Point start = new(25, 30);
            viewModel.ShapeStart(start);

            Assert.NotNull(viewModel.lastShape);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        // testing z index is incremented
        [Fact]
        public void Test7()
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
    }
}
