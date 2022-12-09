///<author>Satyam Mishra</author>
///<summary>
/// This file has ScreenshareClient class's partial implementation
/// In this file functions realted to starting of ScreenCapturing 
/// are implemented
///</summary>

using PlexShareNetwork;
using PlexShareNetwork.Communication;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace PlexShareScreenshare.Client
{
    /// <summary>
    /// Class contains implementation of the ScreenshareClient which will
    /// take the processed images and send it to the server via networking module
    /// </summary>
    public partial class ScreenshareClient : INotificationHandler
    {

        // ScreenshareClient Object
        private static ScreenshareClient? _screenShareClient;

        // Object of networking module through which
        // we will be communicating to the server
        private readonly ICommunicator _communicator;

        // Threads for sending images and the confirmation packet
        private Task? _sendImageTask, _sendConfirmationTask;

        // Capturer and Processor Class Objects
        private readonly ScreenCapturer _capturer;
        private readonly ScreenProcessor _processor;

        // Name and Id of the current client user
        private string? _name;
        private string? _id;

        // Tokens added to be able to stop the thread execution
        private bool _confirmationCancellationToken;
        private bool _imageCancellationToken;

        // View model for screenshare client
        private ScreenshareClientViewModel _viewModel;

        /// <summary>
        /// Timer object which keeps track of the time the CONFIRMATION packet
        /// was received last from the client to tell that the client is still
        /// presenting the screen.
        /// </summary>
        private readonly Timer? _timer;

        /// <summary>
        /// The timeout value in "milliseconds" defining the timeout for the timer in
        /// SharedClientScreen which represents the maximum time to wait for the arrival
        /// of the packet from the client with the CONFIRMATION header.
        /// </summary>
        public static double Timeout { get; } = 20 * 1000;

        /// <summary>
        /// Setting up the ScreenCapturer and ScreenProcessor Class
        /// Taking instance of communicator from communicator factory
        /// and subscribing to it.
        /// </summary>
        private ScreenshareClient(bool isDebugging)
        {
            _capturer = new ScreenCapturer();
            _processor = new ScreenProcessor(_capturer);
            if (!isDebugging)
            {
                _communicator = CommunicationFactory.GetCommunicator();
                _communicator.Subscribe(Utils.ModuleIdentifier, this, true);
            }

            try
            {
                if (!isDebugging)
                {
                    // Create the timer for this client.
                    _timer = new Timer();
                    _timer.Elapsed += new((sender, e) => OnTimeOut());

                    // The timer should be invoked only once.
                    _timer.AutoReset = false;

                    // Set the time interval for the timer.
                    this.UpdateTimer();

                    // Start the timer.
                    //_timer.Enabled = true;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to create the timer: {e.Message}", withTimeStamp: true));
            }

            Trace.WriteLine(Utils.GetDebugMessage("Successfully stopped image processing", withTimeStamp: true));
        }

        /// <summary>
        /// On timeout stop screensharing and make the viewmodel's sharingscreen boolean
        /// value as false for letting viewmodel know that screenshare stopped
        /// </summary>
        public void OnTimeOut()
        {
            StopScreensharing();
            _viewModel.SharingScreen = false;
            Trace.WriteLine(Utils.GetDebugMessage($"Timeout occurred", withTimeStamp: true));
        }

        /// <summary>
        /// Gives an instance of ScreenshareClient class and that instance is always 
        /// the same i.e. singleton pattern.
        /// </summary>
        public static ScreenshareClient GetInstance(ScreenshareClientViewModel viewModel = null, bool isDebugging = false)
        {
            if (_screenShareClient == null)
            {
                _screenShareClient = new ScreenshareClient(isDebugging);
            }
            _screenShareClient._viewModel = viewModel;
            Trace.WriteLine(Utils.GetDebugMessage("Successfully created an instance of ScreenshareClient", withTimeStamp: true));
            return _screenShareClient;
        }

        /// <summary>
        /// When client clicks the screensharing button, this function gets executed
        /// It will send a register packet to the server and it will even start sending the
        /// confirmation packets to the sever
        /// </summary>
        public void StartScreensharing()
        {
            // Start the timer.
            _timer.Enabled = true;

            Debug.Assert(_id != null, Utils.GetDebugMessage("_id property found null"));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null"));

            // sending register packet
            DataPacket dataPacket = new(_id, _name, ClientDataHeader.Register.ToString(), "");
            string serializedData = JsonSerializer.Serialize(dataPacket);
            _communicator.Send(serializedData, Utils.ModuleIdentifier, null);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully sent REGISTER packet to server"));

            SendConfirmationPacket();
            Trace.WriteLine(Utils.GetDebugMessage("Started sending confirmation packet"));

        }

        /// <summary>
        /// This function will be invoked on message from server
        /// If the message is SEND then start capturing, processing and sending functions
        /// Otherwise, if the message was STOP then just stop the image sending part
        /// </summary>
        /// <param name="serializedData"> Serialized data from the network module </param>
        public void OnDataReceived(string serializedData)
        {
            // Deserializing data packet received from server
            Debug.Assert(serializedData != "", Utils.GetDebugMessage("Message from serve found null", withTimeStamp: true));
            DataPacket? dataPacket = JsonSerializer.Deserialize<DataPacket>(serializedData);
            Debug.Assert(dataPacket != null, Utils.GetDebugMessage("Unable to deserialize datapacket from server", withTimeStamp: true));
            Trace.WriteLine(Utils.GetDebugMessage("Successfully received packet from server", withTimeStamp: true));

            if (dataPacket?.Header == ServerDataHeader.Send.ToString())
            {
                // If it is SEND packet then start image sending (if not already started) and 
                // Set the resolution as in the packet
                Trace.WriteLine(Utils.GetDebugMessage("Got SEND packet from server", withTimeStamp: true));

                // Starting capturer, processor and Image Sending
                StartImageSending();

                int windowCount = int.Parse(dataPacket.Data);
                _processor.SetNewResolution(windowCount);
                Trace.WriteLine(Utils.GetDebugMessage("Successfully set the new resolution", withTimeStamp: true));
            }
            else if (dataPacket?.Header == ServerDataHeader.Stop.ToString())
            {
                // Else if it was a STOP packet then stop image sending
                Trace.WriteLine(Utils.GetDebugMessage("Got STOP packet from server", withTimeStamp: true));
                StopImageSending();
            }
            else if (dataPacket?.Header == ServerDataHeader.Confirmation.ToString())
            {
                // Else if it was a CONFIRMATION packet then update the timer to the max value
                Trace.WriteLine(Utils.GetDebugMessage("Got CONFIRMATION packet from server", withTimeStamp: true));
                UpdateTimer();
            }
            else
            {
                // Else it was some invalid packet so add a debug message
                Debug.Assert(false,
                    Utils.GetDebugMessage("Header from server is neither SEND, STOP nor CONFIRMATION"));
            }
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
                _timer.Interval = Timeout;
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to reset the timer: {e.Message}", withTimeStamp: true));
                throw new Exception("Failed to reset the timer", e);
            }
        }

        /// <summary>
        /// Image sending function which will take image pixel diffs from processor and 
        /// send it to the server via the networking module. Images are sent only if there
        /// are any changes in pixels as compared to previous image.
        /// </summary>
        private void ImageSending()
        {

            int cnt = 0;
            while (!_imageCancellationToken)
            {
                string serializedImg = _processor.GetFrame(ref _imageCancellationToken);
                if (_imageCancellationToken) break;

                DataPacket dataPacket = new(_id, _name, ClientDataHeader.Image.ToString(), serializedImg);
                string serializedData = JsonSerializer.Serialize(dataPacket);

                Trace.WriteLine(Utils.GetDebugMessage($"Sent frame {cnt} of size {serializedData.Length}", withTimeStamp: true));
                _communicator.Send(serializedData, Utils.ModuleIdentifier, null);
                cnt++;
            }
        }

        /// <summary>
        /// Starting the image sending function on a thread.
        /// </summary>
        private void StartImageSending()
        {
            _capturer.StartCapture();
            _processor.StartProcessing();
            Trace.WriteLine(Utils.GetDebugMessage("Successfully started capturer and processor"));

            _imageCancellationToken = false;
            _sendImageTask = new Task(ImageSending);
            _sendImageTask.Start();
            Trace.WriteLine(Utils.GetDebugMessage("Successfully started image sending"));
        }

    }
}
