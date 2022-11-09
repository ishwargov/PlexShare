﻿/// <author>Sughandhan S</author>
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

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {

        }

        // TODO: Implement ReplyButtonClick event

        // TODO: Implement StarButtonClick event

        // TODO: Implement DownloadButtonClick event

        // TODO: Implement UpdateScrollBar event
    }
}