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
using System.Windows.Shapes;

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
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            _NavigationFrame.Navigate(new HomePageView());
        }


    }
}
