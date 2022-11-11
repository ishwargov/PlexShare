/******************************************************************************
 * Filename    = SessionsPage.xaml.cs
 *
 * Author      = B Sai Subrahmanyam
 *
 * Product     = PlexShareSolution
 * 
 * Project     = PlexShareAppCloudUX
 *
 * Description = Defines the View Model of the Upload Page.
 *****************************************************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PlexShareCloudUX
{
    public class UploadViewModel :
        INotifyPropertyChanged // Notifies clients that a property value has changed.
        //IMessageListener        // Notifies clients that has a message has been received.
    {
        /// <summary>
        /// Creates an instance of the Upload ViewModel.
        /// <param name="sessionId">Id of the session of the current session.</param>
        /// <param name="userName">The username of the user.</param>
        /// </summary>
        public UploadViewModel(string sessionId, string userName)
        {
            _model = new UploadModel(sessionId, userName);
        }
        
        /// <summary>
        /// Path of the file to be uploaded.
        /// </summary>
        public string UploadFilePath
        {
            set => Function(value);
        }

        /// <summary>
        /// Function to call the UploadDocument function of the model when the path is given
        /// <param name="path"">Path of the file to be uploaded.</param>
        /// </summary>
        public void Function(string path)
        {
            bool isUploaded = _model.UploadDocument(path);
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<bool>((isUploded) =>
                        {
                            lock (this)
                            {
                                if (isUploaded)
                                {
                                    this.OnPropertyChanged("DocumentUploded");
                                }
                            }
                        }),
                        isUploaded);
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
        private UploadModel _model;
    }

}
