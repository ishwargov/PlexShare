/// <author> Harsh Dubey </author>

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
        /// <summary>
        /// Constructor for the class ScreenshareClientView
        /// Set the current DataContext as viewmodel
        /// Initialise the value of SharingScreen to false as initially screen won't be shared
        /// </summary>
        public ScreenshareClientView()
        {
            InitializeComponent();
            ScreenshareClientViewModel viewModel = new();
            this.DataContext = viewModel;
            viewModel.SharingScreen = false;
        }

        /// <summary>
        /// This function is triggered when the user clicks on the Stop Screen Share Button 
        /// It sets the value of SharingScreen boolean to false as screen is not being shared 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnStopButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ScreenshareClientViewModel viewModel)
            {
                viewModel.SharingScreen = false;
            }

            Trace.WriteLine(Utils.GetDebugMessage("Stop Share Button Clicked", withTimeStamp: true));
        }

        /// <summary>
        /// This function is triggered when the user clicks on the Start Screen Share Button 
        /// It sets the value of SharingScreen boolean to true as screen is being shared 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnStartButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ScreenshareClientViewModel viewModel)
            {
                viewModel.SharingScreen = true;
            }

            Trace.WriteLine(Utils.GetDebugMessage("Start Share Button Clicked", withTimeStamp: true));
        }
    }
}
