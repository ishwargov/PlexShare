/******************************************************************************
 * Filename    = SessionsPage.xaml.cs
 *
 * Author      = B Sai Subrahmanyam
 *
 * Product     = PlexShareSolution
 * 
 * Project     = PlexShareApp
 *
 * Description = Defines the View of the Upload Page.
 *****************************************************************************/

using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Win32;
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
using static System.Net.Mime.MediaTypeNames;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for UploadPage.xaml
    /// </summary>
    public partial class UploadPage : Page
    {
        public UploadPage(string sessionId, string userName, bool isServer)
        {
            InitializeComponent();

            UploadViewModel viewModel = new UploadViewModel(sessionId,userName,isServer);
            viewModel.PropertyChanged += UploadInfoUpdate;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Path to the submitted file
        /// </summary>
        public string FilePath;

        /// <summary>
        /// OnPropertyChange handler to trigger when the pdf is uploaded.
        /// </summary>
        private void UploadInfoUpdate(object sender, PropertyChangedEventArgs e)
        {
            Status.Content = "File Submitted";
            LastModified.Content = DateTime.Now;
            string[] array = FilePath.Split('\\');
            PDFName.Content = array[array.Length - 1];
            Submit.Visibility = Visibility.Hidden;
            Upload.Visibility = Visibility.Visible;
            Loading.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Handler to upload button press
        /// </summary>
        private void UploadButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".pdf"; // Required file extension 
            fileDialog.Filter = "PDF (.pdf)|*.pdf"; // Optional file extensions
            fileDialog.InitialDirectory = @"C:\";

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                Submit.Visibility = Visibility.Visible;
                Upload.Visibility = Visibility.Hidden;
                FilePath = fileDialog.FileName;
                string[] array = fileDialog.FileName.Split('\\');
                PDFName.Content = array[array.Length - 1];
            }
        }

        /// <summary>
        /// Handler to submit button press
        /// </summary>
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            Loading.Visibility = Visibility.Visible;
            UploadViewModel viewModel = this.DataContext as UploadViewModel;
            viewModel.UploadFilePath = FilePath;
        }
    }
}
