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
using PlexShareContent.DataModels;
using System.Diagnostics;
using Client.Models;
using OxyPlot;
using PlexShareContent;
using SharpDX.Direct3D11;
using ScottPlot.Renderable;
using System.Linq;
using Microsoft.VisualBasic;
using System.Windows.Interop;

namespace PlexShareApp
{
    public partial class ChatPageView : UserControl
    {

        /// <summary>
        /// All the messages upto now
        /// </summary>
        private readonly ObservableCollection<Message> _allMessages;
        private readonly Message addNewMessage;

        /// <summary>
        /// Creating an instance of our ChatPageView
        /// </summary>
        public ChatPageView()
        {
            InitializeComponent();

            var viewModel = new ChatPageViewModel(true);

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
        /// Property Changed Event in which the view gets updated with new messages
        /// </summary>
        /// <param name="sender"> Sender Notifying the event </param>
        /// <param name="e"> Property Changed Event </param>
        private void Listener(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName; ;
            var viewModel = DataContext as ChatPageViewModel;

            if(propertyName == "ReceivedMsg")
            {
                _allMessages.Add(viewModel.ReceivedMsg);
                Debug.WriteLine(_allMessages);
                UpdateScrollBar(MainChat);
            }
            else if(propertyName == "ReceivedAllMsgs")
            {
                _allMessages.Add(viewModel.ReceivedMsg);
                Debug.WriteLine(_allMessages);
                //UpdateScrollBar(MainChat);
            }
            else if(propertyName == "EditOrDelete")
            {
                Message updatedMsg = null;
                string replyMsgOld = "";
                string replyMsgNew = "";
                for (int i = 0; i < _allMessages.Count; i++)
                {
                    var message = _allMessages[i];
                    if (message.MessageID == viewModel.ReceivedMsg.MessageID)
                    {
                        replyMsgOld = message.IncomingMessage;
                        replyMsgNew = viewModel.ReceivedMsg.IncomingMessage;
                        Message toUpd = new Message();
                        toUpd.ToFrom = message.ToFrom;
                        toUpd.ReplyMessage = message.ReplyMessage;
                        toUpd.Sender = message.Sender;
                        toUpd.Time = message.Time;
                        toUpd.MessageID = message.MessageID;
                        toUpd.Type = message.Type;
                        toUpd.IncomingMessage = viewModel.ReceivedMsg.IncomingMessage;
                        // updating
                        _allMessages[i] = toUpd;
                        updatedMsg = _allMessages[i];
                    }
                }

                // Updating all the Chat bubbles which all have replied to this message that has been Editted/Deleted
                for (int i=0;i < _allMessages.Count;i++)
                {
                    var message = _allMessages[i];
                    if(message.ReplyMessage == replyMsgOld)
                    {
                        Message toUpd = new Message();
                        toUpd.ToFrom = message.ToFrom;
                        toUpd.ReplyMessage = replyMsgNew;
                        toUpd.Sender = message.Sender;
                        toUpd.Time = message.Time;
                        toUpd.MessageID = message.MessageID;
                        toUpd.Type = message.Type;
                        toUpd.IncomingMessage = message.IncomingMessage;
                        // updating
                        _allMessages[i] = toUpd;
                        updatedMsg = _allMessages[i];
                    }
                }
                Debug.WriteLine($"Message ID {updatedMsg.MessageID} was updated with {updatedMsg.IncomingMessage}");
                Debug.WriteLine(_allMessages);
            }
            //else if (propertyName == "StarMessage")
            //{
            //    for (int i = 0; i < _allMessages.Count; i++)
            //    {
            //        if (_allMessages[i].MessageID == viewModel.ReceivedMsg.MessageID)
            //        {
            //            _allMessages[i].
            //        }
            //    }
            //}
        }

        private void UploadButtonClick(object sender, RoutedEventArgs e)
        {
            if(ReplyTextBox.Text == String.Empty)
            {
                var viewModel = DataContext as ChatPageViewModel;

                // Create OpenFileDialog
                var openFileDialog = new OpenFileDialog();

                // Launch OpenFileDialog by calling ShowDialog method
                var result = openFileDialog.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    if (string.IsNullOrEmpty(ReplyTextBox.Text))
                    {
                        viewModel.SendMessage(openFileDialog.FileName, -1, "File");
                    }
                    else
                    {
                        viewModel.SendMessage(openFileDialog.FileName, ReplyMsgId, "File");
                    }
                    //addNewMessage = new Message();
                    //addNewMessage.MessageID = -1;
                    //addNewMessage.Sender = null;
                    //addNewMessage.Time = DateTime.Now.ToShortTimeString();
                    //addNewMessage.Type = true;
                    //addNewMessage.ReplyMessage = null;
                    //addNewMessage.IncomingMessage = openFileDialog.FileName;
                    //addNewMessage.ToFrom = true;
                    //_allMessages.Add(addNewMessage);


                    SendTextBox.Text = string.Empty;
                    ReplyTextBox.Text = string.Empty;
                    //ReplyTextBox.Text = "";
                }
            }
        }

