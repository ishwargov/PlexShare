/// <author>Sughandhan S</author>
/// <created>03/11/2022</created>
/// <summary>
///     Interaction logic for ChatPageView.xaml.
/// </summary>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            // Subscribed to the Property Changed Event
            viewModel.PropertyChanged += Listener;
            DataContext = viewModel;

            // Binding all the messages
            _allMessages = new ObservableCollection<Message>();
            MainChat.ItemsSource = _allMessages;
            
        }

        /// <summary>
        /// Replied message's Message ID 
        /// </summary>
        public int ReplyMsgId { get; set; }

        /// <summary>
        /// Property Changed Evenet in which the view gets updated with new messages
        /// </summary>
        /// <param name="sender"> Sender Notifying the event </param>
        /// <param name="e"> Property Changed Event </param>
        private void Listener(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName; ;
            var viewModel = DataContext as ChatPageViewModel;

            if(propertyName == "ReceivedMsg" || propertyName == "ReceivedAllMsgs")
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

            // Process open file dialog box results
            if(result == true)
            {
                if(string.IsNullOrEmpty(ReplyTextBox.Text))
                {
                    viewModel.SendMessage(openFileDialog.FileName, -1, "File");
                }
                else
                {
                    viewModel.SendMessage(openFileDialog.FileName, ReplyMsgId, "File");
                }
                ReplyTextBox.Text = "";
            }
        }

        /// <summary>
        /// Event Handler on Clicking Send Button
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if(SendTextBox.Text != null && SendTextBox.Text != string.Empty)
            {
                var viewModel = DataContext as ChatPageViewModel;

                if(ReplyTextBox.Text != null && ReplyTextBox.Text != string.Empty)
                {
                    viewModel.SendMessage(SendTextBox.Text, ReplyMsgId, "Chat");
                }
                else
                {
                    viewModel.SendMessage(SendTextBox.Text, -1, "Chat");
                }
            }
            SendTextBox.Text = string.Empty;
            ReplyTextBox.Text = string.Empty;
        }

        // TODO: Implement ReplyButtonClick event
        /// <summary>
        /// Event Handler on Clicking Reply Button
        /// </summary>
        /// <param name="sender"> Notification Sender</param>
        /// <param name="e"> Routed Event Data </param>
        private void ReplyButtonClick(object sender, RoutedEventArgs e)
        {
            if(sender is Button)
            {
                var cmd = (Button)sender;
                if(cmd.DataContext is Message)
                {
                    var msg = (Message)cmd.DataContext;
                    ReplyTextBox.Text = msg.IncomingMessage;
                    ReplyMsgId = msg.MessageID;
                }
            }
        }

        // TODO: Implement StarButtonClick event
        private void StarButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChatPageViewModel;

            if(sender is RadioButton)
            {
                var cmd = (RadioButton)sender;

                if(cmd.DataContext is Message)
                {
                    var msg = (Message)cmd.DataContext;
                    viewModel.StarChatMsg(msg.MessageID);
                }

            }

        }

        // TODO: Implement DownloadButtonClick event

        // TODO: Implement UpdateScrollBar event
    }
}
