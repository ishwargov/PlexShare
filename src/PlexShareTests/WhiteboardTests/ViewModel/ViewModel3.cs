using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlexShareWhiteboard;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
    public class ViewModel3
    {

        WhiteBoardViewModel viewModel;
        public ViewModel3()
        {
            viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(5);
        }
        [Fact]
        public void BrushTest()
        {

            viewModel.ChangeMode("create_rectangle");
            Point start = new(10, 10);
            Point end = new(20, 20);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());
            viewModel.ChangeStrokeThickness(7);
            viewModel.ChangeStrokeBrush(Brushes.Yellow);
            viewModel.ChangeFillBrush(Brushes.Green);


            viewModel.ChangeMode("create_rectangle");
            Point start2 = new(10, 15);
            Point end2 = new(20, 30);
            viewModel.ShapeStart(start);
            viewModel.ShapeBuilding(end);
            viewModel.ShapeFinished(new Point());

            Assert.NotEqual(viewModel.ShapeItems[0].StrokeThickness, viewModel.ShapeItems[1].StrokeThickness);
            Assert.NotEqual(viewModel.ShapeItems[0].Stroke, viewModel.ShapeItems[1].Stroke);
            Assert.NotEqual(viewModel.ShapeItems[0].Fill, viewModel.ShapeItems[1].Fill);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }




    }
}
