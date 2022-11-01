//using PlexShareDashboard.Dashboard.UI.ViewModel;
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
using PlexShareDashboard.Dashboard.UI.ViewModel;

namespace PlexShareDashboard.Dashboard.UI.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// Interaction logic for DashboardView.xaml
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        //defining the dashboard View Model 
        public DashboardViewModel dashboardViewModel { get; set; }
        public DashboardView()
        {
            InitializeComponent();

            //here i can initialise the dashboard view model to be able to use 
            //and to be able to utilise the functionality to update the vm whenever the refresh button is clicked for this purpose 

            this.dashboardViewModel =  new DashboardViewModel();
            this.DataContext = dashboardViewModel;
        }

        //private void InitializeComponent()
        //{
        //    InitializeComponent();
        //}


        //defining the onrefreshButtonClick event for this purpose 
        public void OnRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            //updating the view Model 
            this.dashboardViewModel.UpdateDashboardViewModel();

            //say everything went fine 
            return;
        }


        public void OnSwitchModeButtonClick(object sender, RoutedEventArgs e)
        {
            //just call the session manager to change the mode of the current lab 
            //just call the session manager to change the mode of the current lab 
            //currUser = clientSessionManager.GetUsesr();
            //if (currUser.userId == 1)
            //{
            //    clientSessionManager.ChangeSessionMode();
            //}
            //else
            //{ 
            //    //say you cannot change the mode of the session 

            //}

        }


        public void OnLeaveButtonClick(object sender, RoutedEventArgs e)
        {

            //just call the client session manager to leave the meeting 
          
            //just call the client session manager to leave the meeting 
            //clientSessionManager.RemoveClient();
            //currUser = clientSessionManager.GetUsesr();
            //if(currUser.userId == '1')
            //{
            //    clientSessionmanager.EndMeet();
            //}
            //else
            //{
            //    clientSessionManager.RemoveClient();
            //}
        }

        public void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            //i have to call the view of the cloud UI to give option to upload the file
            //the file will be directly send to the viewmodel of the cloud team 
            //i have to call the view of the cloud UI to give option to upload the file
            //the file will be directly send to the viewmodel of the cloud team 

        }
    }
}
