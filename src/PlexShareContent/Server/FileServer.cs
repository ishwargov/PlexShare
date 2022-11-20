/******************************************************************************
 * Filename    = FileServer.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description = This file handles the file messges and various functionlaities associted with file.
 *****************************************************************************/

using PlexShareContent.DataModels;
using PlexShareContent.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareContent.Server
{
    public class FileServer
    {
        private ContentDB _db;

        /// <summary>
        ///     Constructor to initializes the content Database.
        /// </summary>
        public FileServer(ContentDB contentDB)
        {
            _db = contentDB;
        }

        /// <summary>
        ///     This function is used to preocess the file based on the type of event occured.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Returns the new message</returns>
        public ContentData Receive(ContentData msg)
        {
            Trace.WriteLine("[FileServer] Received message from ContentServer");
            if (msg.Event == MessageEvent.New)
            {
                Trace.WriteLine("[FileServer] Event is New, Saving File");
                return StoreFile(msg);
            }
            else if (msg.Event == MessageEvent.Download)
            {
                Trace.WriteLine("[FileServer] Event is Download, Proceeding to download");
                return FileDownload(msg);
            }
            else
            {
                Trace.WriteLine($"[ChatServer] invalid event");
                return null;
            }
        }

        /// <summary>
        ///     This function is used to save file on Database.
        /// </summary>
        public ContentData StoreFile(ContentData msg)
        {
            msg = _db.FileStore(msg).Copy();
            // the object is going to be typecasted to ReceiveMessageData
            // to be sent to clients, so make filedata null because the filedata
            // will continue to be in memory despite the typecasting
            msg.FileData = null;
            return msg;
        }

        /// <summary>
        ///     This function is used to download the file on download event.
        /// </summary>
        public ContentData FileDownload(ContentData msg)
        {
            var receivedMsg = _db.FilesFetch(msg.MessageID);
            // Doesn't exist on database, return null
            if (receivedMsg == null)
            {
                Trace.WriteLine($"[FileServer] File not found messageId: {msg.MessageID}.");
                return null;
            }
            // Clone the object and add the required fields
            var downloadedFile = receivedMsg.Copy();
            // store file path on which the file will be downloaded on the client's system
            downloadedFile.Data = msg.Data;
            downloadedFile.Event = MessageEvent.Download;
            downloadedFile.SenderID = msg.SenderID;
            return downloadedFile;
        }
    }
}
