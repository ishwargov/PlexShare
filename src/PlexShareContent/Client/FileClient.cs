/******************************************************************************
 * Filename    = FileClient.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Class handling the sending of data to server based on 
 *               the type of file events - New (Upload) and Download.
 *****************************************************************************/

using PlexShareContent.DataModels;
using System.Diagnostics;
using System;
using System.IO;
using PlexShareContent.Enums;
using PlexShareNetwork.Communication;

namespace PlexShareContent.Client
{
    public class FileClient
    {
        /// <summary>
        /// Module identifier for communicator
        /// </summary>
        private readonly string _moduleIdentifier = "Content";
        private readonly IContentSerializer _serializer;
        private ICommunicator _communicator;

        /// <summary>
        /// Constructor that instantiates a communicator and serializer.
        /// </summary>
        /// <param name="communicator">Object implementing the ICommunicator interface</param>
        public FileClient(ICommunicator communicator)
        {
            _communicator = communicator;
            _serializer = new ContentSerializer();
        }

        /// <summary>
        /// Communicator setter function
        /// </summary>
        public ICommunicator Communicator
        {
            set => _communicator = value;
        }


        /// <summary>
        /// Auto-implemented UserID property.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Serializes the ContentData object and send it to the server via networking module. 
        /// </summary>
        /// <param name="contentData">Instance of SendContentData class</param>
        /// <param name="eventType">Type of message event as string</param>
        private void SerializeAndSendToServer(ContentData contentData, string eventType)
        {
            try
            {
                var xml = _serializer.Serialize(contentData);
                Trace.WriteLine($"[File Client] Setting event as '{eventType}' and sending object to server.");
                _communicator.Send(xml, _moduleIdentifier, null);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[File Client] Exception occurred while sending object.\n{e.GetType().Name} : {e.Message}");
            }
        }

        /// <summary>
        /// Converts the input SendContentData object, sets the event type as New, serializes and send it to server.
        /// </summary>
        /// <param name="sendContent">Instance of the SendContentData class</param>
        /// <exception cref="ArgumentException"></exception>
        public void SendFile(SendContentData sendContent)
        {
            // check message type
            if(sendContent.Type != MessageType.File)
            {
                throw new ArgumentException("Message is not of type - 'File'");
            }
            // check if file exists
            if(!File.Exists(sendContent.Data))
            {
                throw new FileNotFoundException($"File at {sendContent.Data} not found");
            }
            ContentData sendData = new();
            sendData.Type = sendContent.Type;
            sendData.Data = sendContent.Data;
            sendData.MessageID = -1;
            sendData.ReceiverIDs = sendContent.ReceiverIDs;
            sendData.ReplyThreadID = -1;
            sendData.SenderID = UserID;
            sendData.SentTime = DateTime.Now;
            sendData.Starred = false;
            sendData.Event = MessageEvent.New;
            sendData.FileData = new SendFileData(sendContent.Data);
            //sendData.ReplyMessageID = sendContent.ReplyMessageID;
            SerializeAndSendToServer(sendData, "New");
        }

        /// <summary>
        /// Creates ContentData object, sets the event type as Download, serializes and sends it to server.
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="savePath">Path to which the file will be downloaded</param>
        public void DownloadFile(int messageID, string savePath)
        {
            if(savePath == null || savePath == "")
            {
                throw new ArgumentException("Invalid save path input argument"); 
            }
            ContentData sendData = new();
            sendData.Type = MessageType.File;
            sendData.Data = savePath;
            sendData.MessageID = messageID;
            sendData.SenderID = UserID;
            sendData.Event = MessageEvent.Download;
            sendData.FileData = null;
            SerializeAndSendToServer(sendData, "Download");
        }
    }
}