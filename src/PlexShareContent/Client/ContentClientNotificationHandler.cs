/******************************************************************************
 * Filename    = ContentClientNotificationHandler.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Notification handler that handles received data from server   
 *****************************************************************************/

using PlexShareContent.DataModels;
using PlexShareNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlexShareContent.Client
{
    public class ContentClientNotificationHandler : INotificationHandler
    {
        private readonly IContentSerializer _serialzer;
        private readonly ContentClient _contentHandler;
        protected ContentData _receivedMessage;
        protected List<ChatThread> _allMessages;

        /// <summary>
        /// Constructor to create type with parameters.
        /// </summary>
        /// <param name="contentHandler">Object that implements IContentClient interface</param>
        public ContentClientNotificationHandler(IContentClient contentHandler)
        {
            _serialzer = new ContentSerializer();
            _contentHandler = contentHandler as ContentClient;
        }

        /// <summary>
        /// Checks the type of messae object and calls the required function 
        /// </summary>
        /// <param name="data">String data from network</param>
        public void OnDataReceived(string data)
        {
            Trace.WriteLine("[ContentClientNotificationHandler] Deserializing data received from network");
            try
            {
                // get type of serialized data
                var deserializedType = _serialzer.GetObjectType(data, "Content");
                // if data is of ContentData type
                if(string.Equals(deserializedType, typeof(ContentData).ToString()))
                {
                    _receivedMessage = _serialzer.Deserialize<ContentData>(data);
                    _contentHandler.OnReceive(_receivedMessage);
                    Trace.WriteLine($"[ContentClientNotificationHandler] Deserialized data and sending it to content client");
                }
                // if data is a List<ChatThread>
                else if(string.Equals(deserializedType, typeof(List<ChatThread>).ToString()))
                {
                    _allMessages = _serialzer.Deserialize<List<ChatThread>>(data);
                    _contentHandler.OnReceive(_allMessages);
                    Trace.WriteLine($"[ContentClientNotificationHandler] Deserialized data and sending it to content client");
                }
                else
                {
                    throw new ArgumentException($"Deserialized object of unknown type : {deserializedType}");
                }
            }
            catch(ArgumentException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentClientNotificationHandler] Error during deserialization of received data.\n{e.GetType().Name} : {e.Message}");
            }
        }
        
    }
}
