using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using PlexShare.Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using Dashboard;
using PlexShareApp;
using ScottPlot.Drawing.Colormaps;
using System.ComponentModel;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainScreenView : Window
    {
        private bool chatOn;
        private static DashboardPage dashboardPage;
        private static WhiteBoardPage whiteBoardPage;
        private static ChatPageView chatPage;
        private static ScreenSharePage screenSharePage;
        public event PropertyChangingEventHandler? PropertyChanged;
        
        public MainScreenView(string name, string email, string picPath, string url, string ip, string port)
        {
            bool verified = true;

            IUXServerSessionManager serverSessionManager = SessionManagerFactory.GetServerSessionManager();
            IUXClientSessionManager clientSessionManafer = SessionManagerFactory.GetClientSessionManager();

            if (ip == "-1")
            {
                MeetingCredentials meetingCredentials = serverSessionManager.GetPortsAndIPAddress();
                verified = clientSessionManafer.AddClient(meetingCredentials.ipAddress, meetingCredentials.port, name);
                ip = meetingCredentials.ipAddress;
                port = meetingCredentials.port.ToString();
            }
            else
            {
                verified = clientSessionManafer.AddClient(ip, int.Parse(port), name);
            }

            if (verified)
            {

                InitializeComponent();
                dashboardPage = new DashboardPage();
                whiteBoardPage = new WhiteBoardPage();
                chatPage = new ChatPageView();
                screenSharePage = new ScreenSharePage();
                Main.Content = dashboardPage;
                ServerIPandPort.Text = "Server IP : " + ip + " Port : " + port;
                // ClientIPandPort.Text = "Client IP : " + meetingCredentials.ipAddress  + " Port : " + meetingCredentials.port;

            }

        }

        /// <summary>
        /// Transfer control to dashboard on click
        /// 
        /// </summary>
        private void DashboardClick(object sender, RoutedEventArgs e)
        {
            Dashboard.Background = Brushes.DarkCyan;
            Whiteboard.Background = Brushes.DarkSlateGray;
            Screenshare.Background = Brushes.DarkSlateGray;

            //Dashboard.Foreground = Brushes.Black;
            //Whiteboard.Foreground = Brushes.SeaShell;
            //Screenshare.Foreground = Brushes.SeaShell;

            Debug.WriteLine("DashBoardUX");
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

            //Dashboard.Foreground = Brushes.SeaShell;
            //Whiteboard.Foreground = Brushes.SeaShell;
            //Screenshare.Foreground = Brushes.Black;

            System.Console.WriteLine("ScreenShareUX");
            Main.Content = screenSharePage;
        }

        /// <summary>
        /// To transfer control to the wihteboard on click
        /// </summary>
        private void WhiteboardClick(object sender, RoutedEventArgs e)
        {
            Dashboard.Background = Brushes.DarkSlateGray;
            Whiteboard.Background = Brushes.DarkCyan;
            Screenshare.Background = Brushes.DarkSlateGray;

            //Dashboard.Foreground = Brushes.SeaShell;
            //Whiteboard.Foreground = Brushes.Black;
            //Screenshare.Foreground = Brushes.SeaShell;

            System.Console.WriteLine("Whiteboard UX");
            Main.Content = whiteBoardPage;
        }

        /// <summary>
        /// Open the chat overlay when clicked
        /// </summary>
        private void ChatButtonClick(object sender, RoutedEventArgs e)
        {
            if (chatOn == false)
            {
                chatOn = true;
                //ChatWindow.Background = Brushes.PeachPuff;
                //ChatIcon.Foreground = Brushes.Black;
                ScreenWithChat.Content = chatPage;
            }
            else
            {
                chatOn=false;
                //ChatWindow.Background = Brushes.DarkSlateGray;
                //ChatIcon.Foreground = Brushes.White;
                ScreenWithChat.Content = null;
            }
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
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Application.Current.Shutdown();
            });
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
            if(WindowState == WindowState.Maximized)
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
