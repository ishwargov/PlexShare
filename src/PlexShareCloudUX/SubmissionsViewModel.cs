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
    public class SubmissionsViewModel :
        INotifyPropertyChanged // Notifies clients that a property value has changed.
    {
        /// <summary>
        /// Creates an instance of the Submissions ViewModel.
        /// Gets the details of the submissions of the session conducted by the user.
        /// Then dispatch the changes to the view.
        /// <param name="sessionId">Id of the session for which we want the submissions.</param>
        /// </summary>
        public SubmissionsViewModel(string sessionId)
        {
            _model = new SubmissionsModel();
            GetSubmissions(sessionId);
        }

        public async void GetSubmissions(string sessionId)
        {
            IReadOnlyList<SubmissionEntity> submissionsList = await _model.GetSubmissions(sessionId);
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<IReadOnlyList<SubmissionEntity>>((submissionsList) =>
                        {
                            lock (this)
                            {
                                // Note that Bitmap cannot be automatically marshalled to the main thread
                                // if it were created on the worker thread. Hence the data model just passes
                                // the path to the image, and the main thread creates an image from it.

                                this.ReceivedSubmissions = submissionsList;

                                this.OnPropertyChanged("ReceivedSubmissions");
                            }
                        }),
                        submissionsList);
        }

        /// <summary>
        /// To store which pdf to download.
        /// </summary>
        public int SubmissionToDownload
        {
            set => _model.DownloadPdf(value);
        }

        /// <summary>
        /// The received submissions.
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
        private SubmissionsModel _model;
    }
}
