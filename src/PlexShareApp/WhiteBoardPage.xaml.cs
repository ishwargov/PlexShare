 /***********************************
 *Filename = WhiteBoardPage.xaml.cs
 *
 *Author = Parvathy S Kumar
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = Whiteboard View
 *************************************/
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
        bool singleTrigger = true;
        int val;

        public WhiteBoardPage(int serverID)
        {
            InitializeComponent();
            //viewModel = new WhiteBoardViewModel();
            viewModel = WhiteBoardViewModel.Instance;
            viewModel.ShapeItems = new ObservableCollection<ShapeItem>();
            this.DataContext = viewModel;
            this.currentTool = "Select";
            this.RestorFrameDropDown.SelectionChanged += RestorFrameDropDownSelectionChanged;
        }

        /// <summary>
        /// Mouse Click Event for the WhiteBorad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(viewModel.canDraw)
            {
                var a = e.GetPosition(sender as Canvas);
                viewModel.ShapeStart(a);
                singleTrigger = true;
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
                else
                {
                    if (this.ShapeToolBar.Visibility == Visibility.Visible)
                        this.ShapeToolBar.Visibility = Visibility.Collapsed;

                }
            }
        }

        /// <summary>
        /// Event capturing the mouse move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            var a = e.GetPosition(sender as Canvas);
            viewModel.ShapeBuilding(a);

        }

        //private void CanvasMouseUp(object sender, MouseEventArgs e)
        //{
        //    var a = e.GetPosition(sender as Canvas);
        //    viewModel.ShapeFinished(a);
        //}
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
                case "text":
                    Cursor = Cursors.Arrow;
                    break;
                case "Line":
                    Cursor = Cursors.Cross;
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

        private void CanvasMouseUp(object sender, MouseEventArgs e)
        {
            if (singleTrigger == true)
            {
                Debug.WriteLine("canvas mouse up 11 ");
                var a = e.GetPosition(sender as Canvas);
                viewModel.ShapeFinished(a);
                e.Handled = true;
            }
            singleTrigger = false;
        }

        private void CanvasMouseUp1(object sender, MouseEventArgs e)
        {
            if (singleTrigger == true)
            {
                Debug.WriteLine("canvas mouse up 11 ");
                var a = e.GetPosition(sender as Canvas);
                viewModel.ShapeFinished(a);
                e.Handled = true;
            }
            singleTrigger = false;
        }
        private void RectangleCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            this.currentTool = "Rectangle";
            if (this.ShapeToolBar.Visibility == Visibility.Collapsed)
                this.ShapeToolBar.Visibility = Visibility.Visible;
            viewModel.ChangeMode("create_rectangle");
        }

        private void EllipseCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            this.currentTool = "Ellipse";
            if (this.ShapeToolBar.Visibility == Visibility.Collapsed)
                this.ShapeToolBar.Visibility = Visibility.Visible;
            viewModel.ChangeMode("create_ellipse");
        }

        //Freehand_create_mode
        private void FreehandCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            Cursor = Cursors.Pen;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            this.currentTool = "Freehand";

            viewModel.ChangeMode("create_freehand");

        }

        private void TextboxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("Enter text mode");
            Debug.WriteLine(e.Key);
            viewModel.TextBoxStart(e.Key);
        }

        private void TextboxCreateMode(object sender, RoutedEventArgs e)
        {

            this.currentTool = "text";

            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
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

        private void ColorNull(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(null);
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
            viewModel.select.ifSelected = false;
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
            viewModel.select.ifSelected = false;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear the canvas ? Cick on Save to save your progress.",
                          "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                viewModel.ClearAllShapes();
                return;
            }
            else
            {
                return;
            }


        }

        private void LineMode(object sender, RoutedEventArgs e)
        {
            this.currentTool = "Line";
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            viewModel.ChangeMode("create_line");
        }

        private void UndoMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            viewModel.CallUndo();
            Debug.WriteLine("Undo called xaml");
            viewModel.modeForUndo = "";
        }

        private void RedoMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;

            viewModel.CallRedo();
        }

        private void SaveMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            viewModel.SaveSnapshot();
        }

        private void RestorFrameDropDownSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = (ListBox)sender;

            if (this.RestorFrameDropDown.SelectedItem != null)
            {

                string item = listbox.SelectedItem.ToString();
                string numeric = new String(item.Where(Char.IsDigit).ToArray());
                int cp = int.Parse(numeric);

                MessageBoxResult result = MessageBox.Show("Are you sure you want to load checkpoint " + numeric + " ? All progress since the last checkpoint would be lost!",
                              "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    viewModel.LoadSnapshot(cp);
                    this.RestorFrameDropDown.SelectedItem = null;
                    return;
                }
                else
                {
                    this.RestorFrameDropDown.SelectedItem = null;
                    return;
                }
            }
            else
            {
                return;
            }
        }

        public void ChangeThickness(object sender, RoutedEventArgs e)
        {
            int thickness = (int)ThicknessSlider.Value;
            viewModel.ChangeStrokeThickness(thickness);
            ShapeThicknessSlider.Value = thickness;
            LineThicknessSlider.Value = thickness;
            

        }

        public void ChangeShapeThickness(object sender, RoutedEventArgs e)
        {
            int thickness = (int)ShapeThicknessSlider.Value;
            viewModel.ChangeStrokeThickness(thickness);
            LineThicknessSlider.Value = thickness;
            ThicknessSlider.Value = thickness;

        }

        public void LineThicknessChange(object sender, RoutedEventArgs e)
        {
            int thickness = (int)LineThicknessSlider.Value;
            viewModel.ChangeStrokeThickness(thickness);
            ShapeThicknessSlider.Value = thickness;
            ThicknessSlider.Value = thickness;
        }



    }
}
