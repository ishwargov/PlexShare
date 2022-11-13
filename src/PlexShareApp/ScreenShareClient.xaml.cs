using PlexShareScreenshare.Client;
using System;
using System.Collections.Generic;
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

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    /// 

    public partial class ScreenShareClient : Page
    {
        public ScreenShareClientViewModel viewModel;
        public ScreenShareClient()
        {
            InitializeComponent();
            viewModel= new ScreenShareClientViewModel();
            this.DataContext = viewModel; 
        }

        public void OnStopButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Stop Button Clicked");
            viewModel.SharingScreen = false;
        }

        public void OnStartButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Start Button Clicked");
            viewModel.SharingScreen = true;
        }
    }
}
