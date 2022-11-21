/// <author>Morem Jayanth Kumar</author>
/// <created>3/11/2022</created>
/// <summary>
///		This file implements the ISummarizer interface and calls all the main functions
/// </summary>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Dashboard.Server.Persistence;
using PlexShareContent.DataModels;
using PlexShareContent;

namespace PlexShareDashboard.Dashboard.Server.Summary
{
    internal class Summarizer : ISummarizer
    {
        private readonly ISummaryPersistence _persister;

        private readonly ChatProcessor _processor;

        /// <summary>
        ///     The Constructor for the summarizer
        ///     for the summary logic module to get
        ///     the summary for the chats and save
        ///     this summary in the database
        /// </summary>
        public Summarizer()
        {
            _processor = new ChatProcessor();   
            _persister = PersistenceFactory.GetSummaryPersistenceInstance();
        }

        /// <summary>
        ///     Function to get the Summary of the chat and
        ///     discussion to present in the Dashboard
        /// </summary>
        /// <param name="chats">
        ///     Array of ChatContext each of
        ///     which would contain an array of chat messages
        ///     which would be used for the summarizer to
        ///     generate the summary
        /// </param>
        /// <returns>
        ///     String which is the summary of the
        ///     chat in the particular discusiion
        /// </returns>
        public string GetSummary(ChatThread[] chats)
        {
            if (chats == null || chats.Length == 0)
            {
                Trace.WriteLine("Empty chat context obtained.");
                return "";
            }
            List<string> discussionChat = new();
            foreach (var chat in chats)
                foreach (var msg in chat.MessageList)
                    if (msg.Type==MessageType.Chat)
                        discussionChat.Add(msg.Data);
            return _processor.Summarize(discussionChat);
        }

        /// <summary>
        ///     Function to save the summary of the dashboard
        ///     discussion that took place into the database
        ///     using the persistence module.
        /// </summary>
        /// <param name="chats">
        ///     Array of ChatContext each of
        ///     which would contain an array of chat messages
        ///     which would be used for the summarizer to
        ///     save the summary in the database
        /// </param>
        /// <returns>
        ///     Returns true if summary was succesfully stored
        ///     and false otherwise
        /// </returns>
        public bool SaveSummary(ChatThread[] chats)
        {
            return _persister.SaveSummary(GetSummary(chats));
        }
    }
}
