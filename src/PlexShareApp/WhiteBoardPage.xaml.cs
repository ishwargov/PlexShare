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
using System.Runtime.Serialization.Formatters;
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
using Client.Models;
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

        public WhiteBoardPage(int serverID)
        {
            InitializeComponent();
            //viewModel = new WhiteBoardViewModel();
            viewModel = WhiteBoardViewModel.Instance;
            Trace.WriteLine("[WhiteBoard] White Board Page is initialised serverId: " + serverID + "viewModel.userId : " + viewModel.userId);
             
            if (serverID == 0)
                viewModel.isServer = true;
            else
                viewModel.isServer = false;

            //if (viewModel.canDraw == true && serverID == 0)
            // init means noone called, ! means someone called (server) and we found out that it is not server
            if (!viewModel.userId.Equals("init") && serverID != 0)
            {
                Trace.WriteLine("[WhiteBoard] recalling setuserid");
                // this might be that the dashboard had called this before
                // default it is not a server so we need to reiniiliase only if this is server
                int passId = Int32.Parse(viewModel.userId);
                viewModel.SetUserId(passId);
            }
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
            Trace.WriteLine("[WhiteBoard Xaml] candraw "+ viewModel.canDraw);
            if (viewModel.canDraw)
            {
                var a = e.GetPosition(sender as Canvas);
                viewModel.ShapeStart(a);
                singleTrigger = true;
                if (viewModel.select.ifSelected)
                {

                    string shapeName = viewModel.select.selectedObject.Geometry.GetType().Name;
                    if (shapeName == "EllipseGeometry" || shapeName == "RectangleGeometry" )
                    {
                        if (this.StrokeToolBar.Visibility == Visibility.Visible)
                            this.StrokeToolBar.Visibility = Visibility.Collapsed;
                        if (this.ShapeToolBar.Visibility == Visibility.Visible)
                            this.ShapeToolBar.Visibility = Visibility.Collapsed;
                        if (this.ShapeSelectionToolBar.Visibility == Visibility.Collapsed)
                            this.ShapeSelectionToolBar.Visibility = Visibility.Visible;
                    }
                    else if(shapeName == "PathGeometry" || shapeName == "LineGeometry")
                    {
                        if (this.ShapeToolBar.Visibility == Visibility.Visible)
                            this.ShapeToolBar.Visibility = Visibility.Collapsed;
                        if (this.StrokeToolBar.Visibility == Visibility.Collapsed)
                            this.StrokeToolBar.Visibility = Visibility.Visible;
                        if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                            this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (this.ShapeToolBar.Visibility == Visibility.Visible)
                            this.ShapeToolBar.Visibility = Visibility.Collapsed;
                        if (this.StrokeToolBar.Visibility == Visibility.Visible)
                            this.StrokeToolBar.Visibility = Visibility.Collapsed;
                        if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                            this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (this.ShapeToolBar.Visibility == Visibility.Visible)
                        this.ShapeToolBar.Visibility = Visibility.Collapsed;
                    if (this.StrokeToolBar.Visibility == Visibility.Visible)
                        this.StrokeToolBar.Visibility = Visibility.Collapsed;
                    if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                        this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// Function corresponding to the event where the mouse enters the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Functoin to invoke when the mouse leaves the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseLeave(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine(this.currentTool + " Leave Got it \n");
            if (this.currentTool != "Select")
                viewModel.UnHighLightIt();
            Cursor = Cursors.Arrow;

        }

        /// <summary>e.Handled = true; 
        /// Canvas Mouseup  or mouse release event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Function corresponding to the rectangle icon in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RectangleCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            this.currentTool = "Rectangle";
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeToolBar.Visibility == Visibility.Collapsed)
                this.ShapeToolBar.Visibility = Visibility.Visible;
            viewModel.ChangeMode("create_rectangle");
        }

        /// <summary>
        /// Ellipse icon function in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EllipseCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            this.currentTool = "Ellipse";
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeToolBar.Visibility == Visibility.Collapsed)
                this.ShapeToolBar.Visibility = Visibility.Visible;
            viewModel.ChangeMode("create_ellipse");
        }

        /// <summary>
        /// FUnction corresponding to the brush iconin the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FreehandCreateMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            Cursor = Cursors.Pen;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            this.currentTool = "Freehand";

            viewModel.ChangeMode("create_freehand");

        }

        /// <summary>
        /// Text box Key Down functions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextboxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("Enter text mode");
            Debug.WriteLine(e.Key);
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            viewModel.TextBoxStart(e.Key);
            int thickness = (int)ThicknessSlider.Value;
            viewModel.ChangeStrokeThickness(thickness);

        }

        private void TextboxPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Debug.WriteLine("Space inserted");
                if (this.ShapeToolBar.Visibility == Visibility.Visible)
                    this.ShapeToolBar.Visibility = Visibility.Collapsed;
                if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                    this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
                if (this.StrokeToolBar.Visibility == Visibility.Visible)
                    this.StrokeToolBar.Visibility = Visibility.Collapsed;
                viewModel.TextBoxStart(e.Key);
            }

        }

        /// <summary>
        /// Function for invoking the text mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextboxCreateMode(object sender, RoutedEventArgs e)
        {
            if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
                return;

            this.currentTool = "text";

            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            Cursor = Cursors.Pen;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            Trace.WriteLine("[WhiteBoard] White Board Page entered the Text Mode");
            this.currentTool = "Textbox";
            viewModel.ChangeStrokeThickness(1);
            viewModel.ChangeMode("create_textbox");

        }

        /// <summary>
        /// Functions for changing the fill colors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Functions for changing the stroke colors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Toolbar Delete button function 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            this.currentTool = "Eraser";
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            viewModel.ChangeMode("delete_mode");
        }


        /// <summary>
        /// Function corresponding to the select mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectMode(object sender, RoutedEventArgs e)
        {
            this.currentTool = "Select";
            viewModel.ChangeMode("select_mode");
        }
    
        
        /// <summary>
        /// Function corresponding to the clear button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// Function corresponding to the line button in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineMode(object sender, RoutedEventArgs e)
        {
            this.currentTool = "Line";
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            viewModel.ChangeMode("create_line");
        }

        /// <summary>
        /// Undo button function in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            viewModel.select.ifSelected = false;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            viewModel.CallUndo();
            Debug.WriteLine("Undo called xaml");
            viewModel.modeForUndo = "";
        }


        /// <summary>
        /// Function called when clicking the redo button in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedoMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();

            viewModel.select.ifSelected = false;
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;

            viewModel.CallRedo();
        }

        private void SaveMode(object sender, RoutedEventArgs e)
        {
            viewModel.UnHighLightIt();
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            viewModel.SaveSnapshot();
            SuccessSaveMessage();
        }

        private int SuccessSaveMessage()
        { 
            MessageBox.Show("The current snapshot is successfully saved","Confirmation",MessageBoxButton.OK,MessageBoxImage.Information);
            return 0;

        }

        private void RestorFrameDropDownSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                this.ShapeToolBar.Visibility = Visibility.Collapsed;
            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                this.StrokeToolBar.Visibility = Visibility.Collapsed;
            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
            ListBox listbox = (ListBox)sender;

            if (this.RestorFrameDropDown.SelectedItem != null)
            {

                string item = listbox.SelectedItem.ToString();
                string numeric = new String(item.Where(Char.IsDigit).ToArray());
                int cp = int.Parse(numeric);

                MessageBoxResult result = MessageBox.Show("Are you sure you want to load snapshot " + numeric + " ? All progress since the snapshot would be lost!",
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
            if(thickness > 1)
            {
                viewModel.ChangeStrokeThickness(thickness);
                ShapeThicknessSlider.Value = thickness;
                LineThicknessSlider.Value = thickness;
            }
            
            
        }


        /// <summary>
        /// Function to control the thickness of stroke (associated with Rectangle and Ellipse)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeShapeThickness(object sender, RoutedEventArgs e)
        {
            int thickness = (int)ShapeThicknessSlider.Value;
            if(thickness > 1)
            {
                viewModel.ChangeStrokeThickness(thickness);
                LineThicknessSlider.Value = thickness;
                ThicknessSlider.Value = thickness;
            }
            
        }


        public void LineThicknessChange(object sender, RoutedEventArgs e)
        {
            int thickness = (int)LineThicknessSlider.Value;
            if(thickness > 1)
            {
                viewModel.ChangeStrokeThickness(thickness);
                ShapeThicknessSlider.Value = thickness;
                ThicknessSlider.Value = thickness;
            }
            
        }



    }
}
