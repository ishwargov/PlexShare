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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PlexShareCloudUX
{
    public class UploadViewModel :
        INotifyPropertyChanged // Notifies clients that a property value has changed.
    {
        /// <summary>
        /// Creates an instance of the Upload ViewModel.
        /// <param name="sessionId">Id of the session of the current session.</param>
        /// <param name="userName">The username of the user.</param>
        /// </summary>
        public UploadViewModel(string sessionId, string userName, bool isServer)
        {
            _model = new UploadModel(sessionId, userName, isServer);
            Trace.WriteLine("[Cloud] Upload View Model created");
        }
        
        /// <summary>
        /// Path of the file to be uploaded.
        /// Call the function to upload the file once the value of the path is set.
        /// </summary>
        public string UploadFilePath
        {
            set => UploadFile(value);
        }

        /// <summary>
        /// Function to call the UploadDocument function of the model when the path is given.
        /// Dispatch the succuessful submission information to the view.
        /// <param name="path"">Path of the file to be uploaded.</param>
        /// </summary>
        public async void UploadFile(string path)
        {
            bool isUploaded = await _model.UploadDocument(path);
            if (isUploaded)
                Trace.WriteLine("[Cloud] File Uploaded successfully");
            else
                Trace.WriteLine("[Cloud] File not Uploaded");
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
        public UploadModel _model;
    }

}
