/******************************************************************************
 * Filename    = SessionsPage.xaml.cs
 *
 * Author      = B Sai Subrahmanyam
 *
 * Product     = PlexShareSolution
 * 
 * Project     = PlexShareApp
 *
 * Description = Defines the View of the Sessions Page.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using PlexShareCloudUX;
using PlexShareCloud;
using System.Diagnostics;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for SessionsPage.xaml
    /// </summary>
    public partial class SessionsPage : Page
    {
        public SessionsPage(string userName)
        {
            InitializeComponent();

            UserName = userName;
            viewModel = new SessionsViewModel(userName);
            this.DataContext = viewModel;
            viewModel.PropertyChanged += Listener;
            sessions = new List<SessionEntity> { };
            Trace.WriteLine("[Cloud] Session View created Successfully");
        }
        /// <summary>
        /// Variable to Store the SubmissionsPage of the Selected session.
        /// </summary>
        private static SubmissionsPage? submissionsPage;

        /// <summary>
        /// ViewModel to use.
        /// </summary>
        private readonly SessionsViewModel viewModel;

        /// <summary>
        /// List of the sessions for this userName.
        /// </summary>
        public IReadOnlyList<SessionEntity>? sessions;

        /// <summary>
        /// Username of the client using the application.
        /// </summary>
        public string UserName;

        /// <summary>
        /// OnPropertyChange handler to handle the change of sessions variable.
        /// </summary>
        private void Listener(object sender, PropertyChangedEventArgs e)
        {
            sessions = viewModel.ReceivedSessions;

            /*
             * Building the UI when no sessions are conducted.
             */
            if(sessions?.Count == 0)
            {
                Label label = new Label()
                {
                    Content = "No Sessions Conducted",
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 16
                };
                Stack.Children.Add(label);
                Trace.WriteLine("[Cloud] No Sessions Conducted by " + UserName);

                return;
            }

            /*
             * Building the UI when there are list of sessions conducted. 
             * Adding Buttons for each session the host has conducted.
             */
            Trace.WriteLine("[Cloud] Sessions data received to view");
            for (int i = 0; i < sessions?.Count; i++)
            {
                Button newButton = new Button();
                newButton.Height = 30;
                newButton.Margin = new Thickness(0, 5, 0, 5);
                newButton.Name = "Button" + i.ToString();
                newButton.Content = $"Session on - {sessions[i].Timestamp.Value.ToLocalTime()}";
                newButton.Click += OnButtonClick;
                
                Stack.Children.Add(newButton);
                Trace.WriteLine("[Cloud] Adding Button for the " + (i + 1) + "th Session");
            }

        }

        /// <summary>
        /// Handler to the sessions button press
        /// </summary>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[Cloud] Session Button pressed");
            Button caller = (Button)sender;
            int index = Convert.ToInt32(caller.Name.Split('n')[1]);

            // Getting the Corresponding Submissions of the selected sessions and showing it in the place provided
            submissionsPage = new SubmissionsPage( sessions[index].SessionId, UserName);
            Trace.WriteLine("[Cloud] SubmissionsPage created");
            SubmissionsPage.Content = submissionsPage;
        }

        /// <summary>
        /// Handler to the Refresh button press
        /// To refresh the session details that is retrieved.
        /// </summary>
        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[Cloud] Session Refresh Button pressed");
            viewModel.GetSessions(UserName);
        }
    }
}
