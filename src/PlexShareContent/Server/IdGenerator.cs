/******************************************************************************
 * Filename    = IdGenerator.cs
 *
 * Author      = Anurag Jha
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareContent
 *
 * Description =  This Class generates unique Ids for message and chat thread and also allows restting them.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareContent.Server
{
    public class IdGenerator
    {
        private static int _msgId;
        private static int _chatContentId;

        /// <summary>
        ///     Generates message id for the message and increment the id.
        /// </summary>
        /// <returns>Returns the generated unique id.</returns>
        public static int GetMsgId()
        {
            var id = _msgId;
            _msgId++;
            return id;
        }

        /// <summary>
        ///     Reset the message id to 0.
        /// </summary>
        /// <returns>Returns the generated unique id.</returns>
        public static void ResetMsgId()
        {
            _msgId = 0;
        }

        /// <summary>
        ///     Generates ids for chat threads.
        /// </summary>
        /// <returns>Returns the generated id.</returns>
        public static int GetChatId()
        {
            var id = _chatContentId;
            _chatContentId++;
            return id;
        }

        /// <summary>
        ///     Reset the chat thread id.
        /// </summary>
        public static void ResetChatId()
        {
            _chatContentId = 0;
        }
    }
}
