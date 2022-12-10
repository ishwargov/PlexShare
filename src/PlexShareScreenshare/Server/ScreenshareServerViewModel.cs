/// <author>Mayank Singla</author>
/// <summary>
/// Defines the "ScreenshareServerViewModel" class which represents the
/// view model for screen sharing on the server side machine.
/// </summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PlexShareScreenshare.Server
{
    /// <summary>
    /// Represents the view model for screen sharing on the server side machine.
    /// </summary>
    public class ScreenshareServerViewModel :
        INotifyPropertyChanged, // Notifies the UX that a property value has changed.
        IMessageListener,       // Notifies the UX that subscribers list has been updated.
        IDisposable             // Handle cleanup work for the allocated resources.
    {
        /// <summary>
        /// The only singleton instance for this class.
        /// </summary>
        private static ScreenshareServerViewModel? _instance;

        /// <summary>
        /// Underlying data model.
        /// </summary>
        private readonly ScreenshareServer? _model;

        /// <summary>
        /// List of all the clients sharing their screens. This list first contains
        /// the clients which are marked as pinned and then the rest of the clients
        /// in lexicographical order of their name.
        /// </summary>
        private List<SharedClientScreen> _subscribers;

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The clients which are on the current page.
        /// </summary>
        private readonly ObservableCollection<SharedClientScreen> _currentWindowClients;

        /// <summary>
        /// The current page that the server is viewing.
        /// </summary>
        private int _currentPage;

        /// <summary>
        /// The total number of pages.
        /// </summary>
        private int _totalPages;

        /// <summary>
        /// Whether the current page that the server is viewing is last page or not.
        /// </summary>
        private bool _isLastPage;

        /// <summary>
        /// The current number of rows of the grid displayed on the screen.
        /// </summary>
        private int _currentPageRows;

        /// <summary>
        /// The current number of columns of the grid displayed on the screen.
        /// </summary>
        private int _currentPageColumns;

        /// <summary>
        /// Whether the popup is open or not.
        /// </summary>
        private bool _isPopupOpen;

        /// <summary>
        /// The text to be displayed on the popup.
        /// </summary>
        private string _popupText;

        /// <summary>
        /// The dispatcher operation returned from the calls to BeginInvoke.
        /// </summary>
        /// <remarks>
        /// They are only required for unit tests.
        /// </remarks>
        private DispatcherOperation? _updateViewOperation, _displayPopupOperation;

        /// <summary>
        /// Creates an instance of the "ScreenshareServerViewModel" which represents the
        /// view model for screen sharing on the server side. It also instantiates the instance
        /// of the underlying data model.
        /// </summary>
        /// <param name="isDebugging">
        /// If we are in debugging mode.
        /// </param>
        protected ScreenshareServerViewModel(bool isDebugging)
        {
            // Get the instance of the underlying data model.
            _model = ScreenshareServer.GetInstance(this, isDebugging);

            // Always display the first page initially.
            _currentPage = InitialPageNumber;

            // Initialize rest of the fields.
            _subscribers = new();
            _disposed = false;
            _currentWindowClients = new();
            _totalPages = InitialTotalPages;
            _isLastPage = InitialIsLastPage;
            _currentPageRows = InitialNumberOfRows;
            _currentPageColumns = InitialNumberOfCols;
            _isPopupOpen = false;
            _popupText = "";

            Trace.WriteLine(Utils.GetDebugMessage("Successfully created an instance for the view model", withTimeStamp: true));
        }

        /// <summary>
        /// Destructor for the class that will perform some cleanup tasks.
        /// This destructor will run only if the Dispose method does not get called.
        /// It gives the class the opportunity to finalize.
        /// </summary>
        ~ScreenshareServerViewModel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(disposing: false) is optimal in terms of
            // readability and maintainability.
            Dispose(disposing: false);
        }

        /// <summary>
        /// Property changed event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifies that subscribers list has been changed.
        /// This will happen when a client either starts or stops screen sharing.
        /// </summary>
        /// <param name="subscribers">
        /// Updated list of clients.
        /// </param>
        public void OnSubscribersChanged(List<SharedClientScreen> subscribers)
        {
            Debug.Assert(subscribers != null, Utils.GetDebugMessage("Received null subscribers list"));

            // Acquire lock because timer threads could also execute simultaneously.
            lock (_subscribers)
            {
                // Move the subscribers marked as pinned to the front of the list
                // keeping the lexicographical order of their name.
                _subscribers = RearrangeSubscribers(subscribers);
            }

            // Recompute the current window clients to notify the UX.
            RecomputeCurrentWindowClients(this.CurrentPage);

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully updated the subscribers list", withTimeStamp: true));
        }

        /// <summary>
        /// Notifies that a client has started screen sharing.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who started screen sharing.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who started screen sharing.
        /// </param>
        public void OnScreenshareStart(string clientId, string clientName)
        {
            if (clientName == "") return;

            Trace.WriteLine(Utils.GetDebugMessage($"{clientName} with Id {clientId} has started screen sharing", withTimeStamp: true));
            DisplayPopup($"{clientName} has started screen sharing");
        }

        /// <summary>
        /// Notifies that a client has stopped screen sharing.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who stopped screen sharing.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who stopped screen sharing.
        /// </param>
        public void OnScreenshareStop(string clientId, string clientName)
        {
            if (clientName == "") return;

            Trace.WriteLine(Utils.GetDebugMessage($"{clientName} with Id {clientId} has stopped screen sharing", withTimeStamp: true));
            DisplayPopup($"{clientName} has stopped screen sharing");
        }

        /// <summary>
        /// Implement "IDisposable". Disposes the managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, we should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Constants for initial page view.
        public static int InitialPageNumber { get; } = 1;
        public static int InitialTotalPages { get; } = 0;
        public static int InitialNumberOfRows { get; } = 1;
        public static int InitialNumberOfCols { get; } = 1;
        public static bool InitialIsLastPage { get; } = true;

        /// <summary>
        /// Gets the maximum number of tiles of the shared screens
        /// on a single page that will be shown to the server.
        /// </summary>
        public static int MaxTiles { get; } = 9;

        /// <summary>
        /// Acts as a map from the number of screens on the current window to
        /// the number of rows and columns of the grid displayed on the screen.
        /// </summary>
        public static List<(int Row, int Column)> NumRowsColumns { get; } = new()
        {
            (1, 1),  // 0 Total Screen.
            (1, 1),  // 1 Total Screen.
            (1, 2),  // 2 Total Screens.
            (2, 2),  // 3 Total Screens.
            (2, 2),  // 4 Total Screens.
            (2, 3),  // 5 Total Screens.
            (2, 3),  // 6 Total Screens.
            (3, 3),  // 7 Total Screens.
            (3, 3),  // 8 Total Screens.
            (3, 3)   // 9 Total Screens.
        };

        /// <summary>
        /// Gets the clients which are on the current page.
        /// </summary>
        public ObservableCollection<SharedClientScreen> CurrentWindowClients
        {
            get => _currentWindowClients;

            private set
            {
                // Note, to update the whole list, we can't simply assign it equal
                // to the new list. We need to clear the list first and add new elements
                // into the list to be able to see the changes on the UI.
                _currentWindowClients.Clear();
                foreach (SharedClientScreen screen in value)
                {
                    _currentWindowClients.Add(screen);
                }
                this.OnPropertyChanged(nameof(this.CurrentWindowClients));
            }
        }

        /// <summary>
        /// Gets the current page that the server is viewing.
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage;

            private set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    this.OnPropertyChanged(nameof(this.CurrentPage));
                }
            }
        }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages
        {
            get => _totalPages;

            private set
            {
                if (_totalPages != value)
                {
                    _totalPages = value;
                    this.OnPropertyChanged(nameof(this.TotalPages));
                }
            }
        }

        /// <summary>
        /// Gets whether the current page that the server is viewing is last page or not.
        /// </summary>
        public bool IsLastPage
        {
            get => _isLastPage;

            private set
            {
                if (_isLastPage != value)
                {
                    _isLastPage = value;
                    this.OnPropertyChanged(nameof(this.IsLastPage));
                }
            }
        }

        /// <summary>
        /// Gets the current number of rows of the grid displayed on the screen.
        /// </summary>
        public int CurrentPageRows
        {
            get => _currentPageRows;

            private set
            {
                if (_currentPageRows != value)
                {
                    _currentPageRows = value;
                    this.OnPropertyChanged(nameof(this.CurrentPageRows));
                }
            }
        }

        /// <summary>
        /// Gets the current number of columns of the grid displayed on the screen.
        /// </summary>
        public int CurrentPageColumns
        {
            get => _currentPageColumns;

            private set
            {
                if (_currentPageColumns != value)
                {
                    _currentPageColumns = value;
                    this.OnPropertyChanged(nameof(this.CurrentPageColumns));
                }
            }
        }

        /// <summary>
        /// Gets whether the popup is open or not.
        /// </summary>
        public bool IsPopupOpen
        {
            get => _isPopupOpen;

            // Don't keep the setter private, as it is bind using two-way binding.
            set
            {
                if (_isPopupOpen != value)
                {
                    _isPopupOpen = value;
                    this.OnPropertyChanged(nameof(this.IsPopupOpen));
                }
            }
        }

        /// <summary>
        /// Gets the text to be displayed on the popup.
        /// </summary>
        public string PopupText
        {
            get => _popupText;

            private set
            {
                if (_popupText != value)
                {
                    _popupText = value;
                    this.OnPropertyChanged(nameof(this.PopupText));
                }
            }
        }

        /// <summary>
        /// Gets the dispatcher to the main thread. In case it is not available
        /// (such as during unit testing) the dispatcher associated with the
        /// current thread is returned.
        /// </summary>
        public static Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Gets a singleton instance of "ScreenshareServerViewModel" class.
        /// </summary>
        /// <param name="isDebugging">
        /// If we are in debugging mode.
        /// </param>
        /// <returns>
        /// A singleton instance of "ScreenshareServerViewModel".
        /// </returns>
        public static ScreenshareServerViewModel GetInstance(bool isDebugging = false)
        {
            // Create a new instance if it was null before.
            _instance ??= new(isDebugging);
            return _instance;
        }

        /// <summary>
        /// Recomputes current window clients using the pagination logic
        /// and notifies the UX. It also notifies the old and new clients
        /// about the new status of sending image packets.
        /// </summary>
        /// <param name="newPageNum">
        /// The new page number for which the current window clients should be recomputed.
        /// </param>
        public void RecomputeCurrentWindowClients(int newPageNum)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));
            Debug.Assert(newPageNum > 0, Utils.GetDebugMessage("new page number should be positive"));

            // Reset the view if the subscribers count is zero.
            if (_subscribers.Count == 0)
            {
                // Update the view to its initial state.
                UpdateView(
                    new(),
                    InitialPageNumber,
                    InitialNumberOfRows,
                    InitialNumberOfCols,
                    InitialTotalPages,
                    InitialIsLastPage,
                    (InitialNumberOfRows, InitialNumberOfCols)
                );

                // Notifies the current clients to stop sending their image packets.
                NotifySubscribers(this.CurrentWindowClients.ToList(), new(), (InitialNumberOfRows, InitialNumberOfCols));
                return;
            }

            int newTotalPages = 0, newNumRows = 1, newNumCols = 1;
            List<SharedClientScreen> previousWindowClients, newWindowClients;
            (int Height, int Width) newTileDimensions = (0, 0);

            lock (_subscribers)
            {
                // Get the total number of pages.
                newTotalPages = GetTotalPages();

                Debug.Assert(newPageNum <= newTotalPages, Utils.GetDebugMessage("page number should be less than the total number of pages"));

                // Total count of all the subscribers sharing screen.
                int totalCount = _subscribers.Count;

                // Get the count of subscribers to skip for the current page.
                int countToSkip = GetCountToSkip(newPageNum);

                Debug.Assert(countToSkip >= 0, Utils.GetDebugMessage("count to skip should be non-negative"));
                Debug.Assert(countToSkip <= totalCount, Utils.GetDebugMessage("count to skip should be less than or equal to the total count"));

                int remainingCount = totalCount - countToSkip;

                // Number of subscribers that will show up on the current page.
                int limit = _subscribers[countToSkip].Pinned ? 1 : Math.Min(remainingCount, MaxTiles);

                Debug.Assert(limit <= MaxTiles, Utils.GetDebugMessage("Number of tiles on the current page should be less than or equal to the maximum number of tiles"));

                // Get the new window clients to be displayed on the current page.
                newWindowClients = _subscribers.GetRange(countToSkip, limit);
                int numNewWindowClients = newWindowClients.Count;

                // Save the previous window clients which are not there in the current window.
                previousWindowClients = this.CurrentWindowClients.ToList();
                previousWindowClients = previousWindowClients
                                        .Where(client => newWindowClients.FindIndex(n => n.Id == client.Id) == -1)
                                        .ToList();

                // The new number of rows and columns to be displayed based on new number of clients.
                (newNumRows, newNumCols) = NumRowsColumns[numNewWindowClients];

                // The new tile dimensions of screen image to be displayed based on new number of clients.
                newTileDimensions = GetTileDimensions(newNumRows, newNumCols);
            }

            // Update the view with the new fields.
            UpdateView(
                newWindowClients,
                newPageNum,
                newNumRows,
                newNumCols,
                newTotalPages,
                newPageNum == newTotalPages,
                newTileDimensions
            );

            // Notifies the old and new clients about the status of sending image packets.
            NotifySubscribers(previousWindowClients, newWindowClients, (newNumRows, newNumCols));

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully recomputed current window clients for the page {this.CurrentPage}", withTimeStamp: true));
        }

        /// <summary>
        /// Mark the client as pinned and switch to the page of that client.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client which is marked as pinned.
        /// </param>
        public void OnPin(string clientId)
        {
            Debug.Assert(clientId != null, Utils.GetDebugMessage("Received null client id"));
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));
            Debug.Assert(_subscribers.Count != 0, Utils.GetDebugMessage("_subscribers has count 0"));

            // The new page on which this pinned client will be displayed.
            int pinnedClientPage = 1;

            // Acquire lock because timer threads could also execute simultaneously.
            lock (_subscribers)
            {
                // Find the index of the client.
                int pinnedScreenIdx = _subscribers.FindIndex(subscriber => subscriber.Id == clientId);

                Debug.Assert(pinnedScreenIdx != -1, Utils.GetDebugMessage($"Client Id: {clientId} not found in the subscribers list"));

                // If client not found.
                if (pinnedScreenIdx == -1)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Client Id: {clientId} not found in the subscribers list", withTimeStamp: true));
                    return;
                }

                // Mark the client as pinned.
                SharedClientScreen pinnedScreen = _subscribers[pinnedScreenIdx];
                pinnedScreen.Pinned = true;

                // Move the subscribers marked as pinned to the front of the list
                // keeping the lexicographical order of their name.
                _subscribers = RearrangeSubscribers(_subscribers);

                pinnedClientPage = GetClientPage(pinnedScreen.Id);
            }

            // Switch to the page of the client.
            RecomputeCurrentWindowClients(pinnedClientPage);

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully pinned the client with id: {clientId}", withTimeStamp: true));
        }

        /// <summary>
        /// Mark the client as pinned and switch to the previous (or the first) page.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client which is marked as unpinned.
        /// </param>
        public void OnUnpin(string clientId)
        {
            Debug.Assert(clientId != null, Utils.GetDebugMessage("Received null client id"));
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));
            Debug.Assert(_subscribers.Count != 0, Utils.GetDebugMessage("_subscribers has count 0"));

            // Acquire lock because timer threads could also execute simultaneously.
            lock (_subscribers)
            {
                // Find the index of the client.
                int unpinnedScreenIdx = _subscribers.FindIndex(subscriber => subscriber.Id == clientId);

                Debug.Assert(unpinnedScreenIdx != -1, Utils.GetDebugMessage($"Client Id: {clientId} not found in the subscribers list"));

                // If client not found.
                if (unpinnedScreenIdx == -1)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Client Id: {clientId} not found in the subscribers list", withTimeStamp: true));
                    return;
                }

                // Mark the client as unpinned.
                SharedClientScreen unpinnedScreen = _subscribers[unpinnedScreenIdx];
                unpinnedScreen.Pinned = false;

                // Move the subscribers marked as pinned to the front of the list
                // keeping the lexicographical order of their name.
                _subscribers = RearrangeSubscribers(_subscribers);
            }

            //  Switch to the previous (or the first) page.
            RecomputeCurrentWindowClients(Math.Max(1, this.CurrentPage - 1));

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully unpinned the client with id: {clientId}", withTimeStamp: true));
        }

        /// <summary>
        /// It executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the destructor and we should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if we are disposing this object.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (_disposed) return;

            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                _model?.Dispose();
                _subscribers.Clear();
                _instance = null;
            }

            // Call the appropriate methods to clean up unmanaged resources here.

            // Now disposing has been done.
            _disposed = true;
        }

        /// <summary>
        /// Moves the subscribers marked as pinned to the front of the list
        /// keeping the lexicographical order of their name.
        /// </summary>
        /// <param name="subscribers">
        /// Input list of subscribers.
        /// </param>
        /// <returns>
        /// List of subscribers with pinned subscribers in front.
        /// </returns>
        private static List<SharedClientScreen> MovePinnedSubscribers(List<SharedClientScreen> subscribers)
        {
            Debug.Assert(subscribers != null, Utils.GetDebugMessage("Received null subscribers list"));

            // Separate pinned and unpinned subscribers.
            List<SharedClientScreen> pinnedSubscribers = new();
            List<SharedClientScreen> unpinnedSubscribers = new();

            foreach (SharedClientScreen subscriber in subscribers)
            {
                if (subscriber.Pinned)
                {
                    pinnedSubscribers.Add(subscriber);
                }
                else
                {
                    unpinnedSubscribers.Add(subscriber);
                }
            }

            // Join both the lists with pinned subscribers followed by the unpinned ones.
            return pinnedSubscribers.Concat(unpinnedSubscribers).ToList();
        }

        /// <summary>
        /// Rearranges the subscribers list by first having the Pinned subscribers followed by
        /// the unpinned subscribers. The pinned and unpinned subscribers are kept in the
        /// lexicographical order of their names.
        /// </summary>
        /// <param name="subscribers">
        /// The subscribers list to rearrange.
        /// </param>
        /// <returns>
        /// The rearranged subscribers list.
        /// </returns>
        private static List<SharedClientScreen> RearrangeSubscribers(List<SharedClientScreen> subscribers)
        {
            // Sort the subscribers in lexicographical order of their name.
            List<SharedClientScreen> sortedSubscribers = subscribers
                                                            .OrderBy(subscriber => subscriber.Name)
                                                            .ToList();

            // Move the subscribers marked as pinned to the front of the list
            // keeping the lexicographical order of their name.
            return MovePinnedSubscribers(sortedSubscribers);
        }

        /// <summary>
        /// Gets the tile dimensions in the grid displayed on the screen
        /// based on the number of rows and columns presented.
        /// </summary>
        /// <param name="rows">
        /// Number of rows of the grid on the screen.
        /// </param>
        /// <param name="columns">
        /// Number of columns of the grid on the screen.
        /// </param>
        /// <returns>
        /// A tuple having the height and width of the each grid tile.
        /// </returns>
        private static (int Height, int Width) GetTileDimensions(int rows, int columns)
        {
            // Get the total system height and width.
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double screenWidth = SystemParameters.PrimaryScreenWidth;

            // The margins which are kept on the UI.
            double marginBetweenImages = (rows + 1) * 6;
            double otherMargins = 100;

            // Compute the tile height and width.
            double remainingHeight = screenHeight - marginBetweenImages - otherMargins;
            int tileHeight = (int)Math.Floor(remainingHeight / rows);
            int tileWidth = (int)Math.Floor(screenWidth / columns);

            Debug.Assert(tileHeight >= 0, Utils.GetDebugMessage("Tile Height should be non-negative"));
            Debug.Assert(tileWidth >= 0, Utils.GetDebugMessage("Tile Width should be non-negative"));

            return (tileHeight, tileWidth);
        }

        /// <summary>
        /// Starts the processing task for the clients.
        /// </summary>
        /// <param name="clients">
        /// The clients for which the processing task needs to be started.
        /// </param>
        private static void StartProcessingForClients(List<SharedClientScreen> clients)
        {
            try
            {
                // Ask all the current window clients to start processing their images.
                foreach (SharedClientScreen client in clients)
                {
                    // The lambda function takes the final image from the final image queue
                    // of the client and set it as the "CurrentImage" variable for the client
                    // and notify the UX about the same.
                    client.StartProcessing(new Action<int>((taskId) =>
                    {
                        // Loop till the task is not canceled.
                        while (client.TaskId == taskId)
                        {
                            try
                            {
                                // Get the final image to be displayed on the UI.
                                Bitmap? finalImage = client.GetFinalImage(taskId);

                                if (finalImage == null) continue;

                                // Update the current image of the client on the screen
                                // by taking the processed images from its final image queue.
                                _ = ApplicationMainThreadDispatcher.BeginInvoke(
                                        DispatcherPriority.Normal,
                                        new Action<Bitmap>((image) =>
                                        {
                                            if (image != null)
                                            {
                                                BitmapImage img = Utils.BitmapToBitmapImage(image);
                                                img.Freeze();
                                                lock (client)
                                                {
                                                    client.CurrentImage = img;
                                                }
                                            }
                                        }),
                                        finalImage
                                    );
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(Utils.GetDebugMessage($"Failed to update the view: {e.Message}", withTimeStamp: true));
                            }

                        }
                    }));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to start the processing: {e.Message}", withTimeStamp: true));
            }
        }

        /// <summary>
        /// Stops the processing task for the clients.
        /// </summary>
        /// <param name="clients">
        /// The clients for which the processing task needs to be stopped.
        /// </param>
        private static void StopProcessingForClients(List<SharedClientScreen> clients)
        {
            foreach (SharedClientScreen client in clients)
            {
                try
                {
                    // Stop the processing for the client asynchronously
                    // so as not to block the main UI thread.
                    client.StopProcessing(stopAsync: true);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Failed to stop the processing: {e.Message}", withTimeStamp: true));
                }
            }
        }

        /// <summary>
        /// Notify the previous/new window clients to stop/send their image packets.
        /// It also asks the previous/new window clients to stop/start their image processing.
        /// </summary>
        /// <param name="prevWindowClients">
        /// List of clients which were there in the previous window.
        /// </param>
        /// <param name="currentWindowClients">
        /// List of clients which are there in the current window.
        /// </param>
        /// <param name="numRowsColumns">
        /// Number of rows and columns for the resolution of the image to be sent by the current window clients.
        /// </param>
        private void NotifySubscribers(List<SharedClientScreen> prevWindowClients, List<SharedClientScreen> currentWindowClients, (int, int) numRowsColumns)
        {
            Debug.Assert(_model != null, Utils.GetDebugMessage("_model is found null"));
            Debug.Assert(prevWindowClients != null, Utils.GetDebugMessage("list of previous window clients is null"));
            Debug.Assert(currentWindowClients != null, Utils.GetDebugMessage("list of current window clients is null"));

            // Ask all the current window clients to start sending image packets with the specified resolution.
            _model.BroadcastClients(currentWindowClients
                                    .Select(client => client.Id)
                                    .ToList(), nameof(ServerDataHeader.Send), numRowsColumns);

            // Start processing for the current window clients.
            StartProcessingForClients(currentWindowClients);

            Trace.WriteLine(Utils.GetDebugMessage("Successfully notified the new current window clients", withTimeStamp: true));

            // Ask all the previous window clients to stop sending image packets.
            _model.BroadcastClients(prevWindowClients
                                    .Select(client => client.Id)
                                    .ToList(), nameof(ServerDataHeader.Stop), (1, 1));

            // Ask all the previous window clients to stop processing their images.
            StopProcessingForClients(prevWindowClients);

            Trace.WriteLine(Utils.GetDebugMessage("Successfully notified the previous window clients", withTimeStamp: true));
        }

        /// <summary>
        /// Updates the view with the new values provided.
        /// </summary>
        /// <param name="newWindowClients">
        /// The new current window clients.
        /// </param>
        /// <param name="newPageNum">
        /// The new page number.
        /// </param>
        /// <param name="newNumRows">
        /// The new number of grid rows on the new page.
        /// </param>
        /// <param name="newNumCols">
        /// The new number of grid columns on the new page.
        /// </param>
        /// <param name="newTotalPages">
        /// The new total number of pages.
        /// </param>
        /// <param name="newIsLastPage">
        /// If the new page is last page or not.
        /// </param>
        /// <param name="newTileDimensions">
        /// The new tile dimension of each grid cell on the new page.
        /// </param>
        private void UpdateView(
            List<SharedClientScreen> newWindowClients,
            int newPageNum,
            int newNumRows,
            int newNumCols,
            int newTotalPages,
            bool newIsLastPage,
            (int Height, int Width) newTileDimensions
        )
        {
            // Update all the fields and notify the UX.
            _updateViewOperation = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<
                    ObservableCollection<SharedClientScreen>, int, int, int, int, bool, (int Height, int Width)
                >((clients, pageNum, numRows, numCols, totalPages, isLastPage, tileDimensions) =>
                {
                    lock (this)
                    {
                        foreach (SharedClientScreen screen in clients)
                        {
                            screen.TileHeight = tileDimensions.Height;
                            screen.TileWidth = tileDimensions.Width;
                        }

                        this.CurrentWindowClients = clients;
                        this.CurrentPage = pageNum;
                        this.CurrentPageRows = numRows;
                        this.CurrentPageColumns = numCols;
                        this.TotalPages = totalPages;
                        this.IsLastPage = isLastPage;
                    }
                }),
                new ObservableCollection<SharedClientScreen>(newWindowClients),
                newPageNum,
                newNumRows,
                newNumCols,
                newTotalPages,
                newIsLastPage,
                newTileDimensions
            );
        }

        /// <summary>
        /// Used to display the popup on the UI with the given message.
        /// </summary>
        /// <param name="message">
        /// Message to be displayed on the popup.
        /// </param>
        private void DisplayPopup(string message)
        {
            _displayPopupOperation = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<string>((text) =>
                {
                    lock (this)
                    {
                        // Close the popup if it was already opened before.
                        if (this.IsPopupOpen) this.IsPopupOpen = false;

                        this.PopupText = text;
                        this.IsPopupOpen = true;
                    }
                }),
                message
            );
        }

        /// <summary>
        /// Computes the number of subscribers to skip up to current page.
        /// </summary>
        /// <param name="currentPageNum">
        /// The current page number the server is viewing.
        /// </param>
        /// <returns>
        /// Returns the number of subscribers to skip up to the current page.
        /// </returns>
        private int GetCountToSkip(int currentPageNum)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));
            Debug.Assert(currentPageNum > 0, Utils.GetDebugMessage("current page number should be positive"));
            Debug.Assert(currentPageNum <= GetTotalPages(), Utils.GetDebugMessage("current page number should be less than or equal to the total number of pages"));

            int countToSkip = 0;
            for (int i = 1; i < currentPageNum; ++i)
            {
                // The first screen on the page "i".
                SharedClientScreen screen = _subscribers[countToSkip];
                if (screen.Pinned)
                {
                    // If the screen is pinned, skip by one.
                    ++countToSkip;
                }
                else
                {
                    // If screen is not pinned, then skip by max number of tiles that are displayed on one page.
                    countToSkip += MaxTiles;
                }
            }
            return countToSkip;
        }

        /// <summary>
        /// Compute the page of the client on which the client screen is displayed.
        /// </summary>
        /// <param name="clientId">
        /// The client Id whose page is to be found.
        /// </param>
        /// <returns>
        /// The page number of the client on which it's screen is displayed.
        /// </returns>
        private int GetClientPage(string clientId)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            // Total count of all the subscribers.
            int totalSubscribers = _subscribers.Count;

            // Index of the first subscriber on the page.
            int startSubscriberIdx = 0;

            // Current page number.
            int pageNum = 1;

            // Loop to the page of the client.
            while (startSubscriberIdx < totalSubscribers)
            {
                SharedClientScreen screen = _subscribers[startSubscriberIdx];
                if (screen.Pinned)
                {
                    if (screen.Id == clientId) return pageNum;

                    // If the screen is pinned, skip by one.
                    ++startSubscriberIdx;
                }
                else
                {
                    // Number of clients on the current page.
                    int limit = Math.Min(MaxTiles, totalSubscribers - startSubscriberIdx);

                    // Check if the client is on the current page.
                    int clientIdx = _subscribers
                                .GetRange(startSubscriberIdx, limit)
                                .FindIndex(sub => sub.Id == clientId);
                    if (clientIdx != -1) return pageNum;

                    // If screen is not pinned, then skip by max number of tiles that are displayed on one page.
                    startSubscriberIdx += limit;
                }

                // Go to next page.
                ++pageNum;
            }

            Debug.Assert(false, Utils.GetDebugMessage($"Page of the client with id: {clientId} not found"));

            // Switch to the first page in case client page can not be found.
            return 1;
        }

        /// <summary>
        /// Gets the total number of pages formed in screen share view.
        /// </summary>
        /// <returns>
        /// Returns the total number of pages formed.
        /// </returns>
        private int GetTotalPages()
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            // Total count of all the subscribers.
            int totalSubscribers = _subscribers.Count;

            // Index of the first subscriber on the page.
            int startSubscriberIdx = 0;

            // Current page number.
            int pageNum = 0;

            // Loop to the page of the client.
            while (startSubscriberIdx < totalSubscribers)
            {
                SharedClientScreen screen = _subscribers[startSubscriberIdx];
                if (screen.Pinned)
                {
                    // If the screen is pinned, skip by one.
                    ++startSubscriberIdx;
                }
                else
                {
                    // Number of clients on the current page.
                    int limit = Math.Min(MaxTiles, totalSubscribers - startSubscriberIdx);

                    // If screen is not pinned, then skip by max number of tiles that are displayed on one page.
                    startSubscriberIdx += limit;
                }

                // Go to next page.
                ++pageNum;
            }

            return pageNum;
        }

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">
        /// The name of the property that is changed.
        /// </param>
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new(property));
        }
    }
}
