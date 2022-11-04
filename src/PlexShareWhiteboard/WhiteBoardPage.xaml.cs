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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for WhiteBoardPage.xaml
    /// </summary>
    public partial class WhiteBoardPage : Page
    {
        WBViewModel viewModel;
        public WhiteBoardPage()
        {
            InitializeComponent();
            tb1.Text = " this is changed via backend";
            viewModel = new WBViewModel();
            viewModel.ShapeItems = new ObservableCollection<ShapeItem>();
            this.DataContext = viewModel;

        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var a = e.GetPosition(sender as Canvas);
            viewModel.ShapeStart(a);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var a = e.GetPosition(sender as Canvas);
            viewModel.ShapeBuilding(a);
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            var a = e.GetPosition(sender as Canvas);
            viewModel.ShapeFinished(a);
        }

        private void Rectangle_create_mode(object sender, RoutedEventArgs e)
        {
            viewModel.changeMode("create_rectangle");
        }

        private void Circle_create_mode(object sender, RoutedEventArgs e)
        {
            viewModel.changeMode("create_circle");

        }
        private void Ellipse_create_mode(object sender, RoutedEventArgs e)
        {
            viewModel.changeMode("create_ellipse");

        }

        //Freehand_create_mode
        private void Freehand_create_mode(object sender, RoutedEventArgs e)
        {
            viewModel.changeMode("create_freehand");

        }



        private void Color_green(object sender, RoutedEventArgs e)
        {
            viewModel.changeFillBrush(Brushes.Green);
        }

        private void Color_red(object sender, RoutedEventArgs e)
        {
            viewModel.changeFillBrush(Brushes.Red);

        }

        private void Color_yellow(object sender, RoutedEventArgs e)
        {
            viewModel.changeFillBrush(Brushes.Yellow);
        }

        private void Increase_zindex(object sender, RoutedEventArgs e)
        {
            viewModel.increaseZIndex();
        }

        private void Decrease_zindex(object sender, RoutedEventArgs e)
        {
            viewModel.decreaseZIndex();

        }

        private void View_mode(object sender, RoutedEventArgs e)
        {
            viewModel.changeMode("view_mode");

        }

        private void Delete_mode(object sender, RoutedEventArgs e)
        {
            viewModel.changeMode("delete_mode");
        }


    }
}
