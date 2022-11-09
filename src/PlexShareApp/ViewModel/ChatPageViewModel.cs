/// <author> Sughandhan S </author>
/// <created> 03/11/2022 </created>
/// <summary>
///     The following is the ViewModel for our ChatPageView.
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using PlexShareContent.DataModels;
using System.Windows.Threading;
using System.Windows

namespace PlexShareApp.ViewModel
{
    public class ChatPageViewModel : INotifyPropertyChanged, IContentListner, IClientSessionNotificaions
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
        public IDictionary<int, string>? Users;

        /// <summary>
        /// Dictionary mapping Message IDs to their corresponding Message String
        /// </summary>
        public IDictionary<int, string>? Messages;

        /// <summary>
        /// Dictionary mapping Messages IDs to their ThreadIds
        /// </summary>
        public IDictionary<int, int>? ThreadIds;


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

                // TODO: Get data model from Dashboard module and subscribe to them
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
        public bool Testing { get; }


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