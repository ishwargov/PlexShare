/******************************************************************************
 * Filename    = MainWindow.xaml.cs
 *
 * Author      = Neel Kabra
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareApp
 *
 * Description = This is start view of the application. It is responsible for starting the SplashScreen and the AuthenticationView.
 * 
 *****************************************************************************/
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using System.Windows.Input;
using Dashboard;
using PlexShare.Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PlexShareDashboard.Dashboard.UI.ViewModel;
using PlexShareDashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;


namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public event PropertyChangingEventHandler? PropertyChanged;
        public MainWindow()
        {
            InitializeComponent();

            SplashScreen splashScreen = new();
            splashScreen.Show();

            //// Instantiate the authentication view in background
            //AuthenticationView authenticationView = new AuthenticationView();

            // Close the splash screen, and open the Authentication Page

            splashScreen.Close();

            //authenticationView.Show();
            MainFrame.Content = new AuthenticationView();
            this.Show();

            //this.Close();

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
            
            ClientSessionManager clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            if(clientSessionManager.GetUser() != null)
            {
                clientSessionManager.RemoveClient();
            }

            Application.Current.Shutdown();
            Environment.Exit(0);
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

    }
}