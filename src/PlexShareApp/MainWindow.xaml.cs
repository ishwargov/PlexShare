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
using System.Xml.Linq;

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
            AuthenticationView authView = new();

            authView.Show();
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