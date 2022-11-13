using PlexShareNetwork;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
///<author>Rudr Tiwari</author>
///<summary>
/// This file has ScreenshareClient class's partial implementation
/// In this file functions realted to stopping of ScreenCapturing 
/// are implemented
///</summary>
using System.Threading.Tasks;

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
            Debug.Assert(_id != null, Utils.GetDebugMessage("_id property found null", withTimeStamp: true));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null", withTimeStamp: true));
            DataPacket deregisterPacket = new(_id, _name, ClientDataHeader.Deregister.ToString(), "");
            var serializedDeregisterPacket = JsonSerializer.Serialize(deregisterPacket);
            StopImageSending();
            // Stops sending confirmation token
            _confirmationCancellationToken = true;
            _sendConfirmationTask?.Wait();
            // Stops the processing and the capturing module
            _processor.StopProcessing();
            _capturer.StopCapture();
            // Sending de-rgister request to server
            _communicator.Send(serializedDeregisterPacket, Utils.ModuleIdentifier, null);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully sent DEREGISTER packet to server", withTimeStamp: true));
        }

        /// <summary>
        /// Method to stop image sending. Implemented separately
        /// in case this is needed to be used somewhere else.
        /// </summary>
        private void StopImageSending()
        {
            _imageCancellationToken = true;
            _sendImageTask?.Wait();
        }

        /// <summary>
        /// Sends confirmation packet to server once every second. The confirmation packet
        /// does not contain any data
        /// </summary>
        private void SendConfirmationPacket()
        {
            _confirmationCancellationToken = false;
            Debug.Assert(_id != null, Utils.GetDebugMessage("_id property found null", withTimeStamp: true));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null", withTimeStamp: true));
            DataPacket confirmationPacket = new(_id, _name, ClientDataHeader.Confirmation.ToString(), "");
            var serializedConfirmationPacket = JsonSerializer.Serialize(confirmationPacket);

            _sendConfirmationTask = new Task(() =>
            {
                while (!_confirmationCancellationToken)
                {
                    _communicator.Send(serializedConfirmationPacket, Utils.ModuleIdentifier, null);
                    Thread.Sleep(1000);
                }
            });

            Trace.WriteLine(Utils.GetDebugMessage("Starting Confirmation packet sending", withTimeStamp: true));
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
            Trace.WriteLine(Utils.GetDebugMessage("Successfully set client name and id", withTimeStamp: true));
        }
    }
}
