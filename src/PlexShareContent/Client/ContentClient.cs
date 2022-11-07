/******************************************************************************
 * Filename    = ContentClient.cs
 *
 * Author      = Narvik Nandan
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = Client-side content manager class which provides interfaces 
 *               to other modules and deals with various user functions.   
 *****************************************************************************/

using Networking;
using Networking.Serialization;
using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlexShareContent.Client
{
    public class ContentClient : IContentClient
    {
        private INotificationHandler _notificationHandler;
        private ICommunicator _communicator;
        private IContentSerializer _serializer;

        private readonly ChatClient _chatHandler;
        private readonly FileClient _fileHandler;

        private List<IContentListener> _subscribers;
        private Dictionary<int, int> _messageIDMap;
        private Dictionary<int, int> _contextMap;
        private readonly Dictionary<MessageEvent, Action<ContentData>> _messageHandler;

        private int _userID;
        private readonly object _lock;

        public ContentClient()
        {
                
        }


        public void ClientSendData(SendContentData contentData)
        {
            switch (contentData.Type)
            {
                case MessageType.Chat:
                    // send message to chat client
                    break;
                case MessageType.File:
                    // send file to file client
                    break;
                default:
                    // throw exception
                    throw new ArgumentException("Invalid Message Field Type");
            }
        }

        public void ClientSubscribe(IContentListener subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("Null subscriber");
            }
            else
            {
                // add subscriber to subsribers' list
            }
        }

        void IContentClient.ClientEdit(int messageID, string newMessage)
        {
            throw new NotImplementedException();
        }

        void IContentClient.ClientDownload(int messageID, string savePath)
        {
            throw new NotImplementedException();
        }

        void IContentClient.ClientStar(int messageID)
        {
            throw new NotImplementedException();
        }

        ChatThread IContentClient.ClientGetThread(int threadID)
        {
            throw new NotImplementedException();
        }

        int IContentClient.GetUserID()
        {
            throw new NotImplementedException();
        }
    }
}
