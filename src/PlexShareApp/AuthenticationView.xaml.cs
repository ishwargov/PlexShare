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
using AuthViewModel;
using PlexShareApp;

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

            AuthenticationViewModel viewModel = new AuthenticationViewModel();
            this.DataContext = viewModel;
        }

        private async void Home_Click(object sender, RoutedEventArgs e)
        {

            AuthenticationViewModel viewModel = this.DataContext as AuthenticationViewModel;
            var returnVal = await viewModel.AuthenticateUser();
            
            if (returnVal == true)
            {
                var homePage = new HomePageView();

                homePage.Show();
                Close(); 
            } else
            {
                this.SignInButton.Content = "Try Again!";
            }
            
        }



    }
}
