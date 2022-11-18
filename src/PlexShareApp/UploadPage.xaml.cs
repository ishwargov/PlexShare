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

            PdfName = "-";
            UploadViewModel viewModel = new UploadViewModel(sessionId,userName,isServer);
            viewModel.PropertyChanged += UploadInfoUpdate;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Name of th epdf submistted
        /// </summary>
        public string PdfName;

        /// <summary>
        /// OnPropertyChange handler to trigger when the pdf is uploaded.
        /// </summary>
        private void UploadInfoUpdate(object sender, PropertyChangedEventArgs e)
        {
            Status.Content = "File Submitted";
            LastModified.Content = DateTime.Now;
            PDFName.Content = PdfName;
        }

        /// <summary>
        /// Handler to submit button press
        /// </summary>
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".pdf"; // Required file extension 
            fileDialog.Filter = "PDF (.pdf)|*.pdf"; // Optional file extensions
            fileDialog.InitialDirectory = @"C:\";

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                UploadViewModel viewModel = this.DataContext as UploadViewModel;
                viewModel.UploadFilePath = fileDialog.FileName;
                PdfName = fileDialog.FileName;
            }
        }

        /// <summary>
        /// Handler to back button press
        /// </summary>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ////Remove the current page
        }

    }
}
