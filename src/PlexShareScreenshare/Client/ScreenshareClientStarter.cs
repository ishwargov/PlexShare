///<author>Satyam Mishra</author>
///<summary>
/// This file has ScreenshareClient class's partial implementation
/// In this file functions realted to starting of ScreenCapturing 
/// are implemented
///</summary>

using PlexShareNetwork;
using PlexShareNetwork.Communication;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
        /// Firstly it will send a REGISTER packet to the server then 
        /// start capturer, processor, image sending function and 
        /// the confirmation sending function.
        /// </summary>
        public void StartScreensharing()
        {
            _isScreenSharing = true;
            Debug.Assert(_id != null, Utils.GetDebugMessage("_id property found null", withTimeStamp: true));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null", withTimeStamp: true));

            // sending register packet
            DataPacket dataPacket = new(_id, _name, ClientDataHeader.Register.ToString(), "");
            string serializedData = JsonSerializer.Serialize(dataPacket);
            _communicator.Send(serializedData, Utils.ModuleIdentifier, null);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully sent REGISTER packet to server", withTimeStamp: true));

            // starting capturer, processor, Image Sending and Confirmation Packet Sending
            Trace.WriteLine(Utils.GetDebugMessage("Started capturer and processor", withTimeStamp: true));
            _capturer.StartCapture();
            _processor.StartProcessing();


            Trace.WriteLine(Utils.GetDebugMessage("Started image sending and confirmation packet sending", withTimeStamp: true));
            StartImageSending();
            SendConfirmationPacket();

        }

        /// <summary>
        /// This function will be invoked on message from server
        /// If the message is SEND then start screen sharing and set the 
        /// appropriate resolution.
        /// Otherwise the message was STOP then stop the screen sharing.
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
                    StartScreensharing();
                }
                int windowCount = int.Parse(dataPacket.Data);
                _processor.SetNewResolution(windowCount);
                Trace.WriteLine(Utils.GetDebugMessage("Successfully set the new resolution", withTimeStamp: true));
            }
            else
            {
                // else if it was a STOP packet then stop image sending
                Trace.WriteLine(Utils.GetDebugMessage("Got STOP packet from server", withTimeStamp: true));
                StopScreensharing();
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
                Trace.WriteLine(Utils.GetDebugMessage("Sent Image packet to server", withTimeStamp: true));
            }
        }

        /// <summary>
        /// Starting the image sending function on a thread.
        /// </summary>
        private void StartImageSending()
        {
            _imageCancellationTokenSource = new();
            _sendImageTask = new Task(ImageSending, _imageCancellationTokenSource.Token);
            _sendImageTask.Start();
            Trace.WriteLine(Utils.GetDebugMessage("Successfully started image sending", withTimeStamp: true));
        }

    }
}
