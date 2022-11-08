using PlexShareCloud;
using PlexShareCloudUX;
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

            viewModel = new SubmissionsViewModel(sessionId, userName);
            this.DataContext = viewModel;
            viewModel.PropertyChanged += Listener;
            submissions = new List<SubmissionEntity> { };
        }

        /// <summary>
        /// ViewModel to use.
        /// </summary>
        private readonly SubmissionsViewModel viewModel;

        /// <summary>
        /// List of submissions made.
        /// </summary>
        public List<SubmissionEntity>? submissions;

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
                    Content = "No Submissions Available"
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
             * Building the UI when there are list of submissions made.
             */
            for (int i = 0; i < submissions?.Count; i++)
            {
                Grid grid = new();
                for (int j = 0; j < 5; j++)
                {
                    ColumnDefinition column = new()
                    {
                        Width = new GridLength(5, GridUnitType.Star)
                    };
                    grid.ColumnDefinitions.Add(column);
                }

                Label sNo = new()
                {
                    Content = i,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };
                Grid.SetColumn(sNo, 0);
                grid.Children.Add(sNo);

                Label studentId = new()
                {
                    Content = submissions[i].UserName,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };
                Grid.SetColumn(studentId, 1);
                grid.Children.Add(studentId);

                Label submissionTime = new()
                {
                    Content = submissions[i].Timestamp,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };
                Grid.SetColumn(submissionTime, 2);
                grid.Children.Add(submissionTime);

                Label pdfName = new()
                {
                    Content = "",
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };
                Grid.SetColumn(pdfName, 3);
                grid.Children.Add(pdfName);

                Button button = new();
                button.Name = i.ToString();
                button.Click += OnButtonClick;
                Grid.SetColumn(button, 4);
                grid.Children.Add(button);

                Stack.Children.Add(grid);

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
        /// Handler to the download button press
        /// </summary>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Button caller = (Button)sender;
            viewModel.SubmissionToDownload = Convert.ToInt32(caller.Name);
        }

        /// <summary>
        /// Handler to the back button press
        /// </summary>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            //Remove the current page
        }

    }

}
