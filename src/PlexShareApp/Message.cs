/******************************************************************************
 * Filename    = Message.cs
 *
 * Author      = Sughandhan S
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareApp
 *
 * Description = The following is the Message Datatype to store the message details.
 *****************************************************************************/

using System;
using System.Text;

namespace PlexShareApp
{
    public class Message
    {
        /// <summary>
        ///     Message datatype will contain fields such as MessageId, SenderName
        ///     the type of message, time of sending, the reply message and to whom 
        ///     it is being sent
        /// </summary>
        
        public Message()
        {
            MessageID = -1;
            Sender = null;
            Time = DateTime.Now.ToShortTimeString();
            Type = true;
            ReplyMessage = null;
            IncomingMessage = null;
            ToFrom = false;
        }

        /// <summary>
        ///     Message ID of the message
        /// </summary>
        public int MessageID
        {
            get; set;
        }

        /// <summary>
        ///     Name of the message sender
        /// </summary>
        public string? Sender
        {
            get; set;
        }

        /// <summary>
        ///     Time of sending the message
        /// </summary>
        public string Time
        {
            get; set;
        }

        /// <summary>
        ///     Type True represents chat message
        ///     Type False represents file message
        /// </summary>
        public bool Type
        {
            get; set;
        }

        /// <summary>
        ///     Incoming message stores the chat message string
        ///     or the file name of the file message.
        /// </summary>
        public string? IncomingMessage
        {
            get; set;
        }

        /// <summary>
        ///     Message to which the current message is being replied to
        ///     NULL value suggests it isn't a reply message
        /// </summary>
        public string? ReplyMessage
        {
            get; set;
        }

        /// <summary>
        ///     set True if sent by current user or else set False
        /// </summary>
        public bool ToFrom
        {
            get; set;
        }


    }
}
