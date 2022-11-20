using Microsoft.Azure.WebJobs.Host.Listeners;
using PlexShareCloud;
using PlexShareCloudUX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for CurrentSubmissionsPage.xaml
    /// </summary>
    public partial class CurrentSubmissionsPage : Page
    {
        public CurrentSubmissionsPage(string sessionId)
        {
            InitializeComponent();

            SessionId = sessionId;
            viewModel = new CurrentSubmissionsViewModel(sessionId);
            this.DataContext = viewModel;
            viewModel.PropertyChanged += Listener;
            submissions = new List<SubmissionEntity> { };
            Trace.WriteLine("[Cloud] Submission View created Successfully");
        }

        /// <summary>
        /// SessionId of this session
        /// </summary>
        private string SessionId;

        /// <summary>
        /// ViewModel to use.
        /// </summary>
        private readonly CurrentSubmissionsViewModel viewModel;

        /// <summary>
        /// List of submissions made.
        /// </summary>
        public IReadOnlyList<SubmissionEntity>? submissions;

        /// <summary>
        /// OnPropertyChange handler to handle the change of submissions varibale.
        /// </summary>
        private void Listener(object sender, PropertyChangedEventArgs e)
        {
            submissions = viewModel.ReceivedSubmissions;
            Stack.Children.Clear();

            /*
             * Building the UI when no submissions are made.
             */
            if (submissions?.Count == 0)
            {
                Label label = new Label()
                {
                    Content = "No Submissions Available",
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 16
                };
                Stack.Children.Add(label);
                return;
            }

            /*
             * Building the UI when there are list of submissions made.
             */
            for (int i = 0; i < submissions?.Count; i++)
            {
                Label label = new Label()
                {
                    Content = submissions[i].UserName,
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 16,
                    BorderBrush = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0,0,0,1)
                };
                Stack.Children.Add(label);
            }
        }

        /// <summary>
        /// Handler to the refresh button press
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.GetSubmissions(SessionId);
        }
    }
}
