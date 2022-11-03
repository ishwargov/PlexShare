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
        private readonly ContentDB _db;

        public FileServer(ContentDB contentDB)
        {
            _db = contentDB;
        }

        public ContentData Receive(ContentData msg)
        {
            Trace.WriteLine("[FileServer] Received message from ContentServer");
            if (msg.Event == MessageEvent.New)
            {
                Trace.WriteLine("[FileServer] Event is NewMessage, Saving File");
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

        public ContentData StoreFile(ContentData msg)
        {
            msg = _db.FileStore(msg).Copy();
            // the object is going to be typecasted to ReceiveMessageData
            // to be sent to clients, so make filedata null because the filedata
            // will continue to be in memory despite the typecasting
            msg.FileData = null;
            return msg;
        }

        public ContentData FileDownload(ContentData msg)
        {
            var receivedMsg = _db.FilesFetch(msg.MessageID);

            // If null is returned by contentDatabase that means it doesn't exist, return null
            if (receivedMsg == null)
            {
                Trace.WriteLine($"[FileServer] File not found messageId: {msg.MessageID}.");
                return null;
            }

            // Clone the object and add the required fields
            var downloadMsg = receivedMsg.Copy();

            // store file path on which the file will be downloaded on the client's system
            downloadMsg.Data = msg.Data;
            downloadMsg.Event = MessageEvent.Download;
            downloadMsg.SenderID = msg.SenderID;
            return downloadMsg;
        }
    }
}
