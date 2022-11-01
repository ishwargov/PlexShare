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
        public MainScreenView()
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
            Debug.WriteLine("DashBoardUX");
            Main.Content = dashboardPage;
        }

        private void ScreenShareClick(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("ScreenShareUX");
            Main.Content = whiteBoardPage;
        }

        private void WhiteboardClick(object sender, RoutedEventArgs e)
        {
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
