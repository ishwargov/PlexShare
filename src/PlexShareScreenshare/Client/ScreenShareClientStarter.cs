///<author>Satyam Mishra</author>
///<summary>
/// This file has ScreenshareClient class's partial implementation
/// In this file functions realted to starting of ScreenCapturing 
/// are implemented
///</summary>

using PlexShareNetwork;
using PlexShareNetwork.Communication;
using PlexShareNetwork.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;
// Each frame consists of the resolution of the image and the ImageDiffList
using Frame = System.Tuple<System.Tuple<int, int>,
                        System.Collections.Generic.List<System.Tuple<System.Tuple<int, int>,
                        System.Tuple<int, int, int>>>>;


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

        // Serializer object from networking module
        private readonly Serializer _serializer;

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
            _communicator.Subscribe("ScreenShare", this, true);
            _serializer = new();
        }

        /// <summary>
        /// Gives an instance of ScreenshareClient class and that instance is always 
        /// the same i.e. singleton pattern.
        /// </summary>
        public ScreenshareClient GetInstance()
        {
            if (_screenShareClient == null)
            {
                _screenShareClient = new ScreenshareClient();
            }
            return _screenShareClient;
        }

        /// <summary>
        /// Firstly it will send a REGISTER packet to the server then 
        /// start capturer, processor, image sending function and 
        /// the confirmation sending function.
        /// </summary>
        public void StartScreenSharing()
        {
            _isScreenSharing = true;
            // sending register packet
            DataPacket dataPacket = new(_id, _name, ClientDataHeader.Register.ToString(), "");
            string serializedData = _serializer.Serialize(dataPacket);
            _communicator.Send(serializedData, "ScreenShare", null);

            StartImageSending();
            SendConfirmationPacket();

            _capturer.StartCapture();
            _processor.StartProcessing();
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
            DataPacket dataPacket = _serializer.Deserialize<DataPacket>(serializedData);
            if (dataPacket.Header == ServerDataHeader.Send.ToString())
            {
                if (!_isScreenSharing)
                {
                    StartScreenSharing();
                }
                Tuple<int, int> res = _serializer.Deserialize<Tuple<int, int>>(dataPacket.Data);
                _processor.SetNewResolution(res);
            }
            else
            {
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
            while (!_imageCancellationTokenSource.IsCancellationRequested)
            {
                Frame img = _processor.GetImage();
                if (img.Item2.Count == 0) continue;
                string serializedImg = _serializer.Serialize(img);
                DataPacket dataPacket = new(_id, _name, ClientDataHeader.Image.ToString(), serializedImg);
                string serializedData = _serializer.Serialize(dataPacket);
                _communicator.Send(serializedData, "ScreenShare", null);
            }
        }

        /// <summary>
        /// Starting the image sending function on a thread.
        /// </summary>
        private void StartImageSending()
        {
            _imageCancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _imageCancellationTokenSource.Token;
            _sendImageTask = new Task(ImageSending, token);
            _sendImageTask.Start();
        }

    }
}
