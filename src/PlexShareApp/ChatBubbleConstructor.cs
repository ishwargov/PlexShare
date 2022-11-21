/******************************************************************************
 * Filename    = ChatBubbleConstructor.cs
 *
 * Author      = Sughandhan S
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareApp
 *
 * Description = The following file helps in constructing the chat bubble using Data Templates based on whether the message
 *               is a chat or file
 *****************************************************************************/

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PlexShareApp
{
    public class ChatBubbleConstructor: DataTemplateSelector
    {
        
        /// <summary>
        ///     Creates the chat bubble
        /// </summary>
        /// <param name="item">template item</param>
        /// <param name="container">dependency object</param>
        /// <returns></returns>
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            var message = item as Message;
            Trace.WriteLine("[ChatBubbleConstructor] Chat bubble created.");
            if (message.ToFrom)
            {
                return message.Type ? SentMsgBubble : SentFileBubble;
            }
            return message.Type ? RecvMsgBubble : RecvFileBubble;
        }

        /// <summary>
        /// Data Template for the sent chat message
        /// </summary>
        public DataTemplate SentMsgBubble { get; set; }

        /// <summary>
        /// Data Template for the received chat message
        /// </summary>
        public DataTemplate RecvMsgBubble { get; set; }

        /// <summary>
        /// Data Template for the sent file message
        /// </summary>
        public DataTemplate SentFileBubble { get; set; }

        /// <summary>
        /// Data Template for the received file message
        /// </summary>
        public DataTemplate RecvFileBubble { get; set; }
    }
}
