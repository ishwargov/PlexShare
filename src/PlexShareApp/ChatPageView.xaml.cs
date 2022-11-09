/// <author>Sughandhan S</author>
/// <created>03/11/2022</created>
/// <summary>
///     Interaction logic for ChatPageView.xaml.
/// </summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Win32;
using PlexShareApp.ViewModel;

namespace PlexShareApp
{
    public partial class ChatPageView : UserControl
    {

        /// <summary>
        /// All the messages upto now
        /// </summary>
        private readonly ObservableCollection<Message> _allMessages;

        /// <summary>
        /// Creating an instance of our ChatPageView
        /// </summary>
        public ChatPageView()
        {
            InitializeComponent();

            var viewModel = new ChatPageViewModel();

            // TODO: Subscribe to the Property Changed Event


            _allMessages = new ObservableCollection<Message>();
            
            // TODO: Binding all the messages
            
        }

        /// <summary>
        /// Replied message's Message ID 
        /// </summary>
        public int ReplyMsgId { get; set; }

        /// <summary>
        /// Upa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Listner(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName; ;
            var viewModel = DataContext as ChatPageViewModel;

            if(propertyName == "ReceivedMsg")
            {
                _allMessages.Add(viewModel.ReceivedMsg);
            }
            else if(propertyName == "ReceivedMsgs")
            {
                _allMessages.Add(viewModel.ReceivedMsg);
            }
        }


        private void UploadButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChatPageViewModel;

            // Create OpenFileDialog
            var openFileDialog = new OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            var result = openFileDialog.ShowDialog();

            // Process open file diaalog box results
            if(result == true)
            {
                if(string.IsNullOrEmpty(ReplyTextBox.Text))
                {
                    viewModel.SendFile(openFileDialog.FileName, -1);
                }
                else
                {
                    viewModel.SendFile(openFileDialog.FileName, ReplyMsgId);
                }
                ReplyTextBox.Text = "";
            }
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {

        }

        // TODO: Implement ReplyButtonClick event

        // TODO: Implement StarButtonClick event

        // TODO: Implement DownloadButtonClick event

        // TODO: Implement UpdateScrollBar event
    }
}
