/******************************************************************************
 * Filename    = ChatPageViewModel.cs
 *
 * Author      = Sughandhan S
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareApp
 *
 * Description = The following is the ViewModel for our ChatPageView.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.ComponentModel;
using PlexShareContent.DataModels;
using System.Windows.Threading;
using System.Windows;
using PlexShareContent;
using PlexShareContent.Client;
using PlexShare.Dashboard.Client.SessionManagement;
using Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShare.Dashboard;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.IO;

namespace PlexShareApp.ViewModel
{

    public class ChatPageViewModel : INotifyPropertyChanged, IContentListener, IClientSessionNotifications
    {
        /// <summary>
        ///     Content Client Data Model
        /// </summary>
        private readonly IContentClient _model;

        /// <summary>
        ///     Dashboard UX Data Model
        /// </summary>
        private readonly IUXClientSessionManager _modelDb;

        /// <summary>
        ///     Dictionary mapping User IDs to their User names
        /// </summary>
        public IDictionary<int, string> Users;

        /// <summary>
        ///     Dictionary mapping Message IDs to their corresponding Message String
        /// </summary>
        public IDictionary<int, string> Messages;

        /// <summary>
        ///     Dictionary mapping Messages IDs to their ThreadIds
        /// </summary>
        public IDictionary<int, int> ThreadIds;

        private readonly ObservableCollection<Message> myMessages;


        /// <summary>
        ///     Constructor for ViewModel
        /// </summary>
        /// <param name="production">true for production mode</param>
        public ChatPageViewModel(bool production = true)
        {
            Trace.WriteLine("[ChatPageViewModel] ViewModel setup");
            Users = new Dictionary<int, string>();
            Messages = new Dictionary<int, string>();
            ThreadIds = new Dictionary<int, int>();

            ProductionMode = production;
            if(ProductionMode)
            {
                // Getting Content Client model and subscribing to the content module
                _model = ContentClientFactory.GetInstance();
                _model.ClientSubscribe(this);

                // Get data model from Dashboard module and subscribe to them
                _modelDb = SessionManagerFactory.GetClientSessionManager();
                _modelDb.SubscribeSession(this);
            }
           
        }

        /// <summary>
        ///     The current user id
        /// </summary>
        public static int UserId { get; private set; }

        /// <summary>
        ///     Message to be sent
        /// </summary>
        public SendContentData MsgToSend { get; private set; }

        /// <summary>
        ///     The received message
        /// </summary>
        public Message ReceivedMsg { get; private set; }

        /// <summary>
        ///     True means Production mode
        /// </summary>
        public bool ProductionMode { get; }

        /// <summary>
        ///     Whenever a property changes, a Property Changed event is raised
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Handling the Property Changed event raised 
        /// </summary>
        /// <param name="property">The name of the property.</param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        ///     Sends the message to content module. Message type is determined by messageType parameter
        /// </summary>
        /// <param name="message"> The string containing the file path </param>
        /// <param name="replyMsgId"> Either the reply ID of the mesage being replied to or -1 denoting its not a reply message </param>
        /// <param name="messageType"> File or Chat </param>
        public void SendMessage(string message, int replyMsgId, string messageType)
        {

            // Creating a SendContentData object
            MsgToSend = new SendContentData();
            // Setting message type field
            if(messageType == "File")
            {
                MsgToSend.Type = MessageType.File;
            }
            else if(messageType == "Chat")
            {
                MsgToSend.Type = MessageType.Chat;
            }            

            // Setting the remaining fields of the SendContentData object
            MsgToSend.ReplyMessageID = replyMsgId;
            MsgToSend.Data = message;
            MsgToSend.ReplyThreadID = replyMsgId != -1 ? ThreadIds[replyMsgId] : -1;

            // Empty list denotes it's broadcast message
            MsgToSend.ReceiverIDs = new int[] { };

            if (ProductionMode)
            {
                if (messageType == "File")
                {
                    Trace.WriteLine("[ChatPageViewModel] I am Sending a File Message");
                }
                else if (messageType == "Chat")
                {
                    Trace.WriteLine("[ChatPageViewModel] I am Sending a Chat Message");
                }
                _model.ClientSendData(MsgToSend);
            }
        }

        /// <summary>
        ///     Download file to specific path on client machine
        /// </summary>
        /// <param name="savePath"> Path to which the file will be downloaded </param>
        /// <param name="msgId"> ID of the message </param>
        public void DownloadFile(string savePath, int msgId)
        {
            Trace.WriteLine("[ChatPageViewModel] Download Request.");
            _model.ClientDownload(msgId, savePath);
        }

        /// <summary>
        ///     Updating the message Data of Message ID with the New Message
        /// </summary>
        /// <param name="msgID"> Message ID </param>
        /// <param name="newMsg"> The updated Chat Message  </param>
        public void EditChatMsg(int msgID, string newMsg)
        {
            Trace.WriteLine("[ChatPageViewModel] Editing Chat Message.");
            _model.ClientEdit(msgID, newMsg);
        }

        /// <summary>
        ///     Updating the message Data of Message ID with the New Message
        /// </summary>
        /// <param name="msgID"> Message ID </param>
        public void DeleteChatMsg(int msgID)
        {
            Trace.WriteLine("[ChatPageViewModel] Deleting Chat Message.");
            _model.ClientDelete(msgID);
        }

        /// <summary>
        ///     Star message for it to be included in the dashboard summary
        /// </summary>
        /// <param name="msgId"> Id of the message </param>
        public void StarChatMsg(int msgId)
        {
            Trace.WriteLine("[ChatPageViewModel] Message has been starred.");
            _model.ClientStar(msgId);
        }

        /// <summary>
        ///     Gets the dispatcher to the main thread. In case it is not available
        ///     (such as during unit testing) the dispatcher associated with the
        ///     current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ? 
                    Application.Current.Dispatcher : 
                    Dispatcher.CurrentDispatcher;
        
        /// <summary>
        ///     Updating users
        /// </summary>
        /// <param name="session"></param>
        public void OnClientSessionChanged(SessionData currentSession)
        {
            // Execute the call on the application's main thread.
            //
            // Also note that we may execute the call asynchronously as the calling
            // thread is not dependent on the callee thread finishing this method call.
            // Hence we may call the dispatcher's BeginInvoke method which kicks off
            // execution async as opposed to Invoke which does it synchronously.

            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                      DispatcherPriority.Normal,
                      new Action<SessionData>(currentSession =>
                      {
                          lock (this)
                          {
                              if(currentSession!= null)
                              {
                                  Trace.WriteLine("[ChatPageViewModel] Users List Received.");
                                  Users.Clear();
                                  foreach (var user in currentSession.users)
                                  {
                                      // adding users for the session
                                      Users.Add(user.userID, user.username);
                                  }
                              }
                          }
                      }),
                      currentSession);
        }

        /// <summary>
        ///     When a new user joins, they receive the list of messages upto then
        /// </summary>
        /// <param name="allMessages"> List of all messages upto now </param>
        public void OnAllMessagesReceived(List<ChatThread> allMessages)
        {
            // Execute the call on the application's main thread.
            //
            // Also note that we may execute the call asynchronously as the calling
            // thread is not dependent on the callee thread finishing this method call.
            // Hence we may call the dispatcher's BeginInvoke method which kicks off
            // execution async as opposed to Invoke which does it synchronously.

            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                      DispatcherPriority.Normal,
                      new Action<List<ChatThread>>(allMessages =>
                      {
                          lock (this)
                          {
                              // clearing existing data entries
                              Messages.Clear();
                              ThreadIds.Clear();
                              // updating the Threads and Messages dictionary and displaying the chat upto now in the listbox in view
                              foreach (var messageList in allMessages)
                              {
                                  foreach (var message in messageList.MessageList)
                                  {
                                      Trace.WriteLine("[ChatPageViewModel] All messages have been received.");
                                      Messages.Add(message.MessageID, message.Data);
                                      ThreadIds.Add(message.MessageID, message.ReplyThreadID);

                                      if (ProductionMode)
                                      {
                                          // Getting user ID of current user
                                          UserId = _model.GetUserID();
                                      }

                                      // Creating object for the received message
                                      // Message object, ReceivedMsg, to be added to the new user's _allmessages
                                      // list upon property changed event
                                      ReceivedMsg = new Message();
                                      ReceivedMsg.MessageID = message.MessageID;
                                      ReceivedMsg.Type = message.Type == MessageType.Chat;
                                      ReceivedMsg.IncomingMessage = message.Data;
                                      ReceivedMsg.Time = message.SentTime.ToString("hh:mm tt"); 
                                      ReceivedMsg.Sender = Users.ContainsKey(message.SenderID) ? Users[message.SenderID] : "Anonymous";
                                      ReceivedMsg.ToFrom = UserId == message.SenderID;
                                      ReceivedMsg.ReplyMessage = message.ReplyMessageID == -1 ? "" : Messages[message.ReplyMessageID];

                                      // Propery Changed Event raised for updating View with current session's chat
                                      OnPropertyChanged("ReceivedAllMsgs");
                                  }
                              }
                          }
                      }),
                      allMessages);
        }

        /// <summary>
        ///     Handles the appropriate even on the received message
        /// </summary>
        /// <param name="contentData"> received messages meta data </param>
        public void OnMessageReceived(ReceiveContentData contentData)
        {
            // Execute the call on the application's main thread.
            //
            // Also note that we may execute the call asynchronously as the calling
            // thread is not dependent on the callee thread finishing this method call.
            // Hence we may call the dispatcher's BeginInvoke method which kicks off
            // execution async as opposed to Invoke which does it synchronously.

            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                      DispatcherPriority.Normal,
                      new Action<ReceiveContentData>(contentData =>
                      {
                          lock (this)
                          {
                              if(contentData.Event == PlexShareContent.Enums.MessageEvent.New)
                              {
                                  Trace.WriteLine("[ChatPageViewModel] New Message has been received.");
                                  Messages.Add(contentData.MessageID, contentData.Data);
                                  ThreadIds.Add(contentData.MessageID, contentData.ReplyThreadID);

                                  if (ProductionMode)
                                  {
                                      // Getting user ID of current user
                                      UserId = _model.GetUserID();
                                  }

                                  // Creating object for the received message
                                  // Message object, ReceivedMsg, to be added to the new user's _allmessages list upon property changed event
                                  ReceivedMsg = new Message();
                                  ReceivedMsg.MessageID = contentData.MessageID;
                                  ReceivedMsg.Type = contentData.Type == MessageType.Chat;
                                  ReceivedMsg.IncomingMessage = Path.GetFileName(contentData.Data);
                                  ReceivedMsg.Time = contentData.SentTime.ToString("hh:mm tt");
                                  ReceivedMsg.Sender = Users.ContainsKey(contentData.SenderID) ? Users[contentData.SenderID] : "Anonymous";
                                  ReceivedMsg.ToFrom = UserId == contentData.SenderID;
                                  ReceivedMsg.ReplyMessage = contentData.ReplyMessageID == -1 ? "" : Messages[contentData.ReplyMessageID];

                                  // Propery Changed Event raised for new message
                                  OnPropertyChanged("ReceivedMsg");
                              }
                              else if(contentData.Event == PlexShareContent.Enums.MessageEvent.Edit || contentData.Event == PlexShareContent.Enums.MessageEvent.Delete)
                              {
                                  Trace.WriteLine("[ChatPageViewModel] Going to Edit/Delete existing message.");
                                  // Events for editing and deleting are handled together due to their similarity
                                  if (contentData.Event == PlexShareContent.Enums.MessageEvent.Edit)
                                  {
                                      Trace.WriteLine("Editing a message");
                                  }
                                  else
                                  {
                                      Trace.WriteLine("Deleting a message");
                                  }

                                  if (ProductionMode)
                                  {
                                      
                                      UserId = _model.GetUserID();
                                  }

                                  // Creating object for the received message
                                  // Message object, ReceivedMsg, will modify the current user's _allmessages list upon property changed event
                                  ReceivedMsg = new Message();
                                  ReceivedMsg.MessageID = contentData.MessageID;
                                  ReceivedMsg.Type = contentData.Type == MessageType.Chat;
                                  ReceivedMsg.IncomingMessage = contentData.Data;
                                  ReceivedMsg.Time = contentData.SentTime.ToString("hh:mm tt");
                                  ReceivedMsg.Sender = Users.ContainsKey(contentData.SenderID) ? Users[contentData.SenderID] : "Anonymous";
                                  ReceivedMsg.ToFrom = UserId == contentData.SenderID;
                                  ReceivedMsg.ReplyMessage = contentData.ReplyMessageID == -1 ? "" : Messages[contentData.ReplyMessageID];
                                  Messages[contentData.MessageID] = ReceivedMsg.IncomingMessage;

                                  // Propery Changed Event raised for Edit/Delete
                                  OnPropertyChanged("EditOrDelete");
                              }
                          }
                      }),
                      contentData);
        }

    }
}