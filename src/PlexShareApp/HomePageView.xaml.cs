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

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : Window
    {
        public HomePageView()
        {
            InitializeComponent();
        }

        private void toMainScreen(object sender, RoutedEventArgs e)
        {

            MainScreenView mainScreenView = new MainScreenView();
            mainScreenView.Show();
            this.Close();
        }

    }
}
