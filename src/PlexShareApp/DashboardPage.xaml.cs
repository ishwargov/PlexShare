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
        //public DashboardPage()
        //{
        //    InitializeComponent();
        //}

        public DashboardViewModel DashboardViewModelInstance { get; set; }
        public DashboardPage()
        {
            InitializeComponent();
            //this.DataContext = 
            //here i can initialise the dashboard view model to be able to use 
            //and to be able to utilise the functionality to update the vm whenever the refresh button is clicked for this purpose 
            ClientSessionManager clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            SessionData sessionData = clientSessionManager.GetSessionData();
            string sessionId = sessionData.sessionId.ToString();
            UserData currUser = clientSessionManager.GetUser();

            string emailId = currUser.userEmail;

            UploadPage FileUploadPage = new UploadPage(sessionId, emailId);
            //Main.Content = FileUploadPage;

            this.DashboardViewModelInstance = new DashboardViewModel();
            this.DataContext = DashboardViewModelInstance;
            //NavigationPage MainPage = new NavigationPage(new DashboardPage());
        }

        //private void InitializeComponent()
        //{
        //    InitializeComponent();
        //}


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

        public void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            //i have to call the view of the cloud UI to give option to upload the file
            //the file will be directly send to the viewmodel of the cloud team 
            //i have to call the view of the cloud UI to give option to upload the file
            //the file will be directly send to the viewmodel of the cloud team 
            //we have to get the user and the email id of the user as the username for this purpose 

            //NewXamlUserControl ui = new NewXamlUserControl();
            //DashboardPage newWindow = new DashboardPage();
            //newWindow.Content = FileUploadPage;
            //newWindow.Show();
            //FileUploadPage.Show();
            //FileUploadPage.ShowDialog);
            //Main.IsEnabled = true;
            return;
        }
    }
}