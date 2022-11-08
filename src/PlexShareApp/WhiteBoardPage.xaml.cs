using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlexShareWhiteboard;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for WhiteBoardPage.xaml
    /// </summary>
    public partial class WhiteBoardPage : Page
    {
        WhiteBoardViewModel viewModel;

        public WhiteBoardPage()
        {
            InitializeComponent();
            viewModel = new WhiteBoardViewModel();
            viewModel.ShapeItems = new ObservableCollection<ShapeItem>();
            this.DataContext = viewModel;
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var a = e.GetPosition(sender as Canvas);
            viewModel.ShapeStart(a);
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            var a = e.GetPosition(sender as Canvas);
            viewModel.ShapeBuilding(a);
        }

        private void CanvasMouseUp(object sender, MouseEventArgs e)
        {
            var a = e.GetPosition(sender as Canvas);
            viewModel.ShapeFinished(a);
        }

        private void RectangleCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeMode("create_rectangle");
        }

        private void CircleCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeMode("create_circle");

        }
        private void EllipseCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeMode("create_ellipse");
        }

        //Freehand_create_mode
        private void FreehandCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeMode("create_freehand");

        }
        private void ColorGreen(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Green);
        }

        private void ColorRed(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Red);
        }

        private void ColorYellow(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Yellow);
        }

        private void IncreaseZIndex(object sender, RoutedEventArgs e)
        {
            viewModel.IncreaseZIndex();
        }

        private void DecreaseZIndex(object sender, RoutedEventArgs e)
        {
            viewModel.DecreaseZIndex();

        }

        private void ViewMode(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeMode("view_mode");

        }

        private void DeleteMode(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeMode("delete_mode");
        }

        private void SelectMode(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeMode("select_mode");
        }

    }
}
