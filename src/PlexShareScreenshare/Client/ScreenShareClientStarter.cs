using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using PlexShareNetwork;
using PlexShareNetwork.Communication;
using PlexShareNetwork.Serialization;
// Each frame consists of the resolution of the image and the ImageDiffList
using Frame = System.Tuple<System.Tuple<int, int>,
                        System.Collections.Generic.List<System.Tuple<System.Tuple<int, int>,
                        System.Tuple<int, int, int>>>>;


namespace PlexShareScreenshare.Client
{
    /// <summary>
    /// Class contains implementation of the ScreenShareClient which will
    /// take the processed images and send it to the server via networking module
    /// </summary>
    public partial class ScreenShareClient : INotificationHandler
    {

        // Object of netowking module through which
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
        private CancellationTokenSource? tokenSource;

        // Varible to store if screen share is active
        private bool _isScreenSharing = false;

        // ScreenShareClient Object
        private ScreenShareClient? _screenShareClient;

        /// <summary>
        /// Called by view model
        /// Subscribe the networking by calling `_communicator.Subscribe()`
        /// Creates the objects of `ScreenCapturer` and `ScreenProcessor`
        /// </summary>
        private ScreenShareClient()
        {
            _capturer = new ScreenCapturer();
            _processor = new ScreenProcessor(_capturer);
            _communicator = CommunicationFactory.GetCommunicator(true);
            _communicator.Subscribe("ScreenShare", this, true);
            _serializer = new();
        }

        /// <summary>
        /// Gives an instance of ScreenShareClient class
        /// And that instance is always the same i.e. 
        /// singleton patter
        /// </summary>
        public ScreenShareClient GetInstance()
        {
            if (_screenShareClient == null)
            {
                _screenShareClient = new ScreenShareClient();
            }
            return _screenShareClient;
        }

        /// <summary>
        /// Firstly it will send a REGISTER packet to the server then 
        /// start capturer, processor, image sending function and 
        /// the confirmation sending function
        /// </summary>
        public void StartScreenSharing()
        {
            _isScreenSharing = true;
            // sending register packet
            DataPacket dataPacket = new(ClientDataHeader.Register.ToString(), "");
            string serializedData = _serializer.Serialize(dataPacket);
            _communicator.Send(serializedData, "ScreenShare");

            StartImageSending();
            SendConfirmationPacket();

            _capturer.StartCapture();
            _processor.StartProcessing();
        }

        /// <summary>
        /// This function will be invoked on message from server
        /// If the message is SEND then start screen sharing and set the 
        /// appropriate resolution
        /// else if the message was STOP then stop the screen sharing
        /// </summary>
        /// <param name="serializedData"> serialized data from the network module </param>
        void INotificationHandler.OnDataReceived(string serializedData) 
        {
            DataPacket dataPacket = _serializer.Deserialize<DataPacket>(serializedData);
            if (dataPacket.Header == ServerDataHeader.Send.ToString())
            {
                if (_isScreenSharing == false)
                {
                    StartScreenSharing();
                }
                Tuple<int, int> res = _serializer.Deserialize<Tuple<int, int>>(dataPacket.Data);
                _processor.SetNewResolution(res);
            }
            else
            {
                StopScreenSharing();
            }
        }

        /// <summary>
        /// Image sending function which will take image pixel diffs from processor and 
        /// send it to the server via the networking module
        /// </summary>
        private void ImageSending()
        {
            while (true)
            {
                Frame img = _processor.GetImage();
                string serializedImg = _serializer.Serialize(img);
                DataPacket dataPacket = new(ClientDataHeader.Image.ToString(), serializedImg);
                string serializedData = _serializer.Serialize(dataPacket);
                _communicator.Send(serializedData, "ScreenShare");
            }
        }

        /// <summary>
        /// Starting the image sending function on a thread
        /// </summary>
        private void StartImageSending()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            _sendImageTask = new Task(ImageSending, token);
            _sendImageTask.Start();
        }

        /// <summary>
        /// Resuming screen share
        /// </summary>
        private void ResumeImageSending()
        {
            StartImageSending();
        }
    }
}