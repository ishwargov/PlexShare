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
using System.Windows.Shapes;
using AuthViewModel;
using PlexShareApp;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for AuthenticationView.xaml
    /// </summary>
    public partial class AuthenticationView : Window
    {
        public AuthenticationView()
        {
            InitializeComponent();
            AuthenticationViewModel viewModel = new AuthenticationViewModel();
            this.DataContext = viewModel;
        }

        private void TitleBarDrag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                WindowState = WindowState.Minimized;
            else
                WindowState = WindowState.Normal;
        }

        private void MaximizeApp(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                WindowState = WindowState.Maximized;
            }
        }

        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new System.Windows.Thickness(8);
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
            }
        }
        private async void Home_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationViewModel viewModel = this.DataContext as AuthenticationViewModel;
            var returnVal = await viewModel.AuthenticateUser();
            
            if (returnVal[0] == "true")
            {
                var homePage = new HomePageView(returnVal[1], returnVal[2], returnVal[3]);
                homePage.Show();
                Close(); 
            } 
            else
            {
                this.SignInButton.Content = "Try Again!";
            }            
        }
    }
}
