using PlexShareCloud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace PlexShareCloudUX
{
    public class SessionsViewModel :
        INotifyPropertyChanged // Notifies clients that a property value has changed.
    {
        /// <summary>
        /// Creates an instance of the Sessions ViewModel.
        /// Gets the details of the sessions conducted by the user.
        /// Then dispatch the changes to the view.
        /// <param name="userName">The username of the user.</param>
        /// </summary>
        public SessionsViewModel(string userName)
        {
            _model = new();
            GetSessions(userName);
        }

        public async void GetSessions(string userName)
        {
            IReadOnlyList<SessionEntity> sessionsList = await _model.GetSessionsDetails(userName);

            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<IReadOnlyList<SessionEntity>>((sessionsList) =>
                        {
                            lock (this)
                            {
                                this.ReceivedSessions = sessionsList;

                                this.OnPropertyChanged("ReceivedSessions");
                            }
                        }),
                        sessionsList);
        }

        /// <summary>
        /// List to store the sessions conducted.
        /// </summary>
        public IReadOnlyList<SessionEntity>? ReceivedSessions;

        /// <summary>
        /// Property changed event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Gets the dispatcher to the main thread. In case it is not available
        /// (such as during unit testing) the dispatcher associated with the
        /// current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Underlying data model.
        /// </summary>
        private SessionsModel _model;
    }

}
