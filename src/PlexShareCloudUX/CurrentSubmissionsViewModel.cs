/******************************************************************************
 * Filename    = SessionsPage.xaml.cs
 *
 * Author      = B Sai Subrahmanyam
 *
 * Product     = PlexShareSolution
 * 
 * Project     = PlexShareAppCloudUX
 *
 * Description = Defines the View Model of the Current Submissions Page.
 *****************************************************************************/
using PlexShareCloud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Diagnostics;

namespace PlexShareCloudUX
{
    public class CurrentSubmissionsViewModel :
        INotifyPropertyChanged      // Notifies clients that a property value has changed.
    {
        /// <summary>
        /// Creates an instance of the CurrentSubmissions ViewModel.
        /// Gets the details of the submission made in this session.
        /// Then dispatch the changes to the view.
        /// <param name="sessionId">The unique session id of the session</param>
        /// </summary>
        public CurrentSubmissionsViewModel(string sessionId)
        {
            _model = new CurrentSubmissionsModel();
            GetSubmissions(sessionId);
            Trace.WriteLine("[Cloud] Current Submission View Model Created");
        }

        /// <summary>
        /// Gets the details of the submission made in this session.
        /// Then dispatch the changes to the view.
        /// <param name="sessionId">The unique session id of the session</param>
        /// </summary>
        public async void GetSubmissions(string sessionId)
        {
            IReadOnlyList<SubmissionEntity> submissionsList = await _model.GetSubmissions(sessionId);
            Trace.WriteLine("[Cloud] Submission Received");
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<IReadOnlyList<SubmissionEntity>>((submissionsList) =>
                        {
                            lock (this)
                            {
                                this.ReceivedSubmissions = submissionsList;

                                this.OnPropertyChanged("ReceivedSubmissions");
                            }
                        }),
                        submissionsList);
        }

        /// <summary>
        /// List to store the Submission recieved in this session.
        /// </summary>
        public IReadOnlyList<SubmissionEntity>? ReceivedSubmissions
        {
            get; private set;
        }

        /// <summary>
        /// Property changed event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        public void OnPropertyChanged(string property)
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
        private CurrentSubmissionsModel _model;

    }
}
