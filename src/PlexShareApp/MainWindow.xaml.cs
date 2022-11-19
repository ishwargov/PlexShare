/******************************************************************************
 * Filename    = MainWindow.xaml.cs
 *
 * Author      = Neel Kabra
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareApp
 *
 * Description = This is start view of the application. It is responsible for starting the authenticationView.
 * 
 *****************************************************************************/
using System.Threading;
using System.Windows;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            /*
             *  Main Code :
             */
            SplashScreen splashScreen = new();
            splashScreen.Show();
            
            // Instantiate the authentication view in background
            AuthenticationView authenticationView = new AuthenticationView();

            // Close the splash screen, and open the Authentication Page
            splashScreen.Close();
            Thread.Sleep(500);
            authenticationView.Show();
            this.Close();

            /*
             *  If everyone does not like to go through authentication while testing
             *  Comment the above lines and uncomment the below lines
             */

            //HomePageView Homeview = new HomePageView("Neel","111901057@smail.iitpkd.ac.in", "https://images.unsplash.com/photo-1661956602868-6ae368943878?ixlib=rb-4.0.3&ixid=MnwxMjA3fDF8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80");
            //Homeview.Show();
            //this.Close();
        }

    }
}