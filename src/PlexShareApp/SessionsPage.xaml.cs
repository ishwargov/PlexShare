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

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for SessionsPage.xaml
    /// </summary>
    public partial class SessionsPage : Page
    {
        private static SubmissionsPage? submissionsPage;
        public SessionsPage(string userName)
        {
            InitializeComponent();

            UserName = userName;
            //viewModel = new SessionsViewModel(userName);
            //this.DataContext = viewModel;
            //viewModel.PropertyChanged += Listener;
            //sessions = new List<SessionEntity> { };
        }

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
                    Content = "No Sessions Conducted"
                };
                Stack.Children.Add(label);

                Button backButton1 = new Button()
                {
                    Width = 60,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10, 10, 10, 10),
                    Content = "Back"
                };
                backButton1.Click += BackButtonClick;
                Stack.Children.Add(backButton1);

                return;
            }

            /*
             * Building the UI when there are list of sessions conducted.
             */
            for (int i = 0; i < sessions?.Count; i += 4)
            {
                StackPanel newStackPanel = new StackPanel();
                newStackPanel.Orientation = Orientation.Vertical;
                newStackPanel.Margin = new Thickness(10, 10, 10, 10);

                for (int j = 0; (j < 4 && (i + j) < sessions.Count); j++)
                {
                    Button newButton = new Button();
                    newButton.Height = 100;
                    newButton.Width = 100;
                    newButton.Padding = new Thickness(10, 10, 10, 10);
                    newButton.Margin = new Thickness(25, 0, 25, 0);
                    newButton.Name = sessions[i + j].SessionId;
                    newButton.Content = $"Session Conducted on - {sessions[i + j].Timestamp}";
                    newButton.Click += OnButtonClick;
                    newStackPanel.Children.Add(newButton);
                }

                Stack.Children.Add(newStackPanel);
            }

            Button backButton = new Button()
            {
                Width = 60,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 10, 10, 10),
                Content = "Back"
            };
            backButton.Click += BackButtonClick;
            Stack.Children.Add(backButton);

        }

        /// <summary>
        /// Handler to the sessions button press
        /// </summary>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Button caller = (Button)sender;
            submissionsPage = new SubmissionsPage(caller.Name, UserName);
            //Shift to submissions view
        }

        /// <summary>
        /// Handler to the back button press.
        /// </summary>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            //Remove the current page
        } 

    }
}