        /// <summary>
        /// Event Handler on Clicking Send Button
        /// </summary>
        /// <param name="sender"> Notification Sender </param>
        /// <param name="e"> Routed Event Data </param>
        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(SendTextBox.Text))
            {
                if(SendTextBox.Text.Length > 300)
                {
                    MessageBox.Show("Please enter less than 300 characters!");
                    return;
                }
                var viewModel = DataContext as ChatPageViewModel;

                if(string.IsNullOrEmpty(ReplyTextBox.Text))
                {
                    viewModel.SendMessage(SendTextBox.Text, -1, "Chat");
                }
                else
                {
                    viewModel.SendMessage(SendTextBox.Text, ReplyMsgId, "Chat");
                }
                //var chumma = SendTextBox.Text;
                //addNewMessage = new Message();
                //addNewMessage.MessageID = 2;
                //addNewMessage.Sender = null;
                //addNewMessage.Time = DateTime.Now.ToShortTimeString();
                //addNewMessage.Type = true;
                //addNewMessage.ReplyMessage = "Hey";
                //addNewMessage.IncomingMessage = chumma;//SendTextBox.Text;
                //addNewMessage.ToFrom = true;
                //_allMessages.Add(addNewMessage);


                SendTextBox.Text = string.Empty;
                ReplyTextBox.Text = string.Empty;
            }
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
                    if(msg.IncomingMessage!= "Message Deleted.")
                    {
                        ReplyTextBox.Text = msg.IncomingMessage;
                        ReplyMsgId = msg.MessageID;
                    }
                }
            }
        }

        private void ClearReplyBox(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                ReplyTextBox.Text = null;
            }
        }

        /// <summary>
        /// Event Handler on Clicking Edit Button
        /// </summary>
        /// <param name="sender"> Notification Sender</param>
        /// <param name="e"> Routed Event Data </param>
        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SendTextBox.Text))
            {
                if (sender is Button)
                {
                    var senderButton = (Button)sender;
                    if (senderButton.DataContext is Message)
                    {
                        var viewModel = DataContext as ChatPageViewModel;
                        Message msg = (Message)senderButton.DataContext;
                        if (msg.IncomingMessage != "Message Deleted.")
                        {
                            var ourEditMessage = SendTextBox.Text;
                            viewModel.EditChatMsg(msg.MessageID, ourEditMessage);
                            SendTextBox.Text = string.Empty;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event Handler on Clicking Delete Button
        /// </summary>
        /// <param name="sender"> Notification Sender</param>
        /// <param name="e"> Routed Event Data </param>
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                if (senderButton.DataContext is Message)
                {
                    var viewModel = DataContext as ChatPageViewModel;
                    Message msg = (Message)senderButton.DataContext;
                    viewModel.DeleteChatMsg(msg.MessageID);
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
            var viewModel = DataContext as ChatPageViewModel;
            if (sender is RadioButton)
            {
                RadioButton senderRadioButton = (RadioButton)sender;

                if (senderRadioButton.DataContext is Message)
                {
                    Message msg = (Message)senderRadioButton.DataContext;
                    //var viewModel = DataContext as ChatPageViewModel;
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
