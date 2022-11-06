///<author>Satyam Mishra</author>
///<summary>
/// This file has ScreenshareClient class's partial implementation
/// In this file functions realted to stopping of ScreenCapturing 
/// are implemented
///</summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareScreenshare;
using PlexShareNetwork;
using PlexShareNetwork.Serialization;
using System.Threading;

namespace PlexShareScreenshare.Client
{
    public partial class ScreenshareClient : INotificationHandler
    {

        /// <summary>
        /// Method to stop screensharing. Calling this will stop sending
        /// both the image sending task and confirmation sending task.
        /// It will also call stop on the processor and capturer.
        /// </summary>
        public void StopScreensharing()
        {
            DataPacket deregisterPacket = new DataPacket(_id, _name, ClientDataHeader.Deregister.ToString(), "");
            var serializedDeregisterPacket = _serializer.Serialize(deregisterPacket);
            StopImageSending();
            _confirmationCancellationTokenSource?.Cancel();
            _sendConfirmationTask?.Wait();
            _processor.StopProcessing();
            _capturer.StopCapture();
            _communicator.Send(serializedDeregisterPacket, "ScreenShare");
        }

        /// <summary>
        /// Method to stop image sending. Implemented separately
        /// in case this is needed to be used somewhere else.
        /// </summary>
        private void StopImageSending()
        {
            _imageCancellationTokenSource?.Cancel();
            _sendImageTask?.Wait();
        }

        /// <summary>
        /// Sends confirmation packet to server once every second. The confirmation packet
        /// does not contain any data
        /// </summary>
        private void SendConfirmationPacket()
        {
            _confirmationCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancelConfirmationToken = _confirmationCancellationTokenSource.Token;
            DataPacket confirmationPacket = new DataPacket(_id, _name, ClientDataHeader.Confirmation.ToString(), "");
            var serializedConfirmationPacket = _serializer.Serialize(confirmationPacket);

            _sendConfirmationTask = new Task(() =>
            {
                while (!_confirmationCancellationTokenSource.IsCancellationRequested)
                {
                    _communicator.Send(serializedConfirmationPacket, "ScreenShare");
                    Thread.Sleep(1000);
                }
            }, cancelConfirmationToken);

            _sendConfirmationTask.Start();
        }

        /// <summary>
        /// Used by dashboard module to set the id and name for the client
        /// </summary>
        /// <param name="id">ID of the client</param>
        /// <param name="name">Name of the client</param>
        public void SetUser(string id, string name)
        {
            _id = id;
            _name = name;
        }
    }
}
