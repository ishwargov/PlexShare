/// <author> Sughandhan S </author>
/// <created> 03/11/2022 </created>
/// <summary>
///     The following is the ViewModel for our ChatPageView.
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using PlexShareContent.DataModels;
using System.Windows.Threading;
using System.Windows;
using PlexShareContent;
using PlexShareDashboard;
using PlexShareNetwork;
using PlexShareContent.Client;
using PlexShare.Dashboard.Client.SessionManagement;
using Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShare.Dashboard;
using System.Security.Cryptography.Xml;
using System.Diagnostics;

namespace PlexShareApp.ViewModel
{

    public class ChatPageViewModel : INotifyPropertyChanged, IContentListener, IClientSessionNotifications
    {
        /// <summary>
        /// Client Content Data Model
        /// </summary>
        private readonly IContentClient _model;

        /// <summary>
        /// Dashboard UX Data Model
        /// </summary>
        private readonly IUXClientSessionManager _modelDb;

        /// <summary>
        /// Dictionary mapping User IDs to their User names
        /// </summary>
        public IDictionary<int, string> Users;

        /// <summary>
        /// Dictionary mapping Message IDs to their corresponding Message String
        /// </summary>
        public IDictionary<int, string> Messages;

        /// <summary>
        /// Dictionary mapping Messages IDs to their ThreadIds
        /// </summary>
        public IDictionary<int, int> ThreadIds;


        public ChatPageViewModel(bool testing = false)
        {
            Users = new Dictionary<int, string>();
            Messages = new Dictionary<int, string>();
            ThreadIds = new Dictionary<int, int>();

            if(!testing)
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
        /// The current user id
        /// </summary>
        public static int UserId { get; private set; }

        /// <summary>
        /// Message to be sent
        /// </summary>
        public SendContentData MsgToSend { get; private set; }

        /// <summary>
        /// The received message
        /// </summary>
        public Message ReceivedMsg { get; private set; }

        /// <summary>
        /// True means testing mode
        /// </summary>
        public bool TestingMode { get; }


        /// <summary>
        ///     Gets the dispatcher to the main thread. In case it is not available
        ///     (such as during unit testing) the dispatcher associated with the
        ///     current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            Application.Current?.Dispatcher != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

        // TODO: COMPLETE CODE

        /// <summary>
        /// Whenever a property changes, a Property Changed event is raised
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Sends the message to content module. Message type is determined by messageType parameter
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

            if (!TestingMode)
            {
                Trace.WriteLine("UX: I am Sending a File Message");
                _model.ClientSendData(MsgToSend);
            }
        }

        public void OnAllMessagesReceived(List<ChatThread> allMessages)
        {
            throw new NotImplementedException();
        }

        public void OnClientSessionChanged(SessionData session)
        {
            throw new NotImplementedException();
        }

        public void OnMessageReceived(ReceiveContentData contentData)
        {
            throw new NotImplementedException();
        }

        // TODO: COMPLETE CODE

        /// <summary>
        /// Handling the Property Changed event raised 
        /// </summary>
        /// <param name="property">The name of the property.</param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}