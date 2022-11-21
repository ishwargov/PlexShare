/******************************************************************************
 * Filename    = SessionsPage.xaml.cs
 *
 * Author      = B Sai Subrahmanyam
 *
 * Product     = PlexShareSolution
 * 
 * Project     = PlexShareApp
 *
 * Description = Defines the View of the Submissions Page.
 *****************************************************************************/

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
    /// Interaction logic for SubmissionsPage.xaml
    /// </summary>
    public partial class SubmissionsPage : Page
    {
        public SubmissionsPage(string sessionId, string userName)
        {
            InitializeComponent();

            viewModel = new SubmissionsViewModel(sessionId);
            this.DataContext = viewModel;
            viewModel.PropertyChanged += Listener;
            submissions = new List<SubmissionEntity> { };
            Trace.WriteLine("[Cloud] Submission View created Successfully");
        }

        /// <summary>
        /// ViewModel to use.
        /// </summary>
        private readonly SubmissionsViewModel viewModel;

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
                Trace.WriteLine("[Cloud] No Submissions detected");
                return;
            }

            /*
             * Building the UI when there are list of submissions made.
             * Adding an entry for each submission in the session with 
             * 4 columns - index, student id, submission time and download button.
             */
            for (int i = 0; i < submissions?.Count; i++)
            {
                Grid grid = new();
                ColumnDefinition column1 = new()
                {
                    Width = new GridLength(0.75, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(column1);
                ColumnDefinition column2 = new()
                {
                    Width = new GridLength(3, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(column2);
                ColumnDefinition column3 = new()
                {
                    Width = new GridLength(3, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(column3);
                ColumnDefinition column4 = new()
                {
                    Width = new GridLength(1.5, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(column4);

                //Index of the submission
                Label sNo = new()
                {
                    Content = i+1,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderBrush = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0,0,0,1),
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 16
                };
                Grid.SetColumn(sNo, 0);
                grid.Children.Add(sNo);

                //Student Id of the submission
                Label studentId = new()
                {
                    Content = submissions[i].UserName,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment= VerticalAlignment.Center,
                    BorderBrush = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0,0,0,1),
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 16
                };
                Grid.SetColumn(studentId, 1);
                grid.Children.Add(studentId);

                //Submissions time of the submission
                Label submissionTime = new()
                {
                    Content = submissions[i].Timestamp.Value.ToLocalTime(),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderBrush = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0,0,0,1),
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 16
                };
                Grid.SetColumn(submissionTime, 2);
                grid.Children.Add(submissionTime);

                //Download button of the submission
                Button button = new();
                button.Content = "Download PDF";
                button.Name = "Button" + i.ToString();
                button.Margin = new Thickness(5,5,5,5);
                button.Click += OnButtonClick;
                button.Background = new SolidColorBrush(Colors.White);
                Grid.SetColumn(button, 3);
                grid.Children.Add(button);

                Stack.Children.Add(grid);
                Trace.WriteLine("[Cloud] Adding Submission entry " + (i + 1));
            }
        }

        /// <summary>
        /// Handler to the download button press
        /// </summary>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[Cloud] Download Button pressed");
            Button caller = (Button)sender;
            int index = Convert.ToInt32(caller.Name.Split('n')[1]);
            viewModel.SubmissionToDownload = index;
        }

    }

}
