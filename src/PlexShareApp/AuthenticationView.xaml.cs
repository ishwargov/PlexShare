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

using AuthViewModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for AuthenticationView.xaml
    /// </summary>
    public partial class AuthenticationView : Page
    {
        private static bool stopAnimation = false;
        private static bool buttonClicked = false;
        public AuthenticationView()
        {
            InitializeComponent();
            
            AuthenticationViewModel viewModel = new();
            this.DataContext = viewModel;

            Trace.WriteLine("[UX] Entering Authentication View");
            // this.FO
            // AnimateAuthScreen(this);
            Task task = new Task(() => AnimateAuthScreen(this));
            task.Start();
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

            while (stopAnimation == false)
            {
                if (v == 0)
                {
                    direction = 1;
                }
                else if (v == 100)
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

                Thread.Sleep(15);
            }

            Trace.WriteLine("[UX] Stopping Animation");
        }

        /// <summary>
        /// Interaction with the Sign In button which redirects to Home Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Home_Click(object sender, RoutedEventArgs e)
        {
            // Sign In Button Should be disabled if it has been clicked once
            if (buttonClicked)
            {
                return;
            }

            buttonClicked = true;
            stopAnimation = true;

            AuthenticationViewModel viewModel = this.DataContext as AuthenticationViewModel;
            var returnVal = await viewModel.AuthenticateUser();

            // Brings back the app to the forefront 
            Application.Current.MainWindow.Activate();
          
            Trace.WriteLine("[UX] Authentiation Completed");

            // There were no errors in authentication
            if (returnVal[0] == "true")
            {
                Trace.WriteLine("[UX] Authentication Successful");
            }
            else
            {
                Trace.WriteLine("[UX] Authentication Unsuccessful");
                // Re-initiaiting the animation
                stopAnimation = false;
                // Button Click re-enabled
                buttonClicked = false;
                AnimateAuthScreen(this);
                return;
            }

            // Creating Home Page and moving forward
            var homePage = new HomePageView(returnVal[1], returnVal[2], returnVal[3]);
            this.NavigationService.Navigate(homePage);
        }
    }
}