using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Dashboard.Server.Persistence;
using PlexShareDashboard.Dashboard.Server.Telemetry;

namespace PlexShareDashboard.Dashboard.Server.Summary
{
    internal class Summarizer : ISummarizer
    {
        public enum MessageType
        {
            File,
            Chat,
            HistoryRequest
        }

        private readonly ISummaryPersistence _persister;

        private readonly ChatProcessor _processor;
        public Summarizer()
        {
            _processor = new ChatProcessor();   
            _persister = PersistenceFactory.GetSummaryPersistenceInstance();
        }
        public string GetSummary(ChatThread[] chats)
        {
            if (chats == null || chats.Length == 0)
            {
                Trace.WriteLine("Empty chat context obtained.");
                return "";
            }
            List<(string, bool)> discussionChat = new();
            foreach (var chat in chats)
                foreach (var msg in chat.MessageList)
                    //if (msg.GetType() == MessageType.Chat)
                    discussionChat.Add((msg.Data, msg.Starred));
            return _processor.Summarize(discussionChat);
        }
        public bool SaveSummary(ChatThread[] chats)
        {
            return _persister.SaveSummary(GetSummary(chats));
        }
    }
}
