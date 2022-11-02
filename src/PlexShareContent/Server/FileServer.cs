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

        public MessageData Receive(MessageData msg)
        {
            Trace.WriteLine("[FileServer] Received message from ContentServer");
            if (msg.Event == MessageEvent.NewMessage)
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

        public MessageData StoreFile(MessageData msg)
        {
            msg = _db.FileStore(msg).Clone();
            // the object is going to be typecasted to ReceiveMessageData
            // to be sent to clients, so make filedata null because the filedata
            // will continue to be in memory despite the typecasting
            msg.FileData = null;
            return msg;
        }

        public MessageData FileDownload(MessageData msg)
        {
            var receivedMsg = _db.FilesFetch(msg.MessageId);

            // If null is returned by contentDatabase that means it doesn't exist, return null
            if (receivedMsg == null)
            {
                Trace.WriteLine($"[FileServer] File not found messageId: {msg.MessageId}.");
                return null;
            }

            // Clone the object and add the required fields
            var downloadMsg = receivedMsg.Clone();

            // store file path on which the file will be downloaded on the client's system
            downloadMsg.Message = msg.Message;
            downloadMsg.Event = MessageEvent.Download;
            downloadMsg.SenderId = msg.SenderId;
            return downloadMsg;
        }
    }
}
