using PlexShareScreenshare;
using PlexShareScreenshare.Client;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for ScreenshareClientView.xaml
    /// </summary>
    /// 
    public partial class ScreenshareClientView : Page
    {
        //Constructor for the class ScreenshareClientView
        //Set the current DataContext as viewmodel
        public ScreenshareClientView()
        {
            InitializeComponent();
            ScreenshareClientViewModel viewModel = new();
            this.DataContext = viewModel;
        }

        /// <summary>
        /// This function is triggered when the user clicks on the Stop Screen Share Button 
        /// It will set the property Sharing Screen to false and call the viewmodel on it
        /// It will set the visibility of panels according to the boolean
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnStopButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ScreenshareClientViewModel viewModel)
            {
                viewModel.SharingScreen = false;
                SharedScreen.Visibility = Visibility.Collapsed;
                NotSharedScreen.Visibility = Visibility.Visible;
            }

            Trace.WriteLine(Utils.GetDebugMessage("Stop Share Button Clicked", withTimeStamp: true));
        }

        /// <summary>
        /// This function is triggered when the user clicks on the Start Screen Share Button 
        /// It will set the property Sharing Screen to true and call the viewmodel on it
        /// It will set the visibility of panels according to the boolean
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnStartButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ScreenshareClientViewModel viewModel)
            {
                viewModel.SharingScreen = true;
                SharedScreen.Visibility = Visibility.Visible;
                NotSharedScreen.Visibility = Visibility.Collapsed;
            }

            Trace.WriteLine(Utils.GetDebugMessage("Stop Button Clicked", withTimeStamp: true));
        }
    }
}
