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

        private void ChatButtonClick(object sender, RoutedEventArgs e)
        {
            if (chatOn == false)
            {
                chatOn = true;
                ScreenWithChat.Content = chatPage;
            }
            else
            {
                chatOn=false;
                ScreenWithChat.Content = null;
            }
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
