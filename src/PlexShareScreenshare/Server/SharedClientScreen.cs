/// <author>Mayank Singla</author>
/// <summary>
/// Defines the "SharedClientScreen" class which represents the screen
/// shared by a client along with some other client information.
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

// The Timer class object.
using Timer = System.Timers.Timer;

namespace PlexShareScreenshare.Server
{
    /// <summary>
    /// Represents the screen shared by a client along with some other client information.
    /// </summary>
    public class SharedClientScreen :
        INotifyPropertyChanged, // Notifies the UX that a property value has changed.
        IDisposable             // Handle cleanup work for the allocated resources.
    {
        /// <summary>
        /// Timer object which keeps track of the time the CONFIRMATION packet
        /// was received last from the client to tell that the client is still
        /// presenting the screen.
        /// </summary>
        private readonly Timer? _timer;

        /// <summary>
        /// The data model defining the callback for the timeout.
        /// </summary>
        private readonly ITimerManager _server;

        /// <summary>
        /// The screen stitcher associated with this client.
        /// </summary>
        private readonly ScreenStitcher _stitcher;

        /// <summary>
        /// Stores the image received from the clients.
        /// </summary>
        private readonly Queue<string> _imageQueue;

        /// <summary>
        /// The final stitched images received after stitching the previous
        /// screen image of the client with the new image. It contains the images
        /// which are ready to be displayed.
        /// </summary>
        private readonly Queue<Bitmap> _finalImageQueue;

        /// <summary>
        /// Task which will continuously pick the image from the "_finalImageQueue"
        /// and update the "CurrentImage" variable to continuously update the screen
        /// image of the client. The function for this task will be provided by the
        /// view model which will also invoke "OnPropertyChanged" to notify the UX.
        /// </summary>
        private Task? _imageSendTask;

        /// <summary>
        /// Lock acquired while modifying "_taskId"
        /// </summary>
        private readonly Object _taskIdLock = new();

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The current screen image of the client being displayed.
        /// </summary>
        private BitmapImage? _currentImage;

        /// <summary>
        /// Whether the client is marked as pinned or not.
        /// </summary>
        private bool _pinned;

        /// <summary>
        /// The height of the tile of the client screen.
        /// </summary>
        private int _tileHeight;

        /// <summary>
        /// The width of the tile of the client screen.
        /// </summary>
        private int _tileWidth;

        /// <summary>
        /// Creates an instance of SharedClientScreen which represents the screen
        /// shared by a client and also stores some other information of the client.
        /// </summary>
        /// <param name="clientId">
        /// The ID of the client.
        /// </param>
        /// <param name="clientName">
        /// The name of the client.
        /// </param>
        /// <param name="server">
        /// The timer manager implementing the callback for the timer object.
        /// </param>
        /// <param name="isDebugging">
        /// If we are in debugging/testing mode.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public SharedClientScreen(string clientId, string clientName, ITimerManager server, bool isDebugging = false)
        {
            this.Id = clientId ?? throw new ArgumentNullException(nameof(clientId));
            this.Name = clientName ?? throw new ArgumentNullException(nameof(clientName));
            _server = server ?? throw new ArgumentNullException(nameof(server));

            // Create a new stitcher object associated to this client.
            _stitcher = new(this);

            // Initialize the queues to be empty.
            _imageQueue = new();
            _finalImageQueue = new();

            // Mark the client as not pinned initially.
            _pinned = false;

            // Initialize these variables as null.
            _imageSendTask = null;
            _currentImage = null;

            // Initialize rest of the properties.
            _disposed = false;
            this.TaskId = 0;
            _tileHeight = 0;
            _tileWidth = 0;

            try
            {
                if (!isDebugging)
                {
                    // Create the timer for this client.
                    _timer = new Timer();
                    _timer.Elapsed += new((sender, e) => _server.OnTimeOut(sender, e, Id));

                    // The timer should be invoked only once.
                    _timer.AutoReset = false;

                    // Set the time interval for the timer.
                    this.UpdateTimer();

                    // Start the timer.
                    _timer.Enabled = true;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to create the timer: {e.Message}", withTimeStamp: true));
                throw new Exception("Failed to create the timer", e);
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully created client with id: {this.Id} and name: {this.Name}", withTimeStamp: true));
        }

        /// <summary>
        /// Destructor for the class that will perform some cleanup tasks.
        /// This destructor will run only if the Dispose method does not get called.
        /// It gives the class the opportunity to finalize.
        /// </summary>
        ~SharedClientScreen()
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
        /// Implement IDisposable. Disposes the managed and unmanaged resources.
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

        /// <summary>
        /// The timeout value in "milliseconds" defining the timeout for the timer in
        /// SharedClientScreen which represents the maximum time to wait for the arrival
        /// of the packet from the client with the CONFIRMATION header.
        /// </summary>
        public static double Timeout { get; } = 20 * 1000;

        /// <summary>
        /// Gets the id of the current image sending task.
        /// </summary>
        public int TaskId { get; private set; }

        /// <summary>
        /// Gets the ID of the client sharing this screen.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name of the client sharing this screen.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the current screen image of the client being displayed.
        /// </summary>
        public BitmapImage? CurrentImage
        {
            get => _currentImage;

            set
            {
                if (_currentImage != value)
                {
                    _currentImage = value;
                    this.OnPropertyChanged(nameof(CurrentImage));
                }
            }
        }

        /// <summary>
        /// Gets whether the client is marked as pinned or not.
        /// </summary>
        public bool Pinned
        {
            get => _pinned;

            set
            {
                if (_pinned != value)
                {
                    _pinned = value;
                    this.OnPropertyChanged(nameof(Pinned));
                }
            }
        }

        /// <summary>
        /// Gets the height of the tile of the client screen.
        /// </summary>
        public int TileHeight
        {
            get => _tileHeight;

            set
            {
                if (_tileHeight != value)
                {
                    _tileHeight = value;
                    this.OnPropertyChanged(nameof(TileHeight));
                }
            }
        }

        /// <summary>
        /// Gets the width of the tile of the client screen.
        /// </summary>
        public int TileWidth
        {
            get => _tileWidth;

            set
            {
                if (_tileWidth != value)
                {
                    _tileWidth = value;
                    this.OnPropertyChanged(nameof(TileWidth));
                }
            }
        }

        /// <summary>
        /// Pops and returns the received image at the beginning of the received image queue.
        /// </summary>
        /// <param name="taskId">
        /// Id of the task in which this function is called.
        /// </param>
        /// <returns>
        /// The received image that is removed from the beginning.
        /// </returns>
        public string? GetImage(int taskId)
        {
            Debug.Assert(_imageQueue != null, Utils.GetDebugMessage("_imageQueue is found null"));

            // Wait until the queue is empty.
            while (_imageQueue != null && _imageQueue.Count == 0)
            {
                // Return if the task is stopped or a new task is started.
                if (taskId != this.TaskId) return "";

                Thread.Sleep(100);
            }

            // Return if the task is stopped or a new task is started.
            if (_imageQueue == null || taskId != this.TaskId) return "";

            lock (_imageQueue)
            {
                try
                {
                    // Return if the task is stopped or a new task is started.
                    if (taskId != this.TaskId) return "";

                    return _imageQueue.Dequeue();
                }
                catch (InvalidOperationException e)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Dequeue failed: {e.Message}", withTimeStamp: true));
                    return null;
                }
            }
        }

        /// <summary>
        /// Insert the received image into the received image queue.
        /// </summary>
        /// <param name="image">
        /// Image to be inserted.
        /// </param>
        /// <param name="taskId">
        /// Id of the task in which this function is called.
        /// </param>
        public void PutImage(string image, int taskId)
        {
            Debug.Assert(_imageQueue != null, Utils.GetDebugMessage("_imageQueue is found null"));

            lock (_imageQueue)
            {
                // Return if the task is stopped or a new task is started.
                if (taskId != this.TaskId) return;

                _imageQueue.Enqueue(image);
            }
        }

        /// <summary>
        /// Pops and returns the final Image at the beginning of the final image queue.
        /// </summary>
        /// <param name="taskId">
        /// Id of the task in which this function is called.
        /// </param>
        /// <returns>
        /// The final image to be displayed that is removed from the beginning.
        /// </returns>
        public Bitmap? GetFinalImage(int taskId)
        {
            Debug.Assert(_finalImageQueue != null, Utils.GetDebugMessage("_finalImageQueue is found null"));

            // Wait until the queue is not empty.
            while (_finalImageQueue != null && _finalImageQueue.Count == 0)
            {
                // Return if the task is stopped or a new task is started.
                if (taskId != this.TaskId) return null;
                Thread.Sleep(100);
            }

            // Return if the task is stopped or a new task is started.
            if (_finalImageQueue == null || taskId != this.TaskId) return null;

            lock (_finalImageQueue)
            {
                try
                {
                    // Return if the task is stopped or a new task is started.
                    if (taskId != this.TaskId) return null;

                    return _finalImageQueue.Dequeue();
                }
                catch (InvalidOperationException e)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Dequeue failed: {e.Message}", withTimeStamp: true));
                    return null;
                }
            }
        }

        /// <summary>
        /// Insert the final image into the final image queue.
        /// </summary>
        /// <param name="image">
        /// Image to be inserted.
        /// </param>
        /// /// <param name="taskId">
        /// Id of the task in which this function is called.
        /// </param>
        public void PutFinalImage(Bitmap image, int taskId)
        {
            Debug.Assert(_finalImageQueue != null, Utils.GetDebugMessage("_finalImageQueue is found null"));

            lock (_finalImageQueue)
            {
                // Return if the task is stopped or a new task is started.
                if (taskId != this.TaskId) return;

                _finalImageQueue.Enqueue(image);
            }
        }

        /// <summary>
        /// Starts image processing by calling the underlying stitcher to process images.
        /// It will also create (if not exist) and start the task for updating the displayed
        /// images and notify the UX.
        /// </summary>
        /// <param name="task">
        /// Task to be executed for updating current image of the client and notifying the view.
        /// </param>
        /// <exception cref="Exception"></exception>
        public void StartProcessing(Action<int> task)
        {
            Debug.Assert(_stitcher != null, Utils.GetDebugMessage("_stitcher is found null"));

            if (_imageSendTask != null)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Trying to start an already started task for the client with Id {this.Id}", withTimeStamp: true));
                return;
            }

            try
            {
                lock (_taskIdLock)
                {
                    // Create a new task Id
                    ++this.TaskId;

                    // Start the stitcher.
                    _stitcher?.StartStitching(this.TaskId);

                    // Create and start a new task.
                    _imageSendTask = new(() => task(this.TaskId));
                    _imageSendTask?.Start();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to start the task: {e.Message}", withTimeStamp: true));
                throw new Exception("Failed to start the task", e);
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully created the processing task with id {this.TaskId} for the client with id {this.Id}", withTimeStamp: true));
        }

        /// <summary>
        /// Stops the processing of the images by stopping the underlying stitcher.
        /// Cancels the task for updating the displayed images and clear the queues
        /// containing images.
        /// </summary>
        /// <param name="stopAsync">
        /// Whether to stop the process asynchronously or not.
        /// </param>
        /// <exception cref="Exception"></exception>
        public void StopProcessing(bool stopAsync = false)
        {
            Debug.Assert(_stitcher != null, Utils.GetDebugMessage("_stitcher is found null"));

            // Check if the task was started before.
            if (_imageSendTask == null)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Trying to stop a task which was never started for the client with Id {this.Id}", withTimeStamp: true));
                return;
            }

            // Store the previous image sending task.
            Task previousImageSendTask;

            try
            {
                lock (_taskIdLock)
                {
                    // Change the task ID to denote task cancellation.
                    ++this.TaskId;

                    // Immediately make the task variable null.
                    previousImageSendTask = _imageSendTask;
                    _imageSendTask = null;

                    // Clear both the queues.
                    lock (_imageQueue)
                    {
                        _imageQueue.Clear();
                    }

                    lock (_finalImageQueue)
                    {
                        _finalImageQueue.Clear();
                    }
                }

                if (!stopAsync)
                {
                    // Stop the stitcher and image sending task.
                    _stitcher?.StopStitching();
                    previousImageSendTask?.Wait();
                }
                else
                {
                    // Stop the stitcher and image sending task asynchronously.
                    Task.Run(() => _stitcher?.StopStitching());
                    Task.Run(() => previousImageSendTask?.Wait());
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to cancel the task: {e.Message}", withTimeStamp: true));
                throw new Exception("Failed to start the task", e);
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully stopped the processing task with id {this.TaskId} for the client with id {this.Id}", withTimeStamp: true));
        }

        /// <summary>
        /// Resets the time of the timer object.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void UpdateTimer()
        {
            Debug.Assert(_timer != null, Utils.GetDebugMessage("_timer is found null"));

            try
            {
                // It will reset the timer to start again.
                _timer.Interval = SharedClientScreen.Timeout;
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to reset the timer: {e.Message}", withTimeStamp: true));
                throw new Exception("Failed to reset the timer", e);
            }
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
                StopProcessing();

                if (_timer != null)
                {
                    // Stop and dispose the timer object.
                    _timer.Enabled = false;
                    _timer?.Dispose();
                }
            }

            // Call the appropriate methods to clean up unmanaged resources here.

            // Now disposing has been done.
            _disposed = true;
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
