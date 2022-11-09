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

        ///<summary>
        ///To move the window
        ///</summary>
        private void TitleBarDrag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        ///<summary>
        /// To close the window
        ///</summary>
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        ///<summary>
        ///  To minimize the application
        ///</summary>
        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                WindowState = WindowState.Minimized;
            else
                WindowState = WindowState.Normal;
        }

        ///<summary>
        ///  To maximise the window
        ///</summary>
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

        ///<summary>
        ///  This is used to add a border thickness in the maximised window
        ///  since window is going out of bounds
        ///</summary>
        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new System.Windows.Thickness(6);
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
            }
        }

        /// <summary>
        /// Interaction with the Sign In button which redirects to Home Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Home_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationViewModel viewModel = this.DataContext as AuthenticationViewModel;
            var returnVal = await viewModel.AuthenticateUser();

            // Brings back the app to the forefront 
            this.Activate();
            
            // There were no errors in authentication
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
