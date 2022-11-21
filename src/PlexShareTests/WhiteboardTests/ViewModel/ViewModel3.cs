/********************************************************************************
 * Filename    = WhiteBoardViewModel.cs
 *
 * Author      = Asha Jose
 *
 * Product     = Plex Share Tests
 * 
 * Project     = White Board Tests
 *
 * Description = This is testing the curve operations in the white board tests
 *               This contains creation, selection, dimension change, deletion,
 *               transformation and translation of curves.
 ********************************************************************************/

using PlexShareWhiteboard;
using System.Windows.Media;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareTests.WhiteboardTests.ViewModel
{
    [Collection("Sequential")]
    public class ViewModel3
    {

        WhiteBoardViewModel viewModel;
        public ViewModel3()
        {
            viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(5);
        }

        [Fact]
        public void BrushTest1()
        {
            // to revert previous test changes
            viewModel.select.ifSelected = false;

            // creating a shape  #shape 1
            viewModel.ChangeMode("create_rectangle");
            viewModel.ShapeStart(new Point(10, 10));
            viewModel.ShapeBuilding(new Point(20, 20));
            viewModel.ShapeFinished(new Point());

            // updating current stroke thickness
            // default thickness is 1
            viewModel.ChangeStrokeThickness(7);
            // updating current color of border
            // default color is black
            viewModel.ChangeStrokeBrush(Brushes.Yellow);
            // updating current fill color
            // default is transparent
            viewModel.ChangeFillBrush(Brushes.Green);
            // decrease z index
            // since originally after one shape it is incremented, this shd give same z index

            // creating a new shape  #shape 2
            viewModel.ChangeMode("create_ellipse");
            viewModel.ShapeStart(new Point(10, 15));
            viewModel.ShapeBuilding(new Point(20, 30));
            viewModel.ShapeFinished(new Point());

            // asserting the values of shape 1 is different from shape2
            Assert.NotEqual(viewModel.ShapeItems[0].StrokeThickness, viewModel.ShapeItems[1].StrokeThickness);
            Assert.NotEqual(viewModel.ShapeItems[0].Stroke, viewModel.ShapeItems[1].Stroke);
            Assert.NotEqual(viewModel.ShapeItems[0].Fill, viewModel.ShapeItems[1].Fill);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void BrushTest2()
        {
            // creating a shape  #shape 1
            viewModel.ChangeMode("create_rectangle");
            viewModel.ShapeStart(new Point(0, 0));
            viewModel.ShapeBuilding(new Point(20, 20));
            viewModel.ShapeFinished(new Point());

            Brush color = viewModel.ShapeItems[0].Fill;
            Brush strokeColor = viewModel.ShapeItems[0].Stroke;

            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(10, 10));
            viewModel.ShapeFinished(new Point());

            // updating color of border for the selected shape
            // default color is black
            viewModel.ChangeStrokeBrush(Brushes.Red);

            // updating fill color  for the selected shape
            // default is transparent
            viewModel.ChangeFillBrush(Brushes.Blue);

            Assert.NotEqual(strokeColor, viewModel.ShapeItems[0].Stroke);
            Assert.NotEqual(color, viewModel.ShapeItems[0].Fill);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void NullCases()
        {
            ShapeItem shape = null;
            viewModel.CreateIncomingShape(shape);
            viewModel.ModifyIncomingShape(shape);
            viewModel.DeleteIncomingShape(shape);

            Assert.Empty(viewModel.ShapeItems);
        }


    }
}