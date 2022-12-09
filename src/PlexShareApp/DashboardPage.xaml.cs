using Dashboard;
using MaterialDesignThemes.Wpf;
using PlexShare.Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.UI.ViewModel;
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


namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        

        public DashboardViewModel DashboardViewModelInstance { get; set; }
        public DashboardPage()
        {
            InitializeComponent();
            

            this.DashboardViewModelInstance = new DashboardViewModel();
            this.DataContext = DashboardViewModelInstance;
        }

        //defining the onrefreshButtonClick event for this purpose 
        public void OnRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            //updating the view Model 
            this.DashboardViewModelInstance.UpdateDashboardViewModel();

            //say everything went fine 
            return;
        }


        public void OnSwitchModeButtonClick(object sender, RoutedEventArgs e)
        {

            //just call the session manager to change the mode of the current lab 
            this.DashboardViewModelInstance.SwitchSessionMode();


        }


        public void OnLeaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.DashboardViewModelInstance.LeaveMeetingProcedure();
        }

       
    }
}