///<author>Satyam Mishra</author>
///<summary>
/// This file has ScreenshareClient class's partial implementation
/// In this file functions realted to starting of ScreenCapturing 
/// are implemented
///</summary>


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
        private CancellationTokenSource? _confirmationCancellationTokenSource;
        private CancellationTokenSource? _imageCancellationTokenSource;

        // Varible to store if screen share is active
        private bool _isScreenSharing = false;

        /// <summary>
        /// Setting up the ScreenCapturer and ScreenProcessor Class
        /// Taking instance of communicator from communicator factory
        /// and subscribing to it.
        /// </summary>
        private ScreenshareClient()
        {
            _capturer = new ScreenCapturer();
            _processor = new ScreenProcessor(_capturer);
            _communicator = CommunicationFactory.GetCommunicator();
            _communicator.Subscribe(Utils.ModuleIdentifier, this, true);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully stopped image processing", withTimeStamp: true));
        }

        /// <summary>
        /// Gives an instance of ScreenshareClient class and that instance is always 
        /// the same i.e. singleton pattern.
        /// </summary>
        public static ScreenshareClient GetInstance()
        {
            if (_screenShareClient == null)
            {
                _screenShareClient = new ScreenshareClient();
            }
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
            _isScreenSharing = true;
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
        void INotificationHandler.OnDataReceived(string serializedData)
        {
            // Deserializing data packet received from server
            Debug.Assert(serializedData != "", Utils.GetDebugMessage("Message from serve found null", withTimeStamp: true));
            DataPacket? dataPacket = JsonSerializer.Deserialize<DataPacket>(serializedData);
            Debug.Assert(dataPacket != null, Utils.GetDebugMessage("Unable to deserialize datapacket from server", withTimeStamp: true));
            Trace.WriteLine(Utils.GetDebugMessage("Successfully received packet from server", withTimeStamp: true));

            if (dataPacket?.Header == ServerDataHeader.Send.ToString())
            {
                // if it is SEND packet then start image sending (if not already started) and 
                // set the resolution as in the packet
                Trace.WriteLine(Utils.GetDebugMessage("Got SEND packet from server", withTimeStamp: true));
                if (!_isScreenSharing)
                {
                    // starting capturer, processor and Image Sending
                    StartImageSending();
                }
                int windowCount = int.Parse(dataPacket.Data);
                _processor.SetNewResolution(windowCount);
                Trace.WriteLine(Utils.GetDebugMessage("Successfully set the new resolution", withTimeStamp: true));
            }
            else
            {
                Debug.Assert(dataPacket?.Header == ServerDataHeader.Stop.ToString(),
                    Utils.GetDebugMessage("Header from server is neither SEND nor STOP"));
                // else if it was a STOP packet then stop image sending
                Trace.WriteLine(Utils.GetDebugMessage("Got STOP packet from server", withTimeStamp: true));
                StopImageSending();
            }
        }

        /// <summary>
        /// Image sending function which will take image pixel diffs from processor and 
        /// send it to the server via the networking module. Images are sent only if there
        /// are any changes in pixels as compared to previous image.
        /// </summary>
        private void ImageSending()
        {
            Debug.Assert(_imageCancellationTokenSource != null,
                Utils.GetDebugMessage("_imageCancellationTokenSource is not null, cannot start ImageSending"));
            _imageCancellationTokenSource.Token.ThrowIfCancellationRequested();

            while (!_imageCancellationTokenSource.Token.IsCancellationRequested)
            {
                _imageCancellationTokenSource.Token.ThrowIfCancellationRequested();

                Frame img = _processor.GetFrame(_imageCancellationTokenSource.Token);
                // if the difference between the current and the new image is nothing then
                // dont send a packet to the server
                if (img.Pixels.Count == 0) continue;

                // else serialize the processed frame and send it to the server
                string serializedImg = JsonSerializer.Serialize(img);
                Debug.Assert(_id != null, Utils.GetDebugMessage("_id property found null", withTimeStamp: true));
                Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null", withTimeStamp: true));

                DataPacket dataPacket = new(_id, _name, ClientDataHeader.Image.ToString(), serializedImg);
                string serializedData = JsonSerializer.Serialize(dataPacket);

                _communicator.Send(serializedData, Utils.ModuleIdentifier, null);
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

            _imageCancellationTokenSource = new();
            _sendImageTask = new Task(ImageSending, _imageCancellationTokenSource.Token);
            _sendImageTask.Start();
            Trace.WriteLine(Utils.GetDebugMessage("Successfully started image sending"));
        }

    }
}
