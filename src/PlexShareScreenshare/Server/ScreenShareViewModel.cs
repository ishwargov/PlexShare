using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
using PlexShareScreenshare;
using PlexShareScreenshare.Client;

namespace PlexShareScreenshare.Server
{
    public class ScreenShareViewModel
    {
        public ObservableCollection<SharedClient> ImageList { get; set; }
        public string Color { get; set; }
        public ScreenShareViewModel(ObservableCollection<SharedClient> list, string c)
        {
            ImageList = list;
            Color = c;
        }

        public void AddImage(SharedClient map)
        {
            Debug.WriteLine(map.ToString());
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<SharedClient>((image) =>
                        {
                            lock (this)
                            {
                                // Note that Bitmap cannot be automatically marshalled to the main thread
                                // if it were created on the worker thread. Hence the data model just passes
                                // the path to the image, and the main thread creates an image from it.

                                ImageList.Add(image);

                                Debug.WriteLine("Hi");

                                OnPropertyChanged("ImageList");
                                //this.OnPropertyChanged("ReceivedCaption");
                            }
                        }),
                        map);
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
            Debug.WriteLine(property);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Gets the dispatcher to the main thread. In case it is not available
        /// (such as during unit testing) the dispatcher associated with the
        /// current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            Application.Current?.Dispatcher != null ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;
    }
}
