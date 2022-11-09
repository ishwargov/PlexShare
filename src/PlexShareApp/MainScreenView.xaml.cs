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
        private static ChatPage chatPage;
        private static ScreenSharePage screenSharePage;
        public MainScreenView(string name,string email,string imageLocation,string server_ip,string server_port)
        {
            InitializeComponent();
            dashboardPage = new DashboardPage();
            whiteBoardPage = new WhiteBoardPage();
            chatPage = new ChatPage();
            screenSharePage = new ScreenSharePage();
            Main.Content = dashboardPage;
        }

        /// <summary>
        /// Transfer control to dashboard on click
        /// </summary>
        private void DashboardClick(object sender, RoutedEventArgs e)
        {
            Dashboard.Background = Brushes.PeachPuff;
            Whiteboard.Background = Brushes.DarkSlateGray;
            Screenshare.Background = Brushes.DarkSlateGray;

            Dashboard.Foreground = Brushes.Black;
            Whiteboard.Foreground = Brushes.SeaShell;
            Screenshare.Foreground = Brushes.SeaShell;


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
            Screenshare.Background = Brushes.PeachPuff;

            Dashboard.Foreground = Brushes.SeaShell;
            Whiteboard.Foreground = Brushes.SeaShell;
            Screenshare.Foreground = Brushes.Black;

            System.Console.WriteLine("ScreenShareUX");
            Main.Content = whiteBoardPage;
        }

        /// <summary>
        /// To transfer control to the wihteboard on click
        /// </summary>
        private void WhiteboardClick(object sender, RoutedEventArgs e)
        {
            Dashboard.Background = Brushes.DarkSlateGray;
            Whiteboard.Background = Brushes.PeachPuff;
            Screenshare.Background = Brushes.DarkSlateGray;

            Dashboard.Foreground = Brushes.SeaShell;
            Whiteboard.Foreground = Brushes.Black;
            Screenshare.Foreground = Brushes.SeaShell;

            System.Console.WriteLine("Whiteboard UX");
            Main.Content = screenSharePage;
        }

        /// <summary>
        /// Open the chat overlay when clicked
        /// </summary>
        private void ChatButtonClick(object sender, RoutedEventArgs e)
        {
            if (chatOn == false)
            {
                chatOn = true;
                ChatWindow.Background = Brushes.PeachPuff;
                ChatIcon.Foreground = Brushes.Black;
                ScreenWithChat.Content = chatPage;
            }
            else
            {
                chatOn=false;
                ChatWindow.Background = Brushes.DarkSlateGray;
                ChatIcon.Foreground = Brushes.White;
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
