/// <author>Sughandhan S</author>
/// <created>03/11/2022</created>
/// <summary>
///     Interaction logic for ChatPageView.xaml.
/// </summary>

using System;
using System.Collections.ObjectModel;
using System.IO;
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
                Button senderButton = (Button)sender;
                if(senderButton.DataContext is Message)
                {
                    Message msg = (Message)senderButton.DataContext;
                    ReplyTextBox.Text = msg.IncomingMessage;
                    ReplyMsgId = msg.MessageID;
                }
            }
        }

        /// <summary>
        /// Event Handler for Clicking on Star Radio Button
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void StarButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                RadioButton senderRadioButton = (RadioButton)sender;

                if (senderRadioButton.DataContext is Message)
                {
                    Message msg = (Message)senderRadioButton.DataContext;
                    var viewModel = DataContext as ChatPageViewModel;
                    viewModel.StarChatMsg(msg.MessageID);
                }

            }

        }

        /// <summary>
        /// Event Handler on clicking Download Button
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var viewModel = DataContext as ChatPageViewModel;

                // Creating a SaveFileDialog
                SaveFileDialog dailogFile = new SaveFileDialog();

                Button senderButton = (Button)sender;
                if (senderButton.DataContext is Message)
                {
                    // Getting the message through download button
                    Message message = (Message)senderButton.DataContext;

                    // Set the default file name and extension
                    dailogFile.FileName = Path.GetFileNameWithoutExtension(message.IncomingMessage);
                    dailogFile.DefaultExt = Path.GetExtension(message.IncomingMessage);

                    // Display save file dialog box
                    var result = dailogFile.ShowDialog();

                    // if Download OK
                    if (result == true)
                    {
                        viewModel.DownloadFile(dailogFile.FileName, message.MessageID);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the Scrollbar to the bottom of the listbox
        /// </summary>
        /// <param name="listBox"> Listbox containing the scrollbar </param>
        private void UpdateScrollBar(ListBox listBox)
        {
            if ((listBox != null) && (VisualTreeHelper.GetChildrenCount(listBox) != 0))
            {
                var border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }

    }
}
