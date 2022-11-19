/******************************************************************************
 * Filename    = MainScreenView.xaml.cs
 *
 * Author      = Neel Kabra
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareApp
 *
 * Description = This is main view of the application. It is responsible for starting all the other modules.
 *               The view instantiates a server or a client using the IP given by the HomeScreenView.
 * 
 *****************************************************************************/
using Dashboard;
using PlexShare.Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for MainScreenView.xaml
    /// </summary>
    public partial class MainScreenView : Window
    {
        private bool chatOn;
        private bool cloudOn;
        private static DashboardPage dashboardPage;
        private static WhiteBoardPage whiteBoardPage;
        private static ChatPageView chatPage;
        private static ScreenshareServerView screenshareServerView;
        private static ScreenshareClientView screenshareClientView;
        private static UploadPage uploadPage;

        public event PropertyChangingEventHandler? PropertyChanged;

        private bool isClient;

        public MainScreenView(string name, string email, string picPath, string url, string ip, string port, bool isServer)
        {


            isClient = !isServer;
            cloudOn = false;
            // The client/server was verified to be correct.
            // We can add the client to meeting, and instantiate all modules.
            InitializeComponent();

            dashboardPage = new DashboardPage();
            Trace.WriteLine("[UX] The Dashboard has started");
            chatPage = new ChatPageView();

            Trace.WriteLine("[UX] The ChatPage has started");
            //  uploadPage = new UploadPage();

            if (isServer)
            {
                whiteBoardPage = new WhiteBoardPage(0);
                Trace.WriteLine("[UX] The Whiteboard Server has started");
                screenshareServerView = new ScreenshareServerView();
                Trace.WriteLine("[UX] The Screenshare Server has started");
            }
            else
            {
                whiteBoardPage = new WhiteBoardPage(1);
                Trace.WriteLine("[UX] The Whiteboard Client has started");
                screenshareClientView = new ScreenshareClientView();
                Trace.WriteLine("[UX] The Screenshare Client has started");
            }

            Main.Content = dashboardPage;
            Trace.WriteLine("[UX] Setting the content to the dashboard");

            Trace.WriteLine("[UX] Setting the IP:Port");
            ServerIPandPort.Text = "Server IP : " + ip + "    Port : " + port;

            ClientSessionManager clientSessionManager;
            clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            SessionData sessionData = clientSessionManager._clientSessionData;
            UserData user = clientSessionManager.GetUser();
            uploadPage = new UploadPage(sessionData.sessionId.ToString(), user.userEmail, isServer);

            //this is to disable backspace to avoid switch tabs
            NavigationCommands.BrowseBack.InputGestures.Clear();

        }

        /// <summary>
        /// Transfer control to dashboard on click 
        /// </summary>
        private void DashboardClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[UX] Redering Dashboard");
            Dashboard.Background = Brushes.DarkCyan;
            Whiteboard.Background = Brushes.DarkSlateGray;
            Screenshare.Background = Brushes.DarkSlateGray;

            Main.Content = dashboardPage;
        }

        /// <summary>
        /// Transfer control to whiteboard on click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenShareClick(object sender, RoutedEventArgs e)
        {
            Dashboard.Background = Brushes.DarkSlateGray;
            Whiteboard.Background = Brushes.DarkSlateGray;
            Screenshare.Background = Brushes.DarkCyan;

            if (isClient)
            {
                Trace.WriteLine("[UX] Rendering Client Screenshare");
                Main.Content = screenshareClientView;
            }
            else
            {
                Trace.WriteLine("[UX] Rendering Server Screenshare");
                Main.Content = screenshareServerView;
            }
        }

        /// <summary>
        /// To transfer control to the wihteboard on click
        /// </summary>
        private void WhiteboardClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[UX] Rendering Whiteboard");
            Dashboard.Background = Brushes.DarkSlateGray;
            Whiteboard.Background = Brushes.DarkCyan;
            Screenshare.Background = Brushes.DarkSlateGray;

            Main.Content = whiteBoardPage;
        }

        /// <summary>
        /// Open the chat overlay when clicked
        /// </summary>
        private void ChatButtonClick(object sender, RoutedEventArgs e)
        {
            if (chatOn == false)
            {
                Trace.WriteLine("[UX] Rendering Chat");
                chatOn = true;
                ScreenWithChat.Content = chatPage;
                Chat.Background = Brushes.DarkCyan;
            }
            else
            {
                Trace.WriteLine("[UX] Removing Chat");
                chatOn = false;
                ScreenWithChat.Content = null;
                Chat.Background = Brushes.Transparent;
            }
        }

        private void UploadClick(object sender, RoutedEventArgs e)
        {
            if (cloudOn)
            {
                Cloud.Background = Brushes.Transparent;
                cloudOn = false;
                CloudPage.Content = null;
            }
            else
            {
                Cloud.Background = Brushes.DarkCyan;
                cloudOn = true;
                CloudPage.Content = uploadPage;
            }
        }

        private void ShowIpandPort(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[UX] Clicked the Info Button");
            ServerIPandPort.Visibility = Visibility.Visible;
        }

        private void HideIpandPort(object sender, RoutedEventArgs e)
        {
            ServerIPandPort.Visibility = Visibility.Hidden;
        }

        ///<summary>
        ///To move the window
        ///</summary>
        private void TitleBarDrag(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("[UX] Trying to move the window");
            DragMove();
        }

        ///<summary>
        /// To close the window
        ///</summary>
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Application.Current.Shutdown();
            });
            System.Environment.Exit(0);
        }

        ///<summary>
        ///  To minimize the application
        ///</summary>
        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Minimized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
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
