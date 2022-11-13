/******************************************************************************
 * Filename    = AuthenticationView.xaml.cs
 *
 * Author      = Parichita Das
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareApp
 *
 * Description = View for the Authentication Module which will authenticate user's google account and
 *               use their profile information
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        bool stopAnimation = false;
        public AuthenticationView()
        {
            InitializeComponent();
            AuthenticationViewModel viewModel = new();
            this.DataContext = viewModel;
            this.Show();
            Trace.WriteLine("[UX] Entering Authentication View");

            AnimateAuthScreen(this);
        }

        /// <summary>
        /// Shows the animation of the Authentication Screen
        /// </summary>
        /// <param name="obj"></param>
        void AnimateAuthScreen(AuthenticationView obj)
        {
            Trace.WriteLine("[UX] Starting Animation");
            int v = 0;
            int direction = 1;

            // Making animation run forever
            while (stopAnimation == false)
            {
                if(v == 0)
                {
                    direction = 1;
                }   
                else if(v == 100)
                {
                    direction = -1;
                }

                v += direction;

                // Adding value to all the objects
                obj.pb1.Dispatcher.Invoke(() => pb1.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb2.Dispatcher.Invoke(() => pb2.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb3.Dispatcher.Invoke(() => pb3.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb4.Dispatcher.Invoke(() => pb4.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb5.Dispatcher.Invoke(() => pb5.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb6.Dispatcher.Invoke(() => pb6.Value = v, System.Windows.Threading.DispatcherPriority.Background);

                Thread.Sleep(40);  
            }

            Trace.WriteLine("[UX] Stopping Animation");
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
            stopAnimation = true;
            AuthenticationViewModel viewModel = this.DataContext as AuthenticationViewModel;
            var returnVal = await viewModel.AuthenticateUser();

            // Brings back the app to the forefront 
            this.Activate();

            Trace.WriteLine("[UX] Authentiation Completed");

            // There were no errors in authentication
            if (returnVal[0] == "true")
            {
                Trace.WriteLine("[UX] Authentication Successful");
                // Creating Home Page and moving forward
                var homePage = new HomePageView(returnVal[1], returnVal[2], returnVal[3]);

                homePage.Show();
                this.Close(); 
            } 
            else
            {
                Trace.WriteLine("[UX] Authentication Unsuccessful");
                // Re-initiaiting the animation
                stopAnimation = false;
                AnimateAuthScreen(this);
            }        
        }
    }
}
