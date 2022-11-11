using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        string currentTool;
        public WhiteBoardPage()
        {
            InitializeComponent();
            viewModel = new WhiteBoardViewModel();
            viewModel.ShapeItems = new ObservableCollection<ShapeItem>();
            this.DataContext = viewModel;
            this.currentTool = "Select";
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var a = e.GetPosition(sender as Canvas);
            viewModel.ShapeStart(a);
            if (viewModel.select.ifSelected)
            {

                string shapeName = viewModel.select.selectedObject.Geometry.GetType().Name;
                if (shapeName == "EllipseGeometry" || shapeName == "RectangleGeometry" || shapeName == "PathGeometry" || shapeName == "LineGeometry")
                {
                    if (this.ShapeToolBar.Visibility == Visibility.Collapsed)
                        this.ShapeToolBar.Visibility = Visibility.Visible;
                }

                else
                {
                    if (this.ShapeToolBar.Visibility == Visibility.Visible)
                        this.ShapeToolBar.Visibility = Visibility.Collapsed;
                }



            }
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
        private void CanvasMouseEnter(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine(this.currentTool + " Got it \n");
            if (this.currentTool != "Select")
                viewModel.UnHighLightIt();
            switch (this.currentTool)
            {
                case "Select":
                    Cursor = Cursors.Arrow;
                    break;
                case "Rectangle":
                    Cursor = Cursors.Cross;
                    break;
                case "Ellipse":
                    Cursor = Cursors.Cross;
                    break;
                case "Freehand":
                    Cursor = Cursors.Pen;
                    break;
                case "Eraser":
                    Cursor = Cursors.Arrow;
                    break;
                default:
                    Cursor = Cursors.Arrow;
                    break;
            }
        }

        private void CanvasMouseLeave(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine(this.currentTool + " Leave Got it \n");
            if (this.currentTool != "Select")
                viewModel.UnHighLightIt();
            Cursor = Cursors.Arrow;

        }
        private void RectangleCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            this.currentTool = "Rectangle";
            if (this.ShapeToolBar.Visibility == Visibility.Collapsed)
                this.ShapeToolBar.Visibility = Visibility.Visible;
            viewModel.ChangeMode("create_rectangle");
        }
        private void EllipseCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            this.currentTool = "Ellipse";
            if (this.ShapeToolBar.Visibility == Visibility.Collapsed)
                this.ShapeToolBar.Visibility = Visibility.Visible;
            viewModel.ChangeMode("create_ellipse");
        }

        //Freehand_create_mode
        private void FreehandCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            Cursor = Cursors.Pen;
            if (this.ShapeToolBar.Visibility == Visibility.Collapsed)
                this.ShapeToolBar.Visibility = Visibility.Visible;
            this.currentTool = "Freehand";

            viewModel.ChangeMode("create_freehand");

        }
        private void Textbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("Enter text mode");
            Debug.WriteLine(e.Key);
            viewModel.TextBoxStart(e.Key);
        }

        private void TextboxCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            Cursor = Cursors.Pen;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            this.currentTool = "Textbox";

            viewModel.ChangeMode("create_textbox");

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

        private void ColorWhite(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.White);
        }

        private void ColorBlue(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Blue);
        }

        private void ColorBlack(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Black);
        }

        private void StrokeColorGreen(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeStrokeBrush(Brushes.Green);
        }

        private void StrokeColorRed(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeStrokeBrush(Brushes.Red);

        }

        private void StrokeColorYellow(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeStrokeBrush(Brushes.Yellow);
        }

        private void StrokeColorWhite(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeStrokeBrush(Brushes.White);
        }

        private void StrokeColorBlue(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeStrokeBrush(Brushes.Blue);

        }

        private void StrokeColorBlack(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeStrokeBrush(Brushes.Black);
        }

        private void DeleteMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            this.currentTool = "Eraser";
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            viewModel.ChangeMode("delete_mode");
        }

        private void SelectMode(object sender, RoutedEventArgs e)
        {
            this.currentTool = "Select";
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            viewModel.ChangeMode("select_mode");
        }

        private void ClearMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            viewModel.ClearAllShapes();

        }

        private void LineMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            viewModel.ChangeMode("create_line");
        }

        private void UndoMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            viewModel.CallUndo();
            Debug.WriteLine("Undo called xaml");
        }

        private void RedoMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;

            viewModel.CallRedo();
        }
    }
}
